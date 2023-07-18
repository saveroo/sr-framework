using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Memories;
using System.Management;
using System.Threading;
using SmartAssembly.Attributes;
using SRUL.Types;
using SRUL.UnitTracker;
using TB.ComponentModel;

namespace SRUL
{

    public class SRReadWrite : Meme
    {

        private readonly Meme _objectInstance;
        public Process? LoadedGameProcess = null;
        public System.Diagnostics.Process[] ProcessList;
        public ActiveTrainer Selected { get; set; } = ActiveTrainer.Instance;
        // private string LoadedGameName { get; set; } = String.Empty;
        // private string LoadedGameProcessName { get; set; } = String.Empty;
        private string LoadedGameVersion { get; set; } = null;
        private bool ArrivalEvent { get; set; } = false;
        ManagementEventWatcher processStartEvent = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStartTrace");
        ManagementEventWatcher processStopEvent = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStopTrace");
        public bool ProcessWatcherStarted;
        private SRMain _srMain;

        public SRReadWrite()
        {
            _objectInstance = this;
            // _srMain = SRMain.Instance;
            ;
            processStartEvent.EventArrived += (processStartEvent_EventArrived);
            processStopEvent.EventArrived += (processStopEvent_EventArrived);
            // Selected = new ActiveTrainer();
            processStartEvent.Start();
            // GetAllProcesses();
        }

        public System.Diagnostics.Process[] GetAllProcesses()
        {
            ProcessList = System.Diagnostics.Process.GetProcesses();
            return ProcessList;
        }
        public void StartProcessWatcher()
        {
            if (!ProcessWatcherStarted)
            {
                processStartEvent.Start();
                processStopEvent.Start();
            }
        }
        public void StopProcessWatcher()
        {
            if (ProcessWatcherStarted)
            {
                processStartEvent.Stop();
                processStopEvent.Stop();
            }
        }

        // TODO: still violating SRP, and need to get rid of memory leaks from somewhere
        private void processStartEvent_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var evt = e.NewEvent;
            var processName = evt.GetPropertyValue("ProcessName");
            var processId = evt.GetPropertyValue("ProcessID");
            // Debug.WriteLine("[Arrive]: " + processName);

            // Guard
            if (Selected.GameProcessNameList == null) { evt.Dispose(); return; }
            if (string.IsNullOrEmpty(Selected.GameProcessName)) { evt.Dispose(); return; }
            
            if ((string?)processName != $"{Selected.GameProcessName}.exe") { evt.Dispose(); return; }
                LoadedGameProcess = LoadProcess((uint)processId); // ArrivalEvent = true;
            if (LoadedGameProcess == null) { evt.Dispose(); return; }

            ArrivalEvent = true; 
            Selected.GamePID = LoadedGameProcess.Id; 
            Selected.GameProcess = LoadedGameProcess; 
            
            // Switch to listen to HasExit
            processStartEvent.Stop(); 
            processStopEvent.Start();
            evt.Dispose();
            // LoadGameVersion(LoadedGameProcess);
        }
        
        // TODO: after reverting to old build, need to separate responsibility
        // Stop Event should only listen when the game is already loaded.
        // and behave to wait for its process to stop.
        private void processStopEvent_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var evt = e.NewEvent;
            // var processName = evt.Properties["ProcessName"].Value.ToString();
            var processID = Convert.ToInt32(evt.Properties["ProcessID"].Value);

            // Debug.WriteLine("[Exit]: " + processName + $" ({processID})");
            // Debug.WriteLine("[Exit] Selected: " + processID + $" ({Selected.GamePID})");

            if (processID != Selected.GamePID)
            {
                evt.Dispose();
                return;
            }
            
