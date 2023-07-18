using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using SRUL;

namespace SRUL
{
    public class SrApiConfig
    {
        public static string UserRefId { get; set; }

        // ?key=sr
        public static byte[] SecretParamKey { get; set; } = 
            { 115, 114 };

        // POST/GET: Referer
        public static byte[] SecretParamReferer { get; set; } = 
            {104,116,116,112,115,58,47,47,103,111,111,103,108,101,46,99,111,109,47 };

        // POST/GET: Agent
        public static byte[] SecretParamAgent { get; set; } =
            { 83, 82, 70, 114, 97, 109, 101, 119, 111, 114, 107 };
        
        // ?token={encrypted}
        public static byte[] SecretParamTokenKey { get; set; } = 
            { 77, 117, 104, 97, 109, 109, 97, 100, 32, 83, 117, 114, 103, 97, 32, 83, 97, 118, 101, 114, 111 };

    public static int CurrentUrlIndex = 0;
        public static string UsedLink { get; set; }
        public static bool OfflineMode { get; set; }
        public static string[] EndpointList { get; set; } =
        {
            "http://localhost:3000/",
            "http://0.0.0.0:8080/",
            "https://srframework.vercel.app/"
        };

        public static IFlurlRequest Connect(EnumPath ePath)
        {
            // Task.Run(() => InitEndpoint().Wait()).Wait();
            var baseUrl = EndpointList[CurrentUrlIndex];
            Url url = (Url)baseUrl;
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
                    url = url.SetQueryParam("ops", "meta");
                    break;
                case EnumPath.Donators:
                    url = url.AppendPathSegment(EndpointPath.Donators);
                    url = url.SetQueryParam("ops", "data");
                    break;
                case EnumPath.Whitelist:
                    url = url.AppendPathSegment(EndpointPath.Whitelist);
                    url = url.SetQueryParam("ops", "device");
                    break;
                case EnumPath.MandatoryUpdate:
                    url = url.AppendPathSegment(EndpointPath.Update);
                    url = url.SetQueryParam("ops", "update");
                    break;
                case EnumPath.OnlineStatus:
                    url = url.AppendPathSegment(EndpointPath.OnlineStatus);
                    url = url.SetQueryParam("ops", "status");
                    break;
                case EnumPath.RetrievePlayers:
                    url = url.AppendPathSegment(EndpointPath.RetrievePlayers);
                    url = url.SetQueryParam("ops", "retrieve");
                    url = url.SetQueryParam("id", UserRefId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(path), path, null);
            }
            var urlToObject = new UriBuilder(url);
            var hmac = new SrCrypto();
            url = url
                .SetQueryParam("key", 
                    Encoding.UTF8.GetString(SrApiConfig.SecretParamKey))
                .SetQueryParam("token", hmac.HashMessage(
                    Encoding.UTF8.GetString(SrApiConfig.SecretParamTokenKey), urlToObject.Path));
            return url.AllowAnyHttpStatus().WithHeaders(new
            {
                Accept = "application/json",
                Referer = Encoding.UTF8.GetString(SrApiConfig.SecretParamReferer), 
                User_Agent = Encoding.UTF8.GetString(SrApiConfig.SecretParamAgent)
            });
        }

        public static async Task<bool> ResolveUrl(string url)
        {
            try
            {
                var link = await url.AllowAnyHttpStatus().HeadAsync();
                    return link.StatusCode == (int)HttpStatusCode.OK;
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
            
            APIEncryptedBody? dataResult = null;
            try
            {
                var conn = Connect(EnumPath.Data)
                    .GetJsonAsync<APIEncryptedBody>()
                    .ConfigureAwait(false);
                dataResult = conn.GetAwaiter().GetResult();
                return (APIEncryptedBody)dataResult;
            }
            catch (FlurlHttpException e)
            {
                StoreError("[FFD]", e);
                throw;
            }
        }

        public async Task<TSteamResponse?> SteamGetPlayersCount()
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
                return conn;
            }
            catch (FlurlHttpException e)
            {
                StoreError("[SGPC]", e);
                return null;
            }
        }
        public async Task<Root?> CheckDataUpdate()
        {
            try
            {
                var conn = await Connect(EnumPath.Update)
                    .GetJsonAsync<APIData>()
                    .ConfigureAwait(false);
                SrConfig.DataServer = conn.body;
                return SrConfig.DataServer;
            }
            catch (FlurlHttpException e)
            {
                XtraMessageBox.Show($"Error: {e.Message}");
                StoreError("[CDU]", e);
                // _errorList.("[CDU]", e);
                // throw;
                return null;
            }
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
                if (result.Result.StatusCode == 200)
                    SRLoader.SteamPlayerProfile = await result
                        .ReceiveJson<SRSteamProfile>();
                SteamCounter += 1;
                return result.Result.StatusCode == 200;
            }
            catch (FlurlHttpException e)
            {
                SteamCounter += 1;
                StoreError("[PSP]", e);
                return false;
            }
        }

        internal async Task<TResponseDeviceApproval> PostDeviceApproval()
        {
            if (IsOfflineMode()) return null;
            try
            {
                var req = new TRequestDeviceApproval();
                req.DeviceID = SRUtils.Instance.GetClientDevice().DeviceID;
                var result = Connect(EnumPath.Whitelist)
                    .PostJsonAsync(req);
                if (result.Result.StatusCode == 200)
                    return await result
                        .ReceiveJson<TResponseDeviceApproval>();
                return new TResponseDeviceApproval();
                // var aw = result;
                // if(aw.statusCode > 0)
                //     Console.WriteLine(aw);
                // return result.body;
            }
            catch (FlurlHttpException e)
            {
                StoreError("[PDA]", e);
                return null;
            }
        }

        internal async Task<APISRPlayers> GetSRPlayers()
        {
            try
            {
                if (IsOfflineMode()) return null;
                var result = await Connect(EnumPath.RetrievePlayers)
                    .GetJsonAsync<APISRPlayers>()
                    .ConfigureAwait(false);
                if (result != null)
                {
                    return result;
                }
                return null;
            }
            catch (FlurlHttpException e)
            {
                StoreError("[GSRP]", e);
                throw;
                // return null;
            }
        }
        internal void PostOfflineStatus()
        {
            if (SrApiConfig.OfflineMode) return;
            try
            {
                if (SrApiConfig.UserRefId == null) return;
                var clientStatusesPayload = new APIOnlineStatusRequest();
                clientStatusesPayload.refId = SrApiConfig.UserRefId;
                clientStatusesPayload.IsOnline = false;
                Connect(EnumPath.OnlineStatus)
                    .PostJsonAsync(clientStatusesPayload);
            }
            catch (Exception e)
            {
                StoreError("[POS]", e);
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
            var endpoint = Task.Run( async () => await SrApiConfig.GetAvailableEndpoint().ConfigureAwait(false));
            return endpoint.Result;
        }
        public IFlurlRequest Connect(EnumPath enumPath)
        {
            return SrApiConfig.Connect(enumPath);
        }
    }
}