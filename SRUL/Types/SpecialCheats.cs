using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace SRUL.Types;

public enum ECheats
{
    Default = 0,
    UnlockAllUnits = 2,
    OneDayBuild = 4,
    OneDayFacility = 32,
    EndDay = 8,
    NoMove = 128
}

public enum ECheatState
{
    Idle,
    Enabling,
    Disabling,
    Clearing,
}
public class Cheat
{
    public ECheats Id { get; set; } = ECheats.Default;
    public bool Checked { get; set; } = false;
    public string Name { get; set; }
    public int Value { get; set; } = 0;

    public Cheat(ECheats id, bool checkState, string name, int value)
    {
        Id = id;
        Checked = checkState;
        Name = name;
        Value = value;
    }
}

public class SpecialCheats
{
    public List<Cheat> Cheats { get; set; }
    public Dictionary<ECheats, int> CheatCache = new Dictionary<ECheats, int>();
    private int _writtenValue = 0;

    public int WrittenValue
    {
        get
        {
            return _writtenValue;
        }
        set => _writtenValue = value;
    }

    public SpecialCheats()
    {
        Cheats = new List<Cheat>();
        // Cheats.Add(new Cheat(ECheats.Default, false, "Default", 0));
        Cheats.Add(new Cheat(ECheats.UnlockAllUnits, false, "Unlock All Units (G)", 2));
        Cheats.Add(new Cheat(ECheats.OneDayBuild, false, "2 Day Army (G)", 4));
        Cheats.Add(new Cheat(ECheats.OneDayFacility, false, "1 Day Facility (?)", 32));
        Cheats.Add(new Cheat(ECheats.EndDay, false, "End Day (G)", 8));
        Cheats.Add(new Cheat(ECheats.NoMove, false, "Unit No Move (G)", 128));
    }

    public void AddEnabledCheat(ECheats c)
    {
        if (!CheatCache.ContainsKey(c))
        {
            CheatCache.Add(c, (int)c);
            Writer();
        }
    }
    
    public void RemoveEnabledCheat(ECheats c)
    {
        if(CheatCache.ContainsKey(c))
            CheatCache.Remove(c);
        Writer();
    }

    public void InitView(ref CheckedListBoxControl clb)
    {
        clb.DataSource = HandmadeFeatures.Instance.BindingSource();
        clb.DataSource = new BindingSource( new BindingList<Cheat>(Cheats), null);
        clb.DisplayMember = "Name";
        clb.ValueMember = "Checked";
        clb.ItemCheck += (sender, args) =>
        {
            CheckedListBoxControl clbc = sender as CheckedListBoxControl;
            // var val = (ECheats)clbc.GetItemValue(args.Index);
            var itm = (Cheat)clbc.GetItem(args.Index);
            itm.Checked = clbc.GetItemChecked(args.Index);
            if (clbc.GetItemChecked(args.Index))
                AddEnabledCheat(itm.Id);
            else
                RemoveEnabledCheat(itm.Id);
        };
    }

    private bool _default = true;
    public void Writer()
    {
        WrittenValue = 0;
        if (CheatCache.Count == 0)
        {
            SRLoaderForm._srLoader.rw.SRWrite("CheatAllUnits", WrittenValue.ToString());
            return;
        }
        else foreach (var cheat in CheatCache)
        {
            WrittenValue += cheat.Value;
        }
        SRLoaderForm._srLoader.rw.SRWrite("CheatAllUnits", WrittenValue.ToString());
    }
}