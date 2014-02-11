namespace Gateway.Rest.AcceptanceTests
{
    public class Settings
    {
        public static string BaseUri = System.Environment.GetEnvironmentVariable("Gateway.BaseUri"); //f.e. "http://api.messagehandler.net/";
        public static string ClientId = System.Environment.GetEnvironmentVariable("Gateway.ClientId"); // "YOURACCOUNT";
        public static string ClientSecret = System.Environment.GetEnvironmentVariable("Gateway.ClientSecret"); // "YOURKEY";
        public static string AcceptanceTestsChannel = System.Environment.GetEnvironmentVariable("Gateway.AcceptanceTestsChannel"); //"acceptancetest";
        public static string AcceptanceTestsEnvironment = System.Environment.GetEnvironmentVariable("Gateway.AcceptanceTestsEnvironment"); //"Development";
        public static string PerformanceTestsChannel = System.Environment.GetEnvironmentVariable("Gateway.PerformanceTestsChannel"); //"performancetest";
        public static string PerformanceTestsEnvironment = System.Environment.GetEnvironmentVariable("Gateway.PerformanceTestsEnvironment"); //"Development";
        public static string Scope = "http://api.messagehandler.net/";
    }
}