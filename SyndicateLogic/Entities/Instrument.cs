using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("app.instruments")]
    public class Instrument
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index, MaxLength(30), Column(TypeName = "varchar")]
        public string Ticker { get; set; }
        [Index, MaxLength(10), Column(TypeName = "varchar")]
        public string StockExchange { get; set; }
        [Index]
        public InstrumentType Type { get; set; }
        [Index]
        public DateTime? LastPriceUpdate { get; set; }
    }
}
