using System;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;
using SRUL.Properties;

namespace SRUL
{
    public partial class SRLoaderFormSplash : SplashScreen
    {
        // public SRLoader _srLoader = new SRLoader();
        public SRLoaderFormSplash()
        {
            InitializeComponent();
            this.labelCopyright.Text = "Copyright © 2020-" + DateTime.Now.Year;
        }

        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
            SplashScreenCommand command = (SplashScreenCommand) cmd;
            switch (command)
            {
                case SplashScreenCommand.ChangeText:
                    labelStatus.Text = arg.ToString();
                    break;
            }
        }

        #endregion

        public enum SplashScreenCommand
        {
            ChangeText
        }

        private void OpenLoader()
        {
            // SRLoaderForm _loader = new SRLoaderForm();
            // _loader.Show();
            // Close();
        }

        private void SRLoaderFormSplash_Load(object sender, EventArgs e)
        {
            this.Close();
            // if(this.Properties)
            // bannerAds1.ShowAd(468, 60, Settings.Default.ApplicationAdsKey);
            // bannerAds2.ShowAd(468, 60, Settings.Default.ApplicationAdsKey);
        }
    }
}