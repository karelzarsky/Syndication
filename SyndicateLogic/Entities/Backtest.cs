using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("fact.backtest")]
    public class Backtest
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StrategyNr { get; set; }
        public DateTime TimeCalculated { get; set; }
        public decimal TakeProfitPercent { get; set; }
        public decimal StopLossPercent { get; set; }
        public int DaysToTimeout { get; set; }
        public string SignalName { get; set; }
        public double SellLimit { get; set; }
        public double BuyLimit { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime TillDate { get; set; }
        public int MinShingleSamples { get; set; }
        public int ShingleInterval { get; set; }

        public int Wins { get; set; }
        public int Loses { get; set; }
        public int Takeprofits { get; set; }
        public int Stoploses { get; set; }
        public int Timeouts { get; set; }
        public int Buys { get; set; }
        public int Sells { get; set; }
        public decimal BuyProfit { get; set; }
        public decimal SellProfit { get; set; }
        public decimal WinProfit { get; set; }
        public decimal LossProfit { get; set; }
        public decimal TimeoutProfit { get; set; }
        public decimal AverageTradeLength { get; set; }
        public string Comment { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int TotalTrades { get { return Wins + Loses + Timeouts; } private set { } }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal TotalProfit { get { return BuyProfit + SellProfit; } private set { } }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal AverageProfit { get { return (TotalTrades > 0) ? TotalProfit / TotalTrades : 0; } private set { } }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal AverageBuyProfit { get { return (Buys > 0) ? BuyProfit / Buys : 0; } private set { } }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal AverageSellProfit { get { return (Sells > 0) ? SellProfit / Sells : 0; } private set { } }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal AverageWinProfit { get { return (Wins > 0) ? WinProfit / Wins : 0; } private set { } }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal AverageLossProfit { get { return (Loses > 0) ? LossProfit / Loses : 0; } private set { } }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal AverageTimeoutProfit { get { return (Timeouts > 0) ? TimeoutProfit / Timeouts : 0; } private set { } }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double WinRatio { get { return (Loses > 0) ? (double)Wins / (Wins + Loses) : 0; } private set { } }
    }
}