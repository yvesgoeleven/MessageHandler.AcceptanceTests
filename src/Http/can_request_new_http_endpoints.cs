using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using RestSharp.Deserializers;

namespace Gateway.Rest.AcceptanceTests
{
    [TestFixture]
    public class can_request_new_http_endpoints
    {
        private readonly IAuthorize _auth = new WithClientCredentials();

        [Test]
        public void using_raw_http()
        {
            var header = _auth.GetAuthorizationHeader();

            var client = new RestClient(Settings.BaseUri);

            var request = new RestRequest("endpoints", Method.POST);
            request.AddHeader(HttpRequestHeader.Authorization.ToString(), header);

            request.AddParameter("protocol", "http");
            request.AddParameter("channel", Settings.AcceptanceTestsChannel);
            request.AddParameter("environment", Settings.AcceptanceTestsEnvironment);

            var response = client.Execute(request);

            var endpoint = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.IsNotNull(endpoint.protocol);
            Assert.IsNotNull(endpoint.host);
            Assert.IsNotNull(endpoint.path);
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
                protocol = "http",
                channel = Settings.AcceptanceTestsChannel,
                environment = Settings.AcceptanceTestsEnvironment
            };

            request.RequestFormat = DataFormat.Json;
            request.AddBody(endpointFor);

            var response = client.Execute(request);

            var endpoint = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.IsNotNull(endpoint.protocol);
            Assert.IsNotNull(endpoint.host);
            Assert.IsNotNull(endpoint.path);
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
                Protocol = "http",
                Channel = Settings.AcceptanceTestsChannel,
                Environment = Settings.AcceptanceTestsEnvironment
            };

            request.RequestFormat = DataFormat.Xml;
            request.XmlSerializer = new RestSharp.Serializers.XmlSerializer();
            request.AddBody(endpointFor);

            var response = client.Execute(request);

            var endpoint = new XmlDeserializer().Deserialize<HttpEndpoint>(response);

            Assert.IsNotNull(endpoint.Protocol);
            Assert.IsNotNull(endpoint.Host);
            Assert.IsNotNull(endpoint.Path);
        }

        public class EndpointRequest
        {
            public string Protocol { get; set; }
            public string Channel { get; set; }
            public string Environment { get; set; }
        }

        public class HttpEndpoint
        {
            public string Protocol { get; set; }
            public string Host { get; set; }
            public string Path { get; set; }
        }

    }
}
