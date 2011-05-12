using System;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml.Linq;
using Microsoft.ServiceModel.Web;
using MonitorWang.Core.Geckoboard.DataProvider;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Geckoboard
{

    public class GeckoboardDataServiceActivityConfig : ConfigBase
    {
        public string ServiceImplementation { get; set; }
        public string Uri { get; set; }
        public string ApiKey { get; set; }
    }

    /// <summary>
    /// Provides a WCF REST ServiceHost suitable for hosting within a windows service
    /// </summary>
    public class GeckoboardDataServiceActivity : IActivityPlugin
    {
        protected readonly GeckoboardDataServiceActivityConfig myConfig;
        protected readonly Type myServiceImplementation;
        protected readonly PluginDescriptor myIdentity;

        protected WebServiceHost2 myServiceHost;

        /// <summary>
        /// default ctor
        /// </summary>
        /// <param name="config"></param>
        public GeckoboardDataServiceActivity(GeckoboardDataServiceActivityConfig config)
        {
            myConfig = config;
            myServiceImplementation = Type.GetType(config.ServiceImplementation, true, true);
            myIdentity = new PluginDescriptor
                             {
                                 Description = string.Format("GeckoboardDataService Host for ServiceType '{0}'", config.ServiceImplementation),
                                 Name = "GeckoboardDataServiceActivity",
                                 TypeId = new Guid("B0DF79D9-AE25-4965-AE7F-7B6AAC94093E")
                             };
        }

        public PluginDescriptor Identity
        {
            get { return myIdentity; }
        }

        public void Start()
        {
            myServiceHost = new WebServiceHost2(myServiceImplementation, false, new Uri(myConfig.Uri));

            if (!string.IsNullOrEmpty(myConfig.ApiKey))
                myServiceHost.Interceptors.Add(new GeckoboardApiKeyInterceptor(myConfig.ApiKey));
            myServiceHost.Open();

            Logger.Debug("\t\t{0} is listening on these endpoints...", Identity.Name);
            myServiceHost.Description.Endpoints.ToList().ForEach(
                t => Logger.Debug("\t\t{0}", t.ListenUri));
        }

        public void Stop()
        {
            switch (myServiceHost.State)
            {
                case CommunicationState.Opened:
                    // this is a blocking call...
                    myServiceHost.Close();
                    break;

                case CommunicationState.Faulted:
                    // ensures resources are released
                    myServiceHost.Abort();
                    break;
            }
        }

        /// <summary>
        /// Pauses the current task activity (gracefully)
        /// </summary>
        public void Pause()
        {

        }

        /// <summary>
        /// Continues task activity
        /// </summary>
        public void Continue()
        {

        }

        public void Initialise()
        {
            // Load all Geckoboard Data Providers
            Logger.Debug("\tRegistering all Geckoboard Data Providers...");
            Container.RegisterAll<IGeckoboardDataProvider>();
        }

        public Status Status { get; set; }

        public bool Enabled
        {
            get { return myConfig.Enabled; }
            set { myConfig.Enabled = value; }
        }
    }

    public class GeckoboardApiKeyInterceptor : RequestInterceptor
    {
        protected readonly string myApiKey;

        public GeckoboardApiKeyInterceptor(string apiKey) 
            : base(false)
        {
            myApiKey = apiKey;
        }

        public override void ProcessRequest(ref RequestContext requestContext)
        {
            if (!IsValidApiKey(requestContext))
                GenerateErrorResponse(requestContext, HttpStatusCode.Unauthorized,
                    "Missing or invalid ApiKey");
        }

        public bool IsValidApiKey(RequestContext requestContext)
        {
            var request = requestContext.RequestMessage;
            var requestProp = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
            var apikey = requestProp.Headers["Authorization"];
            var b64Key = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(myApiKey + ":X"));
            return (string.CompareOrdinal(b64Key, apikey) == 0);
        }

        public void GenerateErrorResponse(RequestContext requestContext,
            HttpStatusCode statusCode, string errorMessage)
        {
            // The error message is padded so that IE shows the response by default
            var errorXml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?><root><error>Access denied</error></root>";
            var response = XElement.Load(new StringReader(string.Format(errorXml, errorMessage)));
            var reply = Message.CreateMessage(MessageVersion.None, null, response);
            var responseProp = new HttpResponseMessageProperty
                                   {
                                       StatusCode = statusCode
                                   };
            responseProp.Headers[HttpResponseHeader.ContentType] = "text/xml";
            reply.Properties[HttpResponseMessageProperty.Name] = responseProp;
            requestContext.Reply(reply);

            // set the request context to null to terminate processing of this request
            requestContext = null;
        }
    }
}