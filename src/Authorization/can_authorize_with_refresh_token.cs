using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace Gateway.Rest.AcceptanceTests
{
    [TestFixture]
    public class can_authorize_with_refresh_token
    {
        private string access_token;
        private string refresh_token;
        private int expires_in;

        private string original_access_token;
        

        [SetUp]
        public void when()
        {
            var client = new RestClient(Settings.BaseUri);

            var request = new RestRequest("authorize", Method.POST);
            request.AddParameter("client_id", Settings.ClientId);
            request.AddParameter("client_secret", Settings.ClientSecret);
            request.AddParameter("scope", Settings.Scope);
            request.AddParameter("grant_type", "client_credentials");

            var response = client.Execute(request);
            
            var token = JsonConvert.DeserializeObject<dynamic>(response.Content);
            original_access_token = token.access_token;
            
            var refreshRequest = new RestRequest("authorize", Method.POST);
            refreshRequest.AddParameter("client_id", Settings.ClientId);
            refreshRequest.AddParameter("client_secret", Settings.ClientSecret);
            refreshRequest.AddParameter("scope", Settings.Scope);
            refreshRequest.AddParameter("grant_type", "refresh_token");
            refreshRequest.AddParameter("refresh_token", (string) token.refresh_token);

            response = client.Execute(refreshRequest);

            token = JsonConvert.DeserializeObject<dynamic>(response.Content);

            access_token = token.access_token;
            refresh_token = token.refresh_token;
            expires_in = token.expires_in;
        }

        [Test]
        public void received_access_token()
        {
            Assert.IsNotNull(access_token);
        }

        [Test]
        public void access_token_is_different()
        {
            Assert.AreNotEqual(original_access_token, access_token);
        }

        [Test]
        public void did_not_received_new_refresh_token()
        {
            Assert.IsNull(refresh_token);
        }

        [Test]
        public void know_when_access_token_expires()
        {
            Assert.IsTrue(expires_in > 0);
        }
    }
}
