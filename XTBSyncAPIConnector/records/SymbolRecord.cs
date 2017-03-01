using System;
using Newtonsoft.Json.Linq;
using xAPI.Codes;

namespace xAPI.Records
{
	using JSONObject = JObject;

    public class SymbolRecord : BaseResponseRecord
	{
		private double? ask;
		private double? bid;
		private string categoryName;
        private long? contractSize;
		private string currency;
        private bool? currencyPair;
        private string currencyProfit;
		private string description;
		private long? expiration;
		private string groupName;
		private double? high;
        private long? initialMargin;
		private long? instantMaxVolume;
        private double? leverage;
		private bool? longOnly;
        private double? lotMax;
        private double? lotMin;
        private double? lotStep;
		private double? low;
        private long? marginHedged;
        private bool? marginHedgedStrong;
        private long? marginMaintenance;
        private MARGIN_MODE marginMode;
		private long? precision;
        private double? percentage;
        private PROFIT_MODE profitMode;
        private long? quoteId;
        private double? spreadRaw;
        private double? spreadTable;
		private long? starting;
		private long? stepRuleId;
        private long? stopsLevel;
        private bool? swapEnable;
        private double? swapLong;
        private double? swapShort;
        private SWAP_TYPE swapType;
        private SWAP_ROLLOVER_TYPE swapRollover;
		private string symbol;
        private double? tickSize;
        private double? tickValue;
		private long? time;
        private string timeString;
		private long? type;

	    public virtual double? Ask
		{
			get
			{
				return ask;
			}
			set
			{
				ask = value;
			}
		}

		public virtual double? Bid
		{
			get
			{
				return bid;
			}
			set
			{
				bid = value;
			}
		}

		public virtual string CategoryName
		{
			get
			{
				return categoryName;
			}
			set
			{
				categoryName = value;
			}
		}

        public virtual long? ContractSize
        {
			get
			{
				return contractSize;
			}
			set
			{
				contractSize = value;
			}
		}

		public virtual string Currency
		{
			get
			{
				return currency;
			}
			set
			{
				currency = value;
			}
		}

        public virtual bool? CurrencyPair
        {
            get
            {
                return currencyPair;
            }
            set
            {
                currencyPair = value;
            }
        }

        [Obsolete("Use Precision instead")]
        public virtual long? Digits
        {
            get { return Precision; }
        }

        public string CurrencyProfit
        {
            get 
            { 
                return currencyProfit; 
            }
            set 
            { 
                currencyProfit = value; 
            }
        }

		public virtual string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		public virtual long? Expiration
		{
			get
			{
				return expiration;
			}
			set
			{
				expiration = value;
			}
		}

		public virtual string GroupName
		{
			get
			{
				return groupName;
			}
			set
			{
				groupName = value;
			}
		}

		public virtual double? High
		{
			get
			{
				return high;
			}
			set
			{
				high = value;
			}
		}

        public virtual long? InitialMargin
        {
			get
			{
				return initialMargin;
			}
			set
			{
				initialMargin = value;
			}
		}

		public virtual long? InstantMaxVolume
		{
			get
			{
				return instantMaxVolume;
			}
			set
			{
				instantMaxVolume = value;
			}
		}

        public virtual double? Leverage
        {
            get
            {
                return leverage;
            }
            set
            {
                leverage = value;
            }
        }

		public virtual bool? LongOnly
		{
			get
			{
				return longOnly;
			}
			set
			{
				longOnly = value;
			}
		}

        public virtual double? LotMax
		{
			get
			{
				return lotMax;
			}
			set
			{
				lotMax = value;
			}
		}

        public virtual double? LotMin
		{
			get
			{
				return lotMin;
			}
			set
			{
				lotMin = value;
			}
		}

        public virtual double? LotStep
        {
			get
			{
				return lotStep;
			}
			set
			{
				lotStep = value;
			}
		}

		public virtual double? Low
		{
			get
			{
				return low;
			}
			set
			{
				low = value;
			}
		}

        public virtual long? MarginHedged
        {
			get
			{
				return marginHedged;
			}
			set
			{
				marginHedged = value;
			}
		}

        public virtual bool? MarginHedgedStrong
        {
			get
			{
				return marginHedgedStrong;
			}
			set
			{
				marginHedgedStrong = value;
			}
		}

        public virtual long? MarginMaintenance
        {
			get
			{
				return marginMaintenance;
			}
			set
			{
				marginMaintenance = value;
			}
		}

        public virtual MARGIN_MODE MarginMode
        {
			get
			{
				return marginMode;
			}
			set
			{
				marginMode = value;
			}
		}

		public virtual long? Precision
		{
			get
			{
				return precision;
			}
			set
			{
				precision = value;
			}
		}

