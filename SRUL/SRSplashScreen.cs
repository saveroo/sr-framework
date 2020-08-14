using System;
using DevExpress.XtraSplashScreen;

namespace SRUL
{
    public partial class SRLoaderFormSplash : SplashScreen
    {
        // public SRLoader _srLoader = new SRLoader();
        public SRLoaderFormSplash()
        {
            InitializeComponent();
            this.labelCopyright.Text = "Copyright © 2020-" + DateTime.Now.Year.ToString();
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
            SRLoaderForm _loader = new SRLoaderForm();
            _loader.Show();
            Close();
        }

        private void SRLoaderFormSplash_Load(object sender, EventArgs e)
        {
            // if(this.Properties)
        }
    }
}