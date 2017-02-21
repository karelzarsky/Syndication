namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class predictions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "fact.predictions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StrategyNr = c.Int(nullable: false),
                        TimeOpen = c.DateTime(nullable: false),
                        TimeClose = c.DateTime(nullable: false),
                        Ticker = c.String(maxLength: 20, unicode: false),
                        BuySignal = c.Boolean(nullable: false),
                        Volume = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OpenPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ClosePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Commision = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StopLoss = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TakeProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Swap = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Margin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Profit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.StrategyNr)
                .Index(t => t.TimeOpen)
                .Index(t => t.Ticker);
            
        }
        
        public override void Down()
        {
            DropIndex("fact.predictions", new[] { "Ticker" });
            DropIndex("fact.predictions", new[] { "TimeOpen" });
            DropIndex("fact.predictions", new[] { "StrategyNr" });
            DropTable("fact.predictions");
        }
    }
}
