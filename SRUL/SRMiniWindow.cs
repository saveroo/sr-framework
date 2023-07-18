using DevExpress.XtraEditors;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using SRUL.Types;

namespace SRUL
{
    public sealed partial class SRMiniWindow : XtraForm
    {
        #region properties

        private MainForm _parent;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x80;
                return cp;
            }
        }
        #endregion

        void parentRowUpdated(object sender, RowObjectEventArgs args)
        {
            gvPersistentUnit.RefreshRow(args.RowHandle);
        }
        void parentDataSourceChanged(object sender, EventArgs args)
        {
            gcPersistentUnit.DataSource = _parent.gvPersistentUnit.DataSource;
            gvPersistentUnit.RefreshData();
            // gcPersistentUnit.DataBindings.Clear();
            // gcPersistentUnit.DataBindings.Add("DataSource", _parent.gvPersistentUnit, "DataSource");
        }
        public SRMiniWindow(MainForm parentForm)
        {
            _parent = parentForm;
            InitializeComponent();

            // this.disp += (sdr, args) =>
            // {
            //     barMiniWindowBtn.Caption = $@"Open In-game Mini Editor";
            // };;
            this.FormClosing += (sender, args) =>
            {
                // gcPersistentUnit.DataBindings.Clear();
                _parent.gvPersistentUnit.RowUpdated -= parentRowUpdated;
                _parent.gvPersistentUnit.DataSourceChanged -= parentDataSourceChanged;
            };
            
            _parent.gvPersistentUnit.RowUpdated += parentRowUpdated;
            _parent.gvPersistentUnit.DataSourceChanged += parentDataSourceChanged;
            
            // Hide From alt-tab
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;


            // populate Listbox
            checkedListBoxControl1.DataSource = HandmadeFeatures.Instance.BindingSource();
            checkedListBoxControl1.DisplayMember = "enums";
            checkedListBoxControl1.ValueMember = "freeze";

            //Init Gridcontrols
            gcCountry.DataSource = _parent.gcCountry.DataSource;
            gcResources.DataSource = _parent.gcResources.DataSource;
            gcWarfare.DataSource = _parent.gcWarfare.DataSource;
            gcFacility.DataSource = _parent.gcFacility.DataSource;
            gcCountry.ForceInitialize();
            gcResources.ForceInitialize();
            gcWarfare.ForceInitialize();
            gcFacility.ForceInitialize();
            
            // so the column formatted nicely
            SRViews.Instance.InitializeGridViews(new []
            {
                gvCountry, 
                gvResources, 
                gvWarfare, 
                gvFacility
            });

            // GV Hihlight
            using (var rs = new SRStyle())
            {
                //Set Row style for editable
                rs.gvRowStyle(gvCountry);
                rs.gvRowStyle(gvResources);
                rs.gvRowStyle(gvWarfare);
                rs.gvRowStyle(gvFacility);
                
                rs.GvHighlight(gvCountry);
                rs.GvHighlight(gvResources);
                rs.GvHighlight(gvWarfare);
                rs.GvHighlight(gvFacility);
            }

            // When DATASOURCE of Gridview is changed
            gvCountry.DataSourceChanged += _parent.GvDataSourceChanged;
            gvResources.DataSourceChanged += _parent.GvDataSourceChanged;
            gvWarfare.DataSourceChanged += _parent.GvDataSourceChanged;
            gvFacility.DataSourceChanged += _parent.GvDataSourceChanged;
            gvModifiedUnit.DataSourceChanged += _parent.GvDataSourceChanged;
            gvPersistentUnit.DataSourceChanged += _parent.GvDataSourceChanged;
            
            leTechEffect1.Tag = _parent.leTechEffect1.Tag;
            leTechEffect2.Tag = _parent.leTechEffect2.Tag;
            leTechEffect1.Validated += _parent.Events.OnTechEffectValidated;
            leTechEffect2.Validated += _parent.Events.OnTechEffectValidated;

            // Method to set gridview init events etc.
            _parent.SetGvEvents(gvCountry, _parent.JsonReader.DataCountry);
            _parent.SetGvEvents(gvResources, _parent.JsonReader.DataResources);
            _parent.SetGvEvents(gvWarfare, _parent.JsonReader.DataWarfare);
            _parent.SetGvEvents(gvFacility, _parent.JsonReader.DataSpecial);
            
            // Set Data binding to these.... so it reflect to parent
            _parent.CeSetDataBindings(cbADayArmy, _parent.ADayArmy);
            _parent.CeSetDataBindings(cbADayBuild, _parent.ADayBuild);
            _parent.CeSetDataBindings(ceSatComm, _parent.SatelliteCommCoverage);
            _parent.CeSetDataBindings(ceSatMilDef, _parent.SatelliteMissileDefenseCoverage);
            _parent.CeSetDataBindings(ceSatRecon, _parent.SatelliteReconCoverage);
            _parent.CeSetDataBindings(ceEfficiency, _parent.ArmyEfficiency);
            _parent.CeSetDataBindings(ceEntrenchment, _parent.ArmyEntrenchment);
            _parent.CeSetDataBindings(ceExperience, _parent.ArmyExperience);
            _parent.CeSetDataBindings(ceMorale, _parent.ArmyMorale);
            _parent.CeSetDataBindings(ceUnitFuel, _parent.ArmyFuel);
            _parent.CeSetDataBindings(ceUnitStrength, _parent.ArmyCurrentStrength);
            _parent.CeSetDataBindings(ceUnitSupplies, _parent.ArmySupply);
            _parent.CeSetDataBindings(ceGarrisonEfficiency, _parent.BuildingGarrisonEfficiency);
            _parent.CeSetDataBindings(ceGarrisonInstant, _parent.BuildingGarrisonActive);

            _parent.SeSetDataBindings(seStaffActive, _parent.seStaffActive);
            _parent.SeSetDataBindings(seStaffReserve, _parent.seStaffReserve);
            _parent.SeSetDataBindings(seUnitStrCurrent, _parent.seUnitStrCurrent);
            _parent.SeSetDataBindings(seUnitStrActual, _parent.seUnitStrActual);
            _parent.SeSetDataBindings(seUnitFuel, _parent.seUnitFuel);
            _parent.SeSetDataBindings(seUnitSupplies, _parent.seUnitSupplies);
            _parent.SeSetDataBindings(seUnitActualFuel, _parent.seUnitActualFuel);
            _parent.SeSetDataBindings(seUnitActualSupplies, _parent.seUnitActualSupplies);
            _parent.SeSetDataBindings(seGarrisonEfficiency, _parent.seGarrisonEfficiency);

            // gcPersistentUnit.MainView = _parent.gcPersistentUnit.MainView;

            _parent.SetControlBindings(gcPersistentUnit, _parent.gcPersistentUnit);
            _parent.SetControlBindings(ceDefenseSelection, _parent.ceDefenseSelection);
            _parent.SetControlBindings(ceDefenseProduction, _parent.ceDefenseProduction);
            
            seUnitStrCurrent.Properties.ValidateOnEnterKey = true;
            seUnitSupplies.Properties.ValidateOnEnterKey = true;
            seUnitFuel.Properties.ValidateOnEnterKey = true;
            seUnitStrActual.Properties.ValidateOnEnterKey = true;
            
            // _parent.ControlSpinEditSafeRead(seStaffActive);
            // _parent.ControlSpinEditSafeRead(seStaffReserve);
            // _parent.ControlSpinEditSafeRead(seUnitStrActual);
            // _parent.ControlSpinEditSafeRead(seUnitStrCurrent);
            // _parent.ControlSpinEditSafeRead(seUnitFuel);
            // _parent.ControlSpinEditSafeRead(seUnitSupplies);
            // _parent.ControlSpinEditSafeRead(seUnitActualFuel);
            // _parent.ControlSpinEditSafeRead(seUnitActualSupplies);
            // _parent.ControlSpinEditSafeRead(seGarrisonEfficiency););

            ceUnitFuel = _parent.ceUnitFuel;
            ceUnitSupplies = _parent.ceUnitSupplies;
            ceUnitStrength = _parent.ceUnitStrength;

            //Binding Checkedit to parent relatives
            // This is only button not speific for 1 feature, so just binded hackaway
            ceBindToParentControl(cbADayResearch);
            ceBindToParentControl(ceMissiles);
            // ceBindToParentControl(ceModeHover);
            // ceBindToParentControl(ceModePersistent);
            
            _parent.SetControlBindingToSettings(ceModePersistent,  
                "checked",
                "WarfareTABModeTracking");
            _parent.SetControlBindingToSettings(ceModeHover, 
                "checked",  
                "WarfareTABModeHover");
            _parent.SetControlBindingToSettings(ceRailRoadDaysToBuild, 
                "checked","SpecialTABRailRoadDaysToBuild");


            //Bind Rating
            rtExperience.EditValueChanged += (_, _) =>
            {
                //parent.rtExperience.EditValue = rtExperience.EditValue;
                _parent.rtExperience.Rating = rtExperience.Rating;
            };

            // Bind, Special Warfare Option
            checkedListBoxControl1.ItemCheck += (sd, args) =>
            {
                CheckedListBoxControl clbc = sd as CheckedListBoxControl;
                // var bl = clbc.Items[HandmadeEnums.Rambo].CheckState == CheckState.Checked;
                // string s = Enum.GetName(typeof(HandmadeEnums), sdargs.Index);
                // int index = checkedListBoxControl1.FindItem(0, true, delegate(ListBoxFindItemArgs ee) {
                //     ee.IsFound = s.Equals(ee.DisplayText);
                // });
                if (clbc.GetItemText(args.Index) == "Rambo" && clbc.GetItemChecked(args.Index))
                {
                    rtExperience.Rating = 3;
                    ceEfficiency.Checked = true;
                    ceExperience.Checked = true;
                    ceMorale.Checked = true;
                }
                else
                {
                    rtExperience.Rating = 0;
                    ceEfficiency.Checked = false;
                    ceExperience.Checked = false;
                    ceMorale.Checked = false;
                }
                // HandmadeFeatures.Instance.Lists[args.Index].freeze = args.State == CheckState.Checked;
                clbc.SetItemValue(args.State == CheckState.Checked, args.Index);
            };
            
            SET_BUTTON_EVENTS();

            // Bind % Bar Tags, to be readed later
            pbUnitHealthBar.Tag = _parent.pbUnitHealthBar.Tag;
            pbUnitFuelBar.Tag = _parent.pbUnitFuelBar.Tag;
            pbUnitSupplyBar.Tag = _parent.pbUnitSupplyBar.Tag;
            pbArmyEfficiencyBar.Tag = _parent.pbArmyEfficiencyBar.Tag;
            pbArmyMoraleBar.Tag = _parent.pbArmyMoraleBar.Tag;
            pbArmyEntrenchmentBar.Tag = _parent.pbArmyEntrenchmentBar.Tag;

            pbUnitHealthBar.CustomDisplayText += _parent.SetProgressBarDisplayOverload;
            pbUnitFuelBar.CustomDisplayText += _parent.SetProgressBarDisplayOverload;
            pbUnitSupplyBar.CustomDisplayText += _parent.SetProgressBarDisplayOverload;
            
            

            /////

            // Set Window Transparency
            this.TransparencyKey = this.BackColor = Color.WhiteSmoke;
            // this.TransparencyKey = this.BackColor = Color.WhiteSmoke;
            // var shipIcon = 
            simpleButton1.BackColor = Color.FromArgb(5, Color.DarkRed);
            //Deactivate += FormDeactivate;

            SET_BAR_EVENTS(pbArmyMoraleBar, _parent.pbArmyMoraleBar);
            SET_BAR_EVENTS(pbArmyEfficiencyBar, _parent.pbArmyEfficiencyBar);
            SET_BAR_EVENTS(pbArmyEntrenchmentBar, _parent.pbArmyEntrenchmentBar);
            SET_BAR_EVENTS(pbUnitHealthBar, _parent.pbUnitHealthBar);
            SET_BAR_EVENTS(pbUnitFuelBar, _parent.pbUnitFuelBar);
            SET_BAR_EVENTS(pbUnitSupplyBar, _parent.pbUnitSupplyBar);


            // Init Special Option Satellite value for cb caption
            ceSatComm.Text =
                $"Communication Coverage {(float.Parse(_parent.JsonReader.feature("SatelliteCommCoverage").value) * 100)}%";
            ceSatMilDef.Text =
                $"Missile Defense Coverage {(float.Parse(_parent.JsonReader.feature("SatelliteMissileDefenseCoverage").value) * 100)}%";
            ceSatRecon.Text =
                $"Reconnaisance Coverage {(float.Parse(_parent.JsonReader.feature("SatelliteReconCoverage").value) * 100)}%";

            // Init History Data Source from memento
            gcModifiedUnit.DataSource = SRMemento.Instance.UnitHistoryList;
            gcModifiedUnit.ForceInitialize();
            
            // Set visibility
            string[]? colNames = new[] { "RowId", "UnitAddress", "ModifiedStats", "UnitStats" };
            gvModifiedUnit.WithColNames(colNames, (column) => column.Visible = false);
            
            // set allow edit
            colNames = new[] { "RowId", "UnitName", "UnitId" };
            gvModifiedUnit.WithColNames(colNames, (column) => column.OptionsColumn.AllowEdit = false);
            // let gc handle this
            colNames = null;
            
            gvModifiedUnit.BestFitColumns();
            _parent.GvLoad(gvModifiedUnit);

            
            _parent.GvMasterDetail(gcModifiedUnit, gvModifiedUnit);
            _parent.TrackedUnitsController.Initialize(
                gcPersistentUnit, 
                gvPersistentUnit, 
                gvPersistentUnitStats);
            gcPersistentUnit.RefreshDataSource();
            // gvPersistentUnit.Columns["UnitStatus"].Visible = false;
            gvPersistentUnit.Columns["UnitKills"].Caption = @"K";
            gvPersistentUnit.Columns["UnitKills"].AppearanceHeader.ForeColor = Color.Red;
            gvPersistentUnit.Columns["UnitPlatoon"].Caption = @"No";
            gvPersistentUnit.Columns["UnitBattleGroup"].Caption = @"G";
            // gvPersistentUnit.Columns["UnitStatus"].Caption = @"A";
            gvPersistentUnit.Columns["UnitHealth"].Caption = @"H";
            gvPersistentUnit.Columns["UnitName"].Caption = @"Model";


            // gvPersistentUnitStats.CustomRowFilter += _parent.gvPersistentUnitStats_CustomRowFilter;
            // _parent.GvLoad(gvPersistentUnit);
            // gcPersistentUnit.ForceInitialize();

            // var Refresher = SRRefresher.CreateInstance();
            _parent.Refresher.InitializeTimer((o) =>
            {
                if(xtabMainControl.SelectedTabPage == xtabCountry) 
                    _parent.RefreshDataGridRowCell(gvCountry);
                if(xtabMainControl.SelectedTabPage == xtabResources) 
                    _parent.RefreshDataGridRowCell(gvResources);
                if(xtabMainControl.SelectedTabPage == xtabWarfare2) 
                    _parent.RefreshDataGridRowCell(gvWarfare);
                if(xtabMainControl.SelectedTabPage == xtabFacilities) 
                    _parent.RefreshDataGridRowCell(gvFacility);
                // if(!gvPersistentUnit.IsEditing)
                // _parent.Memento.RefreshMasterTimer(gvPersistentUnit, _parent.rw);
                // if(!gvPersistentUnitStats.IsEditing)
                // _parent.Memento.RefreshDetailTimer(gvPersistentUnitStats, _parent.rw);
                // gvPersistentUnit.RefreshData();
                // _parent.BuildModeOverride(leBuildModeOverride, ceBuildModeOverride.Checked);
            });
            
            leBuildModeOverride.DataBindings.Add(
                "EditValue", 
                _parent.leBuildModeOverride, 
                "EditValue", 
                false, 
                DataSourceUpdateMode.OnPropertyChanged);
            
            ceBuildModeOverride.DataBindings.Add(
                "checked", 
                _parent.ceBuildModeOverride, 
                "checked", 
                false, 
                DataSourceUpdateMode.OnPropertyChanged);
            
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST)
                m.Result = (IntPtr)(HT_CAPTION);
        }

        private const int WM_NCHITTEST = 0x84;
        private const int HT_CLIENT = 0x1;
        private const int HT_CAPTION = 0x2;

        private void SRMiniWindow_Load(object sender, EventArgs e)
        {
            // WindowsFormsSettings.ThickBorderWidth = 1;
            // Transparent button
            ceModeHover = _parent.ceModeHover;

            timer1.Enabled = true;
            this.Opacity = 90;
            zoomTrackBarControl1.EditValue = 90;


            // this.LookAndFeel.SetSkinStyle(SkinStyle.);
            // var s = skinPaletteDropDownButtonItem1.DropDownControl as GalleryDropDown
            // Enablind timer after datasource set
            //SpinEdit
            _parent.SetControlTag(seStaffActive, "ArmyActiveStaff");
            _parent.SetControlTag(seStaffReserve, "ArmyReserve");
            _parent.SetControlTag(seUnitStrActual, "ArmyActualStrength");
            _parent.SetControlTag(seUnitStrCurrent, "ArmyCurrentStrength");
            _parent.SetControlTag(seUnitFuel, "ArmyFuel");
            _parent.SetControlTag(seUnitSupplies, "ArmySupply");

            // When changed then changed
            _parent.SetSpinEditorEvent(seUnitStrActual);
            _parent.SetSpinEditorEvent(seUnitStrCurrent);
            _parent.SetSpinEditorEvent(seUnitFuel);
            _parent.SetSpinEditorEvent(seUnitSupplies);
            _parent.SetSpinEditorEvent(seStaffReserve);
            
            // Replaced with celistbox
            // Check edit events
            // _parent.SetControlBindings(ceCheatAllUnits, _parent.ceCheatAllUnits);
            // _parent.SetControlBindings(ceCheatFogOfWar, _parent.ceCheatFogOfWar);
            // ceListBoxCheats.DataSource = _parent.ceListBoxCheats.DataSource;
            _parent.SpecialCheats.InitView(ref ceListBoxCheats);
            // ceListBoxCheats.DataBindings.Add(
            //     "CheckedItems", 
            //     _parent.ceListBoxCheats, 
            //     "CheckedItems", 
            //     false, 
            //     DataSourceUpdateMode.OnPropertyChanged);

            // Populate Combo box warfare unit value
            _parent.PopulateWarfareTypes(lookBoxUnitClass, WarfareValueDictionary.Instance.DictUnitClassType);
            _parent.PopulateWarfareTypes(lookBoxUnitMovementType, WarfareValueDictionary.Instance.DictUnitMovementType);
            _parent.PopulateWarfareTypes(lookBoxUnitTargetType, WarfareValueDictionary.Instance.DictUnitTargetType);

            // For Cell Editing custom, spinedit etc in a row.
            _parent.AddRepositoryItem(_parent.JsonReader.FeaturesCountry, gcCountry, gvCountry);
            _parent.AddRepositoryItem(_parent.JsonReader.FeaturesResources, gcResources, gvResources);
            _parent.AddRepositoryItem(_parent.JsonReader.FeaturesWarfare, gcWarfare, gvWarfare);
            _parent.AddRepositoryItem(_parent.JsonReader.FeaturesSpecial, gcFacility, gvFacility);

            // Flash Helper

            // SE percentage
            _parent.SetSpinEditDisplayPercentage(seNavalStrActual, true);
            _parent.SetSpinEditDisplayPercentage(seNavalStrCurrent, true);
            // pbUnitHealthBar.custom

            // Custom Display for HEALTH Bar, supposed to display overload value
            pbUnitHealthBar.Tag = "ArmyCurrentStrength";
            pbUnitFuelBar.Tag = "ArmyFuel";
            pbUnitSupplyBar.Tag = "ArmySupply";
            pbUnitHealthBar.CustomDisplayText += _parent.SetProgressBarDisplayOverload;
            pbUnitFuelBar.CustomDisplayText += _parent.SetProgressBarDisplayOverload;
            pbUnitSupplyBar.CustomDisplayText += _parent.SetProgressBarDisplayOverload;

            // 2.0.0.0 Unit history and Persistent
            
            // Facilities Spin editor Validation
            #region Facilities Control Initiliazization
            //parent.setControlTag(seFacBuildTime, "FacilityBuildTime");

            // Garrison
            _parent.SetControlTag(seGarrisonEfficiency, "BuildingGarrisonEfficiency");

            // Display as percentage
            _parent.SetSpinEditDisplayPercentage(seGarrisonEfficiency, true);
            _parent.SetSpinEditDisplayPercentage(seTechEffectModifier1, true);
            _parent.SetSpinEditDisplayPercentage(seTechEffectModifier2, true);

            // when edited
            //parent.setSpinEditorEvent(seFacBuildTime);
            _parent.SetSpinEditorEvent(seGarrisonEfficiency);

            #endregion

            #region Tech Effect

            _parent.PopulateLookUpEditDataSource(leTechEffect1, 
                TechEffects.TechDictionarySource);
            _parent.PopulateLookUpEditDataSource(leTechEffect2, 
                TechEffects.TechDictionarySource);
            _parent.PopulateLookUpEditDataSource(leBuildModeOverride, 
                BuildingList.Facilities);

            _parent.SetControlTag(seTechEffectModifier1, "TechEffectIndustrializationModifier1");
            _parent.SetControlTag(seTechEffectModifier2, "TechEffectIndustrializationModifier2");
            _parent.SetSpinEditorEvent(seTechEffectModifier1);
            _parent.SetSpinEditorEvent(seTechEffectModifier2);

            #endregion
            btnMakePersistent.DataBindings.Add("ForeColor", 
                _parent.btnMakePersistent, "ForeColor",
                false, DataSourceUpdateMode.OnPropertyChanged);
            btnMakePersistent.DataBindings.Add("BackColor", 
                _parent.btnMakePersistent, "BackColor",
                false, DataSourceUpdateMode.OnPropertyChanged);
            btnMakePersistent.DataBindings.Add("Enabled", 
                _parent.btnMakePersistent, "Enabled",
                false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void SET_BUTTON_EVENTS()
        {

            btnMakePersistent.Click += (o, args) =>
            {
                
                _parent.btnMakePersistent.PerformClick();
                if (!_parent.TrackedUnitsController.IsUnitTracked("ArmyCurrentStrength".GetPointer(_parent.rw))) 
                    _parent.TrackedUnitsController.MakePersistent(_parent.rw, _parent.JsonReader.FeaturesWarfare);
                else 
                    _parent.TrackedUnitsController.RemoveSelectedTrackedUnit(_parent.gvPersistentUnit);
                gcPersistentUnit.RefreshDataSource();
                _parent.gcPersistentUnit.RefreshDataSource();
            };

            void btnRemoveTrackedUnit()
            {
                // if (!SRMemento.Instance.UnitIsPersistent("ArmyCurrentStrength".GetPointer(_parent.rw)))
                //     SRMemento.Instance.RemovePersistantUnit(gvPersistentUnit, "ArmyCurrentStrength".GetPointer(_parent.rw));
                // gcPersistentUnit.RefreshDataSource();
            }

            // Restore Original Stat Value
            btnRestoreUnitToOriginal.Click += (o, args) =>
            {
                if (gvModifiedUnit.RowCount < 1) return;
                var selectedRowIndex = gvModifiedUnit.GetSelectedRows()[0];
                var rowValue = gvModifiedUnit.GetRowCellValue(selectedRowIndex, "UnitAddress");
                SRMemento.Instance.RestoreToOriginal(rowValue.ToString(), _parent.JsonReader.FeaturesWarfare, _parent.rw);

                gcModifiedUnit.RefreshDataSource();
            };
            
            xtabSubWarfare.Selected += (o, args) =>
            {
                gcPersistentUnit.Refresh();
                // gvPersistentUnit.BestFitColumns();
                // gvPersistentUnit.Columns["RowId"].OptionsColumn.FixedWidth = true;
                // gvModifiedUnit.BestFitColumns();
                // gvModifiedUnit.Columns["UnitId"].OptionsColumn.FixedWidth = true;
                
            };
        }

        // private void ArmyControlWriter()
        // {
        //     if (SRMemento.Instance.UnitIsPersistent("ArmyCurrentStrength".GetPointer(_parent.rw))) return;
        //     if (SRMemento.Instance.UnitIsPersistent("HoverArmyCurrentStrength".GetPointer(_parent.rw))) return;
        //
        //     if (ceMorale.Checked)
        //     {
        //         "ArmyMorale".GetFeature().WriteTo(_parent.rw, "5");
        //     }
        //     
        //     // rw.SRWrite("ArmyMorale", "10.0");
        //     if (ceExperience.Checked)
        //     {
        //         var rate = (rtExperience.Rating == 3 ? 5 : rtExperience.Rating).ToString(CultureInfo.InvariantCulture);
        //         "ArmyExperience".GetFeature().WriteTo(_parent.rw, rate);
        //     }
        //
        //     // rw.SRWrite("ArmyExperience", "10.0");
        //     if (ceEfficiency.Checked)
        //     {
        //         "ArmyEfficiency".GetFeature().WriteTo(_parent.rw, "5");
        //         _parent.Hover(() => "HoverArmyEfficiency".GetFeature().WriteTo(_parent.rw, "5"));
        //     }
        //
        //     // rw.SRWrite("ArmyEfficiency", "10.0");
        //
        //     bool IsChecked(HandmadeEnums e)
        //     {
        //         string s = Enum.GetName(typeof(HandmadeEnums), e);
        //         int index = checkedListBoxControl1.FindItem(0, true, delegate(ListBoxFindItemArgs ee) {
        //             ee.IsFound = s.Equals(ee.DisplayText);
        //         });
        //         return checkedListBoxControl1.GetItemCheckState(index) == CheckState.Checked;
        //         // return false;
        //         // return checkedListBoxControl1.Items[e].CheckState == CheckState.Checked;
        //     }
        //
        //     // Init For special 
        //     var actualStrength = _parent.ArmyActualStrength.value.StrToDecimal();
        //     var actualGas = (_parent.ArmyCurrentStrength.value.StrToDecimal() * _parent.UnitFuelCapacity.value.StrToDecimal());
        //     var actualSupply = (_parent.ArmyCurrentStrength.value.StrToDecimal() * _parent.UnitSuppliesCapacity.value.StrToDecimal());
        //
        //
        //     var hoverActualStrength = _parent.HoverArmyActualStrength.value.StrToDecimal();
        //     var hoverActualGas = (_parent.HoverArmyCurrentStrength.value.StrToDecimal() * _parent.HoverUnitFuelCap.value.StrToDecimal());
        //     var hoverActualSupply = (_parent.HoverArmyCurrentStrength.value.StrToDecimal() * _parent.HoverUnitSupplyCap.value.StrToDecimal());
        //
        //
        //     // Multiplier
        //     // var supplyGasMultiplier = 1;
        //     // var strMultiplier = 1;
        // }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

        }

        private void simpleButton1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point loc1 = MousePosition;
                this.Location = loc1;
            }
        }

        // Set mini windows to minimize when Target minimized, and not seen if Target is not visible.
        private void windowBehaviour()
        {
            if (!_parent.Loader.Selected.GameProcess.HasExited)
            {
                var gameProcess = _parent.Loader.Selected.GameProcess.MainWindowHandle;
                var TargetIsForeground = gameProcess == WindowTracker.GetForegroundWindow();
                // var GameIsForeground = WindowTracker.GetWindowThreadProcessId(gameProcess);
                // var TargetIsForeground = Target == WindowTracker.GetForegroundWindow();
                var ThisFormProcessIsForeground = Process.GetCurrentProcess().MainWindowHandle == WindowTracker.GetForegroundWindow();
                // var TargetIsMinimized = WindowTracker.IsIconic(parent.Loader.Selected.GameProcess.MainWindowHandle);

                if (_parent.Focused)
                {
                    Opacity = 0;
                }
                else
                {
                    if(TargetIsForeground || ThisFormProcessIsForeground || this.ContainsFocus)
                        Opacity = (double)(Convert.ToInt32(zoomTrackBarControl1.EditValue)) / 100;
                    else
                        Opacity = 0;      
                }
                // if (!TargetIsForeground && !ThisFormProcessIsForeground && !this.Focused)
                //     Opacity = 0.2;
                // else if (ThisFormProcessIsForeground || TargetIsForeground || this.Focused)
                //     Opacity = (double)(Convert.ToInt32(zoomTrackBarControl1.EditValue))/100;   
            }
            else
            {
                this.Dispose();
                this.Close();
            }
        } 

        private void timer1_Tick(object sender, EventArgs e)
        {
            // rw.IsTrainerNeedRestart();
            // _parent.GvPersistentDetailRuntime();
            windowBehaviour();
            
            // TODO: Quick hax.
            ceDefenseProduction.Properties.AppearanceReadOnly.ForeColor =
                _parent.ceDefenseProduction.Properties.AppearanceReadOnly.ForeColor; 
            
            // TODO: refactor
            // Iterate thru checklistboxcontrol to reflect changes on parent equivalent.
            for (int i = 0; i < checkedListBoxControl1.ItemCount; i++)
            {
                checkedListBoxControl1.SetItemChecked(i, HandmadeFeatures.Instance.Lists[i].freeze);
            }

            for (int i = 0; i < ceListBoxCheats.ItemCount; i++)
            {
                ceListBoxCheats.SetItemChecked(i, _parent.SpecialCheats.Cheats[i].Checked);
            }

            // string unitName = rw.SRRead("UnitName");
            // string unitID = rw.SRRead("UnitID");
            //int unitPDeployed = (unitDeployed.StrToInt());
            //int unitPReserved = unitReserved.StrToInt();
            //int unitTotal = unitPDeployed + unitPReserved;

            //bind missile editvalue to parent relatives
            seMissiles.EditValue = _parent.seMissiles.EditValue;


            //bind Indicator from parent
            ceNavalUnitIndicator.Checked = _parent.ceNavalUnitIndicator.Checked;
            ceNavalUnitIndicator.ReadOnly = _parent.ceNavalUnitIndicator.ReadOnly;
            ceNavalUnitIndicator.ForeColor = _parent.ceNavalUnitIndicator.ForeColor;

            cePersistentUnitIndicator.ReadOnly = _parent.cePersistentUnitIndicator.ReadOnly;
            cePersistentUnitIndicator.ForeColor = _parent.cePersistentUnitIndicator.ForeColor;
            cePersistentUnitIndicator.CheckState = _parent.cePersistentUnitIndicator.CheckState;
            
            // Unit Movement/target/class Refresh every tick
            // ARMY - Class updater and changer. 
            _parent.UnitClassLookUpEditHandler("UnitMovementType", lookBoxUnitMovementType);
            _parent.UnitClassLookUpEditHandler("UnitTargetType", lookBoxUnitTargetType);
            _parent.UnitClassLookUpEditHandler("UnitClass", lookBoxUnitClass);

            // Tech Effect
            // _parent.FacilitiesLookUpEditHandler("TechEffectIndustrializationEffect1", leTechEffect1);
            // _parent.FacilitiesLookUpEditHandler("TechEffectIndustrializationEffect2", leTechEffect2);

            // Reader unified
            // _parent.ArmyControlReader();
            // _parent.ArmyControlWriter();

            // Set specific spinedit to display % when naval is selected
            // _parent.SetSpinEditDisplayPercentage(seUnitStrActual, _parent.IsNaval);
            // _parent.SetSpinEditDisplayPercentage(seUnitStrCurrent, _parent.IsNaval);

            // READ ARMY ACTIVE STAFF REES
            // UnitReader(seStaffActive, jsonReader.FeaturesWarfare);
            // UnitReader(seStaffReserve, jsonReader.FeaturesWarfare);
            // UnitReader(seUnitFuel, jsonReader.FeaturesWarfare);
            // UnitReader(seUnitGas, jsonReader.FeaturesWarfare);

            // This update Army Naval Health blablabla
            // calculateHealth();
            // ArmyControlWriter();

            // _parent.FacilitiesReader();
            lblFacID.Text = _parent.lblFacID.Text;
            lblFacName.Text = _parent.lblFacName.Text;
            // _parent.FacilitiesWriter();

            // _parent.TechEffectReader();
            // _parent.TechEffectWriter();

            // _parent.UnitModelSizeReader();

            // Multiplayer Protection
            // _parent.MultiplayerProtection();

            // bind SpinEditor to parent value
            // _parent.ControlSpinEditSafeRead(seStaffActive);
            // _parent.ControlSpinEditSafeRead(seStaffReserve);
            // _parent.ControlSpinEditSafeRead(seUnitStrActual);
            // _parent.ControlSpinEditSafeRead(seUnitStrCurrent);
            // _parent.ControlSpinEditSafeRead(seUnitFuel);
            // _parent.ControlSpinEditSafeRead(seUnitSupplies);
            // _parent.ControlSpinEditSafeRead(seUnitActualFuel);
            // _parent.ControlSpinEditSafeRead(seUnitActualSupplies);
            // _parent.ControlSpinEditSafeRead(seGarrisonEfficiency);

            // bind bar to parent value
            // pbUnitHealthBar.Properties.Maximum = _parent.pbUnitHealthBar.Properties.Maximum;
            // pbUnitFuelBar.Properties.Maximum = _parent.pbUnitFuelBar.Properties.Maximum;
            // pbUnitSupplyBar.Properties.Maximum = _parent.pbUnitSupplyBar.Properties.Maximum;
            // pbArmyEfficiencyBar.Properties.Maximum = _parent.pbArmyEfficiencyBar.Properties.Maximum;
            // pbArmyMoraleBar.Properties.Maximum = _parent.pbArmyMoraleBar.Properties.Maximum;
            // pbArmyEntrenchmentBar.Properties.Maximum = _parent.pbArmyEntrenchmentBar.Properties.Maximum;
            //
            // pbUnitHealthBar.EditValue = _parent.pbUnitHealthBar.EditValue;
            // pbUnitFuelBar.EditValue = _parent.pbUnitFuelBar.EditValue;
            // pbUnitSupplyBar.EditValue = _parent.pbUnitSupplyBar.EditValue;
            // pbArmyMoraleBar.EditValue = _parent.pbArmyMoraleBar.EditValue;
            // pbArmyEfficiencyBar.EditValue = _parent.pbArmyEfficiencyBar.EditValue;
            // pbArmyEntrenchmentBar.EditValue = _parent.pbArmyEntrenchmentBar.EditValue;

            // _parent.pbUnitHealthBar.EditValueChanged += (o, args) =>
            // {
            //     pbUnitHealthBar.EditValue = _parent.pbUnitHealthBar.EditValue;
            // };
            
            

            // bind persitent button to parent value
            // btnMakePersistent = _parent.btnMakePersistent;
            // btnMakePersistent.Enabled = _parent.btnMakePersistent.Enabled;
            // btnMakePersistent.ForeColor = _parent.btnMakePersistent.ForeColor;

            if (btnMakePersistent.Appearance.BackColor != _parent.btnMakePersistent.Appearance.BackColor)
            {
                btnMakePersistent.Appearance.BackColor = _parent.btnMakePersistent.Appearance.BackColor;
                btnMakePersistent.Text = cePersistentUnitIndicator.Checked ? "Untrack" : "Track Unit";
                gcPersistentUnit.RefreshDataSource();
            }
            
            gcPersistentUnit.DataSource = _parent.gcPersistentUnit.DataSource;
        }
        
        void SET_BAR_EVENTS(ProgressBarControl pbcChild, ProgressBarControl pbcParent)
        {
            pbcParent.EditValueChanged += (o, args) =>
            {
                pbcChild.EditValue = pbcParent.EditValue;
                pbcChild.Properties.Maximum = pbcParent.Properties.Maximum;
                pbcChild.Position = pbcParent.Position;
            };
        }

        public void MiniWindows_Dispose()
        {
            gcPersistentUnit.DataBindings.Clear();
            
            gvWarfare.Dispose();
            gcWarfare.DataSource = null;
            
            gvFacility.Dispose();
            gcFacility.DataSource = null;
            
            gcResources.Dispose();
            gcResources.DataSource = null;
            
            gcCountry.Dispose();
            gcCountry.DataSource = null;
            
            ceModeHover.DataBindings.Clear();
            ceModePersistent.DataBindings.Clear();
            ceBuildModeOverride.DataBindings.Clear();

            pbUnitHealthBar.CustomDisplayText -= _parent.SetProgressBarDisplayOverload;
            pbUnitFuelBar.CustomDisplayText -= _parent.SetProgressBarDisplayOverload;
            pbUnitSupplyBar.CustomDisplayText -= _parent.SetProgressBarDisplayOverload;
            
            gvCountry.DataSourceChanged -= _parent.GvDataSourceChanged;
            gvResources.DataSourceChanged -= _parent.GvDataSourceChanged;
            gvWarfare.DataSourceChanged -= _parent.GvDataSourceChanged;
            gvFacility.DataSourceChanged -= _parent.GvDataSourceChanged;
            gvModifiedUnit.DataSourceChanged -= _parent.GvDataSourceChanged;
            gvPersistentUnit.DataSourceChanged -= _parent.GvDataSourceChanged;
            
            _parent.TrackedUnitsController.ClearEvent(gcPersistentUnit, gvPersistentUnit, gvPersistentUnitStats);
            leBuildModeOverride.Dispose();
            ceListBoxCheats.Dispose();
            _parent = null;
            this.Dispose(true);
            // GC.SuppressFinalize(this);
        }

        private void zoomTrackBarControl1_EditValueChanged(object sender, EventArgs e)
        {
            if (zoomTrackBarControl1.EditValue != null)
            {
                var val = Convert.ToInt32(zoomTrackBarControl1.EditValue);
                this.Opacity = (double)val / 100;
            }
        }

        private void visibleBtn_Click(object sender, EventArgs e)
        {
            xtabMainControl.Visible = !xtabMainControl.Visible;
            if (!xtabMainControl.Visible)
                visibleBtn.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/hide_16x16.png");
            else
                visibleBtn.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");
        }

        private void SRMiniWindow_Resize(object sender, EventArgs e)
        {
            //MoveWindow(hWndDocked, 0, 0, this.Width, this.Height, true);
        }
        
        // preserve space

        private void ceBindToParentControl(CheckEdit childCtrl)
        {
            CheckEdit parentCtrl = (CheckEdit)_parent.Controls.Find(childCtrl.Name.ToString(), true).FirstOrDefault();
            childCtrl.CheckedChanged += (s, e) =>
            {
                parentCtrl.Toggle();
                childCtrl.CheckState = parentCtrl.CheckState;
            };
        }
        private void cbADayBuild_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void btnMakePersistent_Click(object sender, EventArgs e)
        {
            _parent.btnMakePersistent.PerformClick();
        }

        private void cbtnSocials_CheckedChanged(object sender, EventArgs e)
        {
            _parent.GroupedControlToFreezeList(seSocials.EditValue.ToString(), cbtnSocials.Checked, ListOfSortedRow.SocialSpendingList);
        }

        private void SRMiniWindow_Move(object sender, EventArgs e)
        {
            WindowTracker.userX = this.Location.X - WindowTracker.r.Left;
            WindowTracker.userY = this.Location.Y - WindowTracker.r.Top;
        }
    }
}