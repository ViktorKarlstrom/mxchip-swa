using System;
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

namespace MxChip.LoggerApi
{

    public class TableData : TableEntity
    {
        public string Timer { get; set; }
        
        
    }
    public class TaskLogItem : TableEntity
    {
        public string Owner { get; set; }
        public string Name { get; set; }
        public DateTimeOffset StartDate { get; set; }        
        public DateTimeOffset EndDate { get; set; }
    }

    public static class LoggerApi
    {
        [FunctionName("LoggerApi")]
        [return: Table("Log", "uxfu7gvhoe", Connection = "LogDatabaseConnection")]
        public static async Task<TaskLogItem> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Table("Telemetry", "uxfu7gvhoe", Connection = "LogDatabaseConnection")] CloudTable table,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var content = await new StreamReader(req.Body).ReadToEndAsync();

            TaskLogItem taskLogItem = JsonConvert.DeserializeObject<TaskLogItem>(content);

            TableQuery<TableData> query = table.CreateQuery<TableData>();
            query.TakeCount = 2;

            var logRows = (await table.ExecuteQuerySegmentedAsync(query, null)).ToList();

            if (logRows.Any())
            {

                for (var i = 0; i < 2; i++)
                {
                    if(logRows[i].Timer == "true") 
                    {
                        taskLogItem.StartDate = logRows[i].Timestamp;
                    } 
                    else 
                    {
                        taskLogItem.EndDate = logRows[i].Timestamp;
                    } 
                }

                taskLogItem.PartitionKey = "uxfu7gvhoe";
                taskLogItem.RowKey = $"{(DateTimeOffset.MaxValue.Ticks - logRows[0].Timestamp.Ticks):d10}-{Guid.NewGuid():N}";

                return taskLogItem;
            }
            else 
            {
                return null;
            }
        }
    }
}
