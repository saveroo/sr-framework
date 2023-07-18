using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Formatting;
using System.Threading;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using SRUL.Types;
using TB.ComponentModel;

namespace SRUL.UnitTracker;

public partial class TrackedUnitController : IDisposable
{
    public EventHandler OnTrackedUnitsUpdated;

    public void OnTrackedUnitsChanged()
    {
        EventHandler handler = OnTrackedUnitsUpdated; // for thread safety
        if (handler  != null)
            handler (this, EventArgs.Empty);
    }
    
    public IList<TrackedUnit>? TrackedUnits;
    public TrackedUnitViews TrackedUnitViews;
    public TrackedUnitController()
    {
        TrackedUnits = new List<TrackedUnit>();
    }

    public void SetMasterDetailControlViews(ref GridControl gc,ref GridView gvMaster,ref GridView gvDetail)
    {
        TrackedUnitViews.SetControl(ref gc);
        TrackedUnitViews.SetMasterDetailView(ref gvMaster,ref gvDetail);
    }

    public void Initialize(GridControl gc, GridView gvMaster,GridView gvDetail)
    {
        
        // if(TrackedUnitViews == null) 
            TrackedUnitViews = new TrackedUnitViews(gc, gvMaster, gvDetail);
        // else
        // {
        //     TrackedUnitViews.SetControl(gc);
        //     TrackedUnitViews.SetMasterView(gvMaster);
        //     TrackedUnitViews.SetDetailView(gvDetail);
        //     TrackedUnitViews.MasterView.PopulateColumns(TrackedUnits);
        //     TrackedUnitViews.DetailView.PopulateColumns(new TrackedUnitStat());
        // }

        TrackedUnitViews.GridControl.DataSource = TrackedUnits;
        TrackedUnitViews.GridControl.ForceInitialize();

        TrackedUnitViews.InitializeViews(TrackedUnitViews.MasterView, TrackedUnitViews.DetailView);

        // start
        TrackedUnitViews.MasterView.BeginUpdate();
        InitializeMasterEvents(TrackedUnitViews.MasterView);
        TrackedUnitViews.MasterView.EndUpdate();
        //end
        
        // start
        TrackedUnitViews.DetailView.BeginUpdate();
        InitializeDetailEvents(TrackedUnitViews.DetailView);
        TrackedUnitViews.DetailView.EndUpdate();
        // end
    }

    public void ClearEvent(GridControl gc, GridView gvMaster,GridView gvDetail)
    {
        gc.DataBindings.Clear();
        gc.DataSource = null;
        ClearMasterEvents(gvMaster);
        ClearDetailEvents(gvDetail);
    }
    
    public void InitDetailsView(GridControl control, GridView view)
    {
        var spin = control.RepositoryItems["spin"] as RepositoryItemSpinEdit;
        var rating = control.RepositoryItems["rating"] as RepositoryItemRatingControl;
        
        // view.DetailView.PopulateColumns(
        //     view.MasterView.DataController.GetDetailList(0, 0));
        
        // Set Spin Properties
        spin.AllowMouseWheel = true;
        spin.AllowNullInput = DefaultBoolean.False;
        spin.ValidateOnEnterKey = true;
        
        spin.EditValueChanged += (sender, args) =>
        {
            view.PostEditor();
        };
        rating.EditValueChanged += (sdr, ee) =>
        {
            view.PostEditor();
        };
        rating.ItemClick += (sdr, ee) =>
        {
            var o = sdr as RatingControl;
            // XtraMessageBox.Show(ee.Index.ToString());
            view.PostEditor();
        };
    }

    private TrackedUnitStat _tmp;
    private IDisposable _disposableImplementation;

    public void RefreshMasterTimer(GridView? gv, SRReadWrite rw)
    {
        if (gv == null) return;
        if (gv.DataRowCount < 1) return;
        for (int i = 0; i < gv.DataRowCount; i++)
        {
            for (int j = 0; j < TrackedUnits.Count; j++)
            {
                _tmp = TrackedUnits[j].GetStatByName("ArmyCurrentStrength").Read();
                var unitCurrent = decimal.Parse(_tmp.StatValue == String.Empty ? 0.ToString() : _tmp.StatValue);
                unitCurrent = TrackedUnits[j].UnitIsNaval ? unitCurrent * 200m : unitCurrent;
                // TODO: replaced with checking in Rowstyle inside TrackedUnitViesEvents.cs 
                // Check if unit is selected
                // TrackedUnits[j].IsSelected = TrackedUnits[j].Address == "ArmyCurrentStrength".GetPointerUIntPtr(rw);
                    
                    
                TrackedUnits[j].UnitKills = TrackedUnits[j].GetStatByName("ArmyKill").StatValue;
                TrackedUnits[j].UnitBattleGroup = TrackedUnits[j].GetUnitBattleGroup();
                TrackedUnits[j].UnitHealth = (int)(unitCurrent);
                
                // UnitPersistentList[j].UnitIsNaval =
                //     SRLoaderForm._srLoader.rw.SRIsNaval(UnitPersistentList[j].GetStatByName("UnitClass").StatValue.To<int>());

                // gv.RefreshRowCell(j, gv.Columns["UnitStatus"]);
                // gv.RefreshRowCell(j, gv.Columns["UnitHealth"]);
                // gv.RefreshRowCell(j, gv.Columns["UnitKill"]);
                // gv.RefreshRowCell(j, gv.Columns["UnitBattleGroup"]);
            }
            if(gv.IsVisible) 
                if(gv.IsRowVisible(i) != RowVisibleState.Hidden) 
                    gv.RefreshRow(i);
        }
    }
    
