using System.Collections.Specialized;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;

namespace MonitorWang.Core.Geckoboard
{
    public class QueryString
    {
        public static string AsString(string key)
        {
            return Values[key];
        }

        public static string AsString(string key, string defaultValue)
        {
            var value = Values[key];
            return (string.IsNullOrEmpty(value)) ? defaultValue : value;
        }

        public static int AsInt(string key, int defaultValue)
        {
            int value;

            if (!int.TryParse(Values[key], out value))
                value = defaultValue;

            return value;
        }

        public static bool AsBool(string key, bool defaultValue)
        {
            bool value;

            if (!bool.TryParse(Values[key], out value))
                value = defaultValue;

            return value;
        }

        private static NameValueCollection Values
        {
            get
            {
                var request = OperationContext.Current.RequestContext.RequestMessage;
                var requestProperties = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
                return HttpUtility.ParseQueryString(requestProperties.QueryString);
            }
        }
    }
}