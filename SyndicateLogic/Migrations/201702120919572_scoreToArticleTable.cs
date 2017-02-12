namespace SyndicateLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class scoreToArticleTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("rss.articles", "Ticker", c => c.String(maxLength: 10, unicode: false));
            AddColumn("rss.articles", "ScoreMin", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("rss.articles", "ScoreMax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("rss.articles", "Ticker");
            CreateIndex("rss.articles", "ScoreMin");
            CreateIndex("rss.articles", "ScoreMax");
        }
        
        public override void Down()
        {
            DropIndex("rss.articles", new[] { "ScoreMax" });
            DropIndex("rss.articles", new[] { "ScoreMin" });
            DropIndex("rss.articles", new[] { "Ticker" });
            DropColumn("rss.articles", "ScoreMax");
            DropColumn("rss.articles", "ScoreMin");
            DropColumn("rss.articles", "Ticker");
        }
    }
}
