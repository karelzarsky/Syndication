namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addProcessedScore : DbMigration
    {
        public override void Up()
        {
            AddColumn("rss.articles", "ProcessedScore", c => c.Byte(nullable: false));
            CreateIndex("rss.articles", "ProcessedScore");
        }
        
        public override void Down()
        {
            DropIndex("rss.articles", new[] { "ProcessedScore" });
            DropColumn("rss.articles", "ProcessedScore");
        }
    }
}
