using Newtonsoft.Json.Linq;

namespace xAPI.Commands
{
    using JSONObject = JObject;

    public class TradeRecordsCommand : BaseCommand
    {
        public TradeRecordsCommand(JSONObject arguments, bool prettyPrint)
            : base(arguments, prettyPrint)
        {
        }

        public override string CommandName
        {
            get
            {
                return "getTradeRecords";
            }
        }

        public override string[] RequiredArguments
        {
            get
            {
                return new[] { "orders" };
            }
        }
    }

}