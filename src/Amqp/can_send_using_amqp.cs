using System.Net;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using TransportType = Microsoft.ServiceBus.Messaging.TransportType;


namespace Gateway.Rest.AcceptanceTests
{
    [TestFixture]
    public class can_send_using_amqp
    {
        private readonly IAuthorize _auth = new WithClientCredentials();

        [Test]
        public void using_azure_servicebus_sdk()
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
            string username = endpoint.username;
            string password = endpoint.password;
            string path = endpoint.path;

            var factory = MessagingFactory.Create(
                "sb://" + host,
                new MessagingFactorySettings()
                {
                    TransportType = TransportType.Amqp,
                    TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(username, password)
                });

            var sender = factory.CreateMessageSender(path);

            sender.Send(new BrokeredMessage("Hello world"));
            
            sender.Close();
        }
    }
}
