using Microsoft.Azure.Cosmos.Table;

namespace MxChip.Api.Models
{
    public class TableData : TableEntity
    {
        public string Timer { get; set; }    
    }
}
