using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("fact.shingleAction")]
    public class ShingleAction
    {
        [Key, Column(Order = 0), Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int shingleID { get; set; }
        [Key, Column(Order = 1), Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte interval { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? down { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? up { get; set; }
        public DateTime dateComputed { get; set; }
        public int samples { get; set; }
        public int tickers { get; set; }
        public Int32 down10 { get; set; }
        public Int32 down09 { get; set; }
        public Int32 down08 { get; set; }
        public Int32 down07 { get; set; }
        public Int32 down06 { get; set; }
        public Int32 down05 { get; set; }
        public Int32 down04 { get; set; }
        public Int32 down03 { get; set; }
        public Int32 down02 { get; set; }
        public Int32 down01 { get; set; }
        public Int32 down00 { get; set; }
        public Int32 up00 { get; set; }
        public Int32 up01 { get; set; }
        public Int32 up02 { get; set; }
        public Int32 up03 { get; set; }
        public Int32 up04 { get; set; }
        public Int32 up05 { get; set; }
        public Int32 up06 { get; set; }
        public Int32 up07 { get; set; }
        public Int32 up08 { get; set; }
        public Int32 up09 { get; set; }
        public Int32 up10 { get; set; }
        public double stddev { get; set; }
        public double mean { get; set; }
        public double variance { get; set; }
    }
}
