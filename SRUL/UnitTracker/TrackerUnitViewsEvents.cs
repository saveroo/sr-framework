using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using SRUL.Types;
using TB.ComponentModel;

namespace SRUL.UnitTracker;

public partial class TrackedUnitController
{
    void InitializeEvents(GridView master, GridView detail)
    {
        InitializeMasterEvents(master);
        InitializeDetailEvents(detail);
    }
    void InitializeMasterEvents(GridView view)
    {
        view.PopupMenuShowing += OnPopupMenuShowing;
        view.MasterRowExpanded += OnMasterRowExpanded;
        view.ValidatingEditor += OnGvMasterOnValidatingEditor;
        view.RowStyle += OnGvMasterOnRowStyle;
        view.MouseDown += OnGvMouseDown;
        view.CustomRowCellEdit += OnCustomRowCellEdit;
        view.DoubleClick += OnGvDoubleClick;
        
        // TrackedUnitViews
        //         .GetRepositoryItems
        //             <RepositoryItemSpinEdit>("spinForMaster")!.EditValueChanged 
        //     += (sender, args) => TrackedUnitViews.MasterView.PostEditor();
        
    }

    void InitializeDetailEvents(GridView view)
    {
        view.CustomColumnDisplayText += PersistentCustomColumnDisplayText;
        view.CustomRowCellEdit += PersistentCustomRow;
        view.ValidatingEditor += PersistentValidatingEditor_Events;
        view.CustomRowFilter += OnGvTrackedUnitStats_CustomRowFilter;
    } 
   
    void ClearMasterEvents(GridView view)
    {
        view.PopupMenuShowing -= OnPopupMenuShowing;
        view.MasterRowExpanded -= OnMasterRowExpanded;
        view.ValidatingEditor -= OnGvMasterOnValidatingEditor;
        view.RowStyle -= OnGvMasterOnRowStyle;
        view.MouseDown -= OnGvMouseDown;
        view.CustomRowCellEdit -= OnCustomRowCellEdit;
        view.DoubleClick -= OnGvDoubleClick;
    }
    void ClearDetailEvents(GridView view)
    {
        view.CustomColumnDisplayText -= PersistentCustomColumnDisplayText;
        view.CustomRowCellEdit -= PersistentCustomRow;
        view.ValidatingEditor -= PersistentValidatingEditor_Events;
        view.CustomRowFilter -= OnGvTrackedUnitStats_CustomRowFilter;
    } 
    
    public void OnGvDoubleClick(object sender, EventArgs args) {
        var view = (GridView)sender;
        var hitInfo = view.CalcHitInfo(view.GridControl.PointToClient(Control.MousePosition));
        if (hitInfo.InRow || hitInfo.InRowCell) {
            var row = hitInfo.RowHandle;
            if(view.GetMasterRowExpanded(row)) 
                view.CollapseMasterRow(row);
            else 
                view.ExpandMasterRow(row);
        }
    }

    public void OnGvTrackedUnitStats_CustomRowFilter(object sender, RowFilterEventArgs e)
    {
        GridView view = sender as GridView;
        if (view.GetRowHandle(e.ListSourceRow) == view.FocusedRowHandle) return;
        TrackedUnitStat mc = (view.GetRow(e.ListSourceRow) as TrackedUnitStat);
        // e.Visible = !ListOfSortedRow.PersistentUnitExcludedStats.Contains(mc.StatName);
        if (ListOfSortedRow.PersistentUnitExcludedStats.Contains(mc.StatName))
        {
            e.Visible = false; 
            e.Handled = true;
        } 
    }
    
