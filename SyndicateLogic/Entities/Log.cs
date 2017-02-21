using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("app.logs")]
    public class Log
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Index]
        public DateTime Time { get; set; } = DateTime.Now;
        [Index]
        public byte Severity { get; set; }
        public string Message { get; set; }
    }
}
