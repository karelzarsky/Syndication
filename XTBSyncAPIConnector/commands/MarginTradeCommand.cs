using Newtonsoft.Json.Linq;

namespace xAPI.Commands
{
	using JSONObject = JObject;

	public class MarginTradeCommand : BaseCommand
	{
		public MarginTradeCommand(JSONObject arguments, bool prettyPrint) : base(arguments, prettyPrint)
		{
		}

		public override string CommandName
		{
			get
			{
				return "getMarginTrade";
			}
		}

		public override string[] RequiredArguments
		{
			get
			{
				return new[]{"symbol", "volume"};
			}
		}
	}
}