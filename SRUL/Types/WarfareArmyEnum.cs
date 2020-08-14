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