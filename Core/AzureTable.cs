using System;
using System.Linq;
using System.Collections.Generic;
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
    /// </summary>
    internal class AzureTable
    {
        /// <summary></summary>
        public string TableName { get; set; }
    }
}