            // Deactivate, so Loadprocess can load it again?
            ArrivalEvent = false;
            if (Selected.GameProcess != null)
            {
                _objectInstance.CloseProcess();
                LoadedGameProcess.Close();
                Selected.GameProcess.Close();
            }
            Selected.GamePID = 0;
            Selected.GameValidated = false;
            LoadedGameProcess = null;
            Selected.GameProcess = null;
            processStartEvent.Start();
            processStopEvent.Stop();
            evt.Dispose();
        }

        public Process? LoadProcess(uint pid)
        {
            if (_objectInstance.OpenProcess((int)pid)) {
                LoadedGameProcess = _objectInstance.theProc;
                Selected.GameProcess = _objectInstance.theProc;
                LoadedGameVersion = _objectInstance.theProc.MainModule.FileVersionInfo.FileVersion;
                return Selected.GameProcess;
            }

            return null;
        }
        
        public Process? LoadProcess(string procName)
        {
            if (!procName.Contains(".exe"))
                procName = procName + ".exe";
            if (Selected.GameProcessNameList.Contains(procName))
            {
                if (_objectInstance.theProc != null) 
                    if(!_objectInstance.theProc.HasExited) return _objectInstance.theProc;
                try
                {
                    if (!ArrivalEvent)
                        if (_objectInstance.OpenProcess(procName))
                        {
                            LoadedGameProcess = _objectInstance.theProc;
                            Selected.GameProcess = _objectInstance.theProc;
                            LoadedGameVersion = _objectInstance.theProc.MainModule.FileVersionInfo.FileVersion;
                            return Selected.GameProcess;
                        }
                    ArrivalEvent = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("[SR.LoadProcess] Something is not right:" + e);
                }
                ArrivalEvent = false;
                return Selected.GameProcess;
            }
            return Selected.GameProcess;           
        }
        private bool ProcessIsNull()
        {
            if (Selected.GameProcess == null && LoadedGameProcess == null)
                return true;
            return false;
        }

        public void LoadGameVersion(System.Diagnostics.Process proc)
        {
            // if (proc == null) return;
            // if (proc.MainModule != null)
            //     if(Selected.GameState) 
            //         LoadedGameVersion = proc.MainModule.FileVersionInfo.FileVersion;
        }

        public bool CompatibilityCheck()
        {
            
            if (LoadedGameProcess != null &
                string.Equals(LoadedGameVersion, Selected.GameVersion, StringComparison.OrdinalIgnoreCase)
                && Selected.GameProcess != null
                )
            {
                if(Selected.GameProcess.ProcessName == LoadedGameProcess.ProcessName)
                    Selected.GameValidated = true;
            }
            else
            {
                Selected.GameValidated = false;
            }
            
            return Selected.GameValidated;
        }
        // UI GAME SELECT SETTER
        public bool SetGameSelection(string gameName, string procName)
        {
            Selected.GameName = gameName;
            Selected.GameProcessName = procName;
            if(Selected.GameProcess != null)
                return Selected.GameProcess.MainWindowTitle == gameName;
            return false;
        }

        // UI VERSION SELECT SEttER, should be available if the game has been set.
        public bool SetGameVersion(string trainerAvailableGameVersion)
        {
            // if (!string.IsNullOrEmpty(trainerAvailableGameVersion))
            //     Selected.GameVersion = trainerAvailableGameVersion;
            // return Selected.GameVersion == LoadedGameVersion;
            if(LoadedGameProcess != null) 
                return Selected.GameName == LoadedGameProcess.MainWindowTitle && trainerAvailableGameVersion == LoadedGameVersion;
            return false;
        }
 
        [ForceObfuscate(true)]
        public bool SRIsNaval(int classes)
        {
            if (classes >= 15 && classes <= 20)
                return true;
            return false;
        }

        private readonly string[] navalClasses = new[]
        {
            "15",
            "16",
            "17",
            "18",
            "19",
            "20"
        };
        
        [ForceObfuscate(true)]

        public bool SRIsNaval(string classes)
        {
            if(classes == null) return false;
            if (navalClasses.Contains(classes))
                return true;
            return false;
        }
        public void Load(string gname, string procName, string procVersion)
        {
            // if(gname && procName && procVersion)
            
            // SetGameSelection(gname,procName);
            // SetGameVersion(procVersion);
            // if (!Selected.GameValidated && ArrivalEvent == false)
            // {
            //     var proc = LoadProcess(procName);
            //     LoadGameVersion(proc);
            // }
        }

        public dynamic InvokeDelegation(string delegation, string value = null)
        {
            var mInfo = this.GetType().GetProperty(delegation);
            if (value == null)
            {
                if (mInfo != null) return mInfo.GetValue(this, null);
                return null;
            }
            else
            {
                if (mInfo != null) mInfo.SetValue(this, value);
                return true;
            }
        }
        // public void GetGameVersion(int procId)
        // {
        //     if (LoadedGameProcess != null)
        //     {
        //         Game
        //     }
        // }

        private dynamic _execFeature;
        public dynamic SRRead(string varName, bool rawValue = false, IList<Feature> from = null, bool round = true)
        {
            var fitur = varName.GetFeature();
            if (fitur == null) return String.Empty;
            var enabled = fitur.enabled;
            if (!enabled) return String.Empty;
            
            var types = fitur.type;
            var format = fitur.format;
            _execFeature = Read(types, SRMain.Instance.PointerStore(varName), round);
            if (rawValue) return _execFeature;
            switch (types)
            {
                case "float":
                    var f =  _objectInstance.ReadFloat(SRMain.Instance.PointerStore(varName), "", round);
                    if (float.IsNaN(f)) return float.MinValue.ToString();
                    return f.ToString("N");
                case "byte":
                    return _execFeature.ToString("0");
                case "2byte":
                    return _execFeature.ToString("0");
                case "int":
                    return _execFeature.ToString();
                default:
                    return _execFeature.ToString();
            }
        }
        
        public dynamic SRPersistenceRead(TrackedUnitStat stat, bool rawValue = false, bool round = true)
        {
            if (stat == null) return "";
            var types = stat.MetaType;
            var execFeature = Read(types, stat.StatId, false);
            if (rawValue) return execFeature;
            switch (types)
            {
                case "float":
                    if (Double.IsNaN(execFeature)) return 0;
                    var a = execFeature.ToString("N");
                    return a; 
                case "byte":
                    return execFeature.ToString();
                case "int":
                    return execFeature.ToString();
                default:
                    return execFeature.ToString();
            }
        }

        public dynamic SRDisplayFormat(string format, string a)
        {
            dynamic strFormat;
            switch (format)
            {
                case "%":
                case "percentage":
                    strFormat = string.Format("{0:P}", a);
                    return strFormat;
                case "$":
                case "currency":
                    strFormat = string.Format("{0:C}", a);
                    return strFormat;
                default:
                    return "s";
            }
        }

        public bool SRFreezePersistent(string varName, bool unfreezer, string value = null)
        {
            var fitur = varName.GetFeature();
            if (fitur == null) return false;
            var addressBuilder = SRMain.Instance.PointerStore(varName);
            
            var addr = _objectInstance.GetCode(addressBuilder, "");
            MessageBox.Show(addr.ToString());
            var enabled = fitur.enabled;
            if (!enabled) return false;
            var featureFreeze = fitur.freeze;
            var featureType = fitur.type;
            // if (!featureFreeze) return false;

            if (!unfreezer)
            {
                _objectInstance.UnfreezeValue(addressBuilder);
            }
            else
            {
                if(value != null) 
                    _objectInstance.SRFreezeValue(addressBuilder, featureType, value);
                else 
                    _objectInstance.SRFreezeValue(addressBuilder, featureType, fitur.value);     
            }
            
            return false;
        }
        
        public bool SRFreeze(string varName, string value = null, bool allowIncrease = false)
        {
            // var fitur = SRMain.Instance.feature(varName);
            var fitur = varName.GetFeature();
            if (fitur == null) return false;
            var enabled = fitur.enabled;
            if (!enabled) return false;
            
            var featureFreeze = fitur.freeze;
            var featureType = fitur.type;
            // var currentValue = SRRead(varName, true);
            // var clonedValue = currentValue.ToString().Clone();
            if (!featureFreeze) return false;
            // var realValue = value ?? currentValue;

            if (allowIncrease)
            {
                var curr = SRRead(varName, true);
                if(curr > value.StrToDouble())
                    SRWrite(varName, curr.ToString(), featureType);
                else
                    SRWrite(varName, value ?? fitur.value, featureType);
                return true;
            }
            SRWrite(varName, value ?? fitur.value, featureType); 
            return true;
        }
        [DoNotObfuscate]
        public bool SRWrite(string varName, string value, string type = null, bool valueOverride = false)
        {
            var fitur = varName.GetFeature();
            if (fitur == null) return false;
            var addressBuilder = SRMain.Instance.PointerStore(varName);

            var enabled = fitur.enabled;
            if (!enabled) return false;
            
            var featureType = fitur.type;
            var featureValue = String.IsNullOrEmpty(value) ? fitur.value : value;

            if (String.IsNullOrEmpty(featureValue)) return false;
            //TODO: there are 2 choices here,
            // 1: So if value from json is empty, it will use hardcoded valuue.
            // 2: or if SRWrite doesnt specify value, it will use from json instead.
            // 3: json value should be priority, then check hardcoded one.
            Write(featureType, addressBuilder, featureValue);

            return true;
        }

        public bool MemoryAvailable(string address)
        {
            var floatCode = GetCode(address);
            return floatCode.IsValid();
        }

        // public bool SafeWrite(string value)
        // {
        //     
        // }

        public T Read<T>(string type, string varName)
        {
            switch (type)
            {
                case "float":
                    return (T)(object)_objectInstance.ReadFloat(varName, "", false);
                case "double":
                    return (T)(object)_objectInstance.ReadDouble(varName, "", false);
                case "long":
                    return (T)(object)_objectInstance.ReadLong(varName, "");
                case "decimal":
                    return (T)(object)(decimal)_objectInstance.ReadFloat(varName, "", false);
                case "int":
                    return (T)(object)_objectInstance.ReadInt(varName);
                case "2byte":
                    return (T)(object)_objectInstance.Read2Byte(varName);
                case "2bytes":
                    return (T)(object)_objectInstance.Read2Byte(varName);
                case "byte":
                    return (T)(object)_objectInstance.ReadByte(varName);
                case "string":
                    return (T)(object)_objectInstance.ReadString(varName);
                default:
                    return (T)(object)_objectInstance.ReadFloat(varName, "", false);
            }
        }
        
        // TODO: DPA Boxing allocation: 2,054.3 MB allocated, 10 issues conversion from 'float' to 'dynamic' requires boxing of value type, 
        public dynamic Read(string type, string varName, bool round = false)
        {
            switch (type)
            {
                case "float":
                    return _objectInstance.ReadFloat(varName, "", round);
                case "double":
                    return _objectInstance.ReadDouble(varName, "", round);
                case "long":
                    return _objectInstance.ReadLong(varName, "");
                case "decimal":
                    return (decimal)_objectInstance.ReadFloat(varName, "", round);
                case "int":
                    return _objectInstance.ReadInt(varName).ToString("0");;
                case "2byte":
                case "2bytes":
                    return _objectInstance.Read2Byte(varName);
                case "byte":
                    return _objectInstance.ReadByte(varName);
                case "string":
                    return _objectInstance.ReadString(varName);
                default:
                    return _objectInstance.ReadFloat(varName, "", round);
            }
        }
        public bool Write(string type, string address, dynamic value)
        {
            return _objectInstance.WriteMemory(address, type, value);
        }
        
        public dynamic Freeze(string type, string varName)
        {
            return _objectInstance.FreezeValue(varName, type, Read(type, varName));
        }
    }
}
