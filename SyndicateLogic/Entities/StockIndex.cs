using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("int.stockIndices")]
    public class StockIndex
    {
        [Key, MaxLength(10), Column(TypeName = "varchar")]
        public string symbol { get; set; }
        [Column(TypeName = "varchar")]
        public string index_name { get; set; }
        [Column(TypeName = "varchar")]
        public string continent { get; set; }
        [Column(TypeName = "varchar")]
        public string country { get; set; }
    }
}