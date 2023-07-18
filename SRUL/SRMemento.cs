using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using SRUL.Types;
using SRUL.UnitTracker;
using TB.ComponentModel;
using CustomColumnDisplayTextEventArgs = DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs;
using EditFormValidateEditorEventArgs = DevExpress.XtraGrid.Views.Grid.EditFormValidateEditorEventArgs;

namespace SRUL
{
    // Create a class that implements the ISoldier interface.
    public class Soldier<T> where T : class
    {
        public string? SoldierId { get; set; }
        public string? SoldierName { get; set; }
        public bool SoldierAliveStatus { get; set; }
        [Required]
        public IList<T> SoldierStats { get; set; } = new List<T>
        {
            Capacity = 50
        };
    }
    // Create a class that implements the ISoldierStat interface.
    public class SoldierStat
    {
        public CancellationTokenSource StatToken { get; set; }
        public bool StatFreeze { get; set; }
        public string StatAddress { get; set; }
        public string StatName { get; set; }
        public string StatOriginalValue { get; set; }
        public string StatValue { get; set; }
    }

    public interface ISoldierRepository<T> : IRepository<T> where T : class
    {
        // IList<T> GetSoldierStatsById(string soldierId);
        // bool SetSoldierStatByStatName(string fieldName, string soldierId, string statName, string statValue);
        T GetSoldierBySoldierId(string soldierId);
        // T GetSoldierBySoldierName(string soldierName);
        T AddSoldier(T soldier);
    }
    // Create Repository<Soldier>
    public class SoldierRepository<T> : Repository<Soldier<T>>, ISoldierRepository<Soldier<T>> where T : class
    {
        public Soldier<T>? GetSoldierBySoldierId(string soldierId)
        {
            return GetByFieldName("SoldierId", soldierId);
        }

        public Soldier<T> AddSoldier(Soldier<T> soldier)
        { 
            AddIfNotExists(nameof(soldier.SoldierName), soldier.SoldierName, soldier);
            return soldier;
        }
    }

    public class SrSoldierCamp : SoldierRepository<SoldierStat>, IDisposable
    {
        private SRReadWrite readWrite = SRLoaderForm._srLoader.rw;
        public SRMain main = SRLoaderForm._srLoader.jr;
        private SoldierRepository<SoldierStat> _trackedSoldierRepository;
        private SoldierRepository<Feature> _conscriptionSoldierRepository;
        public SrSoldierCamp()
        {
            _conscriptionSoldierRepository = new SoldierRepository<Feature>();
            _trackedSoldierRepository = new SoldierRepository<SoldierStat>();
        }

        public void AddToConscription(IList<Feature> features)
        {
            RecruitmentProfiling((address, features) =>
                {
                    var UnitNam = features.Clone();
                }
            );
        }
        
        public bool RecruitmentProfiling(Action<string, IList<Feature>> act)
        {
            var result = false;
            var feat = main.FeaturesWarfare; // Clone feature
            var ptrClone = "ArmyCurrentStrength".GetPointer(readWrite).Clone().ToString();
            var ptrReference = "ArmyCurrentStrength".GetPointer(readWrite);
            if (ptrClone.Length < 3) return result;
            if (ptrReference.Length < 3) return result;

            var unitId = "UnitID".GetFeature(feat).value;
            if (unitId == "65534" || unitId == "-1") return result; // ID guard
            if (unitId == "0") return result; // ID guard
            
            act(ptrClone, feat);
            return result;
        }
        public bool AddTrackedSoldier(string soldierId, string soldierName)
        {
            return RecruitmentProfiling((soldierAddress, feat) =>
            {
                var soldier = _trackedSoldierRepository.GetSoldierBySoldierId(soldierId);
                if (soldier == null)
                {
                    soldier = new Soldier<SoldierStat>();
                    soldier.SoldierId = soldierAddress;
                    soldier.SoldierAliveStatus = "ArmyCurrentStrength".GetFeature(feat)?.value.To<double>() > 0;
                    soldier.SoldierName = "UnitName".GetFeature(feat).value;
                    foreach (var includedStat in ListOfSortedRow.PersistentUnitIncludedStats)
                    {
                        soldier.SoldierStats.Add(new SoldierStat
                        {
                            StatToken = new CancellationTokenSource(),
                            StatFreeze = false,
                            StatAddress = includedStat.GetPointer(readWrite, feat),
                            StatName = includedStat.GetFeature(feat).name,
                            StatOriginalValue = includedStat.GetFeature(feat).value,
                            StatValue = includedStat.GetFeature(feat).value
                        });
                    }
                    _trackedSoldierRepository.AddSoldier(soldier);
                }
            });
        }

