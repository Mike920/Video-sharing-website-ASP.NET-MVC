using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ASP_Video_Website.ViewModels
{
    public class UploadViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }

        [Required(ErrorMessage = "Select the media file to upload")]
        public HttpPostedFileBase File { get; set; }
    }
}