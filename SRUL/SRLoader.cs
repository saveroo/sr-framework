using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using Flurl.Http;
using SmartAssembly.Attributes;
using SRUL.Views;
using Timer = System.Windows.Forms.Timer;

namespace SRUL
{
    public enum TrainerUpdateEnum
    {
        NoUpdate,
        NoRevision,
        NewVersion,
        NewRevision,
        NewVersionNewRevision,
        NoVersionNoRevision,
        Error
    }
    
    public enum UpdateStatusEnum
    {
        Idle,
        Checking,
        Downloading,
        Installing,
        Error
    }
    
    public enum AutoReloadEnum
    {
        Idle,
        Checking,
        Reloading,
    }
    
    public class ActiveTrainer
    {
        public string[]? GameProcessNameList { get; set; }
        public string? GameName { get; set; }
        public string? GameProcessName { get; set; }
        public System.Diagnostics.Process? GameProcess { get; set; } = null;
        public string? GameVersion { get; set; }
        public int GamePID { get; set; }
        public bool GameValidated { get; set; }
        
        public bool TrainerAvailability { get; set; }
        public bool GameState { get; set; } = false;
        public bool VersionState { get; set; }
        public bool IsRegistered { get; set; }
        public UpdateStatusEnum UpdateState { get; set; } = UpdateStatusEnum.Idle;
        public AutoReloadEnum AutoReloadState { get; set; } = AutoReloadEnum.Idle;
        private static readonly Lazy<ActiveTrainer> _instance = new (() => new ActiveTrainer());

        public static ActiveTrainer Instance => _instance.Value;

    }
    public class SRLoader
    {
        public string CurrentProductVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public string? CurrentProductRevision;
        public bool LocalDataExist;
        public int SteamPlayerCount = 0;
        public static SRSteamProfile? SteamPlayerProfile;
        public static SRPlayers SRPlayers;
        public static TDeviceApproval? SRDeviceApproval;
        // public static SRAdsManager srAdsManager;
        public ActiveTrainer Selected { get; set; } = ActiveTrainer.Instance;
        private XtraForm _trainerForm;
        private XtraForm _loaderForm;
        protected internal SRApi apis = new SRApi();
        // private SREncryptor _encryptor = SREncryptor.Instance;
        protected internal Root? LoaderData = SrConfig.DataMain;
        public SRMain jr;
        protected internal SRReadWrite rw;
        private SrCrypto _srCrypto = new SrCrypto();
        public TrainerUpdateEnum CheckForUpdate(string currentProductVersion, string? currentProductRevision)
        {
            async Task<TrainerUpdateEnum> check()
            {
                var tt = await apis.CheckDataUpdate().ConfigureAwait(false);
                if (tt == null) return TrainerUpdateEnum.Error;
                                if (tt.SRFRevision != currentProductRevision && tt.SRFVersion != currentProductVersion)
                    return TrainerUpdateEnum.NewVersionNewRevision;
                if (tt.SRFRevision != currentProductRevision)
                    return TrainerUpdateEnum.NewRevision;
                if (tt.SRFVersion != currentProductVersion)
                    return TrainerUpdateEnum.NewVersion; 
                return TrainerUpdateEnum.NoVersionNoRevision;
            }
            
            return Task.Run(async () => await check()).Result;
        }

        [ForceObfuscate(true)]
        public bool CheckForApproval()
        {
            return SRDeviceApproval != null && SRDeviceApproval.Approval;
            // var test = await SRLoaderForm._srLoader.apis.PostDeviceApproval().ConfigureAwait(false);
            // // get this async result
            // if (test == null) return false;
            // if (test.statusCode == 200)
            // {
            //     SRDeviceApproval = test.body;
            // }
            //
            // if (!SrApiConfig.OfflineMode)
            //     if (SRDeviceApproval.Value.Approval)
            //         return true;
            // return false;
        }

