using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using Newtonsoft.Json;

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
        Error,
    }
    public class ActiveTrainer
    {
        public string[] GameProcessNameList { get; set; }
        public string GameName { get; set; }
        public string GameProcessName { get; set; }
        public System.Diagnostics.Process GameProcess { get; set; } = null;
        public string GameVersion { get; set; }
        public int GamePID { get; set; }
        public bool GameValidated { get; set; }
        
        public bool TrainerAvailability { get; set; }
        public bool GameState { get; set; }
        public bool VersionState { get; set; }
        
        private static readonly Lazy<ActiveTrainer> _instance = new Lazy<ActiveTrainer>(() => new ActiveTrainer());

        public static ActiveTrainer Instance => _instance.Value;

    }
    public class SRLoader
    {
        public string currentProductVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public string currentProductRevision = null;
        public bool LocalDataExist;
        public bool UpdateAvailable;
        public ActiveTrainer Selected { get; set; } = ActiveTrainer.Instance;

        private XtraForm _trainerForm;
        // public SRFApis api = SRFApis.Instance;
        public readonly SRFApis apis = SRFApis.Instance;
        private Root _mainData = null;
        private SREncryptor _encryptor = SREncryptor.Instance;
        // public SREncryptor SrEncryptor = SREncryptor.Instance;
        public SRMain jr;
        public SRReadWrite rw = new SRReadWrite();
        // private static readonly Lazy<SRLoader> _srLoaderInstance = new Lazy<SRLoader>(() => new SRLoader());
        // private string SRFileTmpPathName = System.IO.Path.GetTempPath() + "SRFramework" + ".tmp";
        // private string SRFileInputTmp = ".tmp";

        // public static SRLoader LoaderInstance => _srLoaderInstance.Value;
        public TrainerUpdateEnum CheckForUpdate(string ProductVersion, string ProductRevision)
        {
            async Task<TrainerUpdateEnum> check()
            {
                var tt = await apis.CheckDataUpdate();
                if (tt == null) return TrainerUpdateEnum.Error;
                if (tt.SRFRevision != ProductRevision && tt.SRFVersion != ProductVersion)
                    return TrainerUpdateEnum.NewVersionNewRevision;
                if (tt.SRFRevision != ProductRevision)
                    return TrainerUpdateEnum.NewRevision;
                if (tt.SRFVersion != ProductVersion)
                    return TrainerUpdateEnum.NewVersion;
                return TrainerUpdateEnum.NoVersionNoRevision;
            }

            return Task.Run(async () => await check()).Result;


            // var newVersion = await CheckNewVersion(ProductVersion);
            // var newRevision = await CheckNewRevision(ProductRevision);
            // Task.WaitAll(new Task[] {newRevision, newVersion});
            // bool NewVersion = false;
            // bool NewRevision = false;
            //
            // async Task<List<TrainerUpdateEnum>> Check()
            // {
            //     var newUpdate = await apis.CheckDataUpdate();
            //     List<TrainerUpdateEnum> result = new List<TrainerUpdateEnum>();
            //     if (newUpdate.SRFVersion != ProductVersion)
            //     {
            //         result.Add(TrainerUpdateEnum.NewVersion);
            //         NewVersion = true;
            //     } 
            //     if (newUpdate.SRFRevision != ProductRevision)
            //     {
            //         result.Add(TrainerUpdateEnum.NewRevision);
            //         NewRevision = true;
            //     }
            //
            //     return result;
            // }
            //
            // Task.Run(async () =>
            // {
            //     var c = await Check();
            //     if (c.Contains(TrainerUpdateEnum.NewRevision))
            //     {
            //         
            //     }
            // })

            // bool UpdateRevision()
            // {
            //     // if (!NewRevision) return;
            //     return DownloadNewData().Result;
            // }
            // bool UpdateVersion()
            // {
            //     // if (!NewRevision) return;
            //     return true;
            // }

            // if (Check().GetAwaiter().GetResult().Contains(TrainerUpdateEnum.NewRevision))
            // {
            //     XtraMessageBox.Show("New Revision update", "Information", MessageBoxButtons.OK);
            //     UpdateRevision();
            // }
            //
            // if (Check().GetAwaiter().GetResult().Contains(TrainerUpdateEnum.NewVersion))
            // {
            //     XtraMessageBox.Show("New Helper Version! download", "Information", MessageBoxButtons.OK);
            //     // UpdateRevision();
            // }
            // Task.Run(async () =>
            // {
            // });
            // task.IsCompleted

            // return newUpdate == TrainerUpdateEnum.NewVersion || newUpdate == TrainerUpdateEnum.NewRevision;
            // if (newVersion.GetAwaiter().GetResult() == TrainerUpdateEnum.NewVersion)
            // {
            //     return newVersion;
            //     MessageBox.Show("New Version available");
            // }
            //
            // if (newRevision.GetAwaiter().GetResult() == TrainerUpdateEnum.NewRevision)
            // {
            //     MessageBox.Show("New Revision Available");
            // }
            // newVersion.GetAwaiter().GetResult()
            // await Task.Run(() =>
            // {
            //     UpdateAvailable = CheckNewVersion(ProductVersion).Result;
            // });
            // var newRevision = CheckNewRevision(ProductRevision).GetAwaiter().GetResult();
        }

        public bool Update(TrainerUpdateEnum t)
        {
            bool updateResult;
            switch (t)
            {
                case TrainerUpdateEnum.NewRevision:
                    if (!Task.Run(() => DownloadNewData()).Result) return false;
                    currentProductRevision = apis.Data.SRFRevision;
                    return true;
                case TrainerUpdateEnum.NewVersion:
                    if(!Task.Run(() => DownloadNewData()).Result) return false;
                    return true;
                case TrainerUpdateEnum.NewVersionNewRevision:
                    updateResult = Task.Run(() => DownloadNewData()).Result;
                    if (!updateResult) return false;
                    // Revision is updated;
                    currentProductRevision = apis.Data.SRFRevision;
                    return true;
                default:
                    return false;
                    break;
            }

            return false;
        }

        public void GenerateDownloadLink(ChangeLogViewer clv = null)
        {
            // XtraMessageBoxArgs args = new XtraMessageBoxArgs();
            // // args.AutoCloseOptions.Delay = 5000;
            // args.AllowHtmlText = DefaultBoolean.True;
            // args.Caption = "<b>Update Download</b>";
            // args.Text = "Open Link with default browser ?";
            // args.Buttons = new DialogResult[] { DialogResult.OK, DialogResult.Cancel};
            // args.AllowHtmlText = DefaultBoolean.True;
            // var dialogResult = XtraMessageBox.Show(args);
            if (clv != null)
            {
                if (DevExpress.XtraEditors.XtraDialog.Show(clv, "New Update", MessageBoxButtons.OKCancel) 
                    ==
                    DialogResult.OK)
                {
                    System.Diagnostics.Process.Start(apis.Data.SRFDownloadLink);
                }
            }
            // if (dialogResult == DialogResult.OK)
            // {
            //     System.Diagnostics.Process.Start(apis.Data.SRFDownloadLink);
            // }
        }

        public void SetUserDeviceRef(string id)
        {
            apis.userRefId = id;
        }

        public void LoadMainData()
        { 
            DownloadOrDecrypt();
            // jr = JSONReader.Instance;
        }

        public void DownloadOrDecrypt()
        {
            LocalDataExist = (File.Exists(apis.filename));
            if (!LocalDataExist)
            {
                LocalDataExist = DownloadNewData().Result;
            }
            else
            {
                if (!DecryptData()) return;
                currentProductRevision = apis.Data.SRFRevision;
                // if (CheckForUpdate(currentProductVersion, currentProductRevision).Result)
                //     MessageBox.Show("New Update/Revision Available");
            }
        }

        // Download instead of Decrypt
        public async Task<bool> DownloadNewData()
        {
            var fetchData = await apis.FetchSRFrameworkData();
            var data = fetchData; 
            apis.JFileSave(data, "SRFramework"); 
            LocalDataExist = File.Exists(_encryptor.filename);
            return LocalDataExist;
        }

        // Decrypt instead of download
        public bool DecryptData()
        {
            var f = File.ReadAllBytes(apis.filename);
                var st = Encoding.UTF8.GetString(f);
                var js = JsonConvert.DeserializeObject<APIEncryptedBody>(st);
                apis.JBufferDecrypt(js, "SRFramework");
                if (apis.Data != null)
                    return true;
                return false;
        }

        public async void GetEncryptedData()
        {
            // var data = await FetchSRFrameworkData().ConfigureAwait(true);
            // // var buffer = new byte[50000];
            // // byte[] buffer = Encoding.UTF8.GetBytes(data.body);
            // // SrEncryptor.BufferDecryption(data.body, "test");
            // // if(CheckForUpdate())
            // // LocalDataExist = JFileSave(data, "SRFramework");
            // // UpdateAvailable = CheckForUpdate(currentProductVersion, Data.SRFRevision);
            //
            // if (LocalDataExist)
            //     sr = JSONReader.CreateSingleton(Data);
            // // JSONReader.Instance.Load(Data);
            // MessageBox.Show(sr.Data.SRFAuthor);
            //
            // Task.Run(async  () =>
            //     {
            //         await DeviceRegister();
            //     }).GetAwaiter().GetResult();
            //     // Debug.WriteLine(data.body);
        }

        public async Task<APIRegisterClient> DeviceRegister()
        {
            SRClient clientDevice = SRUtils.Instance.GetClientDevice();
            APIRegisterClient test = await apis.RegisterNewClient(clientDevice);
            // Debug.WriteLine(test.refId);
            return test;
        }

        // This will populate Game Combo Box
        public void PopulateGameComboBox(LookUpEdit leGame, LookUpEdit leVersion)
        {
            leGame.Properties.DataSource = apis.Data.Games;
            leGame.Properties.DisplayMember = "DisplayName";
            leGame.Properties.ValueMember = "ProcessName";
            leGame.EditValue = "SupremeRulerUltimate.exe";
            // leGame.EditValueChanged += (sender, args) =>
            // {
            //     var gameName = leGame.GetColumnValue("DisplayName").ToString();
            //     var procNameWithoutExe = leGame.EditValue.ToString().Split('.')[0];
            //     Selected.GameProcessName = procNameWithoutExe;
            //     Selected.GameName = gameName;
            //     UpdateVersionCombobox(leGame, leVersion);
            // };

            leGame.QueryPopUp += (sender, args) =>
            {
                (sender as LookUpEdit).Properties.PopulateColumns();
                (sender as LookUpEdit).Properties.Columns["ProcessName"].Visible = false;
                (sender as LookUpEdit).Properties.Columns["Versions"].Visible = false;
            };

            Selected.GameProcessNameList = new string[apis.Data.Games.Count];
            for (int i = 0; i < apis.Data.Games.Count; i++)
            {
                Selected.GameProcessNameList[i] = apis.Data.Games[i].ProcessName;
            }
        }
        public void PopulateVersionComboBox(LookUpEdit leGame, LookUpEdit leVersion)
        {
            // var gameId = leGame.Properties.GetDataSourceRowIndex("ProcessName", leGame.EditValue);
            leVersion.Properties.DataSource = leGame.Properties.GetDataSourceValue("Versions", leGame.ItemIndex);
            leVersion.Properties.DisplayMember = "GameVersion";
            leVersion.Properties.ValueMember = "Availability";
            leVersion.EditValue = "";
            // leVersion.EditValueChanged += (sender, args) =>
            // {
            //     if (!Selected.GameState) return;
            //     Selected.GameVersion = leVersion.GetColumnValue("GameVersion").ToString();
            //     rw.SetGameVersion(Selected.GameVersion);
            //     // rw.LoadGameVersion(Selected.GameProcess);
            // };
            
            // leVersion.QueryPopUp += (sender, args) =>
            // {
            //     (sender as LookUpEdit).Properties.PopulateColumns();
            //     (sender as LookUpEdit).Properties.Columns["Pointers"].Visible = false;
            //     // (sender as LookUpEdit).Properties.Columns["Availability"].Visible = false;
            //     (sender as LookUpEdit).Properties.Columns["Categories"].Visible = false;
            // };
        }

        public void SetComboBoxEvent(LookUpEdit leGame, LookUpEdit leVersion)
        {
            leGame.EditValueChanged += (sender, args) =>
            {
                var gameName = leGame.GetColumnValue("DisplayName").ToString();
                var procNameWithoutExe = leGame.EditValue.ToString().Split('.')[0];
                Selected.GameProcessName = procNameWithoutExe;
                Selected.GameName = gameName;
                UpdateVersionCombobox(leGame, leVersion);
            };
            leVersion.EditValueChanged += (sender, args) =>
            {
                if (!Selected.GameState) return;
                Selected.GameVersion = leVersion.GetColumnValue("GameVersion").ToString();
                rw.SetGameVersion(Selected.GameVersion);
                // rw.LoadGameVersion(Selected.GameProcess);
            };
        }
        
        public void UpdateVersionCombobox(LookUpEdit leGame, LookUpEdit leVersion)
        {
            // var gameId = leGame.Properties.GetDataSourceRowIndex("ProcessName", leGame.EditValue);
            leVersion.Properties.DataSource = leGame.Properties.GetDataSourceValue("Versions", leGame.ItemIndex);
            leVersion.Properties.DisplayMember = "GameVersion";
            leVersion.Properties.ValueMember = "Availability";
        }

        public bool RefreshProcess()
        {
            // Debug.WriteLine(Selected);
            if (!String.IsNullOrEmpty(Selected.GameName) && !String.IsNullOrEmpty(Selected.GameProcessName))
            {
                // rw.Selected = Selected;
                // rw.SetGameSelection(Selected.GameName, Selected.GameProcessName); 
                // if (!String.IsNullOrEmpty(Selected.GameVersion)) 
                //     rw.SetGameVersion(Selected.GameVersion);
                // rw.Load(Selected.GameName, Selected.GameProcessName, Selected.GameVersion);
                return true;
            }

            return false;
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
                    ceGameProcess.Checked = !rw.Selected.GameProcess.HasExited;
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
        public void GameCheckedBox(CheckEdit ce, LookUpEdit le)
        {
            if (le.ItemIndex != -1)
            {
                // var procName = le.GetColumnValue("ProcessName");
                var procName = le.EditValue.ToString();
                var gameName = le.GetColumnValue("DisplayName").ToString();
                var procNameWithoutExe = procName.ToString().Split('.')[0];
                // Selected.GameProcessName = procNameWithoutExe;
                // Selected.GameName = gameName;
                Selected.GameState = rw.SetGameSelection(gameName, procNameWithoutExe);
                // if(rw.Selected.GameProcess != null) 
                    // Selected.GameState = string.Equals(procNameWithoutExe, rw.Selected.GameProcess.ProcessName, StringComparison.CurrentCultureIgnoreCase);
            }
            
            ce.Checked = Selected.GameState;
        }

        public void VersionCheckedBox(CheckEdit ce, LookUpEdit le)
        {
            if (le.ItemIndex == -1) return;
            if (Selected.GameState)
            {
                Selected.GameVersion = le.GetColumnValue("GameVersion").ToString();
                Selected.VersionState = rw.SetGameVersion(Selected.GameVersion);
            }
            Selected.TrainerAvailability = (bool) le.GetColumnValue("Availability");
        }

        public void LoadTrainerForm(XtraForm loaderForm, SimpleButton btn, Timer tmr)
        {
            XtraForm CreateForm(XtraForm loader)
            {
                if (_trainerForm != null) return _trainerForm;
                _trainerForm = new XtraForm1();
                _trainerForm.Text = $"{apis.Data.SRFName}";
                _trainerForm.Tag = $"{apis.Data.SRFVersion}";
                _trainerForm.Closed += (sender, args) =>
                {
                    tmr.Interval = 1000;
                    _trainerForm.Dispose();
                    loader.Show();
                };
                return _trainerForm;
            }
            // CreateForm().Show();
            // var tm = new Timer {Interval = 1000};
            // tm.Tick += (sender, args) =>
            // {
            //     Debug.WriteLine("Still Working");
            //     if (!CreateForm().IsDisposed) return;
            //     loaderForm.Show();
            //     tm.Stop();
            // };
            void OnBtnOnClick(object sender, EventArgs args)
            {
                _trainerForm = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                // tm.Enabled = true;
                // tm.Start();
                jr = SRMain.Instance;
                jr.activeTrainer = ActiveTrainer.Instance;
                SRMain.Instance.Load(apis.Data);
                CreateForm(loaderForm).Show();
                loaderForm.Hide();
                tmr.Interval = 7000;
            }

            btn.Click += OnBtnOnClick;
        }

        public bool TrainerIsAvailable()
        {
            if (Selected.GameState && Selected.VersionState && Selected.TrainerAvailability)
                return true;
            else
                return false;
        }

        public void CloseTrainerForm()
        {
            if (_trainerForm != null)
            {
                _trainerForm.Close();
            }
        }
        public SRLoader()
        {
            // This will load Main Data
            // Downloading or Decrypting
            // Then result can be retrieved from C.DATA
            LoadMainData();
            currentProductRevision = apis.Data.SRFRevision;

            // Websocet for later on
            // using (var ws = new WebSocketSharp.WebSocket("ws://localhost:3333/socket.io/?EIO=2&transport=websocket"))
            // {
            //     ws.OnMessage += (sender, e) =>
            //         Console.WriteLine("New message from controller: " + e.Data);
            //     ws.Connect();
            // }
        }
    }
}