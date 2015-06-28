using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.ModelBinding;
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
            var comm = db.Comments.Include("Video").Include("Parent").Include("Children").Where(c => c.Parent == null);
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
                                        Fullname = currentUser != null ? currentUser.UserName : null,
                                        Picture = HostingEnvironment.MapPath("~/Content/images/user_blank_picture.png"),
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

            if (id != comment.CommentId)
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
            string currentUserId = User.Identity.GetUserId();
            if (currentUserId == null)
                return Unauthorized();
            /*if(VideoId == null)
                return BadRequest(ModelState);
            comment.VideoId = (int)VideoId;*/
            comment.User = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            comment.PostedDate = DateTime.Now;
            ModelState.Clear();
            TryValidateModel(comment);
            if (!TryValidateModel(comment))
            {
                return BadRequest(ModelState);
            }

            db.Comments.Add(comment);
            db.SaveChanges();

            return Ok(new
            {
                success=true, result = comment
            
            });
            //return CreatedAtRoute("DefaultApi", new { id = comment.CommentId }, comment);
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
            return db.Comments.Count(e => e.CommentId == id) > 0;
        }

        protected internal bool TryValidateModel(object model)
        {
            return TryValidateModel(model, null /* prefix */);
        }

        protected internal bool TryValidateModel(object model, string prefix)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType());
            var t = new ModelBindingExecutionContext(new HttpContextWrapper(HttpContext.Current), new System.Web.ModelBinding.ModelStateDictionary());

            foreach (ModelValidationResult validationResult in ModelValidator.GetModelValidator(metadata, t).Validate(null))
            {
                ModelState.AddModelError(validationResult.MemberName, validationResult.Message);
            }

            return ModelState.IsValid;
        }
    }
}