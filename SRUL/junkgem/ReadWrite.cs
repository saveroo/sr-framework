using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Memories;
using System.Management;
using SRUL.Types;

namespace SRUL
{
    public static class DecimalExtensions
    {
        public static Decimal Normalize(this Decimal value)
        {
            return value / 1.000000000000000000000000000000000m;
        }
    }
    public class ReadWrite : Meme, IReadWrite
    {
        public bool loaded;
        private readonly Meme _objectInstance;
        public int ProcId;
        private string SelectedGameName { get; set; }
        private string SelectedGameProcessName { get; set; }
        private string SelectedGameVersion { get; set; }
        // private GameList _gameList = new GameList();
        public string GameVersion;
        public bool Allowed;
        private readonly VariableStore _store = new VariableStore();

        public delegate void DoSomething(bool isCompatible);
        ManagementEventWatcher processStartEvent = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStartTrace");
        ManagementEventWatcher processStopEvent = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStopTrace");
        public bool ProcessWatcherStarted;

        public ReadWrite()
        {
            _objectInstance = this;
            // this.m = new ReadWrite();
            // Load();
            // GetGameVersion(ProcId);
            // IsCompatible();
            processStartEvent.EventArrived += (processStartEvent_EventArrived);
            processStopEvent.EventArrived += (processStopEvent_EventArrived);
            processStartEvent.Start();
            processStopEvent.Start();
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

        public void SeekProcess()
        {
            if (_objectInstance.theProc == null)
            {
                List<System.Diagnostics.Process> processList = System.Diagnostics.Process.GetProcesses().ToList();
                string pName = processList.FirstOrDefault(s => s.ProcessName == SelectedGameProcessName).ProcessName;
                if(pName != null) 
                    _objectInstance.OpenProcess(pName);
            }
            // var query = System.Diagnostics.Process.GetProcesses().Select( process => new Tuple<string, int>( process.ProcessName, process.Id ) );
        }

        private void processStartEvent_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var processName = e.NewEvent.Properties["ProcessName"].Value.ToString();
            var processID = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value).ToString();

            // e.NewEvent.Properties["ProcessName"].
            if (processName != SelectedGameProcessName + ".exe") return;
            
