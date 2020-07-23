using System;
using System.Collections.Generic;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;

namespace SRUL
{
    public class FlashedCellsHelper
    {

        public static AppearanceObject FlashedCellAppearance = new AppearanceObject();

        List<FlashedCell> flashedCells = new List<FlashedCell>();
        private readonly GridView _View;

        public FlashedCellsHelper(GridView view)
        {
            _View = view;

        }

        FlashedCell FindFlashedCell(int rowHanlde, GridColumn col)
        {
            foreach (FlashedCell cell in flashedCells)
            {
                if (cell.RowHandle == rowHanlde && cell.Column == col)
                    return cell;
            }

            FlashedCell result = new FlashedCell(rowHanlde, col, _View);
            flashedCells.Add(result);
            return result;
        }

        public void SetFlashSpeed(int rowHanlde, GridColumn col, int speed)
        {
            FindFlashedCell(rowHanlde, col).Speed = speed;
        }
    }
}