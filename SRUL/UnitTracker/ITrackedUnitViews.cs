using DevExpress.Data;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace SRUL.UnitTracker;

public interface ITrackedUnitViews
{
    GridControl GridControl { get; set; }
    GridView MasterView { get; set; }
    GridView DetailView { get; set; }
    
    void SetControl(ref GridControl Control);
    void SetControl(GridControl Control);
    void SetMasterView(ref GridView MasterView);
    void SetMasterView(GridView MasterView);
    void SetDetailView(ref GridView DetailView);
    void SetDetailView(GridView DetailView);
    void RefreshView();
}