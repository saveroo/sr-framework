using DevExpress.Utils.Controls;
using SRUL.Properties;

namespace SRUL;

public class SRBindings
{
    public bool WarfareArmyMorale;
    public bool WarfareArmyExperience;
    public bool WarfareArmyEfficiency;
    public bool WarfareArmyCurrentStrength;
    public bool WarfareArmyCurrentSupplies;
    public bool WarfareArmyCurrentFuel;
    
    Settings settings;
    
    public SRBindings()
    {
        settings = new Settings();
        WarfareArmyMorale = false;
        WarfareArmyExperience = false;
        WarfareArmyEfficiency = false;
        WarfareArmyCurrentStrength = false;
        WarfareArmyCurrentSupplies = false;
        WarfareArmyCurrentFuel = false;
    }

    public void BindTo<T>(T test) where T : ControlBase
    {
        // test.DataBindings.Add("Text", this, );
    }
}