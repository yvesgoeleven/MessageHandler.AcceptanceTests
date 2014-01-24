using System.Net;
using NUnit.Framework;
using RestSharp;
using Newtonsoft.Json;
using RestSharp.Deserializers;

namespace Gateway.Rest.AcceptanceTests
{
    [TestFixture]
    public class can_request_new_amqp_endpoints
    {
        private readonly IAuthorize _auth = new WithClientCredentials();

        [Test]
        public void using_raw_http()
        {
            var header = _auth.GetAuthorizationHeader();

            var client = new RestClient(Settings.BaseUri);

            var request = new RestRequest("endpoints", Method.POST);
            request.AddHeader(HttpRequestHeader.Authorization.ToString(), header);

            request.AddParameter("protocol", "amqp");
            request.AddParameter("channel", Settings.Channel);
            request.AddParameter("environment", Settings.Environment);

            var response = client.Execute(request);

            var endpoint = JsonConvert.DeserializeObject<dynamic>(response.Content);
            string host = endpoint.host;
            string path = endpoint.path;
            string username = endpoint.username;
            string password = endpoint.password;
            string protocol = endpoint.protocol;

            Assert.IsNotNull(host);
            Assert.IsNotNull(path);
            Assert.IsNotNull(username);
            Assert.IsNotNull(password);
            Assert.IsNotNull(protocol);
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
                protocol = "amqp",
                channel = Settings.Channel,
                environment = Settings.Environment
            };

            request.RequestFormat = DataFormat.Json;
            request.AddBody(endpointFor);

            var response = client.Execute(request);

            var endpoint = JsonConvert.DeserializeObject<dynamic>(response.Content);
            string host = endpoint.host;
            string path = endpoint.path;
            string username = endpoint.username;
            string password = endpoint.password;
            string protocol = endpoint.protocol;

            Assert.IsNotNull(host);
            Assert.IsNotNull(path);
            Assert.IsNotNull(username);
            Assert.IsNotNull(password);
            Assert.IsNotNull(protocol);
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
                Protocol = "amqp",
                Channel = Settings.Channel,
                Environment = Settings.Environment
            };
            request.RequestFormat = DataFormat.Xml;
            request.XmlSerializer = new RestSharp.Serializers.XmlSerializer();
            request.AddBody(endpointFor);

            var response = client.Execute(request);

            var endpoint = new XmlDeserializer().Deserialize<AmqpEndpoint>(response);

            Assert.IsNotNull(endpoint.Host);
            Assert.IsNotNull(endpoint.Path);
            Assert.IsNotNull(endpoint.Password);
            Assert.IsNotNull(endpoint.Username);
            Assert.IsNotNull(endpoint.Protocol);
        }

        public class EndpointRequest
        {
            public string Protocol { get; set; }
            public string Channel { get; set; }
            public string Environment { get; set; }
        }

        public class AmqpEndpoint
        {
            public string Protocol { get; set; }
            public string Host { get; set; }
            public string Path { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

    }
}
