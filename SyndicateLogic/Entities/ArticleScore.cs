using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("fact.articleScore")]
    public class ArticleScore
    {
        [Key, Column(Order = 0), Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int articleID { get; set; }
        [Key, Column(Order = 1), Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte interval { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? score { get; set; }
        [Column(TypeName = "Date")]
        public DateTime dateComputed { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? scoreDown { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? scoreUp { get; set; }
    }
}