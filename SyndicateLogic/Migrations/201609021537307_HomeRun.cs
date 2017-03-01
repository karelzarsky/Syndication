using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class HomeRun : DbMigration
    {
        public override void Up()
        {
            AddColumn("rss.feeds", "ActiveH", c => c.Boolean(nullable: false));
            AddColumn("rss.servers", "NextRunH", c => c.DateTime());
            AddColumn("rss.servers", "ErrorsH", c => c.Int(nullable: false));
            AddColumn("rss.servers", "SuccessH", c => c.Int(nullable: false));
            CreateIndex("rss.servers", "NextRunH");
        }
        
        public override void Down()
        {
            DropIndex("rss.servers", new[] { "NextRunH" });
            DropColumn("rss.servers", "SuccessH");
            DropColumn("rss.servers", "ErrorsH");
            DropColumn("rss.servers", "NextRunH");
            DropColumn("rss.feeds", "ActiveH");
        }
    }
}
