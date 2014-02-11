using System;
using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace Gateway.Rest.AcceptanceTests
{
    [TestFixture]
    public class can_send_batches_using_http_and_json
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

            var serialized = JsonConvert.SerializeObject(batch);

            request.AddBody(serialized);
           
            response = endPointClient.Execute(request);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }
    }
}