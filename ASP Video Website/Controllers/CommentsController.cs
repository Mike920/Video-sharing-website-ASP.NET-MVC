using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using ASP_Video_Website.Models;
using Microsoft.AspNet.Identity;


namespace ASP_Video_Website.Controllers
{
    public class CommentsController : ApiController
    {
        //Disable lazy loading and proxy creation to prevent loop reference issues
        private ApplicationDbContext db = new ApplicationDbContext() { Configuration = { LazyLoadingEnabled = false, ProxyCreationEnabled = false } };

        // GET: api/Comments
        public IHttpActionResult GetComments()
        {
            //db.Configuration.LazyLoadingEnabled = false;
            var comm = db.Comments.Include("element").Include("parent");
            //return Json(new { comments = comm});
            /*var json = JsonConvert.SerializeObject(new {comments = comm});

            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;*/

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            return
                Ok(
                    new
                    {
                        results =
                            new
                            {
                                comments = comm,
                                total_comment = comm.Count(),
                                user =
                                    new
                                    {
                                        user_id = currentUserId,
                                        fullname = currentUser.UserName,
                                        picture = HostingEnvironment.MapPath("~/Content/images/user_blank_picture.png"),
                                        is_logged_in = true,
                                        is_add_allowed = true,
                                        is_edit_allowed = true
                                    }
                            }
                    }
                    );
        }

        // GET: api/Comments/5
        [ResponseType(typeof(Comment))]
        public IHttpActionResult GetComment(int id)
        {
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        // PUT: api/Comments/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutComment(int id, Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != comment.comment_id)
            {
                return BadRequest();
            }

            db.Entry(comment).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Comments
        [ResponseType(typeof(Comment))]
        public IHttpActionResult PostComment(Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Comments.Add(comment);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = comment.comment_id }, comment);
        }

        // DELETE: api/Comments/5
        [ResponseType(typeof(Comment))]
        public IHttpActionResult DeleteComment(int id)
        {
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            db.Comments.Remove(comment);
            db.SaveChanges();

            return Ok(comment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CommentExists(int id)
        {
            return db.Comments.Count(e => e.comment_id == id) > 0;
        }
    }
}