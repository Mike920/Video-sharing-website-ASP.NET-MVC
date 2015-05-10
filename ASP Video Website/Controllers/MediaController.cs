using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using ASP_Video_Website.Extensions;
using ASP_Video_Website.Models;
using ASP_Video_Website.Services;
using ASP_Video_Website.ViewModels;
using Microsoft.AspNet.Identity;

namespace ASP_Video_Website.Controllers
{

    public class MediaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Video()
        {
            return new VideoResult("Media/sintel/vid7.mp4");
        }

        public ActionResult Videos()
        {
            return View();
        }

        public ActionResult Movie()
        {
            return View();
        }

        public ActionResult Mp4()
        {
            return View();
        }


        

        // GET: Media
      
        public ActionResult Index()
        {
           // return View();
             return View(db.MediaFiles.ToList());
        }

        [Route("Media/{id:int}")]
        public ActionResult Display(int id)
        {
            ViewBag.MediaId = id;
            return View();
        }

        // GET: Media/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MediaFile mediaFile = db.MediaFiles.Find(id);
            if (mediaFile == null)
            {
                return HttpNotFound();
            }
            return View(mediaFile);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload([Bind(Include = "Title,Description,IsPrivate")]MediaFile mediaFile, HttpPostedFileBase file)
        {

            mediaFile.ApplicationUserId = User.Identity.GetUserId();

            if (!ModelState.IsValid)
                return View(mediaFile);
            



            if (file != null && file.ContentLength > 0)
            {
                db.MediaFiles.Add(mediaFile);
                db.SaveChanges();

                var dir = Server.MapPath("~/App_Data/Videos/" + mediaFile.Id);

                //todo: check if dir exists
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                

                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(dir, fileName);
                file.SaveAs(path);

                MediaService.ConvertVideo(fileName,mediaFile.Id);

                ViewBag.Info = "Your video was successfully uploaded";

                return View("Info");
            }

            return View(mediaFile);
            
        }

        // GET: Media/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Media/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Title,AssetId,IsPublic")] MediaFile mediaFile)
        {
            if (ModelState.IsValid)
            {
                db.MediaFiles.Add(mediaFile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mediaFile);
        }

        // GET: Media/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MediaFile mediaFile = db.MediaFiles.Find(id);
            if (mediaFile == null)
            {
                return HttpNotFound();
            }
            return View(mediaFile);
        }

        // POST: Media/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Title,AssetId,IsPublic")] MediaFile mediaFile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mediaFile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mediaFile);
        }

        // GET: Media/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MediaFile mediaFile = db.MediaFiles.Find(id);
            if (mediaFile == null)
            {
                return HttpNotFound();
            }
            return View(mediaFile);
        }

        // POST: Media/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MediaFile mediaFile = db.MediaFiles.Find(id);
            db.MediaFiles.Remove(mediaFile);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