            loaded = true;
            IsCompatible();
            Load();
            // SR New Iteration
            if (LoadGameSelection(SelectedGameProcessName))
                loaded = true;
            else
                loaded = false; 
            MessageBox.Show("Ketemu");
            ProcId = Convert.ToInt32(processID);
            // Iteration End
            GetGameVersion(ProcId);
            // Console.WriteLine("Process started. Name: " +processName + " | ID: " + processID);
        }
        private void processStopEvent_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var processName = e.NewEvent.Properties["ProcessName"].Value.ToString();
            var processID = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value).ToString();

            if (processName != SelectedGameProcessName + ".exe") return;
            loaded = false;
            IsCompatible();

            // Console.WriteLine("Process stopped. Name: " + processName + " | ID: " + processID);
        }

        // new iteration for SRXtraFrom
        public bool LoadGameSelection(string procName)
        {
            if (_objectInstance.theProc == null) return loaded;
            try
            {
                if (_objectInstance.OpenProcess(procName))
                    loaded = true;
                return loaded;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                // throw;
            }

            return loaded;
        }
        public bool SetGameSelection(string gameName, string procName)
        {
            SelectedGameName = gameName;
            SelectedGameProcessName = procName;
            return loaded;
        }

        public bool SetGameVersion(string trainerAvailableGameVersion)
        {
            SelectedGameVersion = trainerAvailableGameVersion;
            return IsCompatible();
        }
 
        public bool SRIsNaval(int classes)
        {
            if (classes >= 15 && classes <= 20)
                return true;
            return false;
        }
        public void Load()
        {
            if (loaded || _objectInstance.theProc != null)
                return;
            try
            {
                var gameProcId = _objectInstance.GetProcIdFromName(SelectedGameProcessName); //use task manager to find game name. For CoD MW3 it is iw5sp. Do not add .exe extension
                if (gameProcId != 0)
                {
                    _objectInstance.OpenProcess(SelectedGameProcessName);
                    ProcId = gameProcId;
                    loaded = true;
                    GetGameVersion(gameProcId);
                    IsCompatible();
                }
                else
                {
                    loaded = false;
                    Allowed = false;
                }
            }
            catch (Exception e)
            {
                // Disable ERror when game not loaded then loaded then not loaded
                // Console.WriteLine(e);
                // throw;
            }
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
        public void GetGameVersion(int procId)
        {
            if (!loaded || _objectInstance.theProc == null) return;
            Debug.WriteLine("PID: " + _objectInstance.theProc.Id);
            var proc = System.Diagnostics.Process.GetProcessById(procId);
            if (proc.MainModule != null) 
            {
                GameVersion = proc.MainModule.FileVersionInfo.FileVersion;
            }
            // SelectedGameName = 
            // CompatibleVersion = JSONReader.Instance.activeTrainer.GameVersions.ToString();
        }
        
        public bool IsCompatible()
        {
            if (GameVersion == SelectedGameVersion)
            {
                Allowed = true;
                return true;
            }
            Allowed = false;
            return false;
        }
        
        public dynamic SRRead(string varName, bool rawValue = false)
        {
            var fitur = JSONReader.Instance.feature(varName);
            var enabled = fitur.enabled;
            if (!enabled) return "";
            
            var types = fitur.type;
            var format = fitur.format;
            var execFeature = Read(types, JSONReader.Instance.pointerStore(varName));
            if (rawValue) return execFeature;
            switch (types)
            {
                case "float":
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
            var fitur = JSONReader.Instance.safeFeatureSearch(varName);
            var addressBuilder = JSONReader.Instance.pointerStore(varName);

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
            var fitur = JSONReader.Instance.safeFeatureSearch(varName);
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
            var fitur = JSONReader.Instance.feature(varName);
            var addressBuilder = JSONReader.Instance.pointerStore(varName);

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
        public static string FormatNumber(string n) // Format to Currency
        {
            // CultureInfo culture = new CultureInfo("en-US");
            // if (n < 1000)
            //     return n.ToString();
            // if (n < 10000000)
            //     return String.Format("{0:#,,.##}M", n - 5000);
            // if (n < 100000000)
            //     return String.Format("{0:#,,.#}M", n - 50000);
            // if (n < 1000000000)
            //     return String.Format("{0:#,,.}M", n - 500000);
            // var res = n.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
            // var res = "$ " + n;
            // if (n.Length == 7)
            //     res = "$ " + n.Substring(0, n.Length-7) + " M";
            // if (n.Length > 10)
            //     res = "$ " + n.Substring(0, n.Length-10) + " M";
            // return res;
            var res = n;
            return n.ToString(CultureInfo.CreateSpecificCulture("en-US"));
        }
        
        public dynamic FinTreasury
        {
            // get => FormatNumber((float)Read("float", _store.FinanceTreasury, false));
            get => Read("float", _store.FinanceTreasury, false).ToString("N0");
            set => Write("float", _store.FinanceTreasury, value);
        }

        public dynamic FinGDP
        {
            // get => FormatNumber((float)Read("float", _store.FinanceGDPc, false));
            get => Read("float", _store.FinanceGDPc, false).ToString("N0");
            set => Write("float", _store.FinanceGDPc, value);
        }

        public dynamic DomPopulation
        {
            get
            {
                return Read("float", _store.CountryPopulation).ToString("N0");
            }
            set
            {
                Write("float", _store.CountryPopulation, value);
            }
        }

        public dynamic DomImmigration
        {
            get => Read("float", _store.CountryImmigration).ToString("N0");
            set => Write("float", _store.CountryImmigration, value);
        }

        public dynamic DomEmigration
        {
            get => Read("float", _store.CountryEmigration).ToString("N0");
            set => Write("float", _store.CountryEmigration, value);
    }

        public dynamic DomBirth
        {
            get => Read("float", _store.CountryBirths).ToString("N0");
            set => Write("float", _store.CountryBirths, value);
        }

        public dynamic DomDeath
        {
            get => Read("float", _store.CountryDeaths).ToString("N0");
            set => Write("float", _store.CountryDeaths, value);
        }

        public dynamic DomLiteracy
        {
            get => Read("float", _store.DomesticLiteracy) * 100 + "%";
            set => Write("float", _store.DomesticLiteracy, value);
        }

        public dynamic DomTourism
        {
            get => (Read("float", _store.DomesticTourism, false) * 100).ToString("N1") + "%";
            set => Write("float", _store.DomesticTourism, value);
        }

        public dynamic DomCreditRating
        {
            get => Read("float", _store.FinanceCreditRating)*100 + "%";
            set => Write("float", _store.FinanceCreditRating, value);
        }

        public dynamic DomUNSubsidy
        {
            get => (Read("float", _store.DomesticWorldMarketSubsidyRate) * 100).ToString("N1") + "%";
            set => Write("float", _store.DomesticWorldMarketSubsidyRate, value);
        }

        public dynamic DomUNOpinion
        {
            get
            {
                string SemanticOpinion(float opinion)
                {
                
                    if (opinion <= 0.10)
                        return "(Outraged)";
                    else if (opinion < 0.30)
                        return "(Disapproving)";
                    else if (opinion < 0.40)
                        return "(Concerned)";
                    else if (opinion < 0.50)
                        return "(Indifferent)";
                    else if (opinion < 0.60)
                        return "(Satisfied)";
                    else if (opinion < 0.70)
                        return "(Pleased)";
                    else
                        return opinion > 0.70 ? "(Delighted)" : "";
                }
                var readOpinion = Read("float", _store.DomesticWorldMarketOpinion, false);
                return SemanticOpinion(readOpinion);
            }
            set
            {
                Write("float", _store.DomesticWorldMarketOpinion, value);
            }
        }

        public dynamic DomTreatyIntegrity
        {
            get => (Read("float", _store.DomesticTreatyIntegrity, false) * 100).ToString("N2") + "%";
            set => Write("float", _store.DomesticTreatyIntegrity, value);
        }

        public dynamic DomDomApproval
        {
            get => (Read("float", _store.DomesticDomesticApproval, false) * 100).ToString("N1") + "%";
            set => Write("float", _store.DomesticDomesticApproval, value);
        }

        public dynamic DomMilApproval
        {
            get => (Read("float", _store.DomesticMilitaryApproval, false) * 100).ToString("N1") + "%";
            set => Write("float", _store.DomesticMilitaryApproval, value);
        }

        public dynamic FinInflation
        {
            get => (Read("float", _store.FinanceInflation, false) * 100).ToString("N1") + "%";
            // get => (Read("float", _store.FinanceInflation, false) * 100).ToString();
            set => Write("float", _store.FinanceInflation, value);
        }

        public dynamic DomUnemployment
        {
            get => (Read("float", _store.DomesticUnemployment, false) * 100).ToString("N1") + "%";
            set => Write("float", _store.DomesticUnemployment, value);
        }
        
        public dynamic ADayClickBuild
        {
            get
            {
                return (Read("float", _store.OneDayBuild));
            }
            set
            {
                if(value < 1.0)
                    Write("float", _store.OneDayBuild, "0,99999"); 
            }
        }

        public dynamic ADayClickArmy
        {
            get
            {
                return (Read("float", _store.OneDayArmy));
            }
            set
            {
                Write("float", _store.OneDayArmy, value); 
            }
        }

        public dynamic ADayClickResearch
        {
            get
            {
                return (Read("float", _store.OneDayResearchClick));
            }
            set
            {
                Write("float", _store.OneDayResearchClick, value); 
                Write("float", _store.OneDayResearchTooltip, value);
            }
        }
    }
}