        public bool CheckIfSolderIsTracked(string soldierId)
        {
            return GetByFieldName("SoldierId", soldierId) != null;
            // foreach (var soldier in _trackedSoldierRepository.GetAll())
            // {
            //     if (soldier.SoldierId == soldierId)
            //         return true;
            // }
            // return false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    // Concreate SoldierStats class
    
    // HISTORY
    public struct AddressableModifiedStat
    {
        public string StatAddress { get; set; }
        public string StatName { get; set; }
        public string StatValue { get; set; }
    }

    // History
    public class Unit
    {
        private static object sync = new object();
        private static int _globalCount;
        public int RowId { get; set; } = 0;
        private readonly string _unitAddress;
        private readonly string _unitId;
        private readonly string _unitName;
        private CancellationTokenSource _cts;
        public Dictionary<string, AddressableModifiedStat> ModifiedStats { get; set; }
        private IList<Feature> _unitStats;

        public Unit(string unitAddress, string unitId, string unitName, IList<Feature> unitStats, CancellationTokenSource cts)
        {
            _cts = cts;
            _unitAddress = unitAddress;
            _unitId = unitId;
            _unitName = unitName;
            _unitStats = unitStats;
            ModifiedStats = new Dictionary<string, AddressableModifiedStat>();
            lock (sync)
            {
                RowId = ++_globalCount;
            }
        }

        public string UnitId
        { get { return this._unitId; } }
        
        public string UnitName
        { get { return this._unitName; } }

        public string UnitAddress
        { get { return this._unitAddress; } }

        public IList<Feature> UnitStats
        { get { return this._unitStats; } }
    }
    
    public struct GlobalUnitStoreStruct
    {
        // public string StatAddress { get; set; }
        public string StatName { get; set; }
        public string StatValue { get; set; }
    }

    public class GlobalUnitStore
    {
        public string UnitID { get; set; }
        public string UnitName { get; set; }
        public IList<GlobalUnitStoreStruct> UnitStats { get; set; }
    }
    
    // This Create Box of soldier memento
    public class SRMemento
    {
        // For GridView that show history of modified unit.
        private IList<Unit>? _unitHistoryList;
        // private IList<Unit> _unitPersistentList;

        private IList<GlobalUnitStore> _globalUnitCollection;

        // For GridView that show tracked unit.
        private IList<TrackedUnit> _unitPersistentList;
        
        private ConcurrentDictionary<string, TrackedUnit> UnitePersistenDict;
        
        public ConcurrentDictionary<string, string> TrackedUnitPointerCollection;
        public ConcurrentDictionary<UIntPtr, UIntPtr> TrackedUnitPointerCollectionUIntPtrs;
        // private IList<Unit> _unitList;

        public bool AddToTrackedUnitPointerCollection(string key, string val)
        {
            if (TrackedUnitPointerCollection.ContainsKey(key))
                return false;
            TrackedUnitPointerCollection.TryAdd(key, val);
            return true;
        }
        public string? GetTrackedUnitPointerCollection(string key)
        {
            if (!TrackedUnitPointerCollection.ContainsKey(key))
            {
                string ptr = key.GetStaticAddress().ToUInt32().ToString("X");
                if(AddToTrackedUnitPointerCollection(ptr, ptr))
                    return ptr;
                return null;
            }
            return TrackedUnitPointerCollection[key];
        }
        
         # region UIntPtr caches
        public bool AddToTrackedUnitPointerCollection(UIntPtr key, UIntPtr val)
        {
            if (TrackedUnitPointerCollectionUIntPtrs.ContainsKey(key))
                return false;
            TrackedUnitPointerCollectionUIntPtrs.TryAdd(key, val);
            return true;
        }
        public UIntPtr GetTrackedUnitPointerCollection(UIntPtr key)
        {
            if (!TrackedUnitPointerCollectionUIntPtrs.ContainsKey(key))
            {
                UIntPtr ptr = key;
                if(AddToTrackedUnitPointerCollection(ptr, ptr))
                    return ptr;
                return UIntPtr.Zero;
            }
            return TrackedUnitPointerCollectionUIntPtrs[key];
        }
        #endregion
        
        // Lazy instance.
        private static readonly Lazy<SRMemento> _instance = new Lazy<SRMemento>(() => new SRMemento());
        public SRMemento()
        { 
            TrackedUnitPointerCollection = new ConcurrentDictionary<string, string>();
            TrackedUnitPointerCollectionUIntPtrs = new ConcurrentDictionary<UIntPtr, UIntPtr>();
            _globalUnitCollection = DeserializeGlobalUnitStore();
            _unitHistoryList = new List<Unit>(); 
            _unitPersistentList = new List<TrackedUnit>();   
        }

        public IList<GlobalUnitStore> GlobalUnitCollection
        {
            get { return _globalUnitCollection; }
        }

        public IList<Unit> UnitHistoryList
        {
            get { return _unitHistoryList; }
        }
        public IList<TrackedUnit> UnitPersistentList
        {
            get { return _unitPersistentList; }
        }
        public static SRMemento Instance
        {
            get { return _instance.Value; }
        }

        public IList<GlobalUnitStore> DeserializeGlobalUnitStore()
        {
            if (!File.Exists(@"units.json")) return new List<GlobalUnitStore>();
            using (StreamReader file = File.OpenText(@"units.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                IList<GlobalUnitStore> ret = (IList <GlobalUnitStore>)serializer.Deserialize(file, typeof(IList<GlobalUnitStore>));
                return ret;
            }
        }

        public void SerializeGlobalUnitStore()
        {
            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(@"units.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, GlobalUnitCollection);
            }
        }
        

        public bool SaveToGlobalUnitStore(Dictionary<string, Feature> singleWarfareUnit, SRReadWrite rw)
        {
            if (singleWarfareUnit == null) return false;
            
            // ID Guard
            var unitId = "UnitID".GetFeature().value;
            if (unitId == "65534") return false; // ID Guard
            if (unitId == "0") return false; // ID Guard
            if (unitId == "-1") return false; // ID Guard

            // Unit Name Guard
            var unitName = singleWarfareUnit["UnitName"].value;
            if (_unitHistoryList.Any(s => s.UnitName == unitName)) return false; // Unitname Guard
            if (string.IsNullOrEmpty(unitName)) return false;
            if (unitName == "0") return false;
            if (unitName == "????") return false;
            
            // Pointer Guard
            var ptrClone = "UnitWeight".GetPointerUIntPtr(rw);
            if (!ptrClone.IsValid()) return false; // Pointer Guard length, if it doesnt have pointer, it is not a unit.
            
            var ptrReference = "UnitWeight".GetPointerUIntPtr(rw);
            if (!ptrReference.IsValid()) return false; // Pointer Guard

            // var unitID = singleWarfareUnit.GetFeature("UnitID").value;
            // var unitAddress = SRMain.Instance.pointerStore(singleWarfareUnit.GetFeature("UnitName").name);

            // if(File.Exists("units.json")) DeserializeGlobalUnitStore();
            foreach (var globalUnit in GlobalUnitCollection)
                if (globalUnit.UnitName.Equals(unitName)) 
                    return false;

            // Creating new record for selected unit, and store it to global.
            var newUnit = new GlobalUnitStore();
            newUnit.UnitID = unitId;
            newUnit.UnitName = unitName;
            newUnit.UnitStats = new List<GlobalUnitStoreStruct>();

            foreach (var selectedUnit in singleWarfareUnit)
            {
                if (string.IsNullOrEmpty(selectedUnit.Value.original)) return false;
                var ptr = SRMain.Instance.PointerStore(selectedUnit.Value.name);
                if(ListOfSortedRow.WarfareIncludedFeatureList.Contains(selectedUnit.Value.name))
                    newUnit.UnitStats.Add(new GlobalUnitStoreStruct() 
                    {
                        // StatAddress = rw.GetCode(ptr).ToUInt32().ToString("X"),
                        StatName = selectedUnit.Value.name,
                        StatValue = selectedUnit.Value.original,
                    });
            }

            GlobalUnitCollection.Add(newUnit);
            SerializeGlobalUnitStore();
            return true;
        }

        // TODO: Battle with Heap allocation on cloning, not coupled by calling SRMain Instance
        // Should be called after WarfareTable is edited
        public bool SaveToHistory(IList<Feature> feats, SRReadWrite rw)
        {

            // Unit Name Guard
            var unitName = feats.GetFeatureByName("UnitName").value;
            if (_unitHistoryList.Any(s => s.UnitName == unitName)) return false; // Unitname Guard
            

            // Pointer Guard
            var ptrClone = "UnitWeight".GetPointer(rw);
            if (ptrClone.Length < 3) return false; // Pointer Guard length, if it doesnt have pointer, it is not a unit.
            
            var ptrReference = "UnitWeight".GetPointer(rw);
            if (ptrReference.Length < 3) return false; // Pointer Guard
            
            // ID Guard
            var unitId = "UnitID".GetFeature().value;
            if (unitId == "65534") return false; // ID Guard
            if (unitId == "-1") return false; // ID Guard
            
            // Create New Copied Unit
            var feat = feats.Clone();
            var newUnit = new Unit(ptrClone, 
                "UnitID".GetFeature(feat).value, 
                "UnitName".GetFeature(feat).value, feat,
                new CancellationTokenSource());
            // Add to the list
            _unitHistoryList.Add(newUnit);
            return true;
        }

        // for history unit.
        public void AddModifiedStat(SRReadWrite rw, string statName, string statValue)
        {
            if (_unitHistoryList != null)
            {
                if (_unitHistoryList.Any(s => s.UnitAddress == "UnitWeight".GetPointer(rw)))
                {
                    SRSimpleLog.RecordLog("Modify Stat", $"{statName}: ${statValue}");
                    var history = _unitHistoryList.First(s => s.UnitName == "UnitName".GetFeature().value);
                    if(history.ModifiedStats == null)
                        history.ModifiedStats = new Dictionary<string, AddressableModifiedStat>();
                    history.ModifiedStats[statName] = new AddressableModifiedStat()
                    {
                        StatAddress = statName.GetPointer(rw),
                        StatName = statName,
                        StatValue = statValue
                    };
                }
            }
        }

        // Restoring Persisted Unit to original stat.
        public bool RestoreToOriginal(string unitAddress, IList<Feature> feats, SRReadWrite rw)
        {
            // Unit unique pointer
            var selectedPointer = unitAddress;

            // Init temp bool result
            var isRestored = false;

            // If it isnt in list of modified unit, then return
            if (!_unitHistoryList.Any(s => s.UnitAddress == selectedPointer)) return false;
            
            // Original Stat according to unique pointer
            var original = _unitHistoryList.First(s => s.UnitAddress == selectedPointer);
            
            // We loop thru the original list of stats, if its there.
            var unitName = "";
            foreach (var f in original.UnitStats)
            {
                if (original.ModifiedStats.ContainsKey(f.name))
                {
                    rw.WriteMemory(original.ModifiedStats[f.name].StatAddress, f.type, f.original);
                    isRestored = true;
                    Debug.WriteLine($"[Historical Unit][${f.name}] VAL:${f.value} to ORI:${f.original}");
                } 
            }

            if (isRestored)
            {
                _unitHistoryList.Remove(_unitHistoryList.First(s => s.UnitAddress == selectedPointer));
                SRSimpleLog.RecordLog("Restoring Unit", $"Restored -> {original.UnitStats.First(s => s.name == "UnitName").value}");
                return isRestored;
            }
            SRSimpleLog.RecordLog("Restoring Unit", $"Failed to restore");
            return false;
        }
        
        // Performant Clone of IList<Feature>
        
        

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
        public bool MakePersistent(SRReadWrite rw, IList<Feature> feats, bool freeze = false)
        {
            // var result = false;
            // var feat = ClonedFeatures(feats); // Clone feature
            // var ptrReference = feats.GetFeatureByName("ArmyCurrentStrength").GetPointer(rw);
            // var ptrClone = feat.GetFeatureByName("ArmyCurrentStrength").GetPointer(rw);
            // if (ptrClone.Length < 3) return result;
            // if (ptrReference.Length < 3) return result;
            //
            // var unitId = feat.GetFeatureByName("UnitID").SetFromRead(rw).value;
            // if (unitId.Equals("65535")) return result; // ID guard
            // if (unitId.Equals("-1")) return result; // ID guard
            // if (unitId.Equals("-2")) return result; // ID guard
            //
            // var unitName = feat.GetFeatureByName("UnitName").SetFromRead(rw).value;
            // if (unitName.IsNullOrEmpty()) return result;
            // if (unitName == "Garrison") return result;
            //
            // var unitClass = feat.GetFeatureByName("UnitClass").SetFromRead(rw).value;
            // if (unitName.IsNullOrEmpty()) return result;
            //
            // foreach (var f in feat)
            // {
            //     if (!ListOfSortedRow.PersistentUnitIncludedStats.Contains(f.name)) continue;
            //     f.SetFromRead(rw);
            // }
            //
            // foreach (var s in _unitPersistentList) { if (s.Address == ptrReference) return result; }
            //
            // TrackedUnit trackedUnitDraft;
            // trackedUnitDraft = new TrackedUnit(unitId, unitName) {Address = ptrClone};
            // int classId = ObjectExtension.To<int>(unitClass);
            // trackedUnitDraft.DisplayStats = new List<TrackedUnitStat>();
            // trackedUnitDraft.IsSelected = false;
            // trackedUnitDraft.UnitPlatoon = String.Format("{1} {0}",
            //     "ArmyBattalionNumber".GetFeature(feat).value.ToOrdinal(),
            //     SRMain.Instance.GetFormatData("UnitClass".GetFeature(feat), 1)[classId]
            // );
            //
            // foreach (var feature in ListOfSortedRow.PersistentUnitIncludedStats)
            // {
            //     var clonedStat = feature.GetFeature().Clone() as Feature;
            //     clonedStat = clonedStat.SetFromRead(rw);
            //     var staticPointer = clonedStat.name.GetPointer(rw);
            //     trackedUnitDraft.Stats.Add(staticPointer, clonedStat.value);
            //     
            //     
            //     var unitDetails = new TrackedUnitStat
            //     {
            //         StatToken = new CancellationTokenSource(),
            //         StatFreeze = !ListOfSortedRow.PersistentUnitExcludedStats.Contains(clonedStat.name) ? freeze : false,
            //         StatId = staticPointer,
            //         StatName = clonedStat.name,
            //         StatValue = clonedStat.value,
            //         StatFormattedValue = clonedStat.formattedValue,
            //         MetaFormat = clonedStat.format,
            //         MetaType = clonedStat.type
            //     };
            //     
            //     // Set master row stat
            //     if (clonedStat.name == "ArmyBattleGroup")
            //         trackedUnitDraft.UnitBattleGroup = unitDetails.StatValue;
            //     if (clonedStat.name == "ArmyKill")
            //         trackedUnitDraft.UnitKills = unitDetails.StatValue;
            //
            //     if (clonedStat.name == "ArmyPositionX")
            //         trackedUnitDraft.UnitPositionX = unitDetails.StatValue;
            //     if (clonedStat.name == "ArmyPositionY")
            //         trackedUnitDraft.UnitPositionY = unitDetails.StatValue;
            //     
            //     trackedUnitDraft.DisplayStats.Add(unitDetails);
            //     
            //     if(clonedStat.name == "UnitBattleGroup") 
            //         trackedUnitDraft.PropertyChanged += (sender, args) => 
            //         { 
            //             if(args.PropertyName == "UnitBattleGroup") 
            //                 trackedUnitDraft.UnitBattleGroup = unitDetails.StatValue; 
            //         };
            // }
            //
            // UnitPersistentList.Add(trackedUnitDraft);
            // trackedUnitDraft.GridId = UnitPersistentList.Count - 1;
            // feat = null;
            // return true;
            return false;
        }

        void OnCustomRowFilterForPersistent(object o, RowFilterEventArgs e)
        {
            
        }
        
        // Heal All units
        public void HealAllUnits()
        {
            if (UnitPersistentList.Count < 1) return; 
            foreach (var unit in UnitPersistentList)
            {
                if(unit.UnitStatus) 
                    unit.HealStats();
            }
        }
        
        public void InitializeGridViewForPersistenceMasterAndDetails(GridControl gc, GridView gvMaster, GridView gvDetail)
        {
            // var Events = SREvents.CreateInstance();
            
            RepositoryItemSpinEdit spin = new RepositoryItemSpinEdit();
            RepositoryItemSpinEdit spinPercentage = new RepositoryItemSpinEdit();
            spinPercentage.Name = "spinPercentage";
            RepositoryItemSpinEdit spinForMaster = new RepositoryItemSpinEdit();
            spin.Name = "spin";
            RepositoryItemProgressBar progressBarForMaster = new RepositoryItemProgressBar();
            progressBarForMaster.Name = "progressBarForMaster";
            RepositoryItemRatingControl rating = new RepositoryItemRatingControl();
            rating.Name = "rating";

            gc.RepositoryItems.Add(spin);
            gc.RepositoryItems.Add(rating);
            gc.RepositoryItems.Add(spinPercentage);
            gc.RepositoryItems.Add(progressBarForMaster);


            // progressBarForMaster.LookAndFeel.SetStyle3D();
            // progressBarForMaster.LookAndFeel.SetSkinStyle("Office 2010 Green");
            progressBarForMaster.Appearance.ProgressOptions.UseBackColor = true;
            progressBarForMaster.Appearance.ProgressOptions.UseBorderColor = true;
            progressBarForMaster.Appearance.BackColor2 = Color.LimeGreen;
            progressBarForMaster.Appearance.BackColor = Color.LimeGreen;
            progressBarForMaster.StartColor = Color.Brown;
            progressBarForMaster.EndColor = Color.LimeGreen;
            progressBarForMaster.PercentView = true;
            progressBarForMaster.ShowTitle = true;
            progressBarForMaster.Step = 1;
            // progressBarForMaster.ProgressViewStyle = ProgressViewStyle.Broken;
            progressBarForMaster.ProgressPadding = new Padding(0, 0, 0, 0);
            
            gvMaster.CustomRowCellEdit += (sender, args) =>
            {
                if (args.Column.FieldName != "UnitHealth") return;
                GridView view = sender as GridView;
                var unit = (view.GetRow(args.RowHandle) as TrackedUnit);
                
                if(view.RowCount < 1) return;
                if(unit == null) return;
                var actualHealth = unit.GetStatByName("ArmyActualStrength")
                    .StatValue;
                double d = 100;
                double.TryParse(actualHealth, out d);
                if (unit.UnitIsNaval)
                    d *= 200;
                progressBarForMaster.Minimum = 0;
                progressBarForMaster.Maximum = d > 0 ? Convert.ToInt32(d) : 0;
                args.RepositoryItem = progressBarForMaster;
            };

            // var unitActual = double.Parse(ArmyActualStrength.value == String.Empty ? 0.ToString() : ArmyActualStrength.value);

            
            // progressBarForMaster.EditValueChanged += (sender, args) =>
            // {
            //     var progressBar = sender as ProgressBarControl;
            //     if (progressBar == null) return;
            //     var progress = progressBar.EditValue;
            //     var unit = gvMaster.GetRow(.Tag) as PersistentUnit;
            //     if (unit == null) return;
            //     unit.UnitProgress = progress;
            // };

            string[] excludedColumns = new[]
            {
                "Address",
                "Stats",
                "GlobalStats",
                "UnitPositionX",
                "UnitPositionY",
                "RowId",
            };

            foreach (var col in excludedColumns)
            {
                if(gvMaster.Columns.ColumnByFieldName(col) != null)
                    gvMaster.Columns.ColumnByFieldName(col).Visible = false;
            }
            
            // GridLevelNode node = gc.LevelTree.Nodes["DisplayStats"];
            // node.LevelTemplate = gvDetail;
            // gvMaster.Columns["UnitId"].Visible = false;
            // gvMaster.Columns.ColumnByFieldName("Address").Visible = false;
            // gvMaster.Columns.ColumnByFieldName("Stats").Visible = false;
            // gvMaster.Columns.ColumnByFieldName("GlobalStats").Visible = false;
            // // gvMaster.Columns["DisplayStats"].Visible = true;
            // // gvMaster.Columns["RowId"].Visible = false;
            // gvMaster.Columns["UnitPositionX"].Visible = false;
            // gvMaster.Columns["UnitPositionY"].Visible = false;
            // gvMaster.Columns["colIsSelected"].Visible = false;

            if(gvMaster.Columns.ColumnByFieldName("RowId") != null)
             gvMaster.Columns.ColumnByFieldName("RowId").OptionsColumn.AllowEdit = false;
            if(gvMaster.Columns.ColumnByFieldName("UnitName") != null)
             gvMaster.Columns.ColumnByFieldName("UnitName").OptionsColumn.AllowEdit = false;
            if(gvMaster.Columns.ColumnByFieldName("UnitPlatoon") != null)
             gvMaster.Columns.ColumnByFieldName("UnitPlatoon").OptionsColumn.AllowEdit = false;
            if(gvMaster.Columns.ColumnByFieldName("UnitBattleGroup") != null)
             gvMaster.Columns.ColumnByFieldName("UnitBattleGroup").OptionsColumn.AllowEdit = true;
            if(gvMaster.Columns.ColumnByFieldName("UnitKills") != null)
             gvMaster.Columns.ColumnByFieldName("UnitKills").OptionsColumn.AllowEdit = false;

            gvMaster.OptionsView.AllowHtmlDrawHeaders = true;
            gvMaster.OptionsView.AllowGlyphSkinning = true;
            if(gvMaster.Columns.ColumnByFieldName("UnitKills") != null)
                gvMaster.Columns.ColumnByFieldName("UnitKills").Caption = "Kills";
            if(gvMaster.Columns.ColumnByFieldName("UnitPlatoon") != null)
                gvMaster.Columns.ColumnByFieldName("UnitPlatoon").Caption = "Platoon";
            if(gvMaster.Columns.ColumnByFieldName("UnitBattleGroup") != null)
                gvMaster.Columns.ColumnByFieldName("UnitBattleGroup").Caption = "Group";
            if(gvMaster.Columns.ColumnByFieldName("UnitStatus") != null)
                gvMaster.Columns.ColumnByFieldName("UnitStatus").Caption = "Alive";
            if(gvMaster.Columns.ColumnByFieldName("UnitHealth") != null)
                gvMaster.Columns.ColumnByFieldName("UnitHealth").Caption = "Health";
            // gvMaster.Columns["UnitPositionX"].Caption = "X";
            // gvMaster.Columns["UnitPositionY"].Caption = "Y";

            gvMaster.OptionsSelection.MultiSelect = true;
            gvMaster.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;

            // var manager = new BarManager();
            // var popUp = new PopupMenu();
            // var barBtn = new BarButtonItem();
            // barBtn.Caption = "Heal All";
            // popUp.AddItem(barBtn);
            // barBtn.ItemClick += (sender, args) =>
            // {
            //     if (UnitPersistentList.Count < 1) return;
            //     Parallel.ForEach(UnitPersistentList, unit =>
            //     { if(unit.UnitStatus) unit.HealStats(); });
            // };
            gvMaster.OptionsMenu.EnableColumnMenu = true;
            gvMaster.OptionsMenu.EnableFooterMenu = true;
            gvMaster.OptionsMenu.EnableGroupPanelMenu = true;
            gvMaster.OptionsMenu.EnableGroupRowMenu = true;
            
            gvMaster.MouseDown += (sender, args) =>
            {
                if (args.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(args.Location);
                    DataRow row = view.GetDataRow(hi.RowHandle);
                }
            };

            // ReSharper disable once ComplexConditionExpression
            gvMaster.PopupMenuShowing += (sender, args) =>
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
                    menu.Items.Add(CreateGridMenuItem("Show Position", view, rowHandle, MenuEventShowPosition));
                    var callingUnit = CreateGridMenuItem("Call Unit (WIP)", view, rowHandle, MenuEventCallUnit);
                    menu.Items.Add(callingUnit);
                    menu.Items.Add(CreateGridMenuItem("Remove Unit", view, rowHandle, MenuEventRemoveUnit));
                    menu.Items.Add(CreateGridMenuItem("Remove All Unit", view, rowHandle, MenuEventRemoveAllUnit));
                    callingUnit.Enabled = false;
                }
            };
            
            DXMenuItem CreateSetGroups(string text, GridView gv, int[] rowHandle) {
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
            
            // ClickEvent to set BattleGroups
            void  OnSetBattleGroup(object sender, EventArgs args)
            {
                DXMenuItem menuItem = sender as DXMenuItem;
                RowInfo rowInfo = menuItem.Tag as RowInfo;
                if(rowInfo != null) 
                    foreach (var row in rowInfo.RowHandles)
                    { 
                        // UnitPersistentList[row].SetUnitBattleGroup(rowInfo.OptionalData);
                        (rowInfo.View.GetRow(row) as TrackedUnit).SetUnitBattleGroup(rowInfo.OptionalData);
                        // var unit = UnitPersistentList[row];
                        // unit.SetUnitBattleGroup(i.ToString());
                        // if (o.IsRowSelected(i)) (o.GetRow(i) as PersistentUnit).SetUnitBattleGroup(args.Value.ToString());
                    }
            };   



            DXMenuItem CreateGridMenuItem(string text, GridView gv, int rowHandle, EventHandler evt) {
                DXMenuItem item = new DXMenuItem(text, evt);
                item.Tag = new RowInfo(gv, rowHandle);
                return item;
            }
            
            void MenuEventRemoveUnit(object sender, EventArgs e) {
                DXMenuItem item = sender as DXMenuItem;
                RowInfo info = item.Tag as RowInfo;
                if(info == null) return;
                RemoveSelectedPersistentUnit(info.View);
            }
            
            void MenuEventRemoveAllUnit(object sender, EventArgs e) {
                DXMenuItem item = sender as DXMenuItem;
                RowInfo info = item.Tag as RowInfo;
                if(info == null) return;
                RemoveAllPersistantUnit(info.View);
            }
            
            void MenuEventCallUnit(object sender, EventArgs e) {
                DXMenuItem item = sender as DXMenuItem;
                RowInfo info = item.Tag as RowInfo;
                if(info == null) return;
                (info.View.GetRow(info.RowHandle) as TrackedUnit).TeleportToMouse();
            }
            
            void MenuEventShowPosition(object sender, EventArgs e) {
                DXMenuItem item = sender as DXMenuItem;
                RowInfo info = item.Tag as RowInfo;
                if(info == null) return;
                (info.View.GetRow(info.RowHandle) as TrackedUnit).ShowPosition();
            }
            
            void MenuEventHealUnit(object sender, EventArgs e) {
                DXMenuItem item = sender as DXMenuItem;
                RowInfo info = item.Tag as RowInfo;
                if(info == null) return;
                (info.View.GetRow(info.RowHandle) as TrackedUnit).HealStats();
            }
            
            void MenuEventHealAllUnits(object sender, EventArgs e) {
                DXMenuItem item = sender as DXMenuItem;
                RowInfo info = item.Tag as RowInfo;
                if(info == null) return;
                if (info.View.RowCount < 1) return;
                for (int i = 0; i < UnitPersistentList.Count; i++)
                {
                    (info.View.GetRow(info.View.GetRowHandle(i)) as TrackedUnit).HealStats();
                }
            }

            // gvDetail.Level
            // gvMaster.BestFitColumns();
            // gvMaster.CustomRowFilter += OnCustomRowFilterForPersistent;
            gvMaster.OptionsDetail.ShowDetailTabs = false;
            // gvMaster.OptionsDetail.EnableDetailToolTip = false;

            // gvMaster.OptionsBehavior.EditorShowMode = EditorShowMode.Default;
            gvMaster.OptionsBehavior.EditorShowMode = EditorShowMode.MouseDownFocused;
            gvMaster.OptionsCustomization.AllowFilter = false;
            gvMaster.OptionsCustomization.AllowSort = false;
            gvMaster.OptionsCustomization.AllowColumnMoving = false;
            gvMaster.OptionsDetail.DetailMode = DetailMode.Embedded;
            
            gvDetail.OptionsCustomization.AllowFilter = false;
            gvDetail.OptionsCustomization.AllowSort = false;
            gvDetail.OptionsCustomization.AllowColumnMoving = false;
            gvDetail.OptionsView.ShowAutoFilterRow = false;
            gvDetail.OptionsView.ShowIndicator = false;
            gvDetail.OptionsBehavior.EditorShowMode = EditorShowMode.Default;
            // gvDetail.OptionsDetail.DetailMode = DetailMode.Embedded;
            gvDetail.OptionsView.ShowGroupedColumns = false;
            gvDetail.OptionsView.ShowGroupPanel = false;
            gvDetail.OptionsView.EnableAppearanceEvenRow = true;

            
            // Set BattleGroup Control
            spinForMaster.MinValue = 0;
            spinForMaster.MaxValue = 9;
            spinForMaster.ValidateOnEnterKey = true;
            spinForMaster.AllowMouseWheel = true;
            spin.EditValueChanged += (sender, args) => gvMaster.PostEditor();
            
            // Set Health Control

            if(gvMaster.Columns.ColumnByFieldName("UnitBattleGroup") != null) 
                gvMaster.Columns["UnitBattleGroup"].ColumnEdit = spinForMaster;
            if(gvMaster.Columns.ColumnByFieldName("UnitHealth") != null) 
                gvMaster.Columns["UnitHealth"].ColumnEdit = progressBarForMaster;  
            // gvMaster.EditingValueModified +=
            gvMaster.ValidatingEditor += OnGvMasterOnValidatingEditor;

            // TODO: Hardcoded, still a violation, acessing static, unknown shit from the outside
            // ReSharper disable once ComplexConditionExpression
            gvMaster.RowStyle += OnGvMasterOnRowStyle;
            
            gvDetail.PopulateColumns(new List<TrackedUnitStat>());
            gvDetail.Columns["StatId"].Visible = false;
            gvDetail.Columns["StatToken"].Visible = false;
            gvDetail.Columns["StatFormattedValue"].Visible = false;
            gvDetail.Columns["MetaFormat"].Visible = false;
            gvDetail.Columns["MetaType"].Visible = false;

            gvDetail.Columns["StatId"].OptionsColumn.AllowEdit = false;
            gvDetail.Columns["StatName"].OptionsColumn.AllowEdit = false;
            
            gvDetail.Columns["StatFreeze"].Caption = "Lock";
            gvDetail.Columns["StatName"].Caption = "Stat";
            gvDetail.Columns["StatValue"].Caption = "Value";
            InitCustomPersistentEditor(SRLoaderForm._srLoader.rw, gc, gvDetail, gvMaster);
            gvMaster.MasterRowExpanded += (sender, args) => {
                var control = sender as GridControl; 
                var view = sender as GridView;
                GridView detail = view.GetDetailView(args.RowHandle, args.RelationIndex) as GridView;
                // RepositoryItemSpinEdit spin = new RepositoryItemSpinEdit();
                // spin.Name = "spin";
                // RepositoryItemRatingControl rating = new RepositoryItemRatingControl();
                // rating.Name = "rating";

                if (view.IsMasterRow(view.FocusedRowHandle))
                { 
                    Instance.proxyMasterDetail(detail);
                    // gvDetail.PopulateColumns(_unitPersistentList[args.RowHandle].DisplayStats);
                    // control?.RepositoryItems.AddRange({spin, rating});
                    // InitCustomPersistentEditor(SRLoaderForm._srLoader.rw, gc, detail, view);
                    // GridView detail = view.GetDetailView(args.RowHandle, args.RelationIndex) as GridView;
                    // UnitStat mc = (view.DataSource as List<UnitStat>)[e.ListSourceRow] as UnitStat;
                    // detail.Columns["StatId"].Visible = false;
                    // detail.Columns["StatToken"].Visible = false;
                    // detail.Columns["StatFormattedValue"].Visible = false;
                    // detail.Columns["MetaFormat"].Visible = false;
                    // detail.Columns["MetaType"].Visible = false;
                    // detail.Columns["StatId"].OptionsColumn.AllowEdit = false;
                    // detail.Columns["StatName"].OptionsColumn.AllowEdit = false;
                    // detail.OptionsView.ShowAutoFilterRow = false;
                    // detail.OptionsView.ShowIndicator = false;
                    // var relationName = view.GetRelationName(args.RowHandle, args.RelationIndex);
                    // // if(relationName != null)
                    //     // XtraMessageBox.Show(relationName);
                    // GridView detail = view.GetDetailView(args.RowHandle, args.RelationIndex) as GridView;
                    // detail.CustomRowFilter += (o, eventArgs) =>
                    // {
                    //     var name = detail.GetRowCellValue(eventArgs.ListSourceRow, "StatName");
                    //     if(ListOfSortedRow.PersistentUnitExcludedStats.Contains(name))
                    //         eventArgs.Visible = false;
                    //     eventArgs.Handled = true;
                    // };
                    // int detailIndex = 0;
                    // view.SetMasterRowExpandedEx(view.FocusedRowHandle, detailIndex, true);
                    // ColumnView childView = (ColumnView)view.GetDetailView(view.FocusedRowHandle, detailIndex);
                    // if (childView != null)
                    // {
                    //     childView.ZoomView();
                    //     for (int i = 0; i < childView.DataRowCount; i++)
                    //     {
                    //         var name = childView.GetRowCellValue(i, "StatName");
                    //         if (name == "ArmyExperience")
                    //         {
                    //             e.Visible = false;
                    //             e.Handled = true;
                    //         }
                    //     }
                    // }
                }
            };
            // gvDetail.CustomRowFilter += OnCustomRowFilterForPersistent;
            // gvDetail.CustomRowFilter += OnCustomRowFilterForPersistent;
        }

        private void OnGvMasterOnRowStyle(object sender, RowStyleEventArgs args)
        {
            var view = sender as GridView;
            if (args.RowHandle >= 0)
            {
                TrackedUnit address = view.GetRow(args.RowHandle) as TrackedUnit;
                if (address == null) return;
                if (address.IsSelected)
                {
                    args.Appearance.BackColor = Color.DarkGreen;
                }
                else if (!address.UnitStatus)
                    if (view.Columns.ColumnByFieldName("UnitHealth") != null)
                        args.Appearance.BackColor = Color.FromArgb(49, 2, 8);
            }
        }

        private void StaticAddressCollection()
        {
            
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
        
        

        public void RefreshMasterTimer(GridView? gv, SRReadWrite rw)
        {
            if (gv == null) return;
            if (gv.DataRowCount < 1) return;
            for (int i = 0; i < gv.DataRowCount; i++)
            {
                for (int j = 0; j < UnitPersistentList.Count; j++)
                {
                    var ArmyCurrentStrength = UnitPersistentList[j].GetStatByName("ArmyCurrentStrength").Read();
                    var unitCurrent = double.Parse(ArmyCurrentStrength.StatValue == String.Empty ? 0.ToString() : ArmyCurrentStrength.StatValue);

                    // Check if unit is selected
                    if(gv.IsFocusedView) UnitPersistentList[j].IsSelected =
                        UnitPersistentList[j].Address
                            .Equals("ArmyCurrentStrength".GetPointer(rw));
                    
                    
                    UnitPersistentList[j].UnitKills = UnitPersistentList[j].GetStatByName("ArmyKill").StatValue;
                    UnitPersistentList[j].UnitBattleGroup = UnitPersistentList[j].GetUnitBattleGroup();
                    if (!UnitPersistentList[j].UnitIsNaval)
                        UnitPersistentList[j].UnitHealth = (int)(unitCurrent);
                    else
                        UnitPersistentList[j].UnitHealth = (int)(unitCurrent);
                    // UnitPersistentList[j].UnitIsNaval =
                    //     SRLoaderForm._srLoader.rw.SRIsNaval(UnitPersistentList[j].GetStatByName("UnitClass").StatValue.To<int>());

                    gv.RefreshRowCell(j, gv.Columns["UnitStatus"]);
                    gv.RefreshRowCell(j, gv.Columns["UnitHealth"]);
                    gv.RefreshRowCell(j, gv.Columns["UnitKill"]);
                    gv.RefreshRowCell(j, gv.Columns["UnitBattleGroup"]);
                }
                if(gv.IsVisible) if(gv.IsRowVisible(i) != RowVisibleState.Hidden) gv.RefreshRow(i);
            }
        }

        public void RefreshDetailTimer(GridView? gv, SRReadWrite rw)
        {
            if (gv == null) return;
            if (gv.State == GridState.Editing) return;
            for (int i = 0; i < UnitPersistentList.Count; i++)
            {
                for (int j = 0; j < UnitPersistentList[i].DisplayStats.Count; j++)
                {
                    // if unit is not freezed, then update the value from the SR
                    if (!UnitPersistentList[i].DisplayStats[j].StatFreeze)
                            UnitPersistentList[i].DisplayStats[j].Read();
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

        public void InitCustomHistoryUnitEditor(SRReadWrite rw, GridControl gc, GridView gv)
        {
            RepositoryItemSpinEdit spin = new RepositoryItemSpinEdit();
            gc.RepositoryItems.AddRange(new RepositoryItem[] { spin });

            gv.Columns["StatId"].Visible = false;
            gv.Columns["StatId"].OptionsColumn.AllowEdit = false;
            gv.Columns["StatName"].OptionsColumn.AllowEdit = false;

            // Set Spin Properties
            // spin.AllowMouseWheel = true;
            // spin.AllowNullInput = DefaultBoolean.False;
            // spin.ValidateOnEnterKey = true;

            void HistoryCustomRow(object sender, CustomRowCellEditEventArgs args)
            {
                // GridView gView = sender as GridView;
                // var statValue = (string) gView.GetListSourceRowCellValue(args.RowHandle, "StatValue");
                // var statAddress = (string) gView.GetListSourceRowCellValue(args.RowHandle, "StatId");

                // spin.Mask.EditMask = default;
                if (args.Column.FieldName != "StatValue") return;
                spin.IsFloatValue = true;
                args.RepositoryItem = spin;
            }

            spin.EditValueChanged += (sender, args) =>
            {
                gv.PostEditor();
            };
            gv.ValidatingEditor += (sender, args) =>
            {
                GridView gView = sender as GridView;
                if (gView.FocusedColumn == gView.Columns["UnitStats"])
                {
                    var address = gView.GetRowCellValue(gView.FocusedRowHandle, "StatAddress").ToString();
                    rw.WriteMemory(address, gView.GetRowCellValue(gView.FocusedRowHandle, "MetaType").ToString(), args.Value.ToString());
                    gv.PostEditor();
                }
            };

            gv.CustomRowCellEditForEditing += HistoryCustomRow;
        }

        public static GridView _editor = null;
        public GridView proxyMasterDetail(GridView editor)
        {
            if (_editor == null)
                _editor = editor;
            return _editor;
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
                        // // spin.Increment = 0.1m;
                        args.RepositoryItem = spin;
                        break;
                }
            }
        // Custom Editor for Persistent Unit, using DevExpress Grid.
        public void InitCustomPersistentEditor(SRReadWrite rw, GridControl gc, GridView gv, GridView parentView)
        {
            // RepositoryItemSpinEdit spin = new RepositoryItemSpinEdit();
            // RepositoryItemRatingControl rating = new RepositoryItemRatingControl();

            RepositoryItemSpinEdit spin = gc.RepositoryItems["spin"] as RepositoryItemSpinEdit;
            RepositoryItemRatingControl rating = gc.RepositoryItems["rating"] as RepositoryItemRatingControl;
            if (rw == null) throw new ArgumentNullException(nameof(rw));
            // gc.RepositoryItems.AddRange(new RepositoryItem[] {spin, rating});
            
            TrackedUnitStat GetUnitStat (int parentId, string statName)
            {
                foreach (TrackedUnitStat stat in UnitPersistentList[parentId].DisplayStats)
                {
                    if (statName != stat.StatName) return null;
                        return stat;
                }
                return null;
            }
            
            TrackedUnitStat _tempSpin;
            // Set Spin Properties
            spin.AllowMouseWheel = true;
            spin.AllowNullInput = DefaultBoolean.False;
            spin.ValidateOnEnterKey = true;
            void FormatEditValue(object o, ConvertEditValueEventArgs args)
            {
                // var spinEdit = o as SpinEdit;
                // if(spinEdit.Tag is null) return;
                // var pUnit = spinEdit.Tag.As<UnitStat>().Value;
                // if (!excludedTag.Contains(pUnit.StatName)) return;
                // if (spinEdit.EditValue is null) return;
                // if (spinEdit.Tag.ToString() != "ArmyEntrenchment") return; 
                //     string value = spinEdit.EditValue.ToString();
                // args.Value = NumericExtension.SafePercentage(value.As<decimal>().Value, 5);
                // args.Handled = true;
            }
            
            // spin.FormatEditValue += FormatEditValue;
            spin.EditValueChanged += (sender, args) =>
            {
                gv.PostEditor();
            };
            rating.EditValueChanged += (sdr, ee) =>
            {
                gv.PostEditor();
            };
            rating.ItemClick += (sdr, ee) =>
            {
                var o = sdr as RatingControl;
                // XtraMessageBox.Show(ee.Index.ToString());
                gv.PostEditor();
            };

            void PersistentCustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
            {
                GridView view = sender as GridView;
            
                // Guard if column is not stat value
                if (e.Column.FieldName != "StatValue") return;

                var unit = view.GetRow(view.GetRowHandle(e.ListSourceRowIndex)) as TrackedUnitStat;
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
                if (type == "string" || type == "byte") return;

                // if (realVal >= decimal.MaxValue) return;
                decimal formattedValue = realVal;
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
                        e.DisplayText = realVal.ToString("########0.00");
                        // e.DisplayText = (decimal.Parse(value).To<int>()).ToString("0");
                        break;
                }


            }
            
            // EDIT
            
            
            // To Show rating in master detail grid
            void PersistentValidatingEditor_Events(object sender, BaseContainerValidateEditorEventArgs args)
            {
                ColumnView gView = sender as ColumnView;
                GridColumn column = (args as EditFormValidateEditorEventArgs)?.Column ?? gView.FocusedColumn;
                gView.RefreshEditor(true);
                if (column.Name == "colStatFreeze")
                {
                    args.Valid = true; 
                    args.Value = args.Value.ToString() == "True" ? "True" : "False";
                    gv.PostEditor();
                    return;
                }
                if (column.Name != "colStatValue") return;
                var address = gView.GetRowCellValue(gView.FocusedRowHandle, "StatId").ToString();
                var name = gView.GetRowCellValue(gView.FocusedRowHandle, "StatName").ToString();
                var type = gView.GetRowCellValue(gView.FocusedRowHandle, "MetaType").ToString();
                var value = args.Value;
                
                if (rw.WriteMemory(address, type, value.ToString()))
                    args.Valid = true;
                gv.PostEditor();
                // if (name == "ArmyExperience")
                //     value = args.Value.To<decimal>().ToString("#######.0");
                // if(!gView.GetRowCellValue(gView.FocusedRowHandle, "StatFreeze").To<bool>())
            };
            
            gv.ValidatingEditor += PersistentValidatingEditor_Events;
            gv.CustomColumnDisplayText += PersistentCustomColumnDisplayText;
            gv.CustomRowCellEdit += PersistentCustomRow;
        }

        // Checks whether unit is marked as tracked/persistent or no
        public bool UnitIsPersistent(string address)
        {
            foreach (var unit in UnitPersistentList)
            {
                if (unit.Address.Equals(address))
                    return true;
            }

            return false;
        }
        
        // Based on ArmyCurrentStrength
        // TODO: v3 polish, deprecated, use TrackedUnits instead. remove this
        public int GetTrackedRowId(SRReadWrite rw)
        {
            // if (UnitPersistentList == null) return -1;
            // var address = "ArmyCurrentStrength".GetPointer(rw);
            // if(!UnitIsPersistent(address)) return -1;
            // var result = UnitPersistentList.SingleOrDefault(s => s.Address == address);
            // if (result != null)
            //     return result.RowId;
            // return -1;
            return -1;
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

        // Removing from Tracked unit, also unfreeze the freezed stats.
        public void RemovePersistantUnit(GridView view, string address)
        {
            RemoveSelectedPersistentUnit(view);
            // for (int i = 0; i < view.DataRowCount; i++)
            // {
            //     if (!view.IsRowSelected(i)) return;
            //     foreach (var stat in (view.GetRow(i) as PersistentUnit).DisplayStats)
            //     {
            //         if (SRTokenUnfreezer(stat))
            //             Debug.WriteLine($"Successfully unfreezed stat {stat.StatName} with {stat.StatValue}");
            //     }
            //     UnitPersistentList.RemoveAt(i);
            // }
            // view.BeginUpdate();
            // if (!UnitPersistentList.Any(s => s.Address == address)) return;
            // var unit = UnitPersistentList.SingleOrDefault(s => s.Address == address);
            // if (unit != null)
            // {
            //     foreach (var stat in unit.DisplayStats)
            //     {
            //         if(SRTokenUnfreezer(stat))
            //             Debug.WriteLine($"Successfully unfreezed stat {stat.StatName} with {stat.StatValue}");
            //         // rw.UnfreezeValue(stat.Key);
            //     }
            //     // UnitPersistentList.Remove(unit);
            // }
            // view.EndUpdate();
        }

        public void RemoveSelectedPersistentUnit(GridView view)
        {
            // view.BeginDataUpdate();
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
            // view.DeleteSelectedRows();
            // view.EndDataUpdate();
        }

        // The same as above but in loop thru the private list.
        public void RemoveAllPersistantUnit(GridView view)
        {
            view.BeginDataUpdate();
            foreach (var unit in UnitPersistentList)
            {
                foreach (var stat in unit.DisplayStats)
                {
                    SRTokenUnfreezer(stat);
                    // rw.UnfreezeValue(stat.Key);
                }
            }
            view.EndDataUpdate();
            
            for (int i = 0; i < view.RowCount;)
            {
                    view.DeleteRow(i);
            }
        }

        private static string GetSerializedObject(object objForSerialize, List<string> propertyForSerialization)
        {
            var customObject = new ExpandoObject() as IDictionary<string, Object>;
            Type myType = objForSerialize.GetType();

            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

            foreach (PropertyInfo prop in props)
            {
                foreach (var propForSer in propertyForSerialization)
                {
                    if (prop.Name == propForSer)
                    {
                        customObject.Add(prop.Name, prop.GetValue(objForSerialize, null));
                    }
                }
            }

            return JsonConvert.SerializeObject(customObject);
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
        public GridView View;
        public int RowHandle;
        public string[] OptionalDataCollection;
        public string OptionalData;
        public bool OptionalState;
        public int[] RowHandles;
        
    }

    public class MenuColumnInfo
    {
        public MenuColumnInfo(GridColumn column) {
            this.Column = column;               
        }
        public GridColumn Column;
    }
}