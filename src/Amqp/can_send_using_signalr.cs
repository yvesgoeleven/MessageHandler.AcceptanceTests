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
    public class can_send_using_signalr
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

            // todo, figure out a way to embed channel and environment into the hub, 
            // user passed channel & environment in already
            // when requesting the endpoint, strange to have to pass it again
            channelHubProxy.Invoke(method,
                Settings.AcceptanceTestsChannel,
                Settings.AcceptanceTestsEnvironment,
                new { Message = "Hello world" }
                ).Wait(TimeSpan.FromSeconds(30));
            
        }
    }
}