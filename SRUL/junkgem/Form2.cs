using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Memories;
using Syncfusion.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace SRUL
{
    using System.Runtime.InteropServices;

    using Timer = System.Windows.Forms.Timer;

    public partial class SRULMetroForm : MaterialForm
    {
        public SRULMetroForm()
        {
            InitializeComponent();
        }

        #region DllImports
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, uint vlc);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        protected override void WndProc(ref Message msg)
        {
            if (msg.Msg == 0x0312) //our hot button recognition and function triggering
            {
                int id = msg.WParam.ToInt32();
                if (id == 1)
                { //our designated ID to execute
                  //                    execFunction(); //our function to execute
                }
            }
            base.WndProc(ref msg);
        }

        public Timer timer = new Timer();
        public string codeFile = Application.StartupPath + @"\codes.ini";
        public Meme m = new Meme();
        public bool loaded = false;

        private int[] ArrayOfUnitValue;
        public static VariableStore variableStores = new VariableStore();
        public VariableStore variableStore = variableStores;
        public UnitEditor unitStore = new UnitEditor();
        public string[] ArrayOfVariableStore = new string[29]
                                                {
                                                    variableStores.UnitName,
                                                    variableStores.UnitCrew,
                                                    variableStores.UnitWeight,
                                                    variableStores.UnitCargoCapacity,
                                                    variableStores.UnitMissileSize,
                                                    variableStores.UnitCarrierCapacity,
                                                    variableStores.UnitReactionTime,
                                                    variableStores.UnitBuildTime,
                                                    variableStores.UnitProfileStealth,
                                                    variableStores.UnitSpotting,
                                                    variableStores.UnitMoveSpeed,
                                                    variableStores.UnitMoveRange,
                                                    variableStores.UnitFuelCapacity,
                                                    variableStores.UnitSuppliesCapacity,
                                                    variableStores.UnitRangeGround,
                                                    variableStores.UnitRangeAir,
                                                    variableStores.UnitRangeNaval,
                                                    variableStores.UnitFortAttack,
                                                    variableStores.UnitSoftAttack,
                                                    variableStores.UnitHardAttack,
                                                    variableStores.UnitCloseAttack,
                                                    variableStores.UnitCloseAirAttack,
                                                    variableStores.UnitMidairAttack,
                                                    variableStores.UnitHighairAttack,
                                                    variableStores.UnitSurfaceAttack,
                                                    variableStores.UnitDefenseClose,
                                                    variableStores.UnitDefenseGround,
                                                    variableStores.UnitDefenseAir,
                                                    variableStores.UnitDefenseIndirect,
                                                };
        //        public string[] ArrayOfResource = new string[11]
        //                                               {
        //                                                    variableStores.ResourceStockAggriculture,
        //                                                    variableStores.ResourceStockCoal,
        //                                                    variableStores.ResourceStockTimber,
        //                                                    variableStores.ResourceStockMetalOre,
        //                                                    variableStores.ResourceStockPetroleum,
        //                                                    variableStores.ResourceStockElectricPower,
        //                                                    variableStores.ResourceStockUranium,
        //                                                    variableStores.ResourceStockRubber,
        //                                                    variableStores.ResourceStockConsumerGoods,
        //                                                    variableStores.ResourceStockIndustryGoods,
        //                                                    variableStores.ResourceStockMilitaryGoods,
        //                                               };


        private void Form2_Load(object sender, EventArgs e)
        {
            this.initDataGrid();
            this.timer2.Interval = 2000;
            this.progressBar1.Visible = false;

            //            RegisterHotKey(this.Handle, 1, 0x0002, '1');
            this.populateUnitTraits();

            //0 statName
            //1 StatValue
            //2 statState
            //3 StatDesc
            //4 stat value Type
            this.dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView1.Columns[4].Visible = false;

            this.dataGridView1.Columns[0].ReadOnly = true;
            this.dataGridView1.Rows[0].Cells[1].ReadOnly = true;
            this.dataGridView1.Rows[0].Cells[2].ReadOnly = true;
            this.dataGridView1.Columns[3].ReadOnly = true;


            // DATA GRID - Resource
            this.dataGridView2.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dataGridView2.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dataGridView2.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dataGridView2.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dataGridView2.Columns[3].Visible = false;
            this.dataGridView2.Columns[0].ReadOnly = true;
            ((DataGridViewTextBoxColumn)dataGridView2.Columns[1]).MaxInputLength = 160;



            //            MessageBox.Show(new VariableStore().DictUnitClassType.First().ToString());
            this.comboBox1.DataSource = new BindingSource(new VariableStore().DictUnitClassType, null);
            this.comboBox1.DisplayMember = "Value";
            this.comboBox1.ValueMember = "Key";
            this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            this.comboBox2.DataSource = new BindingSource(new VariableStore().DictUnitMovementType, null);
            this.comboBox2.DisplayMember = "Value";
            this.comboBox2.ValueMember = "Key";
            this.comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;


            this.comboBox3.DataSource = new BindingSource(new VariableStore().DictUnitTargetType, null);
            this.comboBox3.DisplayMember = "Value";
            this.comboBox3.ValueMember = "Key";
            this.comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;


            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
        }


        private void openGame()
        {
            if (loaded)
                return;

            //            int gp = m.GetProcIdFromName("test");
            int gameProcId = m.GetProcIdFromName("SupremeRulerUltimate"); //use task manager to find game name. For CoD MW3 it is iw5sp. Do not add .exe extension

            if (gameProcId != 0)
            {
                label2.Invoke(new MethodInvoker(delegate
                {
                    label2.Text = gameProcId.ToString();
                }));
                m.OpenProcess("SupremeRulerUltimate.exe");
                loaded = true;
            }
            else
            {
                label2.Text = "test";
            }
        }

        private void populateUnitTraits()
        {
            this.checkedListBox3.Items.Add("Trait - NBCProtected", false);
            this.checkedListBox3.Items.Add("Trait - Airdrop", false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process[] myprocesses = System.Diagnostics.Process.GetProcessesByName("SupremeRulerUltimate");

            if (myprocesses.Length != 0)
            {
                MessageBox.Show("Game Loaded!", "Instance found!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Game Not Found!", "No instance found!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            //            while (true) //infinite loop
            //            {
            //               
            //                openGame();
            //
            //                if (!loaded)
            //                    continue;
            //
            ////                this.timer1.Start();
            //             
            //
            ////                checkBox1.Invoke(new MethodInvoker(delegate {
            ////
            ////                    m.WriteMemory("SupremeRulerUltimate.exe+0x00ED3A78,0x74", "float", "0.99");
            ////                    m.WriteMemory("SupremeRulerUltimate.exe+0x00EE4898,0x7D18", "float", "1");
            ////                }));
            //
            //            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            m.WriteMemory("SupremeRulerUltimate.exe+0x00ED3A78,0x74", "float", "0.99");

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            openGame();
            if (!this.loaded) return;


            for (int i = 0; i < this.variableStore.ResourceTable.Rows.Count; i++)
            {
                this.variableStore.ResourceTable.Rows[i][1] = this.m.ReadFloat(this.variableStore.DictOfResource[this.dataGridView2.Rows[i].Cells[0].Value.ToString()]);
                //                    this.dataGridView2.Rows[i].Cells[0].
            }

            if (this.timer2.Enabled)
            {
                this.progressBar1.Maximum = 10;
                if (this.progressBar1.Value < 10) this.progressBar1.Value++;
            }
            else
            {
                this.progressBar1.Value = 0;
            }
            //            this.unitStore.liveTable();

            //            if (!this.dataGridView1.Rows[0].Cells[1].Selected)
            //            {
            //                this.unitStore.unitTable.Rows[0][1] = this.m.Read2Byte(this.variableStore.UnitCrew);
            //            }
            //            else
            //            {
            ////                if(!this.dataGridView1.Rows[0].Cells[1].Value)
            //                this.m.WriteMemory(this.unitStore.variableStore.UnitCrew, "int", this.unitStore.unitTable.Rows[0][1].ToString());
            //                this.label3.Text = this.m.Read2Byte(this.variableStore.UnitCrew).ToString();
            //            }
            //            this.dataGridView1.Rows[0].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitCrew);
            //            this.dataGridView1.Rows[1].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitWeight);
            //            this.dataGridView1.Rows[2].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitProfileStealth);
            //            this.dataGridView1.Rows[3].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitSpotting);
            //            this.dataGridView1.Rows[4].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitMoveSpeed);
            //            this.dataGridView1.Rows[5].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitMoveRange);
            //            this.dataGridView1.Rows[6].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitFuelCapacity);
            //            this.dataGridView1.Rows[7].Cells[1].Value = this.m.ReadFloat(this.variableStore.UnitSuppliesCapacity);
            //            //Attack
            //            this.dataGridView1.Rows[8].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitFortAttack);
            //            this.dataGridView1.Rows[9].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitSoftAttack);
            //            this.dataGridView1.Rows[10].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitHardAttack);
            //            this.dataGridView1.Rows[11].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitCloseAttack);
            //            this.dataGridView1.Rows[12].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitCloseAirAttack);
            //            this.dataGridView1.Rows[13].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitMidairAttack);
            //            this.dataGridView1.Rows[14].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitHighairAttack);
            //            this.dataGridView1.Rows[15].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitSurfaceAttack);
            //
            //            //Defense
            //            this.dataGridView1.Rows[16].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitDefenseClose);
            //            this.dataGridView1.Rows[17].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitDefenseGround);
            //            this.dataGridView1.Rows[18].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitDefenseAir);
            //            this.dataGridView1.Rows[19].Cells[1].Value = this.m.Read2Byte(this.variableStore.UnitDefenseIndirect);


            //            decimal test = (decimal)this.m.ReadFloat("SupremeRulerUltimate.exe+0x00EE4898,0x7D18");
            decimal test = (decimal)this.m.ReadFloat(variableStore.OneDayBuild);

            // 1 Day Build
            if (this.checkedListBox1.GetItemChecked(0))
            {
                if (this.m.ReadFloat(variableStore.OneDayBuild) < 1)
                {
                    this.m.WriteMemory(variableStore.OneDayBuild, "float", "0.99");
                }
            }

            // 1 Day Army
            if (this.checkedListBox1.GetItemChecked(1))
            {
                this.m.WriteMemory(variableStore.OneDayArmy, "float", "0.1");
            }

            // 1 Day Research
            if (this.checkedListBox1.GetItemChecked(2))
            {
                this.m.WriteMemory(this.variableStore.OneDayResearchClick, "float", "1");
                this.m.WriteMemory(this.variableStore.OneDayResearchTooltip, "float", "1");
            }

            //Res Eff
            if (this.cbResearchEfficiency.Checked)
            {
                this.m.WriteMemory(
                    this.variableStore.ResearchEfficiency,
                    "float",
                    this.txtResearchEfficiency.DoubleValue.ToString());
            }
            else
            {
                this.txtResearchEfficiency.DoubleValue = this.m.ReadFloat(this.variableStore.ResearchEfficiency);
            }

            //Army Logic
            this.ArmyLogic();
            //            this.label4.Text = "Unit: " + this.m.ReadString(this.variableStore.ArmyUnitName);
            this.label4.Text = "Unit: " + this.m.ReadString(this.variableStore.UnitName);
            this.label5.Text = "Unit ID: " + this.m.ReadInt(this.variableStore.UnitID);

            //Domestic Finance 
            this.DomesticFinance();

            //World Subsidy
            if (!this.radialSlider1.Capture)
                this.radialSlider1.Value = (m.ReadFloat(this.variableStore.DomesticWorldMarketSubsidyRate) / 1) * 100;

            //World opinon
            if (this.trackBar2.Value <= 10) { this.autoLabel6.Text = "(Outraged)"; }
            if (this.trackBar2.Value >= 20 && this.trackBar2.Value < 30) { this.autoLabel6.Text = "(Dissaproving)"; }
            if (this.trackBar2.Value >= 30 && this.trackBar2.Value < 40) { this.autoLabel6.Text = "(Concerned)"; }
            if (this.trackBar2.Value >= 40 && this.trackBar2.Value < 50) { this.autoLabel6.Text = "(Indifferent)"; }
            if (this.trackBar2.Value >= 50 && this.trackBar2.Value < 60) { this.autoLabel6.Text = "(Satisfied)"; }
            if (this.trackBar2.Value >= 60 && this.trackBar2.Value < 70) { this.autoLabel6.Text = "(Pleased)"; }
            if (this.trackBar2.Value > 70) { this.autoLabel6.Text = "(Delighted)"; }
            if (!this.trackBar2.Capture)
            {
                var worldMarketOpinionValue = (this.m.ReadFloat(this.variableStore.DomesticWorldMarketOpinion) / 1) * 100;
                this.trackBar2.Value = (int)worldMarketOpinionValue;
            }

            //Sattelite
            this.Satellite();

        }

        private bool WorldOpinion()
        {
            if (this.trackBar2.Value <= 10) { return this.m.WriteMemory(this.variableStore.DomesticWorldMarketOpinion, "float", "0.1"); }
            if (this.trackBar2.Value >= 20 && this.trackBar2.Value < 30) { return this.m.WriteMemory(this.variableStore.DomesticWorldMarketOpinion, "float", "0.2"); }
            if (this.trackBar2.Value >= 30 && this.trackBar2.Value < 40) { return this.m.WriteMemory(this.variableStore.DomesticWorldMarketOpinion, "float", "0.3"); }
            if (this.trackBar2.Value >= 40 && this.trackBar2.Value < 50) { return this.m.WriteMemory(this.variableStore.DomesticWorldMarketOpinion, "float", "0.4"); }
            if (this.trackBar2.Value >= 50 && this.trackBar2.Value < 60) { return this.m.WriteMemory(this.variableStore.DomesticWorldMarketOpinion, "float", "0.5"); }
            if (this.trackBar2.Value >= 60 && this.trackBar2.Value < 70) { return this.m.WriteMemory(this.variableStore.DomesticWorldMarketOpinion, "float", "0.7"); }
            if (this.trackBar2.Value > 70) { return this.m.WriteMemory(this.variableStore.DomesticWorldMarketOpinion, "float", "1"); }
            return false;

        }

        private void DomesticFinance()
        {
            if (this.cbDomesticApproval.Checked)
            {
                this.m.WriteMemory(this.variableStore.DomesticDomesticApproval, "float", "1");
            }
            if (this.cbMilitaryApproval.Checked)
            {
                this.m.WriteMemory(this.variableStore.DomesticMilitaryApproval, "float", "1");
            }
            if (this.cbTreatyIntegrity.Checked)
            {
                this.m.WriteMemory(this.variableStore.DomesticTreatyIntegrity, "float", "10");
            }
            if (this.cbLiteracy.Checked)
            {
                this.m.WriteMemory(this.variableStore.DomesticLiteracy, "float", "1");
            }
            if (this.cbUnemployment.Checked)
            {
                this.m.WriteMemory(this.variableStore.DomesticUnemployment, "float", "0");
            }

            if (this.cbArmyStrength.Checked)
            {
                this.m.WriteMemory(this.variableStore.ArmyCurrent, "float", this.txtArmyCurrent.Value.ToString());
            }
            if (this.cbArmyNavalStrength.Checked)
            {
                this.m.WriteMemory(this.variableStore.ArmyCurrent, "float", this.txtArmyHealthNaval.DoubleValue.ToString());
            }
            if (this.cbArmyGas.Checked)
            {
                this.m.WriteMemory(this.variableStore.ArmyGas, "float", this.txtArmyGas.Value.ToString());
            }
            if (this.cbArmySupply.Checked)
            {
                this.m.WriteMemory(this.variableStore.ArmySupply, "float", this.txtArmySupply.Value.ToString());
            }


            //Treasury
            if (this.cbTreasury.Checked) this.m.WriteMemory(this.variableStore.FinanceTreasury, "float", this.txTreasury.ToString());
            if (this.txTreasury.Focused)
            {
                this.m.WriteMemory(this.variableStore.FinanceTreasury, "float", this.txTreasury.Value.ToString());
            }
            else
            {
                //                this.ceTreasury.TextBox.DecimalValue = (decimal)m.ReadFloat(this.variableStore.FinanceTreasury);
                this.txTreasury.Value = (decimal)m.ReadFloat(this.variableStore.FinanceTreasury);
                //                this.ceTreasury.TextBox.va = (decimal)m.ReadFloat(this.variableStore.FinanceTreasury);
            }

            //GDPC
            if (this.cbGDPc.Checked) this.m.WriteMemory(this.variableStore.FinanceGDPc, "float", this.txGDP.Value.ToString());
            if (this.txGDP.Focused)
            {
                this.m.WriteMemory(this.variableStore.FinanceGDPc, "float", this.txGDP.Value.ToString());
            }
            else
            {
                this.txGDP.Value = (decimal)m.ReadFloat(this.variableStore.FinanceGDPc);
            }

            // Inflation
            if (this.txtInflation.Focused)
                this.m.WriteMemory(this.variableStore.FinanceInflation, "float", this.txtInflation.DoubleValue.ToString());
            if (this.cbInflation.Checked)
            {
                this.m.WriteMemory(this.variableStore.FinanceInflation, "float", this.txtInflation.DoubleValue.ToString());
            }
            else
            {
                this.txtInflation.DoubleValue = this.m.ReadFloat(this.variableStore.FinanceInflation);
            }

            // Credit Rating
            if (this.txtCreditRating.Focused)
                this.m.WriteMemory(this.variableStore.FinanceCreditRating, "float", this.txtCreditRating.DoubleValue.ToString());
            if (this.cbCreditRating.Checked)
            {
                this.m.WriteMemory(this.variableStore.FinanceCreditRating, "float", this.txtCreditRating.DoubleValue.ToString());
            }
            else
            {
                this.txtCreditRating.DoubleValue = this.m.ReadFloat(this.variableStore.FinanceCreditRating);
            }

            // Tourism
            if (this.txtTourism.Focused)
                this.m.WriteMemory(this.variableStore.DomesticTourism, "float", this.txtTourism.DoubleValue.ToString());
            if (this.cbTourism.Checked)
            {
                this.m.WriteMemory(this.variableStore.DomesticTourism, "float", this.txtTourism.DoubleValue.ToString());
            }
            else
            {
                this.txtTourism.DoubleValue = this.m.ReadFloat(this.variableStore.DomesticTourism);
            }

        }

        private void Satellite()
        {
            if (this.cbSatelliteCommunicationCoverage.Checked)
            {
                this.m.WriteMemory(this.variableStore.SatelliteCommCoverage, "float", this.txtSatellite.DoubleValue.ToString());
            }
            if (this.cbReconCoverage.Checked)
            {
                this.m.WriteMemory(this.variableStore.SatelliteReconCoverage, "float", this.txtSatellite.DoubleValue.ToString());
            }
            if (this.cbMissileDefenseCoverage.Checked)
            {
                this.m.WriteMemory(this.variableStore.SatelliteMissileDefenseCoverage, "float", this.txtSatellite.DoubleValue.ToString());
            }
        }

        private void initDataGrid()
        {
            this.dataGridView1.DataSource = this.unitStore.Table();
            this.dataGridView2.DataSource = this.variableStore.ResourceTable;
            //            String[] colName = new String[20]
            //                               {
            //                                "UnitCrew",
            //                                "UnitWeight",
            //                                "UnitProfileStealth",
            //                                "UnitSpotting",
            //                                "UnitMoveSpeed",
            //                                "UnitMoveRange",
            //                                "UnitFuelCapacity",
            //                                "UnitSupplyCapacity",
            //                                "UnitFortAttack", "UnitSoftAttack", "UnitHardAttack", "UnitCloseAttack",
            //                                "UnitCloseAirAttack", "UnitMidairAttack", "UnitHighairAttack", "UnitSurfaceAttack",
            //                                "UnitDefenseClose",
            //                                "UnitDefenseGround",
            //                                "UnitDefenseAir",
            //                                "UnitDefenseIndirect",
            //                               };
            //            this.ArrayOfUnitValue = new int[8]
            //                               {
            //                                    this.m.ReadInt(this.variableStore.UnitFortAttack),
            //                                    this.m.ReadInt(this.variableStore.UnitSoftAttack),
            //                                    this.m.ReadInt(this.variableStore.UnitHardAttack),
            //                                    this.m.ReadInt(this.variableStore.UnitCloseAttack),
            //                                    this.m.ReadInt(this.variableStore.UnitCloseAirAttack),
            //                                    this.m.ReadInt(this.variableStore.UnitMidairAttack),
            //                                    this.m.ReadInt(this.variableStore.UnitHighairAttack),
            //                                    this.m.ReadInt(this.variableStore.UnitSurfaceAttack),
            //                               };
            //
            //            for (int i = 0; i < colName.Length; i++)
            //            {
            //                this.dataGridView1.Rows.Add(colName[i], 0, false);
            //
            //            }

            //            //DEF 
            //            String[] defName = new String[4]
            //                               {
            //                                   "UnitDefenseClose",
            //                                    "UnitDefenseGround",
            //                                    "UnitDefenseAir",
            //                                    "UnitDefenseIndirect",
            //                               };
            //            for (int i = 0; i < defName.Length; i++)
            //            {
            //                this.dataGridView2.Rows.Add(defName[i], 0, false);
            //            }
            //            String[] generalName = new String[8]
            //                               {
            //                                "UnitCrew",
            //                                "UnitWeight",
            //                                "UnitProfileStealth",
            //                                "UnitSpotting",
            //                                "UnitMoveSpeed",
            //                                "UnitMoveRange",
            //                                "UnitFuelCapacity",
            //                                "UnitSupplyCapacity",
            //                               };
            //            for (int i = 0; i < generalName.Length; i++)
            //            {
            //                this.dataGridView3.Rows.Add(generalName[i], 0, false);
            //            }

            //            foreach (var colText in colName)
            //            {
            //                this.dataGridView1.Rows.Add(colText, readValue, false);
            //            }
        }

        private void ArmyLogic()
        {
            if (this.cbArmyEfficiency.Checked) this.m.WriteMemory(this.variableStore.ArmyEfficiency, "float", "5");

            if (this.checkedListBox3.GetItemCheckState(0) == CheckState.Checked) this.m.WriteMemory(this.variableStore.UnitTraitsNBCProtected, "byte", "48");
            if (this.checkedListBox3.GetItemCheckState(1) == CheckState.Checked) this.m.WriteMemory(this.variableStore.UnitTraitsAirdrop, "byte", "2");


            if (this.comboBox1.Focused)
                this.m.WriteMemory(this.variableStore.UnitClass, "int", this.comboBox1.SelectedIndex.ToString());
            else
                this.comboBox1.SelectedIndex = this.m.ReadByte(this.variableStore.UnitClass);
            if (this.comboBox2.Focused)
                this.m.WriteMemory(this.variableStore.UnitMovementType, "int", this.comboBox2.SelectedIndex.ToString());
            else
                this.comboBox2.SelectedIndex = this.m.ReadByte(this.variableStore.UnitMovementType);
            if (this.comboBox3.Focused)
                this.m.WriteMemory(this.variableStore.UnitTargetType, "int", this.comboBox3.SelectedIndex.ToString());
            else
                this.comboBox3.SelectedIndex = this.m.ReadByte(this.variableStore.UnitTargetType);


            //Missile
            if (this.cbMissileMultiplier.Checked)
            {
                this.m.WriteMemory(
                    this.variableStore.ArmyMissileAvailableCargoQuantity,
                    "float",
                    this.txtMissileMultiplier.Value.ToString());
                this.m.WriteMemory(
                    this.variableStore.ArmyMissileAvailableStorageQuantity,
                    "float",
                    this.txtMissileMultiplier.Value.ToString());
                this.m.WriteMemory(
                    this.variableStore.ArmyMissileStrategicPoolAssigned,
                    "float",
                    this.txtMissileMultiplier.Value.ToString());
                this.m.WriteMemory(
                    this.variableStore.ArmyMissileStrategicPoolReserve,
                    "float",
                    this.txtMissileMultiplier.Value.ToString());
            }

            // STAFF
            this.txtArmyActive.ReadOnly = true;
            this.txtArmyActive.Text = this.m.ReadFloat(this.variableStore.ArmyActiveStaff).ToString();

            if (!this.txtArmyReserve.Focused)
            {
                this.txtArmyReserve.Value = (decimal)this.m.ReadFloat(this.variableStore.ArmyReserve);
            }

            // Army Morale
            if (this.cbArmyMorale.Checked)
            {
                this.m.WriteMemory(
                    this.variableStore.ArmyMorale,
                    "float",
                    "5");
            }
            //            if (!this.trackBar1.Focused || !this.trackBar1.Capture)
            //            {
            //                var armyMorale = this.m.ReadFloat(this.variableStore.ArmyMorale);
            //                var armytoPercentage = (armyMorale / 1) * 100;
            //                var castedArmyMorale = Math.Round(armytoPercentage);
            ////                this.trackBar1.Value = (int)castedArmyMorale;
            //            }
            //            this.cbArmyMorale.Text = "Morale: " + this.trackBar1.Value + "%";

            // Army - HEAL
            if (this.checkedListBox2.GetItemChecked(0))
            {
                this.m.WriteMemory(variableStore.ArmyCurrent, "float", this.m.ReadFloat(variableStore.ArmyActual).ToString());
                this.m.WriteMemory(variableStore.ArmySupply, "float", (this.m.ReadFloat(variableStore.UnitSuppliesCapacity) * this.m.ReadFloat(this.variableStore.ArmyActual)).ToString());
                this.m.WriteMemory(variableStore.ArmyGas, "float", (this.m.ReadFloat(variableStore.UnitFuelCapacity) * this.m.ReadFloat(this.variableStore.ArmyActual)).ToString());
            }

            // Army- HEALTH NAVAL
            if (this.m.ReadFloat(variableStore.ArmyActual) == 1)
            {
                var actualHealth = this.m.ReadFloat(variableStore.ArmyActual);
                var currentHealth = this.m.ReadFloat(variableStore.ArmyCurrent);

                if (!this.txtArmyHealthNaval.Focused && !this.cbArmyNavalStrength.Checked)
                {
                    this.txtArmyHealthNaval.PercentValue = (currentHealth / actualHealth) * 100;
                }
                if (!this.txtArmyNavalMaxHealth.Focused)
                {
                    this.txtArmyNavalMaxHealth.PercentValue = (actualHealth / actualHealth) * 100;
                }
                if (this.checkedListBox2.GetItemChecked(1)) { this.m.WriteMemory(variableStore.ArmyCurrent, "float", this.txtArmyHealthNaval.DoubleValue.ToString()); }
                if (this.checkedListBox2.GetItemChecked(2)) { this.m.WriteMemory(variableStore.ArmyActual, "float", this.txtArmyNavalMaxHealth.DoubleValue.ToString()); }
            }
            else
            {
                if (!this.txtArmyActual.Focused)
                {
                    this.txtArmyActual.Value = (decimal)this.m.ReadFloat(variableStore.ArmyActual);
                }

                if (!this.txtArmyCurrent.Focused && !this.cbArmyStrength.Checked)
                {
                    this.txtArmyCurrent.Value = (decimal)this.m.ReadFloat(variableStore.ArmyCurrent);
                }

                if (this.checkedListBox2.GetItemChecked(1)) { this.m.WriteMemory(variableStore.ArmyCurrent, "float", this.txtArmyCurrent.Value.ToString()); }
                if (this.checkedListBox2.GetItemChecked(2)) { this.m.WriteMemory(variableStore.ArmyActual, "float", this.txtArmyActual.Value.ToString()); }
            }

            // Army - Supply Gass
            if (!this.txtArmyGas.Focused && !this.cbArmyGas.Checked)
                this.txtArmyGas.Value = (decimal)this.m.ReadFloat(variableStore.ArmyGas);
            if (!this.txtArmySupply.Focused && !this.cbArmySupply.Checked)
                this.txtArmySupply.Value = (decimal)this.m.ReadFloat(variableStore.ArmySupply);
            //            var gasToFloat = this.txtArmyGas.Value;
            //            var supplyToFloat = this.txtArmySupply.Value;
            //            if (this.checkedListBox2.GetItemChecked(3)) { this.m.WriteMemory(variableStore.ArmyGas, "float", gasToFloat.ToString()); }
            //            if (this.checkedListBox2.GetItemChecked(4)) { this.m.WriteMemory(variableStore.ArmySupply, "float", supplyToFloat.ToString()); }

            //            if (this.ratingControl1.Value == 1)
            //            {
            //                this.m.WriteMemory(variableStore.ArmyExperience, "float", "0.9");
            //            }
            //            if (this.ratingControl1.Value == 2)
            //            {
            //                this.m.WriteMemory(variableStore.ArmyExperience, "float", "2");
            //            }
            //            if (this.ratingControl1.Value == 3)
            //            {
            //                this.m.WriteMemory(variableStore.ArmyExperience, "float", "6");
            //            }

            if (this.checkedListBox2.GetItemChecked(6))
            {
                switch ((int)this.ratingControl1.Value)
                {
                    case 1:
                        this.m.WriteMemory(variableStore.ArmyExperience, "float", "0.9");
                        //                        this.ratingControl1
                        break;
                    case 2:
                        this.m.WriteMemory(variableStore.ArmyExperience, "float", "2");
                        break;
                    case 3:
                        this.m.WriteMemory(variableStore.ArmyExperience, "float", "6");
                        break;
                }
            }


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void radialGauge1_Click(object sender, EventArgs e)
        {

        }

        private void ratingControl1_Click(object sender, EventArgs e)
        {
            if (this.ratingControl1.Value == 1)
            {

                this.ratingControl1.ItemHighlightColor = Color.Gray;
                this.ratingControl1.ItemSelectionColor = Color.Gray;

                this.ratingControl1.ItemHighlightStartColor = Color.Gray;
                this.ratingControl1.ItemHighlightEndColor = Color.DarkGray;

                this.ratingControl1.ItemSelectionStartColor = Color.Gray;
                this.ratingControl1.ItemSelectionEndColor = Color.DarkGray;
            }
            if (this.ratingControl1.Value == 2)
            {
                this.ratingControl1.ItemHighlightColor = Color.Maroon;
                this.ratingControl1.ItemSelectionColor = Color.Maroon;

                this.ratingControl1.ItemHighlightStartColor = Color.Red;
                this.ratingControl1.ItemHighlightEndColor = Color.DarkRed;

                this.ratingControl1.ItemSelectionStartColor = Color.Red;
                this.ratingControl1.ItemSelectionEndColor = Color.DarkRed;
            }
            if (this.ratingControl1.Value == 3)
            {
                this.ratingControl1.ItemHighlightColor = Color.Silver;
                this.ratingControl1.ItemSelectionColor = Color.Silver;

                this.ratingControl1.ItemHighlightStartColor = Color.GhostWhite;
                this.ratingControl1.ItemHighlightEndColor = Color.Silver;

                this.ratingControl1.ItemSelectionStartColor = Color.Silver;
                this.ratingControl1.ItemSelectionEndColor = Color.GhostWhite;
            }
        }

        private void txtArmyGas_ValueChanged(object sender, EventArgs e)
        {
            var gasToFloat = this.txtArmyGas.Value;
            if (this.checkedListBox2.GetItemChecked(3)) { this.m.WriteMemory(variableStore.ArmyGas, "float", gasToFloat.ToString()); }

        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            //                MessageBox.Show(((float)(((float)this.trackBar1.Value) / 100)).ToString());
            //           this.m.WriteMemory(this.variableStore.ArmyMorale, "float", ((float)(((float)this.trackBar1.Value) / 100)).ToString());
        }

        private void CreateTimer(Action cb, bool c)
        {
            timer.Interval = 1000;
            timer.Enabled = true;
            timer.Tick += delegate { cb(); };
        }

        private void cbSocialSpending_CheckedChanged(object sender, EventArgs e)
        {
            Timer socialSpendingTimer = new Timer();
            socialSpendingTimer.Interval = 1000;
            if (this.cbSocialSpending.Checked)
            {
                socialSpendingTimer.Enabled = true;
                socialSpendingTimer.Tick += delegate
                {
                    this.m.WriteMemory(this.variableStore.SocialSpendingCultureSubsidy, "float", "2");
                    this.m.WriteMemory(this.variableStore.SocialSpendingEducation, "float", "2");
                    this.m.WriteMemory(this.variableStore.SocialSpendingEnvironment, "float", "2");
                    this.m.WriteMemory(this.variableStore.SocialSpendingFamilySubsidy, "float", "2");
                    this.m.WriteMemory(this.variableStore.SocialSpendingHealthCare, "float", "2");
                    this.m.WriteMemory(this.variableStore.SocialSpendingInfrastructure, "float", "2");
                    this.m.WriteMemory(this.variableStore.SocialSpendingLawEnforcement, "float", "2");
                    this.m.WriteMemory(this.variableStore.SocialSpendingSocialAssistance, "float", "2");
                };
            }
            else
            {
                socialSpendingTimer.Enabled = false;
                socialSpendingTimer.Dispose();
            }


        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {

        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            this.WorldOpinion();
        }

        private void radialSlider1_ValueChanged(object sender, Syncfusion.Windows.Forms.Tools.RadialSlider.ValueChangedEventArgs args)
        {
            this.m.WriteMemory(this.variableStore.DomesticWorldMarketSubsidyRate, "float", (this.radialSlider1.Value / 100).ToString());
        }

        private void cbDomesticApproval_CheckedChanged(object sender, EventArgs e)
        {
            //            if (!this.cbDomesticApproval.Checked)
            //            {
            //                this.timer.Stop();
            //                this.timer.Dispose();
            //            }
            //            this.CreateTimer(new Action(() => this.m.WriteMemory(this.variableStore.DomesticDomesticApproval, "float", "1")), this.cbDomesticApproval.Checked);
        }

        private void cbMilitaryApproval_CheckedChanged(object sender, EventArgs e)
        {
            //            if (!this.cbMilitaryApproval.Checked)
            //            {
            //                this.timer.Stop();
            //                this.timer.Dispose();
            //            }
            //            this.CreateTimer(new Action(() => this.m.WriteMemory(this.variableStore.DomesticMilitaryApproval, "float", "1")), this.cbMilitaryApproval.Checked);
        }

        private void txtArmySupply_ValueChanged(object sender, EventArgs e)
        {
            var supplyToFloat = this.txtArmySupply.Value;
            if (this.checkedListBox2.GetItemChecked(4)) { this.m.WriteMemory(variableStore.ArmySupply, "float", supplyToFloat.ToString()); }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            //            MessageBox.Show(e.ToString());
            //            DataGridViewCell dataGridViewCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            //            if (dataGridViewCell is DataGridViewCheckBoxCell)
            //                {
            ////                    this.unitStore.unitTable.Rows[e.RowIndex].Frozen = (bool)dataGridViewCell.Value;
            //                }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //            MessageBox.Show(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex]);

            //            dataGridView1.Rows[e.RowIndex].Cells[4].

            this.m.WriteMemory(this.ArrayOfVariableStore[e.RowIndex]
                , this.dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString()
                , this.unitStore.unitTable.Rows[e.RowIndex][1].ToString());
            this.timer2.Enabled = true;


        }

        private void timer2_Tick_1(object sender, EventArgs e)
        {


            for (int i = 0; i < this.unitStore.unitTable.Rows.Count; i++)
            {

                if (this.dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("Attack"))
                {
                    this.dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.DarkRed;
                    this.dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.White;
                }
                if (this.dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("Defense"))
                {
                    this.dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.DarkGreen;
                    this.dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.White;
                }
                if (this.dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("Ranged"))
                {
                    this.dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.DodgerBlue;
                    this.dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }

                if ((Boolean)this.dataGridView1.Rows[i].Cells[2].Value == true)
                {
                    this.m.WriteMemory(
                        this.ArrayOfVariableStore[i],
                        this.dataGridView1.Rows[i].Cells[4].Value.ToString(),
                        this.unitStore.unitTable.Rows[i][1].ToString());
                }
                else
                {
                    if (this.dataGridView1.Rows[i].Cells[4].Value.ToString() == "int")
                        this.unitStore.unitTable.Rows[i][1] = this.m.Read2Byte(this.ArrayOfVariableStore[i]);
                    if (this.dataGridView1.Rows[i].Cells[4].Value.ToString() == "byte")
                        this.unitStore.unitTable.Rows[i][1] = this.m.ReadByte(this.ArrayOfVariableStore[i]);
                    if (this.dataGridView1.Rows[i].Cells[4].Value.ToString() == "float")
                        this.unitStore.unitTable.Rows[i][1] = this.m.ReadFloat(this.ArrayOfVariableStore[i]);
                    if (this.dataGridView1.Rows[i].Cells[4].Value.ToString() == "string")
                        this.unitStore.unitTable.Rows[i][1] = this.m.ReadString(this.ArrayOfVariableStore[i]);
                }
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            this.timer2.Enabled = false;
        }

        private void dataGridView1_VisibleChanged(object sender, EventArgs e)
        {
            if (this.dataGridView1.Visible)
            {
                this.progressBar1.Visible = true;
                this.timer2.Enabled = true;
            }
            else
            {
                this.progressBar1.Visible = false;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtArmyReserve_ValueChanged(object sender, EventArgs e)
        {
            this.m.WriteMemory(this.variableStore.ArmyReserve, "float", this.txtArmyReserve.Value.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (var dict in this.variableStore.DictOfResource)
            {
                this.m.WriteMemory(dict.Value, "float", (this.m.ReadFloat(dict.Value) + 1000000).ToString());
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void cbArmyMorale_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cbSatelliteCommunicationCoverage_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void timer2_Tick_2(object sender, EventArgs e)
        {

        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {

        }
    }
}
