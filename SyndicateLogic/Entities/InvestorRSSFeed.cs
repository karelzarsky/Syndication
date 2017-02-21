using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("app.investorRSS")]
    public class InvestorRSSFeed
    {
        [Key, Required, MaxLength(10), Column(TypeName = "varchar"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Ticker { get; set; }
        [Required, Column(TypeName = "varchar"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Index(IsUnique = true), MaxLength(300)]
        public string Url { get; set; }
    }
}
