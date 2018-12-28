namespace LoadBikePoints
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.WindowsAzure.Storage.Table;
    using System.Collections.Generic;
    using System.Linq;

    public static class BikePointsStoreContext
    {
        public static async Task StoreBikePointsInStore(CloudTable cloudTable, List<BikePoint> bikePoints, TraceWriter tw)
        {
            var skipRows = 0;
            const int takeRows = 50;
            var bikePointBatchGenerator = GenerateBikePointsBatch(bikePoints);
            var bikePointBatch = bikePointBatchGenerator(0, takeRows);
            while (bikePointBatch.Count > 0)
            {
                TableBatchOperation tableOperations = GetTableBatchOperation(bikePointBatch);
                await AddBikePointsToTableStore(cloudTable, tableOperations, tw);
                var rangeIdsString = $"[{bikePointBatch.First().Id} - {bikePointBatch.Last().Id}]";
                tw.Info($"Saved bike points from {rangeIdsString}");
                lock (typeof(object))
                {
                    skipRows += 50;
                    bikePointBatch = bikePointBatchGenerator(skipRows, takeRows);
                }
            }
        }

        private static async Task AddBikePointsToTableStore(CloudTable cloudTable, TableBatchOperation tableOperations, TraceWriter tw)
        {
            try
            {
                await cloudTable.ExecuteBatchAsync(tableOperations);
            }
            catch (Exception error)
            {
                tw.Warning(error.Message);
            }
        }

        private static Func<int, int, List<BikePoint>> GenerateBikePointsBatch(List<BikePoint> bikePoints)
        {
            return (skipRows, takeRows) => bikePoints.Skip(skipRows).Take(takeRows).ToList();
        }

        private static TableBatchOperation GetTableBatchOperation(List<BikePoint> bikePoints)
        {
            var itemsToAdd = new TableBatchOperation();
            foreach (var bikePoint in bikePoints)
            {
                itemsToAdd.Insert(bikePoint);
            }
            return itemsToAdd;
        }

    }
}
