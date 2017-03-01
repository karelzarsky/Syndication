using Newtonsoft.Json.Linq;

namespace xAPI.Commands
{
	using JSONObject = JObject;

	public class LoginCommand : BaseCommand
	{
		public LoginCommand(JSONObject arguments, bool prettyPrint) : base(arguments, prettyPrint)
		{
		}

		public override string CommandName
		{
			get
			{
				return "login";
			}
		}

		public override string[] RequiredArguments
		{
			get
			{
				return new[] {"userId", "password"};
			}
		}
	}
}