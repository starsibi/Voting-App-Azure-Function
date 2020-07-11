using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using Microsoft.AspNetCore.Mvc;

namespace Voting_App
{
    public static class AddVote
    {
        [FunctionName("AddVote")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        [Table("Votes", Connection = "AzureWebJobsStorage")]CloudTable table,
        ILogger log)
        {
            string type = req.Query["type"];
            if (!string.IsNullOrEmpty(type))
            {
                string PartitionKey = string.Empty, RowKey = string.Empty;
                if(string.Equals(type,"Intresting", StringComparison.InvariantCultureIgnoreCase))
                {
                    PartitionKey = RowKey = "1";
                }
                else if(string.Equals(type, "Not Bad", StringComparison.InvariantCultureIgnoreCase))
                {
                    PartitionKey = RowKey = "2";
                }
                else if(string.Equals(type, "Boring", StringComparison.InvariantCultureIgnoreCase))
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

            return type != null
                ? (ActionResult)new OkObjectResult("Thanks for the Vote...")
                : new BadRequestObjectResult("Bad Request");
        }
    }
}


public class Votes: TableEntity
{
    public int id { get; set; }
    public string type { get; set; }
    public int count { get; set; }
}