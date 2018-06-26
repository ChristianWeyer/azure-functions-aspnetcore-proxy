using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace AspNetCoreProxyFunctionApp
{
    public static class KeepItWarm
    {
        [FunctionName("KeepItWarm")]
        public static void Run([TimerTrigger("0 */9 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
