using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Flurl;
using Flurl.Http;
using Flurl.Http.Testing;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SRUL
{
    public class SrApiConfig
    {
        public static string UserRefId { get; set; }
        // ?key=sr
        public static string SecretParamKey { get; set; } = "sr";
        // POST/GET: Referer
        public static string SecretParamReferer { get; set; } = "https://google.com/";
        // POST/GET: Agent
        public static string SecretParamAgent { get; set; } = "SRFramework";
        // ?token={encrypted}
        public static string SecretParamTokenKey { get; set; } = "Muhammad Surga Savero";
        
        public static int CurrentUrlIndex = 0;
        public static string UsedLink { get; set; }
        public static bool OfflineMode { get; set; }
        public static string[] EndpointList { get; set; } =
        {
            "http://slocalhost:3000",
            "https://ssrframework.vercel.app"
        };

        public static IFlurlRequest Connect(EnumPath ePath)
        {
            // Task.Run(() => InitEndpoint().Wait()).Wait();
            var baseUrl = EndpointList[CurrentUrlIndex];
            var url = baseUrl;
            url = url.AppendPathSegment(EndpointPath.AbsolutePath);
            string ops = "";
            string path = "";
            switch (ePath)
            {
                case EnumPath.Steam:
                    url = url.AppendPathSegment(EndpointPath.PostSteamId);
                    url = url.SetQueryParam("ops", "steam");
                    break;
                case EnumPath.Register:
                    url = url.AppendPathSegment(EndpointPath.Register);
                    url = url.SetQueryParam("ops", "register");
                    break;
                case EnumPath.Update:
                    url = url.AppendPathSegment(EndpointPath.Update);
                    url = url.SetQueryParam("ops", "update");
                    break;
                case EnumPath.Data:
                    url = url.AppendPathSegment(EndpointPath.Data);
                    url = url.SetQueryParam("ops", "data");
                    break;
                case EnumPath.MandatoryUpdate:
                    url = url.AppendPathSegment(EndpointPath.Update);
                    url = url.SetQueryParam("ops", "update");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(path), path, null);
            }
            var urlToObject = new UriBuilder(url);
            var hmac = new SrCrypto();
            url = url
                .SetQueryParam("key", SrApiConfig.SecretParamKey)
                .SetQueryParam("token", hmac.HashMessage(SrApiConfig.SecretParamTokenKey, urlToObject.Path));
            return url.AllowAnyHttpStatus().WithHeaders(new
            {
                Accept = "application/json",
                Referer = SrApiConfig.SecretParamReferer, 
                User_Agent = SrApiConfig.SecretParamAgent
            });
        }

        public static async Task<bool> ResolveUrl(string url)
        {
            try
            {
                var link = await url.AllowAnyHttpStatus().HeadAsync();
                return link.StatusCode == HttpStatusCode.OK;
                // using (var cli = new FlurlClient(url))
                // {
                //     // var link = await cli.Request().AllowAnyHttpStatus().GetAsync();
                // }
            }
            catch(FlurlHttpException e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
        public static async Task<string> GetAvailableEndpoint()
        {
            var urlCount = EndpointList.Length;
            while (!OfflineMode)
            {
                if (CurrentUrlIndex < urlCount)
                {
                    if (await ResolveUrl(EndpointList[CurrentUrlIndex]))
                    {
                        return EndpointList[CurrentUrlIndex];
                    }
                    CurrentUrlIndex += 1;
                    continue;
                }
                OfflineMode = true;
                return null;
            }

            return null;
            // for (CurrentUrlIndex = 0; CurrentUrlIndex < EndpointList.Length; CurrentUrlIndex++)
            // {
            //     if (await ResolveUrl(EndpointList[CurrentUrlIndex]))
            //     { 
            //         UsedLink = EndpointList[CurrentUrlIndex];
            //         return UsedLink;
            //     }
            // }
            //
            // return UsedLink;
        }
    }
    public class SRApi : SrCrypto
    {
        private string _endpoint { get; set; }

        public int SteamCounter { get; set; }
        public Root ServerMeta { get; set; } = null;
        private static Dictionary<string, Exception> _errorList { get; set; } = new Dictionary<string, Exception>();

        public SRApi() : base()
        {
            // var test = Connect(ApiEnumPath.Update).GetJsonAsync<APIData>();
            // InitEndpoint().Wait();
        }

        public bool IsOfflineMode()
        {
            return SrApiConfig.OfflineMode;
        }

        public async Task<APIEncryptedBody> FetchFullData()
        {
            
            APIEncryptedBody dataResult = null;
            try
            {
                var conn = Connect(EnumPath.Data)
                    .GetJsonAsync<APIEncryptedBody>()
                    .ConfigureAwait(false);
                dataResult = conn.GetAwaiter().GetResult();
                return dataResult;
            }
            catch (FlurlHttpException e)
            {
                StoreError("[FFD]", e);
                throw;
            }
        }

        public async Task<int> SteamGetPlayersCount()
        {
            try
            {
                var conn = await "https://api.steampowered.com"
                    .AppendPathSegment("ISteamUserStats")
                    .AppendPathSegment("GetNumberOfCurrentPlayers")
                    .AppendPathSegment("v1")
                    .SetQueryParam("appid", "314980")
                    .GetJsonAsync<TSteamResponse>()
                    .ConfigureAwait(false);
                return conn.response.player_count;
            }
            catch (FlurlHttpException e)
            {
                StoreError("[SGPC]", e);
                return 0;
            }
        }
        public async Task<Root> CheckDataUpdate()
        {
            try
            {
                var conn = await Connect(EnumPath.Update)
                    .GetJsonAsync<APIData>()
                    .ConfigureAwait(false);
                SrConfig.DataServer = conn.body;
            }
            catch (FlurlHttpException e)
            {
                StoreError("[CDU]", e);
                // _errorList.("[CDU]", e);
                // throw;
                return null;
            }
            return SrConfig.DataServer;
        }
        public async Task<APIRegisterClient> PostRegisterClient(SRClient clientDevice)
        {
            var registerClient = new APIRegisterClient();
            var pass = "SRFramework";
            var salt = "MuhammadSurgaSavero";
            registerClient.deviceId = clientDevice.DeviceID;
            var serializedData = JsonConvert.SerializeObject(clientDevice);
            registerClient.data = SrEncryptor(serializedData);

            try
            {
                var result = await Connect(EnumPath.Register)
                    .PostJsonAsync(registerClient)
                    .ReceiveJson<APIRegisterClient>()
                    .ConfigureAwait(false);
                SrApiConfig.UserRefId = result.refId;
                return result;
            }
            catch (FlurlHttpException e)
            {
                StoreError("[PRC]", e);
                throw;
            }
        }
        public async Task<bool> PostSteamProfile(APIRegisterClient clientData)
        {
            var regsiterSteamId = new APIRegisterClient();
            regsiterSteamId.refId = clientData.refId;
            regsiterSteamId.deviceId = clientData.deviceId;
            regsiterSteamId.playerSteamID = clientData.playerSteamID;
            regsiterSteamId.playerSteamPersona = clientData.playerSteamPersona;
            try
            {
                var result = Connect(EnumPath.Steam)
                    .PostJsonAsync(regsiterSteamId);
                if (result.Result.IsSuccessStatusCode)
                    SRLoader.SteamPlayerProfile = await result
                        .ReceiveJson<SRSteamProfile>()
                        .ConfigureAwait(false);
                SteamCounter += 1;
                return result.Result.IsSuccessStatusCode;
            }
            catch (FlurlHttpException e)
            {
                SteamCounter += 1;
                StoreError("[PSP]", e);
                return false;
            }
        }

        public async Task<string> DownloadFile(string path, string filename)
        {
            string dl;
            try
            {
                dl = await Connect(EnumPath.Data)
                    .DownloadFileAsync(path, filename)
                    .ConfigureAwait(false);
                return dl;
            }
            catch (Exception e)
            {
                StoreError("[DL]", e);
                XtraMessageBox.Show( "Possible Cause:\n1. Connection unavailable\n2. Update server down\n3. Not running as admin\n\nPlease contact: sysadmin47@gmail.com", "There's bug, try to restart");
                SrConfig.FileDeleteFromPath();
                throw e;
            }
        }

        public void StoreError(string key, Exception value)
        {
            if (_errorList.ContainsKey(key))
                _errorList[key] = value;
            _errorList.Add(key, value);
            Debug.WriteLine($"{key}: {value}");
        }
        public string InitEndpoint()
        {
            var endpoint = Task.Run( async () => await SrApiConfig.GetAvailableEndpoint());
            return endpoint.Result;
        }
        public IFlurlRequest Connect(EnumPath enumPath)
        {
            return SrApiConfig.Connect(enumPath);
            // Task.Run(() => InitEndpoint().Wait()).Wait();
            // var baseUrl = InitEndpoint();
            // if (string.IsNullOrEmpty(baseUrl))
            // {
            //     // XtraMessageBox.Show("Couldn't connect anywhere, \nswitching to offline mode\nPlease download required data manually.", "Error");
            //     // SrApiConfig.OfflineMode = true;
            //     return null;
            // }
            // var url = baseUrl;
            // url = url.AppendPathSegment(NewApiPath.AbsolutePath);
            // string ops = "";
            // string path = "";
            // switch (enumPath)
            // {
            //     case EnumPath.Steam:
            //         url = url.AppendPathSegment(NewApiPath.PostSteamId);
            //         url = url.SetQueryParam("ops", "steam");
            //         break;
            //     case EnumPath.Register:
            //         url = url.AppendPathSegment(NewApiPath.Register);
            //         url = url.SetQueryParam("ops", "register");
            //         break;
            //     case EnumPath.Update:
            //         url = url.AppendPathSegment(NewApiPath.Update);
            //         url = url.SetQueryParam("ops", "update");
            //         break;
            //     case EnumPath.Data:
            //         url = url.AppendPathSegment(NewApiPath.Data);
            //         url = url.SetQueryParam("ops", "data");
            //         break;
            //     case EnumPath.MandatoryUpdate:
            //         url = url.AppendPathSegment(NewApiPath.Update);
            //         url = url.SetQueryParam("ops", "update");
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException(nameof(path), path, null);
            // }
            // var urlToObject = new UriBuilder(url);
            // url = url
            //     .SetQueryParam("key", SrApiConfig.SecretParamKey)
            //     .SetQueryParam("token", HashMessage(SrApiConfig.SecretParamTokenKey, urlToObject.Path));
            // return url.AllowAnyHttpStatus().WithHeaders(new
            // {
            //     Accept = "application/json",
            //     Referer = SrApiConfig.SecretParamReferer, 
            //     User_Agent = SrApiConfig.SecretParamAgent
            // });
        }
    }
}