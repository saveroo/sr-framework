using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace SRUL
{
    public partial class Loader : MetroFramework.Forms.MetroForm
    {
        public static ReadWrite Rw = new ReadWrite();
        private Form1 TrainerForm = null;

        public Loader()
        {
            InitializeComponent();
            this.StyleManager = LoaderStyleManager;
        }

        private void Loader_Load(object sender, EventArgs e)
        {
            metroTile1.Enabled = false;
            timerLoader.Enabled = true;
        }

        private void IsEnabled()
        {
            Rw.Load();

            if (Rw.TrainerAvailability())
            {
                metroLabel3.Text = "Available";
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
                metroLabel3.Text = "Not Available";
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

                this.Show();
                if(TrainerForm != null) if(!TrainerForm.IsDisposed) TrainerForm.Close();
            }
        }

        private void OpenTrainer()
        {
            void ShowTrainer()
            {
                TrainerForm = new Form1();
                TrainerForm.Show();
            }

            if (TrainerForm != null && TrainerForm.IsDisposed)
                ShowTrainer();
            else
                ShowTrainer();
        }

        private void timerLoader_Tick(object sender, EventArgs e)
        {
            IsEnabled();
        }

        private void metroTile1_Click(object sender, EventArgs e)
        {
            // Application.Run(mainForm);
            OpenTrainer();
            this.Hide();
        }
    }
}