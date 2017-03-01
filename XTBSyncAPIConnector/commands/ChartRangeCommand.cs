using Newtonsoft.Json.Linq;

namespace xAPI.Commands
{
	using JSONObject = JObject;

	public class ChartRangeCommand : BaseCommand
	{
		public ChartRangeCommand(JSONObject arguments, bool prettyPrint) : base(arguments, prettyPrint)
		{
		}

		public override string CommandName
		{
			get
			{
				return "getChartRangeRequest";
			}
		}

		public override string[] RequiredArguments
		{
			get
			{
				return new[]{"info"};
			}
		}
	}
}