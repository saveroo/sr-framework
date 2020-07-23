using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Memories;

namespace SRUL
{
    using System.Data;
    using System.Timers;

    public class VariableStore
    {
        private static string ModuleName = "SupremeRulerUltimate.exe";
        private static string ptrResDomSocEffEtc = $"{ModuleName}+0x0104D12C";
        private static string ptrArmyName = $"{ModuleName}+0xEE40EC";
        private static string ptrUnitRest = $"{ModuleName}+0x01033CB4";
        private static string ptrMissAvailStorageQuantity = $"{ModuleName}+0x010370E0";
        private static string ptrMissAvailCargoAssigned = $"{ModuleName}+0x010370E8";
        private static string ptrMissStratPoolAssigned = $"{ModuleName}+0x01037128";
        private static string ptrMissStratPoolReserve = $"{ModuleName}+0x010370D8";
        private static string ptr1DayBuild = $"{ModuleName}+0x01033CA0";
        private static string ptr1DayArmy = $"{ModuleName}+0x01037088";
        private static string ptr1DayResClick = $"{ModuleName}+0x0128027C";
        private static string ptr1DayResToolt = $"{ModuleName}+0x009C5430";
        private static string ptrUnitID = $"{ModuleName}+0x1038358";
        private static string ptrUnitReserve = $"{ModuleName}+0x1036D48";
        private static string ptrUnitDeployed = $"{ModuleName}+0x1036D44";
        private static string ptrUnitBattleGroup = $"{ModuleName}+0x1036D44";
        private static string ptrUnitSelected = $"{ModuleName}+0x1036D44";

        public string DomesticDomesticApproval = $"{ptrResDomSocEffEtc},0x7ABC";
        public string DomesticMilitaryApproval = $"{ptrResDomSocEffEtc},0x7AC0";
        public string DomesticTourism = $"{ptrResDomSocEffEtc},0x7AD4";
        public string DomesticLiteracy = $"{ptrResDomSocEffEtc},0x7AD8";
        public string DomesticWorldMarketOpinion = $"{ptrResDomSocEffEtc},0x7AB0"; 
        //outraged, dissaproving, concerned, indifferent, satisfied
        //pleased, delighted
        public string DomesticWorldMarketSubsidyRate = $"{ptrResDomSocEffEtc},0x7AC8";
        public string DomesticTreatyIntegrity = $"{ptrResDomSocEffEtc},0x7AB4";
        public string DomesticUnemployment = $"{ptrResDomSocEffEtc},0x7B10";

        public string CountryPopulation = $"{ptrResDomSocEffEtc},0x7AF0";
        public string CountryImmigration = $"{ptrResDomSocEffEtc},0x7B14";
        public string CountryEmigration = $"{ptrResDomSocEffEtc},0x7B18";
        public string CountryBirths = $"{ptrResDomSocEffEtc},0x7B1C";
        public string CountryDeaths = $"{ptrResDomSocEffEtc},0x7B20";

        public string ArmyActiveStaff = $"{ptrResDomSocEffEtc},0x7B08"; // read only
        public string ArmyReserve = $"{ptrResDomSocEffEtc},0x7B0C";

        // Read Only
        public string UnitReserve = $"{ModuleName}+1036D48";
        public string UnitDeployed = $"{ModuleName}+1036D44";
        public string UnitBattleGroup = $"{ModuleName}+1036D4C";
        public string UnitSelected = $"{ModuleName}+1036D40";

        public string FinanceTreasury = $"{ptrResDomSocEffEtc},0x7B30";
        public string FinanceInflation = $"{ptrResDomSocEffEtc},0x7BF0";
        public string FinanceCreditRating = $"{ptrResDomSocEffEtc},0x7AD0";
        public string FinanceGDPc = $"{ptrResDomSocEffEtc},0x7BE0";

