using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;

namespace SRUL
{
    public partial class SRLoaderFormSplash : SplashScreen
    {
        // public SRLoader _srLoader = new SRLoader();
        public SRLoaderFormSplash()
        {
            InitializeComponent();
            this.labelCopyright.Text = "Copyright © 1998-" + DateTime.Now.Year.ToString();
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