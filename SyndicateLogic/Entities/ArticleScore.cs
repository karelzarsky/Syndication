using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("fact.articleScore")]
    public class ArticleScore
    {
        [Key, Column(Order = 0), Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int32 ModelNr { get; set; }
        [Key, Column(Order = 1), Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ArticleID { get; set; }
        [Key, Column(Order = 2), Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte Interval { get; set; }
        public float? Score { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DateComputed { get; set; }
        public float? ScoreDown { get; set; }
        public float? ScoreUp { get; set; }
    }
}