using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class EAIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Articles", new[] { "FeedID" });
            CreateIndex("dbo.Articles", "FeedID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Articles", new[] { "FeedID" });
            CreateIndex("dbo.Articles", "FeedID");
        }
    }
}
