using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Data.Extensions;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using SRUL.Types;

namespace SRUL
{
    public struct Unit
    {
        private string _unitAddress;
        private string _unitId;
        private string _unitName;
        private CancellationTokenSource _cts;
        public Dictionary<string, string> ModifiedStats { get; set; }
        private IList<Feature> _unitStats;
        public Unit(string unitAddress, string unitId, string unitName, IList<Feature> unitStats, CancellationTokenSource cts)
        {
            _cts = cts;
            _unitAddress = unitAddress;
            _unitId = unitId;
            _unitName = unitName;
            _unitStats = unitStats;
            ModifiedStats = new Dictionary<string, string>();
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

    public class UnitStat
    {
        // public string ArmyCurrentStrength;
        // public string ArmyActualStrength;
        // public string ArmyFuel;
        // public string ArmySupply;
        // public string ArmyMorale;
        // public string ArmyExperience;
        // public string ArmyEfficiency;
        // public string ArmyEntrenchment;
        public string StatId { get; set; }
        public string StatName { get; set; }
        public string StatValue { get; set; }
    }
    public class PersistentUnit
    {
        private static object sync = new object();
        private static int _globalCount;
        public int RowId { get; set; } = 0;
        public string UnitId { get; set; }
        public string UnitName { get; set; }
        public string Address { get; set; }

        public Dictionary<string, string> Stats { get; set; } =
            new Dictionary<string, string>();
        public List<UnitStat> DisplayStats { get; set; }
        // public IDictionary<string, string> DisplayStats { get; set; }
        // public ICollection<UnitStat> Stats { get; } =
        //      new Collection<UnitStat>();

        public PersistentUnit(string unitId, string unitName)
        {
            UnitId = unitId;
            UnitName = unitName;
            lock (sync)
            {
                RowId = ++_globalCount;
            }
        }

    }
    public class SRMemento
    {
        private IList<Unit> _unitHistoryList;
        // private IList<Unit> _unitPersistentList;

        private IList<PersistentUnit> _unitPersistentList { get; set; }
        // private IList<Unit> _unitList;
        private static readonly Lazy<SRMemento> _instance = new Lazy<SRMemento>(() => new SRMemento());
        public SRMemento()
        {
          _unitHistoryList = new List<Unit>();   
          _unitPersistentList = new List<PersistentUnit>();   
        }

        public IList<Unit> UnitHistoryList
        {
            get { return _unitHistoryList; }
        }
        public IList<PersistentUnit> UnitPersistentList
        {
            get { return _unitPersistentList; }
        }
        public static SRMemento Instance
        {
            get { return _instance.Value; }
        }

        // TODO: Battle with Heap allocation on cloning, not coupled by calling SRMain Instance
        // Should be called after WarfareTable is edited
        public bool SaveToHistory(IList<Feature> feats, SRReadWrite rw)
        {
            var unitName = feats.GetFeature("UnitName").value;
            if (_unitHistoryList.Any(s => s.UnitName == unitName)) return false; // Unitname Guard
            
            var ptrClone = "UnitWeight".GetPointer(rw);
            var ptrReference = "UnitWeight".GetPointer(rw);
            
            if (ptrClone.Length < 3 || ptrReference.Length < 3) return false; // Pointer Guard
            
            var unitId = "UnitID".GetFeature().value;
            if (unitId == "65535") return false; // ID Guard
            
            // Create New Copied Unit
            var feat = feats.Clone();
            var newUnit = new Unit(
                ptrClone, 
                "UnitID".GetFeature(feat).value, 
                "UnitName".GetFeature(feat).value, feat,
                new CancellationTokenSource()
                );
            // Add to the list
            _unitHistoryList.Add(newUnit);
            return true;
        }

        public void AddModifiedStat(SRReadWrite rw, string statName, string statValue)
        {
            if (_unitHistoryList != null)
            {
                if (_unitHistoryList.Any(s => s.UnitAddress == "UnitWeight".GetPointer(rw)))
                {
                    var history = _unitHistoryList.First(s => s.UnitName == "UnitName".GetFeature().value);
                    if(history.ModifiedStats == null)
                        history.ModifiedStats = new Dictionary<string, string>();
                    history.ModifiedStats[statName] = statValue;
                }
            }
        }

        public void RestoreToOriginal(string unitAddress, IList<Feature> feats, SRReadWrite rw)
        {
            var selectedPointer = unitAddress;
            if (!_unitHistoryList.Any(s => s.UnitAddress == selectedPointer)) return;
            var original = _unitHistoryList.First(s => s.UnitAddress == selectedPointer);
            foreach (var f in original.UnitStats)
            {
                if(original.ModifiedStats.ContainsKey(f.name)) 
                    rw.WriteMemory(SRMain.Instance.pointerStore(f.name), f.type, f.value);
            }
        }

        public bool MakePersistent(SRReadWrite rw, IList<Feature> feats)
        {
            var result = false;
            var feat = feats.Clone(); // Clone feature
            var ptrClone = "ArmyCurrentStrength".GetPointer(rw).Clone().ToString();
            var ptrReference = "ArmyCurrentStrength".GetPointer(rw);
            if (ptrClone.Length < 3 || ptrReference.Length < 3) return result;

            var unitId = "UnitID".GetFeature(feat).value;
            if (unitId == "65535") return result; // ID guard

            var unitName = feat.Single(s => s.name == "UnitName").value;
            
            var cts = new CancellationTokenSource();
            // var newUnit = new Unit(ptrClone, unitId, unitName, feat, cts);
            // _unitPersistentList.Add(newUnit);
            // if (_unitPersistentList.Count  0) return;
            if (_unitPersistentList.Any(s => s.Address == ptrReference)) return result;

            var aCurrentStrength = "ArmyCurrentStrength".GetFeature().Copy();
            var aActualStrength = "ArmyActualStrength".GetFeature().Copy();
            var aFuel = "ArmyFuel".GetFeature().Copy();
            var aSupply = "ArmySupply".GetFeature().Copy();
            var aMorale = "ArmyMorale".GetFeature().Copy();
            var aExperience = "ArmyExperience".GetFeature().Copy();
            var aEfficiency = "ArmyEfficiency".GetFeature().Copy();

            var pCurrentStrength = aCurrentStrength.name.GetPointer(rw);
            var pActualStrength = aActualStrength.name.GetPointer(rw);
            var pFuel = aFuel.name.GetPointer(rw);
            var pSupply = aSupply.name.GetPointer(rw);
            var pMorale = aMorale.name.GetPointer(rw);
            var pExperience = aExperience.name.GetPointer(rw);
            var pEfficiency = aEfficiency.name.GetPointer(rw);
            
            // Add Persistent Unit
            var persistentUnit = new PersistentUnit(unitId, unitName) {Address = ptrClone};
            persistentUnit.Stats.Add(pCurrentStrength, aCurrentStrength.value);
            persistentUnit.Stats.Add(pActualStrength, aActualStrength.value);
            persistentUnit.Stats.Add(pFuel, aFuel.value);
            persistentUnit.Stats.Add(pSupply, aSupply.value);
            persistentUnit.Stats.Add(pMorale, aMorale.value);
            persistentUnit.Stats.Add(pExperience, aExperience.value);
            persistentUnit.Stats.Add(pEfficiency, aEfficiency.value);
            
            var uCurrentStrength = new UnitStat() {StatId = pCurrentStrength, StatName = aCurrentStrength.name, StatValue = aCurrentStrength.value};
            var uActualStrength = new UnitStat()
                {StatId = pActualStrength, StatName = aActualStrength.name, StatValue = aActualStrength.value};
            var uFuel = new UnitStat() {StatId = pFuel, StatName = aFuel.name, StatValue = aFuel.value};
            var uSupply = new UnitStat() {StatId = pSupply, StatName = aSupply.name, StatValue = aSupply.value};
            var uMorale = new UnitStat() {StatId = pMorale, StatName = aMorale.name, StatValue = aMorale.value};
            var uExperience = new UnitStat()
                {StatId = pExperience, StatName = aExperience.name, StatValue = aExperience.value};
            var uEfficiency = new UnitStat()
                {StatId = pEfficiency, StatName = aEfficiency.name, StatValue = aEfficiency.value};

            persistentUnit.DisplayStats = new List<UnitStat>
            {
                uCurrentStrength,
                uActualStrength,
                uFuel,
                uSupply,
                uMorale,
                uExperience,
                uEfficiency,
            };
            _unitPersistentList.Add(persistentUnit);
            
            // _unitPersistentList.Where()

            rw.FreezeValue(pCurrentStrength, "float", uCurrentStrength.StatValue);
            rw.FreezeValue(pActualStrength, "float", uActualStrength.StatValue);
            rw.FreezeValue(pFuel, "float", uFuel.StatValue);
            rw.FreezeValue(pSupply, "float", uSupply.StatValue);
            rw.FreezeValue(pMorale, "float", uMorale.StatValue);
            rw.FreezeValue(pExperience, "float", uExperience.StatValue);
            rw.FreezeValue(pEfficiency, "float", uEfficiency.StatValue);
            
            return true;
            // foreach (var p in persistentUnit.Stats)
            // {
            //     
            // }
            // ThreadFreeze(cts, aCurrentStrength, rw);
            // ThreadFreeze(cts, aActualStrength, rw);
            // ThreadFreeze(cts, aFuel, rw);
            // ThreadFreeze(cts, aSupply, rw);
            // ThreadFreeze(cts, aMorale, rw);
            // ThreadFreeze(cts, aExperience, rw);
            // ThreadFreeze(cts, aEfficiency, rw);

            // foreach (var p in persistentUnit.stats)
            // {
            //     Task.Factory.StartNew(() =>
            //     {
            //         while (!cts.Token.IsCancellationRequested)
            //         {
            //             
            //         }
            //
            //     }, cts.Token);
            // }
        }

        public void InitCustomPersitentEditor(SRReadWrite rw, GridControl gc, GridView gv)
        {
            RepositoryItemSpinEdit spin = new RepositoryItemSpinEdit();
            gc.RepositoryItems.AddRange(new RepositoryItem[] {spin});

            gv.Columns["StatId"].Visible = false;
            gv.Columns["StatId"].OptionsColumn.AllowEdit = false;
            gv.Columns["StatName"].OptionsColumn.AllowEdit = false;

            // Set Spin Properties
            spin.AllowMouseWheel = true;
            spin.AllowNullInput = DefaultBoolean.False;
            spin.ValidateOnEnterKey = true;
            
            void PersistentCustomRow(object sender, CustomRowCellEditEventArgs args)
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
                if (gView.FocusedColumn == gView.Columns["StatValue"])
                { 
                    var address = gView.GetRowCellValue(gView.FocusedRowHandle, "StatId").ToString(); 
                    rw.FreezeValue(address, "float", args.Value.ToString());
                    gv.PostEditor();
                }

            };

            gv.CustomRowCellEditForEditing += PersistentCustomRow;
        }

        public bool UnitIsPersistent(string address)
        {
            return _unitPersistentList.Any(s => s.Address == address);
        }
        
        public int GetTrackedRowId(SRReadWrite rw)
        {
            if (_unitPersistentList == null) return -1;
            var address = "ArmyCurrentStrength".GetPointer(rw);
            if(!UnitIsPersistent(address)) return -1;
            var result = _unitPersistentList.Single(s => s.Address == address).RowId;
            if (result != null)
                return result;
            return -1;
        }

        public void RemovePersistantUnit(SRReadWrite rw, string address)
        {
            if (!_unitPersistentList.Any(s => s.Address == address)) return;
            rw.UnfreezeValue(address); 
            _unitPersistentList.Remove(_unitPersistentList.First(s => s.Address == address));
        }

        public void RemoveAllPersistantUnit(SRReadWrite rw)
        {
            foreach (var unit in _unitPersistentList)
            {
                rw.UnfreezeValue(unit.Address);
            }
            _unitPersistentList.Clear();
            // if (_unitPersistentList.Any(s => s.Address != address)) return;
            // _unitPersistentList.Clear();
            // rw.UnfreezeValue(address); 
            // _unitPersistentList.Remove(_unitPersistentList.First(s => s.Address == address));
        }
        public void ThreadFreeze(CancellationTokenSource cts, Feature feat, SRReadWrite rw)
        {
            Task.Factory.StartNew(() =>
                {
                    while (!cts.Token.IsCancellationRequested)
                    {
                        feat.WriteIntoMemory(rw);
                        Thread.Sleep(25);
                    }
                },
                cts.Token);
        }
    }
    
