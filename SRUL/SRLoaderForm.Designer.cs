namespace SRUL
{
    partial class SRLoaderForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            DevExpress.XtraSplashScreen.SplashScreenManager SRsplashScreenManager = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::SRUL.SRLoaderFormSplash), false, false);
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SRLoaderForm));
            DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipItem toolTipItem2 = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SuperToolTip superToolTip3 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipItem toolTipItem3 = new DevExpress.Utils.ToolTipItem();
            this.ceGameName = new DevExpress.XtraEditors.CheckEdit();
            this.ceGameVersion = new DevExpress.XtraEditors.CheckEdit();
            this.ceGameStatus = new DevExpress.XtraEditors.CheckEdit();
            this.ceTrainerStatus = new DevExpress.XtraEditors.CheckEdit();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.leGameSelection = new DevExpress.XtraEditors.LookUpEdit();
            this.leGameVersionSelection = new DevExpress.XtraEditors.LookUpEdit();
            this.timerLoader = new System.Windows.Forms.Timer(this.components);
            this.memoEdit1 = new DevExpress.XtraEditors.MemoEdit();
            this.btnCheckUpdate = new DevExpress.XtraEditors.SimpleButton();
            this.lblVersion = new DevExpress.XtraEditors.LabelControl();
            this.lblRevision = new DevExpress.XtraEditors.LabelControl();
            this.sidePanel1 = new DevExpress.XtraEditors.SidePanel();
            this.lblPlayers = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.lblRevisionValue = new DevExpress.XtraEditors.LabelControl();
            this.lblVersionValue = new DevExpress.XtraEditors.LabelControl();
            this.iconCheckUpdate = new DevExpress.XtraEditors.SvgImageBox();
            this.toolbarFormManager1 = new DevExpress.XtraBars.ToolbarForm.ToolbarFormManager(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.toolbarFormManager2 = new DevExpress.XtraBars.ToolbarForm.ToolbarFormManager(this.components);
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl3 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize) (this.ceGameName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.ceGameVersion.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.ceGameStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.ceTrainerStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.leGameSelection.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.leGameVersionSelection.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.memoEdit1.Properties)).BeginInit();
            this.sidePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.iconCheckUpdate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.toolbarFormManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.toolbarFormManager2)).BeginInit();
            this.SuspendLayout();
            // 
            // SRsplashScreenManager
            // 
            SRsplashScreenManager.ClosingDelay = 100;
            // 
            // ceGameName
            // 
            this.ceGameName.Location = new System.Drawing.Point(12, 82);
            this.ceGameName.Name = "ceGameName";
            this.ceGameName.Properties.AllowFocused = false;
            this.ceGameName.Properties.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.ceGameName.Properties.Appearance.BackColor2 = System.Drawing.Color.Crimson;
            this.ceGameName.Properties.Appearance.BorderColor = System.Drawing.Color.Lime;
            this.ceGameName.Properties.Appearance.Font = new System.Drawing.Font("Bahnschrift", 10F);
            this.ceGameName.Properties.Appearance.ForeColor = System.Drawing.Color.White;
            this.ceGameName.Properties.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal;
            this.ceGameName.Properties.Appearance.Options.UseBackColor = true;
            this.ceGameName.Properties.Appearance.Options.UseBorderColor = true;
            this.ceGameName.Properties.Appearance.Options.UseFont = true;
            this.ceGameName.Properties.Appearance.Options.UseForeColor = true;
            this.ceGameName.Properties.Caption = "Game Selection";
            this.ceGameName.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Radio;
            this.ceGameName.Properties.CheckBoxOptions.SvgColorChecked = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical;
            this.ceGameName.Properties.CheckBoxOptions.SvgColorGrayed = System.Drawing.Color.Red;
            this.ceGameName.Properties.CheckBoxOptions.SvgColorUnchecked = DevExpress.LookAndFeel.DXSkinColors.ForeColors.DisabledText;
            this.ceGameName.Properties.FullFocusRect = true;
            this.ceGameName.Properties.ReadOnly = true;
            this.ceGameName.Size = new System.Drawing.Size(172, 20);
            this.ceGameName.TabIndex = 0;
            // 
            // ceGameVersion
            // 
            this.ceGameVersion.Location = new System.Drawing.Point(12, 133);
            this.ceGameVersion.Name = "ceGameVersion";
            this.ceGameVersion.Properties.AllowFocused = false;
            this.ceGameVersion.Properties.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.ceGameVersion.Properties.Appearance.Font = new System.Drawing.Font("Bahnschrift", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.ceGameVersion.Properties.Appearance.ForeColor = System.Drawing.Color.White;
            this.ceGameVersion.Properties.Appearance.Options.UseBackColor = true;
            this.ceGameVersion.Properties.Appearance.Options.UseFont = true;
            this.ceGameVersion.Properties.Appearance.Options.UseForeColor = true;
            this.ceGameVersion.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            this.ceGameVersion.Properties.Caption = "Game Version";
            this.ceGameVersion.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Radio;
            this.ceGameVersion.Properties.FullFocusRect = true;
            this.ceGameVersion.Properties.ReadOnly = true;
            this.ceGameVersion.Size = new System.Drawing.Size(172, 20);
            this.ceGameVersion.TabIndex = 1;
            // 
            // ceGameStatus
            // 
            this.ceGameStatus.Location = new System.Drawing.Point(12, 184);
            this.ceGameStatus.Name = "ceGameStatus";
            this.ceGameStatus.Properties.AllowFocused = false;
            this.ceGameStatus.Properties.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.ceGameStatus.Properties.Appearance.Font = new System.Drawing.Font("Bahnschrift", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.ceGameStatus.Properties.Appearance.ForeColor = System.Drawing.Color.White;
            this.ceGameStatus.Properties.Appearance.Options.UseBackColor = true;
            this.ceGameStatus.Properties.Appearance.Options.UseFont = true;
            this.ceGameStatus.Properties.Appearance.Options.UseForeColor = true;
            this.ceGameStatus.Properties.Caption = "Game Process";
            this.ceGameStatus.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Radio;
            this.ceGameStatus.Properties.FullFocusRect = true;
            this.ceGameStatus.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.ceGameStatus.Properties.ReadOnly = true;
            this.ceGameStatus.Size = new System.Drawing.Size(172, 20);
            this.ceGameStatus.TabIndex = 2;
            // 
            // ceTrainerStatus
            // 
            this.ceTrainerStatus.Location = new System.Drawing.Point(12, 209);
            this.ceTrainerStatus.Name = "ceTrainerStatus";
            this.ceTrainerStatus.Properties.AllowFocused = false;
            this.ceTrainerStatus.Properties.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.ceTrainerStatus.Properties.Appearance.Font = new System.Drawing.Font("Bahnschrift", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.ceTrainerStatus.Properties.Appearance.ForeColor = System.Drawing.Color.White;
            this.ceTrainerStatus.Properties.Appearance.Options.UseBackColor = true;
            this.ceTrainerStatus.Properties.Appearance.Options.UseFont = true;
            this.ceTrainerStatus.Properties.Appearance.Options.UseForeColor = true;
            this.ceTrainerStatus.Properties.Caption = "Trainer Status";
            this.ceTrainerStatus.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Radio;
            this.ceTrainerStatus.Properties.FullFocusRect = true;
            this.ceTrainerStatus.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.ceTrainerStatus.Properties.ReadOnly = true;
            this.ceTrainerStatus.Size = new System.Drawing.Size(172, 20);
            this.ceTrainerStatus.TabIndex = 3;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Appearance.Font = new System.Drawing.Font("Bahnschrift SemiLight SemiConde", 15F, System.Drawing.FontStyle.Bold);
            this.simpleButton1.Appearance.Options.UseFont = true;
            this.simpleButton1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.simpleButton1.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.simpleButton1.Location = new System.Drawing.Point(0, 279);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(195, 50);
            toolTipItem1.Text = "Open Selected Trainer";
            superToolTip1.Items.Add(toolTipItem1);
            this.simpleButton1.SuperTip = superToolTip1;
            this.simpleButton1.TabIndex = 5;
            this.simpleButton1.Text = "GAME ON";
            // 
            // simpleButton2
            // 
            this.simpleButton2.Appearance.Font = new System.Drawing.Font("Arial Black", 8.25F);
            this.simpleButton2.Appearance.Options.UseFont = true;
            this.simpleButton2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.simpleButton2.ImageOptions.ImageToTextIndent = -5;
            this.simpleButton2.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage) (resources.GetObject("simpleButton2.ImageOptions.SvgImage")));
            this.simpleButton2.ImageOptions.SvgImageSize = new System.Drawing.Size(22, 22);
            this.simpleButton2.Location = new System.Drawing.Point(0, 248);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(195, 31);
            toolTipItem2.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            toolTipItem2.Text = resources.GetString("toolTipItem2.Text");
            superToolTip2.Items.Add(toolTipItem2);
            this.simpleButton2.SuperTip = superToolTip2;
            this.simpleButton2.TabIndex = 6;
            this.simpleButton2.Text = "Cup of tea for developer";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // leGameSelection
            // 
            this.leGameSelection.Location = new System.Drawing.Point(12, 103);
            this.leGameSelection.Name = "leGameSelection";
            this.leGameSelection.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.leGameSelection.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (0)))), ((int) (((byte) (48)))), ((int) (((byte) (58)))));
            this.leGameSelection.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.leGameSelection.Properties.Appearance.Options.UseBackColor = true;
            this.leGameSelection.Properties.Appearance.Options.UseFont = true;
            this.leGameSelection.Properties.Appearance.Options.UseTextOptions = true;
            this.leGameSelection.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.leGameSelection.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.leGameSelection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.leGameSelection.Properties.NullText = "- Select -";
            this.leGameSelection.Size = new System.Drawing.Size(172, 20);
            this.leGameSelection.TabIndex = 11;
            // 
            // leGameVersionSelection
            // 
            this.leGameVersionSelection.Location = new System.Drawing.Point(12, 154);
            this.leGameVersionSelection.Name = "leGameVersionSelection";
            this.leGameVersionSelection.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (0)))), ((int) (((byte) (48)))), ((int) (((byte) (58)))));
            this.leGameVersionSelection.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.leGameVersionSelection.Properties.Appearance.Options.UseBackColor = true;
            this.leGameVersionSelection.Properties.Appearance.Options.UseFont = true;
            this.leGameVersionSelection.Properties.Appearance.Options.UseTextOptions = true;
            this.leGameVersionSelection.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.leGameVersionSelection.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.leGameVersionSelection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.leGameVersionSelection.Properties.NullText = "- Select -";
            this.leGameVersionSelection.Size = new System.Drawing.Size(172, 20);
            this.leGameVersionSelection.TabIndex = 12;
            // 
            // timerLoader
            // 
            this.timerLoader.Tick += new System.EventHandler(this.timerLoader_Tick);
            // 
            // memoEdit1
            // 
            this.memoEdit1.EditValue = "1\r\n2\r\n3\r\n4\r\n5\r\n6\r\n7\r\n8\r\n9";
            this.memoEdit1.Location = new System.Drawing.Point(360, 83);
            this.memoEdit1.Name = "memoEdit1";
            this.memoEdit1.Size = new System.Drawing.Size(159, 96);
            this.memoEdit1.TabIndex = 13;
            // 
            // btnCheckUpdate
            // 
            this.btnCheckUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCheckUpdate.ImageOptions.ImageToTextIndent = 0;
            this.btnCheckUpdate.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage) (resources.GetObject("btnCheckUpdate.ImageOptions.SvgImage")));
            this.btnCheckUpdate.ImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
            this.btnCheckUpdate.Location = new System.Drawing.Point(344, 25);
            this.btnCheckUpdate.Name = "btnCheckUpdate";
            this.btnCheckUpdate.Size = new System.Drawing.Size(74, 44);
            toolTipItem3.Text = "Check for update";
            superToolTip3.Items.Add(toolTipItem3);
            this.btnCheckUpdate.SuperTip = superToolTip3;
            this.btnCheckUpdate.TabIndex = 14;
            this.btnCheckUpdate.Text = "Check";
            this.btnCheckUpdate.Click += new System.EventHandler(this.btnCheckUpdate_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.Location = new System.Drawing.Point(17, 6);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(39, 13);
            this.lblVersion.TabIndex = 10;
            this.lblVersion.Text = "Version:";
            // 
            // lblRevision
            // 
            this.lblRevision.Location = new System.Drawing.Point(12, 25);
            this.lblRevision.Name = "lblRevision";
            this.lblRevision.Size = new System.Drawing.Size(44, 13);
            this.lblRevision.TabIndex = 9;
            this.lblRevision.Text = "Revision:";
            // 
            // sidePanel1
            // 
            this.sidePanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.sidePanel1.Controls.Add(this.lblPlayers);
            this.sidePanel1.Controls.Add(this.labelControl1);
            this.sidePanel1.Controls.Add(this.lblRevisionValue);
            this.sidePanel1.Controls.Add(this.lblVersionValue);
            this.sidePanel1.Controls.Add(this.iconCheckUpdate);
            this.sidePanel1.Controls.Add(this.lblVersion);
            this.sidePanel1.Controls.Add(this.lblRevision);
            this.sidePanel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.sidePanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.sidePanel1.Location = new System.Drawing.Point(0, 0);
            this.sidePanel1.Name = "sidePanel1";
            this.sidePanel1.Size = new System.Drawing.Size(195, 67);
            this.sidePanel1.TabIndex = 15;
            this.sidePanel1.Text = "sidePanel1";
            // 
            // lblPlayers
            // 
            this.lblPlayers.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPlayers.Appearance.Options.UseFont = true;
            this.lblPlayers.Location = new System.Drawing.Point(62, 44);
            this.lblPlayers.Name = "lblPlayers";
            this.lblPlayers.Size = new System.Drawing.Size(14, 13);
            this.lblPlayers.TabIndex = 19;
            this.lblPlayers.Text = "55";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(17, 44);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(39, 13);
            this.labelControl1.TabIndex = 18;
            this.labelControl1.Text = "Players:";
            // 
            // lblRevisionValue
            // 
            this.lblRevisionValue.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblRevisionValue.Appearance.Options.UseFont = true;
            this.lblRevisionValue.Location = new System.Drawing.Point(62, 25);
            this.lblRevisionValue.Name = "lblRevisionValue";
            this.lblRevisionValue.Size = new System.Drawing.Size(40, 13);
            this.lblRevisionValue.TabIndex = 17;
            this.lblRevisionValue.Text = "r12394";
            // 
            // lblVersionValue
            // 
            this.lblVersionValue.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblVersionValue.Appearance.Options.UseFont = true;
            this.lblVersionValue.Location = new System.Drawing.Point(62, 6);
            this.lblVersionValue.Name = "lblVersionValue";
            this.lblVersionValue.Size = new System.Drawing.Size(37, 13);
            this.lblVersionValue.TabIndex = 16;
            this.lblVersionValue.Text = "1.0.0.0";
            // 
            // iconCheckUpdate
            // 
            this.iconCheckUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.iconCheckUpdate.Dock = System.Windows.Forms.DockStyle.Right;
            this.iconCheckUpdate.ItemAppearance.Hovered.BorderColor = System.Drawing.SystemColors.HotTrack;
            this.iconCheckUpdate.ItemAppearance.Hovered.BorderThickness = 1F;
            this.iconCheckUpdate.ItemAppearance.Hovered.FillColor = System.Drawing.SystemColors.HotTrack;
            this.iconCheckUpdate.Location = new System.Drawing.Point(155, 0);
            this.iconCheckUpdate.Name = "iconCheckUpdate";
            this.iconCheckUpdate.Size = new System.Drawing.Size(40, 66);
            this.iconCheckUpdate.SvgImage = ((DevExpress.Utils.Svg.SvgImage) (resources.GetObject("iconCheckUpdate.SvgImage")));
            this.iconCheckUpdate.TabIndex = 15;
            this.iconCheckUpdate.Text = "svgImageBox1";
            this.iconCheckUpdate.Click += new System.EventHandler(this.iconCheckUpdate_Click);
            // 
            // toolbarFormManager1
            // 
            this.toolbarFormManager1.DockControls.Add(this.barDockControlTop);
            this.toolbarFormManager1.DockControls.Add(this.barDockControlBottom);
            this.toolbarFormManager1.DockControls.Add(this.barDockControlLeft);
            this.toolbarFormManager1.DockControls.Add(this.barDockControlRight);
            this.toolbarFormManager1.Form = this;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.toolbarFormManager1;
            this.barDockControlTop.Size = new System.Drawing.Size(195, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 329);
            this.barDockControlBottom.Manager = this.toolbarFormManager1;
            this.barDockControlBottom.Size = new System.Drawing.Size(195, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Manager = this.toolbarFormManager1;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 329);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(195, 0);
            this.barDockControlRight.Manager = this.toolbarFormManager1;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 329);
            // 
            // toolbarFormManager2
            // 
            this.toolbarFormManager2.DockControls.Add(this.barDockControl1);
            this.toolbarFormManager2.DockControls.Add(this.barDockControl2);
            this.toolbarFormManager2.DockControls.Add(this.barDockControl3);
            this.toolbarFormManager2.DockControls.Add(this.barDockControl4);
            this.toolbarFormManager2.Form = this;
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControl1.Location = new System.Drawing.Point(0, 0);
            this.barDockControl1.Manager = this.toolbarFormManager2;
            this.barDockControl1.Size = new System.Drawing.Size(195, 0);
            // 
            // barDockControl2
            // 
            this.barDockControl2.CausesValidation = false;
            this.barDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControl2.Location = new System.Drawing.Point(0, 329);
            this.barDockControl2.Manager = this.toolbarFormManager2;
            this.barDockControl2.Size = new System.Drawing.Size(195, 0);
            // 
            // barDockControl3
            // 
            this.barDockControl3.CausesValidation = false;
            this.barDockControl3.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControl3.Location = new System.Drawing.Point(0, 0);
            this.barDockControl3.Manager = this.toolbarFormManager2;
            this.barDockControl3.Size = new System.Drawing.Size(0, 329);
            // 
            // barDockControl4
            // 
            this.barDockControl4.CausesValidation = false;
            this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl4.Location = new System.Drawing.Point(195, 0);
            this.barDockControl4.Manager = this.toolbarFormManager2;
            this.barDockControl4.Size = new System.Drawing.Size(0, 329);
            // 
            // SRLoaderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(195, 329);
            this.Controls.Add(this.sidePanel1);
            this.Controls.Add(this.memoEdit1);
            this.Controls.Add(this.btnCheckUpdate);
            this.Controls.Add(this.simpleButton2);
            this.Controls.Add(this.leGameVersionSelection);
            this.Controls.Add(this.leGameSelection);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.ceTrainerStatus);
            this.Controls.Add(this.ceGameStatus);
            this.Controls.Add(this.ceGameVersion);
            this.Controls.Add(this.ceGameName);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Controls.Add(this.barDockControl3);
            this.Controls.Add(this.barDockControl4);
            this.Controls.Add(this.barDockControl2);
            this.Controls.Add(this.barDockControl1);
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Glow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IconOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.None;
            this.MaximizeBox = false;
            this.Name = "SRLoaderForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SR Helper Loader";
            this.Load += new System.EventHandler(this.SRLoaderForm_Load);
            ((System.ComponentModel.ISupportInitialize) (this.ceGameName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.ceGameVersion.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.ceGameStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.ceTrainerStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.leGameSelection.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.leGameVersionSelection.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.memoEdit1.Properties)).EndInit();
            this.sidePanel1.ResumeLayout(false);
            this.sidePanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.iconCheckUpdate)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.toolbarFormManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.toolbarFormManager2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarDockControl barDockControl2;
        private DevExpress.XtraBars.BarDockControl barDockControl3;
        private DevExpress.XtraBars.BarDockControl barDockControl4;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraEditors.SimpleButton btnCheckUpdate;
        private DevExpress.XtraEditors.CheckEdit ceGameName;
        private DevExpress.XtraEditors.CheckEdit ceGameStatus;
        private DevExpress.XtraEditors.CheckEdit ceGameVersion;
        private DevExpress.XtraEditors.CheckEdit ceTrainerStatus;
        private DevExpress.XtraEditors.SvgImageBox iconCheckUpdate;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl lblPlayers;
        private DevExpress.XtraEditors.LabelControl lblRevision;
        private DevExpress.XtraEditors.LabelControl lblRevisionValue;
        private DevExpress.XtraEditors.LabelControl lblVersion;
        private DevExpress.XtraEditors.LabelControl lblVersionValue;
        private DevExpress.XtraEditors.LookUpEdit leGameSelection;
        private DevExpress.XtraEditors.LookUpEdit leGameVersionSelection;
        private DevExpress.XtraEditors.MemoEdit memoEdit1;
        private DevExpress.XtraEditors.SidePanel sidePanel1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private System.Windows.Forms.Timer timerLoader;
        private DevExpress.XtraBars.ToolbarForm.ToolbarFormManager toolbarFormManager1;
        private DevExpress.XtraBars.ToolbarForm.ToolbarFormManager toolbarFormManager2;

        #endregion
    }
}