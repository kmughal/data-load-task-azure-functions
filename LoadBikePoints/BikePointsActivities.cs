namespace LoadBikePoints
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using System.Net.Http;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.WindowsAzure.Storage.Table;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.WindowsAzure.Storage.Blob;


    public static class BikePointsActivities
    {
        [FunctionName("A_Is_BikePoints_Different")]
        public static async Task<bool> BikePointsBlobIsDifferent([ActivityTrigger] string newXml,
         [Blob("refdata/bike-points.xml", Connection = "AzureWebJobsStorage")]CloudBlobContainer cloudBlobContainer)
        {
            bool fileExits = await cloudBlobContainer.ExistsAsync();
            const string resourcename = "bike-points.xml";

            if (fileExits)
            {
                var reference = cloudBlobContainer.GetBlockBlobReference(resourcename);
                MemoryStream stream = new MemoryStream();
                await reference.DownloadToStreamAsync(stream);
                var oldXml = Encoding.UTF8.GetString(stream.ToArray());
                if (oldXml != newXml)
                {
                    await cloudBlobContainer.DeleteAsync();
                    await cloudBlobContainer.CreateAsync();
                    reference = cloudBlobContainer.GetBlockBlobReference(resourcename);
                    await reference.UploadTextAsync(newXml);
                    return true;
                }
            }
            else
            {
                await cloudBlobContainer.CreateAsync();
                var reference = cloudBlobContainer.GetBlockBlobReference(resourcename);
                await reference.UploadTextAsync(newXml);
                return true;
            }

            return default(bool);
        }


        [FunctionName("A_Put_BikePoints_In_Store")]
        public static async Task<bool> PutBikePointsInStore([ActivityTrigger] List<BikePoint> bikePoints,
             [Table("bikepoints", Connection = "AzureWebJobsStorage")] CloudTable cloudTable,
            TraceWriter tw)
        {
            tw.Info($"Writing {bikePoints.Count} in the store.");
            await BikePointsStoreContext.StoreBikePointsInStore(cloudTable, bikePoints, tw);
            tw.Info($"Completed writing {bikePoints.Count} in the store.");
            return true;
        }

        [FunctionName("A_Download_Asset")]
        public static async Task<string> DownloadAsset([ActivityTrigger] string url, TraceWriter tw)
        {
            var httpClient = new HttpClient();
            var request = await httpClient.GetAsync(url);
            string xmlInString = await request.Content.ReadAsStringAsync();
            tw.Info($"Xml response received from  {url}");
            return xmlInString;
        }

        [FunctionName("A_Refresh_BikePoint_Store")]
        public static async Task ResetBikePointStore(
            [ActivityTrigger] bool areBikePointsDifferent,
            [Table("bikepoints", Connection = "AzureWebJobsStorage")] CloudTable cloudTable, TraceWriter tw)
        {
            if (!areBikePointsDifferent) { tw.Info("bike points are same so skipping A_Refresh_BikePoint_Store"); return; }

            tw.Info("Start resetting Bike Point stores");
            await cloudTable.DeleteIfExistsAsync();
            await cloudTable.CreateIfNotExistsAsync();
            tw.Info("Reset Bike Point stores completed.");
        }

        [FunctionName("A_Deserialize_Xml_Response")]
        public static List<BikePoint> ExtractBikePointsOutOfXml([ActivityTrigger]string xmlInString, TraceWriter tw)
        {
            List<BikePoint> bikePoints = BikePointsGenerator.CreateBikePoints(xmlInString, tw);
            return bikePoints;
        }
    }
}
