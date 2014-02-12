//using System;
//using System.Net;
//using Microsoft.ServiceBus;
//using Microsoft.ServiceBus.Messaging;
//using Newtonsoft.Json;
//using NUnit.Framework;
//using RestSharp;
//using TransportType = Microsoft.ServiceBus.Messaging.TransportType;


//namespace Gateway.Rest.AcceptanceTests
//{
//    [TestFixture]
//    public class can_send_batches_using_amqp_and_json
//    {
//        private readonly IAuthorize _auth = new WithClientCredentials();

//        [Test]
//        public void using_azure_servicebus_sdk()
//        {
//            var header = _auth.GetAuthorizationHeader();

//            var client = new RestClient(Settings.BaseUri);

//            var request = new RestRequest("endpoints", Method.POST);
//            request.AddHeader(HttpRequestHeader.Authorization.ToString(), header);

//            request.AddParameter("protocol", "amqp");
//            request.AddParameter("channel", Settings.PerformanceTestsChannel);
//            request.AddParameter("environment", Settings.PerformanceTestsEnvironment);

//            var response = client.Execute(request);

//            var endpoint = JsonConvert.DeserializeObject<dynamic>(response.Content);
//            string host = endpoint.host;
//            string username = endpoint.username;
//            string password = endpoint.password;
//            string path = endpoint.path;

//            var factory = MessagingFactory.Create(
//                "sb://" + host,
//                new MessagingFactorySettings
//                {
//                    TransportType = TransportType.Amqp,
//                    TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(username, password)
//                });

//            var sender = factory.CreateMessageSender(path);

//            // keep a total message size limit of 256KB in mind 
//            var batch = new Object[1000];
//            for (var i = 0; i < 1000; i++)
//            {
//                var message = new
//                {
//                    Temperature = 25,
//                    When = DateTime.UtcNow.ToString("o")
//                };
//                batch[i] = message;
//            }

//            var serialized = JsonConvert.SerializeObject(batch);

//            sender.Send(new BrokeredMessage(serialized));
            
//            sender.Close();
//        }
//    }
//}
