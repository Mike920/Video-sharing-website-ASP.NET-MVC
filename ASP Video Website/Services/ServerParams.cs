using ASP_Video_Website.Utility;
using System.Collections.Generic;

namespace ASP_Video_Website.Services
{
    static public class ServerParams
    {
    static public class CategoriesList
    {
        public static List<string> List
        {
            get
            {
                return new List<string>
                {
                    "Music",
                    "Movie",
                    "Entertainment",
                    "News",
                    "Sport"
                };
            }
        }
    }
    
        static public class VideoParams
            {
                static public MediaInfo p360
                {
                    get
                    {
                        var mi = new MediaInfo();
                        mi.Video.Bitrate = 750;
                        mi.Video.Resolution.Heigth = 360;
                        mi.Video.Resolution.Width = 640;
                        return mi;
                    }
                }
                static public MediaInfo p480
                {
                    get
                    {
                        var mi = new MediaInfo();
                        mi.Video.Bitrate = 1000;
                        mi.Video.Resolution.Heigth = 480;
                        mi.Video.Resolution.Width = 854;
                        return mi;
                    }
                }
                static public MediaInfo p720
                {
                    get
                    {
                        var mi = new MediaInfo();
                        mi.Video.Bitrate = 2500;
                        mi.Video.Resolution.Heigth = 720;
                        mi.Video.Resolution.Width = 1280;
                        return mi;
                    }
                }
                static public MediaInfo p1080
                {
                    get
                    {
                        var mi = new MediaInfo();
                        mi.Video.Bitrate = 5000;
                        mi.Video.Resolution.Heigth = 1080;
                        mi.Video.Resolution.Width = 1920;
                        return mi; 
                    }
                }


            static public MediaInfo GetVideoParams(VideoQuality vq)
            {
                switch (vq)
                {
                    case VideoQuality.p360: return p360;
                    case VideoQuality.p480: return p480;
                    case VideoQuality.p720: return p720;
                    case VideoQuality.p1080: return p1080;
                }
                return p360;
            }

                /// <summary>
                /// Returns: 
                /// true: m1 better or equal m2,
                /// false: m1 worse than m2
                /// </summary>
                /// <param name="m1"></param>
                /// <param name="m2"></param>
                /// <returns></returns>
                static public bool CompareVideo(MediaInfo m1, MediaInfo m2)
                {
                    if (m1.Video.Bitrate < m2.Video.Bitrate || m1.Video.Resolution.Heigth < m2.Video.Resolution.Heigth)
                        return false;
                    else 
                        return true;
                }


           
             static public VideoQuality ClassifyVideo(MediaInfo video)
            {
                if (CompareVideo(video, p1080))
                    return VideoQuality.p1080;
                if (CompareVideo(video, p720))
                    return VideoQuality.p720;
                if (CompareVideo(video, p480))
                    return VideoQuality.p480;
                
                return VideoQuality.p360;
            }

            }
    }

   
    
} 
public enum VideoQuality
    {
        p360,
        p480,
        p720,
        p1080
    }