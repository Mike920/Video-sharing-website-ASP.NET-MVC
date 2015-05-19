using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Helpers;
using System.Web.Hosting;
using Newtonsoft.Json;

namespace ASP_Video_Website.Utility
{
    public class FFMPEG
    {
        public enum ProgramToRun
        {
            FFMPEG,
            FFPROBE
        }

        #region Properties

        private ProgramToRun programToRun;
        private string _ffExe;
        

        public string ffExe
        {
            get
            {
                return _ffExe;
            }
            set
            {
                _ffExe = value;
            }
        }
        #endregion

        #region Constructors
        public FFMPEG(ProgramToRun programToRun = ProgramToRun.FFMPEG)
        {
            this.programToRun = programToRun;
            Initialize();
        }
        public FFMPEG(string ffmpegExePath)
        {
           

            _ffExe = ffmpegExePath;
            Initialize();
        }
        #endregion

        #region Initialization
        private void Initialize()
        {
            //first make sure we have a value for the ffexe file setting
            if (string.IsNullOrEmpty(_ffExe))
            {
                object o = ConfigurationManager.AppSettings["ffmpeg:ExeLocation"];
                o = HostingEnvironment.MapPath(o.ToString());
                if (o == null)
                {
                    throw new Exception("Could not find the location of the ffmpeg exe file.  The path for ffmpeg.exe " +
                    "can be passed in via a constructor of the ffmpeg class (this class) or by setting in the app.config or web.config file.  " +
                    "in the appsettings section, the correct property name is: ffmpeg:ExeLocation");
                }
                else
                {
                    if (string.IsNullOrEmpty(o.ToString()))
                        throw new Exception("No value was found in the app setting for ffmpeg:ExeLocation");

                    if (programToRun == ProgramToRun.FFPROBE)
                        o = o.ToString().Replace("ffmpeg", "ffprobe");

                    _ffExe = o.ToString();
                }
            }

            if (!File.Exists(_ffExe))
                throw new Exception("Could not find a copy of ffmpeg.exe");
        }
        #endregion

        #region Run the process
        public string RunCommand(string Parameters, int? mediaId = null, Dictionary<string, double> ConversionPercentages = null,string currentConv=null, double? duration = null)
        {
            HttpContext httpContext = HttpContext.Current;
            //create a process info
            ProcessStartInfo oInfo = new ProcessStartInfo(this._ffExe, Parameters)
            {
                WorkingDirectory = HostingEnvironment.MapPath("~/MediaData")
            };

            oInfo.UseShellExecute = false;
            oInfo.CreateNoWindow = true;
            oInfo.RedirectStandardOutput = true;
            oInfo.RedirectStandardError = true;

            //Create the output and streamreader to get the output
            string output = ""; StreamReader srOutput = null;

            //try the process
            try
            {
                //run the process
                Process proc = System.Diagnostics.Process.Start(oInfo);

                proc.ErrorDataReceived += (sender, args) =>
                {
                    
                        if (args.Data != null)
                        {
                            Debug.WriteLine("Error: " + args.Data);

                            if (programToRun == ProgramToRun.FFMPEG)
                            {
                                output += args.Data + Environment.NewLine;
                                if (mediaId != null && ConversionPercentages != null && currentConv != null &&  duration != null)
                                {
                                    //SaveProgress(args.Data, (int)mediaId, (int)maxPercentage, (double)duration);
                                    var data = args.Data;
                                    if (data.Contains("time="))
                                    {
                                        var maxPercentage = ConversionPercentages[currentConv];
                                        string stime = data.Substring(data.IndexOf("time="));
                                        stime = stime.Replace("time=", "");
                                        stime = stime.Split('.')[0];

                                        var seconds = TimeSpan.Parse(stime).TotalSeconds;
                                        
                                        double perc = Math.Round(seconds,5) / Math.Round((double)duration,5);
                                        perc = Math.Round (perc*(double)maxPercentage,5);
                                        if (double.IsInfinity(perc))
                                            perc = 0.0;
                                       
                                            if (currentConv == "sd")
                                            {
                                                httpContext.Cache[mediaId.ToString()] = perc;
                                            }
                                            if (currentConv == "hd")
                                            {
                                                httpContext.Cache[mediaId.ToString()] = ConversionPercentages["sd"] + perc;
                                            }
                                            if (currentConv == "audio")
                                            {
                                                if(ConversionPercentages.ContainsKey("hd"))
                                                    httpContext.Cache[mediaId.ToString()] = ConversionPercentages["sd"] + ConversionPercentages["hd"] + perc;
                                                else
                                                    httpContext.Cache[mediaId.ToString()] = ConversionPercentages["sd"] + perc;
                                            }
                                            /* httpContext.Cache[mediaId.ToString()] =
                                                (string)httpContext.Cache[mediaId.ToString()] + Environment.NewLine+
                                                perc.ToString();*/
                                          
                                    }
                                }

                            }
                        }
                };

                proc.OutputDataReceived += (sender, args) =>
                    {
                        
                            if (args.Data != null)
                            {
                                Debug.WriteLine("Output: " + args.Data);
                                output += args.Data + Environment.NewLine;
                            }
                    };

                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();

                proc.WaitForExit();
                
                //get the output
                //srOutput = proc.StandardError;

                //now put it in a string
                //output = srOutput.ReadToEnd();

                proc.Close();
            }
            catch (Exception e)
            {
                output = string.Empty;
            }
            finally
            {
                //now, if we succeded, close out the streamreader
                if (srOutput != null)
                {
                    srOutput.Close();
                    srOutput.Dispose();
                }
            }
            return output;
        }
        #endregion

        //todo: test - func may not finish when converting is in progress
        public MediaInfo GetMediaInfo(string path)
        {
            var param = String.Format(" -show_streams -print_format json \"{0}\" ", path);
            var mediaInfo = new MediaInfo();

            try
            {
                var info = RunCommand(param);
                dynamic data = Json.Decode(info);

                foreach (var stream in data.streams)
                {

                    if (stream.codec_type == "audio")
                        mediaInfo.Audio.HasAudio = true;

                    if (stream.codec_type == "video")
                    {
                        mediaInfo.Video.HasVideo = true;
                        mediaInfo.Video.Bitrate = int.Parse(stream.bit_rate)/1000;
                        mediaInfo.Video.Duration = Convert.ToDouble((string) stream.duration,
                            CultureInfo.InvariantCulture);
                        mediaInfo.Video.Resolution.Width = stream.width;
                        mediaInfo.Video.Resolution.Heigth = stream.height;

                    }
                }
            }
            catch (Exception)
            {
                return mediaInfo;
            }
            return mediaInfo;
        }

        private void SaveProgress(string output, int mediaId, double maxPercentage, double duration)
        {
           
            // HttpContext.Current.Cache[mediaId.ToString()]
        }
    }

    

     public struct MediaInfo
            {
                public Video_ Video;
                public Audio_ Audio;

                public struct Video_
                {
                    public bool HasVideo;
                    public Resolution_ Resolution;
                    //in kilobits
                    public int Bitrate;
                    public double Duration;

                    public struct Resolution_
                    {
                        public int Heigth;
                        public int Width;

                        public override string ToString()
                        {
                            return Width.ToString() + "x" + Heigth.ToString();
                        }
                    }
                }

                public struct Audio_
                {
                    public bool HasAudio;
                }
            }
}