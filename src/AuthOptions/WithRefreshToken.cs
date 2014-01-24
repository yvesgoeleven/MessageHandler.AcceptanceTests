using System;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using RestSharp;

namespace Gateway.Rest.AcceptanceTests
{
    public class WithRefreshToken : IAuthorize
    {
        private const string Scope = "http://api.messagehandler.net/";

        public string GetAuthorizationHeader()
        {
            var client = new RestClient(Settings.BaseUri);

            var originalRequest = new RestRequest("authorize", Method.POST);
            originalRequest.AddParameter("client_id", Settings.ClientId);
            originalRequest.AddParameter("client_secret", Settings.ClientSecret);
            originalRequest.AddParameter("scope", Scope);
            originalRequest.AddParameter("grant_type", "client_credentials");

            var response = client.Execute(originalRequest);

            var token = JsonConvert.DeserializeObject<dynamic>(response.Content);
            
            string refresh_token = token.refresh_token;
            int expires_in = token.expires_in;

//            Thread.Sleep(expires_in);

            var refreshRequest = new RestRequest("authorize", Method.POST);
            refreshRequest.AddParameter("client_id", Settings.ClientId);
            refreshRequest.AddParameter("client_secret", Settings.ClientSecret);
            refreshRequest.AddParameter("scope", Scope);
            refreshRequest.AddParameter("grant_type", "refresh_token");
            refreshRequest.AddParameter("refresh_token", refresh_token);

            response = client.Execute(refreshRequest);

            string access_token = JsonConvert.DeserializeObject<dynamic>(response.Content).access_token;

            return "Bearer " + Convert.ToBase64String(Encoding.UTF8.GetBytes(access_token));
        }

    }
}