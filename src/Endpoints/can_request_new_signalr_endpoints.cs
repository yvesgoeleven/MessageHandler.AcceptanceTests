using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using RestSharp.Deserializers;

namespace Gateway.Rest.AcceptanceTests
{
    [TestFixture]
    public class can_request_new_signalr_endpoints
    {
        private readonly IAuthorize _auth = new WithClientCredentials();

        [Test]
        public void using_raw_http()
        {
            var header = _auth.GetAuthorizationHeader();

            var client = new RestClient(Settings.BaseUri);

            var request = new RestRequest("endpoints", Method.POST);
            request.AddHeader(HttpRequestHeader.Authorization.ToString(), header);

            request.AddParameter("protocol", "signalr");
            request.AddParameter("channel", Settings.Channel);
            request.AddParameter("environment", Settings.Environment);

            var response = client.Execute(request);

            var endpoint = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.IsNotNull(endpoint.protocol);
            Assert.IsNotNull(endpoint.address);
        }

        [Test]
        public void using_json()
        {
            var header = _auth.GetAuthorizationHeader();

            var client = new RestClient(Settings.BaseUri);

            var request = new RestRequest("endpoints", Method.POST);
            request.AddHeader(HttpRequestHeader.Authorization.ToString(), header);

            var endpointFor = new
            {
                protocol = "signalr",
                channel = Settings.Channel,
                environment = Settings.Environment
            };

            request.RequestFormat = DataFormat.Json;
            request.AddBody(endpointFor);

            var response = client.Execute(request);

            var endpoint = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.IsNotNull(endpoint.protocol);
            Assert.IsNotNull(endpoint.address);
        }

        [Test]
        public void using_xml()
        {
            var header = _auth.GetAuthorizationHeader();

            var client = new RestClient(Settings.BaseUri);

            var request = new RestRequest("endpoints", Method.POST);
            request.AddHeader(HttpRequestHeader.Authorization.ToString(), header);
            request.AddHeader("Accept", "text/xml");

            var endpointFor = new EndpointRequest // datatype required for serializer
            {
                Protocol = "signalr",
                Channel = Settings.Channel,
                Environment = Settings.Environment
            };

            request.RequestFormat = DataFormat.Xml;
            request.XmlSerializer = new RestSharp.Serializers.XmlSerializer();
            request.AddBody(endpointFor);

            var response = client.Execute(request);

            var endpoint = new XmlDeserializer().Deserialize<SignalrEndpoint>(response);

            Assert.IsNotNull(endpoint.Protocol);
            Assert.IsNotNull(endpoint.Address);
        }

        public class EndpointRequest
        {
            public string Protocol { get; set; }
            public string Channel { get; set; }
            public string Environment { get; set; }
        }

        public class SignalrEndpoint
        {
            public string Protocol { get; set; }
            public string Address { get; set; }
        }
    }
}
