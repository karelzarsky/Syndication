using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class backtestTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "fact.backtest",
                c => new
                    {
                        StrategyNr = c.Int(nullable: false, identity: true),
                        TimeCalculated = c.DateTime(nullable: false),
                        TakeProfitPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StopLossPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DaysToTimeout = c.Int(nullable: false),
                        SignalName = c.String(),
                        SellLimit = c.Double(nullable: false),
                        BuyLimit = c.Double(nullable: false),
                        FromDate = c.DateTime(nullable: false),
                        TillDate = c.DateTime(nullable: false),
                        MinShingleSamples = c.Int(nullable: false),
                        ShingleInterval = c.Int(nullable: false),
                        Wins = c.Int(nullable: false),
                        Loses = c.Int(nullable: false),
                        Takeprofits = c.Int(nullable: false),
                        Stoploses = c.Int(nullable: false),
                        Timeouts = c.Int(nullable: false),
                        Buys = c.Int(nullable: false),
                        Sells = c.Int(nullable: false),
                        BuyProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SellProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WinProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LossProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeoutProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AverageTradeLength = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Comment = c.String()
                        //TotalTrades = c.Int(nullable: false),
                        //TotalProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        //AverageProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        //AverageBuyProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        //AverageSellProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        //AverageWinProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        //AverageLossProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        //AverageTimeoutProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        //WinRatio = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.StrategyNr);
            Sql("ALTER TABLE fact.backtest ADD TotalTrades AS Wins + Loses + Timeouts");
            Sql("ALTER TABLE fact.backtest ADD TotalProfit AS BuyProfit + SellProfit");
            Sql("ALTER TABLE fact.backtest ADD AverageProfit AS CASE WHEN (Buys + Sells>0) THEN (BuyProfit + SellProfit) / (Buys + Sells) ELSE 0 END");
            Sql("ALTER TABLE fact.backtest ADD AverageBuyProfit AS CASE WHEN (Buys>0) THEN BuyProfit / Buys ELSE 0 END");
            Sql("ALTER TABLE fact.backtest ADD AverageSellProfit AS CASE WHEN (Sells>0) THEN SellProfit / Sells ELSE 0 END");
            Sql("ALTER TABLE fact.backtest ADD AverageWinProfit AS CASE WHEN (Wins>0) THEN WinProfit / Wins ELSE 0 END");
            Sql("ALTER TABLE fact.backtest ADD AverageLossProfit AS CASE WHEN (Loses>0) THEN LossProfit / Loses ELSE 0 END");
            Sql("ALTER TABLE fact.backtest ADD AverageTimeoutProfit AS CASE WHEN (Timeouts>0) THEN TimeoutProfit / Timeouts ELSE 0 END");
            Sql("ALTER TABLE fact.backtest ADD WinRatio AS CASE WHEN (Loses>0) THEN CAST (Wins as FLOAT) / cast( Loses AS FLOAT) ELSE 0 END");
        }

        public override void Down()
        {
            DropTable("fact.backtest");
        }
    }
}
