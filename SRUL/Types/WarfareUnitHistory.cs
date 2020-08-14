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

    public static class NumericExtension
    {
        public static int SafeIntDivision(this int Numerator, int Denominator)
        {
            return (Denominator == 0) ? 0 : Numerator / Denominator;
        }
        
        public static string SafePercentage(this decimal current, decimal total)
        {
            return (total == 0) ? "0" : (current / total).ToString("P1");
        }
    }

    public static class SystemExtension
    {
        public static T SerializeCloneMethod<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
        
        public static IList<T> Clone<T>(this IList<T> listToClone) where T:ICloneable {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
        
        public static IList<SRUL.Feature> CloneFeatures<T>(this IList<SRUL.Feature> source)
        {
            IList<Feature> newClone = new List<Feature>();
            foreach (var clone in source)
            {
                newClone.Add(clone.ShallowCopy());
            }
            return newClone;
        }
        public static bool IsBetween(this double value, double minimum, double maximum)
        {
            return value > minimum && value < maximum;
        }

        public static double StrToDouble(this string value)
        {
            return Convert.ToDouble(value);
        }

        public static decimal StrToDecimal(this string value)
        {
            return String.IsNullOrEmpty(value) ? 0 : Convert.ToDecimal(value);
        }
        
        public static int StrToInt(this string value)
        {
            int i;
            if (!int.TryParse(value, out i)) i = 0;
            // return Convert.ToInt32(Convert.ToDecimal(value));
            return i;
        }

        public static object gvGetRowCellValue(this GridView gv, string fieldName, string varName)
        {
            object val = "0";
            for (int i = 0; i < gv.RowCount; i++)
            {
                var name = gv.GetRowCellValue(i, gv.Columns["name"]);
                if (varName != name) continue;
                val = gv.GetRowCellValue(i, fieldName);
                break;
            }
            return val;
        }
        public static void gvSetRowCellValue(this GridView gv, string fieldName, string varName, string value, bool setSource = false)
        {
            var c = gv.DataController.ListSourceRowCount;
            object getCellValue(int id, string ss) => gv.GetListSourceRowCellValue(id, gv.Columns[ss]);
            for (var i = 0; i < c; i++)
            {
                if (!string.Equals(varName, getCellValue(i, "name").ToString(), StringComparison.CurrentCultureIgnoreCase)) continue;
                if (setSource)
                    SRMain.Instance.FeatureByCategoryAndName(getCellValue(i, "category").ToString(),
                        getCellValue(i, "name").ToString()).value = value;
                else
                {
                    gv.SetRowCellValue(i, gv.Columns["value"], value); gv.PostEditor();
                }
                break;
                // return true;
            }
            // return false;
        }
        public static bool gvSetFreezeRowCellValue(this GridView gv, string varName, bool value)
        {
            for (int i = 0; i < gv.DataRowCount; i++)
            {
                var name = gv.GetRowCellValue(i, gv.Columns["name"]);
                if (varName != name as string) continue;
                gv.SetRowCellValue(i, "freeze", value);
                return true;
            }

            return false;
        }
    }

    public class UnitHistory
    {
        public string UnitID { get; set; }
        public string UnitName { get; set; }
        public IList<Feature> UnitStats { get; set; }

        public UnitHistory(IList<Feature> ustats)
        {
            var lqName = ustats.First(s => s.name == "UnitName").value;
            var lqId = ustats.First(s => s.name == "UnitID").value;
            
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

        public void AddIfNotExists(IList<Feature> f)
        {
            if (!jr.activeTrainer.GameValidated) return;
            // string unitId = jr.getUnitId() ?? throw new ArgumentNullException("jr.getUnitId()");
            string uname = jr.getUnitName(f) ?? throw new ArgumentNullException("jr.getUnitName()");
            if (UnitCheck(uname)) return;
            if (UnitList == null)
            {
                UnitList = new List<UnitHistory>{ 
                    new UnitHistory(new List<Feature>(f))
                };                
            }

            if (!UnitList.Exists(unit => unit.UnitName == uname && unit.UnitName != String.Empty) ) {
                UnitList.Add(new UnitHistory(new List<Feature>(f)));
            }
            
        }

        public IList<Feature> GetUnitOriginalValueByName(string uname)
        {
            if (UnitList == null) return null;
            return UnitList.Any(s => s.UnitName == uname) ? UnitList.First(s => s.UnitName == uname).UnitStats : null;
        }

        public static UnitHistoryList Instance
        {
            get { return _unitHistory.Value; }
        }
    }
}
