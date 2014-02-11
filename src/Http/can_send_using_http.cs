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
    public class can_send_using_http
    {
        private readonly IAuthorize _auth = new WithClientCredentials();

        [Test]
        public void using_rest_sharp()
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
            string host = endpoint.host;
            string path = endpoint.path;

            var endPointClient = new RestClient(host);

            request = new RestRequest(path, Method.POST);
            request.AddHeader(HttpRequestHeader.Authorization.ToString(), header);

            request.AddBody("Hello world");
           
            response = endPointClient.Execute(request);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }
    }
}