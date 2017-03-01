using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class Prices : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Prices",
                c => new
                    {
                        ticker = c.String(nullable: false, maxLength: 128),
                        date = c.DateTime(nullable: false),
                        open = c.Decimal(nullable: false, precision: 18, scale: 5),
                        high = c.Decimal(nullable: false, precision: 18, scale: 5),
                        low = c.Decimal(nullable: false, precision: 18, scale: 5),
                        close = c.Decimal(nullable: false, precision: 18, scale: 5),
                        volume = c.Decimal(nullable: false, precision: 18, scale: 5),
                        ex_dividend = c.Decimal(nullable: false, precision: 18, scale: 5),
                        split_ratio = c.Decimal(nullable: false, precision: 18, scale: 5),
                        adj_open = c.Decimal(nullable: false, precision: 18, scale: 5),
                        adj_high = c.Decimal(nullable: false, precision: 18, scale: 5),
                        adj_low = c.Decimal(nullable: false, precision: 18, scale: 5),
                        adj_close = c.Decimal(nullable: false, precision: 18, scale: 5),
                        adj_volume = c.Decimal(nullable: false, precision: 18, scale: 5)
                    })
                .PrimaryKey(t => new { t.ticker, t.date })
                .ForeignKey("dbo.Companies", t => t.ticker, cascadeDelete: true)
                .Index(t => t.ticker);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Prices", "ticker", "dbo.Companies");
            DropIndex("dbo.Prices", new[] { "ticker" });
            DropTable("dbo.Prices");
        }
    }
}
