using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace Gateway.Rest.AcceptanceTests
{
    [TestFixture]
    public class can_authorize_with_valid_client_credentials
    {
        private string access_token;
        private string refresh_token;
        private int expires_in;

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
        public void received_refresh_token()
        {
            Assert.IsNotNull(refresh_token);
        }

        [Test]
        public void know_when_access_token_expires()
        {
            Assert.IsTrue(expires_in > 0);
        }
    }
}
