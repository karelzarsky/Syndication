using System;
using Newtonsoft.Json.Linq;
using xAPI.Errors;

namespace xAPI.Responses
{
    using JSONAware = JContainer;
    using JSONObject = JObject;

    public class BaseResponse
	{
		private bool? status;
		private string errorDescr;
		private ERR_CODE errCode;
        private JSONAware returnData;
        private string customTag;

		public BaseResponse(string body)
		{
			JSONObject ob;
			try
			{
				ob = JSONObject.Parse(body);
                
			}
			catch (Exception x)
			{
				throw new APIReplyParseException("JSON Parse exception: " + body + "\n" + x.Message);
			}
			
			if (ob == null)
			{
				throw new APIReplyParseException("JSON Parse exception: " + body);
			}
		    status = (bool?) ob["status"];
		    errCode = new ERR_CODE((string)ob["errorCode"]);
		    errorDescr = (string) ob["errorDescr"];
		    returnData = (JSONAware) ob["returnData"];
		    customTag = (string)ob["customTag"];

		    if (status == null)
		    {
		        Console.Error.WriteLine(body);
		        throw new APIReplyParseException("JSON Parse error: " + "\"status\" is null!");
		    }

		    if ((status==null) || ((bool)!status))
		    {
		        // If status is false check if redirect exists in given response
		        if (ob["redirect"] == null)
		        {
		            if (errorDescr == null)
		                errorDescr = ERR_CODE.getErrorDescription(errCode.StringValue);
		            throw new APIErrorResponse(errCode, errorDescr, body);
		        }
		    }
		}

		public virtual object ReturnData
		{
			get
			{
				return returnData;
			}
		}

		public virtual bool? Status
		{
			get
			{
				return status;
			}
		}

        public virtual string ErrorDescr
        {
            get
            {
                return errorDescr;
            }
        }

        public string CustomTag
        {
            get
            {
                return customTag;
            }
        }

        public string ToJSONString()
        {
            JSONObject obj = new JSONObject();
            obj.Add("status", status);

            if (returnData != null)
                obj.Add("returnData", returnData.ToString());

            if (errCode != null)
                obj.Add("errorCode", errCode.StringValue);

            if (errorDescr != null)
                obj.Add("errorDescr", errorDescr);

            if (customTag != null)
                obj.Add("customTag", customTag);

            return obj.ToString();
        }
	}
}