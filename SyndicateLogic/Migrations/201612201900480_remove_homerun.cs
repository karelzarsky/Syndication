namespace SyndicateLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_homerun : DbMigration
    {
        public override void Up()
        {
            DropIndex("rss.articles", new[] { "Processed1" });
            DropIndex("rss.articles", new[] { "Processed2" });
            DropIndex("rss.articles", new[] { "Processed3" });
            DropIndex("rss.articles", new[] { "Processed4" });
            DropIndex("rss.articles", new[] { "Processed5" });
            DropIndex("rss.servers", new[] { "NextRunH" });
            DropColumn("rss.articles", "Processed1");
            DropColumn("rss.articles", "Processed2");
            DropColumn("rss.articles", "Processed3");
            DropColumn("rss.articles", "Processed4");
            DropColumn("rss.articles", "Processed5");
            DropColumn("rss.feeds", "ActiveH");
            DropColumn("rss.servers", "NextRunH");
            DropColumn("rss.servers", "ErrorsH");
            DropColumn("rss.servers", "SuccessH");
        }
        
        public override void Down()
        {
            AddColumn("rss.servers", "SuccessH", c => c.Int(nullable: false));
            AddColumn("rss.servers", "ErrorsH", c => c.Int(nullable: false));
            AddColumn("rss.servers", "NextRunH", c => c.DateTime());
            AddColumn("rss.feeds", "ActiveH", c => c.Boolean(nullable: false));
            AddColumn("rss.articles", "Processed5", c => c.Byte(nullable: false));
            AddColumn("rss.articles", "Processed4", c => c.Byte(nullable: false));
            AddColumn("rss.articles", "Processed3", c => c.Byte(nullable: false));
            AddColumn("rss.articles", "Processed2", c => c.Byte(nullable: false));
            AddColumn("rss.articles", "Processed1", c => c.Byte(nullable: false));
            CreateIndex("rss.servers", "NextRunH");
            CreateIndex("rss.articles", "Processed5");
            CreateIndex("rss.articles", "Processed4");
            CreateIndex("rss.articles", "Processed3");
            CreateIndex("rss.articles", "Processed2");
            CreateIndex("rss.articles", "Processed1");
        }
    }
}
