namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
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
