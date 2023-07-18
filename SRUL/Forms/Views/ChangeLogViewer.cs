using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraRichEdit;

namespace SRUL.Views
{
    public class ChangeLogViewer : XtraUserControl
    {
        private RichEditControl? re = new RichEditControl();
        private LayoutControl? lc = new LayoutControl();
        private string? _changeLog;
        public ChangeLogViewer(IList<SRFChangelog>? clData)
        {
            if (clData == null) throw new ArgumentNullException(nameof(clData));
            // LayoutControl lc = new LayoutControl();
            lc.Dock = DockStyle.Fill; 
            lc.AllowDrop = false;
            // MemoEdit me = new MemoEdit();
            // me.EditValue = changeLogData;
            // me.ReadOnly = true;
            // RichEditControl re = new RichEditControl();
            re.ActiveViewType = RichEditViewType.Simple;
            re.Views.SimpleView.AdjustColorsToSkins = true;
            re.Appearance.Text.FontSizeDelta = 2;
            re.Options.Behavior.Drop = DocumentCapability.Disabled;
            SRInfo.Instance.SRChangeLog(re, clData);
            // re.LookAndFeel.ActiveLookAndFeel = UserLookAndFeel.Default;
            lc.AddItem(String.Empty, re).TextVisible = false;
            Controls.Add(lc);
            Height = 200;
            Dock = DockStyle.Top;
            base.Disposed -= OnDisposed;
            base.Disposed += OnDisposed;
        }
        
        protected void OnDisposed(object sender, EventArgs e) => Dispose(sender, e);

        private void Dispose(object sender, EventArgs args)
        {
            re = null;
            lc = null;
            _changeLog = null;
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}