    public void RefreshDetailTimer(GridView? gv, SRReadWrite rw)
    {
        if (gv == null) return;
        if (gv.State == GridState.Editing) return;
        for (int i = 0; i < TrackedUnits.Count; i++)
        {
            for (int j = 0; j < TrackedUnits[i].DisplayStats.Count; j++)
            {
                // if unit is not freezed, then update the value from the SR
                if (!TrackedUnits[i].DisplayStats[j].StatFreeze)
                    TrackedUnits[i].DisplayStats[j].Read();
                // rw.Read( UnitPersistentList[i].DisplayStats[j].MetaType, 
                //     UnitPersistentList[i].DisplayStats[j].StatId).ToString();
            }
        }
        for (int j = 0; j < gv.DataRowCount; j++)
        {
            // if (gv.GetRowCellValue(j, "gridId").Equals(j)) 
            // gv.SetRowCellValue(j, gv.Columns["gridId"], j);
            gv.RefreshRowCell(j, gv.Columns["StatValue"]);
            // gv.RefreshRowCell(j, gv.Columns["StatName"]);
            gv.RefreshRowCell(j, gv.Columns["StatFreeze"]);
        }
    }
    
    // Instead of .Clone
        IList<Feature> ClonedFeatures(IEnumerable<Feature> originalFeatures)
        {
            var newFeatures = new List<Feature>();
            foreach (var originalFeature in SRMain.Instance.WarfareIndexedFeatures)
            {
                if (!ListOfSortedRow.PersistentUnitIncludedStats.Contains(originalFeature.Key)) 
                    if(!ListOfSortedRow.PersistentUnitExcludedStats.Contains(originalFeature.Key)) continue;
                if (originalFeature.Value != null)
                    newFeatures.Add(new Feature()
                    {
                        pointerId = originalFeature.Value.pointerId,
                        name = originalFeature.Value.name,
                        type = originalFeature.Value.type,
                        offset = originalFeature.Value.offset,
                        format = originalFeature.Value.format,
                        formattedValue = originalFeature.Value.formattedValue,
                        value = originalFeature.Value.value,
                    });
            }
            return newFeatures;
        } 
        // Making persistent unit
        // First are cloning the pointers, using predicted stat as a haystack.
        // Keep the reference pointer to compare
        // Create the new cloned unit to the class and list.
        // then freeze.
        private TrackedUnit? _trackedUnitDraft;
        public bool MakePersistent(SRReadWrite rw, IList<Feature> feats, bool freeze = false)
        {
            var result = false;
            var feat = ClonedFeatures(feats); // Clone feature
            var ptrReference = feats.GetFeatureByName("ArmyCurrentStrength").GetPointerUIntPtr(rw);
            var ptrClone = feat.GetFeatureByName("ArmyCurrentStrength").GetPointerUIntPtr(rw);
            
            if (ptrClone == UIntPtr.Zero) return result;
            if (ptrClone.ToUInt64() < 0x10000) return result;
            if (ptrReference == UIntPtr.Zero) return result;
            if (ptrReference.ToUInt64() < 0x10000) return result;
            
            var unitId = feat.GetFeatureByName("UnitID").SetFromRead(rw).value;
            if (unitId.Equals("65535")) return result; // ID guard
            if (unitId.Equals("-1")) return result; // ID guard
            if (unitId.Equals("-2")) return result; // ID guard
            
            var unitName = feat.GetFeatureByName("UnitName").SetFromRead(rw).value;
            if (string.IsNullOrEmpty(unitName)) return result;
            if (unitName == "Garrison") return result;

            var unitClass = feat.GetFeatureByName("UnitClass").SetFromRead(rw).value;
            if (string.IsNullOrEmpty(unitClass)) return result;
            // public static bool IsNullOrEmpty(this string value) => value == null || value.Length == 0;

            foreach (var f in feat)
            {
                if (!ListOfSortedRow.PersistentUnitIncludedStats.Contains(f.name)) continue;
                f.SetFromRead(rw);
            }
            
            foreach (var s in TrackedUnits) { if (s.Address == ptrReference) return result; }
            _trackedUnitDraft = new TrackedUnit(unitId, unitName) {Address = ptrClone};
            int classId = ObjectExtension.To<int>(unitClass);
            _trackedUnitDraft.DisplayStats = new List<TrackedUnitStat>();
            _trackedUnitDraft.IsSelected = false;
            _trackedUnitDraft.UnitPlatoon = StringBuffer.Format("{1} {0}",
                "ArmyBattalionNumber".GetFeature(feat).value.ToOrdinal(),
                SRMain.Instance.GetFormatData("UnitClass".GetFeature(feat), 1)[classId]
            );

            foreach (var feature in ListOfSortedRow.PersistentUnitIncludedStats)
            {
                // TODO: do some clone wisely other than radically loop thru all those useless fatures
                var clonedStat = feature.GetFeature(SREnum.CategoryName.Warfare)?.Clone() as Feature;
                if (clonedStat == null) return false;
                clonedStat = clonedStat.SetFromRead(rw);
                var staticPointer = clonedStat.name.GetPointer(rw);
                _trackedUnitDraft.Stats.Add(staticPointer, clonedStat.value);

                var unitDetails = new TrackedUnitStat
                {
                    StatToken = new CancellationTokenSource(),
                    StatFreeze = !ListOfSortedRow.PersistentUnitExcludedStats.Contains(clonedStat.name) ? freeze : false,
                    StatId = staticPointer,
                    StatName = clonedStat.name,
                    StatValue = clonedStat.value,
                    StatFormattedValue = clonedStat.formattedValue,
                    MetaFormat = clonedStat.format,
                    MetaType = clonedStat.type
                };
                
                // Set master row stat
                if (clonedStat.name == "ArmyBattleGroup")
                    _trackedUnitDraft.UnitBattleGroup = unitDetails.StatValue;
                if (clonedStat.name == "ArmyKill")
                    _trackedUnitDraft.UnitKills = unitDetails.StatValue;

                if (clonedStat.name == "ArmyPositionX")
                    _trackedUnitDraft.UnitPositionX = unitDetails.StatValue;
                if (clonedStat.name == "ArmyPositionY")
                    _trackedUnitDraft.UnitPositionY = unitDetails.StatValue;
                
                _trackedUnitDraft.DisplayStats.Add(unitDetails);
                
                if(clonedStat.name == "UnitBattleGroup") 
                    _trackedUnitDraft.PropertyChanged += (sender, args) => 
                    { 
                        if(args.PropertyName == "UnitBattleGroup") 
                            _trackedUnitDraft.UnitBattleGroup = unitDetails.StatValue; 
                    };
            }

            // TODO: CLEAR after?
            TrackedUnits.Add(_trackedUnitDraft);
            _trackedUnitDraft.GridId = TrackedUnits.Count - 1;
            feat = null;
            OnTrackedUnitsChanged();
            return true;
        }
    