        public bool Update(TrainerUpdateEnum t)
        {
            switch (t)
            {
                case TrainerUpdateEnum.NewRevision:
                    Selected.UpdateState = UpdateStatusEnum.Downloading;
                    if (!Task.Run(async () => await DownloadNewData().ConfigureAwait(false)).Result)
                    {
                        Selected.UpdateState = UpdateStatusEnum.Error;
                        return false;
                    }
                    Selected.UpdateState = UpdateStatusEnum.Installing;
                    DecryptDataFromFile();
                    CurrentProductRevision = LoaderData.SRFRevision;
                    Selected.UpdateState = UpdateStatusEnum.Idle;
                    return true;
                case TrainerUpdateEnum.NewVersion:
                    Selected.UpdateState = UpdateStatusEnum.Downloading;
                    var newVersion = Task.Run(async () => await DownloadNewData().ConfigureAwait(false)).Result;
                    if(newVersion) 
                        Selected.UpdateState = UpdateStatusEnum.Idle;
                    else
                        Selected.UpdateState = UpdateStatusEnum.Error;
                    return newVersion;
                case TrainerUpdateEnum.NewVersionNewRevision:
                    Selected.UpdateState = UpdateStatusEnum.Downloading;
                    var updateResult = Task.Run(async () => await DownloadNewData().ConfigureAwait(false)).Result;
                    if (updateResult)
                    {
                        Selected.UpdateState = UpdateStatusEnum.Installing;
                        if (DecryptDataFromFile())
                        {
                            CurrentProductVersion = LoaderData.SRFVersion;
                            CurrentProductRevision = LoaderData.SRFRevision;
                            Selected.UpdateState = UpdateStatusEnum.Idle;
                        }
                        else
                        {
                            Selected.UpdateState = UpdateStatusEnum.Error;
                            return false;
                        }
                        CurrentProductRevision = LoaderData.SRFRevision;
                        Selected.UpdateState = UpdateStatusEnum.Idle;
                        return true;
                    }
                    Selected.UpdateState = UpdateStatusEnum.Error;
                    return false;
                default:
                    Selected.UpdateState = UpdateStatusEnum.Idle;
                    return false;
            }
        }

        public void GenerateDownloadLink(IList<SRFChangelog> changelog)
        {
            if (changelog.Count == 0) return;
            using (ChangeLogViewer clv = new ChangeLogViewer(changelog))
            { 
                DialogResult dialog = XtraDialog.Show(clv, $"New Update {LoaderData?.SRFVersion}", MessageBoxButtons.YesNo); 
                if (dialog == DialogResult.Yes) 
                    System.Diagnostics.Process.Start(LoaderData.SRFDownloadLink);
            }
        }

        // TODO: No Override.
        private void GenerateDownloadLink(Root? serverData)
        {
            if (serverData == null) 
                serverData = LoaderData;
            // if (clv == null)
            //     clv = new ChangeLogViewer(serverData.SRFChangelog);
            if (serverData?.SRFChangelog != null)
                using (var clv = new ChangeLogViewer(serverData?.SRFChangelog))
                {
                    var dialog = XtraDialog.Show(clv, $"New Update {serverData?.SRFVersion}", MessageBoxButtons.YesNo);
                    if (dialog == DialogResult.Yes)
                        if (serverData.SRFDownloadLink != null)
                            System.Diagnostics.Process.Start(serverData.SRFDownloadLink);
                }
        }

        public void SetUserDeviceRef(string id)
        {
            SrApiConfig.UserRefId = id;
        }

        private void OfflineModeDecryptor()
        {
            if (!SrApiConfig.OfflineMode) return;
            if(SrConfig.IsDataFileExist())
                if (DecryptDataFromFile()) return;
            if(!SrConfig.IsDataFileExist())
                ShowMessageBox("Data is not found\nPlease download the data manually.", "Error");
            return;
        }

        private async Task LoadMainData()
        {
            if (string.IsNullOrEmpty(apis.InitEndpoint()))
            {
                ShowMessageBox("[GAE] Couldn't reach server.\nAuto Update disabled\nPlease dont run check for update.", "Error");
                OfflineModeDecryptor();
            }
            else
            {
                try
                {
                    await DownloadOrDecrypt().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    ShowMessageBox("[GAE] Couldn't reach server", "Error");
                    throw new Exception($"Error {e}");
                }
            } 
            
            // jr = JSONReader.Instance;
        }

