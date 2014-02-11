using System;
using System.Text;
using Newtonsoft.Json;
using RestSharp;

namespace Gateway.Rest.AcceptanceTests
{
    public class WithClientCredentials : IAuthorize
    {
        private const string Scope = "http://api.messagehandler.net/";
        private static string bearer;
        private static string refresh_token;
        private static DateTime expires;

        public string GetAuthorizationHeader()
        {
            var client = new RestClient(Settings.BaseUri);
            RestRequest request = null;

            if (bearer == null)
            {
                request = new RestRequest("authorize", Method.POST);
                request.AddParameter("client_id", Settings.ClientId);
                request.AddParameter("client_secret", Settings.ClientSecret);
                request.AddParameter("scope", Scope);
                request.AddParameter("grant_type", "client_credentials");
            }
            else if (expires <= DateTime.Now.AddMinutes(1))
            {
                request = new RestRequest("authorize", Method.POST);
                request.AddParameter("client_id", Settings.ClientId);
                request.AddParameter("client_secret", Settings.ClientSecret);
                request.AddParameter("scope", Scope);
                request.AddParameter("grant_type", "refresh_token");
                request.AddParameter("refresh_token", refresh_token);
            }

            if (request != null)
            {
                var response = client.Execute(request);

                var token = JsonConvert.DeserializeObject<dynamic>(response.Content);

                bearer = "Bearer " + Convert.ToBase64String(Encoding.UTF8.GetBytes((string)token.access_token));
                expires = DateTime.Now.AddSeconds((int)token.expires_in);
                refresh_token = token.refresh_token;
            }
            return bearer;
        }

    }
}