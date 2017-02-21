using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("app.indexComponents")]
    public class IndexComponent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Index, MaxLength(20), Column(TypeName = "varchar")]
        public string IndexSymbol { get; set; }
        [Index, MaxLength(10), Column(TypeName = "varchar")]
        public string StockTicker { get; set; }
    }
}