    void PersistentCustomRow(object sender, CustomRowCellEditEventArgs args)
    {
        GridView gView = sender as GridView;
        GridControl gCtrl = gView.ParentView.GridControl;
        RepositoryItemSpinEdit spin = gCtrl.RepositoryItems["spin"] as RepositoryItemSpinEdit;
        RepositoryItemRatingControl rating = gCtrl.RepositoryItems["rating"] as RepositoryItemRatingControl;
        RepositoryItemSpinEdit spinPercentage = gCtrl.RepositoryItems["spinPercentage"] as RepositoryItemSpinEdit;
        // if(gView.State != GridState.Editing) return;
        if (args.Column.FieldName != "StatValue") return;
        TrackedUnitStat srow = gView.GetRow(args.RowHandle) as TrackedUnitStat;
        var statName = srow.StatName;
        var statValue = srow.StatValue;
        var formatType = srow.MetaFormat;
        var valueType = srow.MetaType;

        if (valueType == "2bytes" || valueType == "2byte") return;
        switch (valueType)
        {
            case "float":
                spin.IsFloatValue = true;
                break;
            default:
                spin.IsFloatValue = false;
                break;
        }

        switch (formatType)
        {
            case "rating":
                if (!statName.Equals("ArmyExperience")) break;
                // TODO : Rating doesn't show default stars if value is below 1, 2 or 3, temporary fix set rating.FirstItemValue to 0.25m
                rating.FirstItemValue = 0.25m;
                rating.ItemCount = 3;
                args.RepositoryItem = rating;
                break;
            case "percentage":
                spinPercentage.EditMask = "#########0%";
                // // var test = value.As<float>().Value;
                // // var tst = test.IsBetween(1, 100);
                // // var ss = tst;
                if (statValue.As<double>().Value.IsBetween(0, 0.1))
                    spinPercentage.Increment = (decimal)float.Parse("0.01");
                else if (statValue.As<double>().Value.IsBetween(0.1, 1))
                    spinPercentage.Increment = (decimal) float.Parse("0.05");
                else if (statValue.As<double>().Value.IsBetween(1, 10))
                    spinPercentage.Increment = (decimal) float.Parse("0.10");
                else if (statValue.As<double>().Value.IsBetween(10, 100))
                    spinPercentage.Increment = (decimal) float.Parse("1");
                else 
                    spinPercentage.Increment = (decimal) float.Parse("0.10");
                // spin.IsFloatValue = true;
                args.RepositoryItem = spinPercentage;
                break;
            default:
                if (valueType.Equals("float"))
                {
                    spin.EditMask = "#########0.00"; 
                    spin.Increment = (decimal)double.Parse("1");
                }
                else
                {
                    spin.EditMask = "#########0";
                    spin.Increment = 1;
                }
                
                var prView = gView.ParentView.GetRow(gView.SourceRowHandle) as TrackedUnit;
                if (statName.Equals("ArmyCurrentStrength"))
                    args.RepositoryItem = !prView.UnitIsNaval ? spin : spinPercentage;
                else if (statName.Equals("ArmyActualStrength"))
                    args.RepositoryItem = !prView.UnitIsNaval ? spin : spinPercentage; 
                else 
                    args.RepositoryItem = spin;
                // // spin.Increment = 0.1m;
                break;
        }
    }
    
    void PersistentValidatingEditor_Events(object sender, BaseContainerValidateEditorEventArgs args)
    {
        ColumnView gView = sender as ColumnView;
        GridColumn column = (args as EditFormValidateEditorEventArgs)?.Column ?? gView.FocusedColumn;
        gView.RefreshEditor(true);
        if (column.Name == "colStatFreeze")
        {
            args.Valid = true; 
            args.Value = args.Value.ToString() == "True" ? "True" : "False";
            gView.PostEditor();
            return;
        }
        if (column.Name != "colStatValue") return;
        var address = gView.GetRowCellValue(gView.FocusedRowHandle, "StatId").ToString();
        var name = gView.GetRowCellValue(gView.FocusedRowHandle, "StatName").ToString();
        var type = gView.GetRowCellValue(gView.FocusedRowHandle, "MetaType").ToString();
        var value = args.Value;
                
        if (SRLoaderForm._srLoader.rw.WriteMemory(address, type, value.ToString()))
            args.Valid = true;
        gView.PostEditor();
        // if (name == "ArmyExperience")
        //     value = args.Value.To<decimal>().ToString("#######.0");
        // if(!gView.GetRowCellValue(gView.FocusedRowHandle, "StatFreeze").To<bool>())
    }
    
