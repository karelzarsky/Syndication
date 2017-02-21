using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("int.companies")]
    public class Company
    {
        [Key, Column(TypeName = "varchar")]
        public string ticker { get; set; } // Stock market ticker symbol associated with the companies common ticker securities
        [Column(TypeName = "varchar")]
        public string cik { get; set; } // Central Index Key issued by the SEC
        [Column(TypeName = "nvarchar"), Index, MaxLength(100)]
        public string name { get; set; } // short form
        [Column(TypeName = "varchar")]
        public string template { get; set; }
        public bool standardized_active { get; set; }
        public bool valuation_active { get; set; }
    }
}