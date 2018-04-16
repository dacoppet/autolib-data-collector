#r "Newtonsoft.Json"

using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public static async Task Run(TimerInfo myTimer, ICollector<string> outputEventHubMessage, TraceWriter log)
{
    log.Info($"Data Intragrator Triggered at: {DateTime.Now}");
    log.Info("Starting integration of Autolib Data...");
    
    // Retrieve from Rest API
    List<Station> stations = await GetStations();

    log.Info("Total Station infos received:"+ stations.Count);

    log.Info("Sending data to Event Hub...");
    
    // Send to EventHub
    foreach(var item in stations)
    {
        var json = JsonConvert.SerializeObject(item);
        
        log.Info("Data to json : "+json);
    
        outputEventHubMessage.Add(json);
    }

    //stations == null ?   log.Info("BadRequest, Please pass a name on the query string or in the request body")
    // : log.Info("OK, ${stations}");
}


        internal static async Task<List<Station>> GetStations()
        {
            Trace.TraceInformation("StationsUtility::GetStationList");
            
            List<Station> stations = null;
            // Add url to app settings
            var url = string.Format("https://opendata.paris.fr/api/v2/catalog/datasets/autolib-disponibilite-temps-reel/exports/json?rows=-1&pretty=false&timezone=UTC");

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var result = await client.GetStringAsync(url);
                    Debug.WriteLine("Result of api call:");
                    Debug.WriteLine(result);

                    stations = JsonConvert.DeserializeObject<List<Station>>(result);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(ex.Message);
                throw ex;
            }

            return stations;
        }

    public class Station
    {
        //[JsonProperty("cars", Required = Required.Always)]
        public int Cars { get; set; }
        [JsonProperty("cars_counter_bluecar", Required = Required.Always)]
        public int CarsCounterBluecar { get; set; }
        [JsonProperty("cars_counter_utilib", Required = Required.Always)]
        public int CarsCounterUtilib { get; set; }
        [JsonProperty("cars_counter_utilib_1.4", Required = Required.Always)]
        public int CarsCounterUtilib_1_4 { get; set; }
        [JsonProperty("charge_slots", Required = Required.Always)]
        public int ChargeSlots{ get; set; }      
        [JsonProperty("charging_status", Required = Required.Always)]
        public string ChargingStatus { get; set; }      
        //[JsonProperty("id", Required = Required.Always)]
        public string ID { get; set; }      
        [JsonProperty("rental_status", Required = Required.Always)]
        public string RentalStatus { get; set; }    
       // [JsonProperty("slots", Required = Required.Always)]
        public int Slots { get; set; }
        //[JsonProperty("status", Required = Required.Always)]
        public string  Status { get; set; }
    }



