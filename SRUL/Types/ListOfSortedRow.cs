using System.Collections.Generic;
using SmartAssembly.Attributes;

namespace SRUL.Types
{
    [DoNotPrune]
    static class ListOfSortedRow
    {
        public static string[] warfareExcludedInDataView = {
            "ArmyActiveStaff",
            "ArmyReserve",
            "ArmyCurrentStrength",
            "ArmyActualStrength",
            "ArmyExperience",
            "ArmyMorale",
            "ArmySupply",
            "ArmyGas",
            "ArmyEfficiency",
            "UnitClass",
            "UnitMovementType",
            "UnitTargetType",
            "UnitRefittedDesign",
            "UnitReplacedDesign",
            "UnitReserve",
            "UnitDeployed",
            "UnitBattleGroup",
            "UnitSelected"
        };
        [DoNotPrune]
        public static string[] WarfareIncludedFeatureList = new string[] {};
        [DoNotPrune]
        public static string[] CountryIncludedFeatureList = new string[] {};
        [DoNotPrune]
        public static string[] FacilityIncludedFeatureList = new string[] {};
        [DoNotPrune]
        public static string[] ResourcesIncludedFeatureList = new string[] {};
        [DoNotPrune]
        public static string[] SocialSpendingList = new string[] {};
        [DoNotPrune]
        public static string[] SRGridIncludedFeatures = new string[] {};
        
        [DoNotPrune]
        public static string[] SRAllFeaturesName = new string[] {};
        
        [DoNotPrune]
        public static string[] WarfareExcludedFeatureList = new string[] {};
        [DoNotPrune]
        public static string[] CountryExcludedFeatureList = new string[] {};
        [DoNotPrune]
        public static string[] FacilityExcludedFeatureList = new string[] {};
        [DoNotPrune]
        public static string[] ResourcesExcludedFeatureList = new string[] {};
        [DoNotPrune]
        public static string[] SRGridExcludedFeatures = new string[] {};

        [DoNotPrune]
        public static string[] SRGridCategories = new string[] {};
        [DoNotPrune]
        public static string[] SRGridAllSubCategories = new string[] {};


        // public static string[] ResourcesIncludedFeatureList = new string[] {};
        // public static string[] ResourcesIncludedFeatureList = new string[] {};
        // public static string[] ResourcesIncludedFeatureList = new string[] {};

        public static readonly string[] PersistentUnitIncludedStats = new string[]
        {
            "ArmyCurrentStrength",
            "ArmyActualStrength",
            // "UnitID", 
            // "UnitName", 
            "ArmyFuel",
            "UnitFuelCapacity",
            "ArmySupply",
            "UnitSuppliesCapacity",
            "ArmyMorale",
            "ArmyExperience",
            "ArmyEfficiency",
            "ArmyEntrenchment",
            "ArmyBattalionNumber",
            "ArmyBattleGroup",
            "ArmyKill",
            "ArmyPositionX",
            "ArmyPositionY",
            "UnitClass",
        };

        public static readonly string[] PersistentUnitExcludedStats = new string[]{ 
            "ArmyBattleGroup", 
            "ArmyKill", 
            "ArmyBattalionNumber",    
            "UnitID",
            "ArmyPositionX",
            "ArmyPositionY",
            "UnitName",
            "UnitClass"
        };
        
        // For right click
        public static string[] ResourceSubcategoryMemberList = new string[]
        {
            "Stock",
            "Demand",
            "ActualUse",
            "Production",
            "ProductionCapacity",
            "Cost",
            "Price",
            "Margin",
            "BaseCost",
            "FullCost",
            "CityProduction",
            "NodeProduction",
            "MaxDemand",
            "MinDemand",
        };
        
        public static string[] ResourceSubcategoryMemberListDisplayName = new string[]
        {
            "Stock",
            "Demands",
            "Actual Use",
            "Production",
            "Production Capacity",
            "Production Cost",
            "Market Price",
            "Margin",
            "Base Cost",
            "Full Cost",
            "City Production",
            "Node Production",
            "Max Demand",
            "Min Demand",
        };
        
        public static Dictionary<string, bool> resourceRowsVisibleState { get; set; } = new Dictionary<string, bool>();

        public static string[] ResourceIconNames = new string[]
        {
            "Agriculture",
            "Rubber",
            "Timber",
            "Petroleum",
            "Coal",
            "Uranium",
            "Metal Ore",
            "Electric Power",
            "Consumer Goods",
            "Industry Goods",
            "Military Goods",
        };
            
        public static string[] ResourcesNameFieldName = new string[]
        {
            "Agriculture",
            "Rubber",
            "Timber",
            "Petroleum",
            "Coal",
            "Uranium",
            "MetalOre",
            "ElectricPower",
            "ConsumerGoods",
            "IndustryGoods",
            "MilitaryGoods",
        };

        public static string[] rowNameText = new string[]
        {
            "StockAgriculture",
            "StockRubber",
            "StockTimber",
            "StockPetroleum",
            "StockCoal",
            "StockMetalOre",
            "StockUranium",
            "StockElectricPower",
            "StockConsumerGoods",
            "StockIndustryGoods",
            "StockMilitaryGoods",
            "SocialSpendingCultureSubsidy",
            "SocialSpendingEducation",
            "SocialSpendingEnvironment",
            "SocialSpendingFamilySubsidy",
            "SocialSpendingHealthCare",
            "SocialSpendingInfrastructure",
            "SocialSpendingLawEnforcement",
            "SocialSpendingSocialAssistance"
        };

        public static string[] rowDisplayNameText = new string[]
        {
            "Agriculture",
            "Rubber",
            "Timber",
            "Petroleum",
            "Coal",
            "Metal Ore",
            "Uranium",
            "Electric Power",
            "Consumer Goods",
            "Industry Goods",
            "Military Goods",
            "Social Culture Subsidy",
            "Social Education",
            "Social Environment",
            "Social Family Subsidy",
            "Social Healthcare",
            "Social Infrastructure",
            "Social Law Enforcement",
            "Social Social Assistance",
            "Unit ProfileStealth",
            "Unit Spotting",
            "Unit MoveSpeed",
            "Unit MoveRange",
            "Unit Fuel Capacity",
            "Unit Supplies Capacity",
            "Unit Carrier Capacity",
            "Unit Missile Size",
            "Unit Fort Att.",
            "Unit Soft Att.",
            "Unit Hard Att.",
            "Unit Close Att.",
            "Unit Close Air Att.",
            "Unit Mid Air Att.",
            "Unit High Air Att.",
            "Unit Surface Attack",
            "Unit Defense Close",
            "Unit Defense Ground",
            "Unit Defense Air",
            "Unit DefenseIndirect",
            "Unit Ammo Weight",
            "Unit Missile Capacity",
            "Unit Uranium Cost",
            "Unit Mil.G Needs",
            "Treasury",
            "GDP/c",
            "Military Approval (MAR)",
            "Domestic Approval (DAR)",
            "Credit Rating",
            "Treaty Integrity",
            "World UN Opinion",
            "Population",
            // "Stock",
            // "Demands",
            // "Actual Use",
            // "Production",
            // "Production Capacity",
            // "Production Cost",
            // "Market Price",
            // "Margin",
            // "Base Cost",
            // "Full Cost",
            // "City Production",
            // "Node Production",
            // "Max Demand",
            // "Min Demand",
        };
    }
}
