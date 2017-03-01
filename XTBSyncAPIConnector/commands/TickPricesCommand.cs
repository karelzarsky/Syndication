using Newtonsoft.Json.Linq;

namespace xAPI.Commands
{
    using JSONObject = JObject;

    public class TickPricesCommand : BaseCommand
    {
        public TickPricesCommand(JSONObject arguments, bool prettyPrint)
            : base(arguments, prettyPrint)
        {
        }

        public override string CommandName
        {
            get
            {
                return "getTickPrices";
            }
        }

        public override string[] RequiredArguments
        {
            get
            {
                return new[] { "symbols", "timestamp" };
            }
        }
    }
}