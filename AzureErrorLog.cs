using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.AzureErrorLog.Core;

namespace System.AzureErrorLog
{
    /// <summary>
    /// </summary>
    /// <remarks></remarks>
    public abstract class AzureErrorLog
    {
        #region === member variables ===
        /// <summary></summary>
        private readonly IAzureErrorLogSettings settings = null;
        /// <summary></summary>
        private readonly AzureTableStorage Storage = null;
        #endregion

        #region === constructor ===
        /// <summary>
        /// </summary>
        /// <param name="settings"></param>
        public AzureErrorLog(IAzureErrorLogSettings settings)
        {
            this.settings = settings;
            Storage = new AzureTableStorage(this.settings);
        } 
        #endregion

        #region === private properties ===       
        /// <summary>
        /// 
        /// </summary>
        private string LogName
        {
            get
            {
                string value = settings.LogName;
                if (string.IsNullOrEmpty(value))
                    value = "Errors";

                return value;
            }
        }
        #endregion

        #region === public methods ===
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public void Trace<T>(T value) where T : AzureErrorLogEntry
        {
            if (string.IsNullOrEmpty(settings.AzureKey))
                throw new ArgumentNullException();
            if (string.IsNullOrEmpty(settings.AzureAccount))
                throw new ArgumentNullException();

            try
            {
                Storage.CreateTableIfNotExist(LogName);                
                Storage.CreateEntry(value, LogName);
            }
            catch
            {
                // nothing
            }
        }
        #endregion
    }
}