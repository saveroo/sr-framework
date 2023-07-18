using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using SRUL.Views;

namespace SRUL
{
    public enum TrainerEnum
    {
        Paypal,
        DonorBox
    }
    public sealed class SRInfo : IDisposable
    {
        private static readonly Lazy<SRInfo> _instance = new Lazy<SRInfo>(() => new SRInfo());

        public static SRInfo Instance => _instance.Value;
            
        private SRInfo()
        {
            
        }

        public void SRMemo(MemoEdit memo)
        {
        }

        public string? _clHtmlTexts;
        public string? SRChangeLog(RichEditControl? re, IList<SRFChangelog> clData)
        {
            re.ForeColor = Color.Azure;
            re.ShowCaretInReadOnly = false;
            re.VerticalScrollValue = 0;
            re.ReadOnly = true;
            // re.Appearance.Text.ForeColor = Color.Red;
            // re.Enabled = false;
            _clHtmlTexts = "<html><head></head><body>";
            foreach (var log in clData)
            {
                _clHtmlTexts += $@"<span style='color:#80CBE1'>{log.Version} {log.Title} ({log.Date})</span>";
                _clHtmlTexts += "\n\n"; 
                string pattern = @"-(.*[dep|deps|perf|feat|fix])(\(.*\)):";
                Regex c = new Regex(pattern);
                foreach (var ch in log.Changes)
                { 
                    _clHtmlTexts += "<br/>"; 
                    if(c.IsMatch(ch))                          
                        _clHtmlTexts += $@"{c.Replace(ch, "-<span style='color:#FFECBE;'>$1</span><span style='color:#98E17F'>$2</span>:")}";
                    else
                        _clHtmlTexts += $"{ch}";
                }
                _clHtmlTexts += "\n<br/>";
                _clHtmlTexts += $@"==========================";
                _clHtmlTexts += "\n<br/>";
            }
            _clHtmlTexts += "</body></html>";

            re.HtmlText = _clHtmlTexts;

            return _clHtmlTexts;
            // re.Document.DefaultCharacterProperties.FontSize = 5;
            // re.DocumentLoaded += (sender, args) =>
            // {
            //     re.Document.DefaultCharacterProperties.FontSize = (float?) 1;
            // };
        }

        public void SRSteamID()
        {
            
        }

        public void SRLoadCheatTable(RichEditControl re)
        {
            if(File.Exists($"{Environment.CurrentDirectory}./cheats.html"))
            {
                re.LoadDocument($"{Environment.CurrentDirectory}./cheats.html");
            }
            // var info = SRMain.Instance.Data;
            // var SRStatus = (info.SRFStatus ? "Active" : "Inactive");
            //
            // re.ReadOnly = true;
            // re.ShowCaretInReadOnly = false;
            // re.ForeColor = Color.Azure;
            // Document document = re.Document;
            // Table table = document.Tables.Create(document.Range.End, 21, 2);
            //
            // table.BeginUpdate();
            // //Insert the header data
            // document.InsertText(table[0, 0].Range.Start, "cheat allowcheats");
            // document.InsertText(table[1, 0].Range.Start, "cheat shelovesmenot");
            // document.InsertText(table[2, 0].Range.Start, "cheat fullmapshow");
            // document.InsertText(table[3, 0].Range.Start, "cheat maxsat");
            // document.InsertText(table[4, 0].Range.Start, "cheat endday");
            // document.InsertText(table[5, 0].Range.Start, "cheat georgew");
            // document.InsertText(table[6, 0].Range.Start, "cheat satellite");
            // document.InsertText(table[7, 0].Range.Start, "cheat instantwin");
            // document.InsertText(table[8, 0].Range.Start, "cheat allunit");
            // document.InsertText(table[9, 0].Range.Start, "cheat onedaybuild");
            // document.InsertText(table[10, 0].Range.Start, "cheat finalexam");
            // document.InsertText(table[11, 0].Range.Start, "cheat e=mc2");
            // document.InsertText(table[12, 0].Range.Start, "cheat saddam");
            // document.InsertText(table[13, 0].Range.Start, "cheat saddamme");
            // document.InsertText(table[14, 0].Range.Start, "cheat eventnow");
            // document.InsertText(table[15, 0].Range.Start, "cheat breakground");
            // document.InsertText(table[16, 0].Range.Start, "cheat moreoffers");
            // document.InsertText(table[17, 0].Range.Start, "cheat nomove");
            // document.InsertText(table[18, 0].Range.Start, "cheat blueskies");
            // document.InsertText(table[19, 0].Range.Start, "cheat georgeww");
            // document.InsertText(table[20, 0].Range.Start, "cheat trumpme");
            //
            // document.InsertHtmlText(table[0, 1].Range.Start, $"");
            // document.InsertHtmlText(table[0, 1].Range.Start, $@"Causes the World Market or the U.N. to have 0% relations with you. In SR2020 your U.N. membership may be lost.
            // ");
            // document.InsertHtmlText(table[0, 1].Range.Start, $"");
            // document.InsertHtmlText(table[0, 1].Range.Start, $"");
            // document.InsertHtmlText(table[0, 1].Range.Start, $"");
            // document.InsertHtmlText(table[0, 1].Range.Start, $"");
            // table.EndUpdate();
            // wb.DocumentText = page;
            // wb.ScriptErrorsSuppressed = true;
        }

