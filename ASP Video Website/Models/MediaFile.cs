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
        public bool Hd { get; set; }
        public bool IsBeingConverted { get; set; }
        public VideoQuality VideoQuality { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public bool IsHd()
        {
            return VideoQuality != VideoQuality.p360;
        }
    }
}