        public virtual double? Percentage
        {
			get
			{
				return percentage;
			}
			set
			{
				percentage = value;
			}
		}

        public virtual PROFIT_MODE ProfitMode
        {
			get
			{
				return profitMode;
			}
			set
			{
				profitMode = value;
			}
		}

        public long? QuoteId
        {
            get 
            { 
                return quoteId; 
            }
            set 
            { 
                quoteId = value; 
            }
        }

        public virtual double? SpreadRaw
        {
            get
            {
                return spreadRaw;
            }
            set
            {
                spreadTable = value;
            }
        }

        public virtual double? SpreadTable
        {
            get
            {
                return spreadTable;
            }
            set
            {
                spreadTable = value;
            }
        }

		public virtual long? Starting
		{
			get
			{
				return starting;
			}
			set
			{
				starting = value;
			}
		}
		
		public virtual long? StepRuleId
		{
			get
			{
				return stepRuleId;
			}
			set
			{
				stepRuleId = value;
			}
		}
		
		public virtual long? StopsLevel
		{
			get
			{
				return stopsLevel;
			}
			set
			{
				stopsLevel = value;
			}
		}

        public virtual bool? SwapEnable
        {
			get
			{
				return swapEnable;
			}
			set
			{
				swapEnable = value;
			}
		}

        public virtual double? SwapLong
        {
			get
			{
				return swapLong;
			}
			set
			{
				swapLong = value;
			}
		}

        public virtual double? SwapShort
        {
			get
			{
				return swapShort;
			}
			set
			{
				swapShort = value;
			}
		}

        public virtual SWAP_TYPE SwapType
        {
			get
			{
				return swapType;
			}
			set
			{
				swapType = value;
			}
		}

        public virtual SWAP_ROLLOVER_TYPE SwapRollover
        {
			get
			{
				return swapRollover;
			}
			set
			{
				swapRollover = value;
			}
		}

		public virtual string Symbol
		{
			get
			{
				return symbol;
			}
			set
			{
				symbol = value;
			}
		}

        public virtual double? TickSize
        {
			get
			{
				return tickSize;
			}
			set
			{
				tickSize = value;
			}
		}

        public virtual double? TickValue
        {
			get
			{
				return tickValue;
			}
			set
			{
				tickValue = value;
			}
		}

        public virtual long? Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }

        public virtual string TimeString
        {
            get
            {
                return timeString;
            }
            set
            {
                timeString = value;
            }
        }

		public virtual long? Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

        public void FieldsFromJSONObject(JSONObject value)
        {
            Ask = (double?)value["ask"];
            Bid = (double?)value["bid"];
            CategoryName = (string)value["categoryName"];
            Currency = (string)value["currency"];
            CurrencyPair = (bool?)value["currencyPair"];
            CurrencyProfit = (string)value["currencyProfit"];
            Description = (string)value["description"];
            Expiration = (long?)value["expiration"];
            GroupName = (string)value["groupName"];
            High = (double?)value["high"];
            InstantMaxVolume = (long?)value["instantMaxVolume"];
            Leverage = (double)value["leverage"];
            LongOnly = (bool?)value["longOnly"];
            LotMax = (double?)value["lotMax"];
            LotMin = (double?)value["lotMin"];
            LotStep = (double?)value["lotStep"];
            Low = (double?)value["low"];
            Precision = (long?)value["precision"];
            Starting = (long?)value["starting"];
            StopsLevel = (long?)value["stopsLevel"];
            Symbol = (string)value["symbol"];
            Time = (long?)value["time"];
            TimeString = (string)value["timeString"];
            Type = (long?)value["type"];
            ContractSize = (long?) value["contractSize"];
            InitialMargin = (long?)value["initialMargin"];
            MarginHedged = (long?)value["marginHedged"];
            MarginHedgedStrong = (bool?) value["marginHedgedStrong"];
            MarginMaintenance = (long?)value["marginMaintenance"];
            MarginMode = new MARGIN_MODE((long)value["marginMode"]);
            Percentage = (double?)value["percentage"];
            ProfitMode = new PROFIT_MODE((long)value["profitMode"]);
            QuoteId = (long?)value["quoteId"];
            SpreadRaw = (double?)value["spreadRaw"];
            SpreadTable = (double?)value["spreadTable"];
			StepRuleId = (long?)value["stepRuleId"];
            SwapEnable = (bool?)value["swapEnable"];
            SwapLong = (double?)value["swapLong"];
            SwapShort = (double?)value["swapShort"];
            SwapType = new SWAP_TYPE((long) value["swapType"]);
            SwapRollover = new SWAP_ROLLOVER_TYPE((long)value["swap_rollover3days"]);
            TickSize = (double?)value["tickSize"];
            TickValue = (double?)value["tickValue"];
        }
    }
}