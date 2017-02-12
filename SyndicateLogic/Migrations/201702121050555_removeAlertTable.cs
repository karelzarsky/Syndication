namespace SyndicateLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeAlertTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("fact.alerts", "ArticleID", "rss.articles");
            DropIndex("fact.alerts", new[] { "issued" });
            DropIndex("fact.alerts", new[] { "ArticleID" });
            DropTable("fact.alerts");
        }
        
        public override void Down()
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
                .PrimaryKey(t => new { t.ticker, t.issued });
            
            CreateIndex("fact.alerts", "ArticleID");
            CreateIndex("fact.alerts", "issued");
            AddForeignKey("fact.alerts", "ArticleID", "rss.articles", "ID", cascadeDelete: true);
        }
    }
}
