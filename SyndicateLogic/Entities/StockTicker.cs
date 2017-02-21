using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("app.stockTickers")]
    public class StockTicker
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Index, MaxLength(20), Column(TypeName = "varchar")]
        public string ticker { get; set; }
    }
}