    public int GetTrackedRowId(SRReadWrite rw)
    {
        if (TrackedUnits == null) return -1;
        var address = "ArmyCurrentStrength".GetPointerUIntPtr(rw);
        if(!IsUnitTracked(address)) return -1;
        // check if trackedunits matches address without linq
        foreach (var trackedUnit in TrackedUnits)
        {
            if (trackedUnit.Address == address) 
                return trackedUnit.GridId;
        }
        return -1;
    }
    
    public bool IsUnitTracked(string address)
    {
        foreach (var unit in TrackedUnits)
        {
            if (unit.Address.ToUInt32().ToString("X") == address)
                return true;
        }

        return false;
    }
    
    public bool IsUnitTracked(UIntPtr address)
    {
        foreach (var unit in TrackedUnits)
        {
            if (unit.Address == address)
                return true;
        }

        return false;
    }
    
    public void RemoveAllTrackedUnit(GridView view)
    {
        view.BeginDataUpdate();
        foreach (var unit in TrackedUnits)
        {
            foreach (var stat in unit.DisplayStats)
            {
                SRTokenUnfreezer(stat);
            }
        }
        view.EndDataUpdate();
            
        for (int i = 0; i < view.RowCount;)
        {
            view.DeleteRow(i);
        }
    }
    
    public void RemoveSelectedTrackedUnit(GridView view)
    {
        view.BeginUpdate();
        foreach (var unit in TrackedUnits)
        {
            if (unit.CheckIfSelected())
            {
                foreach (var stat in unit.DisplayStats)
                {
                    SRTokenUnfreezer(stat);
                }
                TrackedUnits.Remove(unit);
                break;
            }
        }
        view.RefreshData();
        view.EndUpdate();
        OnTrackedUnitsChanged();
    }
    

    public void RemoveSelectedTrackedUnitInGridview(GridView view)
    {
        for (int i = 0; i < view.GetSelectedRows().Length;)
        {
            if (i > 200) break;
            // cancel token
            var unit = view.GetRow(view.GetSelectedRows()[i]) as TrackedUnit;
            if (unit == null) continue;
            foreach (var stat in unit.DisplayStats)
            {
                SRTokenUnfreezer(stat);
            }
            view.DeleteSelectedRows();
        }
    }
    
    public bool SRTokenUnfreezer(TrackedUnitStat trackedUnit)
    {
        try
        {
            if(trackedUnit.StatFreeze)
                trackedUnit.StatFreeze = false;
            else
                trackedUnit.StatToken.Cancel();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    void IDisposable.Dispose()
    {
        _disposableImplementation.Dispose();
        if (this.TrackedUnits != null)
        {
            this.TrackedUnits.Clear();
            SRViews.CreateInstance().Dispose();
        }
    }
}