        private async Task DownloadOrDecrypt()
        {
            LocalDataExist = SrConfig.IsDataFileExist();
            if (!LocalDataExist)
            {
                await DownloadNewData().ConfigureAwait(false);
                DecryptDataFromFile();
            }
            else
            {
                if (!DecryptDataFromFile())
                    ShowMessageBox("Something went wrong with the data", "Error");
            }
        }

        // Download instead of Decrypt
        public async Task<bool> DownloadNewData()
        {
            // var fetchData = await apis.FetchFullData();
            // var data = Task.FromResult(fetchData);;
            SrConfig.FileDeleteFromPath();
            Task.Run(() =>
            {
                apis.DownloadFile(SrConfig.DataFilePath, SrConfig.DataFileName).Wait();
                var bytesFromFile = File.ReadAllBytes(SrConfig.FileGetPathName());
                _srCrypto.OverwriteFileWith(bytesFromFile);
                File.Encrypt(SrConfig.FileGetPathName());
            }).Wait();
            LocalDataExist = SrConfig.IsDataFileExist();
            return LocalDataExist;
            
            // SrConfig.DataFilePath = newFilePath;
        }

        public void ShowMessageBox(string text, string caption)
        {
            XtraMessageBox.Show(text, caption);
        }
        
        // Decrypt instead of download
        [ForceObfuscate(true)]
        private bool DecryptDataFromFile()
        {
            try
            {
                LoaderData = null;
                File.Decrypt(SrConfig.FileGetPathName());
                var readFromFile = File.ReadAllBytes(SrConfig.FileGetPathName());
                var decodedBytes = _srCrypto.SrDecryptor(Encoding.UTF8.GetString(readFromFile));
                var frombase64 = Convert.FromBase64String(decodedBytes);
                var tostr = Encoding.UTF8.GetString(frombase64);
                var deserialized = _srCrypto.DeserializeAs<APIEncryptedBody>(tostr);
                var finalData = _srCrypto.SrDecryptor(deserialized.body);
                SrConfig.DataMain = _srCrypto.DeserializeAs<Root>(finalData);
                LoaderData = SrConfig.DataMain;
                if (SrConfig.DataMain.Games.Count < 1)
                    return false;
                CurrentProductRevision = SrConfig.DataMain.SRFRevision;
                return true;
            }
            catch (Exception e)
            {
                ShowMessageBox($"Corrupt Data", "Error!");
                SrConfig.FileDeleteFromPath();
                return DownloadNewData().Result;
            }
        }
        
        [ForceObfuscate(true)]
        public async Task<APIRegisterClient> DeviceRegister()
        {
            var clientDevice = SRUtils.Instance.GetClientDevice();
            var postRegistration = await apis.PostRegisterClient(clientDevice).ConfigureAwait(false);
            return postRegistration;
        }
        // Deferring Device Register
        public async Task<bool> PostSteamProfile()
        {
            if (SrApiConfig.OfflineMode) return false;
            // if (apis.SteamCounter > 3) return false;

            // Post Steam Persona to server
            async Task<bool> GetSteamProfile()
            { 
                var postProfile = await apis.PostSteamProfile(new APIRegisterClient()
                {
                    refId = SrApiConfig.UserRefId,
                    deviceId = SRUtils.Instance.GetClientDevice().DeviceID, // Get Device ID, required to update 
                    playerSteamPersona = "PlayerSteamPersona".GetFeature().Read(rw),
                    playerSteamID = "PlayerSteamID".GetFeature().Read(rw).ToString()
                });
                if (SteamPlayerProfile != null) return false;
                if (SteamPlayerProfile?.Steamid.Length < 1) return false;
                return postProfile;
            }

            return await GetSteamProfile().ConfigureAwait(false);
            // Task.Run(async () => await GetSteamProfile()).Wait();
        }

