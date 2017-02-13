namespace SyndicateLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class investorRSSFeed : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "app.investorRSS",
                c => new
                    {
                        Ticker = c.String(nullable: false, maxLength: 10, unicode: false),
                        Url = c.String(nullable: false, maxLength: 8000, unicode: false),
                    })
                .PrimaryKey(t => t.Ticker);
            
        }
        
        public override void Down()
        {
            DropTable("app.investorRSS");
        }
    }
}
