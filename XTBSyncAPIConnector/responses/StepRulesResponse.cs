using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using xAPI.Records;

namespace xAPI.Responses
{
    using JSONArray = JArray;
    using JSONObject = JObject;

    public class StepRulesResponse : BaseResponse
	{
		private LinkedList<StepRuleRecord> stepRulesRecords = new LinkedList<StepRuleRecord>();

        public StepRulesResponse(string body)
            : base(body)
		{
            JSONArray stepRulesRecords = (JSONArray)ReturnData;
            foreach (JSONObject e in stepRulesRecords)
            {
                StepRuleRecord stepRulesRecord = new StepRuleRecord();
                stepRulesRecord.FieldsFromJSONObject(e);
                this.stepRulesRecords.AddLast(stepRulesRecord);
            }
		}

        public virtual LinkedList<StepRuleRecord> StepRulesRecords
		{
			get
			{
                return stepRulesRecords;
			}
		}
	}

}