using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Memories;

namespace SRUL
{
    using System.Data;
    using System.Windows.Forms;

    public class UnitEditor
    {
        //        public Tuple<T1>

        public DataTable unitTable = new DataTable();
        public DataColumn unitColumn;
        public DataRow unitRow;
        public VariableStore variableStore = new VariableStore();
        public Meme mem = new Meme();
        DataGridTableStyle ts = new DataGridTableStyle();

        public DataTable Table()
        {
            this.unitTable.Columns.AddRange(new DataColumn[]
                                                {
                                                    new DataColumn("statName"),
                                                    new DataColumn("statValue", typeof(string)),
                                                    new DataColumn("statState", typeof(bool)),
                                                    new DataColumn("statDescription"),
                                                    new DataColumn("statValueType"),
                                                    new DataColumn("statVar", typeof(string)),
                                                });
            //            this.unitTable.Columns.Add("Stat Name", typeof(string));
            //            this.unitTable.Columns.Add("Stat Value");
            //            this.unitTable.Columns.Add("F", typeof(bool));
            //            this.unitTable.Columns.Add("Stat Description", typeof(string));
            unitTable.Columns[5].ColumnMapping = MappingType.Hidden;
            unitTable.AcceptChanges();

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "ID";
            this.unitRow["statValue"] = "Not Loaded";
            this.unitRow["statVar"] = variableStore.UnitID;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "Unit ID";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Name";
            this.unitRow["statValue"] = "Not Loaded";
            this.unitRow["statVar"] = variableStore.UnitName;
            this.unitRow["statState"] = false; 
            this.unitRow["statDescription"] = "Unit Name";
            this.unitRow["statValueType"] = "string";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Year";
            this.unitRow["statValue"] = this.mem.ReadByte(this.variableStore.UnitYear);
            this.unitRow["statVar"] = variableStore.UnitYear;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "Unit Year";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Crew"; 
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitCrew);
            this.unitRow["statVar"] = variableStore.UnitCrew;
            this.unitRow["statState"] = false; 
            this.unitRow["statDescription"] = "Unit Crew";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Weight";
            this.unitRow["statValue"] = this.mem.ReadFloat(this.variableStore.UnitWeight);
            this.unitRow["statVar"] = variableStore.UnitWeight;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "Unit Weight";
            this.unitRow["statValueType"] = "float";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Cargo Capacity";
            this.unitRow["statValue"] = this.mem.ReadFloat(this.variableStore.UnitCargoCapacity);
            this.unitRow["statVar"] = variableStore.UnitCargoCapacity;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "As stated!";
            this.unitRow["statValueType"] = "float";
            this.unitTable.Rows.Add(this.unitRow);
            this.unitRow = this.unitTable.NewRow();

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Max Missile Size";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitMissileSize);
            this.unitRow["statVar"] = variableStore.UnitMissileSize;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "As stated!";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);
            this.unitRow = this.unitTable.NewRow();

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Carrier Size";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitCarrierCapacity);
            this.unitRow["statVar"] = variableStore.UnitCarrierCapacity;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "As stated!";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);
            this.unitRow = this.unitTable.NewRow();

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Initiative";
            this.unitRow["statValue"] = this.mem.ReadByte(this.variableStore.UnitReactionTime);
            this.unitRow["statVar"] = variableStore.UnitReactionTime;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "(Reaction time) between low (1) to high (8)";
            this.unitRow["statValueType"] = "byte";
            this.unitTable.Rows.Add(this.unitRow);
            this.unitRow = this.unitTable.NewRow();

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Build Time";
            this.unitRow["statValue"] = this.mem.ReadFloat(this.variableStore.UnitBuildTime);
            this.unitRow["statVar"] = variableStore.UnitBuildTime;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "Unit Crew";
            this.unitRow["statValueType"] = "float";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Profile Stealth";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitProfileStealth);
            this.unitRow["statVar"] = variableStore.UnitProfileStealth;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "Unit Crew";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Spotting";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitSpotting);
            this.unitRow["statVar"] = variableStore.UnitSpotting;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "Represents the distance that a unit is capable of spotting. Depending on the stealth, terrain and actions of any units with in that range you may not see every unit.";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Move Speed";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitMoveSpeed);
            this.unitRow["statVar"] = variableStore.UnitMoveSpeed;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "Unit Crew";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Move Range";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitMoveRange);
            this.unitRow["statVar"] = variableStore.UnitMoveRange;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "This reports the distance that a unit can travel on a full load of fuel.";
            this.unitRow["statValueType"] = "float";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Capacity Fuel";
            this.unitRow["statValue"] = this.mem.ReadFloat(this.variableStore.UnitFuelCapacity);
            this.unitRow["statVar"] = variableStore.UnitFuelCapacity;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "Volume of a fuel carried";
            this.unitRow["statValueType"] = "float";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Capacity Supplies";
            this.unitRow["statValue"] = this.mem.ReadFloat(this.variableStore.UnitSuppliesCapacity);
            this.unitRow["statVar"] = variableStore.UnitSuppliesCapacity;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "The ammunition held in a squad.";
            this.unitRow["statValueType"] = "float";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Ammo Weight";
            this.unitRow["statValue"] = this.mem.ReadFloat(this.variableStore.UnitAmmoWeight);
            this.unitRow["statVar"] = variableStore.UnitAmmoWeight;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "Lower values affect combat times (0.1/0 equals to weight/supplies)";
            this.unitRow["statValueType"] = "float";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Ranged Ground";
            this.unitRow["statValue"] = this.mem.ReadFloat(this.variableStore.UnitRangeGround);
            this.unitRow["statVar"] = variableStore.UnitRangeGround;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "Range of Ground Attack";
            this.unitRow["statValueType"] = "float";
            this.unitTable.Rows.Add(this.unitRow);
            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Ranged Air";
            this.unitRow["statValue"] = this.mem.ReadFloat(this.variableStore.UnitRangeAir);
            this.unitRow["statVar"] = variableStore.UnitRangeAir;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "Range of Air Attack";
            this.unitRow["statValueType"] = "float";
            this.unitTable.Rows.Add(this.unitRow);
            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Ranged Surface";
            this.unitRow["statValue"] = this.mem.ReadFloat(this.variableStore.UnitRangeNaval);
            this.unitRow["statVar"] = variableStore.UnitRangeNaval;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "Range of Naval Attack";
            this.unitRow["statValueType"] = "float";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Attack Fortification";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitFortAttack);
            this.unitRow["statVar"] = variableStore.UnitFortAttack;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "The strength and range of a unit when attacking facilities,complexes and bridges.";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Attack Soft";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitSoftAttack);
            this.unitRow["statVar"] = variableStore.UnitSoftAttack;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "The strength and range of a unit when attacking soft targets.(generally any unarmored land unit)";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Attack Hard";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitHardAttack);
            this.unitRow["statVar"] = variableStore.UnitHardAttack;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "The strength and range of a unit when attacking hard targets.(generally any armored land unit)";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Attack Close/Sub";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitCloseAttack);
            this.unitRow["statVar"] = variableStore.UnitCloseAttack;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "The strength and range of a unit when attacking submarines. / The attack strength of a unit when in close combat conditions.(generally cities and dense forests)";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Attack Close Air";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitCloseAirAttack);
            this.unitRow["statVar"] = variableStore.UnitCloseAirAttack;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "The strength and range of a unit when attacking Low flying air targets.(helicopters, some missiles, and some planes during attack runs)";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Attack Mid Air";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitMidairAttack);
            this.unitRow["statVar"] = variableStore.UnitMidairAttack;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "The strength and range of a unit when Mid altitude air targets. (most planes)";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Attack High Air";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitHighairAttack);
            this.unitRow["statVar"] = variableStore.UnitHighairAttack;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "The strength and range of a unit when attacking high altitude air targets.(Some missiles and planes)";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Attack Surface";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitSurfaceAttack);
            this.unitRow["statVar"] = variableStore.UnitSurfaceAttack;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "The strength and range of a unit when attacking Surface naval vessels";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Defense Close";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitDefenseClose);
            this.unitRow["statVar"] = variableStore.UnitDefenseClose;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "The defensive strength of a unit in close combat.";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Defense Ground";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitDefenseGround);
            this.unitRow["statVar"] = variableStore.UnitDefenseGround;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "The defensive strength of a unit when attacked in a form other than close combat, indirect, or air.";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Defense Air";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitDefenseAir);
            this.unitRow["statVar"] = variableStore.UnitDefenseAir;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "The defensive strength of a unit when attacked from the air.";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

            this.unitRow = this.unitTable.NewRow();
            this.unitRow["statName"] = "Defense Indirect";
            this.unitRow["statValue"] = this.mem.Read2Byte(this.variableStore.UnitDefenseIndirect);
            this.unitRow["statVar"] = variableStore.UnitDefenseIndirect;
            this.unitRow["statState"] = false;
            this.unitRow["statDescription"] = "The defensive strength of a unit when attacked with indirect fire.(artillery, some-naval ships,missiles and bombers)";
            this.unitRow["statValueType"] = "int";
            this.unitTable.Rows.Add(this.unitRow);