    void PersistentCustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
    {
        GridView view = sender as GridView;
    
        // Guard if column is not stat value
        if (e.Column.FieldName != "StatValue") return;

        var unit = view.GetRow(view.GetRowHandle(e.ListSourceRowIndex)) as TrackedUnitStat;
        var parentView = view.ParentView.GetRow(view.SourceRowHandle) as TrackedUnit;
        if (unit == null) return;
        var formatType = unit.MetaFormat;
        var type = unit.MetaType;
        var name = unit.StatName;
        var value = unit.StatValue;
        // var type = (string) view.GetRowCellValue(e.ListSourceRowIndex, "MetaType");
        // var name = (string) view.GetRowCellValue(e.ListSourceRowIndex, "StatName");
        // var value = (string) view.GetRowCellValue(e.ListSourceRowIndex, "StatValue");

        if (name == "ArmySupply")
        {
            e.DisplayText = value.As<float>().Value.ToString();
            return;
        }

        decimal realVal = 0;
        decimal.TryParse(value, out realVal);
        // Guard before converting
        if (e.Value == null || e.Value == "") return;
        // Guard if type is not float
        if (type.IsEither("string", "byte")) return;

        // if (realVal >= decimal.MaxValue) return;
        decimal formattedValue = realVal;

        if (name == "UnitFuelCapacity")
            formattedValue = parentView.GetStatByName("ArmyCurrentStrength").StatValue.As<decimal>().Value * realVal;
        if (name == "UnitSuppliesCapacity")
            formattedValue = parentView.GetStatByName("ArmyCurrentStrength").StatValue.As<decimal>().Value * realVal;

        switch (formatType)
        {
            case "rating":
                if(name == "ArmyExperience") 
                    e.DisplayText = formattedValue.ConvertRatingForDisplay().ToString();
                break;
            case "percentage":
                // if (name == "ArmyExperience") return;
                e.DisplayText = NumericExtension.SafePercentage(formattedValue, 1, "P2");
                break;
            default:
                // if (name == "ArmyExperience") return;
                if (name.Equals("ArmyCurrentStrength"))
                    e.DisplayText = parentView.UnitIsNaval ?
                        NumericExtension.SafePercentage(formattedValue, 1, "P2")
                            : formattedValue.ToString("########0.00");
                else if (name.Equals("ArmyActualStrength"))
                    e.DisplayText = parentView.UnitIsNaval ?
                        NumericExtension.SafePercentage(formattedValue, 1, "P2")
                        : formattedValue.ToString("########0.00");
                else
                    e.DisplayText = formattedValue.ToString("########0.00");
                // e.DisplayText = (decimal.Parse(value).To<int>()).ToString("0");
                break;
        }
    }

    void OnCustomRowCellEdit(object sender, CustomRowCellEditEventArgs args)
    {
        if (args.Column.FieldName != "UnitHealth") return;
        GridView view = sender as GridView;
        GridControl gc = view.GridControl;
        RepositoryItemProgressBar? prgBar = gc.RepositoryItems["progressBarForMaster"] as RepositoryItemProgressBar;
        var unit = (view.GetRow(args.RowHandle) as TrackedUnit);
                
        if(view.RowCount < 1) return;
        if(unit == null) return;
        var actualHealth = unit.GetStatByName("ArmyActualStrength")
            .StatValue;
        decimal d = 1;
        decimal.TryParse(actualHealth, out d);
        if (unit.UnitIsNaval)
            d *= 200;
        if (prgBar != null)
        {
            prgBar.Minimum = 0;
            prgBar.Maximum = d > 0 ? Convert.ToInt32(d) : 0;
            args.RepositoryItem = prgBar;
        }
        // view.RefreshRowCell(args.RowHandle, view.Columns["UnitHealth"]);
    }

    void OnGvMouseDown(object sender, MouseEventArgs args)
    {
        if (args.Button == System.Windows.Forms.MouseButtons.Right)
        {
            GridView view = sender as GridView;
            GridHitInfo hi = view.CalcHitInfo(args.Location);
            DataRow row = view.GetDataRow(hi.RowHandle);
        }
    }
    
    private void OnGvMasterOnValidatingEditor(object sender, BaseContainerValidateEditorEventArgs args)
    {
        GridView o = sender as GridView;
        if (o.SelectedRowsCount >= 0)
        {
            try
            {
                o.BeginUpdate();
                for (int i = 0; i < o.DataRowCount; i++)
                {
                    if (o.IsRowSelected(i)) (o.GetRow(i) as TrackedUnit).SetUnitBattleGroup(args.Value.ToString());
                }
            }
            finally
            {
                o.EndUpdate();
            }
        }
    }
    
    void OnMasterRowExpanded(object sender, CustomMasterRowEventArgs args)
    {
        var control = sender as GridControl; 
        var view = sender as GridView;
        GridView detail = view.GetDetailView(args.RowHandle, args.RelationIndex) as GridView;
        
        // Set options allow edit

        //Set Columns Captions
        // detail.AutoFillColumn = detail.Columns["StatFreeze"];
        if (view.IsMasterRow(view.FocusedRowHandle))
        {
            SRMemento.Instance.proxyMasterDetail(detail);
        }
    }

