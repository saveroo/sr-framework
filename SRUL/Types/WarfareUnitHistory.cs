using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;

namespace SRUL.Types
{
    public class WarfareUnitHistory
    {
        public static WarfareUnitHistory CreateInstance()
        {
            return new WarfareUnitHistory();
        }

        public int UnitID { get; set; }
        public string UnitName { get; set; }
        public int UnitYear { get; set; }
        public float UnitPrice { get; set; }
        public double UnitMilitaryGoodsNeeded { get; set; }
        public float UnitAnnualMaintenanceCost { get; set; }
        public int UnitUraniumCost { get; set; }
        public float UnitSupplyLevel { get; set; }
        public int UnitClass { get; set; }
        public int UnitMovementType { get; set; }
        public int UnitTargetType { get; set; }
        public int UnitFortAttack { get; set; }
        public int UnitSoftAttack { get; set; }
        public int UnitHardAttack { get; set; }
        public int UnitCloseAttack { get; set; }
        public int UnitCloseAirAttack { get; set; }
        public int UnitMidairAttack { get; set; }
        public int UnitHighairAttack { get; set; }
        public int UnitSurfaceAttack { get; set; }
        
        public int UnitDefenseClose { get; set; }
        public int UnitDefenseGround { get; set; }
        public int UnitDefenseAir { get; set; }
        public int UnitDefenseIndirect { get; set; }
        
        public int UnitCrew { get; set; }
        public float UnitWeight { get; set; }
        public float UnitCargoCapacity { get; set; }
        public int UnitReactionTime { get; set; }
        
        public int UnitBuildTime { get; set; }
        public int UnitModelCode { get; set; }
        public int UnitProfileStealth { get; set; }
        public int UnitSpotting { get; set; }
        
        public int UnitMoveSpeed { get; set; }
        public float UnitMoveRange { get; set; }
        public float UnitFuelCapacity { get; set; }
        public float UnitSuppliesCapacity { get; set; }
        public float UnitAmmoWeight { get; set; }
        
        public int UnitCarrierCapacity { get; set; }
        public int UnitMissileSize { get; set; }
        
        public int UnitTechReqID1 { get; set; }
        public int UnitTechReqID2 { get; set; }
        
        public float UnitRangeGround { get; set; }
        public float UnitRangeNaval { get; set; }
        public float UnitRangeAir { get; set; }
        
    }
    
    public class UnitHistory
    {
        public string UnitID { get; set; }
        public string UnitName { get; set; }
        public IList<Feature> UnitStats { get; set; }

        public UnitHistory(IList<Feature> ustats)
        {
            var lqName = "UnitName".GetFeature().value;
            var lqId = "UnitID".GetFeature().value;
            
            UnitID = lqId;
            UnitName = lqName;
            UnitStats = ustats.Clone();
        }
    }

    public class UnitHistoryList
    {
        private static readonly Lazy<UnitHistoryList> _unitHistory = new Lazy<UnitHistoryList>(() => new UnitHistoryList());
        public List<UnitHistory> UnitList { get; set; }
        // private ReadWrite rw = Loader.Rw;
        private readonly SRMain jr = SRMain.Instance;
        public UnitHistoryList()
        {
            // this.Add(); 
            Console.WriteLine("Created UnitHistoryList");
            // unitIdList = new List<>();
        }

        private bool UnitCheck(string uname)
        {
            return uname == "" || uname == null;
        }

        public bool AddIfNotExists(IList<Feature> f)
        {
            if (!jr.activeTrainer.GameValidated) return false;
            // string unitId = jr.getUnitId() ?? throw new ArgumentNullException("jr.getUnitId()");
            string uname = jr.getUnitName(f) ?? throw new ArgumentNullException("jr.getUnitName()");
            string uid = jr.FeatureIndexedStore["UnitID"].value;
            if (UnitCheck(uname) || uid == "65535" || uid == "-1") return false;
            
            // TODO: V3 Revise
            if (UnitList == null)
            {
                UnitList = new List<UnitHistory>();
                UnitList.Add(new(new List<Feature>(f)));
                return true;
            }
            else if (!UnitList.Exists(unit => unit.UnitName == uname))
            {
                UnitList.Add(new(new List<Feature>(f)));
                return true;
            }

            return false;
        }

        public IList<Feature> GetUnitOriginalValueByName(string uname)
        {
            if (UnitList == null) return null;
            return UnitList.Any(s=> s.UnitName == uname) 
                ? UnitList.Single(s => s.UnitName == uname).UnitStats 
                : null;
        }

        public static UnitHistoryList Instance
        {
            get { return _unitHistory.Value; }
        }
    }
}
