// using System;
// using System.Diagnostics;
// using System.Net;
// using System.Net.Http;
// using System.Net.Http.Headers;
// using System.Text;
// using System.Threading.Tasks;
// using DevExpress.XtraEditors;
// using Newtonsoft.Json;
//
// // TODO: Refactor the whole thing with IServiceCollection, DI.
// // Interchangeable URL
//
// namespace SRUL
// {
//     public static class ApiPath
//     {
//         public static readonly string ApiRegister = "/api/client";
//         public static readonly string ApiPostSteamID = "/api/client";
//         public static readonly string ApiUpdate = "/api/check";
//         public static readonly string ApiData = "/api/data";
//     }
//     
//     public static class NewApiPath
//     {
//         public static readonly string AbsolutePath = "api";
//         public static readonly string Register = "client";
//         public static readonly string PostSteamId = "client";
//         public static readonly string Update = "check";
//         public static readonly string Data = "data";
//     }
//
//     public static class ApiConfig
//     {
//         public static bool ApiProduction { get; set; } = false;
//         // public static string ApiUrl { get; set; } = "https://srframework.vercel.app";
//         public static string ApiUrlProduction { get; set; } = "http://localhost:3000";
//         public static string ApiUrlLocal = "http://localhost:3000";
//         public static string ApiCurrentUrl { get; set; } = "https://srframework.vercel.app";
//         public static string[] ApiUrlList { get; set; } = new string[]
//         {
//             "http://localhost:3000",
//             "https://srframework.vercel.app"
//         };
//         public static string ApiKey { get; set; } = "sr";
//         public static string ApiReferer { get; set; } = "https://google.com";
//         public static string ApiAgent { get; set; } = "SRFramework";
//         public static string ApiToken { get; set; } = "";
//         public static string ApiTokenKey { get; set; } = "Muhammad Surga Savero";
//         public static int CurrentUrlIndex = 0;
//
//         public async static Task<bool> GoToUrl(string url)
//         {
//             try
//             {
//                 var request = WebRequest.Create(url);
//                 request.Method = "HEAD";
//                 using (var response = (HttpWebResponse) request.GetResponse())
//                 {
//                     return response.StatusCode == HttpStatusCode.OK;
//                 }
//             }
//             catch
//             {
//                 //XtraMessageBox.Show(e.ToString(), "Error");
//                 //Console.WriteLine(e);
//                 return false;
//             }
//         }
//         
//         public async static Task<string> GetAvailableLink()
//         {
//             while (true)
//             {
//                 var urlCount = ApiUrlList.Length;
//                 if (CurrentUrlIndex < urlCount)
//                 {
//                     if (await GoToUrl(ApiUrlList[CurrentUrlIndex]))
//                     {
//                         return ApiUrlList[CurrentUrlIndex];
//                     }
//                     CurrentUrlIndex++;
//                     continue;
//                 }
//                 XtraMessageBox.Show("[GAL] Couldn't reach server.", "Error");
//                 return null;
//                 break;
//             }
//         }
//     }
//
//     // public class SRApiClient
//     // {
//     //     private readonly IHttpClientFactory _httpClientFactory;
//     //
//     //     public SRApiClient(IHttpClientFactory httpClientFactory)
//     //     {
//     //         _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
//     //     }
//     //
//     //     public async Task<IReadOnlyCollection<Root>> GetRepo(CancellationToken cancellationToken)
//     //     {
//     //         var httpClient = _httpClientFactory.CreateClient("local");
//     //         var result = await httpClient.GetStringAsync(github)
//     //     }
//     // }
//
//
//     public sealed class SRFApis : SREncryptor
//     {
//         public static bool APIAvailability { get; set; } = false;
//         // public static string APIUrl = "https://srframework.vercel.app/api";
//         public static readonly string APIUrl = "http://localhost:3000";
//         // public static string APIUrl = "https://srframework.saveroo27.vercel.app/api";
//         private static string _apIkey = "?key=sr";
//         private static string _apiJustKey = "sr";
//         public int SteamCounter = 0; // check if has been posting to server just once
//         public static readonly string CompareURIPath = "/compare";
//         public static readonly string URIPathSRFramework = "/sr-framework?key=sr";
//         // private static readonly  Lazy<SRFApis> _instance = new Lazy<SRFApis>(() => new SRFApis());
//         private static SRFApis _instance = null;
//         public string userRefId { get; set; }
//         private static readonly object Padlock = new object();
//         static HttpClient client = new HttpClient();
//         private static int singletonCounter = 0;
//         private string token;
//         public Root ServerMeta = null;
//
//         // public static SRFApis Instance => _instance.Value;
//         public static SRFApis Instance
//         {
//             get
//             {
//                 lock (Padlock)
//                 {
//                     if(_instance == null)
//                         _instance = new SRFApis();
//                 }
//
//                 return _instance;
//             }
//         }
//
//         public SRFApis() : base()
//         {
//             var t1 = Task.Factory.StartNew(() => RunAsync());
//             try
//             {
//                 Task.WaitAll(t1);
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 XtraMessageBox.Show(e.ToString());
//                 throw;
//             }
//             singletonCounter++;
//             Debug.WriteLine("API Singleton: " + singletonCounter);
//             
//         }
//         static async Task RunAsync()
//         {
//             client.BaseAddress = new Uri("https://srframework.vercel.app");
//             client.DefaultRequestHeaders.Accept.Clear();
//             // Set UA
//             var header = new ProductHeaderValue(ApiConfig.ApiAgent);
//             var userAgent = new ProductInfoHeaderValue(header);
//             client.DefaultRequestHeaders.UserAgent.Add(userAgent);
//             
//             // Set Referer
//             client.DefaultRequestHeaders.Add("referer", ApiConfig.ApiReferer);
//         }
//         public async Task<APIData> GetAPIData(string path)
//         {
//             APIData data = null;
//             HttpResponseMessage response;
//             var t1 = Task.Factory.StartNew(() => client.GetAsync(path));
//             try
//             {
//                 Task.WaitAll(t1);
//                 response = t1.Result.Result;
//                 if (response.IsSuccessStatusCode)
//                 {
//                     var t2 = Task.Run(() => response.Content.ReadAsAsync<APIData>());
//                     try
//                     {
//                         Task.WaitAll(t2);
//                         data = t2.Result;
//                     }
//                     catch
//                     {
//                         XtraMessageBox.Show("[GADR] Couldn't read data");
//                     }
//                 }
//                 return data;
//             }
//             catch
//             {
//                 XtraMessageBox.Show("[GAD] Couldn't reach update server.");
//             }
//
//             return null;
//         }
//         
//         public async Task<HttpResponseMessage> PostAsJson(string path, string param, dynamic data)
//         {
//             // var response = "";
//                 // var js = JsonConvert.SerializeObject(data);
//                 HttpResponseMessage response = await HttpClientExtensions.PostAsJsonAsync(client, path + param, data).ConfigureAwait(false);
//                 
//                 // try
//             // {
//             //     // if (response.StatusCode == HttpStatusCode.OK)
//             //     // {
//             //     //     return response.Content.ReadAsAsync<SRClient>();
//             //     // }
//             //     return response;
//             // }
//             // catch (Exception e)
//             // {
//             //     Debug.WriteLine(e);
//             //     MessageBox.Show("Error 123");
//             // }
//
//             return response;
//         }
//
//         public string UriPath(EnumPath path, bool onlyParam = false)
//         {
//             string tkn, url;
//             string key = $"?key={ApiConfig.ApiKey}";
//             switch (path)
//             {
//                 case EnumPath.Steam:
//                     url = !onlyParam ? $"{ApiPath.ApiRegister}" : "";
//                     tkn = $"{url}{key}&ops=steam&token={HashMessage(ApiConfig.ApiTokenKey, ApiPath.ApiRegister)}";
//                     break;
//                 case EnumPath.Register:
//                     url = !onlyParam ? $"{ApiPath.ApiRegister}" : "";
//                     tkn = $"{url}{key}&ops=register&token={HashMessage(ApiConfig.ApiTokenKey, ApiPath.ApiRegister)}";
//                     break;
//                 case EnumPath.Update:
//                     url = !onlyParam ? $"{ApiPath.ApiUpdate}" : "";
//                     tkn = $"{url}{key}&token={HashMessage(ApiConfig.ApiTokenKey, ApiPath.ApiUpdate)}";
//                     break;
//                 case EnumPath.Data:
//                     url = !onlyParam ? $"{ApiPath.ApiData}" : "";
//                     tkn = $"{url}{key}&token={HashMessage(ApiConfig.ApiTokenKey, ApiPath.ApiData)}";
//                     break;
//                 case EnumPath.MandatoryUpdate:
//                     url = !onlyParam ? $"{ApiPath.ApiUpdate}" : "";
//                     tkn = $"{url}{key}&token={HashMessage(ApiConfig.ApiTokenKey, ApiPath.ApiUpdate)}";
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException(nameof(path), path, null);
//             }
//             return tkn;
//         }
//
//         public async Task<APIRegisterClient> RegisterNewClient(SRClient clientDevice)
//         {
//             // var data = null;
//             var registerClient = new APIRegisterClient();
//             registerClient.deviceId = clientDevice.DeviceID;
//             var js = JsonConvert.SerializeObject(clientDevice);
//             byte[] gb = Encoding.UTF8.GetBytes(js);
//             // registerClient.data = Convert.ToBase64String(gb);
//             registerClient.data = SrEncryptor(gb, "MuhammadSurgaSavero", "SRFramework");
//             // Debug.WriteLine(registerClient);
//             try
//             {
//                 HttpResponseMessage response = await PostAsJson(
//                     ApiPath.ApiRegister, UriPath(EnumPath.Register, true), registerClient);
//                 if (!response.IsSuccessStatusCode) return registerClient;
//                 var res = await response.Content.ReadAsAsync<APIRegisterClient>();
//                 userRefId = res.refId;
//                 return res;
//                 // var deserialize = JsonConvert.DeserializeObject(response);
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 throw;
//             }
//            
//             // return URI of the created resource.
//         }
//         public async Task<bool> PostSteamProfile(string deviceId, string steamId, string steamPersona = null)
//         {
//             // var data = null;
//             var regsiterSteamId = new APIRegisterClient();
//             regsiterSteamId.refId = userRefId;
//             regsiterSteamId.deviceId = deviceId;
//             regsiterSteamId.playerSteamID = steamId;
//             regsiterSteamId.playerSteamPersona = steamPersona;
//             // regsiterSteamId.playerSteamID = clientDevice.pl;
//             // regsiterSteamId.PlayerSteamPersona = clientDevice.DeviceID;
//             // var js = JsonConvert.SerializeObject(clientDevice);
//             // byte[] gb = Encoding.UTF8.GetBytes(js);
//             // // regsiterSteamId.data = Convert.ToBase64String(gb);
//             // regsiterSteamId.data = SrEncryptor(gb, "MuhammadSurgaSavero", "SRFramework");
//             // Debug.WriteLine(regsiterSteamId);
//             try
//             {
//                 HttpResponseMessage response = await PostAsJson(
//                     ApiPath.ApiPostSteamID, UriPath(EnumPath.Steam, true), regsiterSteamId);
//                 SteamCounter += 1;
//                 if (!response.IsSuccessStatusCode) return false;
//                 var res = await response.Content.ReadAsAsync<SRSteamProfile>();
//                 SRLoader.SteamPlayerProfile = res;
//                 return true;
//                 // var deserialize = JsonConvert.DeserializeObject(response);
//             }
//             catch (Exception e)
//             {
//                 SteamCounter += 1;
//                 Console.WriteLine(e);
//                 return false;
//             }
//            
//             // return URI of the created resource.
//         }
//         
//         
//         // Get Encrypted Body
//         public async Task<APIEncryptedBody> GetAPIDataEncrypted(string path)
//         {
//             APIEncryptedBody data = null;
//             var t1 = client.GetAsync(path).ContinueWith(async (Task<HttpResponseMessage> res) =>
//             {
//                 Debug.WriteLine(res);
//                 if (res.Result.IsSuccessStatusCode)
//                     data = await res.Result.Content.ReadAsAsync<APIEncryptedBody>();
//                 return data; 
//             });
//             try
//             {
//                 Task.WaitAll(t1);
//             }
//             catch (Exception e)
//             {
//                 XtraMessageBox.Show("[GADE] Something went wrong when retrieving the data", "Error");
//                 throw;
//             }
//             return data;
//         }
//         
//         // Check Data Update api/check
//         public async Task<Root> CheckDataUpdate()
//         {
//             Root serverData = null;
//             // var apd = GetAPIData(UriPath(ApiEnumPath.Update));
//             var t1 = Task.Factory.StartNew(() => GetAPIData(UriPath(EnumPath.Update)));
//             try
//             {
//                 Task.WaitAll(t1);
//                 var apd = t1.Result.Result;
//                 XtraMessageBox.Show("apd", apd.ToString());
//                 if(apd != null) 
//                     serverData = apd.body;
//                 ServerMeta = serverData;
//                 return serverData;
//             }
//             catch
//             {
//                 XtraMessageBox.Show("[CDU] Can't check for update", "Error");
//             }
//
//             return serverData;
//         }
//         // Fetch DATA and return APIEncryptedBody
//         public async Task<APIEncryptedBody> FetchSRFrameworkData()
//         {
//             var uri = UriPath(EnumPath.Data);
//             var mainData = await GetAPIDataEncrypted(uri);
//             return mainData;
//         }
//         // Compare between version in api and local
//         public bool CompareLocalToAPI(string key, string val)
//         {
//             try
//             {
//                 using (var api = new WebClient())
//                 {
//                     var data = api.DownloadString(APIUrl+CompareURIPath+"?key="+key+"&value="+val);
//                     APICompare json = JsonConvert.DeserializeObject<APICompare>(data);
//                     if (json.status == 200)
//                     {
//                         return json.result;
//                     }
//
//                     return false;
//                 }
//             }
//             catch (WebException e)
//             {
//                 Console.WriteLine(e.Status);
//                 return false;
//             }
//         }
//
//         public async Task<int> FetchSteamPlayerCount()
//         {
//             try
//             {
//                 using (var steamApi = new HttpClient())
//                 {
//                     steamApi.BaseAddress = new Uri("https://api.steampowered.com");
//                     var tryGet = await steamApi.GetAsync("ISteamUserStats/GetNumberOfCurrentPlayers/v1/?appid=314980");
//                     if (tryGet.IsSuccessStatusCode)
//                     {
//                         var res = tryGet.Content.ReadAsAsync<TSteamResponse>();
//                         return res.Result.response.player_count;
//                     }
//                 }
//             }
//             catch (Exception e)
//             {
//                 Debug.WriteLine("API_STEAM Error: " +e);
//                 return 0;
//             }
//             return 0;
//         }
//     }
// }