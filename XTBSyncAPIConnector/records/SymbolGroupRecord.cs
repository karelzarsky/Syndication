using System;
using Newtonsoft.Json.Linq;

namespace xAPI.Records
{
    using JSONObject = JObject;

	public class SymbolGroupRecord : BaseResponseRecord
	{
		private long? type;
		private string description;
		private string name;

        [Obsolete("Command getAllSymbolGroups is not available in API any more")]
		public SymbolGroupRecord()
		{
		}

		public virtual long? Type
		{
			get
			{
				return type;
			}
		}

		public virtual string Description
		{
			get
			{
				return description;
			}
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

        public void FieldsFromJSONObject(JSONObject value)
        {
            type = (long?)value["type"];
            description = (string)value["description"];
            name = (string)value["name"];
        }
    }
}