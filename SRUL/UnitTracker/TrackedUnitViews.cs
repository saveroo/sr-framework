using System;
using System.Drawing;
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout.Utils;
using Padding = System.Windows.Forms.Padding;

namespace SRUL.UnitTracker;

public struct ViewsCollection
{
    public static readonly string[] ExcludedMasterColumns = new[]
    {
        // "UnitId",
        // "Address",
        // "Stats",
        // "GlobalStats",
        // "UnitPositionX",
        // "UnitPositionY",
        // "RowId",
        // "IsSelected",
        "UnitBattalionNumber",
        "UnitPlatoon",
        "UnitName",
        "UnitBattleGroup",
        "UnitKills",
        "UnitStatus",
        "UnitHealth",
    };
    
    public static readonly string[] ExcludedDetailColumns = new[]
    {
        "StatId", 
        "StatToken", 
        "StatFormattedValue", 
        "MetaFormat", 
        "MetaType", 
        // "StatName"
    };

}

public class TrackedUnitViews : ITrackedUnitViews
{
    private GridControl _gridControl;
    private GridView _masterView;
    private GridView _detailView;

    public TrackedUnitViews(GridControl gridControl, GridView masterView, GridView detailView)
    {
        _gridControl = gridControl;
        _masterView = masterView;
        _detailView = detailView;
    }

    public GridControl GridControl
    {
        get => _gridControl;
        set => _gridControl = value;
    }

    public GridView MasterView
    {
        get => _masterView;
        set => _masterView = value;
    }

    public GridView DetailView
    {
        get => _detailView;
        set => _detailView = value;
    }

    public void SetControl(ref GridControl control)
    {
        GridControl = control;
    }
    public void SetControl( GridControl control)
    {
        GridControl = control;
    }

    public void SetMasterView(ref GridView masterView)
    {
        MasterView = masterView;
    }
    public void SetMasterView(GridView masterView)
    {
        MasterView = masterView;
    }


    public void SetDetailView(ref GridView detailView)
    {
        DetailView = detailView; 
    }
    public void SetDetailView( GridView detailView)
    {
        DetailView = detailView; 
    }

    public void SetMasterDetailView(ref GridView masterView, ref GridView detailView)
    {
        SetMasterView(ref masterView);
        SetDetailView(ref detailView);
    }

    public void InitializeViews(GridView master, GridView detail)
    {
        InitializeRepositoryEditor();
        InitializeRepositoryEditorProperties();
        InitializeMasterViews(master);
        InitializeDetailViews(detail);

        master.OptionsView.ShowIndicator = false;
        detail.OptionsView.ShowIndicator = false;
    }