        public string ResourceStockAggriculture = $"{ptrResDomSocEffEtc},0x7D18";
        public string ResourceStockRubber = $"{ptrResDomSocEffEtc},0x7DF0";
        public string ResourceStockTimber = $"{ptrResDomSocEffEtc},0x7EC8";
        public string ResourceStockPetroleum = $"{ptrResDomSocEffEtc},0x7FA0";
        public string ResourceStockCoal = $"{ptrResDomSocEffEtc},0x8078";
        public string ResourceStockMetalOre = $"{ptrResDomSocEffEtc},0x8150";
        public string ResourceStockUranium = $"{ptrResDomSocEffEtc},0x8228";
        public string ResourceStockElectricPower = $"{ptrResDomSocEffEtc},0x8300";
        public string ResourceStockConsumerGoods = $"{ptrResDomSocEffEtc},0x83D8";
        public string ResourceStockIndustryGoods = $"{ptrResDomSocEffEtc},0x84B0";
        public string ResourceStockMilitaryGoods = $"{ptrResDomSocEffEtc},0x8588";

//        public string[] arrayOfResource = new string[9];

//        public string OneDayBuild = "SupremeRulerUltimate.exe+0x00ED3A78,0x74";"SupremeRulerUltimate.exe"+01033CA0
        public string OneDayBuild = $"{ptr1DayBuild},0x74";
//        public string OneDayArmy = "SupremeRulerUltimate.exe+00ED6D98,0x7C"; // Old
        public string OneDayArmy = $"{ptr1DayArmy},0x7C";

//        public string OneDayResearchClick = "SupremeRulerUltimate.exe+00ED70E8,0xAC,0x1C"; //old pointer version for references
        public string OneDayResearchClick = $"{ptr1DayResClick},0xBC,0x1C";
//        public string OneDayResearchTooltip = "SupremeRulerUltimate.exe+0x86325C,0x24,0x1C"; //Old value for references
        public string OneDayResearchTooltip = $"{ptr1DayResToolt},0x28,0x1C";

//        public string ResearchEfficiency = "SupremeRulerUltimate.exe+00EE4898,0x7c74"; // old
        public string ResearchEfficiency = $"{ptrResDomSocEffEtc},0x7c74";

        public string ArmyUnitName = "SupremeRulerUltimate.exe+EE40EC";
        public string ArmyCurrent = $"{ptrUnitRest},0x74";
        public string ArmyActual = $"{ptrUnitRest},0x78";
        public string ArmyExperience = $"{ptrUnitRest},0x84";
        public string ArmyMorale = $"{ptrUnitRest},0x88";
        public string ArmySupply = $"{ptrUnitRest},0x68";
        public string ArmyGas = $"{ptrUnitRest},0x64";

        public string ArmyKill = $"{ptrUnitRest},0xAC";
        public string ArmyEfficiency = $"{ptrUnitRest},0x80";

        // Army Logistic

        //Static ARMY Attack
        public string UnitID = "SupremeRulerUltimate.exe+1038358";
//        public string UnitName = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x0,0x0";
        public string UnitName = $"{ptrUnitRest},0x1AC,0x0,0x0";
        public string UnitYear = $"{ptrUnitRest},0x1AC,0x6";

        public string UnitClass = $"{ptrUnitRest},0x1AC,0x4"; //byte
        public string UnitMovementType = $"{ptrUnitRest},0x1AC,0xA"; //bye
        public string UnitTargetType = $"{ptrUnitRest},0x1AC,0x8"; //byte
        public string UnitFortAttack = $"{ptrUnitRest},0x1AC,0x68";
        public string UnitSoftAttack = $"{ptrUnitRest},0x1AC,0x64";
        public string UnitHardAttack = $"{ptrUnitRest},0x1AC,0x66";
        public string UnitCloseAttack = $"{ptrUnitRest},0x1AC,0x74";
        public string UnitCloseAirAttack = $"{ptrUnitRest},0x1AC,0x6A";
        public string UnitMidairAttack = $"{ptrUnitRest},0x1AC,0x6C";
        public string UnitHighairAttack = $"{ptrUnitRest},0x1AC,0x6E";
        public string UnitSurfaceAttack = $"{ptrUnitRest},0x1AC,0x70";

        //Static Army Def
//        public string UnitDefenseClose = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x7E"; // OLD pointer
        public string UnitDefenseClose = $"{ptrUnitRest},0x1AC,0x7E";
        public string UnitDefenseGround = $"{ptrUnitRest},0x1AC,0x78";
        public string UnitDefenseAir = $"{ptrUnitRest},0x1AC,0x7A";
        public string UnitDefenseIndirect = $"{ptrUnitRest},0x1AC,0x7C";

