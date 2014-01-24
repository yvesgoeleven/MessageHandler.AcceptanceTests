using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace Gateway.Rest.AcceptanceTests
{
    [TestFixture]
    public class cannot_authorize_with_invalid_client_credentials
    {
        [Test]
        public void gives_401_on_invalid_client_id()
        {
            var client = new RestClient(Settings.BaseUri);

            var request = new RestRequest("authorize", Method.POST);
            request.AddParameter("client_id", "Bogus");
            request.AddParameter("client_secret", Settings.ClientSecret);
            request.AddParameter("scope", Settings.Scope);
            request.AddParameter("grant_type", "client_credentials");

            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);

            var exception = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.IsNotNull(exception.message);
        }

        [Test]
        public void gives_403_on_invalid_client_secret()
        {
            var client = new RestClient(Settings.BaseUri);

            var request = new RestRequest("authorize", Method.POST);
            request.AddParameter("client_id", Settings.ClientId );
            request.AddParameter("client_secret", "Bogus");
            request.AddParameter("scope", Settings.Scope);
            request.AddParameter("grant_type", "client_credentials");

            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);

            var exception = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.IsNotNull(exception.message);
        }

        [Test]
        public void gives_403_on_invalid_scope()
        {
            var client = new RestClient(Settings.BaseUri);

            var request = new RestRequest("authorize", Method.POST);
            request.AddParameter("client_id", Settings.ClientId);
            request.AddParameter("client_secret", Settings.ClientSecret);
            request.AddParameter("scope", "Bogus");
            request.AddParameter("grant_type", "client_credentials");

            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);

            var exception = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.IsNotNull(exception.message);
        }

        [Test]
        public void gives_403_on_invalid_refresh_token()
        {
            var client = new RestClient(Settings.BaseUri);

            var request = new RestRequest("authorize", Method.POST);
            request.AddParameter("client_id", Settings.ClientId);
            request.AddParameter("client_secret", Settings.ClientSecret);
            request.AddParameter("scope", Settings.Scope);
            request.AddParameter("refresh_token", "Bogus");
            request.AddParameter("grant_type", "refresh_token");

            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);

            var exception = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.IsNotNull(exception.message);
        }

        [Test]
        public void gives_500_on_invalid_grant_type()
        {
            var client = new RestClient(Settings.BaseUri);

            var request = new RestRequest("authorize", Method.POST);
            request.AddParameter("client_id", Settings.ClientId);
            request.AddParameter("client_secret", Settings.ClientSecret);
            request.AddParameter("scope", Settings.Scope);
            request.AddParameter("grant_type", "Bogus");

            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);

            var exception = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.IsNotNull(exception.message);
        }

        [Test]
        public void gives_500_on_valid_grant_type_with_invalid_parameters()
        {
            var client = new RestClient(Settings.BaseUri);

            var request = new RestRequest("authorize", Method.POST);
            request.AddParameter("client_id", Settings.ClientId);
            request.AddParameter("client_secret", Settings.ClientSecret);
            request.AddParameter("scope", Settings.Scope);
            request.AddParameter("grant_type", "refresh_token");

            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);

            var exception = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.IsNotNull(exception.message);
        }

    }
}
