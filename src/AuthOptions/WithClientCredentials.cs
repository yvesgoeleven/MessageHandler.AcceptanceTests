using System;
using System.Text;
using Newtonsoft.Json;
using RestSharp;

namespace Gateway.Rest.AcceptanceTests
{
    public class WithClientCredentials : IAuthorize
    {
       private const string Scope = "http://api.messagehandler.net/";

        public string GetAuthorizationHeader()
        {
            var client = new RestClient(Settings.BaseUri);

            var request = new RestRequest("authorize", Method.POST);
            request.AddParameter("client_id", Settings.ClientId);
            request.AddParameter("client_secret", Settings.ClientSecret);
            request.AddParameter("scope", Scope);
            request.AddParameter("grant_type", "client_credentials");

            var response = client.Execute(request);

            string token = JsonConvert.DeserializeObject<dynamic>(response.Content).access_token;

            return "Bearer " + Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
        }

    }
}