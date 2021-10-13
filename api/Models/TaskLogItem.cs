using System;
using Microsoft.Azure.Cosmos.Table;

namespace MxChip.Api.Models
{
    public class TaskLogItem : TableEntity
    {
        public string Owner { get; set; }
        public string Name { get; set; }
        public DateTimeOffset StartDate { get; set; }        
        public DateTimeOffset EndDate { get; set; }
    }
}
