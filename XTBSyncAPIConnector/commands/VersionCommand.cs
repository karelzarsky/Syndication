using Newtonsoft.Json.Linq;

namespace xAPI.Commands
{
    using JSONObject = JObject;

    public class VersionCommand : BaseCommand
    {
        public VersionCommand(JSONObject arguments, bool prettyPrint)
            : base(arguments, prettyPrint)
        {
        }

        public override string CommandName
        {
            get
            {
                return "getVersion";
            }
        }

        public override string[] RequiredArguments
        {
            get
            {
                return new string[] { };
            }
        }
    }

}