    private void InitializeMasterViews(GridView master = null)
    {
        var masterView = master ?? MasterView;
        // Set Column visibility
        
        // Set options allow edit
        // if (GetColumnByFieldName(masterView, "RowId") != null)
        // { 
            SetColumnsVisibility(ref masterView, ViewsCollection.ExcludedMasterColumns, true);
            // GetColumnByFieldName(masterView, "RowId").OptionsColumn.AllowEdit = false;
            GetColumnByFieldName(masterView, "UnitName").OptionsColumn.AllowEdit = false;
            GetColumnByFieldName(masterView, "UnitPlatoon").OptionsColumn.AllowEdit = false;
            GetColumnByFieldName(masterView, "UnitBattleGroup").OptionsColumn.AllowEdit = true;
            GetColumnByFieldName(masterView, "UnitKills").OptionsColumn.AllowEdit = false;
        
            //Set Columns Captions
            GetColumnByFieldName(masterView, "UnitKills").Caption = @"Kills";
            GetColumnByFieldName(masterView, "UnitPlatoon").Caption = @"Platoon";
            GetColumnByFieldName(masterView, "UnitBattleGroup").Caption = @"Group";
            if(GetColumnByFieldName(masterView, "UnitStatus") != null) 
                GetColumnByFieldName(masterView, "UnitStatus").Caption = @"Alive";
            GetColumnByFieldName(masterView, "UnitHealth").Caption = @"Health";    
            GetColumnByFieldName(masterView, "UnitName").Caption = @"Model";    
        // }
        
        
        masterView.OptionsMenu.EnableColumnMenu = true;
        masterView.OptionsMenu.EnableFooterMenu = true;
        masterView.OptionsMenu.EnableGroupPanelMenu = true;
        masterView.OptionsMenu.EnableGroupRowMenu = true;
        masterView.OptionsView.AllowHtmlDrawHeaders = true;
        masterView.OptionsView.AllowGlyphSkinning = true;
        masterView.OptionsSelection.MultiSelect = true;
        masterView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
        masterView.OptionsDetail.ShowDetailTabs = false;
        masterView.OptionsBehavior.EditorShowMode = EditorShowMode.MouseDownFocused;
        masterView.OptionsCustomization.AllowFilter = false;
        masterView.OptionsCustomization.AllowSort = false;
        masterView.OptionsCustomization.AllowColumnMoving = false;
        masterView.OptionsDetail.DetailMode = DetailMode.Embedded;
        
        if(masterView.Columns.ColumnByFieldName("UnitBattleGroup") != null) 
            masterView.Columns["UnitBattleGroup"].ColumnEdit = 
                GridControl
                    .RepositoryItems["spinForMaster"];
        if(masterView.Columns.ColumnByFieldName("UnitHealth") != null) 
            masterView.Columns["UnitHealth"].ColumnEdit = 
                GridControl
                    .RepositoryItems["progressBarForMaster"];  
    }

    public void InitializeDetailViews(GridView detail = null)
    {
        var detailView = detail ?? DetailView;

        SetColumnsVisibility(ref detailView, ViewsCollection.ExcludedDetailColumns, false);
        detailView.OptionsCustomization.AllowFilter = false;
        detailView.OptionsCustomization.AllowSort = false;
        detailView.OptionsCustomization.AllowColumnMoving = false;
        detailView.OptionsView.ShowAutoFilterRow = false;
        detailView.OptionsView.ShowIndicator = false;
        detailView.OptionsBehavior.EditorShowMode = EditorShowMode.Default;
        // detailView.OptionsDetail.DetailMode = DetailMode.Embedded;
        detailView.OptionsView.ShowGroupedColumns = false;
        detailView.OptionsView.ShowGroupPanel = false;
        detailView.OptionsView.EnableAppearanceEvenRow = true;
        
        // detailView.PopulateColumns(new List<TrackedUnitStat>());

        // if (GetColumnByFieldName(detailView, "StatId") != null)
        // {
            // GetColumnByFieldName(detailView, "StatId").OptionsColumn.AllowEdit = false;
            // GetColumnByFieldName(detailView, "StatToken").OptionsColumn.AllowEdit = false;
            // GetColumnByFieldName(detailView, "StatFormattedValue").OptionsColumn.AllowEdit = false;
            // GetColumnByFieldName(detailView, "MetaFormat").OptionsColumn.AllowEdit = true;
            // GetColumnByFieldName(detailView, "MetaType").OptionsColumn.AllowEdit = false;
            GetColumnByFieldName(detailView, "StatName").OptionsColumn.AllowEdit = false;
            GetColumnByFieldName(detailView, "StatFreeze").Caption = @"Lock";
            GetColumnByFieldName(detailView, "StatName").Caption = @"Stat";
            GetColumnByFieldName(detailView, "StatValue").Caption = @"Value";
        // }
        
        var spin = GridControl.RepositoryItems["spin"] as RepositoryItemSpinEdit;
        var rating = GridControl.RepositoryItems["rating"] as RepositoryItemRatingControl;
        
        // view.detailView.PopulateColumns(
        //     view.MasterView.DataController.GetDetailList(0, 0));
        
        // Set Spin Properties
        spin.AllowMouseWheel = true;
        spin.AllowNullInput = DefaultBoolean.False;
        spin.ValidateOnEnterKey = true;
        
        spin.EditValueChanged += (sender, args) =>
        {
            DetailView.PostEditor();
        };
        rating.EditValueChanged += (sdr, ee) =>
        {
            DetailView.PostEditor();
        };
        rating.ItemClick += (sdr, ee) =>
        {
            var o = sdr as RatingControl;
            // XtraMessageBox.Show(ee.Index.ToString());
            DetailView.PostEditor();
        };
    }