    // The Memento interface provides a way to retrieve the memento's metadata,
    // such as creation date or name. However, it doesn't expose the
    // Originator's state.
    public interface IMemento
    {
        string GetName();

        string GetState();

        DateTime GetDate();
    }
    
    // The Concrete Memento contains the infrastructure for storing the
    // Originator's state.
    class ConcreteMemento : IMemento
    {
        private string _state;

        private DateTime _date;

        public ConcreteMemento(string state)
        {
            this._state = state;
            this._date = DateTime.Now;
        }

        // The Originator uses this method when restoring its state.
        public string GetState()
        {
            return this._state;
        }
        
        // The rest of the methods are used by the Caretaker to display
        // metadata.
        public string GetName()
        {
            return $"{this._date} / ({this._state.Substring(0, 9)})...";
        }

        public DateTime GetDate()
        {
            return this._date;
        }
    }
    
    // The Caretaker doesn't depend on the Concrete Memento class. Therefore, it
    // doesn't have access to the originator's state, stored inside the memento.
    // It works with all mementos via the base Memento interface.
    class Caretaker
    {
        private List<IMemento> _mementos = new List<IMemento>();

        private Originator _originator = null;

        public Caretaker(Originator originator)
        {
            this._originator = originator;
        }

        public void Backup()
        {
            Console.WriteLine("\nCaretaker: Saving Originator's state...");
            this._mementos.Add(this._originator.Save());
        }

