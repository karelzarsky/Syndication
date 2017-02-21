using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("rss.servers")]
    public class RSSServer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index, MaxLength(40)]
        [Column(TypeName = "varchar")]
        public string HostName { get; set; }
        [Index]
        public DateTime? LastCheck { get; set; }
        public int Interval { get; set; } = 15;
        [Index]
        public DateTime? NextRun { get; set; }
        public int Errors { get; set; }
        public int Success { get; set; }
    }
}