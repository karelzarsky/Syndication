namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class nextRun : DbMigration
    {
        public override void Up()
        {
            AddColumn("rss.servers", "NextRun", c => c.DateTime());
            CreateIndex("rss.servers", "NextRun");
        }
        
        public override void Down()
        {
            DropIndex("rss.servers", new[] { "NextRun" });
            DropColumn("rss.servers", "NextRun");
        }
    }
}
