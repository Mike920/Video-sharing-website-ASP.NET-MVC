using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc.Async;
using ASP_Video_Website.Models;
using ASP_Video_Website.Utility;

namespace ASP_Video_Website.Services
{
    public class MediaService
    {
        static public  void ConvertVideo(string mediaName,int mediaId)
        {
            var db = new ApplicationDbContext();
            var dbRecord = db.MediaFiles.Find(mediaId);

            var baseDir = HostingEnvironment.MapPath("~/MediaData/Videos/" + mediaId);
            var mediaDir = Path.Combine(baseDir, mediaName);

            var mediaInfo = GetMediaInfo(mediaDir);
            //todo error when mediinfo doesnt have video
            var logFilePath = HostingEnvironment.MapPath("~/MediaData/log.txt");
            
            StreamWriter logFile = new StreamWriter(logFilePath, true);
            logFile.WriteLine("//////////////////////////////////////////// START CONVERTING MEDIA "+mediaId+" ///////////////////////////////////////////////////");

            if (!mediaInfo.Video.HasVideo)
                //todo: implement this in the controller
                throw new Exception("Video file is not supported");
                

            VideoQuality videoQuality = ServerParams.VideoParams.ClassifyVideo(mediaInfo);
            MediaInfo videoParams = ServerParams.VideoParams.GetVideoParams(videoQuality);

            dbRecord.VideoQuality = videoQuality;

            var outputVidSd = Path.Combine(baseDir, "sd.mp4");
            var outputVidHd = Path.Combine(baseDir, "hd.mp4");
            var outputAudio = Path.Combine(baseDir, "audio.mp4");
            var outputMobile = Path.Combine(baseDir, "mobile.mp4");
            var outputMobileHd = Path.Combine(baseDir, "mobileHd.mp4");
            var outputThumbnail = Path.Combine(baseDir, "thumbnail.jpg");
            var outputPasslogFile = Path.Combine(baseDir, "passlog");

            var segmentsDir = Path.Combine(baseDir, "segments");

            if (!Directory.Exists(segmentsDir))
                Directory.CreateDirectory(segmentsDir);

            var hcontext = HttpContext.Current;

            var task = Task.Factory.StartNew(() =>
            {
                HttpContext.Current = hcontext;
                FFMPEG ffmpeg = new FFMPEG();

                //What part of total progress is the current conversion
                Dictionary<string, double> ConversionPercentages = new Dictionary<string, double>();
               
                if (videoQuality == VideoQuality.p360)
                {
                    ConversionPercentages["sd"] = 0.9;
                    ConversionPercentages["audio"] = 0.1;
                }
                else
                {
                    ConversionPercentages["sd"] = 0.2;
                    ConversionPercentages["hd"] = 0.7;
                    ConversionPercentages["audio"] = 0.1;
                }
                
                // Convert to SD
                string command = String.Format("-i \"{0}\" -an -b:v {1}k -s {2} -vcodec libx264 -r 24  -g 48 -keyint_min 48 -sc_threshold 0 -pass 1 -passlogfile \"{3}\" \"{4}\"",
                            mediaDir, ServerParams.VideoParams.p360.Video.Bitrate, ServerParams.VideoParams.p360.Video.Resolution, outputPasslogFile, outputVidSd);
                var result = ffmpeg.RunCommand(command,mediaId,ConversionPercentages,"sd",mediaInfo.Video.Duration);
                logFile.WriteLine("//////////////////////// SD Conversion:");
                logFile.Write("COMMAND:  "+command);
                logFile.WriteLine(result);

                //Convert to HD
                if (videoQuality != VideoQuality.p360)
                {
                    command = String.Format("-i \"{0}\" -an -b:v {1}k -s {2} -vcodec libx264 -r 24  -g 48 -keyint_min 48 -sc_threshold 0 -pass 1 -passlogfile \"{3}\" \"{4}\"",
                        mediaDir, videoParams.Video.Bitrate, videoParams.Video.Resolution, outputPasslogFile, outputVidHd);
                    result = ffmpeg.RunCommand(command, mediaId, ConversionPercentages, "hd", mediaInfo.Video.Duration);
                    logFile.WriteLine("//////////////////////// HD Conversion:");
                    logFile.Write("COMMAND:  " + command);
                    logFile.WriteLine(result);
                }

                //Convert Audio
                command = String.Format("-i \"{0}\" -vn -strict experimental -c:a aac -b:a 128k \"{1}\"",
                    mediaDir, outputAudio);
                result = ffmpeg.RunCommand(command, mediaId, ConversionPercentages,"audio", mediaInfo.Video.Duration);
                logFile.WriteLine("//////////////////////// Audio Conversion:");
                logFile.Write("COMMAND:  " + command);
                logFile.WriteLine(result);

                //Extract thumbnail from the middle of the video
                command = String.Format(" -ss {0} -i \"{1}\"  -vframes 1 -an -s 360x240  \"{2}\" ", (mediaInfo.Video.Duration / 2.0).ToString(CultureInfo.InvariantCulture),
                    mediaDir, outputThumbnail);
                result = ffmpeg.RunCommand(command);
                logFile.WriteLine("//////////////////////// Thumbnail Conversion:");
                logFile.Write("COMMAND:  " + command);
                logFile.Write("vid duration:  " + mediaInfo.Video.Duration);
                logFile.Write("extract frame from time:  " + (mediaInfo.Video.Duration / 2.0).ToString(CultureInfo.InvariantCulture));
                logFile.WriteLine(result);

                //Convert to mobile (add sound to sd video)
                command = String.Format("-i \"{0}\" -i \"{1}\" -c:v copy -c:a copy \"{2}\"",
                        outputVidSd,outputAudio,outputMobile);
                result = ffmpeg.RunCommand(command);
                logFile.WriteLine("//////////////////////// Mobile Conversion:");
                logFile.Write("COMMAND:  " + command);
                logFile.WriteLine(result);

                //Convert to mobile Hd
                if (videoQuality != VideoQuality.p360)
                {
                    command = String.Format("-i \"{0}\" -i \"{1}\" -c:v copy -c:a copy \"{2}\"",
                        outputVidHd, outputAudio, outputMobileHd);
                    result = ffmpeg.RunCommand(command);
                    logFile.WriteLine("//////////////////////// Mobile HD Conversion:");
                    logFile.Write("COMMAND:  " + command);
                    logFile.WriteLine(result);
                }

                //Segment videos and audio 
                Mp4Box mp4Box = new Mp4Box();
                if (videoQuality == VideoQuality.p360)
                    command = String.Format("-dash 2000 -frag 2000 -bs-switching no -segment-name \"%s_\" -url-template -out \"{0}\" \"{1}\"  \"{2}\" ", Path.Combine(segmentsDir, "video.mpd"), outputVidSd, outputAudio);
                else
                    command = String.Format("-dash 2000 -frag 2000 -bs-switching no -segment-name \"%s_\" -url-template -out \"{0}\" \"{1}\" \"{2}\" \"{3}\" ", Path.Combine(segmentsDir, "video.mpd"), outputVidSd, outputVidHd, outputAudio);

                result = mp4Box.RunCommand(command);
                logFile.WriteLine("//////////////////////// Segmenting:");
                logFile.Write("COMMAND:  " + command);
                logFile.WriteLine(result);
                logFile.Close();

                File.Delete(mediaDir);
                File.Delete(outputVidSd);
                if(File.Exists(outputVidHd)) File.Delete(outputVidHd);
                File.Delete(outputAudio);

                //todo: add entry to db
            }, TaskCreationOptions.LongRunning);

            dbRecord.IsBeingConverted = false;
            db.Entry(dbRecord).State = EntityState.Modified;
            db.SaveChanges();
        }

     

        static public MediaInfo GetMediaInfo(string path)
        {
            FFMPEG ffmpeg = new FFMPEG(FFMPEG.ProgramToRun.FFPROBE);
            return ffmpeg.GetMediaInfo(path);
        }
    }
}