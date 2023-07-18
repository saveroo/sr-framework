using System.Threading;

namespace SRUL.UnitTracker;

public interface ITrackedUnitStat
{
    CancellationTokenSource StatToken { get; set; }
    bool StatFreeze { get; set; }
    string StatId { get; set; }
    string StatName { get; set; }

    string StatValue
    {
        get;
        // get => _statValue;
        set;
    }

    string StatFormattedValue { get; set; }
    string MetaFormat { get; set; }
    string MetaType { get; set; }
    void Write (string value);
    TrackedUnitStat Read(string? format = null);
}