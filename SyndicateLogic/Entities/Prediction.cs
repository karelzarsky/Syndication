using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("fact.predictions")]
    public class Prediction
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Index]
        public int StrategyNr { get; set; }
        [Index]
        public DateTime TimeOpen { get; set; }
        public DateTime TimeClose { get; set; }
        [Index, MaxLength(20), Column(TypeName = "varchar")]
        public string Ticker { get; set; }
        public bool BuySignal { get; set; }
        public decimal Volume { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal Commision { get; set; }
        public decimal StopLoss { get; set; }
        public decimal TakeProfit { get; set; }
        public decimal Swap { get; set; }
        public decimal Margin { get; set; }
        public decimal Profit { get; set; }
        public string Comment { get; set; }
        public ExitReason Exit { get; set; }
    }
}