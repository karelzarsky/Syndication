namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class articleScoreMax : DbMigration
    {
        public override void Up()
        {
            DropIndex("rss.articles", new[] { "ScoreMin" });
            DropIndex("rss.articles", new[] { "ScoreMax" });
            AddColumn("rss.articles", "ScoreDownMin", c => c.Decimal(nullable: false, storeType: "smallmoney"));
            AddColumn("rss.articles", "ScoreUpMax", c => c.Decimal(nullable: false, storeType: "smallmoney"));
            AddColumn("fact.articleScore", "scoreDown", c => c.Decimal(storeType: "smallmoney"));
            AddColumn("fact.articleScore", "scoreUp", c => c.Decimal(storeType: "smallmoney"));
            CreateIndex("rss.articles", "ScoreDownMin");
            CreateIndex("rss.articles", "ScoreUpMax");
        }
        
        public override void Down()
        {
            DropIndex("rss.articles", new[] { "ScoreUpMax" });
            DropIndex("rss.articles", new[] { "ScoreDownMin" });
            DropColumn("fact.articleScore", "scoreUp");
            DropColumn("fact.articleScore", "scoreDown");
            DropColumn("rss.articles", "ScoreUpMax");
            DropColumn("rss.articles", "ScoreDownMin");
            CreateIndex("rss.articles", "ScoreMax");
            CreateIndex("rss.articles", "ScoreMin");
        }
    }
}