        public string UnitCrew = $"{ptrUnitRest},0x1AC,0x16";
        public string UnitWeight = $"{ptrUnitRest},0x1AC,0x3C";
        public string UnitCargoCapacity = $"{ptrUnitRest},0x1AC,0x44";
        public string UnitReactionTime = $"{ptrUnitRest},0x1AC,0xB";

        public string UnitUraniumCost = $"{ptrUnitRest},0x1AC,0x34";
        public string UnitBuildTime = $"{ptrUnitRest},0x1AC,0x2C"; //Float
        public string UnitModelCode = $"{ptrUnitRest},0x1AC,0xC2";

        public string UnitProfileStealth = $"{ptrUnitRest},0x1AC,0xC";
        public string UnitSpotting = $"{ptrUnitRest},0x1AC,0x24";

        public string UnitMoveSpeed = $"{ptrUnitRest},0x1AC,0x20";
        public string UnitMoveRange = $"{ptrUnitRest},0x1AC,0x48";
        public string UnitFuelCapacity = $"{ptrUnitRest},0x1AC,0x58";
        public string UnitSuppliesCapacity = $"{ptrUnitRest},0x1AC,0x60"; // This affect Combat Time
        public string UnitAmmoWeight = $"{ptrUnitRest},0x1AC,0x5C"; // This affect Combat Time

        public string UnitTraitsNBCProtected = $"{ptrUnitRest},0x1AC,0x90"; // This affect Combat Time
        public string UnitTraitsAirdrop = $"{ptrUnitRest},0x1AC,0x92"; // This affect Combat Time
        public string UnitTraitsRefittedDesign = $"{ptrUnitRest},0x1AC,1C";
        public string UnitTraitsReplacedDesign = $"{ptrUnitRest},0x1AC,18";

        public string UnitCarrierCapacity = $"{ptrUnitRest},0x1AC,0xD"; // This affect Combat Time
        public string UnitMissileSize = $"{ptrUnitRest},0x1AC,0x22"; // This affect Combat Time

        public string UnitTechReq1 = $"{ptrUnitRest},0x1AC,0x28"; // This affect Combat Time
        public string UnitTechReq2 = $"{ptrUnitRest},0x1AC,0x2A"; // This affect Combat Time

        public string UnitRangeGround = $"{ptrUnitRest},0x1AC,0x80"; // This affect Combat Time
        public string UnitRangeNaval = $"{ptrUnitRest},0x1AC,0x84"; // This affect Combat Time
        public string UnitRangeAir = $"{ptrUnitRest},0x1AC,0x88"; // This affect Combat Time

        //Army - Available Missile
//        public string ArmyMissileAvailableStorageQuantity = "SupremeRulerUltimate.exe+00ED6DC4,0x74";
//        public string ArmyMissileAvailableCargoQuantity = "SupremeRulerUltimate.exe+00ED6DC8,0x74";
//        public string ArmyMissileStrategicPoolAssigned = "SupremeRulerUltimate.exe+00ED6DE8,0x74";
//        public string ArmyMissileStrategicPoolReserve = "SupremeRulerUltimate.exe+00ED6DC0,0x74";
        public string ArmyMissileAvailableStorageQuantity = $"{ptrMissAvailStorageQuantity},0x74";
        public string ArmyMissileAvailableCargoQuantity = $"{ptrMissAvailCargoAssigned},0x74";
        public string ArmyMissileStrategicPoolAssigned = $"{ptrMissStratPoolAssigned},0x74";
        public string ArmyMissileStrategicPoolReserve = $"{ptrMissStratPoolReserve},0x74";

        //Satelite
//        public string SatelliteCommCoverage = "SupremeRulerUltimate.exe+00EE4898,0x8834"; OLD
        public string SatelliteCommCoverage = $"{ptrResDomSocEffEtc},0x8834";
        public string SatelliteReconCoverage = $"{ptrResDomSocEffEtc},0x8838";
        public string SatelliteMissileDefenseCoverage = $"{ptrResDomSocEffEtc},0x883C";


