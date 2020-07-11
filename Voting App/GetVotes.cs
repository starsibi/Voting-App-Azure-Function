using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Collections.Generic;

namespace Voting_App
{
    public static class GetVotes
    {        
        [FunctionName("GetVotes")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,            
            ILogger log)
        {
            log.LogInformation("GetVotes Method gets started...");
            var acc = new CloudStorageAccount(
                         new StorageCredentials("storageaccountvotinad34", "wkvYBAxb+fKwsQVt/X4lNPVVF8IeFq9HJJjmvIYNUzlXYjM1/pslq339TVhzot+tQGZrC1pETDKPzWQXnY9Uyg=="), true);
            var tableClient = acc.CreateCloudTableClient();
            var table = tableClient.GetTableReference("Votes");
            TableContinuationToken token = null;
            List<Votes> entities = new List<Votes>();
            do
            {
                var queryResult = table.ExecuteQuerySegmentedAsync(new TableQuery<Votes>(), token);
                entities.AddRange(queryResult.Result);
                //token = queryResult.ContinuationToken;
            } while (token != null);

            return new JsonResult(entities);
        }

    }
}
