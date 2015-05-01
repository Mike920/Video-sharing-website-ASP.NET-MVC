using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using ASP_Video_Website.Models;

namespace ASP_Video_Website.Controllers
{
    public class MediaController : Controller
    {
        private MediaDbContext db = new MediaDbContext();

        public ActionResult Video()
        {
            return new VideoResult();
        }

        public ActionResult Videos()
        {
            return View();
        }

        public ActionResult Movie()
        {
            return View();
        }


        public ActionResult File(string id)
        {
            if(String.IsNullOrWhiteSpace(id))
                return HttpNotFound();

            //TODO: if movie return videoresult coz movie needs to be streamed, return error if not found
            var filename = HostingEnvironment.MapPath("~/Media/sintel/" + id);

            string contentType = MimeMapping.GetMimeMapping(id);


            return File(filename, contentType, id);
        }

        // GET: Media
        public ActionResult Index()
        {
           // return View();
             return View(db.MediaElements.ToList());
        }

        // GET: Media/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MediaElement mediaElement = db.MediaElements.Find(id);
            if (mediaElement == null)
            {
                return HttpNotFound();
            }
            return View(mediaElement);
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
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
        public ActionResult Create([Bind(Include = "Id,UserId,Title,AssetId,IsPublic")] MediaElement mediaElement)
        {
            if (ModelState.IsValid)
            {
                db.MediaElements.Add(mediaElement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mediaElement);
        }

        // GET: Media/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MediaElement mediaElement = db.MediaElements.Find(id);
            if (mediaElement == null)
            {
                return HttpNotFound();
            }
            return View(mediaElement);
        }

        // POST: Media/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Title,AssetId,IsPublic")] MediaElement mediaElement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mediaElement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mediaElement);
        }

        // GET: Media/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MediaElement mediaElement = db.MediaElements.Find(id);
            if (mediaElement == null)
            {
                return HttpNotFound();
            }
            return View(mediaElement);
        }

        // POST: Media/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MediaElement mediaElement = db.MediaElements.Find(id);
            db.MediaElements.Remove(mediaElement);
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
