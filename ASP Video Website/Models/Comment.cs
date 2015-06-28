using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace ASP_Video_Website.Models
{
    public  class Comment
    {
        public Comment()
        {
            Children = new List<Comment>();
        }

        [Key]
        public int CommentId { get; set; } //comment_id
        public string Text { get; set; } //text
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime PostedDate { get; set; } //posted_date

        public string InReplyTo  //in_reply_to
        {
            get { return Parent != null && Parent.User != null ? Parent.User.UserName : null; }
        }
        public string Fullname {
            get { return User != null? User.UserName : null; }
        } //fullname
        public string Picture { get { return HostingEnvironment.MapPath("~/Content/images/user_blank_picture.png"); } } //picture

        public int VideoId { get; set; }  //element_id
        public virtual MediaFile Video { get; set; } //element

        public virtual int? ParentId { get; set; } //parent_id
        public virtual Comment  Parent {get; set;} //parent
        public virtual ICollection<Comment> Children { get; set; } //childrens
        public string UserId { get; set; } //created_by
        public virtual ApplicationUser User { get; set; }
        
    }
}