        public void SRCreateChangelogTable(RichEditControl re, IList<SRFChangelog> clData)
        {
            string AddChanges(IList<string> changes)
            {
                string str = "";
                for (int i = 0; i < changes.Count; i++)
                {
                    str += $"{changes[i]} <br>";
                }

                return str;
            }

            for (int i = 0; i < clData.Count; i++)
            { 
                Document document = re.Document;
                var logProp = typeof(SRFChangelog).GetProperties();
                document.BeginUpdate();
                var table = document.Tables.Create(document.Range.End, logProp.Length, 2);
                document.EndUpdate();
                
                table.BeginUpdate();
                var startRow = table.Rows.Count != logProp.Length ? logProp.Length : 0;
                    for (int j = 0; j < logProp.Length; j++)
                    {
                        re.Document.InsertText(table[j + startRow, 0].Range.Start, logProp[j].Name);
                        if (logProp[j].Name.ToLower() == "changes")
                            re.Document.InsertHtmlText(table[j + startRow, 1].Range.Start, $"{AddChanges((IList<string>)logProp[j].GetValue(clData[i]))}");
                        else 
                            re.Document.InsertHtmlText(table[j + startRow, 1].Range.Start, $"{logProp[j].GetValue(clData[i])}");
                    }
                table.EndUpdate();
            }
        }
        public void SRProductInformation(RichEditControl re)
        {
            if (re == null) return;
            var info = SRMain.Instance.Data;
            var SRStatus = (info.SRFStatus ? "Active" : "Inactive");
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            
            re.ReadOnly = true;
            re.ShowCaretInReadOnly = false;
            //re.Enabled = false;
            
            re.ForeColor = Color.Azure;
            // re.Appearance.Text.Font.Size = 10;
            // re.Font.Size;
            Document document = re.Document;
            Table? table = document?.Tables.Create(document?.Range.End, 12, 2);
            if(table == null) return;

            string AddSocialRow()
            {
                string str = "";
                for (int i = 0; i < info.SRFSocial.Count; i++)
                {
                    str += $"{info.SRFSocial[i].SocialName}: {info.SRFSocial[i].SocialAccount} <br>";
                }

                return str;
            }

            string InfoWhichDataIsArrayToBreakString(string[] data)
            {
                string str = "";
                foreach (var dt in data)
                {
                    str += $"{dt}<br>";
                }

                return str;
            }
            
            table.BeginUpdate();
            //Insert the header data
            document.InsertText(table[0, 0].Range.Start, "Product Name");
            document.InsertText(table[1, 0].Range.Start, "Product Description");
            document.InsertText(table[2, 0].Range.Start, "Product Meta Version");
            document.InsertText(table[3, 0].Range.Start, "Product Status");
            document.InsertText(table[4, 0].Range.Start, "Product Update Link");
            document.InsertText(table[5, 0].Range.Start, "Author Name");
            document.InsertText(table[6, 0].Range.Start, "Author Contact");
            document.InsertText(table[7, 0].Range.Start, "Author Website");
            document.InsertText(table[8, 0].Range.Start, "Author Social");
            document.InsertText(table[9, 0].Range.Start, "@Known Bugs");
            document.InsertText(table[10, 0].Range.Start, "@Todos");
            document.InsertText(table[11, 0].Range.Start, "@Revision Logs");
            
            document.InsertHtmlText(table[0, 1].Range.Start, $"{info.SRFName}");
            document.InsertHtmlText(table[1, 1].Range.Start, $"{info.SRFDescription}");
            document.InsertHtmlText(table[2, 1].Range.Start, $"{info.SRFVersion} (Rev. {info.SRFRevision})");
            document.InsertHtmlText(table[3, 1].Range.Start, $"{SRStatus}");
            document.InsertHtmlText(table[4, 1].Range.Start, $"{info.SRFDownloadLink}");
            document.InsertHtmlText(table[5, 1].Range.Start, $"{info.SRFAuthor}");
            document.InsertHtmlText(table[6, 1].Range.Start, $"{info.SRFContact}");
            document.InsertHtmlText(table[7, 1].Range.Start, $"{info.SRFWebsite}");
            document.InsertHtmlText(table[8, 1].Range.Start, $"{AddSocialRow()}");
            document.InsertHtmlText(table[9, 1].Range.Start, $"{InfoWhichDataIsArrayToBreakString(info.SRFKnownBugs)}");
            document.InsertHtmlText(table[10, 1].Range.Start, $"{InfoWhichDataIsArrayToBreakString(info.SRFTodos)}");
            document.InsertHtmlText(table[11, 1].Range.Start, $"{InfoWhichDataIsArrayToBreakString(info.SRFRevisionLogs)}");
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

        public void SRGenerateLinksPopup()
        {
            XtraDialogArgs args = new XtraDialogArgs(null, 
                new DonationLinkViewer(), 
                "Links", new[] { DialogResult.No }, 0);
            args.Showing += (sender, e) =>
            {
                e.Buttons[DialogResult.No].Text  = "No Way!";
            };
            XtraDialog.Show(args);
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

        public void Dispose()
        {
        }
    }
}