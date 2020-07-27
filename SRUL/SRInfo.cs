using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Management.Instrumentation;
using System.Reflection;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

namespace SRUL
{
    public enum TrainerEnum
    {
        Paypal,
        DonorBox
    }
    public sealed class SRInfo
    {
        
        private static readonly Lazy<SRInfo> _instance = new Lazy<SRInfo>(() => new SRInfo());

        public static SRInfo Instance => _instance.Value;
            
        private SRInfo()
        {
            
        }

        public void SRMemo(MemoEdit memo)
        {
        }

        public void SRChangeLog(RichEditControl re, IList<SRFChangelog> clData = null)
        {
            IList<SRFChangelog> cl;
            if (clData == null)
                cl = SRFApis.Instance.Data.SRFChangelog;
            else
                cl = clData; 
            re.ForeColor = Color.Azure;
            re.ShowCaretInReadOnly = false;
            re.ReadOnly = true;
            // re.Enabled = false;
            
            foreach (var log in cl)
            {
                re.HtmlText += $@"<b>{log.Version} {log.Title} ({log.Date})</b>";
                re.HtmlText += "\n";
                foreach (var ch in log.Changes)
                {
                    re.HtmlText += $@"{ch}";
                }
                re.HtmlText += "\n";
                re.HtmlText += $@"==========================";
                re.HtmlText += "";
            }
        }

        public void SRProductInformation(RichEditControl re)
        {
            var info = SRMain.Instance.Data;
            var SRStatus = (info.SRFStatus ? "Active" : "Inactive");
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            re.ReadOnly = true;
            re.ShowCaretInReadOnly = false;
            re.Enabled = false;
            
            re.ForeColor = Color.Azure;
            Document document = re.Document;
            Table table = document.Tables.Create(document.Range.End, 9, 2);

            string AddSocialRow()
            {
                string str = "";
                for (int i = 0; i < info.SRFSocial.Count; i++)
                {
                    str += $"{info.SRFSocial[i].SocialName}: {info.SRFSocial[i].SocialAccount} {System.Environment.NewLine}";
                }

                return str;
            }
            
            table.BeginUpdate();
            //Insert the header data
            document.InsertSingleLineText(table[0, 0].Range.Start, "Product Name");
            document.InsertSingleLineText(table[1, 0].Range.Start, "Product Description");
            document.InsertSingleLineText(table[2, 0].Range.Start, "Product Meta");
            document.InsertSingleLineText(table[3, 0].Range.Start, "Product Status");
            document.InsertSingleLineText(table[4, 0].Range.Start, "Product Update Link");
            document.InsertSingleLineText(table[5, 0].Range.Start, "Author Name");
            document.InsertSingleLineText(table[6, 0].Range.Start, "Author Contact");
            document.InsertSingleLineText(table[7, 0].Range.Start, "Author Website");
            document.InsertSingleLineText(table[8, 0].Range.Start, "Author Social");
            
            document.InsertHtmlText(table[0, 1].Range.Start, $"{info.SRFName}");
            document.InsertHtmlText(table[1, 1].Range.Start, $"{info.SRFDescription}");
            document.InsertHtmlText(table[2, 1].Range.Start, $"{info.SRFVersion} (Rev. {info.SRFRevision})");
            document.InsertHtmlText(table[3, 1].Range.Start, $"{SRStatus}");
            document.InsertHtmlText(table[4, 1].Range.Start, $"{info.SRFDownloadLink}");
            document.InsertHtmlText(table[5, 1].Range.Start, $"{info.SRFAuthor}");
            document.InsertHtmlText(table[6, 1].Range.Start, $"{info.SRFContact}");
            document.InsertHtmlText(table[7, 1].Range.Start, $"{info.SRFWebsite}");
            document.InsertHtmlText(table[8, 1].Range.Start, $"{AddSocialRow()}");
            table.EndUpdate();
            
            
            // re.HtmlText = string.Empty;
            // re.HtmlText += $@"Product Name: <b>{info.SRFName}</b>";
            // re.HtmlText += $@"Product Description: <b>{info.SRFDescription}</b>";
            // re.HtmlText += $@"Product Version: <b>{info.SRFVersion}</b>";
            // re.HtmlText += $@"Product Status: <b>{(info.SRFStatus ? "Active" : "Inactive")}</b>";
            // re.HtmlText += $@"Product Download Link: <b>{info.SRFDownloadLink}</b>";
            // re.HtmlText += $@"Author Name: <b>{info.SRFName}";
            // re.HtmlText += $@"Author Contact: <b>{info.SRFContact}</b>";
            // re.HtmlText += $@"Author Website: <a href='{info.SRFWebsite}'>{info.SRFWebsite}</a>";
            // re.HtmlText += $@"Author Twitter: <b>{info.SRFSocial[0].SocialAccount}</b>";
        }
        public void SRDonationButton (SimpleButton sb, TrainerEnum te)
        {
            string link = "";
            if (te == TrainerEnum.Paypal)
                link = Properties.Settings.Default.PaypalLink;
            else
                link = Properties.Settings.Default.DonorboxLink;
            sb.Click += (object sender, EventArgs e) =>
            {
                System.Diagnostics.Process.Start(link);
            };
        }
        public void SRBarDonationHeader (BarHeaderItem sb)
        {
            sb.ItemClick += (object sender, ItemClickEventArgs e) =>
            {
                System.Diagnostics.Process.Start(Properties.Settings.Default.DonorboxLink);
            };
        }
        
        public void SRBarDonationButton (BarButtonItem sb)
        {
            sb.ItemClick += (object sender, ItemClickEventArgs e) =>
            {
                System.Diagnostics.Process.Start(Properties.Settings.Default.DonorboxLink);
            };
        }
    }
}