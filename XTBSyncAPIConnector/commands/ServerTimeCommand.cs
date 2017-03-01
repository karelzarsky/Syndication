using Newtonsoft.Json.Linq;

namespace xAPI.Commands
{
	using JSONObject = JObject;

	public class ServerTimeCommand : BaseCommand
	{
		public ServerTimeCommand(bool? prettyPrint) : base(new JSONObject(), prettyPrint)
		{
		}

		public override string CommandName
		{
			get
			{
				return "getServerTime";
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