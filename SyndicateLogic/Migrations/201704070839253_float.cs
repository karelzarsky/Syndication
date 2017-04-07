namespace SyndicateLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _float : DbMigration
    {
        public override void Up()
        {
            DropIndex("rss.articles", new[] { "ScoreDownMin" });
            DropIndex("rss.articles", new[] { "ScoreUpMax" });
            AlterColumn("rss.articles", "ScoreMin", c => c.Single(nullable: false));
            AlterColumn("rss.articles", "ScoreMax", c => c.Single(nullable: false));
            AlterColumn("rss.articles", "ScoreDownMin", c => c.Single(nullable: false));
            AlterColumn("rss.articles", "ScoreUpMax", c => c.Single(nullable: false));
            AlterColumn("fact.articleScore", "score", c => c.Single());
            AlterColumn("fact.articleScore", "scoreDown", c => c.Single());
            AlterColumn("fact.articleScore", "scoreUp", c => c.Single());
            AlterColumn("fact.shingleAction", "down", c => c.Single());
            AlterColumn("fact.shingleAction", "up", c => c.Single());
            AlterColumn("fact.shingleAction", "stddev", c => c.Single(nullable: false));
            AlterColumn("fact.shingleAction", "mean", c => c.Single(nullable: false));
            AlterColumn("fact.shingleAction", "variance", c => c.Single(nullable: false));
            CreateIndex("rss.articles", "ScoreDownMin");
            CreateIndex("rss.articles", "ScoreUpMax");
        }
        
        public override void Down()
        {
            DropIndex("rss.articles", new[] { "ScoreUpMax" });
            DropIndex("rss.articles", new[] { "ScoreDownMin" });
            AlterColumn("fact.shingleAction", "variance", c => c.Double(nullable: false));
            AlterColumn("fact.shingleAction", "mean", c => c.Double(nullable: false));
            AlterColumn("fact.shingleAction", "stddev", c => c.Double(nullable: false));
            AlterColumn("fact.shingleAction", "up", c => c.Decimal(storeType: "smallmoney"));
            AlterColumn("fact.shingleAction", "down", c => c.Decimal(storeType: "smallmoney"));
            AlterColumn("fact.articleScore", "scoreUp", c => c.Decimal(storeType: "smallmoney"));
            AlterColumn("fact.articleScore", "scoreDown", c => c.Decimal(storeType: "smallmoney"));
            AlterColumn("fact.articleScore", "score", c => c.Decimal(storeType: "smallmoney"));
            AlterColumn("rss.articles", "ScoreUpMax", c => c.Decimal(nullable: false, storeType: "smallmoney"));
            AlterColumn("rss.articles", "ScoreDownMin", c => c.Decimal(nullable: false, storeType: "smallmoney"));
            AlterColumn("rss.articles", "ScoreMax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("rss.articles", "ScoreMin", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("rss.articles", "ScoreUpMax");
            CreateIndex("rss.articles", "ScoreDownMin");
        }
    }
}
