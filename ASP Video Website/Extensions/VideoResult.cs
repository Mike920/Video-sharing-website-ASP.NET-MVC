using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace ASP_Video_Website.Extensions
{
    public class VideoResult : ActionResult
    {
        private string videoFilePath;

        //example videoPath: Media/vid7.mp4
        public VideoResult(string videoPath)
        {
            //The File Path
            videoFilePath = videoPath;
        }
       /* public override void ExecuteResult(ControllerContext context)
        {
            var name = videoFilePath.Split('\\').Last();
            string contentType = MimeMapping.GetMimeMapping(name);
            //The header information
            context.HttpContext.Response.AddHeader("Content-Disposition", "filename=" + name + "; inline=false;");
            context.HttpContext.Response.AddHeader("Content-Type", contentType + ";");
            var file = new FileInfo(videoFilePath);
            //Check the file exist,  it will be written into the response
            if (file.Exists)
            {
                var stream = file.OpenRead();
                var bytesinfile = new byte[stream.Length];
                stream.Read(bytesinfile, 0, (int)file.Length);
                context.HttpContext.Response.BinaryWrite(bytesinfile);
            }
        } */

         public override void ExecuteResult(ControllerContext context)
         {

             string fullpath = videoFilePath;
            long size, start, end, length, fp = 0;
            using (StreamReader reader = new StreamReader(fullpath))
            {

                size = reader.BaseStream.Length;
                start = 0;
                end = size - 1;
                length = size;
                // Now that we've gotten so far without errors we send the accept range header
                /* At the moment we only support single ranges.
                 * Multiple ranges requires some more work to ensure it works correctly
                 * and comply with the spesifications: http://www.w3.org/Protocols/rfc2616/rfc2616-sec19.html#sec19.2
                 *
                 * Multirange support annouces itself with:
                 * header('Accept-Ranges: bytes');
                 *
                 * Multirange content must be sent with multipart/byteranges mediatype,
                 * (mediatype = mimetype)
                 * as well as a boundry header to indicate the various chunks of data.
                 */
                context.HttpContext.Response.AddHeader("Accept-Ranges", "0-" + size);
                // header('Accept-Ranges: bytes');
                // multipart/byteranges
                // http://www.w3.org/Protocols/rfc2616/rfc2616-sec19.html#sec19.2

                if (!String.IsNullOrEmpty(context.HttpContext.Request.ServerVariables["HTTP_RANGE"]))
                {
                    long anotherStart = start;
                    long anotherEnd = end;
                    string[] arr_split = context.HttpContext.Request.ServerVariables["HTTP_RANGE"].Split(new char[] { Convert.ToChar("=") });
                    string range = arr_split[1];

                    // Make sure the client hasn't sent us a multibyte range
                    if (range.IndexOf(",") > -1)
                    {
                        // (?) Shoud this be issued here, or should the first
                        // range be used? Or should the header be ignored and
                        // we output the whole content?
                        context.HttpContext.Response.AddHeader("Content-Range", "bytes " + start + "-" + end + "/" + size);
                        throw new HttpException(416, "Requested Range Not Satisfiable");

                    }

                    // If the range starts with an '-' we start from the beginning
                    // If not, we forward the file pointer
                    // And make sure to get the end byte if spesified
                    if (range.StartsWith("-"))
                    {
                        // The n-number of the last bytes is requested
                        anotherStart = size - Convert.ToInt64(range.Substring(1));
                    }
                    else
                    {
                        arr_split = range.Split(new char[] { Convert.ToChar("-") });
                        anotherStart = Convert.ToInt64(arr_split[0]);
                        long temp = 0;
                        anotherEnd = (arr_split.Length > 1 && Int64.TryParse(arr_split[1].ToString(), out temp)) ? Convert.ToInt64(arr_split[1]) : size;
                    }
                    /* Check the range and make sure it's treated according to the specs.
                     * http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html
                     */
                    // End bytes can not be larger than $end.
                    anotherEnd = (anotherEnd > end) ? end : anotherEnd;
                    // Validate the requested range and return an error if it's not correct.
                    if (anotherStart > anotherEnd || anotherStart > size - 1 || anotherEnd >= size)
                    {

                        context.HttpContext.Response.AddHeader("Content-Range", "bytes " + start + "-" + end + "/" + size);
                        throw new HttpException(416, "Requested Range Not Satisfiable");
                    }
                    start = anotherStart;
                    end = anotherEnd;

                    length = end - start + 1; // Calculate new content length
                    fp = reader.BaseStream.Seek(start, SeekOrigin.Begin);
                    context.HttpContext.Response.StatusCode = 206;
                }
            }
            // Notify the client the byte range we'll be outputting
            context.HttpContext.Response.AddHeader("Content-Range", "bytes " + start + "-" + end + "/" + size);
            context.HttpContext.Response.AddHeader("Content-Length", length.ToString());
            // Start buffered download
            context.HttpContext.Response.WriteFile(fullpath, fp, length);
            context.HttpContext.Response.End();

        }
    }
}