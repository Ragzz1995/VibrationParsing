using System;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Globalization;

namespace Vibration
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-idap.infinite-uptime.com/login");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            var AccessToken = "";


            
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
             {
                
                 string json = "{ \"email\": \"titan@titan.com\",\"password\": \"titan@123\" }";

                 streamWriter.Write(json);
             }

             var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

             using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
             {
                 var result = streamReader.ReadToEnd();

                JObject json = JObject.Parse(result);
                 AccessToken = (string)json["accessToken"];

                //Console.WriteLine(json["scopeSelector"]["subOrganizations"]);
                //string Suborganization_JSON_String = json["scopeSelector"]["subOrganizations"];
                var Suborganization_JSON_String = json["scopeSelector"]["subOrganizations"].ToString();
                dynamic Suborganization_JSON_obj = Newtonsoft.Json.JsonConvert.DeserializeObject(Suborganization_JSON_String);

               Console.WriteLine(Suborganization_JSON_obj.Count);
            
             }
        
            Console.WriteLine(AccessToken);


            // GetMonitorByPlant ID 
            var httpWebRequestPlant = (HttpWebRequest)WebRequest.Create("https://api-idap.infinite-uptime.com/api/2.0/admin/getMonitorsByPlantId");
            httpWebRequestPlant.ContentType = "application/json";
            httpWebRequestPlant.Method = "POST";
            httpWebRequestPlant.PreAuthenticate = true;
            httpWebRequestPlant.Headers.Add("Authorization", "Bearer " + AccessToken);


            using (var streamWriter = new StreamWriter(httpWebRequestPlant.GetRequestStream()))
             {
                
                 string json = "{\"id\":201,\"qtype\":2}";

                 streamWriter.Write(json);
             }

             var httpResponsePlant = (HttpWebResponse)httpWebRequestPlant.GetResponse();

             using (var streamReader = new StreamReader(httpResponsePlant.GetResponseStream()))
             {
                 var result = streamReader.ReadToEnd();
                 
                 //JObject json = JObject.Parse(result);
                dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                 Console.WriteLine(obj.Count);
                 
            
             }
            double[] x = {0,0,0,0,0};
            var j = GetEquipmentHistory("201","2021-01-23T08:30:00","2021-01-23T19:30:00");

            foreach(double perc in j)
            {
                Console.WriteLine(perc);
            }

            GetTrendHistory();
        }
    

    static double[] GetEquipmentHistory(string monitorId,string from,string to)
    {
            Console.WriteLine(from);
            var dt_from = DateTime.Parse(from);
            var dt_to   =   DateTime.Parse(to);
            //dt_from = dt_from.AddHours(5.5);
            //dt_to   =   dt_to.AddHours(5.5);
            var dt_from_utc = dt_from.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            var dt_to_utc = dt_to.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            
            

            float[] EquipmentHistoryTime = {0.0f,0.0f,0.0f,0.0f,0.0f};
            double[] EquipmentHistoryPercentage = {0.0f,0.0f,0.0f,0.0f,0.0f};
            double TotalTime = 0.0;

             var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-idap.infinite-uptime.com/api/2.0/md/equipment-history?monitorId="+monitorId+"&from="+dt_from_utc+"&to="+dt_to_utc+"&type=report&direction=1");
             httpWebRequest.ContentType = "application/json";
             httpWebRequest.Method = "GET";
             var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
             //Console.WriteLine("response : ",httpResponse);

             using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
              {
                  var result = streamReader.ReadToEnd();
                 
                  //JObject json = JObject.Parse(result);
                 dynamic EquipmentHistoryobj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                 //Console.WriteLine(EquipmentHistoryobj);
                 foreach(var obj in EquipmentHistoryobj)
                 {
                     int index = (int)obj["state"];
                     EquipmentHistoryTime[index-1] = EquipmentHistoryTime[index-1] + (float)obj["timeDiff"];
                 }           

                 foreach(float TimeDiff in EquipmentHistoryTime)
                 {
                    TotalTime = TotalTime + (double)TimeDiff;
                 }
                int i = 0;
                foreach(var time in EquipmentHistoryTime)
                {
                    EquipmentHistoryPercentage[i] = ((double)time/(double)TotalTime)*100;
                    i = i+1;
                }
                return EquipmentHistoryPercentage;
                 
              }
    }

    static void GetTrendHistory()
    {
        var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-idap.infinite-uptime.com/api/2.0/md/trend-history?monitorId=201&from=2021-01-24T02:30:00Z&to=2021-01-24T12:30:00Z&intervalUnit=minute&intervalValue=1");
             httpWebRequest.ContentType = "application/json";
             httpWebRequest.Method = "GET";
             var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
             Console.WriteLine("History response : ",httpResponse);

             using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
              {
                  var result = streamReader.ReadToEnd();
                 
                  JObject json = JObject.Parse(result);
                 dynamic TrendHistoryObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                 Console.WriteLine(TrendHistoryObj);
              }
    }


    








    }







}
