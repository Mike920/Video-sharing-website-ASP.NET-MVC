using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASP_Video_Website.Models;

namespace ASP_Video_Website.Controllers
{
    public class AbController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Ab
        public ActionResult Index()
        {
            var mediaFiles = db.MediaFiles.Include(m => m.ApplicationUser);
            return View(mediaFiles.ToList());
        }

        // GET: Ab/Details/5
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

        // GET: Ab/Create
        public ActionResult Create()
        {
         //   ViewBag.ApplicationUserId = new SelectList(db., "Id", "Email");
            return View();
        }

        // POST: Ab/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ApplicationUserId,CategoryId,Title,Description,IsPrivate,Hd,IsBeingConverted,VideoQuality")] MediaFile mediaFile)
        {
            if (ModelState.IsValid)
            {
                db.MediaFiles.Add(mediaFile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

           // ViewBag.ApplicationUserId = new SelectList(db.ApplicationUsers, "Id", "Email", mediaFile.ApplicationUserId);
            return View(mediaFile);
        }

        // GET: Ab/Edit/5
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
          //  ViewBag.ApplicationUserId = new SelectList(db.ApplicationUsers, "Id", "Email", mediaFile.ApplicationUserId);
            return View(mediaFile);
        }

        // POST: Ab/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ApplicationUserId,CategoryId,Title,Description,IsPrivate,Hd,IsBeingConverted,VideoQuality")] MediaFile mediaFile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mediaFile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
          //  ViewBag.ApplicationUserId = new SelectList(db.ApplicationUsers, "Id", "Email", mediaFile.ApplicationUserId);
            return View(mediaFile);
        }

        // GET: Ab/Delete/5
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

        // POST: Ab/Delete/5
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
