using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LoadBikePoints
{
    public static class OrchestractorClientForLoadBikePoints
    {
        [FunctionName("O_Load_Bike_Points_After_Blob_Trigger")]
        public static async Task<bool> LoadBikePointsAfterBlobTrigger([OrchestrationTrigger]DurableOrchestrationContext ctx,
            TraceWriter tw)
        {

            var xmlInString = ctx.GetInput<string>();

            // Checksum blob.
            var areBikePointsDifferent = true;

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
