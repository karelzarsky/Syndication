using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("rss.feeds")]
    public class Feed
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTime? LastCheck { get; set; }
        public DateTime? LastArticleReceived { get; set; }
        public DateTime? LastError { get; set; }
        public bool Active { get; set; }
        [Column(TypeName = "varchar"), MaxLength(300)]
        [Index(IsUnique = true)]
        public string Url { get; set; }
        public string Title { get; set; }
        public string ErrorMessage { get; set; }
        [Column(TypeName = "varchar")]
        public string Language { get; set; }
        public string Categories { get; set; }
        [Column(TypeName = "varchar")]
        public string Links { get; set; }
        //[Index, ForeignKey("RSSServer")]
        //public int RSSServerID { get; set; }
        public virtual RSSServer RSSServer { get; set; }
        public virtual Instrument AffectedInstrument { get; set; }
        public string HostName()
        {
            Uri myUri = new Uri(Url);
            return myUri.Host;
        }
    }
}