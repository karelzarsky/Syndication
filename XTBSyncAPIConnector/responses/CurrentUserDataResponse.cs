using System;
using Newtonsoft.Json.Linq;

namespace xAPI.Responses
{
    using JSONObject = JObject;

	public class CurrentUserDataResponse : BaseResponse
	{
        private string currency;
        private long? leverage;
        private double? leverageMultiplier;
        private string group;
        private int? companyUnit;
        private string spreadType;
        private bool? ibAccount;

        public CurrentUserDataResponse(string body)
            : base(body)
		{
			JSONObject ob = (JSONObject) ReturnData;
            currency = (string)ob["currency"];
            leverage = (long?)ob["leverage"];
            leverageMultiplier = (double?)ob["leverageMultiplier"];
            group = (string)ob["group"];
            companyUnit = (int?)ob["companyUnit"];
            spreadType = (string)ob["spreadType"];
            ibAccount = (bool?)ob["ibAccount"];
		}

        public virtual string Currency
        {
            get
            {
                return currency;
            }
        }

        [Obsolete("Use LeverageMultiplier instead")]
		public virtual long? Leverage
		{
			get
			{
				return leverage;
			}
		}

        public virtual double? LeverageMultiplier
        {
            get
            {
                return leverageMultiplier;
            }
        }

        public virtual string Group
        {
            get
            {
                return group;
            }
        }

        public virtual int? CompanyUnit
        {
            get
            {
                return companyUnit;
            }
        }

        public virtual string SpreadType
        {
            get
            {
                return spreadType;
            }
        }

        public virtual bool? IbAccount
        {
            get
            {
                return ibAccount;
            }
        }
	}

}