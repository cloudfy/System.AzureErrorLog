using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.AzureErrorLog
{
    /// <summary>
    /// A common interface for providing settings.
    /// </summary>
    public interface IAzureErrorLogSettings
    {
        /// <summary>Gets the Azure Shared Secret Key.</summary>
        string AzureKey { get; }
        /// <summary>Gets the Azure account name.</summary>
        string AzureAccount { get; }
        /// <summary>Gets the Log name name.</summary>
        string LogName { get; }
    }
}