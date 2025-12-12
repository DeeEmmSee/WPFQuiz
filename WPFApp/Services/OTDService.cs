using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WPFApp.Entities;

namespace WPFApp.Services
{
    public class OTDService
    {
        string BaseURL = "https://opentdb.com/api.php";
        public async Task<OTDQuestion[]> GetQuestions(int amount = 10)
        {
            OTDQuestion[] questions = [];

            try
            {
                HttpClient client = new HttpClient();
                // Headers go here

                HttpResponseMessage response = await client.GetAsync(BaseURL + "?amount=" + amount);

                response.EnsureSuccessStatusCode();
                
                string strRes = await response.Content.ReadAsStringAsync();

                OTDServiceResponse? resObj = JsonConvert.DeserializeObject<OTDServiceResponse>(strRes);

                if (resObj != null)
                    questions = resObj.results;
            }
            catch (Exception ex)
            {
                // Error Logging (log4net)
            }

            return questions;
        }
    }
}
