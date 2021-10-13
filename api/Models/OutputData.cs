using System;
using MxChip.LoggerApi;

namespace MxChip.Api.Models
{
    public class OutputData : TaskLogItem
    {
        public TimeSpan MinutesSpent { get; set; }        
    }
}
