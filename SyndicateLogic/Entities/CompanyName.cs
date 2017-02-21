using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("app.companyNames")]
    public class CompanyName
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Index, MaxLength(100)]
        public string name { get; set; }
        public string ticker { get; set; }
    }
}
