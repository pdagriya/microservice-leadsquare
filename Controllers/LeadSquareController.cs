using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using microservice_lingk_leadsquare_bdm_util.Helper;
using Microsoft.Extensions.Logging;
using System.Net;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace microservice_lingk_leadsquare_bdm_util.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadSquareController : ControllerBase
    {
        ILogger Logger { get; set; }

        //private readonly string boundary = Environment.GetEnvironmentVariable("BOUNDARY");
        //private readonly string networkUserName = Environment.GetEnvironmentVariable("NATWORKUSERNAME");
        //private readonly string networkPassword = Environment.GetEnvironmentVariable("NETWORKPASSWORD");
        //private readonly string appXtenderInstanceURI = Environment.GetEnvironmentVariable("APPXTENDER_INSTANCE_URI");
        //private readonly string appXtenderBatchEndPoint = Environment.GetEnvironmentVariable("APPXTENDER_BATCH_ENDPOINT");
        //private readonly string appXtenderIndexEndPoint = Environment.GetEnvironmentVariable("APPXTENDER_INDEX_ENDPOINT");
        //private readonly string appXtenderAppId = Environment.GetEnvironmentVariable("APPXTENDER_APP_ID");

        private readonly string boundary = "--85b890d4-4faf-4261-bcbb-187c4dddcbc6";
        private readonly string networkUserName = "LINGK";
        private readonly string networkPassword = "hBHakmSKGjsYwzUzPVm6";
        private readonly string appXtenderInstanceURI = @"https://bdmtest.nec.edu/AppXtenderReST/api/axdatasources/NECPPRD";
        private readonly string appXtenderBatchEndPoint = "/axbatches/";
        private readonly string appXtenderIndexEndPoint = "/axdocs/";
        private readonly string appXtenderAppId = "509";

        public LeadSquareController(ILogger<LeadSquareController> logger)
        {
            Logger = logger;
        }

        // POST
        [HttpPost]
        [Route("document")]
        public object PostDocumentToBanner(ApplicationInfo appInfo)
        {
            try
            {
                JArray apiResultArray = new JArray();
                var documentInfoList = new List<DocumentInfo>();
                foreach (var records in appInfo.DocumentData)
                {
                    Logger.LogInformation("Requested document URL: {0}", records.DocumentURL);
                    // Create http get request to the url and convert response into byte array
                    WebRequest RequestObj = WebRequest.Create(records.DocumentURL);
                    RequestObj.Method = "GET";

           
                    byte[] myBinaryResponse = new byte[0];
                    using (HttpWebResponse ResponseObj = (HttpWebResponse)RequestObj.GetResponse())
                    {
                        using (Stream stream = ResponseObj.GetResponseStream())
                        {
                            MemoryStream ms = new MemoryStream();
                            stream.CopyTo(ms);
                            myBinaryResponse = ms.ToArray();
                        }
                    }

                    var docInfo = new DocumentInfo();
                    docInfo.DocumentByteData = myBinaryResponse;
                    docInfo.DocumentDisplayName = records.DocumentDisplayName;
                    docInfo.DocumentName = records.DocumentName;
                    documentInfoList.Add(docInfo);
                }

                // Create new batch in banner 
                Program batch = new Program();
                var newBatchId = batch.CreateBatch(documentInfoList);

                // Create new index for created batch
                JObject newIndexResponse = new JObject();
                if (!string.IsNullOrEmpty(newBatchId))
                {
                    newIndexResponse = CreateIndexInBanner(newBatchId, appInfo); 
                }
                else
                {
                    throw new Exception("Batch not created in banner");
                }

                ResponseModel res = new ResponseModel();
                res.Code = 200;
                res.Message = "Success";
                res.Details = newIndexResponse;
                return JsonConvert.SerializeObject(res);

            }
            catch (Exception ex)
            {
                ResponseModel res = new ResponseModel();
                res.Code = 500;
                res.Message = ex.Message;
                res.Details = appInfo;
                return JsonConvert.SerializeObject(res);
            }

        }

        /// <summary>
        /// This method will create batch in banner
        /// </summary>
        /// <param name="documentDatas"></param>
        /// <param name="appInfo"></param>
        /// <returns></returns>
        private string CreateBatchInBanner(List<DocumentInfo> documentDatas, ApplicationInfo appInfo)
        {
            //string data = "{\"Name\": \"batch name\",\"Description\": \"batch description\", \"Private\": false}";
            string data = "{\"Name\": \"" + appInfo.BatchName + "\",\"Description\": \"" + appInfo.BatchDescription + "\",\"Private\": false}";
            var documentContent = new MultipartFormDataContent(boundary);

            // Convert data into http string content
            documentContent.Add(new StringContent(data, System.Text.Encoding.UTF8, "application/vnd.emc.ax+json"), "data");

            // Loop to add all documents into document content
            foreach (var info in documentDatas)
            {
                documentContent.Add(new ByteArrayContent(info.DocumentByteData), info.DocumentDisplayName, info.DocumentName);
            }

            var hhanlder = new HttpClientHandler() { };
            hhanlder.Credentials = new NetworkCredential(networkUserName, networkPassword);

            JObject finalDocumentApiResult = new JObject();
            using (var client = new HttpClient(hhanlder))
            {
                var response = client.PostAsync(appXtenderInstanceURI + appXtenderBatchEndPoint + appXtenderAppId, documentContent).Result;
                var responseContent = response.Content.ReadAsStringAsync().Result;
                finalDocumentApiResult = (JObject)JsonConvert.DeserializeObject(responseContent);
            }

            string batchId = finalDocumentApiResult["ID"].ToString();
            return batchId;
        }


        /// <summary>
        /// This method will create index in banner
        /// </summary>
        /// <param name="newBatchId"></param>
        /// <param name="appInfo"></param>
        /// <returns></returns>
        private JObject CreateIndexInBanner(string newBatchId, ApplicationInfo appInfo)
        {
            string data = "{\"TargetDoc\": {\"ID\": 0},	\"NewIndex\": {	\"indexid\": 0,	\"values\": [{\"FieldID\": \"Banner ID\",\"FieldValue\": \" " + appInfo.BannerID + 
                          "\"},{\"FieldID\": \"BDM Document Type\",	\"FieldValue\": \" "+ appInfo.BDMDocumentType +
                          "\"},	{\"FieldID\": \"Term\",	\"FieldValue\": \" "+ appInfo.Term +
                          " \"},{\"FieldID\": \"Banner Application Number\",\"FieldValue\": \" "+ appInfo.BannerApplicationNumber +
                          "\"},{\"FieldID\": \"Banner Checklist Item\",\"FieldValue\": \" "+ appInfo.BannerChecklistItem +
                          " \"}]},\"FromBatch\": {\"ID\": " + newBatchId + 
                          "},\"BatchPageNum\": 0,	\"IngoreDuplicateIndex\": false,	\"IgnoreDlsViolation\": false}";
            
            
            var documentContent = new MultipartFormDataContent(boundary);
            documentContent.Add(new StringContent(data, System.Text.Encoding.UTF8, "application/vnd.emc.ax+json"), "data");

            var hhanlder = new HttpClientHandler() { };
            hhanlder.Credentials = new NetworkCredential(networkUserName, networkPassword);

            JObject finalDocumentApiResult = new JObject();
            using (var client = new HttpClient(hhanlder))
            {
                var response = client.PostAsync(appXtenderInstanceURI + appXtenderIndexEndPoint + appXtenderAppId, documentContent).Result;
                var responseContent = response.Content.ReadAsStringAsync().Result;
                finalDocumentApiResult = (JObject)JsonConvert.DeserializeObject(responseContent);
            }

            return finalDocumentApiResult;
        }
    }
}
