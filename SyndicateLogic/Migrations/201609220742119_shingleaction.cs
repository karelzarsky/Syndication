namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class shingleaction : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("fact.tickerPriceAction", "Instrument_ID", "app.instruments");
            DropIndex("fact.tickerPriceAction", new[] { "Instrument_ID" });
            CreateTable(
                "fact.shingleAction",
                c => new
                    {
                        shingleID = c.Int(nullable: false),
                        interval = c.Byte(nullable: false),
                        down = c.Decimal(storeType: "smallmoney"),
                        up = c.Decimal(storeType: "smallmoney"),
                        dateComputed = c.DateTime(nullable: false, storeType: "date"),
                        samples = c.Int(nullable: false),
                        tickers = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.shingleID, t.interval });
            
            DropTable("fact.tickerPriceAction");
        }
        
        public override void Down()
        {
            CreateTable(
                "fact.tickerPriceAction",
                c => new
                    {
                        Instrument_ID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false, storeType: "date"),
                        Interval = c.Byte(nullable: false),
                        changeUp = c.Single(nullable: false),
                        changeDown = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new { t.Instrument_ID, t.Date, t.Interval });
            
            DropTable("fact.shingleAction");
            CreateIndex("fact.tickerPriceAction", "Instrument_ID");
            AddForeignKey("fact.tickerPriceAction", "Instrument_ID", "app.instruments", "ID", cascadeDelete: true);
        }
    }
}
