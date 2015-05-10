using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using ASP_Video_Website.Utility;

namespace ASP_Video_Website.Controllers
{
    public class TestController : Controller
    {

        public string Index()
        {
            return "Index";
        }
        // GET: Test
        public string Convert(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
                return "File name not specified.";

            var input = HostingEnvironment.MapPath("~/Media/vid7.mp4");
            var output = HostingEnvironment.MapPath("~/Media/"+id+".mp4");

            if (System.IO.File.Exists(output))
                return "The file already exists";

            //var command = String.Format("-i \"{0}\" ", input );
            var command = String.Format("-i \"{0}\" -an -b:v 250k -s 1024x436 -vcodec libx264 \"{1}\"", input, output);


            var task = Task.Factory.StartNew(() => ConvertTask(command), TaskCreationOptions.LongRunning);
            // Get file information :) d
            

            return "The file is being converted";
        }

        public void ConvertTask(string command)
        {
            FFMPEG ffmpeg = new FFMPEG();
            var result = ffmpeg.RunCommand(command);
                Debug.WriteLine(result);
        }

        public string FileDetails()
        {
            FFMPEG ffmpeg = new FFMPEG(FFMPEG.ProgramToRun.FFPROBE);
            var info = ffmpeg.GetMediaInfo(HostingEnvironment.MapPath("~/Media/vid7.mp4"));

            return "aa";
        }
    }
}