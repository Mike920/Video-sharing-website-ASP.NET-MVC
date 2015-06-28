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
        [Key]
        public int comment_id { get; set; }
        public string text { get; set; }
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime posted_date { get; set; }
        public string in_reply_to { get; set; }
        public string fullname { get; set; }
        public string picture { get { return HostingEnvironment.MapPath("~/Content/images/user_blank_picture.png"); } }

      //  public int element_id { get; set; }
        public virtual MediaFile element { get; set; }

        public virtual int? parent_id { get; set; }
        public virtual Comment  parent {get; set;}
        public virtual ICollection<Comment> childrens { get; set; }
        //user id
        public string created_by { get; set; }
    }
}