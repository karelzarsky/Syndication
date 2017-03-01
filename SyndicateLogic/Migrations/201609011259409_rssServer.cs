using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class rssServer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "rss.servers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        HostName = c.String(maxLength: 40),
                        LastCheck = c.DateTime(),
                        Interval = c.Int(nullable: false)
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.HostName)
                .Index(t => t.LastCheck);
            
            AddColumn("rss.feeds", "RSSServer_ID", c => c.Int());
            CreateIndex("rss.feeds", "RSSServer_ID");
            AddForeignKey("rss.feeds", "RSSServer_ID", "rss.servers", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("rss.feeds", "RSSServer_ID", "rss.servers");
            DropIndex("rss.servers", new[] { "LastCheck" });
            DropIndex("rss.servers", new[] { "HostName" });
            DropIndex("rss.feeds", new[] { "RSSServer_ID" });
            DropColumn("rss.feeds", "RSSServer_ID");
            DropTable("rss.servers");
        }
    }
}
