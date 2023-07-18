using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SRUL.UnitTracker;

public interface ITrackedUnit
{
    int RowId { get; set; }
    bool IsSelected { get; set; }
    int GridId { get; set; }
    string UnitId { get; set; }
    string UnitPlatoon { get; set; }
    string UnitName { get; set; }
    string UnitBattleGroup { get; set; }
    string UnitKills { get; set; }
    int UnitHealth { get; set; }
    bool UnitIsNaval { get; }
    string UnitPositionX { get; set; }
    string UnitPositionY { get; set; }
    bool UnitStatus { get; }
    UIntPtr Address { get; set; }
    Dictionary<string, string> Stats { get; set; }
    List<TrackedUnitStat> DisplayStats { get; set; }
    IList<GlobalUnitStoreStruct> GlobalStats { get; set; }
    void GoToLocation();
    void ShowPosition();
    void TeleportToMouse();
    bool CheckIfSelected();
    TrackedUnitStat GetStatByName(string statName);
    T GetStatValueByName<T>(string statName) where T : struct;
    bool SetStatByName(string statName, string value);
    void HealStats();
    string GetUnitBattleGroup();
    bool SetUnitBattleGroup(string battleGroup);
    void FreezeStatByName(string statName);
    void UnFreezeStatByName(string statName);
}