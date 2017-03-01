using Newtonsoft.Json.Linq;

namespace xAPI.Commands
{
    using JSONObject = JObject;

    public class TradesCommand : BaseCommand
    {
        public TradesCommand(JSONObject arguments, bool prettyPrint)
            : base(arguments, prettyPrint)
        {
        }

        public override string CommandName
        {
            get
            {
                return "getTrades";
            }
        }

        public override string[] RequiredArguments
        {
            get
            {
                return new[] { "openedOnly" };
            }
        }
    }

}