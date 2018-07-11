using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
	[Table("nn.trainSchema")]
	public class TrainSchema
	{
		[Column(Order = 0), Key]
		public int NetworkID { get; set; }
		[Column(Order = 1), Key]
		public int NeuronID { get; set; }
		public bool Input { get; set; }
		public int? ShingleID {get; set;}
		public int? ForecastDays {get; set;}
    }
}