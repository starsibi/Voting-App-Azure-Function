using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace Voting_App
{
    public static class UpdateVote
    {
        [FunctionName("UpdateVote")]
        public static async void Run([QueueTrigger("votes", Connection = "AzureWebJobsStorage")]string myQueueItem,
             [Table("Votes", Connection = "AzureWebJobsStorage")]CloudTable table,
            ILogger log)
        {
            string type = myQueueItem;
            if (!string.IsNullOrEmpty(type))
            {
                string PartitionKey = string.Empty, RowKey = string.Empty;
                if (string.Equals(type, "Intresting", StringComparison.InvariantCultureIgnoreCase))
                {
                    PartitionKey = RowKey = "1";
                }
                else if (string.Equals(type, "Not Bad", StringComparison.InvariantCultureIgnoreCase))
                {
                    PartitionKey = RowKey = "2";
                }
                else if (string.Equals(type, "Boring", StringComparison.InvariantCultureIgnoreCase))
                {
                    PartitionKey = RowKey = "3";
                }

                TableOperation operation = TableOperation.Retrieve<Votes>(PartitionKey, RowKey);
                TableResult result = await table.ExecuteAsync(operation);
                if (result.Result != null)
                {
                    Votes votes = (Votes)result.Result;
                    votes.count = votes.count + 1;
                    operation = TableOperation.Replace(votes);
                    await table.ExecuteAsync(operation);
                }
            }
        }
    }
}