    public T? GetRepositoryItems<T>(string name) where T : class
    {
        return GridControl.RepositoryItems[name] as T;
    }

    public void InitializeRepositoryEditorProperties()
    {
        var item = GridControl.RepositoryItems["progressBarForMaster"] as RepositoryItemProgressBar;
        if (item != null)
        {
            item.Appearance.ProgressOptions.UseBackColor = true;
            item.Appearance.ProgressOptions.UseBorderColor = true;
            item.Appearance.BackColor2 = Color.LimeGreen;
            item.Appearance.BackColor = Color.LimeGreen;
            item.StartColor = Color.Brown;
            item.EndColor = Color.LimeGreen;
            item.PercentView = true;
            item.ShowTitle = true;
            item.Step = 1;
            item.ProgressPadding = new Padding(0, 0, 0, 0);
            // item.EditValueChanged += (sender, args) =>
            // {
            //     var progressBar = sender as ProgressBarControl;
            //     if (progressBar == null) return;
            //     var progress = progressBar.EditValue;
            //     var unit = gvMaster.GetRow(.Tag) as PersistentUnit;
            //     if (unit == null) return;
            //     unit.UnitProgress = progress;
            // };
        }
        
        var itemSpin = GridControl.RepositoryItems["spinForMaster"] as RepositoryItemSpinEdit;
        if (itemSpin != null)
        {
            itemSpin.MinValue = 0;
            itemSpin.MaxValue = 9;
            itemSpin.ValidateOnEnterKey = true;
            itemSpin.AllowMouseWheel = true;
        }
        
    }
    
    public GridColumn GetColumnByFieldName(GridView view, string fieldName)
    {
        if (view.Columns.ColumnByFieldName(fieldName) != null)
            return view.Columns.ColumnByFieldName(fieldName);
        return view.Columns.ColumnByFieldName(fieldName);
        // throw new Exception($"Column with field name {fieldName} not found"); 
    }

    public void SetColumnsVisibility(ref GridView gridView,string[] includedColumnsFieldName, bool cond)
    {
        gridView.BeginUpdate();
        foreach (GridColumn view in gridView.Columns)
        {
            if(includedColumnsFieldName.Contains(view.FieldName)) 
                view.Visible = cond;
            else
                view.Visible = !cond;
        }
        gridView.EndUpdate();
    }

    public void RowVisibility()
    {
        
    }

    public void InitializeRepositoryEditor()
    {
        GridControl.RepositoryItems.AddRange(new RepositoryItem[]
        {
            CreateRepositoryEditor<RepositoryItemSpinEdit>("spin"),
            CreateRepositoryEditor<RepositoryItemSpinEdit>("spinForMaster"),
            CreateRepositoryEditor<RepositoryItemSpinEdit>("spinPercentage"),
            CreateRepositoryEditor<RepositoryItemProgressBar>("progressBarForMaster"),
            CreateRepositoryEditor<RepositoryItemRatingControl>("rating"), 
        });
    }

    public T CreateRepositoryEditor<T>(string name) where T : RepositoryItem
    {
        switch (typeof(T))
        {
            case var a when a == typeof(RepositoryItemSpinEdit):
                return new RepositoryItemSpinEdit(){Name = name} as T ?? throw new InvalidOperationException();
            case var a when a == typeof(RepositoryItemRatingControl):
                return new RepositoryItemRatingControl(){Name = name} as T ?? throw new InvalidOperationException();
            case var a when a == typeof(RepositoryItemProgressBar):
                return new RepositoryItemProgressBar(){Name = name} as T ?? throw new InvalidOperationException();
            default:
                return new RepositoryItemSpinEdit(){Name = name} as T ?? throw new InvalidOperationException();
        }
    }

    public void RefreshView()
    {
        MasterView.RefreshData();
    }
}