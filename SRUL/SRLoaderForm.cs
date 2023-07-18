using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraDialogs.FileExplorerExtensions;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native.Extensions;
using DevExpress.XtraSplashScreen;
using SRUL.Properties;
using SRUL.Views;
using Settings = SRUL.Properties.Settings;

namespace SRUL
{
    public partial class SRLoaderForm : XtraForm
    {
        public static SRLoader _srLoader;
        private static SRMain jr;
        // private readonly SRAds _srAds = SRAds.Instance;
        public SRLoaderForm()
        {

            SplashScreenManager.ShowForm(typeof(SRLoaderFormSplash));
            InitializeComponent();
            cmd("Starting..");
            _srLoader = new SRLoader(this);
            _srLoader.rw = new SRReadWrite();
            
            
            cmd("Initialization..");
            _srLoader.Init();

            // SRAds.Instance.Init().Await();
            // SRLoader.srAdsManager = new SRAdsManager();

            cmd("Initialized..");
            if (SrConfig.DataMain != null)
            {
                jr = SRMain.CreateSingleton(SrConfig.DataMain);
                cmd("Core function loaded..");
            }
            else
            {
                jr = null;
                // XtraMessageBox.Show("1. Something is wrong with the data, contact developer!");
                Environment.Exit(2);
            }

            Debug.WriteLine(jr);
            cmd("Checking Trainer status");
            if (jr != null && jr.Data != null)
            {
                if(jr.Data.SRFStatus) 
                    cmd("Trainer status Active!");
            }
            else
            {
                // XtraMessageBox.Show("2. Something is wrong with the data, contact developer!");
                Environment.Exit(3);
            }
            
            cmd("Populating Data..");
            _srLoader.PopulateGameComboBox(leGameSelection, leGameVersionSelection);
            _srLoader.PopulateVersionComboBox(leGameSelection, leGameVersionSelection);
            _srLoader.SetComboBoxEvent(leGameSelection, leGameVersionSelection);
            _srLoader.LoadTrainerForm(this, simpleButton1, timerLoader);

            // cmd("Registering.");
            // var register = _srLoader.DeviceRegister().GetAwaiter();
            //
            // if (!register.IsCompleted)
            //     cmd("Registering...");

            var refId = "Unregistered";
            cmd("Creating Task..");
            // SRLoader.srAdsManager.AddAd(new ManagedInterstitialAd(AdsLoaderForm, true));
            // SRLoader.srAdsManager.ShowOnce();
            async Task DeviceRegistration()
            {
                try
                {
                    cmd("Awaiting task..");
                    var register = await _srLoader.DeviceRegister().ConfigureAwait(false);
                    if (register.refId != null) 
                        refId = register.refId;
                    Properties.Settings.Default.UserId = register.deviceId;
                    // return register;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            


            // async Task TaskGetSteamPlayers()
            // {
            //     lblPlayers.Invoke(new MethodInvoker(async () =>
            //     {
            //         lblPlayers.Text = (await _srLoader.GetSteamPlayerCount()).ToString();
            //     }));
            // }
            
            lblPlayers.Text = _srLoader.GetSteamPlayerCount().Result.ToString();
            // Running Task
            // cmd("waiting...");
            // SplashScreenManager.CloseForm();
            var registerTask = DeviceRegistration();
            registerTask.GetAwaiter().GetResult();
            cmd("Task awaited..");
            if (registerTask.IsCompleted)
            {
                _srLoader.SetUserDeviceRef(refId);
                registerTask.Dispose();
                cmd("All is well..");
                cmd("Ignite!");
                // SplashScreenManager.CloseDefaultWaitForm();
                // SplashScreenManager.CloseDefaultSplashScreen();
                SplashScreenManager.CloseForm();
                // lblTrainerStatus.Text = refId;
            }
            else
            {
                // registerTask.Dispose();
                cmd("Something went wrong!, continuing..");
                SplashScreenManager.CloseForm();
            }
            
            // register.OnCompleted(() =>
            // {
            //     cmd("Registered...");
            //     if (register.GetResult().refId != null)
            //     {
            //         lblTrainerStatus.Text = register.GetResult().refId;
            //         cmd("Registered..");
            //         
            //         cmd("Process Completed!");
            //         SplashScreenManager.CloseForm(); 
            //     }   
            // });
        }

        private void cmd (string txt) {
            try
            {
                if(SplashScreenManager.Default != null)
                SplashScreenManager.Default.SendCommand(SRLoaderFormSplash.SplashScreenCommand.ChangeText, txt);
                Thread.Sleep(50);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void refreshControl()
        {
            lblVersionValue.Text = _srLoader.CurrentProductVersion;
            lblRevisionValue.Text = _srLoader.CurrentProductRevision;
            _srLoader.PopulateGameComboBox(leGameSelection, leGameVersionSelection);
            _srLoader.PopulateVersionComboBox(leGameSelection, leGameVersionSelection);
        }
        
        private void SRLoaderForm_Load(object sender, EventArgs e)
        {
            // memoEdit1.Text = SRUtils.Instance.GetClientDevice().DeviceID;
            // _srAds.ShowAd(AdsLoaderForm, true,25000);
            // bannerAds1.ShowAd(300, 250, Properties.Settings.Default.ApplicationAdsKey);
            // bannerAds2.ShowAd(300, 250, Properties.Settings.Default.ApplicationAdsKey);

            timerLoader.Enabled = true;
            lblRevisionValue.Text = _srLoader.CurrentProductRevision;
            lblVersionValue.Text = _srLoader.CurrentProductVersion;

            var positiveColor = (Color) System.Drawing.ColorTranslator.FromHtml("#0088aa");
            var negativeColor = (Color) System.Drawing.ColorTranslator.FromHtml("#006680");
            var textColor = Color.White;

            void ceChanged(object s, EventArgs args)
            {
                var ce = s as CheckEdit;
                if (ce.CheckState == CheckState.Checked)
                {
                    ce.ForeColor = textColor;
                    ce.BackColor = negativeColor;
                    ce.Properties.Appearance.BackColor2 = Color.Crimson;
                }
                else
                {
                    ce.ForeColor = textColor;
                    ce.BackColor = Color.DimGray;
                }
            }

            void initCheckEditStyling(CheckEdit c)
            {
                c.ForeColor = textColor;
                c.BackColor = Color.DimGray;
                c.Properties.Appearance.BackColor2 = Color.Crimson;
                c.CheckStateChanged += ceChanged;
            }

            initCheckEditStyling(ceGameName);
            initCheckEditStyling(ceGameStatus);
            initCheckEditStyling(ceGameVersion);
            initCheckEditStyling(ceTrainerStatus);
            
            btnGameSelection.Click += BtnGameSelection_Click;
            
            // Check for mandatory Update
            // Task.Run(async () => await _srLoader.CheckMandatoryUpdate()).Wait();
            _srLoader.CheckMandatoryUpdate();
            
            

            if(!_srLoader.CheckForApproval()) 
                showAdOnce();
        }

        private bool _postProcedureAlreadyRan;
        public void PostProcedure()
        {
            if (_postProcedureAlreadyRan) return;
            // clearing ads artifact
            if (!AdsLoaderForm.Visible && _alreadyShown)
            {
                AdsLoaderForm.Dispose();
                GC.SuppressFinalize(this);
                System.GC.Collect(0, GCCollectionMode.Forced);
                System.GC.WaitForFullGCComplete();
                _postProcedureAlreadyRan = true;
            }
        }

        private bool _alreadyShown = false;
        private void showAdOnce()
        {
            Action ret = () =>
            {
                if (_alreadyShown == false)
                    AdsLoaderForm.ShowInterstitialAd(Properties.Settings.Default.ApplicationAdsKey);
                _alreadyShown = true;
            };
            if(_alreadyShown == false) 
                ret();
        }

        private void BtnGameSelection_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("steam://run/314980");
                _srLoader.rw.LoadProcess(_srLoader.Selected.GameProcessName);
                // _srLoader.rw.start
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show("Auto Update is disabled\nPlease download the data manually.");
                throw;
            }
        }
        public void GenerateDonateLinkPopup()
        {
            XtraDialogArgs args = new XtraDialogArgs(null, 
                new DonationLinkViewer(), 
                "Links", new[] { DialogResult.No }, 0);
            args.Showing += (sender, e) =>
            {
                e.Buttons[DialogResult.No].Text  = "No Way!";
            };
            XtraDialog.Show(args);
        }

        // Showing Donation links
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            GenerateDonateLinkPopup();
        }

        private bool LoaderStatus()
        {
            if (ceGameName.Checked && ceGameStatus.Checked)
                if (ceGameVersion.Checked && ceTrainerStatus.Checked)
                    return true;
            return false;
        }
        
        private void timerLoader_Tick(object sender, EventArgs e)
        {
//             memoEdit1.EditValue = $@"
// PID: {ActiveTrainer.Instance.GamePID}
// gName: {ActiveTrainer.Instance.GameName}
// pName: {ActiveTrainer.Instance.GameProcessName}
// v: {ActiveTrainer.Instance.GameVersion}
// Proc: {ActiveTrainer.Instance.GameProcess}
// Game State: {ActiveTrainer.Instance.GameState}
// Version State: {ActiveTrainer.Instance.VersionState}
// Validated: {ActiveTrainer.Instance.GameValidated}
// JR Revision: {jr.Data.SRFRevision}
// Apis Revision: {_srLoader.apis.Data.SRFRevision}
// ";
            
            if (LoaderStatus())
            {
                
                _srLoader.Selected.GameValidated = true;
                simpleButton1.Enabled = true;
            }
            else
            {
                _srLoader.Selected.GameValidated = false;
                simpleButton1.Enabled = false;
            }

            if (leGameSelection.ItemIndex == -1) return;
            
            
            _srLoader.UpdateVersionCombobox(leGameSelection, leGameVersionSelection);
            // if(_srLoader.rw.Selected.GameProcess != null)
            
            // Will check game checked box
            _srLoader.GameCheckedBox(ceGameName, leGameSelection);
 
            // Version checked box after game is check
            _srLoader.VersionCheckedBox(ceGameVersion, leGameVersionSelection);
            
            // if (!_srLoader.RefreshProcess())
            // {
            //     ceGameName.Checked = false;
            //     ceGameVersion.Checked = false;
            //     ceGameStatus.Checked = false;
            // }
            // Init all the checkboxes
            _srLoader.initCheckBox(ceGameName, ceGameVersion, ceGameStatus, ceTrainerStatus);

            // TODO: v3 replaced with Reloader() in mainForm
            // if(_srLoader.LoaderData != null) 
            //     _srLoader.AutoReloadTrainerData();
            // lblGameStatus.Text = leGameSelection.EditValue.ToString();
        }

        public void UpdateDescription(string txt, int sleep)
        { 
            SplashScreenManager.Default.SetWaitFormDescription(txt);
            Thread.Sleep(sleep);
        }
        public void CheckUpdate()
        {
            if (SrApiConfig.OfflineMode)
            {
                XtraMessageBox.Show("Auto Update is disabled\nPlease download the data manually.");
                return;
            }
            SplashScreenManager.ShowForm(this, typeof(SRWaitForm), 
                true, 
                true, false);
            var check = _srLoader
                .CheckForUpdate(_srLoader.CurrentProductVersion, _srLoader.CurrentProductRevision);
            switch (check)
            {
                case TrainerUpdateEnum.NewVersion:
                    UpdateDescription("New Version Available!", 1000);
                    // If Update Checked
                    if (_srLoader.Update(TrainerUpdateEnum.NewVersion))
                    { UpdateDescription("Download Link Available!", 1000); 
                        UpdateDescription("Please download new version", 1000); 
                    
                    // generate download link and changelog View
                    UpdateDescription("Generating download link...", 900); 
                    SplashScreenManager.CloseForm();
                    _srLoader.GenerateDownloadLink(SrConfig.DataMain.SRFChangelog);
                    }
                    else
                    {
                        UpdateDescription("Something went wrong..", 900); 
                        SplashScreenManager.CloseForm();
                    }
                    break;
                case TrainerUpdateEnum.NewRevision:
                    UpdateDescription("New Revision Available!", 1000);
                    UpdateDescription("Update revision.", 500);
                    var isUpdated = _srLoader.Update(TrainerUpdateEnum.NewRevision);
                    UpdateDescription("Update revision..", 500);
                    if (isUpdated)
                    { 
                        UpdateDescription("Updated completed!", 1000); 
                        refreshControl();
                        SplashScreenManager.CloseForm();
                    }
                    else
                    {
                        UpdateDescription("Update failed!", 1000); 
                        SplashScreenManager.CloseForm();
                    }
                    break;
                case TrainerUpdateEnum.NewVersionNewRevision:
                    UpdateDescription("Update available..", 1000);
                    UpdateDescription("Updating revision.", 500);
                    var updateRevision = _srLoader.Update(TrainerUpdateEnum.NewRevision);
                    UpdateDescription("Updating revision..", 500);
                    UpdateDescription("Updating revision...", 500);
                    UpdateDescription("Updating revision....", 500);
                    if (updateRevision)
                    {
                        refreshControl();
                        UpdateDescription("Revision Update Completed!", 1500);
                    }
                    var updateVersion = _srLoader.Update(TrainerUpdateEnum.NewVersion);
                    if (updateVersion)
                    {
                        if (SrConfig.DataMain?.SRFChangelog == null) break; 
                        _srLoader.GenerateDownloadLink(SrConfig.DataMain.SRFChangelog);
                        UpdateDescription("Generating download link.", 900);
                        UpdateDescription("Generating download link..", 900);
                        UpdateDescription("Generating download link...", 900);
                        SplashScreenManager.CloseForm();
                        break;
                    }
                    SplashScreenManager.CloseForm();
                    break;
                case TrainerUpdateEnum.NoVersionNoRevision:
                    UpdateDescription("No Update", 1500);
                    SplashScreenManager.CloseForm();
                    break;
                case TrainerUpdateEnum.Error:
                    UpdateDescription("Update error!", 1000);
                    SplashScreenManager.CloseForm();
                    break;
                default:
                    UpdateDescription("No Update", 1500);
                    SplashScreenManager.CloseForm();
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void btnCheckUpdate_Click(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        private void iconCheckUpdate_Click(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

        }

    }

}