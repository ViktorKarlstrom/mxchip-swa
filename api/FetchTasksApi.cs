using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using System.Linq;
using MxChip.Api.Models;

namespace MxChip.FetchTasksApi
{
    public static class FetchTasksApi
    {
        [FunctionName("FetchTasksApi")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Table("TaskLog", "uxfu7gvhoe", Connection = "LogDatabaseConnection")] CloudTable table,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var content = await new StreamReader(req.Body).ReadToEndAsync();

            InputOwner inputOwner = JsonConvert.DeserializeObject<InputOwner>(content);
            
            TableQuery<OutputData> query = table.CreateQuery<OutputData>();
   
            var tasks = (await table.ExecuteQuerySegmentedAsync(query, null)).ToList().Where(user => user.Owner == inputOwner.owner);

            foreach (var task in tasks)
            {
                task.MinutesSpent = task.EndDate - task.StartDate;
                task.StartDate = task.StartDate.AddHours(2);
            }

            return new OkObjectResult(tasks.OrderBy(task => task.StartDate));
        }
    }
}
