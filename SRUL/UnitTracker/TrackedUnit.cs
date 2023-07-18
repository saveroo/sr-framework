using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using DevExpress.XtraEditors;
using SRUL.Types;

namespace SRUL.UnitTracker;

public sealed class TrackedUnit : INotifyPropertyChanged, ITrackedUnit
{
    private static object sync = new object();
    private static int _globalCount;
    private string _unitPlatoon;
    private bool _unitStatus;
    private string _unitBattleGroup;
    private string _unitPositionX = "0";
    private string _unitPositionY = "0";
    private int _unitHealth;
    private bool _unitIsNaval = false;
    public int RowId { get; set; } = 0;
    public bool IsSelected { get; set; } = false;
    public int GridId { get; set; }
    public string UnitId { get; set; }

    public string UnitPlatoon
    {
        get => _unitPlatoon;
        set
        {
            if (value.Equals(_unitPlatoon)) return;
            _unitPlatoon = value;
            OnPropertyChanged(_unitBattleGroup);
        }
    }
    public string UnitName { get; set; }
    public string UnitBattleGroup
    {
        get
        {
            return _unitBattleGroup;
        }
        set
        {
            if (value.Equals(_unitBattleGroup)) return;
            _unitBattleGroup = value;
            OnPropertyChanged(_unitBattleGroup);
        }
    }
    public string UnitKills { get; set; }

    public int UnitHealth
    {
        get
        {
            // if (UnitIsNaval)
            //     return _unitHealth * 100;
            return _unitHealth;
        }
        set
        {
            if (value == _unitHealth) return; 
            _unitHealth = value;
        }
    }

    public bool UnitIsNaval
    {
        get => SRLoaderForm._srLoader.rw.SRIsNaval(GetStatByName("UnitClass").StatValue);
    }

    public string UnitPositionX
    {
        get => GetStatByName("ArmyPositionX").StatValue ?? _unitPositionX;
        set
        {
            // if (value.Equals(_unitPositionX)) return;
            _unitPositionX = value;
            SetStatByName("ArmyPositionX", _unitPositionX);
        }
    }

    public string UnitPositionY
    {
        get => GetStatByName("ArmyPositionY").StatValue ?? _unitPositionY;
        set
        { 
            // if (value.Equals(_unitPositionY)) return;
            _unitPositionY = value;
            SetStatByName("ArmyPositionY", _unitPositionY);
        }
    }
    public bool UnitStatus
    {
        get
        {
            return !(String.IsNullOrEmpty(GetStatByName("ArmyCurrentStrength").StatValue) || GetStatByName("ArmyCurrentStrength").StatValue == "0");
        }
    }

    public UIntPtr Address { get; set; }

    public Dictionary<string, string> Stats { get; set; } =
        new Dictionary<string, string>();
    public List<TrackedUnitStat> DisplayStats { get; set; }
    public IList<GlobalUnitStoreStruct> GlobalStats { get; set; }

    public void GoToLocation()
    {
        "MouseClickHexPosX".GetFeature().WriteTo(SRLoaderForm._srLoader.rw, UnitPositionX);
        "MouseClickHexPosY".GetFeature().WriteTo(SRLoaderForm._srLoader.rw, UnitPositionY);
    }
        
    public void ShowPosition()
    {
        if (UnitPositionX.Equals("0"))
        {
            XtraMessageBox.Show($"{UnitName} Can't get located");
            return;
        }
        "MeasurementMode".GetFeature().WriteTo(SRLoaderForm._srLoader.rw, "1");
        "MeasurementModeStartPositionX".GetFeature().SafeWrite(SRLoaderForm._srLoader.rw, UnitPositionX);
        "MeasurementModeStartPositionY".GetFeature().SafeWrite(SRLoaderForm._srLoader.rw, UnitPositionY);
    }

    public void TeleportToMouse()
    {
        GetStatByName("ArmyPositionX").Write("MouseClickHexPosX".GetFeature().Read(SRLoaderForm._srLoader.rw));
        GetStatByName("ArmyPositionY").Write("MouseClickHexPosY".GetFeature().Read(SRLoaderForm._srLoader.rw));
    }

