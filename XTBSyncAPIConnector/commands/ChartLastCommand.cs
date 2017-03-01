using Newtonsoft.Json.Linq;

namespace xAPI.Commands
{
	using JSONObject = JObject;

	public class ChartLastCommand : BaseCommand
	{
		public ChartLastCommand(JSONObject arguments, bool prettyPrint) : base(arguments, prettyPrint)
		{
		}

		public override string CommandName
		{
			get
			{
                return "getChartLastRequest";
			}
		}

		public override string[] RequiredArguments
		{
			get
			{
				return new[] {"info"};
			}
		}
	}
}