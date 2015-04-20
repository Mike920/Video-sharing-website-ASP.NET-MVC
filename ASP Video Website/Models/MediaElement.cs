using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASP_Video_Website.Models
{
    public class MediaElement
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string AssetId { get; set; }
        public bool IsPublic { get; set; } 
    }
}