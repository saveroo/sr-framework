using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DeviceId;
using Newtonsoft.Json;

namespace SRUL
{
    public class APICompare
    {
        public string query { get; set; }
        public string value { get; set; }
        public bool result { get; set; }
        public int status { get; set; }
    }
    
    public class APIData
    {
        public int status { get; set; }
        public Root body { get; set; }
    }
    
    public class APIEncryptedBody
    {
        public int status { get; set; }
        public string author { get; set; }
        public string body { get; set; }
    }

    public class APIRegisterClient
    {
        // [JsonProperty("ref")]
        public string refId { get; set; }
        public string deviceId { get; set; }
        public string data { get; set; }
    }
    
    public sealed class SRFApis : SREncryptor
    {
        public static bool APIAvailability { get; set; } = false;
        // public static string APIUrl = "https://srframework.vercel.app";
        public static string APIUrl = "http://localhost:3000/api";
        private static string APIkey = "?key=sr";
        private static string APIJustKey = "sr";
        public static string CompareURIPath = "/compare";
        public static string URIPathSRFramework = "/sr-framework?key=sr";
        // private static readonly  Lazy<SRFApis> _instance = new Lazy<SRFApis>(() => new SRFApis());
        private static SRFApis _instance = null;
        public string userRefId = null;
        private static readonly object padlock = new object();
        static HttpClient client = new HttpClient();
        private static int singletonCounter = 0;

        // public static SRFApis Instance => _instance.Value;
        public static SRFApis Instance
        {
            get
            {
                lock (padlock)
                {
                    if(_instance == null)
                        _instance = new SRFApis();
                }

                return _instance;
            }
        }

        public SRFApis() : base()
        {
            RunAsync().Wait();
            singletonCounter++;
            Debug.WriteLine("API Singleton: " + singletonCounter);
        }
        public async Task<APIData> GetAPIData(string path)
        {
            APIData data = null;
            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync(path + APIkey);
                if (response.IsSuccessStatusCode)
                    data = await response.Content.ReadAsAsync<APIData>().ConfigureAwait(false);
                return data;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                XtraMessageBox.Show("Update server couldn't be reach");
            }

            return null;
        }
        
        public async Task<HttpResponseMessage> PostAsJson(string path, string param, dynamic data)
        {
            // var response = "";
                // var js = JsonConvert.SerializeObject(data);
                HttpResponseMessage response = await HttpClientExtensions.PostAsJsonAsync(client, path + param, data).ConfigureAwait(false);
                
                // try
            // {
            //     // if (response.StatusCode == HttpStatusCode.OK)
            //     // {
            //     //     return response.Content.ReadAsAsync<SRClient>();
            //     // }
            //     return response;
            // }
            // catch (Exception e)
            // {
            //     Debug.WriteLine(e);
            //     MessageBox.Show("Error 123");
            // }

            return response;
        }

        public async Task<APIRegisterClient> RegisterNewClient(SRClient clientDevice)
        {
            // var data = null;
            var registerClient = new APIRegisterClient();
            registerClient.deviceId = clientDevice.DeviceID;
            var js = JsonConvert.SerializeObject(clientDevice);
            byte[] gb = Encoding.UTF8.GetBytes(js);
            // registerClient.data = Convert.ToBase64String(gb);
            registerClient.data = SrEncryptor(gb, "MuhammadSurgaSavero", "SRFramework");
            // Debug.WriteLine(registerClient);
            try
            {
                HttpResponseMessage response = await PostAsJson(
                    "/api/client",$"{APIkey}&ops=register&key={APIJustKey}", registerClient);
                if (!response.IsSuccessStatusCode) return registerClient;
                var res = await response.Content.ReadAsAsync<APIRegisterClient>(); 
                return res;
                // var deserialize = JsonConvert.DeserializeObject(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
            // return URI of the created resource.
        }
        
        // Get Encrypted Body
        public async Task<APIEncryptedBody> GetAPIDataEncrypted(string path)
        {
            APIEncryptedBody data = null;
            client.GetAsync(path + APIkey).ContinueWith(async (Task<HttpResponseMessage> res) =>
            {
                Debug.WriteLine(res);
                if (res.Result.IsSuccessStatusCode)
                    data = await res.Result.Content.ReadAsAsync<APIEncryptedBody>();
                return data; 
            }).Wait();
            return data;
        }
        
        public async Task<string> GetAPIDataAsString(string path)
        {
            string data = null;
            HttpResponseMessage response = await client.GetAsync(path + APIkey);
            if (response.IsSuccessStatusCode)
            {
                data = response.Content.ReadAsStringAsync().Result;
            }
            
            return data;
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri(APIUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            // client.DefaultRequestHeaders.Accept.Add(
            //     new MediaTypeWithQualityHeaderValue("application/json")
            // );
            ProductHeaderValue header = new ProductHeaderValue("SRFramework");
            ProductInfoHeaderValue userAgent = new ProductInfoHeaderValue(header);
            client.DefaultRequestHeaders.UserAgent.Add(userAgent);
        }
        public bool GetAvailability()
        {
            try
            {
                using (var api = new WebClient())
                {
                    var data = api.DownloadString(APIUrl+CompareURIPath+"?key=SRFStatus&=true");
                    APICompare json = JsonConvert.DeserializeObject<APICompare>(data);
                    if (json.status == 200 && json.value.ToString() == "true")
                    {
                        APIAvailability = true;
                        return APIAvailability;
                    }
                    else
                    {
                        APIAvailability = false;
                        return APIAvailability;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return APIAvailability;
            }
        }
        
        public async Task<Root> CheckDataUpdate()
        {
            Root serverData = null;
            var apd = await GetAPIData("/api/check");
            if(apd != null) 
                serverData = apd.body;
            return serverData;
        }

        public async Task<TrainerUpdateEnum> CheckNewVersion(string currentVersion)
        {
            APIData apd = await GetAPIData("/api/check?key=sr");
            string version = apd.body.SRFVersion;
            if (version == currentVersion)
                return TrainerUpdateEnum.NewRevision;
            return TrainerUpdateEnum.NoRevision;
        }
        public async Task<TrainerUpdateEnum> CheckNewRevision(string currentRevision)
        {
            APIData apd = await GetAPIData("/api/check?key=sr");
            string version = apd.body.SRFRevision;
            if (version == currentRevision)
                return TrainerUpdateEnum.NewVersion;
            return TrainerUpdateEnum.NoUpdate;
        }
        
        // Fetch DATA and return APIEncryptedBody
        public async Task<APIEncryptedBody> FetchSRFrameworkData()
        {
            APIEncryptedBody apd = await GetAPIDataEncrypted($"/api/data");
            return apd;
        }
        // Compare between version in api and local
        public bool CompareLocalToAPI(string key, string val)
        {
            try
            {
                using (var api = new WebClient())
                {
                    var data = api.DownloadString(APIUrl+CompareURIPath+"?key="+key+"&value="+val);
                    APICompare json = JsonConvert.DeserializeObject<APICompare>(data);
                    if (json.status == 200)
                    {
                        return json.result;
                    }

                    return false;
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Status);
                return false;
            }
        }
    }
}