        // This will populate Game Combo Box
        // TODO: legame.editvalue shouldn't be defined by default. moreover with specific game name.
        public void PopulateGameComboBox(LookUpEdit leGame, LookUpEdit leVersion)
        {
            if(LoaderData == null) throw new Exception("NO META!??");
            if(LoaderData?.Games == null) throw new Exception("NO GAMES???");
            leGame.Properties.DataSource = LoaderData.Games;
            leGame.Properties.DisplayMember = "DisplayName";
            leGame.Properties.ValueMember = "ProcessName";
            //RGA
            leGame.EditValue = "SupremeRulerUltimate.exe";

            leGame.Properties.PopulateColumns();
            void WhatHappenWhenGameSelectionIsPoppingUp(object sender, CancelEventArgs args)
            {
                var sdr = sender as LookUpEdit;
                if (sender is not LookUpEdit) throw new Exception("LE NOT FOUND");
                if (sdr != null)
                    if (sdr.Properties.DataSource != null)
                    {
                        leGame.Properties.PopulateColumns();
                        var processNameColumn = leGame.Properties.Columns["ProcessName"];
                        processNameColumn.Visible = false;
                        var versionsColumn = leGame.Properties.Columns["Versions"];
                        versionsColumn.Visible = false;
                        var formatTypesColumn = leGame.Properties.Columns["SRFormatTypes"];
                        formatTypesColumn.Visible = false;
                        // sdr.Properties.DataSource = LoaderData.Games;
                    }
                // (le)?.Properties.PopulateColumns();
                // try
                // {
                //     var processNameColumn = ((LookUpEdit) sender).Properties.Columns["ProcessName"];
                //     processNameColumn.Visible = false;
                //     var versionsColumn = ((LookUpEdit) sender).Properties.Columns["Versions"];
                //     versionsColumn.Visible = false;
                //     var formatTypesColumn = ((LookUpEdit) sender).Properties.Columns["FormatTypes"];
                //     formatTypesColumn.Visible = false;
                // }
                // catch (Exception e)
                // {
                //     throw new Exception($"Error: {e.Message}");
                // }
            }

            leGame.QueryPopUp += WhatHappenWhenGameSelectionIsPoppingUp;

            Selected.GameProcessNameList = new string[LoaderData.Games.Count];
            if(LoaderData.Games.Count < 1) throw new Exception("No Game Data");
            for (int i = 0; i < LoaderData.Games.Count; i++)
            {
                Selected.GameProcessNameList[i] = LoaderData.Games[i].ProcessName;
            }
        }

        // Populating Controls
        public void PopulateVersionComboBox(LookUpEdit leGame, LookUpEdit leVersion)
        {
            // var gameId = leGame.Properties.GetDataSourceRowIndex("ProcessName", leGame.EditValue);
            leVersion.Properties.DataSource = leGame.Properties.GetDataSourceValue("Versions", leGame.ItemIndex);
            leVersion.Properties.DisplayMember = "GameVersion";
            leVersion.Properties.ValueMember = "GameVersion"; // 3.0.0.0, changed to Gameversion from availability
            // bug: just fixed evasive bug which stack trace failure to pinpoint the location.
            // bug: so i got string was not valid boolean stuff because of default value of editvalue setted as string.
            // bug: because valuemember contain boolean by default which i hadnt notice, i had to set editvalue to the same datatype instead of empty string.
            // leVersion.EditValue = false;

            void WhatHappenWhenVersionSelectionIsPoppingUp(object sender, CancelEventArgs args)
            {
                (sender as LookUpEdit)?.Properties.PopulateColumns();
                var pointersColumn = ((LookUpEdit) sender).Properties.Columns["Pointers"];
                    pointersColumn.Visible = false;
                // (sender as LookUpEdit).Properties.Columns["Availability"].Visible = false;
                var categoryColumn = ((LookUpEdit) sender).Properties.Columns["Categories"];
                    categoryColumn.Visible = false;
            }
            

            leVersion.QueryPopUp += WhatHappenWhenVersionSelectionIsPoppingUp;
        }

