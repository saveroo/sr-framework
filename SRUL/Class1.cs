using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Memory;
namespace SRUL
{
    using System.Data;
    using System.Timers;

    public class VariableStore
    {
        public string DomesticDomesticApproval = "SupremeRulerUltimate.exe+0x00EE4898,0x7ABC";
        public string DomesticMilitaryApproval = "SupremeRulerUltimate.exe+0x00EE4898,0x7AC0";
        public string DomesticTourism = "SupremeRulerUltimate.exe+0x00EE4898,0x7AD4";
        public string DomesticLiteracy = "SupremeRulerUltimate.exe+0x00EE4898,0x7AD8";
        public string DomesticWorldMarketOpinion = "SupremeRulerUltimate.exe+0x00EE4898,0x7AB0"; 
        //outraged, dissaproving, concerned, indifferent, satisfied
        //pleased, delighted
        public string DomesticWorldMarketSubsidyRate = "SupremeRulerUltimate.exe+0x00EE4898,0x7AC8";
        public string DomesticTreatyIntegrity = "SupremeRulerUltimate.exe+0x00EE4898,0x7AB4";
        public string DomesticUnemployment = "SupremeRulerUltimate.exe+0x00EE4898,0x7B10";

        public string CountryPopulation = "SupremeRulerUltimate.exe+0x00EE4898,0x7AF0";
        public string CountryImmigration = "SupremeRulerUltimate.exe+0x00EE4898,0x7B14";
        public string DomesticEmigration = "SupremeRulerUltimate.exe+0x00EE4898,0x7B18";
        public string DomesticBirths = "SupremeRulerUltimate.exe+0x00EE4898,0x7B1C";
        public string DomesticDeaths = "SupremeRulerUltimate.exe+0x00EE4898,0x7B20";

        public string ArmyActiveStaff = "SupremeRulerUltimate.exe+0x00EE4898,0x7B08";
        public string ArmyReserve = "SupremeRulerUltimate.exe+0x00EE4898,0x7B0C";

        public string FinanceTreasury = "SupremeRulerUltimate.exe+0x00EE4898,0x7B30";
        public string FinanceInflation = "SupremeRulerUltimate.exe+0x00EE4898,0x7BF0";
        public string FinanceCreditRating = "SupremeRulerUltimate.exe+0x00EE4898,0x7AD0";
        public string FinanceGDPc = "SupremeRulerUltimate.exe+0x00EE4898,0x7BE0";

        public string ResourceStockAggriculture = "SupremeRulerUltimate.exe+0x00EE4898,0x7D18";
        public string ResourceStockRubber = "SupremeRulerUltimate.exe+0x00EE4898,0x7DF0";
        public string ResourceStockTimber = "SupremeRulerUltimate.exe+0x00EE4898,0x7EC8";
        public string ResourceStockPetroleum = "SupremeRulerUltimate.exe+0x00EE4898,0x7FA0";
        public string ResourceStockCoal = "SupremeRulerUltimate.exe+0x00EE4898,0x8078";
        public string ResourceStockMetalOre = "SupremeRulerUltimate.exe+0x00EE4898,0x8150";
        public string ResourceStockUranium = "SupremeRulerUltimate.exe+0x00EE4898,0x8228";
        public string ResourceStockElectricPower = "SupremeRulerUltimate.exe+0x00EE4898,0x8300";
        public string ResourceStockConsumerGoods = "SupremeRulerUltimate.exe+0x00EE4898,0x83D8";
        public string ResourceStockIndustryGoods = "SupremeRulerUltimate.exe+0x00EE4898,0x84B0";
        public string ResourceStockMilitaryGoods = "SupremeRulerUltimate.exe+0x00EE4898,0x8588";

//        public string[] arrayOfResource = new string[9];

        public string OneDayBuild = "SupremeRulerUltimate.exe+0x00ED3A78,0x74";
        public string OneDayArmy = "SupremeRulerUltimate.exe+00ED6D98,0x7C";

