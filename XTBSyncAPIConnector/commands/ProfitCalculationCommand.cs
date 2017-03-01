using Newtonsoft.Json.Linq;

namespace xAPI.Commands
{
	using JSONObject = JObject;

    public class ProfitCalculationCommand : BaseCommand
	{

		public ProfitCalculationCommand(JSONObject arguments, bool prettyPrint) : base(arguments, prettyPrint)
		{
		}

		public override string CommandName
		{
			get
			{
                return "getProfitCalculation";
			}
		}

		public override string[] RequiredArguments
		{
			get
			{
                return new[] { "cmd", "symbol", "volume", "openPrice", "closePrice" };
			}
		}
	}
}