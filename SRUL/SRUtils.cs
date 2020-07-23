using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using DeviceId;
using DeviceId.Components;
using Newtonsoft.Json;
using SRUL.Types;

namespace SRUL
{
    public class ClientOS
    {
        public string Name { get; set; }
        public int Build { get; set; }
        public string Version { get; set; }
        public string Architecture { get; set; }
        public uint MaxProcessCount { get; set; }
        public ulong MaxProcessRAM { get; set; }
        public string SerialNumber { get; set; }
    }

    public class ClientCPU
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        // public string Socket { get; set; }
        // public ushort Architecture { get; set; }
        // public ushort AddressWidth { get; set; }
        // public ushort DataWidth { get; set; }
        // // public string CpuArchitecture { get; set; }
        // public uint SpeedMHz { get; set; }
        // public uint BusSpeedMHz { get; set; }
        // public ulong L2Cache { get; set; }
        // public ulong L3Cache { get; set; }
        // public uint Cores { get; set; }
        // public uint Threads { get; set; }
    }

    public class SRClient
    {
        [JsonProperty("ref")]
        public string ID { get; set; }
        public string DeviceID { get; set; }
        public string UUID { get; set; }
        public string MachineName { get; set; }
        public string ExePath { get; set; }
        public DateTime FirstRun { get; set; }
        public DateTime LastActive { get; set; }
        public string ElapsedTime { get; set; }
        public bool IsOnline { get; set; }
        public string SRVersion { get; set; }
        public string SRRevision { get; set; }
        public IList<ClientOS> OS { get; set; }
        public IList<ClientCPU> CPU { get; set; }
    }
    public class SRUtils
    {
        private static readonly Lazy<SRUtils> _instance = new Lazy<SRUtils>(() => new SRUtils());
        public static SRUtils Instance => _instance.Value;
        public SRClient GetClientDevice ()
        {
            DeviceIdBuilder cDeviceId = new DeviceIdBuilder();
            cDeviceId.AddProcessorId();
            cDeviceId.AddMachineName();
            cDeviceId.AddOSVersion();
            cDeviceId.AddSystemUUID();
            cDeviceId.AddUserName();
            
            // DeviceIdComponent dc = new DeviceIdComponent("test", );
            var wmi = new ManagementObjectSearcher( "select * from Win32_OperatingSystem" )
                .Get()
                .Cast<ManagementObject>()
                .First();
            var cpu = new ManagementObjectSearcher( "select * from Win32_Processor" )
                .Get()
                .Cast<ManagementObject>()
                .First();

            var cMachineName = Environment.MachineName;
            var cExePath = Environment.CommandLine;
            var cOSVersion = Environment.OSVersion;
            var cProcessorID = new WmiDeviceIdComponent("ProcessorId", "Win32_Processor", "ProcessorId").GetValue();
            var cProcessorCount = System.Environment.ProcessorCount;
            var cUUID = new WmiDeviceIdComponent("SystemUUID", "Win32_ComputerSystemProduct", "UUID").GetValue();
            var cNetwork = new NetworkAdapterDeviceIdComponent(false, false).GetValue();
            
            var c = new SRClient();
            c.MachineName = cMachineName;
            c.DeviceID = cDeviceId.ToString();
            c.FirstRun = DateTime.Now;
            c.LastActive = DateTime.Now;
            c.ExePath = cExePath;
            c.UUID = cUUID;
            
            var cOS = new ClientOS();
            cOS.Name = ((string)wmi["Caption"]).Trim();;
            cOS.Build = ((string) wmi["BuildNumber"]).StrToInt();
            cOS.Version = (string)wmi["Version"];
            cOS.SerialNumber = (string)wmi["SerialNumber"];
            cOS.Architecture = (string)wmi["OSArchitecture"];
            cOS.MaxProcessCount = (uint)wmi["MaxNumberOfProcesses"];
            cOS.MaxProcessRAM = (ulong)wmi["MaxProcessMemorySize"];
            c.OS = new List<ClientOS>(){cOS};
            
            var cCPU = new ClientCPU();
            cCPU.ID = (string)cpu["ProcessorId"];
            cCPU.Name = (string)cpu["Name"];
            cCPU.Description = (string)cpu["Caption"];
            // cCPU.Socket = (string)cpu["SocketDesignation"];
            // cCPU.AddressWidth = (ushort)cpu["AddressWidth"];
            // cCPU.DataWidth = (ushort)cpu["DataWidth"];
            // cCPU.Architecture = (ushort)cpu["Architecture"];
            // cCPU.SpeedMHz = (uint)cpu["MaxClockSpeed"];
            // cCPU.BusSpeedMHz = (uint)cpu["ExtClock"];
            // cCPU.L2Cache = (uint)cpu["L2CacheSize"] * (ulong)1024;
            // cCPU.L3Cache = (uint)cpu["L3CacheSize"] * (ulong)1024;
            // cCPU.Cores = (uint)cpu["NumberOfCores"];
            // cCPU.Threads = (uint)cpu["NumberOfLogicalProcessors"];
            c.CPU = new List<ClientCPU>(){cCPU};

            // var js = JsonConvert.SerializeObject(c);
            // Debug.WriteLine(js);
            return c;
        }
    }
}