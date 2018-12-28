namespace LoadBikePoints
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class BikePointOrchestrator
    {
        
        [FunctionName("O_Load_BikePoints")]
        public static async Task<bool> InitializeDataLoadTask([OrchestrationTrigger]DurableOrchestrationContext ctx,
            TraceWriter tw)
        {
            var settings = ctx.GetInput<string>();

            // Download xml.
            if (!ctx.IsReplaying) tw.Info($"starting downloading the task from : {settings}");
            string xmlInString = await ctx.CallActivityAsync<string>("A_Download_Asset", settings);

            // Checksum blob.
            var areBikePointsDifferent = await ctx.CallActivityAsync<bool>("A_Is_BikePoints_Different", xmlInString);

            // Reset Store
            await ctx.CallActivityAsync("A_Refresh_BikePoint_Store", areBikePointsDifferent);


            // Extract and serialize data
            if (!ctx.IsReplaying) tw.Info("Deserializing xml in to bike points");
            List<BikePoint> bikePoints = await ctx.CallActivityAsync<List<BikePoint>>("A_Deserialize_Xml_Response", xmlInString);

            // Save bike points into store
            if (areBikePointsDifferent)
            {
                bool bikePointSavedInStore = await ctx.CallActivityAsync<bool>("A_Put_BikePoints_In_Store", bikePoints);
                return bikePointSavedInStore;
            }

            return false;
        }
    }
}