    public bool CheckIfSelected()
    {
        if (Address == "ArmyCurrentStrength".GetPointerUIntPtr(SRLoaderForm._srLoader.rw))
        {
            IsSelected = true;
            return IsSelected;
        }
        IsSelected = false;
        return IsSelected;
    }
        
    // public IDictionary<string, string> DisplayStats { get; set; }
    // public ICollection<UnitStat> Stats { get; } =
    //      new Collection<UnitStat>();
        
    public TrackedUnitStat GetStatByName(string statName)
    {
        foreach (var stat in DisplayStats)
        {
            if (stat.StatName == statName)
                return stat;
        }
        return null;
    }
        
    public T GetStatValueByName<T>(string statName) where T : struct
    {
        var stat = GetStatByName(statName);
        if (stat == null)
            return default;
        return (T)Convert.ChangeType(stat.StatValue, typeof(T));
    }

    public bool SetStatByName(string statName, string value)
    {
        foreach (var stat in DisplayStats)
        {
            if (stat.StatName != statName) continue;
            if (SRLoaderForm._srLoader.rw.Write(stat.MetaType, stat.StatId, value))
            {
                stat.StatValue = value;
                return true;
            }
            return false;
        }
        return false;
    }

    public void HealStats()
    {
        if (!UnitStatus) return;
        SetStatByName("ArmyCurrentStrength", GetStatByName("ArmyActualStrength").StatValue);
        SetStatByName("ArmySupplies", (
            GetStatValueByName<float>("ArmyActualStrength") 
            * GetStatValueByName<float>("UnitSuppliesCapacity")).ToString(CultureInfo.InvariantCulture));         
        SetStatByName("ArmyFuel", (
            GetStatValueByName<float>("ArmyActualStrength") 
            * GetStatValueByName<float>("UnitFuelCapacity")).ToString(CultureInfo.InvariantCulture));
        SetStatByName("ArmyMorale", "1");
        SetStatByName("ArmyEfficiency", "2");
            
    }
    
    public void ThreeStarStats()
    {
        if (!UnitStatus) return;
        SetStatByName("ArmyExperience", "3");
    }

    public string GetUnitBattleGroup()
    {
        return GetStatByName("ArmyBattleGroup").StatValue;
    }
    public bool SetUnitBattleGroup(string battleGroup)
    {
        if (string.IsNullOrEmpty(battleGroup)) return false;
        if (_unitBattleGroup == battleGroup) return false;
        foreach (var stat in DisplayStats)
        { 
            if(stat.StatName != "ArmyBattleGroup") continue;
            stat.StatValue = battleGroup;
            return SRLoaderForm._srLoader.rw.Write(stat.MetaType, stat.StatId, battleGroup);
        }
        return false;
    } 
        
    // freeze UnitStats in DisplayStats
    public void FreezeStatByName(string statName)
    {
        foreach (var stat in DisplayStats)
        {
            if (stat.StatName.Equals(statName, StringComparison.InvariantCultureIgnoreCase))
            {
                stat.StatFreeze = true;
                break;
            }
        }
    }

    public void FreezeStats(bool flag) {
        foreach (var stat in DisplayStats) {
            if(ListOfSortedRow.PersistentUnitExcludedStats.Contains(stat.StatName)) continue;
            if(stat.StatName.Equals("UnitSuppliesCapacity")) continue;
            if(stat.StatName.Equals("UnitFuelCapacity")) continue;
            if(stat.StatFreeze != flag) 
                stat.StatFreeze = flag;
        }
    }

    public void UnFreezeStatByName(string statName)
    {
        // Unfreeze stat by name
        foreach (var stat in DisplayStats)
        {
            if (stat.StatName.Equals(statName, StringComparison.InvariantCultureIgnoreCase))
            {
                stat.StatFreeze = false;
                break;
            }
        }
    }

    public TrackedUnit(string unitId, string unitName)
    {
        UnitId = unitId;
        UnitName = unitName;
        lock (sync)
        {
            RowId = ++_globalCount;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [Annotations.NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}