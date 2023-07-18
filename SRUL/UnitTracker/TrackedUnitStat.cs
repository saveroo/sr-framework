using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SRUL.Annotations;

namespace SRUL.UnitTracker;

public sealed class TrackedUnitStat : INotifyPropertyChanged, ITrackedUnitStat
{
    private bool TaskIsUseless(Task? task)
    {
        if (task == null)
            return true;
        if(task.IsCompleted)
            return true;
        if(task.IsCanceled)
            return true;
        if(task.IsFaulted)
            return true;
        if(task.Status == TaskStatus.RanToCompletion)
            return true;
        if(task.Status == TaskStatus.Created)
            return false;
        if(task.Status == TaskStatus.Running)
            return false;
        return false;
    }
    private Task statTask
    {
        get => _statTask;
        set => _statTask = value;
    }

    private bool _statFreeze;
    private CancellationTokenSource? _statToken;
    private string? _statValue;
    private Task _statTask = null;
    [NotNull] private string _statId;
    [NotNull] private string _statName;

    public CancellationTokenSource StatToken
    {
        get => _statToken;
        set => _statToken = value;
    }

    public bool StatFreeze
    {
        get
        {
            return _statFreeze;
        }
        set
        {
            _statFreeze = value;
            if (value)
            {
                if (_statToken == null)
                    _statToken = new CancellationTokenSource();
                else if(_statToken.Token.IsCancellationRequested)
                    _statToken = new CancellationTokenSource();

                if(TaskIsUseless(statTask)) statTask = Task.Factory.StartNew(() => {
                    Debug.WriteLine($"{StatName} Task Created!");
                    while (!_statToken.Token.IsCancellationRequested)
                    {
                        SRLoaderForm
                            ._srLoader
                            .rw
                            .WriteMemory(StatId, MetaType, StatValue);
                        // Debug.WriteLine($"{StatName} Is Running~!");
                        Thread.Sleep(125);
                    } 
                    Debug.WriteLine($"{StatName} Is Cancelled");
                }, _statToken.Token);
                Debug.WriteLine($"{StatName} is frozen");
                Debug.WriteLine($"Task: {statTask.Status}");
                Debug.WriteLine($"Token: {_statToken.Token.IsCancellationRequested}");
            }
            else
            {
                if (statTask != null) {
                    try
                    {
                        _statToken.Cancel();
                        Debug.WriteLine($"{StatName} Unfreeze");
                        Debug.WriteLine($"Cancelled: {_statToken.Token.IsCancellationRequested}");
                    }
                    finally
                    {
                        Debug.WriteLine($"#Task Status: {statTask.Status}");
                        if (TaskIsUseless(statTask))
                        {
                            statTask.Dispose();
                            statTask = null;
                        }
                        Debug.WriteLine($"#Task: {statTask} Disposed");
                        Debug.WriteLine($"#Token: {_statToken}");
                    }
                }
            }
        }
    }

    [NotNull]
    public string StatId
    {
        get => _statId;
        set => _statId = value;
    }

    [NotNull]
    public string StatName
    {
        get => _statName;
        set => _statName = value;
    }

    [NotNull]
    public string StatValue
    {
        get => _statValue;
        // get => _statValue;
        set
        {
            // if (value.Equals(_statValue)) return;
            _statValue = value;
        }
    }

    public string StatFormattedValue { get; set; }
    [NotNull]
    public string MetaFormat { get; set; }
    [NotNull]
    public string MetaType { get; set; }
        
    public void Write (string value)
    {
        SRLoaderForm._srLoader.rw.Write(MetaType, StatId, value);
    }
        
        
    public TrackedUnitStat Read(string? format = null)
    {
        // dont' read name
        if (StatName == "UnitName") return this;
        if (format != null)
        {
            StatValue = SRLoaderForm._srLoader.rw.Read(MetaType, StatId).ToString(format);
            return this;
        }
        
        // TODO: Dpa analysis, boxing stuff
        if(MetaType == "float") 
            StatValue = SRLoaderForm._srLoader.rw.Read<float>(MetaType, StatId).ToString("N");
        if (MetaType == "string")
            StatValue = SRLoaderForm._srLoader.rw.Read<string>(MetaType, StatId);
        if (MetaType == "2bytes" || MetaType == "2byte")
            StatValue = SRLoaderForm._srLoader.rw.Read<Int32>(MetaType, StatId).ToString();
        if (MetaType == "byte")
            StatValue = SRLoaderForm._srLoader.rw.Read<int>(MetaType, StatId).ToString();
        return this;
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    [Annotations.NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}