using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("app.currencies")]
    public class Currency
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index, MaxLength(100)]
        public string Entity { get; set; }
        [Index, MaxLength(100)]
        public string CurrencyName { get; set; }
        [Index, MaxLength(3), Column(TypeName = "varchar")]
        public string AplhabeticCode { get; set; }
        public Int16? NumericCode { get; set; }
        public byte MinorUnit { get; set; }
    }
}