        public void Undo()
        {
            if (this._mementos.Count == 0)
            {
                return;
            }

            var memento = this._mementos.Last();
            this._mementos.Remove(memento);

            Console.WriteLine("Caretaker: Restoring state to: " + memento.GetName());

            try
            {
                this._originator.Restore(memento);
            }
            catch (Exception)
            {
                this.Undo();
            }
        }

        public void ShowHistory()
        {
            Console.WriteLine("Caretaker: Here's the list of mementos:");

            foreach (var memento in this._mementos)
            {
                Console.WriteLine(memento.GetName());
            }
        }
    }

    public class Originator
    {
        private string _state;

        public Originator(string state)
        {
            this._state = state;
            Console.WriteLine("Originator: initial state > " + state);
        }
        
        // The Originator's business logic may affect its internal state.
        // Therefore, the client should backup the state before launching
        // methods of the business logic via the save() method.
        public void DoSomething()
        {
            Console.WriteLine("Originator: waa");
            this._state = "test";
        }
        
        public IMemento Save()
        {
            return new ConcreteMemento(this._state);
        }
        
        public void Restore(IMemento memento)
        {
            if (!(memento is ConcreteMemento))
            {
                throw new Exception("Unknown memento class " + memento.ToString());
            }

            this._state = memento.GetState();
            Console.Write($"Originator: My state has changed to: {_state}");
        }
    }
}