using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using ASP_Video_Website.Utility;

namespace ASP_Video_Website.Services
{
    public class MediaService
    {
        static public void ConvertVideo(string mediaName,int mediaId)
        {
            var baseDir = HostingEnvironment.MapPath("~/App_Data/Videos/" + mediaId);
            var mediaDir = Path.Combine(baseDir, mediaName);

            var mediaInfo = GetMediaInfo(mediaDir);


            if (!mediaInfo.Video.HasVideo)
                //todo: implement this in the controller
                throw new Exception("Video file is not supported");
                

            VideoQuality videoQuality = ServerParams.VideoParams.ClassifyVideo(mediaInfo);
            MediaInfo videoParams = ServerParams.VideoParams.GetVideoParams(videoQuality);

            var outputVidSd = Path.Combine(baseDir, "sd.mp4");
            var outputVidHd = Path.Combine(baseDir, "hd.mp4");
            var outputAudio = Path.Combine(baseDir, "audio.mp4");
            var outputMobile = Path.Combine(baseDir, "mobile.mp4");
            var outputThumbnail = Path.Combine(baseDir, "thumbnail.jpg");

            var segmentsDir = Path.Combine(baseDir, "segments");

            if (!Directory.Exists(segmentsDir))
                Directory.CreateDirectory(segmentsDir);

            var task = Task.Factory.StartNew(() =>
            {
                FFMPEG ffmpeg = new FFMPEG();

                // Convert to SD
                string command = String.Format("-i \"{0}\" -an -b:v {1}k -s {2} -vcodec libx264 \"{3}\"",
                            mediaDir, ServerParams.VideoParams.p360.Video.Bitrate, ServerParams.VideoParams.p360.Video.Resolution, outputVidSd);
                var result = ffmpeg.RunCommand(command);

                //Convert to HD
                if (videoQuality != VideoQuality.p360)
                {
                    command = String.Format("-i \"{0}\" -an -b:v {1}k -s {2} -vcodec libx264 \"{3}\"",
                        mediaDir, videoParams.Video.Bitrate, videoParams.Video.Resolution, outputVidHd);
                    result = ffmpeg.RunCommand(command);
                }

                //Convert Audio
                command = String.Format("-i \"{0}\" -vn -strict experimental -c:a aac -b:a 192k \"{1}\"",
                    mediaDir, outputAudio);
                result = ffmpeg.RunCommand(command);

                //Extract thumbnail from the middle of the video
                command = String.Format(" -ss {0} -i \"{1}\"  -t 1 -s 360x240 -f image2 \"{2}\" ", (mediaInfo.Video.Duration/2).ToString(CultureInfo.InvariantCulture),
                    mediaDir, outputThumbnail);
                result = ffmpeg.RunCommand(command);

                //Convert to mobile (add sound to sd video)
                command = String.Format("-i \"{0}\" -i \"{1}\" -c:v copy -c:a copy \"{2}\"",
                        outputVidSd,outputAudio,outputMobile);
                result = ffmpeg.RunCommand(command);

                //Segment videos and audio 
                Mp4Box mp4Box = new Mp4Box();
                command = String.Format("-dash 1000 -bs-switching no -segment-name \"%s_\" -url-template -out \"{0}\" \"{1}\" \"{2}\" \"{3}\" ", Path.Combine(segmentsDir, "video.mpd"),outputVidSd,outputVidHd,outputAudio);
                result = mp4Box.RunCommand(command);

                File.Delete(mediaDir);
                File.Delete(outputVidSd);
                File.Delete(outputVidHd);
                File.Delete(outputAudio);

                //todo: add entry to db
            }, TaskCreationOptions.LongRunning);
            

        }

     

        static public MediaInfo GetMediaInfo(string path)
        {
            FFMPEG ffmpeg = new FFMPEG(FFMPEG.ProgramToRun.FFPROBE);
            return ffmpeg.GetMediaInfo(path);
        }
    }
}