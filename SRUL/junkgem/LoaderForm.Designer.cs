using System.ComponentModel;

namespace SRUL
{
    partial class Loader
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerLoader = new System.Windows.Forms.Timer(this.components);
            this.metroTile1 = new MetroFramework.Controls.MetroTile();
            this.cbGameName = new MetroFramework.Controls.MetroCheckBox();
            this.metroProgressSpinner1 = new MetroFramework.Controls.MetroProgressSpinner();
            this.cbGameVersion = new MetroFramework.Controls.MetroCheckBox();
            this.cbTrainerAllowed = new MetroFramework.Controls.MetroCheckBox();
            this.LoaderStyleManager = new MetroFramework.Components.MetroStyleManager(this.components);
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.lblGameStatus = new MetroFramework.Controls.MetroLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cboxGameName = new MetroFramework.Controls.MetroComboBox();
            this.cboxGameVersion = new MetroFramework.Controls.MetroComboBox();
            this.cbTrainerStatus = new MetroFramework.Controls.MetroCheckBox();
            this.lblTrainerStatus = new MetroFramework.Controls.MetroLabel();
            this.mtDonation = new MetroFramework.Controls.MetroTile();
            ((System.ComponentModel.ISupportInitialize) (this.LoaderStyleManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // timerLoader
            // 
            this.timerLoader.Enabled = true;
            this.timerLoader.Tick += new System.EventHandler(this.timerLoader_Tick);
            // 
            // metroTile1
            // 
            this.metroTile1.ActiveControl = null;
            this.metroTile1.BackColor = System.Drawing.Color.Red;
            this.metroTile1.Location = new System.Drawing.Point(23, 257);
            this.metroTile1.Name = "metroTile1";
            this.metroTile1.Size = new System.Drawing.Size(152, 59);
            this.metroTile1.Style = MetroFramework.MetroColorStyle.Green;
            this.metroTile1.TabIndex = 0;
            this.metroTile1.Text = "Open Trainer";
            this.metroTile1.TileImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.metroTile1.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.metroTile1.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile1.UseSelectable = true;
            this.metroTile1.UseStyleColors = true;
            this.metroTile1.UseTileImage = true;
            this.metroTile1.Click += new System.EventHandler(this.metroTile1_Click);
            // 
            // cbGameName
            // 
            this.cbGameName.AutoSize = true;
            this.cbGameName.Enabled = false;
            this.cbGameName.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.cbGameName.FontWeight = MetroFramework.MetroCheckBoxWeight.Bold;
            this.cbGameName.Location = new System.Drawing.Point(23, 75);
            this.cbGameName.Name = "cbGameName";
            this.cbGameName.Size = new System.Drawing.Size(112, 19);
            this.cbGameName.TabIndex = 0;
            this.cbGameName.Text = "Game Name:";
            this.cbGameName.UseSelectable = true;
            // 
            // metroProgressSpinner1
            // 
            this.metroProgressSpinner1.Location = new System.Drawing.Point(258, 257);
            this.metroProgressSpinner1.Maximum = 100;
            this.metroProgressSpinner1.Name = "metroProgressSpinner1";
            this.metroProgressSpinner1.Size = new System.Drawing.Size(62, 59);
            this.metroProgressSpinner1.TabIndex = 1;
            this.metroProgressSpinner1.UseSelectable = true;
            this.metroProgressSpinner1.Value = 100;
            // 
            // cbGameVersion
            // 
            this.cbGameVersion.AutoSize = true;
            this.cbGameVersion.Enabled = false;
            this.cbGameVersion.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.cbGameVersion.FontWeight = MetroFramework.MetroCheckBoxWeight.Bold;
            this.cbGameVersion.Location = new System.Drawing.Point(23, 135);
            this.cbGameVersion.Name = "cbGameVersion";
            this.cbGameVersion.Size = new System.Drawing.Size(121, 19);
            this.cbGameVersion.Style = MetroFramework.MetroColorStyle.Green;
            this.cbGameVersion.TabIndex = 2;
            this.cbGameVersion.Text = "Game Version:";
            this.cbGameVersion.UseSelectable = true;
            // 
            // cbTrainerAllowed
            // 
            this.cbTrainerAllowed.AutoSize = true;
            this.cbTrainerAllowed.Enabled = false;
            this.cbTrainerAllowed.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.cbTrainerAllowed.FontWeight = MetroFramework.MetroCheckBoxWeight.Bold;
            this.cbTrainerAllowed.Location = new System.Drawing.Point(23, 195);
            this.cbTrainerAllowed.Name = "cbTrainerAllowed";
            this.cbTrainerAllowed.Size = new System.Drawing.Size(112, 19);
            this.cbTrainerAllowed.Style = MetroFramework.MetroColorStyle.Green;
            this.cbTrainerAllowed.TabIndex = 3;
            this.cbTrainerAllowed.Text = "Game Status:";
            this.cbTrainerAllowed.UseSelectable = true;
            // 
            // LoaderStyleManager
            // 
            this.LoaderStyleManager.Owner = this;
            this.LoaderStyleManager.Style = MetroFramework.MetroColorStyle.Green;
            this.LoaderStyleManager.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(409, 10);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(149, 19);
            this.metroLabel1.TabIndex = 7;
            this.metroLabel1.Text = "Supreme Ruler Ultimate";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(426, 29);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(44, 19);
            this.metroLabel2.TabIndex = 8;
            this.metroLabel2.Text = "9, 2, 4";
            // 
            // lblGameStatus
            // 
            this.lblGameStatus.AutoSize = true;
            this.lblGameStatus.Location = new System.Drawing.Point(132, 195);
            this.lblGameStatus.Name = "lblGameStatus";
            this.lblGameStatus.Size = new System.Drawing.Size(88, 19);
            this.lblGameStatus.TabIndex = 9;
            this.lblGameStatus.Text = "Not Available";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(258, 257);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(62, 59);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // cboxGameName
            // 
            this.cboxGameName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboxGameName.FormattingEnabled = true;
            this.cboxGameName.ItemHeight = 23;
            this.cboxGameName.Items.AddRange(new object[] {"Supreme Ruler Ultimate", "Supreme Ruler 1936", "Supreme Ruler Cold War"});
            this.cboxGameName.Location = new System.Drawing.Point(43, 100);
            this.cboxGameName.Name = "cboxGameName";
            this.cboxGameName.Size = new System.Drawing.Size(186, 29);
            this.cboxGameName.TabIndex = 11;
            this.cboxGameName.UseSelectable = true;
            this.cboxGameName.SelectedIndexChanged += new System.EventHandler(this.metroComboBox1_SelectedIndexChanged);
            // 
            // cboxGameVersion
            // 
            this.cboxGameVersion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboxGameVersion.FormattingEnabled = true;
            this.cboxGameVersion.ItemHeight = 23;
            this.cboxGameVersion.Items.AddRange(new object[] {"9, 2, 5", "9, 2, 4"});
            this.cboxGameVersion.Location = new System.Drawing.Point(43, 160);
            this.cboxGameVersion.Name = "cboxGameVersion";
            this.cboxGameVersion.Size = new System.Drawing.Size(132, 29);
            this.cboxGameVersion.TabIndex = 12;
            this.cboxGameVersion.UseSelectable = true;
            // 
            // cbTrainerStatus
            // 
            this.cbTrainerStatus.AutoSize = true;
            this.cbTrainerStatus.Enabled = false;
            this.cbTrainerStatus.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.cbTrainerStatus.FontWeight = MetroFramework.MetroCheckBoxWeight.Bold;
            this.cbTrainerStatus.Location = new System.Drawing.Point(23, 220);
            this.cbTrainerStatus.Name = "cbTrainerStatus";
            this.cbTrainerStatus.Size = new System.Drawing.Size(120, 19);
            this.cbTrainerStatus.Style = MetroFramework.MetroColorStyle.Green;
            this.cbTrainerStatus.TabIndex = 13;
            this.cbTrainerStatus.Text = "Trainer Status:";
            this.cbTrainerStatus.UseSelectable = true;
            // 
            // lblTrainerStatus
            // 
            this.lblTrainerStatus.AutoSize = true;
            this.lblTrainerStatus.Location = new System.Drawing.Point(142, 220);
            this.lblTrainerStatus.Name = "lblTrainerStatus";
            this.lblTrainerStatus.Size = new System.Drawing.Size(88, 19);
            this.lblTrainerStatus.TabIndex = 14;
            this.lblTrainerStatus.Text = "Not Available";
            // 
            // mtDonation
            // 
            this.mtDonation.ActiveControl = null;
            this.mtDonation.Location = new System.Drawing.Point(181, 257);
            this.mtDonation.Name = "mtDonation";
            this.mtDonation.Size = new System.Drawing.Size(71, 59);
            this.mtDonation.TabIndex = 15;
            this.mtDonation.Text = "Donate";
            this.mtDonation.UseSelectable = true;
            this.mtDonation.Click += new System.EventHandler(this.mtDonation_Click);
            // 
            // Loader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 339);
            this.Controls.Add(this.mtDonation);
            this.Controls.Add(this.lblTrainerStatus);
            this.Controls.Add(this.cbTrainerStatus);
            this.Controls.Add(this.cboxGameVersion);
            this.Controls.Add(this.cboxGameName);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblGameStatus);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.cbTrainerAllowed);
            this.Controls.Add(this.cbGameVersion);
            this.Controls.Add(this.metroProgressSpinner1);
            this.Controls.Add(this.cbGameName);
            this.Controls.Add(this.metroTile1);
            this.MaximizeBox = false;
            this.Name = "Loader";
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.Style = MetroFramework.MetroColorStyle.Green;
            this.Text = "SRU Helper Loader";
            this.Theme = MetroFramework.MetroThemeStyle.Default;
            this.Load += new System.EventHandler(this.Loader_Load);
            ((System.ComponentModel.ISupportInitialize) (this.LoaderStyleManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private MetroFramework.Controls.MetroCheckBox cbGameName;
        private MetroFramework.Controls.MetroCheckBox cbGameVersion;
        private MetroFramework.Controls.MetroComboBox cboxGameName;
        private MetroFramework.Controls.MetroComboBox cboxGameVersion;
        private MetroFramework.Controls.MetroCheckBox cbTrainerAllowed;
        private MetroFramework.Controls.MetroCheckBox cbTrainerStatus;
        private MetroFramework.Controls.MetroLabel lblGameStatus;
        private MetroFramework.Controls.MetroLabel lblTrainerStatus;
        public MetroFramework.Components.MetroStyleManager LoaderStyleManager;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroProgressSpinner metroProgressSpinner1;
        private MetroFramework.Controls.MetroTile metroTile1;
        private MetroFramework.Controls.MetroTile mtDonation;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timerLoader;

        #endregion
    }
}