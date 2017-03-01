using Newtonsoft.Json.Linq;

namespace xAPI.Commands
{
	using JSONObject = JObject;

	public class MarginLevelCommand : BaseCommand
	{
		public MarginLevelCommand(bool? prettyPrint) : base(new JSONObject(), prettyPrint)
		{
		}

		public override string CommandName
		{
			get
			{
				return "getMarginLevel";
			}
		}

		public override string[] RequiredArguments
		{
			get
			{
				return new string[]{};
			}
		}
	}
}