    void OnPopupMenuShowing(object sender, PopupMenuShowingEventArgs args)
    {
        GridView view = sender as GridView;
        if (args.MenuType == GridMenuType.Row)
        {
            // GridHitInfo hitInfo = view.CalcHitInfo(args.Point);
            int rowHandle = args.HitInfo.RowHandle;
            DXPopupMenu menu = args.Menu;
            menu.Appearance.Options.UseImage = false;
            menu.Appearance.Font = new Font(FontFamily.GenericMonospace, 2);
            menu.Items.Clear();
            menu.Items.Add(CreateSetGroups("Set Groups", view, view.GetSelectedRows()));
            
            menu.Items.Add(CreateGridMenuItem("Heal All Units", view, rowHandle, MenuEventHealAllUnits));
            menu.Items.Add(CreateGridMenuItem("Heal Unit", view, rowHandle, MenuEventHealUnit));
            
            menu.Items.Add(CreateGridMenuItem("3-Star All Units", view, rowHandle, MenuEventVeteranAllUnits));
            menu.Items.Add(CreateGridMenuItem("3-Star Unit", view, rowHandle, MenuEventVeteranUnit));
            
            menu.Items.Add(CreateGridMenuItem("Lock All Unit Stats", view, rowHandle, MenuEventLockAllUnits));
            menu.Items.Add(CreateGridMenuItem("Lock Unit Stats", view, rowHandle, MenuEventLockUnit));
            
            menu.Items.Add(CreateGridMenuItem("Unlock All Units Stats", view, rowHandle, MenuEventUnlockAllUnitsStats));
            
            menu.Items.Add(CreateGridMenuItem("Show Position", view, rowHandle, MenuEventShowPosition));
            var callingUnit = CreateGridMenuItem("Call Unit (WIP)", view, rowHandle, MenuEventCallUnit);
            menu.Items.Add(callingUnit);
            menu.Items.Add(CreateGridMenuItem("Remove Unit", view, rowHandle, MenuEventRemoveUnit));
            menu.Items.Add(CreateGridMenuItem("Remove All Unit", view, rowHandle, MenuEventRemoveAllUnit));
            menu.Items.Add(CreateGridMenuItem("Collapse All", view, rowHandle, MenuEventCollapse));
            callingUnit.Enabled = false;
        }
    }

    void MenuEventCollapse(object sender, EventArgs args)
    {
        DXMenuItem menuItem = sender as DXMenuItem;
        RowInfo rowInfo = menuItem.Tag as RowInfo;
        rowInfo.View.CollapseAllDetails();
    }

    DXMenuItem CreateSetGroups(string text, GridView gv, int[] rowHandle)
    {
        DXSubMenuItem subItem = new DXSubMenuItem("Set Battle Group");
        for (int i = 0; i < 10; i++)
        {
            DXMenuItem item = new DXMenuItem($"Group {i}");
            var ri = new RowInfo(gv, rowHandle);
            ri.OptionalData = i.ToString();
            item.Tag = ri;
            item.Click += OnSetBattleGroup;
            subItem.Items.Add(item);
        }

        return subItem;
    }

    void OnSetBattleGroup(object sender, EventArgs args)
    {
        DXMenuItem menuItem = sender as DXMenuItem;
        RowInfo rowInfo = menuItem.Tag as RowInfo;
        if (rowInfo != null)
            foreach (var row in rowInfo.RowHandles)
            {
                // UnitPersistentList[row].SetUnitBattleGroup(rowInfo.OptionalData);
                (rowInfo.View.GetRow(row) as TrackedUnit).SetUnitBattleGroup(rowInfo.OptionalData);
                // var unit = UnitPersistentList[row];
                // unit.SetUnitBattleGroup(i.ToString());
                // if (o.IsRowSelected(i)) (o.GetRow(i) as PersistentUnit).SetUnitBattleGroup(args.Value.ToString());
            }
    }

    private DXMenuItem CreateGridMenuItem(string text, GridView gv, int rowHandle, EventHandler evt)
    {
        DXMenuItem item = new DXMenuItem(text, evt);
        item.Tag = new RowInfo(gv, rowHandle);
        return item;
    }

    private void MenuEventRemoveUnit(object sender, EventArgs e)
    {
        DXMenuItem? item = sender as DXMenuItem;
        RowInfo info = item.Tag as RowInfo;
        if (info == null) return;
        RemoveSelectedTrackedUnitInGridview(info.View);
    }

    private void MenuEventRemoveAllUnit(object sender, EventArgs e)
    {
        DXMenuItem? item = sender as DXMenuItem;
        RowInfo info = item.Tag as RowInfo;
        if (info == null) return;
        RemoveAllTrackedUnit(info.View);
    }

    private void MenuEventCallUnit(object sender, EventArgs e)
    {
        DXMenuItem? item = sender as DXMenuItem;
        RowInfo info = item.Tag as RowInfo;
        if (info == null) return;
        (info.View.GetRow(info.RowHandle) as TrackedUnit)?.TeleportToMouse();
    }

