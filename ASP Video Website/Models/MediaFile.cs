using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASP_Video_Website.Models
{
    public class MediaFile
    {
        //Files path depends on Id
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string Title { get; set; }
      //  public string AssetId { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}