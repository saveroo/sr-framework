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
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.LoaderStyleManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // timerLoader
            // 
            this.timerLoader.Tick += new System.EventHandler(this.timerLoader_Tick);
            // 
            // metroTile1
            // 
            this.metroTile1.ActiveControl = null;
            this.metroTile1.BackColor = System.Drawing.Color.Red;
            this.metroTile1.Location = new System.Drawing.Point(23, 142);
            this.metroTile1.Name = "metroTile1";
            this.metroTile1.Size = new System.Drawing.Size(207, 59);
            this.metroTile1.Style = MetroFramework.MetroColorStyle.Green;
            this.metroTile1.TabIndex = 0;
            this.metroTile1.Text = "Open Trainer";
            this.metroTile1.TileImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.metroTile1.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.metroTile1.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
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
            this.cbGameName.Style = MetroFramework.MetroColorStyle.Green;
            this.cbGameName.TabIndex = 0;
            this.cbGameName.Text = "Game Name:";
            this.cbGameName.UseSelectable = true;
            // 
            // metroProgressSpinner1
            // 
            this.metroProgressSpinner1.Location = new System.Drawing.Point(239, 142);
            this.metroProgressSpinner1.Maximum = 100;
            this.metroProgressSpinner1.Name = "metroProgressSpinner1";
            this.metroProgressSpinner1.Size = new System.Drawing.Size(59, 59);
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
            this.cbGameVersion.Location = new System.Drawing.Point(23, 96);
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
            this.cbTrainerAllowed.Location = new System.Drawing.Point(23, 117);
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
            this.metroLabel1.Location = new System.Drawing.Point(149, 75);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(149, 19);
            this.metroLabel1.TabIndex = 7;
            this.metroLabel1.Text = "Supreme Ruler Ultimate";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(149, 96);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(44, 19);
            this.metroLabel2.TabIndex = 8;
            this.metroLabel2.Text = "9, 2, 4";
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(149, 117);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(88, 19);
            this.metroLabel3.TabIndex = 9;
            this.metroLabel3.Text = "Not Available";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::SRUL.Properties.Resources._256x256;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(236, 142);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(62, 59);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // Loader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 214);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.metroLabel3);
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
            ((System.ComponentModel.ISupportInitialize)(this.LoaderStyleManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.Timer timerLoader;

        #endregion

        private MetroFramework.Controls.MetroTile metroTile1;
        private MetroFramework.Controls.MetroCheckBox cbGameName;
        private MetroFramework.Controls.MetroProgressSpinner metroProgressSpinner1;
        private MetroFramework.Controls.MetroCheckBox cbGameVersion;
        private MetroFramework.Controls.MetroCheckBox cbTrainerAllowed;
        private MetroFramework.Components.MetroStyleManager LoaderStyleManager;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}