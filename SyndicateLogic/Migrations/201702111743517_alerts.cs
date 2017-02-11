namespace SyndicateLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alerts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "fact.alerts",
                c => new
                    {
                        ticker = c.String(nullable: false, maxLength: 10, unicode: false),
                        issued = c.DateTime(nullable: false),
                        scoreMin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        scoreMax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ArticleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ticker, t.issued })
                .ForeignKey("rss.articles", t => t.ArticleID, cascadeDelete: true)
                .Index(t => t.issued)
                .Index(t => t.ArticleID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("fact.alerts", "ArticleID", "rss.articles");
            DropIndex("fact.alerts", new[] { "ArticleID" });
            DropIndex("fact.alerts", new[] { "issued" });
            DropTable("fact.alerts");
        }
    }
}
