using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class tPriceAction : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "fact.tickerPriceAction",
                c => new
                    {
                        Instrument_ID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false, storeType: "date"),
                        Interval = c.Byte(nullable: false),
                        changeUp = c.Single(nullable: false),
                        changeDown = c.Single(nullable: false)
                    })
                .PrimaryKey(t => new { t.Instrument_ID, t.Date, t.Interval })
                .ForeignKey("app.instruments", t => t.Instrument_ID, cascadeDelete: true)
                .Index(t => t.Instrument_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("fact.tickerPriceAction", "Instrument_ID", "app.instruments");
            DropIndex("fact.tickerPriceAction", new[] { "Instrument_ID" });
            DropTable("fact.tickerPriceAction");
        }
    }
}
