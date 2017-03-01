using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("rss.articleRelations")]
    public class ArticleRelation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index, ForeignKey("Article")]
        [Index("IX_ArticleAndInstrument", 1, IsUnique = true)]
        public int ArticleID { get; set; }
        public virtual Article Article { get; set; }
        [Index, ForeignKey("Instrument")]
        [Index("IX_ArticleAndInstrument", 2, IsUnique = true)]
        public int InstrumentID { get; set; }
        public virtual Instrument Instrument { get; set; }
    }
}