using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("xtb.ETFStocksUS")]
    public class ETFStock
    {
        [Key, Required, MaxLength(10), Column(TypeName = "varchar"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Ticker { get; set; }
    }
}
