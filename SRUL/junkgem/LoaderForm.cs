using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars.FluentDesignSystem;
using DevExpress.XtraEditors;
using MaterialSkin.Controls;
using MetroFramework;
using MetroFramework.Components;

namespace SRUL
{
    public partial class Loader : MetroFramework.Forms.MetroForm
    {
        private string SrFrameworkName = "SRU Helper";
        private string SrFrameworkCurrentVersion = "1.0.0.0";
        private string SrFrameworkAvailableVersion = "1.0.0.0";
        public static ReadWrite Rw = new ReadWrite();
        private XtraForm _trainerForm = null;
        // private GameList _gameList = GameList.Instance;
        public JSONReader JsonReader;
        public Loader()
        {
            InitializeComponent();
            this.StyleManager = LoaderStyleManager;
            
            // Set Version & name
            SREncryptor.Instance.Load();
            // SrFrameworkAvailableVersion = "";
        }

        private void Loader_Load(object sender, EventArgs e)
        {
            JsonReader = JSONReader.Instance;
            SrFrameworkName = JsonReader.Data.SRFName;
            // SrFrameworkName = "";
            SrFrameworkCurrentVersion = ProductVersion;
            SrFrameworkAvailableVersion = JsonReader.Data.SRFVersion;
            metroTile1.Enabled = false;
            timerLoader.Enabled = true;
            if (SrFrameworkCurrentVersion != SrFrameworkAvailableVersion)
            {
                MessageBox.Show($"Current Version {SrFrameworkCurrentVersion} \n New Version: {SrFrameworkAvailableVersion}", "New update Available");
            }
        }

        private void TrainerLoader()
        {
            
            cboxGameName.DataSource = JsonReader.Data.Games;
            cboxGameName.DisplayMember = "DisplayName";
            cboxGameName.ValueMember = "Versions";

            cboxGameVersion.DataSource = cboxGameName.SelectedValue;
            cboxGameVersion.DisplayMember = "GameVersion";
            cboxGameVersion.ValueMember = "Availability";
            
        }

        private void IsEnabled()
        {
            if (!SREncryptor.Instance.Done)
            {
                SREncryptor.Instance.Load();
                return;
            }
            Rw.Load();
            TrainerLoader();
            if (Rw.loaded && Rw.IsCompatible() && cbTrainerStatus.Checked)
            {
                LoaderStyleManager.Style = MetroColorStyle.Green;
                lblGameStatus.Text = "Found";
                metroProgressSpinner1.Spinning = false; 
                metroProgressSpinner1.Visible = false;
                metroTile1.UseCustomBackColor = false;
                metroTile1.Enabled = true;
                pictureBox1.Show();

                cbGameName.Checked = true;
                cbGameVersion.Checked = true;
                cbTrainerAllowed.Checked = true;
            }
            else
            {
                LoaderStyleManager.Style = MetroColorStyle.Red;
                lblGameStatus.Text = "Not Found";
                metroProgressSpinner1.Spinning = true;
                metroProgressSpinner1.Visible = true;
                metroTile1.UseCustomBackColor = true;
                pictureBox1.Hide();
                metroTile1.BackColor = Color.Gray;
                metroProgressSpinner1.Value = 50;
                metroTile1.Enabled = false;

                cbGameName.Checked = false;
                cbGameVersion.Checked = false;
                cbTrainerAllowed.Checked = false;

                Show();
                if(_trainerForm != null) if(!_trainerForm.IsDisposed) _trainerForm.Close();
            } 
            if ((bool) cboxGameVersion.SelectedValue)
            {
                cbTrainerStatus.Checked = true;
                lblTrainerStatus.Text = "Available";
            }
            else
            {
                cbTrainerStatus.Checked = false;
                lblTrainerStatus.Text = "Unavailable";
            }
            // Save as Active Game
            // using (JSONReader.Instance)
            // {
            //     // JsonReader.activeTrainer.GameName = cboxGameName.SelectedItem.ToString();
            //     // JsonReader.activeTrainer.GameVersion = cboxGameVersion.SelectedText.ToString();
            // }
        }

        private void OpenTrainer()
        {
                void ShowTrainer() 
                {
                    _trainerForm = new XtraForm1();
                    _trainerForm.Text = $"{SrFrameworkName}";
                    _trainerForm.Tag = $"{SrFrameworkCurrentVersion}";
                    _trainerForm.Show();
                }

                if (JsonReader.SelectedTrainer(cboxGameName.Text, cboxGameVersion.Text))
                {
                    if (_trainerForm != null && _trainerForm.IsDisposed)
                        ShowTrainer();
                    else
                        ShowTrainer();    
                }
                else
                {
                    MessageBox.Show("Contact Developer: sysadmin47@gmail.com", "Something went wrong");
                }
                
        }

        private void timerLoader_Tick(object sender, EventArgs e)
        {
            IsEnabled();
            if (_trainerForm != null)
            {
                if (!_trainerForm.Visible)
                {
                    Close();
                }
            }
        }

        private void metroTile1_Click(object sender, EventArgs e)
        {
            // Application.Run(mainForm);
            OpenTrainer();
            Hide();
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void mtDonation_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Settings.Default.DonorboxLink);
        }
    }
}