        public void SetComboBoxEvent(LookUpEdit leGame, LookUpEdit leVersion)
        {
            void LoaderGameValueChanged(object sender, EventArgs args)
            {
                var le = sender as LookUpEdit;
                if(le == null) return;
                var columnValue = le.GetColumnValue("DisplayName");
                if(columnValue == null) throw new Exception("No Column Value");
                
                var procName = le.EditValue; 
                if(procName == null) throw new Exception("No Process Name");

                var procNameWithoutExe = dotSplitter(procName).ToString();
                if (procNameWithoutExe != null) Selected.GameProcessName = procNameWithoutExe;
                Selected.GameName = columnValue.ToString();;
                UpdateVersionCombobox(leGame, leVersion);
            }
            leGame.EditValueChanged += LoaderGameValueChanged;
            
            void LoaderVersionValueChanged(object sender, EventArgs args)
            {
                if (Selected.GameState) return;
                if (Selected.UpdateState == UpdateStatusEnum.Idle)
                {
                    var test = (sender as LookUpEdit).EditValue.ToString();
                    Selected.GameVersion = test;
                    Selected.VersionState = rw.SetGameVersion(Selected.GameVersion);
                    // TODO: V3 SRP Violation
                    // this shouldn't be here,
                    // Loader only pass information flag about the current state of the UI
                    // Then checking wheter the process is valid according its Name/Version sould be done by ProcessManager 
                    if (Selected.VersionState == false)
                        rw.LoadProcess(Selected.GameProcessName);
                }
                // rw.LoadGameVersion(Selected.GameProcess);
            }
            leVersion.EditValueChanged += LoaderVersionValueChanged;
        }
        
        public void UpdateVersionCombobox(LookUpEdit leGame, LookUpEdit leVersion)
        {
            // var gameId = leGame.Properties.GetDataSourceRowIndex("ProcessName", leGame.EditValue);
            if (leVersion.Properties.DataSource == null)
            {
                leVersion.Properties.DataSource = leGame.Properties.GetDataSourceValue("Versions", leGame.ItemIndex);
                leVersion.Properties.DisplayMember = "GameVersion";
                leVersion.Properties.ValueMember = "Availability";
            } 
        }

        public void initCheckBox(CheckEdit ceGame, CheckEdit ceVersion, CheckEdit ceGameProcess, CheckEdit ceTrainerStatus)
        {
            // ceGame.BackColor = Color.Brown;
            // ceVersion.BackColor = Color.Brown;
            // ceTrainerStatus.BackColor = Color.Brown;
            // ceGameProcess.BackColor = Color.Brown;
            ceGame.Checked = Selected.GameState;

            if (ceGame.Checked && Selected.GameState)
            { 
                ceVersion.Checked = Selected.VersionState;
                if (rw.Selected.GameProcess != null)
                {
                    ceGameProcess.Checked = !rw.Selected.GameProcess.HasExited;
                }
                ceTrainerStatus.Checked = Selected.TrainerAvailability;
            }
            else
            {
                ceVersion.Checked = false;
                if (rw.Selected.GameProcess != null)
                    ceGameProcess.Checked = rw.Selected.GameProcess.HasExited;
                ceTrainerStatus.Checked = Selected.TrainerAvailability;
            }
            
            // ceGameProcess.Checked = rw.loaded && SelectedGameState && SelectedGameVersionState && rw.theProc != null;
        }

        public ReadOnlySpan<char> dotSplitter(object s)
        {
            string str = s.ToString();
            return str.AsSpan().Slice(0, str.IndexOfInvariantCulture("."));
        }
        // Assign/Coupiling checked box to LookUpEdit
        public void GameCheckedBox(CheckEdit ce, LookUpEdit le)
        {
            if (le.ItemIndex != -1)
            {
                // var procName = le.GetColumnValue("ProcessName");
                var gameName = le.GetColumnValue("DisplayName").ToString();
                var procNameWithoutExe = dotSplitter(le.EditValue.ToString());
                // Selected.GameProcessName = procNameWithoutExe;
                // Selected.GameName = gameName;
                Selected.GameState = rw.SetGameSelection(gameName, procNameWithoutExe.ToString());
                // if(rw.Selected.GameProcess != null) 
                    // Selected.GameState = string.Equals(procNameWithoutExe, rw.Selected.GameProcess.ProcessName, StringComparison.CurrentCultureIgnoreCase);
            }
            
            ce.Checked = Selected.GameState;
        }

