namespace SRUL.Types;

public static class SREnum
{
    public enum CategoryName {
        Resources,
        Warfare,
        Country,
        Special,
    }

    public enum TabSelection
    {
        Unknown = 99,
        Land = 0,
        Resources = 1,
        Research = 2,
        Finance = 3,
        State = 4,
        DefenseProduction = 5,
        Defense = 7,
    }

    public static TabSelection ConvertTabSelectionToString(string readValue)
    {
        switch (readValue)
        {
            case "0":
                return TabSelection.Land;
            case "1":
                return TabSelection.Resources;
            case "2":
                return TabSelection.Research;
            case "3":
                return TabSelection.Finance;
            case "4":
                return TabSelection.State;
            case "5":
                return TabSelection.DefenseProduction;
            case "7":
                return TabSelection.Defense;
            default:
                return TabSelection.Unknown;
        }
    }
}