        public string SocialSpendingCultureSubsidy = $"{ptrResDomSocEffEtc},0x878C";
        public string SocialSpendingEducation = $"{ptrResDomSocEffEtc},0x8778";
        public string SocialSpendingEnvironment = $"{ptrResDomSocEffEtc},0x8780";
        public string SocialSpendingFamilySubsidy = $"{ptrResDomSocEffEtc},0x8784";
        public string SocialSpendingHealthCare = $"{ptrResDomSocEffEtc},0x8774";
        public string SocialSpendingInfrastructure = $"{ptrResDomSocEffEtc},0x877C";
        public string SocialSpendingLawEnforcement = $"{ptrResDomSocEffEtc},0x8788";
        public string SocialSpendingSocialAssistance = $"{ptrResDomSocEffEtc},0x8790";

        public Dictionary<int, string> DictUnitMovementType = new Dictionary<int, string>();
        public Dictionary<int, string> DictUnitTargetType = new Dictionary<int, string>();
        public Dictionary<int, string> DictUnitClassType = new Dictionary<int, string>();
        public Dictionary<string, string> DictOfResource = new Dictionary<string, string>();
        public Dictionary<string, int> DictOfUnitTraits = new Dictionary<string, int>();

        public DataTable ResourceTable = new DataTable();
        public Meme mem = new Meme();

        public DataRow ResourceRow;

        private static Timer aTimer;

        private static bool isStopped;


        public VariableStore()
        {

            DictOfUnitTraits.Add("NBCProtected", 16);

            DictUnitClassType.Add(0, "Infantry");
            DictUnitClassType.Add(1, "Recon");
            DictUnitClassType.Add(2, "Tank");
            DictUnitClassType.Add(3, "Anti-Tank");
            DictUnitClassType.Add(4, "Artillery/MLRS/Mortar");
            DictUnitClassType.Add(5, "Air Defense");
            DictUnitClassType.Add(6, "Ground Transport/Bridging");
            DictUnitClassType.Add(7, "Helicopter");
            DictUnitClassType.Add(8, "Missiles");
            DictUnitClassType.Add(9, "Fighter Interceptor");
            DictUnitClassType.Add(10, "Fighter Attack/Bomber");
            DictUnitClassType.Add(11, "Fighter Multi Role");
            DictUnitClassType.Add(12, "Strategic Bomber");
            DictUnitClassType.Add(13, "Patrol/AWACS Fixed Wing/ECM");
            DictUnitClassType.Add(14, "Air Transport/Tanker");
            DictUnitClassType.Add(15, "Submarine");
            DictUnitClassType.Add(16, "Carrier");
            DictUnitClassType.Add(17, "Battleship/Cruiser/Destroyer");
            DictUnitClassType.Add(18, "Frigate/Corvette");
            DictUnitClassType.Add(19, "Patrol/Support ");
            DictUnitClassType.Add(20, "Sea Transport ");
            DictUnitClassType.Add(21, "Facility ");

            DictUnitTargetType.Add(0, "Soft");
            DictUnitTargetType.Add(1, "Hard");
            DictUnitTargetType.Add(2, "Fort");
            DictUnitTargetType.Add(3, "Close Air");
            DictUnitTargetType.Add(4, "Mid Air");
            DictUnitTargetType.Add(5, "High Air");
            DictUnitTargetType.Add(6, "Surface");
            DictUnitTargetType.Add(7, "Submerged");

            DictUnitMovementType.Add(0, "Unmounted");
            DictUnitMovementType.Add(1, "Wheel");
            DictUnitMovementType.Add(2, "Half Track");
            DictUnitMovementType.Add(3, "Tracked");
            DictUnitMovementType.Add(4, "Towed");
            DictUnitMovementType.Add(5, "Variable");
            DictUnitMovementType.Add(6, "Mech");
            DictUnitMovementType.Add(7, "Hover");
            DictUnitMovementType.Add(8, "Unused");
            DictUnitMovementType.Add(9, "Unused");
            DictUnitMovementType.Add(10, "Naval");
            DictUnitMovementType.Add(11, "Unused");
            DictUnitMovementType.Add(12, "Air Close");
            DictUnitMovementType.Add(13, "Air Mission");
//            DictUnitMovementType.Add(14, "Internal Use 14");
//            DictUnitMovementType.Add(15, "Internal Use 15");

            this.DictOfResource.Add("Aggriculture", this.ResourceStockAggriculture);
            this.DictOfResource.Add("Rubber", this.ResourceStockRubber);
            this.DictOfResource.Add("Timber", this.ResourceStockTimber);
            this.DictOfResource.Add("Petroleum", this.ResourceStockPetroleum);
            this.DictOfResource.Add("Coal", this.ResourceStockCoal);
            this.DictOfResource.Add("MetalOre", this.ResourceStockMetalOre);
            this.DictOfResource.Add("Uranium", this.ResourceStockUranium);
            this.DictOfResource.Add("ElectricPower", this.ResourceStockElectricPower);
            this.DictOfResource.Add("ConsumerGoods", this.ResourceStockConsumerGoods);
            this.DictOfResource.Add("IndustryGoods", this.ResourceStockIndustryGoods);
            this.DictOfResource.Add("MilitaryGoods", this.ResourceStockMilitaryGoods);

            this.ResourceTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("resourceName"),
                new DataColumn("resourceValue", typeof(string)),
                new DataColumn("resourceState", typeof(bool)),
                new DataColumn("resourceValueType")
            });

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "Aggriculture";
            this.ResourceRow["resourceValue"] = this.mem.ReadFloat(this.ResourceStockAggriculture);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "double";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "Coal";
            this.ResourceRow["resourceValue"] = this.mem.ReadFloat(this.ResourceStockCoal);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "Rubber";
            this.ResourceRow["resourceValue"] = this.mem.ReadFloat(this.ResourceStockRubber);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "MetalOre";
            this.ResourceRow["resourceValue"] = this.mem.ReadFloat(this.ResourceStockMetalOre);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "Petroleum";
            this.ResourceRow["resourceValue"] = this.mem.ReadFloat(this.ResourceStockPetroleum);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "Timber";
            this.ResourceRow["resourceValue"] = this.mem.ReadFloat(this.ResourceStockTimber);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "Uranium";
            this.ResourceRow["resourceValue"] = this.mem.ReadFloat(this.ResourceStockUranium);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "ElectricPower";
            this.ResourceRow["resourceValue"] = this.mem.ReadFloat(this.ResourceStockElectricPower);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "ConsumerGoods";
            this.ResourceRow["resourceValue"] = this.mem.ReadFloat(this.ResourceStockConsumerGoods);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "double";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "IndustryGoods";
            this.ResourceRow["resourceValue"] = this.mem.ReadFloat(this.ResourceStockIndustryGoods);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "double";
            this.ResourceTable.Rows.Add(this.ResourceRow);
            this.ResourceRow = this.ResourceTable.NewRow();

