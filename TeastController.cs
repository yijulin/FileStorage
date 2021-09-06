using _20210907.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;

namespace _20210907.APIControllers
{
    public class TeastController : ApiController
    {
        static string path = $@"{System.Environment.CurrentDirectory}\data";
        static string filepath = $@"{path}\testdata.txt";
        // GET api/<controller>/5
        [HttpGet]
        public ApiResult GetData(string url,string way)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var result = "";
                //獲取內容
                using(var resder = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    result = resder.ReadToEnd();
                }

                //檢查目標路徑是否有對應資料夾
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Console.WriteLine("建立data資料夾");
                }
                //建立檔案
                using (FileStream file = File.Create(filepath))
                {
                    byte[] txt = new UTF8Encoding(true).GetBytes(result);
                    file.Write(txt, 0, txt.Length);
                    Console.WriteLine("建立testdata.txt");
                }
                
                var data = (way=="Json") ? jsonRead():XMLRead();

                return new ApiResult(ApiStatus.Success, string.Empty, data);
            }
            catch(Exception ex)
            {
                return new ApiResult(ApiStatus.Fail, ex.Message, null);
            }
        }

        public ApiResult jsonRead()
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                StreamReader reader = new StreamReader(filepath,System.Text.Encoding.UTF8);
                var data = reader.ReadToEnd().Trim('[',']');

                JObject jObject = JObject.Parse(data);
                var result = jObject["result"]["records"];
                
                apiResult.Status = ApiStatus.Success;
                apiResult.Result = result;
                reader.Close();
            }
            catch(Exception ex)
            {
                apiResult.Status = ApiStatus.Fail;
                apiResult.ErrorMessage = ex.Message;
            }
            return apiResult;
        }



        public ApiResult XMLRead()
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                StreamReader reader = new StreamReader(filepath, System.Text.Encoding.UTF8);
                var data = reader.ReadToEnd();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(data);
                string jsonText = JsonConvert.SerializeXmlNode(xmlDoc);

                JObject jObject = JObject.Parse(jsonText);
                var result = jObject["DocumentElement"]["DataA53000000A-000003-002"];

                apiResult.Status = ApiStatus.Success;
                apiResult.Result = result;
                reader.Close();
            }
            catch (Exception ex)
            {
                apiResult.Status = ApiStatus.Fail;
                apiResult.ErrorMessage = ex.Message;
            }
            return apiResult;
        }

    }
}