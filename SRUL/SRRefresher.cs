using System;
using System.Windows.Controls;
using Timer = System.Windows.Forms.Timer;

namespace SRUL;

public class SRRefresher
{
    public static SRRefresher CreateInstance()
    {
        return new SRRefresher();
    }

    System.Windows.Forms.Timer _windowTimer = new Timer();
    public static GridView[] _gridViews;
    private int invokeCount = 0;
    private int maxCount = int.MaxValue;
    private System.Threading.Timer _threadingTimer;

    public void TimerCallback(Object stateInfo)
    {
        var autoResetEvent = (Timer)stateInfo;
        if (invokeCount == maxCount)
        {
            invokeCount = 0;
            autoResetEvent.Stop();
        }
    }

    public void InitializeTimer(Action<object> doAct)
    {
        _windowTimer = new System.Windows.Forms.Timer();
        _windowTimer.Interval = 300;
        _windowTimer.Tick += (sender, e) =>
        {
            doAct(this);
            invokeCount++;
        };
        _windowTimer.Start();
    }
}