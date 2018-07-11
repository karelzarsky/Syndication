using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("nn.trainValue")]
    public class TrainValue
    {
		[Column(Order = 0), Key]
		public int NetworkID { get; set; }
		[Column(Order = 1), Key]
		public int NeuronID { get; set; }
		[Column(Order = 2), Key]
		public int ArticleID { get; set; }
		public float Value { get; set; } = 0.0f;
    }
}