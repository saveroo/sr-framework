using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Memories;
using System.Management;
using System.Threading;
using SRUL.Types;

namespace SRUL
{
    public class SRReadWrite : Meme
    {
        private readonly Meme _objectInstance;
        public System.Diagnostics.Process LoadedGameProcess = null;
        public System.Diagnostics.Process[] ProcessList;
        public IList<Games> GameList;
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
            StartProcessWatcher();
            GetAllProcesses();
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

        public void InitAvailableGames(IList<Games> gms)
        {
            GameList = gms;
        }
        // public bool SeekProcess(string selectedProcName)
        // {
        //     foreach (var proc in ProcessList)
        //     {
        //         if (proc.ProcessName == selectedProcName)
        //         {
        //             
        //         }
        //     }
        // }

        private void processStartEvent_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var processName = e.NewEvent.Properties["ProcessName"].Value.ToString();
            var processID = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value).ToString();

            Debug.WriteLine("[Arrive]: " + processName);

            if (Selected.GameProcessNameList != null)
            { 
                if (Selected.GameProcessNameList.Contains(processName)) return;
                // ArrivalEvent = true;
                LoadedGameProcess = LoadProcess(Selected.GameProcessName);
                if (LoadedGameProcess != null) ArrivalEvent = true;
                Selected.GamePID = LoadedGameProcess.Id;
                Selected.GameProcess = LoadedGameProcess;
            }
            // LoadGameVersion(LoadedGameProcess);
        }
        private void processStopEvent_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var processName = e.NewEvent.Properties["ProcessName"].Value.ToString();
            var processID = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value);

            Debug.WriteLine("[Exit]: " + processName + $" ({processID})");
            Debug.WriteLine("[Exit] Selected: " + processID + $" ({Selected.GamePID})");
            
            if (processID != Selected.GamePID) return;
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
        }

        public System.Diagnostics.Process LoadProcess(string procName)
        {
            if (_objectInstance.theProc != null) return _objectInstance.theProc;
            try
            {
                if(!ArrivalEvent) 
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
                Console.WriteLine("Something is not right:"+e);
            } 
            ArrivalEvent = false; 
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
 
        public bool SRIsNaval(int classes)
        {
            if (classes >= 15 && classes <= 20)
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

        public dynamic SRRead(string varName, bool rawValue = false, IList<Feature> from = null)
        {
            var fitur = SRMain.Instance.feature(varName, from);
            if (fitur == null) return "";
            var enabled = fitur.enabled;
            if (!enabled) return "";
            
            var types = fitur.type;
            var format = fitur.format;
            var execFeature = Read(types, SRMain.Instance.pointerStore(varName));
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
            var fitur = SRMain.Instance.feature(varName);
            var addressBuilder = SRMain.Instance.pointerStore(varName);

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
            var fitur = SRMain.Instance.feature(varName);
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

        public bool SRWrite(string varName, string value, string type = null)
        {
            var fitur = SRMain.Instance.feature(varName);
            var addressBuilder = SRMain.Instance.pointerStore(varName);

            var enabled = fitur.enabled;
            if (!enabled) return false;
            
            var featureType = fitur.type;
            if (featureType == "byte")
                Write("int", addressBuilder, value);
            else
                Write(featureType, addressBuilder, value);

            return true;
        }
        public dynamic Read(string type, string varName, bool round = true)
        {
            var instance = _objectInstance;
            switch (type)
            {
                case "float":
                    return instance.ReadFloat(varName, "", round);
                case "double":
                    return instance.ReadDouble(varName, "", round);
                case "long":
                    return instance.ReadLong(varName, "");
                case "decimal":
                    return (decimal)instance.ReadFloat(varName, "", round);
                case "int":
                    return instance.Read2Byte(varName);
                case "2byte":
                    return instance.Read2Byte(varName);
                case "byte":
                    return instance.ReadByte(varName);
                case "string":
                    return instance.ReadString(varName);
                default:
                    return instance.ReadFloat(varName, "", round);
            }
        }
        public bool Write(string type, string varName, dynamic value)
        {
            return _objectInstance.WriteMemory(varName, type, value);
        }
        
        public dynamic Freeze(string type, string varName)
        {
            return _objectInstance.FreezeValue(varName, type, Read(type, varName));
        }
    }
}
