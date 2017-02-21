using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{

    [Table("int.prices")]
    public class Price
    {
        [Column(Order = 0, TypeName = "varchar"), Key, MaxLength(8)]
        public string ticker { get; set; }
        [Column(Order = 1, TypeName = "Date"), Key]
        public DateTime date { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal open { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal high { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal low { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal close { get; set; }
        public long? volume { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? ex_dividend { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? split_ratio { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? adj_open { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? adj_high { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? adj_low { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? adj_close { get; set; }
        public long? adj_volume { get; set; }
    }
}