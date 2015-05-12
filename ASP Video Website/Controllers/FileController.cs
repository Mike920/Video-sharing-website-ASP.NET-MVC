using System;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.IO;
using ASP_Video_Website.Extensions;

namespace ASP_Video_Website.Controllers
{
    public class FileController : Controller
    {
        public ActionResult File(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
                return HttpNotFound();

            if (!System.IO.File.Exists(HostingEnvironment.MapPath("~/Media/sintel/" + id)))
                return HttpNotFound();

            //todo jesli mp4 return videoresult
            if (id.Split('.').Last() == "mp4")
                return new VideoResult("Media/sintel/" + id);




            //TODO: if movie return videoresult coz movie needs to be streamed, return error if not found
            var filename = HostingEnvironment.MapPath("~/Media/sintel/" + id);

            string contentType = MimeMapping.GetMimeMapping(id);


            return File(filename, contentType, id);


        }

        [Route("File/MediaSegment/{id}/{filename}")]
        public ActionResult MediaSegment(int id, string filename)
        {

            var filepath = HostingEnvironment.MapPath("~/MediaData/Videos/" + id + "/segments/" + filename);

            if (!System.IO.File.Exists(filepath))
                return HttpNotFound();

            string contentType = MimeMapping.GetMimeMapping(filepath);

            return File(filepath, contentType, filename);
        }

        [Route("File/MediaFile/{id}/{filename}")]
        public ActionResult MediaFile(int id, string filename)
        {

            var filepath = HostingEnvironment.MapPath("~/MediaData/Videos/" + id + "/" + filename);

            if (!System.IO.File.Exists(filepath))
                return HttpNotFound();

            string contentType = MimeMapping.GetMimeMapping(filepath);

            return File(filepath, contentType, filename);
        }

        public string LogFile()
        {
            if (System.IO.File.Exists(HostingEnvironment.MapPath("~/MediaData/log.txt")))
                return System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/MediaData/log.txt"));
            return "Log file doesn't exist";
        }

        public ActionResult GetLogFile()
        {
            var logFile = HostingEnvironment.MapPath("~/MediaData/log.txt");
            string contentType = MimeMapping.GetMimeMapping(logFile);

            return File(logFile, contentType, "log.txt");
        }
    }
}