        public void VersionCheckedBox(CheckEdit ce, LookUpEdit le)
        {
            // if (false) return;
            if (le.ItemIndex == -1) return;
            // if(le.Properties.Columns)
            if (Selected.GameState)
            {
                Selected.GameVersion = le.GetColumnValue("GameVersion").ToString();
                Selected.VersionState = rw.SetGameVersion(Selected.GameVersion);
            } 
            Selected.TrainerAvailability = Convert.ToBoolean(le.GetColumnValue("Availability"));
        }

        public void ForceUpdate()
        {
            CheckForUpdate(CurrentProductVersion, CurrentProductRevision);
            // Update(TrainerUpdateEnum.NewRevision);
        }
        public void ReloadTrainerData()
        {
            // TODO: V3 Pre, dispose wont neede for auto realod
            // jr.Dispose();
            if (LoaderData != null) 
                jr.Load(LoaderData, rw);
        }
        public void AutoReloadTrainerData()
        {
            ActiveTrainer.Instance.AutoReloadState = AutoReloadEnum.Checking;
            if (SRMain.Instance.FeaturePointerStore == null) return;

            var staticAddr = SRMain.Instance.FeaturePointerStore["Treasury"];
            var dynamicAddr = SRMain.Instance.FeaturePointerStoreRaw?["Treasury"];
            if(dynamicAddr == null) return;
            if(staticAddr == null) return;
            if (rw.Read("float", staticAddr) != rw.Read("float", dynamicAddr))
            {
                ActiveTrainer.Instance.AutoReloadState = AutoReloadEnum.Reloading;
                ReloadTrainerData();
            }
        }

        bool IsFileExist(string path, string fileName = "", string ext = "", string ext2 = "")
        {
            var dir = Path.Combine(path, fileName + ext + ext2);
            if (File.Exists(dir)) return true;
            return false;
        }

