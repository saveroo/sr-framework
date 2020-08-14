using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraRichEdit;
using DevExpress.XtraSplashScreen;
using Timer = System.Threading.Timer;

namespace SRUL
{
    public partial class SRLoaderForm : DevExpress.XtraEditors.XtraForm
    {
        public static SRLoader _srLoader;
        public static SRMain jr;
        public SRLoaderForm()
        {
            // SplashScreenManager.ShowForm(this, typeof(SRLoaderFormSplash), true, true, false);
            // SplashScreenManager.Default.SetWaitFormCaption("Initialization");
            // GoBack: 
            // for (int i = 0; i < 100; i++)
            // {
            //     Thread.Sleep(25);
            // }

            
            InitializeComponent();
            SplashScreenManager.ShowForm(typeof(SRLoaderFormSplash));  
            // your initialize1  
            
            cmd("Starting..");
            _srLoader = new SRLoader();
            
            cmd("Initialization..");
            jr = SRMain.CreateSingleton(_srLoader.apis.Data);
            cmd("Instantiated..");

            cmd("Checking Trainer status");
            if (jr.Data.SRFStatus)
            {
                cmd("Trainer status Active!");
            }
            else
            {
                XtraMessageBox.Show("Something is wrong with the data, contact developer!");
                Application.Exit();
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
            cmd("Creating task.");
            async Task DeviceRegistration()
            { 
                cmd("Running task..");
                var register = await _srLoader.DeviceRegister();
                if (register.refId != null) refId = register.refId;
            }
            
            Task.Run(() =>
            {
                lblPlayers.Text = _srLoader.GetSteamPlayerCount().Result.ToString();
            }).Wait();

            // Running Task
            var registerTask = Task.Run(DeviceRegistration);
            registerTask.ConfigureAwait(false);
            if (registerTask.IsCompleted)
            {
                cmd("Task Completed!");
                cmd("Ignite!");
                // lblTrainerStatus.Text = refId;
                _srLoader.SetUserDeviceRef(refId);
                SplashScreenManager.CloseForm(); 
                registerTask.Dispose();
            }
            else
            {
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
        
        public void cmd (string txt) {
            SplashScreenManager.Default.SendCommand(SRLoaderFormSplash.SplashScreenCommand.ChangeText, txt);
            Thread.Sleep(200);
        }

        public void refreshControl()
        {
            lblVersionValue.Text = _srLoader.currentProductVersion;
            lblRevisionValue.Text = _srLoader.currentProductRevision;
            _srLoader.PopulateGameComboBox(leGameSelection, leGameVersionSelection);
            _srLoader.PopulateVersionComboBox(leGameSelection, leGameVersionSelection);
        }
        
        private void SRLoaderForm_Load(object sender, EventArgs e)
        {
            timerLoader.Enabled = true;
            lblRevisionValue.Text = _srLoader.currentProductRevision;
            lblVersionValue.Text = _srLoader.currentProductVersion;

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

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Settings.Default.DonorboxLink);
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
            
            if (ceGameName.Checked 
                && ceGameStatus.Checked 
                && ceGameVersion.Checked 
                && ceTrainerStatus.Checked)
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
            
            if (leGameSelection.ItemIndex == -1) return;

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
            _srLoader.initCheckBox(ceGameName, ceGameVersion, ceGameStatus, ceTrainerStatus);

            // lblGameStatus.Text = leGameSelection.EditValue.ToString();
        }

        private void CheckUpdate()
        {
            void description(string txt, int sleep)
            { 
                SplashScreenManager.Default.SetWaitFormDescription(txt);
                Thread.Sleep(sleep);
            }
            SplashScreenManager.ShowForm(this, typeof(SRWaitForm), true, true, false);
            var check = _srLoader.CheckForUpdate(_srLoader.currentProductVersion, _srLoader.currentProductRevision);
            switch (check)
            {
                case TrainerUpdateEnum.NewVersion:
                    description("New Version Available!", 1000);
                    description("Generating Download Link!", 1000);
                    
                    // If Update Checked
                    if (_srLoader.Update(TrainerUpdateEnum.NewVersion))
                    { 
                        description("Download Link Available!", 1000); 
                        description("Please download new version", 1000); 
                    SplashScreenManager.CloseForm();                 
                    Thread.Sleep(200);
                    
                    // generate download link and changelog View
                    _srLoader.GenerateDownloadLink(new ChangeLogViewer(_srLoader.apis.Data.SRFChangelog));
                    }
                    break;
                case TrainerUpdateEnum.NewRevision:
                    description("New Revision Available!", 1000);
                    description("Update revision.", 1000);
                    description("Update revision..", 1000);
                    var isUpdated = _srLoader.Update(TrainerUpdateEnum.NewRevision);
                    description("Update revision...", 1000);
                    if (isUpdated)
                    { 
                        description("Updated completed!", 1000); 
                        refreshControl();
                        SplashScreenManager.CloseForm();
                    }
                    else
                    {
                        description("Update failed!", 1000); 
                        SplashScreenManager.CloseForm();
                    }
                    break;
                case TrainerUpdateEnum.NewVersionNewRevision:
                    description("Update available..", 1000);
                    description("Updating revision.", 1000);
                    description("Updating revision..", 1000);
                    var updateRevision = _srLoader.Update(TrainerUpdateEnum.NewRevision);
                    if (updateRevision)
                    {
                        refreshControl();
                        description("Revision Update Completed!", 1500);
                    }
                    var updateVersion = _srLoader.Update(TrainerUpdateEnum.NewVersion);
                    if (updateVersion)
                    {
                        description("Generating download link.", 1100);
                        description("Generating download link.,", 1100);
                        description("Generating download link.,", 1100);
                        SplashScreenManager.CloseForm();
                        _srLoader.GenerateDownloadLink(new ChangeLogViewer(_srLoader.apis.Data.SRFChangelog));
                        break;
                    }
                    SplashScreenManager.CloseForm();
                    break;
                case TrainerUpdateEnum.NoVersionNoRevision:
                    description("No Update", 1500);
                    SplashScreenManager.CloseForm();
                    break;
                case TrainerUpdateEnum.Error:
                    description("Update error!", 1000);
                    SplashScreenManager.CloseForm();
                    break;
                default:
                    description("No Update", 1500);
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
    }
    public class ChangeLogViewer : XtraUserControl 
    {
        public ChangeLogViewer(IList<SRFChangelog> clData)
        {
            LayoutControl lc = new LayoutControl();
            lc.Dock = DockStyle.Fill;
            // MemoEdit me = new MemoEdit();
            // me.EditValue = changeLogData;
            // me.ReadOnly = true;
            RichEditControl re = new RichEditControl();
            re.ActiveViewType = RichEditViewType.Simple; 
            re.Views.SimpleView.AdjustColorsToSkins = true;
            SRInfo.Instance.SRChangeLog(re, clData);
            // re.Enabled = true;
            // re.ReadOnly = true;
                // re.Appearance.Text.ForeColor = Color.Brown;
                // re.ForeColor = Color.Crimson;
                // re.BackColor = Color.Gray;
                // re.LookAndFeel.ActiveLookAndFeel = UserLookAndFeel.Default;
            SeparatorControl separatorControl = new SeparatorControl();
            lc.AddItem(String.Empty, re).TextVisible = false;
            Controls.Add(lc);
            Height = 200;
            Dock = DockStyle.Top;
        }
    }
    
}