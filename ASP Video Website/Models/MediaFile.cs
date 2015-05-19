using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ASP_Video_Website.Models
{
    public class MediaFile
    {
        //Files path depends on Id
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [Required]
        public string Title { get; set; }
      //  public string AssetId { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [DisplayName("Make private")]
        public bool IsPrivate { get; set; }
        public bool Hd { get; set; }
        public bool IsBeingConverted { get; set; }
        public VideoQuality VideoQuality { get; set; }
        [Required]
        public virtual string Category { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public bool IsHd()
        {
            return VideoQuality != VideoQuality.p360;
        }
    }

 /*   public enum Category
    {
        Music,
        Movie,
        Entertainment,
        News,
        Sport
    }*/
   /* public static class Category
    {
        public static string Music { get { return "Music"; } }
        public static string Movie { get { return "Movie"; } }
        public static string Entertainment { get { return "Entertainment"; } }
        public static string News { get { return "News"; } }
        public static string Sport { get { return "Sport"; } } 
    }*/
}