            this.ResourceRow["resourceName"] = "MilitaryGoods";
            this.ResourceRow["resourceValue"] = this.mem.ReadFloat(this.ResourceStockMilitaryGoods);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "double";
            this.ResourceTable.Rows.Add(this.ResourceRow);
        }

        public float ReadFloat(String address)
        {

            isStopped = false;
            aTimer = new Timer(2000);
            float result = 0;
            aTimer.Elapsed += (sender, e) =>
                {
                   Console.WriteLine("Reading"+address);
                   result = this.mem.ReadFloat(address);
                };
            aTimer.Enabled = true;
            if (isStopped)
            {
                aTimer.Stop();
                aTimer.Dispose();
            }
            return result;
        }
        public class Dictionary<TKey1, TKey2, TValue> : Dictionary<Tuple<TKey1, TKey2>, TValue>, IDictionary<Tuple<TKey1, TKey2>, TValue>
        {

            public TValue this[TKey1 key1, TKey2 key2]
            {
                get { return base[Tuple.Create(key1, key2)]; }
                set { base[Tuple.Create(key1, key2)] = value; }
            }

            public void Add(TKey1 key1, TKey2 key2, TValue value)
            {
                base.Add(Tuple.Create(key1, key2), value);
            }

            public bool ContainsKey(TKey1 key1, TKey2 key2)
            {
                return base.ContainsKey(Tuple.Create(key1, key2));
            }
        }

    }
}
