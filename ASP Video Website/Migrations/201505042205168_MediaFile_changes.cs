namespace ASP_Video_Website.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MediaFile_changes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MediaFiles", "Description", c => c.String());
            DropColumn("dbo.MediaFiles", "AssetId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MediaFiles", "AssetId", c => c.String());
            DropColumn("dbo.MediaFiles", "Description");
        }
    }
}
