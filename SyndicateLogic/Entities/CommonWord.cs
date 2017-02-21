using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("app.commonWords")]
    public class CommonWord
    {
        [Required, MaxLength(2), Key, Column(TypeName = "varchar", Order = 0)]
        public string language { get; set; }
        [Required, MaxLength(50), Key, Column(Order = 1)]
        public string text { get; set; }
    }
}
