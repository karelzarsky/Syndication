namespace SyndicateLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class stockTickersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "app.stockTickers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ticker = c.String(maxLength: 20, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ticker);
            
        }
        
        public override void Down()
        {
            DropIndex("app.stockTickers", new[] { "ticker" });
            DropTable("app.stockTickers");
        }
    }
}
