using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace microservice_lingk_leadsquare_bdm_util.Helper
{
    public class Program
    {
        public String CreateBatch(List<DocumentInfo> documentDatas)
        {
            Console.WriteLine("Hello World!");
            try
            {
                //    string data = "{\"TargetDoc\": {\"ID\": 0},	\"NewIndex\": {\"indexid\": 0,\"values\": [{\"FieldID\": \"Banner ID\",\"FieldValue\": \"UG018214\"},{\"FieldID\": \"BDM Document Type\",\"FieldValue\": \"HIGH SCHOOL TRNSCRPT\"},{\"FieldID\": \"Term\",\"FieldValue\": \"202208\"},			{				\"FieldID\": \"Banner Application Number\",				\"FieldValue\": \"1\"			},			{				\"FieldID\": \"Banner Checklist Item\",				\"FieldValue\": \"GED\"			}		]	},	\"FromBatch\": {		\"ID\": 570	},	\"BatchPageNum\": 0,	\"IngoreDuplicateIndex\": false,	\"IgnoreDlsViolation\": false}";
                //    //var documentContent = new MultipartFormDataContent("--85b890d4-4faf-4261-bcbb-187c4dddcbc6");
                //    //documentContent.Add(new StringContent(data, System.Text.Encoding.UTF8, "application/vnd.emc.ax+json"), "data");

                //    var documentContent = new StringContent(data, System.Text.Encoding.UTF8, "application/vnd.emc.ax+json");

                //    var hhanlder = new HttpClientHandler() { };
                //    hhanlder.Credentials = new NetworkCredential("LINGK", "hBHakmSKGjsYwzUzPVm6");


                //    using (var client = new HttpClient(hhanlder))
                //    {
                //        //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //        var response = client.PostAsync(@"https://bdmtest.nec.edu/AppXtenderReST/api/axdatasources/NECPPRD/axdocs/509", documentContent).Result;
                //        var responseContent = response.Content.ReadAsStringAsync().Result;
                //        Console.Write(responseContent);
                //    }

                //string data = "{\"Name\": \"UG - Online\",\"Description\": \"UG - Online\", \"Private\": false}";

                string data = "{\"Name\": \"batch name\",\"Description\": \"batch description\", \"Private\": false}";
                var documentContent = new MultipartFormDataContent("--85b890d4-4faf-4261-bcbb-187c4dddcbc6");

                // documentContent.Add(new StringContent("AnalyticsPage.xlsx"), "title");
                documentContent.Add(new StringContent(data, System.Text.Encoding.UTF8, "application/vnd.emc.ax+json"), "data");
                documentContent.Add(new ByteArrayContent(System.IO.File.ReadAllBytes("78d21cc5.jpeg")), "New", "78d21cc5.jpeg");
                    
                //documentContent.Add(new ByteArrayContent(File.ReadAllBytes("bfb3e279.jpeg")), "bin", "bfb3e279.jpeg");
                //documentContent.Add(new ByteArrayContent(File.ReadAllBytes("bfb3e279.jpeg")), "bin", "bfb3e279.jpeg");

                var hhanlder = new HttpClientHandler() { };
                hhanlder.Credentials = new NetworkCredential("LINGK", "hBHakmSKGjsYwzUzPVm6");

                JObject finalDocumentApiResult = new JObject();
                using (var client = new HttpClient(hhanlder))
                {
                    var response = client.PostAsync(@"https://bdmtest.nec.edu/AppXtenderReST/api/axdatasources/NECPPRD/axbatches/509", documentContent).Result;
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    finalDocumentApiResult = (JObject)JsonConvert.DeserializeObject(responseContent);
                }
                string batchId = finalDocumentApiResult["ID"].ToString();
                return batchId;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Console.Read();
                return ex.ToString();
            }
        }
    }
}
