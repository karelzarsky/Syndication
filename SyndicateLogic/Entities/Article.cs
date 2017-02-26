using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("rss.articles")]
    public class Article
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("Feed")]
        [Index]
        public int FeedID { get; set; }
        public virtual Feed Feed { get; set; }
        [Index, MaxLength(500)]
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Categories { get; set; }
        [Index]
        [Display(Name = "Published")]
        public DateTime PublishedUTC { get; set; }
        [Display(Name = "Received")]
        public DateTime ReceivedUTC { get; set; }
        [Index]
        public ProcessState Processed { get; set; }
        [Index]
        public ProcessState ProcessedScore { get; set; }
        public string URI_links { get; set; }
        [Index, Column(TypeName = "varchar"), MaxLength(10)]
        public string Ticker { get; set; } // Stock market ticker symbol associated with the companies common ticker securities
        public decimal ScoreMin { get; set; }
        public decimal ScoreMax { get; set; }
        [Index, Column(TypeName = "smallmoney")]
        public decimal ScoreDownMin { get; set; }
        [Index, Column(TypeName = "smallmoney")]
        public decimal ScoreUpMax { get; set; }
        [Index, MaxLength(2), Column(TypeName = "varchar")]
        public string language { get; set; }

        public int ComputeHash()
        { return Text().ToLower().GetHashCode(); }

        public string Text()
        { return Title + " | " + Summary; }
    }
}