//            this.unitTable.Rows.Add("UnitCrew", 0, false, 0);
//            this.unitTable.Rows.Add("UnitWeight", 0, false, 0);
//            this.unitTable.Rows.Add("UnitProfileStealth", 0, false, 0);
//            this.unitTable.Rows.Add("UnitSpotting", 0, false, 0);
//            this.unitTable.Rows.Add("UnitMoveSpeed", 0, false, 0);
//            this.unitTable.Rows.Add("UnitMoveRange", 0, false, 0);
//            this.unitTable.Rows.Add("UnitFuelCapacity", 0, false, 0);
//            this.unitTable.Rows.Add("UnitSupplyCapacity", 0, false, 0);
//            this.unitTable.Rows.Add("UnitFortAttack", 0, false, 0);
//            this.unitTable.Rows.Add("UnitSoftAttack", 0, false, 0);
//            this.unitTable.Rows.Add("UnitHardAttack", 0, false, 0);
//            this.unitTable.Rows.Add("UnitCloseAttack", 0, false, 0);
//            this.unitTable.Rows.Add("UnitCloseAirAttack", 0, false, 0);
//            this.unitTable.Rows.Add("UnitMidairAttack", 0, false, 0);
//            this.unitTable.Rows.Add("UnitHighairAttack", 0, false, 0);
//            this.unitTable.Rows.Add("UnitSurfaceAttack", 0, false, 0);
//            this.unitTable.Rows.Add("UnitDefenseClose", 0, false, 0);
//            this.unitTable.Rows.Add("UnitDefenseGround", 0, false, 0);
//            this.unitTable.Rows.Add("UnitDefenseAir", 0, false, 0);
//            this.unitTable.Rows.Add("UnitDefenseIndirect", 0, false, 0);

            this.unitTable.RowChanging += new DataRowChangeEventHandler(Row_Changing);
            this.unitTable.RowChanged += new DataRowChangeEventHandler(Row_Changing);

            return this.unitTable;
        }

        private dynamic readInitialData(string type, string varName)
        {
            var m = this.mem;
            switch (type)
            {
                case "float":
                    return m.ReadFloat(varName).ToString();
                case "int":
                    return m.Read2Byte(varName);
                case "2byte":
                    return m.Read2Byte(varName);
                case "byte":
                    return m.ReadByte(varName);
                case "string":
                    return m.ReadString(varName);
                default:
                    return m.ReadFloat(varName);
            }
        }

//        private Form1 frm1;

        private void Row_Changing(object sender, DataRowChangeEventArgs e)
        {
//            Console.WriteLine("Row_Changing Event: name={0}; action={1}",
//                e.Row["statName"], e.Action);
//            var history = "Action: " + e.Action;
//            var rowHistory = "Stat: " + e.Row["statName"];
//            var rowValue = this.readInitialData(e.Row["statValueType"].ToString(), e.Row["statVar"].ToString());
//            ActionHistory.addToHistory("0");
//            this.frm1.addAsAction(ActionHistory.HistoryList.ToArray());
        }

        private void Row_Changed(object sender, DataRowChangeEventArgs e)
        {
//            DataGridViewCell dataGridViewCell = e.Row;
//            if (dataGridViewCell is DataGridViewCheckBoxCell)
//            {
//                this.unitTable.Rows[e.RowIndex].Frozen = (bool)dataGridViewCell.Value;
//            }
        }

        public void liveTable()
        {

//            this.unitTable.Rows[0][1] = this.mem.Read2Byte(this.variableStore.UnitCrew);
//            this.unitTable.Rows[1][1] = this.mem.Read2Byte(this.variableStore.UnitDefenseAir);
//            this.unitTable.Rows[2][1] = 231;
        }
    }

}
