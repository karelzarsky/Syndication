namespace SyndicateLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UseForML : DbMigration
    {
        public override void Up()
        {
            AddColumn("rss.articles", "UseForML", c => c.Boolean(nullable: false));
            AddColumn("rss.shingles", "UseForML", c => c.Boolean(nullable: false));
            CreateIndex("rss.articles", "UseForML");
            CreateIndex("rss.shingles", "UseForML");
        }
        
        public override void Down()
        {
            DropIndex("rss.shingles", new[] { "UseForML" });
            DropIndex("rss.articles", new[] { "UseForML" });
            DropColumn("rss.shingles", "UseForML");
            DropColumn("rss.articles", "UseForML");
        }
    }
}
