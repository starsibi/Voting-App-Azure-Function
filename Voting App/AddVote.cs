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
        [Queue("votes")] IAsyncCollector<string> voteQueue,
        ILogger log)
        {
            string type = req.Query["type"];
            await voteQueue.AddAsync(type);

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