using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace Gateway.Rest.AcceptanceTests
{
    [TestFixture]
    public class can_send_parallel_batches_using_http_and_json
    {
        private readonly IAuthorize _auth = new WithClientCredentials();

        [Test]
        public void using_rest_sharp()
        {
            //just increase k to see how far we can go
            for (var k = 0; k < 100; k += 10)
            {
                Console.WriteLine("Sending " + k * 1000 + " messages");

                ServicePointManager.DefaultConnectionLimit = k + 2;
                ServicePointManager.MaxServicePointIdleTime = 2500;
                var maxConcurrentThingsToProcess = new Semaphore(ServicePointManager.DefaultConnectionLimit, ServicePointManager.DefaultConnectionLimit);


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

                for (var j = 0; j < k; j++)
                {
                    maxConcurrentThingsToProcess.WaitOne();

                    try
                    {

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

                        endPointClient.ExecuteTaskAsync(request).ContinueWith(
                            task =>
                            {
                                var r = task.Result;
                                
                                maxConcurrentThingsToProcess.Release();

                                Assert.IsTrue(r.StatusCode == HttpStatusCode.OK);
                            });

                        
                    
                    }
                    catch (Exception)
                    {
                        maxConcurrentThingsToProcess.Release();

                        throw;
                    }
                    
                }
            }
        }
    }

    namespace RestSharpEx
    {
        public static class RestClientExtensions
        {
            public static Task<IRestResponse> ExecuteTaskAsync(this RestClient c, RestRequest request)
            {
                if (c == null)
                    throw new NullReferenceException();

                var tcs = new TaskCompletionSource<IRestResponse>();

                c.ExecuteAsync(request, (response) =>
                {
                    if (response.ErrorException != null)
                        tcs.TrySetException(response.ErrorException);
                    else
                        tcs.TrySetResult(response);
                });

                return tcs.Task;
            }
        }
    }
}