using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Dialogs.Core.Internal;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;

namespace SRUL.Views
{
    public class DonationLinkViewer : XtraUserControl
    {
        public DonationLinkViewer()
        {
            LayoutControl lc = new LayoutControl();
            lc.Dock = DockStyle.Fill;
            lc.AllowDrop = false;
            lc.HorizontalScroll.Enabled = false;

            SimpleButton paypalButton = new SimpleButton();
            paypalButton.Text = "Donate to Paypal";
            paypalButton.ToolTip = "Will be redirected to https://paypal.com";
            paypalButton.Click += (sender, args) => { Process.Start(Properties.Settings.Default.PaypalLink); };

            SimpleButton kofiButton = new SimpleButton();
            kofiButton.Text = "Donate with Coffee";
            kofiButton.ToolTip = "You'll be redirected to https://ko-fi.com/saveroo";
            kofiButton.Click += (sender, args) => { Process.Start("https://ko-fi.com/saveroo"); };

            SimpleButton buymeacoffeeButton = new SimpleButton();
            buymeacoffeeButton.Text = "Donate with Tea";
            buymeacoffeeButton.ToolTip = "You'll be redirected to https://www.buymeacoffee.com/saveroo";
            buymeacoffeeButton.Click += (sender, args) => { Process.Start("https://www.buymeacoffee.com/saveroo"); };

            SimpleButton binanceGiftCard = new SimpleButton();
            binanceGiftCard.Text = "Mail Binance Gift Card";
            binanceGiftCard.ToolTip = "You'll be redirected to https://www.binance.com/en/gift-card";
            binanceGiftCard.Click += (sender, args) =>
            {
                XtraMessageBox.Show("You can mail to sysadmin47@gmail.com after create");
                Process.Start("https://www.binance.com/en/gift-card");
            };
            
            /*
             *  Below are info
             */
            
            LabelControl lblControl = new LabelControl();
            lblControl.Text = "\t  Want to remove ads?\n\t  Need more information?\n";
            lblControl.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
            // lblControl.Appearance.TextOptions = HorzAlignment.Center;
            lblControl.Font = new Font(lblControl.Font, FontStyle.Bold);
            
            TextEdit UUID = new TextEdit();
            UUID.EditValue = $"{Properties.Settings.Default.UserId}";
            UUID.ReadOnly = true;
            UUID.Click += (sender, args) =>
            {
                Clipboard.SetText(Properties.Settings.Default.UserId);
            };
            
            SimpleButton copyUUID = new SimpleButton();
            copyUUID.Text = "Copy UID";
            copyUUID.Click += (sender, args) =>
            {
                
                Clipboard.SetText(Properties.Settings.Default.UserId);
                (sender as SimpleButton).Text = "Copied!";
            };

            SimpleButton lblGoToProfile = new SimpleButton();
            lblGoToProfile.Text = "Join Steam Group Chat";
            lblGoToProfile.Font = new Font(lblControl.Font, FontStyle.Regular);
            lblGoToProfile.ToolTip = "You'll be invited to SRU Steam Group Chat";
            lblGoToProfile.Click += (sender, args) =>
            {
                Process.Start($"https://s.team/chat/JMkp2A9D");
            };
            // steam://friends/joinchat/76561198094580895
            SplitContainerControl splitContainer = new SplitContainerControl();
            splitContainer.Panel1.Controls.Add(lblControl);
            splitContainer.Panel2.Controls.Add(paypalButton);
            
            // lc.Appearance.Control. = HorzAlignment.Center;
            lc.AddItem("test", paypalButton);
            SeparatorControl separatorControl = new SeparatorControl();
            // separatorControl.Text = "Little Perks";
            lc.AddItem(String.Empty, kofiButton);
            lc.AddItem(String.Empty, buymeacoffeeButton);
            lc.AddItem(String.Empty, binanceGiftCard);
            lc.AddItem("Info:", separatorControl);
            lc.AddItem(String.Empty, lblControl);
            lc.AddItem(String.Empty, lblGoToProfile);
            lc.AddItem("UID:", UUID);
            lc.AddItem(String.Empty, copyUUID);
            // lc.Height = 160;
            lc.Height = 160;
            lc.Width = 190;
            Controls.Add(lc);
            // Height = 160;
            Height = 252;
            Width = 190;
        }
    }
}