using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Columns;

namespace SRUL
{
    public class FlashedCell : GridCell
    {

        Timer timer = new Timer();

        public FlashedCell(int rowHandle, GridColumn column, GridView view)
            : base(rowHandle, column)
        {
            _View = view;
            view.RowCellStyle += view_RowCellStyle;
            timer.Tick += timer_Tick;
            timer.Enabled = true;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            isColored = !isColored;
            _View.RefreshRowCell(RowHandle, Column);
        }

        bool isColored;
        void view_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (isColored)
                if (e.RowHandle == RowHandle && e.Column == Column)
                    e.Appearance.Assign(FlashedCellsHelper.FlashedCellAppearance);
        }

        private int _Speed;
        private readonly GridView _View;
        public int Speed
        {
            get { return _Speed; }
            set
            {
                if (value < 0 || _Speed == value)
                    return;
                _Speed = value;
                timer.Stop();
                if(_Speed == 0) return;
                timer.Interval = value;
                timer.Start();
            }
        }


    }
}