        public string OneDayResearchClick = "SupremeRulerUltimate.exe+00ED70E8,0xAC,0x1C";
        public string OneDayResearchTooltip = "SupremeRulerUltimate.exe+0x86325C,0x24,0x1C";

        public string ResearchEfficiency = "SupremeRulerUltimate.exe+00EE4898,0x7c74";

        public string ArmyUnitName = "SupremeRulerUltimate.exe+EE40EC";
        public string ArmyCurrent = "SupremeRulerUltimate.exe+00ED6E58,0x74";
        public string ArmyActual = "SupremeRulerUltimate.exe+00ED6E58,0x78";
        public string ArmyExperience = "SupremeRulerUltimate.exe+00ED6E58,0x84";
        public string ArmyMorale = "SupremeRulerUltimate.exe+00ED6E58,0x88";
        public string ArmySupply = "SupremeRulerUltimate.exe+00ED6E58,0x68";
        public string ArmyGas = "SupremeRulerUltimate.exe+00ED6E58,0x64";

        public string ArmyKill = "SupremeRulerUltimate.exe+00ED6E58,0xAC";
        public string ArmyEfficiency = "SupremeRulerUltimate.exe+00ED6E58,0x80";

        // Army Logistic

        //Static ARMY Attack

        public string UnitName = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x0,0x0";
        public string UnitClass = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x4"; //byte
        public string UnitMovementType = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0xA"; //bye
        public string UnitTargetType = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x8"; //byte
        public string UnitFortAttack = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x68";
        public string UnitSoftAttack = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x64";
        public string UnitHardAttack = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x66";
        public string UnitCloseAttack = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x74";
        public string UnitCloseAirAttack = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x6A";
        public string UnitMidairAttack = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x6C";
        public string UnitHighairAttack = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x6E";
        public string UnitSurfaceAttack = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x70";

        //Static Army Def
        public string UnitDefenseClose = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x7E";
        public string UnitDefenseGround = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x78";
        public string UnitDefenseAir = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x7A";
        public string UnitDefenseIndirect = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x7C";

        public string UnitCrew = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x16";
        public string UnitWeight = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x3C";
        public string UnitCargoCapacity = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x44";
        public string UnitReactionTime = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0xB";

        public string UnitUraniumCost = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x34";
        public string UnitBuildTime = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x2C"; //Float
        public string UnitModelCode = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0xC2";

        public string UnitProfileStealth = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0xC";
        public string UnitSpotting = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x24";

        public string UnitMoveSpeed = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x20";
        public string UnitMoveRange = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x48";
        public string UnitFuelCapacity = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x58";
        public string UnitSuppliesCapacity = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x60"; // This affect Combat Time

        public string UnitTraitsNBCProtected = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x90"; // This affect Combat Time
        public string UnitTraitsAirdrop = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x92"; // This affect Combat Time


        public string UnitCarrierCapacity = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0xD"; // This affect Combat Time
        public string UnitMissileSize = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x22"; // This affect Combat Time

        public string UnitTechReq1 = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x28"; // This affect Combat Time
        public string UnitTechReq2 = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x2A"; // This affect Combat Time

        public string UnitRangeGround = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x80"; // This affect Combat Time
        public string UnitRangeNaval = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x84"; // This affect Combat Time
        public string UnitRangeAir = "SupremeRulerUltimate.exe+00ED3A88,0x1AC,0x88"; // This affect Combat Time

        //Army - Available Missile
        public string ArmyMissileAvailableStorageQuantity = "SupremeRulerUltimate.exe+00ED6DC4,0x74";
        public string ArmyMissileAvailableCargoQuantity = "SupremeRulerUltimate.exe+00ED6DC8,0x74";
        public string ArmyMissileStrategicPoolAssigned = "SupremeRulerUltimate.exe+00ED6DE8,0x74";
        public string ArmyMissileStrategicPoolReserve = "SupremeRulerUltimate.exe+00ED6DC0,0x74";

