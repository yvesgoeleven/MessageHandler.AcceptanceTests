using System;
using System.Net;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace Gateway.Rest.AcceptanceTests
{
    [TestFixture]
    public class can_send_batches_using_signalr
    {
        private readonly IAuthorize _auth = new WithClientCredentials();

        [Test]
        public void using_signalr_net_client()
        {
            var header = _auth.GetAuthorizationHeader();

            var client = new RestClient(Settings.BaseUri);

            var request = new RestRequest("endpoints", Method.POST);
            request.AddHeader(HttpRequestHeader.Authorization.ToString(), header);

            request.AddParameter("protocol", "signalr");
            request.AddParameter("channel", Settings.AcceptanceTestsChannel);
            request.AddParameter("environment", Settings.AcceptanceTestsEnvironment);

            var response = client.Execute(request);

            var endpoint = JsonConvert.DeserializeObject<dynamic>(response.Content);
            string address = endpoint.address;
            string hub = endpoint.hub;
            string method = endpoint.method;

            var hubConnection = new HubConnection(address);

            hubConnection.EnsureReconnecting();
            hubConnection.Headers.Add(HttpRequestHeader.Authorization.ToString(), header);

            var channelHubProxy = hubConnection.CreateHubProxy(hub);

            hubConnection.Start(new LongPollingTransport())
                .Wait(TimeSpan.FromSeconds(30));

            //keep a total message size limit of 256KB in mind
            var batch = new Object[1000];
            for (var i = 0; i < 1000; i++)
            {
                var message = new
                {
                    Temperature = 25,
                    When = DateTime.UtcNow.ToString("o")
                };
                batch[i] = message;
            }

            channelHubProxy.Invoke(method,
                Settings.AcceptanceTestsChannel,
                Settings.AcceptanceTestsEnvironment,
                new { Message = batch }
                ).Wait(TimeSpan.FromSeconds(30));

        }
    }
}