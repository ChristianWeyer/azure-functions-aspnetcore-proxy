using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using SampleWebApiAspNetCore;

namespace WebApiProxyFunctionApp
{
    public static class WebApiProxy
    {
        private static readonly HttpClient Client;

        static WebApiProxy()
        {
            var functionPath = new FileInfo(typeof(WebApiProxy).Assembly.Location).Directory.Parent.FullName;
            Directory.SetCurrentDirectory(functionPath);
            var server = CreateServer(functionPath);
            Client = server.CreateClient();
        }

        private static TestServer CreateServer(string functionPath)
        {
            return new TestServer(WebHost
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config
                        .SetBasePath(functionPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{builderContext.HostingEnvironment.EnvironmentName}.json",
                            optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();
                })
                .UseStartup<Startup>()
                .UseContentRoot(functionPath));
        }

        [FunctionName("WebApiProxy")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get", "post", "put", "patch", "options",
                Route = "{*x:regex(^(?!admin|debug|monitoring).*$)}")] HttpRequestMessage req,
            TraceWriter log)
        {
            log.Info("***HTTP ASP.NET Core Web API Proxy: trigger function processed a request.");

            var response = await Client.SendAsync(req);

            return response;
        }
    }
}