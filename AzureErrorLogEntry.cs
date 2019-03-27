using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.AzureErrorLog.Core;

namespace System.AzureErrorLog
{
    /// <summary>
    /// </summary>
    public abstract class AzureErrorLogEntry
    {
        /// <summary>
        /// </summary>
        public AzureErrorLogEntry()
        {
            this.RowKey = DateTime.UtcNow.ToFileTimeUtc().ToString();
            this.PartitionKey = System.Environment.MachineName;
        }
        /// <summary></summary>
        public string PartitionKey { get; private set; }
        /// <summary></summary>
        public string RowKey { get; private set; }
        /// <summary></summary>
        public string Message { get; set; }
        /// <summary></summary>
        public string ExceptionDetails { get; set; }
    }    
}