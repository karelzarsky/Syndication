using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("int.json")]
    public class IntrinioJson
    {
        public IntrinioJson()
        {
            time = DateTime.Now;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime time { get; set; }
        [Column(TypeName = "varchar")]
        public string Url { get; set; }
        [Column(TypeName = "varchar")]
        public string ClassT { get; set; }
        public byte[] Compressed { get; set; }
        public string Json()
        { return DataLayer.Unzip(Compressed); }
    }
}