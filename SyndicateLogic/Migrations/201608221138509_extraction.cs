using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class extraction : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        Sha256Hash = c.Binary(nullable: false, maxLength: 32),
                        FeedID = c.Int(nullable: false),
                        Title = c.String(),
                        Summary = c.String(),
                        Content = c.String(),
                        Categories = c.String(),
                        PublishedUTC = c.DateTime(nullable: false)
                    })
                .PrimaryKey(t => t.Sha256Hash)
                .ForeignKey("dbo.Feeds", t => t.FeedID, cascadeDelete: true)
                .Index(t => t.FeedID)
                .Index(t => t.PublishedUTC);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Articles", "FeedID", "dbo.Feeds");
            DropIndex("dbo.Articles", new[] { "PublishedUTC" });
            DropIndex("dbo.Articles", new[] { "FeedID" });
            DropTable("dbo.Articles");
        }
    }
}
