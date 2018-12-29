namespace LoadBikePoints
{
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Blob;


    public static class LoadBikePointsFromNewFileUpload
    {
        [FunctionName("LoadBikePointsFromNewFileUpload")]
        public static async Task Run(
            [BlobTrigger("refdata/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, 
            [Blob("refdata/{name}",Connection ="AzureWebJobsStorage")] CloudBlobContainer cloudBlobContainer,
            [OrchestrationClient] DurableOrchestrationClient client,
            string name, 
            ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            string xml = await StreamHelpers.ReadXmlContentsFromStreamAsync(myBlob);

             await client.StartNewAsync("O_Load_Bike_Points_After_Blob_Trigger", xml);
        }
    }
}
