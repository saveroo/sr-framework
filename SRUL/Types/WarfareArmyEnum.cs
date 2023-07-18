using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using NUnit.Framework;
using SmartAssembly.Attributes;

namespace SRUL.Types
{
    public enum WarfareArmyEnum
    {
        Heal,
        Rambo,
        GasSupply2x,
        GasSupply4x,
        Current,
        Actual,
        Fuel,
        Supply,
        Experience,
        Morale,
        Str2x,
        Str4x,
        Str6x,
        EnemySOON,
    }

    public enum HandmadeEnums
    {
        Heal,
        Rambo,
        GasSupply2x,
        GasSupply4x,
        Current,
        Actual,
        Fuel,
        Supply,
        Experience,
        Morale,
        Str2x,
        Str4x,
        Str6x,
        EnemySOON,
    }

    public class HandmadeFeature
    {

        public HandmadeEnums enums { get; set; }
        public string name { get; set; }
        public bool freeze { get; set; }
        public int value { get; set; }
        public string description { get; set; }
        public HandmadeFeature reference { get; set; }

        public HandmadeFeature()
        {
            
        }
        public HandmadeFeature(HandmadeEnums id, string name, bool freezeState, string description)
        {
            this.enums = id;
            this.name = Enum.GetName(typeof(HandmadeEnums), id);
            this.value = (int)id;
            this.freeze = freezeState;
            this.description = description;
            this.reference = this;
        }
        //
        // public HandmadeFeature(HandmadeEnums id, bool freezeState, string desc)
        // {
        //     // this.enums = id;
        //     // name = Enum.GetName(typeof(HandmadeEnums), id);
        //     // value = (int)id;
        //     // freeze = freezeState;
        //     // description = "";
        // }

        // public void Register(HandmadeEnums id, bool freezeState)
        // {
        //     this.id = id;
        //     this.name = nameof(id);
        //     this.value = (int)id;
        //     this.freeze = freezeState;
        // }
    }

    public class HandmadeFeatures
    {

        public List<HandmadeFeature>? Lists { get; set; }

        private static readonly Lazy<HandmadeFeatures> _instance =
            new Lazy<HandmadeFeatures>(() => new HandmadeFeatures());

        public static HandmadeFeatures Instance
        {
            get
            {
                if( _instance.Value.Lists == null)
                {
                    _instance.Value.Lists = new List<HandmadeFeature>
                    {
                        Capacity = 10
                    };
                    return _instance.Value;
                }
                
                return _instance.Value;
                
            }
        }

        public HandmadeFeature GetById(HandmadeEnums id)
        {
            if (Lists != null) return Lists[(int)id];
            return new HandmadeFeature();
        }

        public void Add(HandmadeFeature feature)
        {
            if (Lists != null) Lists.Add(feature);
        }

        public BindingSource BindingSource()
        {
            if (Lists == null)
                Lists = new List<HandmadeFeature>();
            var handmadeFeatureBindingList = new BindingList<HandmadeFeature>(HandmadeFeatures.Instance.Lists);
            return new BindingSource(handmadeFeatureBindingList, null);   
        }
        
        public void SetHandmadeState(HandmadeEnums which,bool cond)
        {
            foreach (var item in Lists)
            {
                if(item.enums == which) 
                    item.freeze = cond;
            }
        }

        public void Populate()
        {
            if(Instance.Lists.Count > 0) return;
            Instance.Add(new HandmadeFeature(HandmadeEnums.Heal, "", false, "")); 
            Instance.Add(new HandmadeFeature(HandmadeEnums.Rambo,"", false, ""));
            Instance.Add(new HandmadeFeature(HandmadeEnums.Str2x, "",  false, ""));
            Instance.Add(new HandmadeFeature(HandmadeEnums.Str4x, "", false, ""));
            Instance.Add(new HandmadeFeature(HandmadeEnums.GasSupply2x, "", false, ""));
            Instance.Add(new HandmadeFeature(HandmadeEnums.GasSupply4x, "", false, ""));
        }
    }

    public enum av
    {
        ArmyActiveStaff,
        ArmyReserve,
        ArmyActualStrength,
        ArmyCurrentStrength,
        ArmySupply,
        ArmyFuel,
        ArmyEfficiency,
        ArmyEntrenchment,
        ArmyExperience,
        ArmyMorale,
        UnitSuppliesCapacity,
        UnitFuelCapacity,
        UnitClass,
        UnitMovementType,
        UnitTargetType,
        HoverArmyActualStrength,
        HoverArmyCurrentStrength,
        HoverArmySupply,
        HoverArmyFuel,
        HoverArmyEfficiency,
        HoverArmyExperience,
        HoverArmyMorale,
    }

    public static class WarfareArrayUtils
    {
        public static string[] ArmyFeatureList = 
        {
            "ArmyActiveStaff",
            "ArmyReserve",
            "ArmyActualStrength",
            "ArmyCurrentStrength",
            "ArmySupply",
            "ArmyFuel",
            "ArmyEfficiency",
            "ArmyExperience",
            "ArmyEntrenchment",
            "ArmyMorale",
            "UnitSuppliesCapacity",
            "UnitFuelCapacity",
            "UnitClass",
            "UnitMovementType",
            "UnitTargetType",
            "HoverArmyActualStrength",
            "HoverArmyCurrentStrength",
            "HoverArmySupply",
            "HoverArmyFuel",
            "HoverArmyEfficiency",
            "HoverArmyExperience",
            "HoverArmyEntrenchment",
            "HoverArmyMorale",
        };
        public static string[] HoverArmyHaystack = 
        {
            "ArmyActualStrength",
            "ArmyCurrentStrength",
            "ArmySupply",
            "ArmyFuel",
            "ArmyEfficiency",
            "ArmyExperience",
            "ArmyMorale",
            "UnitSuppliesCapacity",
            "UnitFuelCapacity",
        };
    }
}