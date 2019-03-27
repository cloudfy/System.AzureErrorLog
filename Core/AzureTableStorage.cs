using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace System.AzureErrorLog.Core
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class AzureTableStorage
    {
        #region === member variables ===
        private string Key { get; set; }
        private string Account { get; set; }
        #endregion

        #region === constructor ===
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="account"></param>
        public AzureTableStorage(string key, string account)
        {
            this.Key = key;
            this.Account = account;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public AzureTableStorage(IAzureErrorLogSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("Settings");
            if (string.IsNullOrEmpty(settings.AzureAccount))
                throw new ArgumentNullException("AzureAccount");
            if (string.IsNullOrEmpty(settings.AzureKey))
                throw new ArgumentNullException("AzureKey");

            this.Key = settings.AzureKey;
            this.Account = settings.AzureAccount;
        }
        #endregion

        #region === private methods ===
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canonicalizedString"></param>
        /// <returns></returns>
        private String CreateAuthorizationHeader(String canonicalizedString)
        {
            String signature = String.Empty;

            using (HMACSHA256 hmacSha256 = new HMACSHA256(Convert.FromBase64String(this.Key)))
            {
                Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(canonicalizedString);
                signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
            }

            String authorizationHeader = String.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}:{2}",
                Constants.SharedKeyAuthorizationScheme,
                this.Account,
                signature
            );

            return authorizationHeader;
        }
        #endregion

        #region === private properties ===
        /// <summary>
        /// 
        /// </summary>
        private string TableEndPoint
        {
            get
            {
                return string.Format(Constants.TableEndPoint, this.Account);
            }
        }
        #endregion

        #region === create methods ===
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool CreateEntry(AzureErrorLogEntry entry, string tableName)
        {
            string body = JsonConvert.SerializeObject(entry);
            string url = tableName;
            var data = Execute("POST", url, body);
            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool CreateTableIfNotExist(string tableName)
        {
            var count = ListTables().Count(
                t => t.TableName.ToLower() == tableName.ToLower());

            if (count > 0)
                return false;

            CreateTable(tableName);
            return true;
        } 
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        public void CreateTable(string tableName)
        {
            AzureTable table = new AzureTable { TableName = tableName };
            Execute("POST", "Tables", JsonConvert.SerializeObject(table));
        }
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public AzureTable[] ListTables()
        {
            string response = ListEntity("Tables()");

            return JsonConvert.DeserializeObject<AzureTableCollection>(response).Value;
        }
        /// <summary>
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string ListEntity(String tableName)
        {
            return Execute("GET", tableName);
        }

        private string Execute(string requestMethod, String urlPath, string body)
        {
            // write body as UTF-8 encoding
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(body);

            String dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            String canonicalizedResource = String.Format("/{0}/{1}", Account, urlPath);

            //String stringToSign 
            // = requestMethod + "\n" +
            //   "\n" +
            //   "\n" +
            //   dateInRfc1123Format + "\n" +
            //   canonicalizedResource;

            //String stringToSign = 
            //    requestMethod + "\n" + /*HTTP Verb*/
            //    "\n" + /*Content-Encoding*/
            //    "\n" + /*Content-Language*/
            //    "\n" + /*Content-Length (include value when zero)*/
            //    "\n" + /*Content-MD5*/
            //    "\n" + /*Content-Type*/
            //    dateInRfc1123Format + "\n" +
            //    canonicalizedResource;

            String stringToSign = String.Format(
                "{0}\n{1}\n{2}\n{3}\n{4}",
                requestMethod,
                string.Empty,
                "application/json",
                dateInRfc1123Format,
                canonicalizedResource);

            String authorizationHeader = CreateAuthorizationHeader(stringToSign);
            
            Uri uri = new Uri(this.TableEndPoint + urlPath);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = requestMethod;
            request.Headers.Add("x-ms-date", dateInRfc1123Format);
            request.Headers.Add("x-ms-version", Constants.StorageServiceVersion);
            request.Headers.Add("Authorization", authorizationHeader);
            request.Headers.Add("Accept-Charset", "UTF-8");
            request.ContentType = "application/json";
            request.Accept = Constants.RequestAccept;

            if (request.Method == "POST" | request.Method == "PUT")
            {
                // set content length
                request.ContentLength = bytes.Length;

                using (Stream requestStream = request.GetRequestStream())
                    requestStream.Write(bytes, 0, bytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="requestMethod"></param>
        /// <param name="urlPath"></param>
        /// <returns></returns>
        private string Execute(string requestMethod, String urlPath)
        {
            String dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            String canonicalizedResource = String.Format("/{0}/{1}", Account, urlPath);

            //String stringToSign = requestMethod + "\n" +
            //   "\n" +
            //   "\n" +
            //   dateInRfc1123Format + "\n" +
            //   canonicalizedResource;
            String stringToSign = requestMethod + "\n" +
               "\n" +
               "\n" +
               dateInRfc1123Format + "\n" +
               canonicalizedResource;

            String authorizationHeader = CreateAuthorizationHeader(stringToSign);

            Uri uri = new Uri(this.TableEndPoint + urlPath);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = requestMethod;
            request.Headers.Add("x-ms-date", dateInRfc1123Format);
            request.Headers.Add("x-ms-version", Constants.StorageServiceVersion);
            request.Headers.Add("Authorization", authorizationHeader);
            request.Headers.Add("Accept-Charset", "UTF-8");
            request.Accept = Constants.RequestAccept;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
