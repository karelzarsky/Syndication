using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class removeArticlesTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Articles", "FeedID", "dbo.Feeds");
            DropIndex("dbo.Articles", new[] { "FeedID" });
            DropIndex("dbo.Articles", new[] { "PublishedUTC" });
            DropTable("dbo.Articles");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        Sha256Hash = c.Binary(nullable: false, maxLength: 32),
                        FeedID = c.Int(nullable: false),
                        Title = c.String(),
                        ReceivedUTC = c.DateTime(nullable: false),
                        RSS20 = c.String(),
                        PublishedUTC = c.DateTime(nullable: false)
                    })
                .PrimaryKey(t => t.Sha256Hash);
            
            CreateIndex("dbo.Articles", "PublishedUTC");
            CreateIndex("dbo.Articles", "FeedID");
            AddForeignKey("dbo.Articles", "FeedID", "dbo.Feeds", "ID", cascadeDelete: true);
        }
    }
}
