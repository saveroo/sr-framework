namespace SRUL;

public static class SRGuard
{
    public static bool Allowed()
    {
        if (ActiveTrainer.Instance.TrainerAvailability)
            if (ActiveTrainer.Instance.GameValidated)
                // if (ActiveTrainer.Instance.IsRegistered)
                    return true;
        return false;
    }

    private static string _lastId = "";
    private static string _lastNum = "";
    public static bool TrackedBtnIsAllowedToContinue()
    {
        var id = "UnitID".GetFeature().value;
        var num = "ArmyBattalionNumber".GetFeature().value;
        if (id == _lastId && num == _lastNum) return false; 
        _lastId = id; 
        _lastNum = num;
        return true;
    }
}