    private void MenuEventShowPosition(object sender, EventArgs e)
    {
        DXMenuItem? item = sender as DXMenuItem;
        RowInfo info = item.Tag as RowInfo;
        if (info == null) return;
        (info.View.GetRow(info.RowHandle) as TrackedUnit)?.ShowPosition();
    }

    private void MenuEventHealUnit(object sender, EventArgs e)
    {
        DXMenuItem? item = sender as DXMenuItem;
        RowInfo info = item.Tag as RowInfo;
        if (info == null) return;
        for (int i = 0; i < info.View.GetSelectedRows().Length; i++)
            (info.View.GetRow(info.View.GetSelectedRows()[i]) as TrackedUnit)?.HealStats();
    }

    private void MenuEventHealAllUnits(object sender, EventArgs e)
    {
        DXMenuItem? item = sender as DXMenuItem;
        RowInfo info = item.Tag as RowInfo;
        if (info == null) return;
        if (info.View.RowCount < 1) return;
        for (int i = 0; i < TrackedUnits.Count; i++)
            (info.View.GetRow(info.View.GetRowHandle(i)) as TrackedUnit)?.HealStats();
    }
    
    private void MenuEventVeteranAllUnits(object sender, EventArgs e)
    {
        DXMenuItem? item = sender as DXMenuItem;
        RowInfo info = item.Tag as RowInfo;
        if (info == null) return;
        if (info.View.RowCount < 1) return;
        for (int i = 0; i < TrackedUnits.Count; i++) {
            (info.View.GetRow(info.View.GetRowHandle(i)) as TrackedUnit)?.ThreeStarStats();
        }
    }
    private void MenuEventVeteranUnit(object sender, EventArgs e)
    {
        DXMenuItem? item = sender as DXMenuItem;
        RowInfo info = item.Tag as RowInfo;
        if (info == null) return;
        for (int i = 0; i < info.View.GetSelectedRows().Length; i++)
            (info.View.GetRow(info.View.GetSelectedRows()[i]) as TrackedUnit)?.ThreeStarStats();
    }
    
    private void MenuEventLockAllUnits(object sender, EventArgs e)
    {
        DXMenuItem? item = sender as DXMenuItem;
        RowInfo info = item.Tag as RowInfo;
        if (info == null) return;
        if (info.View.RowCount < 1) return;
        if (TrackedUnits == null) return;
        for (int i = 0; i < info.View.RowCount; i++) 
            Task.Factory.StartNew((unit) =>
            {
                (unit as TrackedUnit)?.FreezeStats(true);
            }, info.View.GetRow(info.View.GetRowHandle(i)));
    }
    
    private void MenuEventLockUnit(object sender, EventArgs e)
    {
        DXMenuItem? item = sender as DXMenuItem;
        RowInfo info = item.Tag as RowInfo;
        if (info == null) return;
        for (int i = 0; i < info.View.GetSelectedRows().Length; i++)
            (info.View?.GetRow(info.View.GetSelectedRows()[i]) as TrackedUnit)?.FreezeStats(true);
    }

    private void MenuEventUnlockAllUnitsStats(object sender, EventArgs e)
    {
        DXMenuItem? item = sender as DXMenuItem;
        RowInfo info = item.Tag as RowInfo;
        if (info == null) return;
        if (TrackedUnits == null) return;
        Task.Factory.StartNew((trackedUnits) =>
        {
            var t = trackedUnits as IList<TrackedUnit>;
            for (int i = 0; i < t.Count; i++)
                (t[i])?.FreezeStats(false);
        }, TrackedUnits);
    }
    
    private void OnGvMasterOnRowStyle(object sender, RowStyleEventArgs args)
    {
        var view = sender as GridView;
        if (args.RowHandle >= 0)
        {
            TrackedUnit address = view.GetRow(args.RowHandle) as TrackedUnit;
            if (address == null) return;
            if (address.CheckIfSelected())
            {
                args.Appearance.BackColor = Color.DarkGreen;
            }
            else if (!address.UnitStatus)
                if (view.Columns.ColumnByFieldName("UnitHealth") != null)
                    args.Appearance.BackColor = Color.FromArgb(49, 2, 8);
        }
    }
}

class RowInfo {
    public RowInfo(GridView view, int rowHandle) {
        this.RowHandle = rowHandle;
        this.View = view;
    }
    public RowInfo(GridView view, int[] rowHandle) {
        this.RowHandles = rowHandle;
        this.View = view;
    }
    public GridView? View;
    public int RowHandle;
    public int[]? RowHandles;
    public string? OptionalData;
    public string[]? OptionalDataCollection = null;
        
}