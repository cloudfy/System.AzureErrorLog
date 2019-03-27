using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace System.AzureErrorLog.Core
{
    /// <summary>
    /// Provides a collection of azure tables.
    /// </summary>
    internal class AzureTableCollection
    {
        /// <summary>
        /// Collection of tables.
        /// </summary>
        public AzureTable[] Value { get; set; }
    }

}
