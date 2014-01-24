namespace Gateway.Rest.AcceptanceTests
{
    public class Settings
    {
        public static string BaseUri = System.Environment.GetEnvironmentVariable("Gateway.BaseUri"); //f.e. "http://api.messagehandler.net/";
        public static string ClientId = System.Environment.GetEnvironmentVariable("Gateway.ClientId"); // "YOURACCOUNT";
        public static string ClientSecret = System.Environment.GetEnvironmentVariable("Gateway.ClientSecret"); // "YOURKEY";
        public static string Channel = "demo";
        public static string Environment = "Development";
        public static string Scope = "http://api.messagehandler.net/";
    }
}