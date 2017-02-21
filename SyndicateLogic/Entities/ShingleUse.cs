using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("rss.shingleUse")]
    public class ShingleUse
    {
        [Key, Column(Order = 0), Required, ForeignKey("Shingle"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ShingleID { get; set; }
        public virtual Shingle Shingle { get; set; }
        [Key, Column(Order = 1), Required, ForeignKey("Article"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ArticleID { get; set; }
        public virtual Article Article { get; set; }
    }
}