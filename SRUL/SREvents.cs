using System;
using DevExpress.XtraEditors;

namespace SRUL;

public enum EUnitSelection
{
    None,
    Tracked,
    Untracked,
}
public class SREvents
{
    /// <summary>
    /// NAVAL selection event to subscrit
    /// </summary>
    public event EventHandler OnNavalIsSelected;
    protected virtual void OnNavalSelected(bool isNaval)
    {
        EventHandler handler = OnNavalIsSelected; // for thread safety
        if (handler  != null)
            handler (this, EventArgs.Empty);
    }
    private bool _isNaval;
    public bool IsNaval { get => _isNaval; set
        { if (_isNaval == value) return; _isNaval = value;
            OnNavalSelected(value);
        }
    }
    /// <summary>
    /// Build Mode Override event to subscribe
    /// </summary>
    public event EventHandler OnBuildModeOverrideIsChanged;
    protected virtual void OnBuildModeOverride(bool Nanana)
    {
        EventHandler handler = OnBuildModeOverrideIsChanged; // for thread safety
        if (handler  != null)
            handler (this, EventArgs.Empty);
    }

    private bool _buildModeOverride;
    private string _buildModeOverrideId;

    public string BuildModeOverrideId { get => _buildModeOverrideId;
        set { if (_buildModeOverrideId == value) return; _buildModeOverrideId = value;
            OnBuildModeOverride(value == _buildModeOverrideId);
        }
    }

    public bool BuildModeOverride { get => _buildModeOverride; 
        set { if (_buildModeOverride == value) return; _buildModeOverride = value;
            OnBuildModeOverride(value);
        }
    }
    /// <summary>
    /// Unit selected to subscribe
    /// </summary>
    public event EventHandler OnUnitIsSelected;
    private UIntPtr _isUnitSelected;
    public int UnitSelectionCounter;

    public UIntPtr IsUnitSelected
    {
        get
        {
            return _isUnitSelected;
        }
        set
        {
            if (_isUnitSelected == value) {
                UnitSelectionCounter = 0;
                return;
            }
            _isUnitSelected = value;
            if(UnitSelectionCounter == 0) 
                OnUnitSelectedChange(value);
        }
    }

    protected virtual void OnUnitSelectedChange(UIntPtr unitSelectionState)
    {
        EventHandler handler = OnUnitIsSelected; // for thread safety
        if (handler  != null)
            handler (this, EventArgs.Empty);
    }
    public void OnTechEffectValidated(object sender, EventArgs _)
    {
        var obj = sender as LookUpEdit;
        if (!obj.ItemIndex.Equals(-1)) obj.Tag.ToString().GetFeature().WriteTo(SRLoaderForm._srLoader.rw, obj.EditValue.ToString());
    }

    public static SREvents CreateInstance()
    {
        return new SREvents();
    }
}