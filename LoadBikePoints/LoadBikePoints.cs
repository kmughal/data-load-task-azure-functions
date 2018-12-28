using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace LoadBikePoints
{
    public static class LoadBikePoints
    {
        [FunctionName("LoadBikePoints")]
        public static async Task<HttpResponseMessage> Run(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestMessage req,
           [OrchestrationClient] DurableOrchestrationClient client,
           ILogger log)
        {
            ITaskSettings settings = new BikePointTaskSettings();
            var instanceId = await client.StartNewAsync("O_Load_BikePoints", settings.Url);
            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}

