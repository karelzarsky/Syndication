using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class firstTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Articles",
                c => new
                {
                    Hash = c.Int(false),
                    FeedID = c.Int(false),
                    Title = c.String(),
                    ReceivedUTC = c.DateTime(false),
                    RSS20 = c.String(),
                    PublishedUTC = c.DateTime(false)
                })
                .PrimaryKey(t => t.Hash)
                .ForeignKey("dbo.Feeds", t => t.FeedID, true)
                .Index(t => t.FeedID);

            CreateTable(
                "dbo.Feeds",
                c => new
                {
                    ID = c.Int(false, true),
                    LastCheck = c.DateTime(),
                    LastArticleReceived = c.DateTime(),
                    Active = c.Boolean(false),
                    Url = c.String()
                })
                .PrimaryKey(t => t.ID);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Articles", "FeedID", "dbo.Feeds");
            DropIndex("dbo.Articles", new[] {"FeedID"});
            DropTable("dbo.Feeds");
            DropTable("dbo.Articles");
        }
    }
}