using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("rss.shingles")]
    public class Shingle
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index]
        public byte tokens { get; set; }
        [Index]
        public ShingleKind kind { get; set; }
        [Required, MaxLength(200), Index]
        public string text { get; set; }
        [Required, MaxLength(2), Index, Column(TypeName = "varchar")]
        public string language { get; set; }
        [Index]
        public DateTime? LastRecomputeDate { get; set; }
		[Index]
		public bool UseForML { get; set; }
	}
}