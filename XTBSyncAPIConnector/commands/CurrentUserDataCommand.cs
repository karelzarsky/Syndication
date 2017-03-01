using Newtonsoft.Json.Linq;

namespace xAPI.Commands
{
	using JSONObject = JObject;

	public class CurrentUserDataCommand : BaseCommand
	{
		public CurrentUserDataCommand(bool prettyPrint) : base(new JSONObject(), prettyPrint)
		{
		}

		public override string CommandName
		{
			get
			{
                return "getCurrentUserData";
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