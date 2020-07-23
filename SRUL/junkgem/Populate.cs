using System.Management.Instrumentation;
using System.Windows.Forms;
using Memories;

namespace SRUL
{
    public static class Populate
    {
        private static Meme instance;
        static Populate()
        {
            
        }
        public static void ADayFeaturePopulateLv(ListView lv)
        {
            lv.BeginUpdate();
            lv.Clear();
            lv.View = View.Details;
            lv.Columns.Add("1 Day Click Feature");
            lv.Columns.Add("Description");

            ListViewItem lvi;
            lvi = new ListViewItem(new string[] { "1 Day Build", "Click" });
            lv.Items.Add(lvi);
            lvi = new ListViewItem(new string[] { "1 Day Army", "Click" });
            lv.Items.Add(lvi);
            lvi = new ListViewItem(new string[] { "1 Day Research", "Description" });
            lv.Items.Add(lvi);
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            lv.EndUpdate();
        }

        public static void ADayFeatureActivation(ListView lv, ReadWrite m)
        {
            if (lv.Items[0].Checked)
                m.ADayClickBuild = m.ADayClickBuild;
            if (lv.Items[1].Checked)
                m.ADayClickArmy = "0.00001";
            if (lv.Items[2].Checked)
                m.ADayClickResearch = "0.00001";
        }
    }
}