        //Satelite
        public string SatelliteCommCoverage = "SupremeRulerUltimate.exe+00EE4898,0x8834";
        public string SatelliteReconCoverage = "SupremeRulerUltimate.exe+00EE4898,0x8838";
        public string SatelliteMissileDefenseCoverage = "SupremeRulerUltimate.exe+00EE4898,0x883C";


        public string SocialSpendingCultureSubsidy = "SupremeRulerUltimate.exe+0x00EE4898,0x878C";
        public string SocialSpendingEducation = "SupremeRulerUltimate.exe+0x00EE4898,0x8778";
        public string SocialSpendingEnvironment = "SupremeRulerUltimate.exe+0x00EE4898,0x8780";
        public string SocialSpendingFamilySubsidy = "SupremeRulerUltimate.exe+0x00EE4898,0x8784";
        public string SocialSpendingHealthCare = "SupremeRulerUltimate.exe+0x00EE4898,0x8774";
        public string SocialSpendingInfrastructure = "SupremeRulerUltimate.exe+0x00EE4898,0x877C";
        public string SocialSpendingLawEnforcement = "SupremeRulerUltimate.exe+0x00EE4898,0x8788";
        public string SocialSpendingSocialAssistance = "SupremeRulerUltimate.exe+0x00EE4898,0x8790";

        public Dictionary<int, string> DictUnitMovementType = new Dictionary<int, string>();
        public Dictionary<int, string> DictUnitTargetType = new Dictionary<int, string>();
        public Dictionary<int, string> DictUnitClassType = new Dictionary<int, string>();
        public Dictionary<string, string> DictOfResource = new Dictionary<string, string>();
        public Dictionary<string, int> DictOfUnitTraits = new Dictionary<string, int>();

        public DataTable ResourceTable = new DataTable();
        public Mem mem = new Mem();

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
            DictUnitMovementType.Add(14, "Internal Use 14");
            DictUnitMovementType.Add(15, "Internal Use 15");

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
            this.ResourceRow["resourceValue"] = 0;
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "Coal";
            this.ResourceRow["resourceValue"] = this.mem.readFloat(this.ResourceStockCoal);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "Rubber";
            this.ResourceRow["resourceValue"] = this.mem.readFloat(this.ResourceStockRubber);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "MetalOre";
            this.ResourceRow["resourceValue"] = this.mem.readFloat(this.ResourceStockMetalOre);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "Petroleum";
            this.ResourceRow["resourceValue"] = this.mem.readFloat(this.ResourceStockPetroleum);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "Timber";
            this.ResourceRow["resourceValue"] = this.mem.readFloat(this.ResourceStockTimber);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "Uranium";
            this.ResourceRow["resourceValue"] = this.mem.readFloat(this.ResourceStockUranium);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "ElectricPower";
            this.ResourceRow["resourceValue"] = this.mem.readFloat(this.ResourceStockElectricPower);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "ConsumerGoods";
            this.ResourceRow["resourceValue"] = this.mem.readFloat(this.ResourceStockConsumerGoods);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);

            this.ResourceRow = this.ResourceTable.NewRow();
            this.ResourceRow["resourceName"] = "IndustryGoods";
            this.ResourceRow["resourceValue"] = this.mem.readFloat(this.ResourceStockIndustryGoods);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);
            this.ResourceRow = this.ResourceTable.NewRow();

            this.ResourceRow["resourceName"] = "MilitaryGoods";
            this.ResourceRow["resourceValue"] = this.mem.readFloat(this.ResourceStockMilitaryGoods);
            this.ResourceRow["resourceState"] = false;
            this.ResourceRow["resourceValueType"] = "float";
            this.ResourceTable.Rows.Add(this.ResourceRow);
        }

        public float readFloat(String address)
        {

            isStopped = false;
            aTimer = new Timer(2000);
            float result = 0;
            aTimer.Elapsed += (sender, e) =>
                {
                    Console.WriteLine("reading"+address);
                   result = this.mem.readFloat(address);
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