        byte[] ImageToBytes(string path, string fileName, string ext = "", string ext2 = "")
        {
            var dir = Path.Combine(path, fileName+ext+ext2);
            if (File.Exists(dir))
            {
                using (FileStream fs = new FileStream(dir, FileMode.Open, FileAccess.Read, FileShare.Read))
                {

                    // Do a blocking read
                    int index = 0;
                    long fileLength = fs.Length;
                    if (fileLength > Int32.MaxValue)
                        throw new IOException("Something went wrong.");
                    int count = (int) fileLength;
                    byte[] bt = new byte[count];
                    while (count > 0)
                    {
                        int n = fs.Read(bt, index, count);
                        index += n;
                        count -= n;
                    }

                    return bt;
                }
            }
            return null;
        }
        public async Task GetSRPlayers()
        {
            try
            {
                var fetchedSRPlayers = await apis.GetSRPlayers().ConfigureAwait(false);
                var smallPath = Path.Combine(Path.GetTempPath(), "Heapstech", "SR Helper", "cache", "avatar", "small");
                var mediumPath = Path.Combine(Path.GetTempPath(), "Heapstech", "SR Helper", "cache", "avatar", "medium");
                if(!Directory.Exists(smallPath)) Directory.CreateDirectory(smallPath);
                if(!Directory.Exists(mediumPath)) Directory.CreateDirectory(mediumPath);

                if (fetchedSRPlayers != null)
                {
                    var users = new SRPlayers();
                    users.Players = new List<SRPlayer>();
                    var filteredSRPlayers = fetchedSRPlayers.Players
                        .GroupBy(x => x.STEAM.Steamid)
                        .Select(x => x.Last())
                        .ToList();
                    Parallel.Invoke( () =>
                    {
                        Parallel.ForEach(filteredSRPlayers, (SRClient player) =>
                        {
                            var newPlayer = new SRPlayer
                            {
                                Steamid = player.STEAM.Steamid,
                                IsOnline = player.IsOnline,
                                Personaname = player.STEAM.Personaname,
                                Avatar = ImageToBytes(smallPath, player.STEAM.Avatarhash, ".jpg"),
                                Avatarhash = player.STEAM.Avatarhash,
                                Avatarmedium = null,
                                Profilestate = player.STEAM.Profilestate,
                                Profileurl = player.STEAM.Profileurl,
                                Communityvisibilitystate = player.STEAM.Communityvisibilitystate,
                            };
                            if (newPlayer == null) return;
                            users.Players.Add(newPlayer);
                            if (player.STEAM.Steamid == SteamPlayerProfile.Steamid)
                            {
                                users.Me = newPlayer;
                                users.Me.Avatarmedium = ImageToBytes(mediumPath, SteamPlayerProfile.Avatarmedium,
                                    "_medium.jpg");
                                users.Me.IsOnline = true;
                            }
                        });
                    }, () =>
                    {
                        Parallel.ForEach(filteredSRPlayers, async player =>
                        {
                            if (!IsFileExist(smallPath, player.STEAM.Avatarhash, ".jpg"))
                                await player.STEAM.Avatar.DownloadFileAsync(smallPath).ConfigureAwait(false);
                            if (!IsFileExist(mediumPath, player.STEAM.Avatarhash, "_medium.jpg"))
                                await player.STEAM.Avatarmedium.DownloadFileAsync(mediumPath).ConfigureAwait(false);
                        });
                    });

                    // var ss = new ImageCollection().
                    SRPlayers = users;
                    // if (SRPlayers != null)
                    // {
                    //     filteredSRPlayers = null;
                    //     users  = null;
                    //     smallPath = null;
                    //     mediumPath = null;
                    // }
                }
                // return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        

        [DoNotPrune]
        XtraForm CreateForm(XtraForm loader, Timer loaderTimer)
        {
            if (_trainerForm != null) return _trainerForm;
            _trainerForm = new MainForm();
            _trainerForm.Text = $@"{LoaderData.SRFName}";
            _trainerForm.Tag = $@"{LoaderData.SRFVersion}";

            void OnTrainerFormOnClosed(object sender, EventArgs args)
            {
                foreach (var category in SrConfig.DataMain.Games[0].Versions[0].Categories)
                {
                    if (category.features != null) category.features.DisposeSequence();
                }

                loaderTimer.Interval = 1000;
                _trainerForm?.Dispose();
                _trainerForm = null;
                loader.Show();
            }

            _trainerForm.Closed -= OnTrainerFormOnClosed;
            _trainerForm.Closed += OnTrainerFormOnClosed;
            return _trainerForm;
        }
        
        [DoNotPrune]
        public void LoadTrainerForm(XtraForm loaderForm, SimpleButton btn, Timer tmr)
        {
            
            async void OnBtnOnClick(object sender, EventArgs args)
            {
                _trainerForm = null;
                jr = SRMain.Instance;
                jr.activeTrainer = ActiveTrainer.Instance; 
                SRMain.Instance.Load(LoaderData, rw);

                // if(await PostSteamProfile().ConfigureAwait(false)) 
                //     await GetSRPlayers();
                // if(await PostSteamProfile().ConfigureAwait(false)) 
                    // await GetSRPlayers().ConfigureAwait(false);
                
                CreateForm(loaderForm, tmr).Show();
                loaderForm.Hide();
                // try
                // {
                // }
                // catch (Exception e)
                // { 
                //     ShowMessageBox($"EXCEPTION ${e}", "Create Form");
                //     throw;
                // }
                
                ((SRLoaderForm)loaderForm).PostProcedure();
                tmr.Interval = 10000;
            }

            
            btn.Click += OnBtnOnClick;
        }

        public bool TrainerIsAvailable()
        {
            if (Selected.GameState && Selected.VersionState && Selected.TrainerAvailability)
                return true;
            return false; 
        }

        public void CloseTrainerForm()
        {
            if (_trainerForm != null)
            {
                _trainerForm.Close();
            }
        }

        // TODO: Simplify
        public void CheckMandatoryUpdate()
        {
            if (SrApiConfig.OfflineMode) return;
            Task.Factory.StartNew(async (apicall) =>
            {
                var aCall = (apicall as SRApi);
                if (aCall == null)
                {
                    ShowMessageBox("API is not available", "Info");
                    return;
                }

                var check = await aCall.CheckDataUpdate();
                var meta = check;
                if (meta == null)
                {
                    ShowMessageBox("Something went wrong", "Check Data Update");
                    return;
                }

                if (CurrentProductRevision != meta.SRFRevision)
                {
                    ShowMessageBox("There is new revision, click the update button, to update.",
                        "New Data Revision: " + meta.SRFRevision);
                }
                if (CurrentProductVersion != meta.SRFVersion)
                {
                    if (meta.SRFMandatoryUpdate.Equals(true))
                    {
                        ShowMessageBox(meta.SRFMandatoryUpdateMessage ?? "????", "New Version: " + meta.SRFVersion);
                        if (meta.SRFChangelog != null)
                            if(_loaderForm.InvokeRequired) 
                                _loaderForm.Invoke(() => GenerateDownloadLink(meta));
                        Environment.Exit(99);
                    }
                    else
                    {
                        ShowMessageBox(meta.SRFUpdateMessage ?? "????", "New Version: " + meta.SRFVersion);
                        if (meta.SRFChangelog != null)
                            if(_loaderForm.InvokeRequired) 
                                _loaderForm.Invoke(() => GenerateDownloadLink(meta));
                    }
                }
            }, apis);
            // if(UpdateAvailable)
        }

        public async Task<int> GetSteamPlayerCount()
        {
            try
            {
                var playerCount = await apis.SteamGetPlayersCount().ConfigureAwait(false);
                SteamPlayerCount = playerCount?.response.player_count ?? 0;
                return playerCount?.response.player_count ?? 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public SRLoader(XtraForm loaderForm)
        {
            this._loaderForm = loaderForm;
        }
        public SRLoader Init()
        {
            // rw = new SRReadWrite();
            // Load User Settings
            UserLookAndFeel.Default.SkinName = Properties.Settings.Default.UserSkin;
            // This will load Main Data
            // Downloading or Decrypting
            // Then result can be retrieved from C.DATA
            var result = Task.Run(LoadMainData).ConfigureAwait(false);
            result.GetAwaiter().GetResult();

            //test
            try
            {
                if(result.GetAwaiter().IsCompleted) 
                    if (SrConfig.DataMain == null) 
                        LoaderData = SrConfig.DataMain;
                
                var approval = DeviceApproval.CheckForApproval(apis).ConfigureAwait(false);
                SRDeviceApproval = approval.GetAwaiter().GetResult();
                if (!result.GetAwaiter().IsCompleted && LoaderData == null)
                {
                    ShowMessageBox("Mandatory data couldn't get loaded", "Error");
                    Environment.Exit(1);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Metadata not found");
            }
            

            if (LoaderData != null)
            {
                // var initapiTask = Task.Factory.StartNew(CheckMandatoryUpdate);
                // try
                // {
                //     Task.WaitAll(initapiTask);
                // }
                // catch (Exception e)
                // {
                //     Console.WriteLine(e);
                //     throw;
                // }
                CurrentProductRevision = LoaderData.SRFRevision;
            }
            
            return this;
            // var initApiTask = Task.Run(() => CheckMandatoryUpdate());
            // try
            // {
            //     Task.WaitAll(initApiTask);
            //     var tt = initApiTask;
            //     XtraMessageBox.Show(tt.IsCompleted.ToString());
            //     currentProductRevision = LoaderData.SRFRevision;
            //     XtraMessageBox.Show(currentProductRevision);
            // }
            // catch (Exception e)
            // {
            //     XtraMessageBox.Show("[SRL] Loader can't be initialized");
            //     throw;
            // }
        }
    }
}