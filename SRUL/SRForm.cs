using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraTab;
using SRUL.Types;

namespace SRUL
{
    // TODO: Refactor SharedEvent to own Class, so not clutered
    // TODO: Form Class supposed for using to init and placement
    public partial class XtraForm1 : DevExpress.XtraBars.ToolbarForm.ToolbarForm
    {
        #region Properties
        // Information countryInfoTable = new Information(Loader.Rw);

        // Inherit from Loader Class
        // private JSONReader jsonReader = SRLoader.LoaderInstance.jr;
        // private SRReadWrite rw = SRLoaderForm._sr.LoaderInstance.rw;
        private SRMain jsonReader = SRLoaderForm._srLoader.jr;
        private SRLoader Loader = SRLoaderForm._srLoader;
        private SRReadWrite rw = SRLoaderForm._srLoader.rw;
        
        // Obsrever
        // private UnitTracker _unitTracker = new UnitTracker();
        // private UnitReporter _unitReporter = new UnitReporter("# Unit");

        // Gridview Initiliazation
        private int gvCurrentRow;
        private bool gvCurrentEnabledState;
        private bool gvCurrentRowName;

        private IList<Feature> warfareList;
        
        // Player Feature
        private Feature ArmyEfficiency;
        private Feature ArmyEntrenchment;
        private Feature ArmyActualStrength;
        private Feature ArmyCurrentStrength;
        private Feature ArmyMorale;
        private Feature ArmyExperience;
        private Feature ArmyFuel;
        private Feature ArmySupply;
        private Feature UnitFuelCapacity;
        private Feature UnitSuppliesCapacity;
        private Feature ArmyActiveStaff;
        private Feature ArmyReserve;
        private Feature UnitClass;
        
        // enemy Feature
        private Feature HoverArmyEfficiency;
        private Feature HoverArmyEntrenchment;
        private Feature HoverArmyActualStrength;
        private Feature HoverArmyCurrentStrength;
        private Feature HoverArmyMorale;
        private Feature HoverArmyExperience;
        private Feature HoverArmyFuel;
        private Feature HoverArmySupply;
        private Feature HoverUnitFuelCap;
        private Feature HoverUnitSupplyCap;
        private bool _isNaval;

        #endregion
        
        #region Constructor
        public XtraForm1()
        {
            InitializeComponent();

            barsiBtmTrainerStatus.Caption = jsonReader.Data.SRFStatus ? "Active" : "Inactive";
            checkedListBoxControl1.Items.Add(WarfareArmyEnum.Heal);
            checkedListBoxControl1.Items.Add(WarfareArmyEnum.Rambo);
            checkedListBoxControl1.Items.Add(WarfareArmyEnum.Str2x);
            checkedListBoxControl1.Items.Add(WarfareArmyEnum.Str4x);
            checkedListBoxControl1.Items.Add(WarfareArmyEnum.GasSupply2x);
            checkedListBoxControl1.Items.Add(WarfareArmyEnum.GasSupply4x);

            // Set Data Source for each Category control
            gcCountry.DataSource = jsonReader.FeaturesCountry;
            gcResources.DataSource = jsonReader.FeaturesResources;
            gcWarfare.DataSource = jsonReader.FeaturesWarfare;
            gcCountry.ForceInitialize();
            gcResources.ForceInitialize();
            gcWarfare.ForceInitialize();

            gvLoad(gvWarfare);
            gvLoad(gvResources);
            gvLoad(gvCountry);

            Deactivate += FormDeactivate;
            
            // _unitReporter.Subscribe(_unitTracker);

            // Init History Data Source from memento
            gcModifiedUnit.DataSource = SRMemento.Instance.UnitHistoryList;
            gcModifiedUnit.ForceInitialize();
            gvModifiedUnit.Columns["UnitAddress"].Visible = false;
            gvModifiedUnit.Columns["ModifiedStats"].Visible = false;
            gvModifiedUnit.Columns["UnitStats"].Visible = false;
            gvModifiedUnit.Columns["UnitName"].OptionsColumn.AllowEdit = false;
            gvModifiedUnit.Columns["UnitId"].OptionsColumn.AllowEdit = false;
            gvLoad(gvModifiedUnit);
            
            // Init Persistent From Memento
            gcPersistentUnit.DataSource = SRMemento.Instance.UnitPersistentList;
            gcPersistentUnit.ForceInitialize();
            gvPersistentUnit.Columns["UnitId"].Visible = false;
            gvPersistentUnit.Columns["Address"].Visible = false;
            gvPersistentUnit.Columns["Stats"].Visible = false;
            // gvPersistentUnit.Columns["DisplayStat"].Visible = false;
            gvPersistentUnit.Columns["UnitName"].OptionsColumn.AllowEdit = false;
            gvLoad(gvPersistentUnit);
            
            // _Helper = new FlashedCellsHelper(gvResources);
            // gvResources.CellValueChanged += gvCellValueChanged;

            // This line of code is generated by Data Source Configuration Wizard
            // Fill a JsonDataSource asynchronously
            // JSONGameVersion.FillAsync();
        }
        #endregion
        private void gvLoad(GridView gv)
        {
            gv.OptionsMenu.EnableColumnMenu = false;
            gv.OptionsMenu.EnableFooterMenu = false;
            gv.OptionsCustomization.AllowSort = false;
            gv.OptionsView.ColumnAutoWidth = true;
            // initFlashCell(gv);
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }

        private void barHeaderItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }

        void showHealthBar()
        {
            if (jsonReader.feature("UnitName").value == "") return;
            var UnitAct = jsonReader.feature("ArmyActualStrength");
            var UnitCur = jsonReader.feature("ArmyCurrentStrength");
            var UnitF = jsonReader.feature("ArmyFuel");
            var UnitS = jsonReader.feature("ArmySupply");
            var UnitFCap = jsonReader.feature("UnitFuelCapacity");
            var UnitSCap = jsonReader.feature("UnitSuppliesCapacity");
            var UnitClass = jsonReader.feature("UnitClass").value;

            var UnitSupply = decimal.Parse(UnitS.value);
            var UnitFuel = decimal.Parse(UnitF.value);
            var UnitFuelCap = decimal.Parse(UnitFCap.value);
            var UnitSupplyCap = decimal.Parse(UnitSCap.value);
            var UnitCurrent = decimal.Parse(UnitCur.value);
            var UnitActual = decimal.Parse(UnitAct.value);

            // pbUnitHealthBar.Tag = "Health";
            // pbUnitFuelBar.Tag = "Fuel";
            // pbUnitSupplyBar.Tag = "Supply";

            if (rw.SRIsNaval(UnitClass.StrToInt()))
                UnitActual = UnitActual * 100;
            pbUnitHealthBar.Properties.Maximum = (int) UnitActual;
            pbUnitFuelBar.Properties.Maximum = (int) ((int) UnitCurrent * UnitFuelCap);
            pbUnitSupplyBar.Properties.Maximum = (int) UnitCurrent * (int) UnitSupplyCap;

            pbUnitHealthBar.EditValue = (int) UnitCurrent;
            pbUnitFuelBar.EditValue = (int) UnitFuel;
            pbUnitSupplyBar.EditValue = (int) UnitSupply;


            // pbUnitHealthBar.Tag = UnitF;
        }

        private void InitInfo()
        {
            SRInfo.Instance.SRDonationButton(btnDonate, TrainerEnum.Paypal);
            SRInfo.Instance.SRDonationButton(btnDonorBoxDonate, TrainerEnum.DonorBox);
            SRInfo.Instance.SRBarDonationHeader(barBtnDonation);
            SRInfo.Instance.SRBarDonationButton(barBtnDonationBottom);
            barHeaderVersion.Caption = Loader.currentProductVersion;
            barStaticItem1.Caption = $@"Game Version: {jsonReader.activeTrainer.GameVersion}";

            SRInfo.Instance.SRProductInformation(reInfo);
            SRInfo.Instance.SRChangeLog(reInfoChangelog);
            SRInfo.Instance.SRLoadCheatTable(reExtraCheatTable);
        }

        private void XtraForm1_Load(object sender, EventArgs e)
        {
            InitInfo();
            // xtabMainControl.SelectedTabPage = xtabAbout;
            warfareList = jsonReader.seekWarfareVariable(WarfareArrayUtils.ArmyFeatureList);
            ArmyEfficiency = warfareList.Single(s => s.name == av.ArmyEfficiency.ToString());
            ArmyEntrenchment = warfareList.Single(s => s.name == av.ArmyEntrenchment.ToString());
            ArmyActualStrength = warfareList.Single(s => s.name == av.ArmyActualStrength.ToString());
            ArmyCurrentStrength = warfareList.Single(s => s.name == av.ArmyCurrentStrength.ToString());
            ArmyMorale = warfareList.Single(s => s.name == av.ArmyMorale.ToString());
            ArmyExperience = warfareList.Single(s => s.name == av.ArmyExperience.ToString());
            ArmyFuel = warfareList.Single(s => s.name == av.ArmyFuel.ToString());
            ArmySupply = warfareList.Single(s => s.name == av.ArmySupply.ToString());
            UnitFuelCapacity = warfareList.Single(s => s.name == av.UnitFuelCapacity.ToString());
            UnitSuppliesCapacity = warfareList.Single(s => s.name == av.UnitSuppliesCapacity.ToString());
            ArmyActiveStaff = warfareList.Single(s => s.name == av.ArmyActiveStaff.ToString());
            ArmyReserve = warfareList.Single(s => s.name == av.ArmyReserve.ToString());
            UnitClass = warfareList.Single(s => s.name == av.UnitClass.ToString());
            
            // Enemy variable init for easy call
            HoverArmyActualStrength = "HoverArmyActualStrength".GetFeature();
            HoverArmyCurrentStrength = "HoverArmyCurrentStrength".GetFeature();
            HoverArmyEfficiency = "HoverArmyEfficiency".GetFeature();
            HoverArmyExperience = "HoverArmyExperience".GetFeature();
            HoverArmyEntrenchment = "HoverArmyEntrenchment".GetFeature();
            HoverArmyMorale = "HoverArmyMorale".GetFeature();
            HoverArmyFuel = "HoverArmyFuel".GetFeature();
            HoverUnitFuelCap = "HoverUnitFuelCapacity".GetFeature();
            HoverArmySupply = "HoverArmySupply".GetFeature();
            HoverUnitSupplyCap = "HoverUnitSuppliesCapacity".GetFeature();

            // Load Steam Player Info
            barItemPlayerOnline.Caption = "Players: " + Loader.SteamPlayerCount;
            barBtnSteamPlayerRefresh.ItemClick += (o, args) =>
            {
                Task.Run(() =>
                {
                barItemPlayerOnline.Caption = "Players: .";
                Thread.Sleep(200);
                barItemPlayerOnline.Caption = "Players: ..";
                Thread.Sleep(200);
                barItemPlayerOnline.Caption = "Players: ...";
                Thread.Sleep(200);
                    barItemPlayerOnline.Caption = "Players: " + Loader.GetSteamPlayerCount().Result;
                });
            };

            ceModeHover.CheckStateChanged += (o, args) =>
            {
                jsonReader.FeatureArmyEnemyEnabled = ceModeHover.Checked;
            };

            // this.LookAndFeel.SetSkinStyle(SkinStyle.VisualStudio2013Dark);
            // var s = skinPaletteDropDownButtonItem1.DropDownControl as GalleryDropDown
            // Enablind timer after datasource set
            mainTimer.Enabled = true;
            if (UnitClass.value != string.Empty)
                _isNaval = rw.SRIsNaval(Convert.ToInt32(UnitClass.value));
            //SpinEdit
            setContorlTag(seStaffActive, "ArmyActiveStaff");
            setContorlTag(seStaffReserve, "ArmyReserve");
            setContorlTag(seNavalStrActual, "ArmyActualStrength");
            setContorlTag(seNavalStrCurrent, "ArmyCurrentStrength");
            setContorlTag(seUnitStrActual, "ArmyActualStrength");
            setContorlTag(seUnitStrCurrent, "ArmyCurrentStrength");
            setContorlTag(seUnitFuel, "ArmyFuel");
            setContorlTag(seUnitSupplies, "ArmySupply");
            //CheckEdit
            setContorlTag(ceEfficiency, "ArmyEfficiency");
            setContorlTag(ceExperience, "ArmyExperience");
            setContorlTag(ceMorale, "ArmyMorale");
            // setContorlTag(ceNavalStrength, "ArmyCurrentStrength,ArmyActualStrength");
            setContorlTag(ceUnitFuel, "ArmySupply");
            setContorlTag(ceUnitStrength, "ArmyCurrentStrength");
            setContorlTag(ceUnitSupplies, "ArmyFuel");
            // setSpinEdit(new SpinEdit[]{seNavalStrActual, seNavalStrCurrent}, ceNavalStrength.Tag.CastTo<List<Feature>>());
            // setSpinEdit(new []{seUnitStrActual, seUnitStrCurrent}, ceUnitStrength.Tag.CastTo<List<Feature>>());

            // When changed then changed
            setSpinEditorEvent(seUnitStrActual);
            setSpinEditorEvent(seUnitStrCurrent);
            setSpinEditorEvent(seNavalStrActual);
            setSpinEditorEvent(seNavalStrCurrent);
            setSpinEditorEvent(seUnitFuel);
            setSpinEditorEvent(seUnitSupplies);
            setSpinEditorEvent(seStaffReserve);
            // Set Global Event for multiple control
            // gvCountry.ValidateRow += new ValidateRowEventHandler(gvValidateRow);
            // gvResources.ValidateRow += new ValidateRowEventHandler(gvValidateRow);
            // gvWarfare.ValidateRow += new ValidateRowEventHandler(gvValidateRow);

            // gvCountry.ShowingEditor += gvShowingEditor;
            // gvResources.ShowingEditor += gvShowingEditor;
            // gvWarfare.ShowingEditor += gvShowingEditor;

            // gvCountry.ShowingEditor += gvShowingEditor;
            // gvResources.ShowingEditor += gvShowingEditor;
            // gvWarfare.ShowingEditor += gvShowingEditor;

            // When DATASOURCE of Gridview is changed
            gvCountry.DataSourceChanged += gvDataSourceChanged;
            gvResources.DataSourceChanged += gvDataSourceChanged;
            gvWarfare.DataSourceChanged += gvDataSourceChanged;
            gvModifiedUnit.DataSourceChanged += gvDataSourceChanged;
            gvPersistentUnit.DataSourceChanged += gvDataSourceChanged;

            // Method to set gridview init events etc.
            setGvEvents(gvCountry);
            setGvEvents(gvResources);
            setGvEvents(gvWarfare);

            // Populate Combo box warfare unit value
            PopulateWarfareTypes(lookBoxUnitClass, WarfareValueDictionary.Instance.DictUnitClassType);
            PopulateWarfareTypes(lookBoxUnitMovementType, WarfareValueDictionary.Instance.DictUnitMovementType);
            PopulateWarfareTypes(lookBoxUnitTargetType, WarfareValueDictionary.Instance.DictUnitTargetType);

            // Sort Datagrid record Order based on SRUL.Types collection,
            // jsonReader.dgOrder(gvCountry);
            // jsonReader.dgOrder(gvResources);
            // jsonReader.dgOrder(gvWarfare);
            // gvWarfare.Columns["original"].Visible = true;

            //Hihlight
            gvHighlight(gvCountry);
            gvHighlight(gvResources);
            gvHighlight(gvWarfare);

            //Set Row style for editable
            jsonReader.gvRowStyle(gvCountry);
            jsonReader.gvRowStyle(gvResources);
            jsonReader.gvRowStyle(gvWarfare);

            // gvCountry.FormatRules[0].Column = gvCountry.Columns["value"];

            // Custom Text Display Tonnes, $ sign, % when row is displayed
            // gvCountry.CustomColumnDisplayText += gvCustomColumnDisplayText;
            // gvWarfare.CustomColumnDisplayText += gvCustomColumnDisplayText;
            // gvResources.CustomColumnDisplayText += gvCustomColumnDisplayText;

            // gvCountry.CustomColumnSort += OnCustomColumnSort;

            // For Cell Editing custom, spinedit etc in a row.
            AddRepositoryItem(jsonReader.FeaturesCountry, gcCountry, gvCountry);
            AddRepositoryItem(jsonReader.FeaturesResources, gcResources, gvResources);
            AddRepositoryItem(jsonReader.FeaturesWarfare, gcWarfare, gvWarfare);

            // Flash Helper

            // SE percentage
            // SetSpinEditDisplayPercentage(seNavalStrActual);
            // SetSpinEditDisplayPercentage(seNavalStrCurrent);
            SetSpinEditDisplayPercentage(seNavalStrActual, true);
            SetSpinEditDisplayPercentage(seNavalStrCurrent, true);
            // pbUnitHealthBar.custom


            // Custom Display for HEALTH Bar, supposed to display overload value
            pbUnitHealthBar.Tag = "ArmyCurrentStrength";
            pbUnitFuelBar.Tag = "ArmyFuel";
            pbUnitSupplyBar.Tag = "ArmySupply";
            pbUnitHealthBar.CustomDisplayText += SetProgressBarDisplayOverload;
            pbUnitFuelBar.CustomDisplayText += SetProgressBarDisplayOverload;
            pbUnitSupplyBar.CustomDisplayText += SetProgressBarDisplayOverload;

            // Special Warfare Option
            checkedListBoxControl1.ItemCheck += (sd, args) =>
            {
                CheckedListBoxControl clbc = sd as CheckedListBoxControl;
                var bl = clbc.Items[WarfareArmyEnum.Rambo].CheckState == CheckState.Checked;
                if (bl)
                {
                    ceEfficiency.Checked = true;
                    ceExperience.Checked = true;
                    ceMorale.Checked = true;
                }
                else
                {
                    ceEfficiency.Checked = false;
                    ceExperience.Checked = false;
                    ceMorale.Checked = false;
                }
            };
            
            // 2.0.0.0 Unit history and Persistent
            btnRemovePersistentUnit.Click += (s, args) =>
            {
                if (gvPersistentUnit.RowCount < 1) return;
                var selectedRowIndex = gvPersistentUnit.GetSelectedRows()[0];
                var rowValue = gvPersistentUnit.GetRowCellValue(selectedRowIndex, "Address");
                SRMemento.Instance.RemovePersistantUnit(rw, rowValue.ToString());
                // gcPersistentUnit.DataSource = SRMemento.Instance.UnitPersistentList;
                gcPersistentUnit.RefreshDataSource();
            };

            btnRemoveAllPersistentUnit.Click += (s, args) =>
            {
                if (gvPersistentUnit.RowCount < 1) return;
                SRMemento.Instance.RemoveAllPersistantUnit(rw);
                gcPersistentUnit.RefreshDataSource();
            };
            
            btnMakePersistent.Click += (o, args) =>
            {
                if(!SRMemento.Instance.UnitIsPersistent("ArmyCurrentStrength".GetPointer(rw))) 
                    SRMemento.Instance.MakePersistent(rw, jsonReader.FeaturesWarfare);
                gcPersistentUnit.RefreshDataSource();
            };

            void btnRemoveTrackedUnit()
            {
                if(!SRMemento.Instance.UnitIsPersistent("ArmyCurrentStrength".GetPointer(rw)))
                    SRMemento.Instance.RemovePersistantUnit(rw, "ArmyCurrentStrength".GetPointer(rw));
                gcPersistentUnit.RefreshDataSource();
            }

            BarTimerEnabler.Checked = true;
            BarTimerEnabler.CheckedChanged += (o, args) =>
            {
                var enabler = o as BarCheckItem;
                if (enabler.Checked)
                {
                    mainTimer.Enabled = true;
                }
                else
                {
                    mainTimer.Enabled = false;
                }
            };

            barSettingsBtnReloadTrainerData.ItemClick += (o, args) =>
            {
                Loader.ReloadTrainerData();
            };

            // Restore Original Stat Value
            btnRestoreUnitToOriginal.Click += (o, args) =>
            {
                if (gvModifiedUnit.RowCount < 1) return;
                var selectedRowIndex = gvModifiedUnit.GetSelectedRows()[0];
                var rowValue = gvModifiedUnit.GetRowCellValue(selectedRowIndex, "UnitAddress");
                SRMemento.Instance.RestoreToOriginal(rowValue.ToString(), jsonReader.FeaturesWarfare, rw);
                
                gcModifiedUnit.RefreshDataSource();
            };
            
            gvPersistentMasterDetail();
        }

        private void SetProgressBarDisplayOverload(object sender, CustomDisplayTextEventArgs e)
        {
            ProgressBarControl pbc = sender as ProgressBarControl;
            string unitSelected = jsonReader.feature("UnitSelected", jsonReader.FeaturesWarfare).value;
            Feature feature = jsonReader.feature(pbc.Tag.ToString(), jsonReader.FeaturesWarfare);
            // Feature realTag = pbc.Properties.Tag as Feature;
            if (pbc.Position == pbc.Properties.Maximum)
            {
                pbc.Position = pbc.Position - 1;
            }

            if (pbc != null && unitSelected.StrToInt() > 0)
            {
                if (pbc.Properties.Maximum == 0) return;
                if (pbc.Position == 0)
                {
                    e.DisplayText = "Empty";
                }

                // var tg = SystemExtension.StrToInt(pbc.Tag.ToString());
                var p = Convert.ToDouble(Convert.ToDecimal(feature.value) /
                                         Convert.ToDecimal(pbc.Properties.Maximum.ToString()));
                // if (pbc.Position == 0 && pbc.Properties.Maximum > 0) return;
                // if (pbc.Position >= pbc.Properties.Maximum)
                // {
                // memoEdit1.EditValue += pbc.Name + "\n"; 
                // memoEdit1.EditValue += pbc.Position.ToString() + "\n";
                // memoEdit1.EditValue += pbc.Properties.Maximum.ToString() + "\n\n";
                e.DisplayText = String.Format("{0:P1}", p);
            }

            // e.DisplayText = pbc.Position.ToString();
            // }
        }

        private void SetSpinEditDisplayPercentage(SpinEdit se, bool isNaval)
        {
            if (isNaval)
            {
                se.Properties.Increment = 1;
                se.Properties.IsFloatValue = true;
                se.Properties.DisplayFormat.FormatType = FormatType.Numeric;
                se.Properties.DisplayFormat.FormatString = "P";
                se.Properties.Mask.EditMask = "p";
            }
            else
            {
                se.Properties.Increment = 10;
                se.Properties.IsFloatValue = false;
                se.Properties.Mask.EditMask = default;
                se.Properties.DisplayFormat.FormatType = default;
                se.Properties.DisplayFormat.FormatString = default;
            }
        }

        private void setGvEvents(GridView gv)
        {
            // Set Global Event for multiple control, SR Write when validated.
            gv.ValidateRow += gvValidateRow;
            // When editor is showing, change ?
            gv.ShowingEditor += gvShowingEditor;
            // When DATASOURCE of Gridview is changed
            gv.DataSourceChanged += gvDataSourceChanged;
            // Custom Text Display Tonnes, $ sign, % when row is displayed
            gv.CustomColumnDisplayText += gvCustomColumnDisplayText;
            // Set Custom Row Filter for Warfare;
            gv.CustomRowFilter += jsonReader.gvRowFilterExclusion;
            // Sort Datagrid record Order based on SRUL.Types collection,
            jsonReader.dgOrder(gv);
        }

        private void gvCellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            // int speed = (int) e.Value;
            view.RefreshData();
        }

        private void AddRepositoryItem(IList<Feature> fs, GridControl gc, GridView gv)
        {
            RepositoryItemSpinEdit spin = new RepositoryItemSpinEdit();
            RepositoryItemLookUpEdit lookUp = new RepositoryItemLookUpEdit();
            gc.RepositoryItems.AddRange(new RepositoryItem[] {spin, lookUp});

            // Set Spin Properties
            spin.AllowMouseWheel = true;
            spin.AllowNullInput = DefaultBoolean.False;
            spin.ValidateOnEnterKey = true;
            // foreach (var feature in fs)
            // {
            //     if (feature.format == "percentage")
            //     {
            //         spin.Increment = 100;
            //     }
            //     else
            //     {
            //         spin.Increment = 2;
            //     }
            // }

            // Editing All GRID VIEW with spinEditor
            void gvCustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
            {

                GridView gView = sender as GridView;
                ColumnView cView = sender as ColumnView;

                var formatType = (string) gView.GetListSourceRowCellValue(e.RowHandle, "format");
                var valueType = (string) gView.GetListSourceRowCellValue(e.RowHandle, "type");
                var name = (string) gView.GetListSourceRowCellValue(e.RowHandle, "name");

                spin.Mask.EditMask = default;
                if (e.Column.FieldName != "value") return;
                switch (formatType.ToLower())
                {
                    case "opinion":
                        // Enum[] en = {
                        //     WorldUNOpinion.Unknown,
                        //     WorldUNOpinion.Concerned,
                        //     WorldUNOpinion.Outraged,
                        //     WorldUNOpinion.Dissaproving,
                        //     WorldUNOpinion.Indifferent,
                        //     WorldUNOpinion.Pleased,
                        //     WorldUNOpinion.Satisfied,
                        //     WorldUNOpinion.Delighted,
                        // };
                        //
                        // lookUp.DataSource = en;
                        // // lookUp.Columns.Add(new LookUpColumnInfo("test", "test"));
                        // lookUp.PopulateColumns();
                        // e.RepositoryItem = lookUp;
                        break;
                    case "currency":
                        if (name.ToLower() == "treasury")
                        {
                            // spin.Mask.EditMask = "N";
                            spin.Increment = 1000000000000;
                        }
                        else spin.Increment = 2000;

                        break;
                    case "percentage":
                        spin.Mask.EditMask = "#############.0%";
                        spin.Increment = (decimal) float.Parse("0,1");
                        break;
                    default:
                        // spin.Increment = (decimal) float.Parse("0,1");
                        break;
                }

                if (formatType.Contains("volumes"))
                {
                    spin.Mask.EditMask = "####,###,###,###,###";
                    spin.Increment = 1000000;
                }

                switch (valueType)
                {
                    case "float":
                        spin.IsFloatValue = true;
                        break;
                    default:
                        spin.IsFloatValue = false;
                        break;
                }

                // if(formatType.ToLower() != "opinion")
                e.RepositoryItem = spin;
            }

            // Spin repisotry validation;
            spin.Validating += (sender, args) =>
            {
                // gv.PostEditor();
            };

            void SaveToHistory(IList<Feature> unit, string statName, string statValue)
            {
                // var unitName = unit.GetFeature("UnitName").value;
                if (unit[0].category.ToLower() != "warfare") return;
                if (SRMemento.Instance.SaveToHistory(unit, rw))
                {
                    SRMemento.Instance.AddModifiedStat(rw, statName, statValue);
                    gvModifiedUnit.RefreshData(); 
                }
            }

            // Should save History Here
            // Spin repository when edit value changed.
            spin.EditValueChanged += (object sender, EventArgs e) =>
            {
                gv.PostEditor();
            };

            // Specific Gridview when validating 
            gv.ValidatingEditor += (sender, args) =>
            {
                GridView gView = sender as GridView;
                if (gView.FocusedColumn == gView.Columns["value"])
                {
                    var name = gView.GetRowCellValue(gView.FocusedRowHandle, "name");
                    SaveToHistory(fs, name.ToString(), args.Value.ToString());
                    rw.SRWrite(name.ToString(), args.Value.ToString());
                    gv.PostEditor();
                    // DateTime? orderDate = view.GetRowCellValue(view.FocusedRowHandle, colOrderDate) as DateTime?;
                    // if (requiredDate < orderDate) {
                    //     e.Valid = false;
                    //     e.ErrorText = "Required Date is earlier than the order date";
                    // }
                }
            };

            // Add custom editing component to gridview row
            gv.CustomRowCellEditForEditing += gvCustomRowCellEditForEditing;
        }

        private WorldUNOpinion ConvertWorldOpinion(float opinion)
        {
            if (opinion <= 0.10)
                return WorldUNOpinion.Outraged;
            else if (opinion < 0.30)
                return WorldUNOpinion.Dissaproving;
            else if (opinion < 0.40)
                return WorldUNOpinion.Concerned;
            else if (opinion < 0.50)
                return WorldUNOpinion.Indifferent;
            else if (opinion < 0.60)
                return WorldUNOpinion.Satisfied;
            else if (opinion < 0.70)
                return WorldUNOpinion.Pleased;
            else
                return opinion > 0.70 ? WorldUNOpinion.Delighted : WorldUNOpinion.Unknown;
        }

        private void gvCustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            CultureInfo ciUSA = new CultureInfo("en-US");
            // CultureInfo ciUSA = new Cul("en-US");
            // if (e.Column.FieldName != "value" || e.ListSourceRowIndex == GridControl.InvalidRowHandle) return;
            if (e.Column.FieldName != "value") return;
            var formatType = (string) view.GetListSourceRowCellValue(e.ListSourceRowIndex, "format");
            var type = (string) view.GetListSourceRowCellValue(e.ListSourceRowIndex, "type");
            var name = (string) view.GetListSourceRowCellValue(e.ListSourceRowIndex, "name");
            var value = (string) view.GetListSourceRowCellValue(e.ListSourceRowIndex, "value");
            // if (e.Column.Caption != "value") return;
            if (e.Value == null || e.Value == "") return;
            if (type == "string" || type == "byte") return;
            decimal formattedValue = Convert.ToDecimal(value == "" ? "0" : value);
            // var formattedValue = rw.SRRead(value, true);
            // var formattedValue = e.Value;
            switch (formatType)
            {
                case "opinion":
                    e.DisplayText = ConvertWorldOpinion((float) formattedValue).ToString();
                    break;
                case "currency":
                    // e.DisplayText = String.Format(ciUSA,"{0:C6}M", formattedValue);
                    e.DisplayText = String.Format(new MoneyFormat(), "{0:M}", formattedValue);
                    break;
                case "percentage":
                    e.DisplayText = String.Format("{0:P}", formattedValue);
                    break;
                case "volumes,Tonnes":
                    e.DisplayText = String.Format("{0:N} - Tonnes", formattedValue);
                    break;
                case "volumes,Barrels":
                    e.DisplayText = String.Format("{0:N} - Barrels", formattedValue);
                    break;
                case "volumes,kg":
                    e.DisplayText = String.Format("{0:N} - kg", formattedValue);
                    break;
                case "volumes,MWh":
                    e.DisplayText = String.Format("{0:N} - MWh", formattedValue);
                    break;
                case "volumes,m3":
                    e.DisplayText = String.Format("{0:N} - m3", formattedValue);
                    break;
            }
        }

        void updateSpinEdit(bool frz, SpinEdit ctrl)
        {
            if (frz)
                // ctrl.EditValue = rw.SRRead(ctrl.Tag.ToString());
                // rw.SRFreeze(ctrl.Tag.ToString(), ctrl.EditValue.ToString(), tglFreezeAllowIncrease.Checked);
                rw.SRWrite(ctrl.Tag.ToString(), ctrl.EditValue.ToString());
            else
                ctrl.EditValue = jsonReader.feature(ctrl.Tag.ToString()).value;
            // ctrl.EditValue = rw.SRRead(ctrl.Tag.ToString());
        }

        void setContorlTag(CheckEdit ce, string tag)
        {
            var temp = new List<Feature>();

            // If CheckBox tag contains , split
            if (tag.Contains(","))
            {
                var split = tag.Split(',');

                // Add splitted varname in tag to Temp List
                foreach (var str in split)
                {
                    temp.Add(jsonReader.feature(str));
                }

                // Apply checkbox Tag with Temporary list
                // Add Event when checkstate is changed
                ce.Tag = temp;
                ce.CheckStateChanged += ceCheckStateChangedEvent;
            }
            else
            {
                // Else, contains only 1 varname param, then do the same.
                temp.Add(jsonReader.feature(tag));
                ce.Tag = temp;
                ce.CheckStateChanged += ceCheckStateChangedEvent;
            }
        }

        void setContorlTag(SpinEdit se, string tag)
        {
            se.Tag = tag;
        }

        // Set Spin Edit
        void setSpinEdit(SpinEdit[] se, IList<Feature> f)
        {
            // var ctg = f;

            foreach (var spin in se)
            {
                var ft = f.First(s => spin.Tag.ToString() == s.name);
                spin.EditValue = ft.value;
            }

            // f.Where( s => )
            // for (var i = 0; i < f.Count; i++)
            // {
            //     if (f[i].name == se[i].Tag)
            //     {
            //         se[i].EditValue = f[i].value;
            //     }
            // }
        }

        void UnitClassLookUpEditHandler(string featName, LookUpEdit le)
        {
            var dtsVal = le.Properties.GetDataSourceValue("Key", Int32.Parse(rw.SRRead(featName).ToString()));
            if (le.Focused)
            {
                if (le.EditValue != null && le.ItemIndex != null)
                {
                    // le.focu
                    rw.SRWrite(featName, le.ItemIndex.ToString(), "int");
                    // labelControl6.Text = le.EditValue.ToString();
                }
            }
            else
            {
                // Dillematic, supposed take from json which coupled, or just the varname but coupled to r/w
                if (jsonReader.feature(featName).value != null)
                {
                    le.EditValue = dtsVal;
                }
            }
        }

        // void leValidated(object sender, validated)
        void OnCustomColumnSort(object sender, CustomColumnSortEventArgs e)
        {
            // if (e.Column.FieldName == "displayName" || e.Column.FieldName == "display Name")
            // {
            //     int dayIndex1 = getIndexing((string)e.Value1);
            //     int dayIndex2 = 0;
            //     e.Result = dayIndex1.CompareTo(dayIndex2);
            //     e.Handled = true;
            // }
        }

        private void PopulateWarfareTypes(LookUpEdit leControl, Dictionary<int, string> dict)
        {
            // var a = new WarfareValueDictionary();
            leControl.Properties.DataSource = dict;
            leControl.Properties.DisplayMember = "Value";
            leControl.Properties.ValueMember = "Key";
            // leControl.EditValue = 0;
            leControl.ItemIndex = 0;
        }

        private void gvCountry_CustomColumnSort(object sender,
            DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
        {
            if (e.Column.FieldName == "displayName")
            {
                e.Handled = true;
            }
        }

        //When DAtasource changed by updatedaragridview()
        private void gvDataSourceChanged(object sender, EventArgs e)
        {
            GridView view = (GridView) sender;
            // gvHighlight();
            view.PostEditor();
            // view.refre
        }

        private void GvPersistentDetailRuntime()
        {
            cePersistentUnitIndicator.ReadOnly = true;
            var trackedRowId = SRMemento.Instance.GetTrackedRowId(rw);
            if (trackedRowId != -1)
            {
                cePersistentUnitIndicator.Text = "Tracked Unit (" + trackedRowId + ")";
                cePersistentUnitIndicator.ForeColor = Color.LimeGreen;
                cePersistentUnitIndicator.CheckState = CheckState.Checked;
                btnMakePersistent.Enabled = false;
                btnMakePersistent.Text =
                    string.Format("({0}) [{1}] is tracked", trackedRowId, "UnitName".GetFeature().value);
                btnMakePersistent.ForeColor = Color.DarkCyan;
            }
            else
            {
                cePersistentUnitIndicator.ForeColor = Color.DimGray;
                cePersistentUnitIndicator.Text = "Untracked Unit";
                cePersistentUnitIndicator.CheckState = CheckState.Unchecked;
                btnMakePersistent.Enabled = !string.IsNullOrEmpty("UnitName".GetFeature().value);
                btnMakePersistent.Text = "Track selected unit: [" + "UnitName".GetFeature().value + "]";
                btnMakePersistent.ForeColor = Color.Cyan;
            }
            // Debug.WriteLine(gvPersistentUnit.FocusedColumn.FieldName);
            // Debug.WriteLine(gvPersistentUnit.ChildGridLevelName);
            // Debug.WriteLine(gvPersistentUnit.DetailLevel);
            // Debug.WriteLine(gvPersistentUnitStats.FocusedColumn.FieldName);
            // Debug.WriteLine(gvPersistentUnitStats.ChildGridLevelName);
            // Debug.WriteLine(gvPersistentUnitStats.DetailLevel);
            // if(gvPersistentUnitStats.getselec)
        }

        private void gvPersistentMasterDetail()
        {
            gvPersistentUnit.OptionsDetail.ShowDetailTabs = false;
            gvPersistentUnit.MasterRowExpanded += (sender, e) =>
            {
                GridView dView = gvPersistentUnit.GetDetailView(e.RowHandle, (sender as GridView).GetVisibleDetailRelationIndex(e.RowHandle)) as GridView;
                SRMemento.Instance.InitCustomPersitentEditor(rw, gcPersistentUnit, dView);
            };
            // gvPersistentUnitStats.editor
            // GridLevelNode node = gcPersistentUnit.LevelTree.Nodes["Stats"];
            // // if(node == null) return
            // GridView test= new GridView(gcPersistentUnit);
            // node.LevelTemplate = test;
            // gvPersistentUnitStats.MasterRowGetChildList += ()
            // gvLoad(gvPersistentUnit);
            // gvLoad(gvPersistentUnitStats);

            // gvPersistentUnit.OptionsBehavior.Editable = true;
            // gvPersistentUnitStats.OptionsBehavior.Editable = true;
            // gvPersistentUnitStats.OptionsDetail.ShowDetailTabs = false;

            // gvPersistentUnit.MasterRowEmpty += (sender, args) =>
            // {
            //     GridView gv = sender as GridView;
            //     PersistentUnit pu = gv.GetRow(args.RowHandle) as PersistentUnit;
            //     if (pu != null)
            //         args.IsEmpty = !SRMemento.Instance.UnitPersistentList.Any(s => s.Address == pu.Address);
            // };
            // gvPersistentUnit.MasterRowGetChildList += (sender, args) =>
            // {
            //     GridView gv = sender as GridView;
            //     PersistentUnit pu = gv.GetRow(args.RowHandle) as PersistentUnit;
            //     if (pu != null)
            //     {
            //         args.ChildList = pu.DisplayStats.ToList();
            //     }
            //     // gvPersistentUnitStats.Columns["Value"].OptionsColumn.AllowEdit = true;
            // };
            //
            // // gvPersistentUnitStats.
            //
            // gvPersistentUnit.MasterRowGetRelationCount += (sender, args) =>
            // {
            //     args.RelationCount = 1;
            // };
            //
            // gvPersistentUnit.MasterRowGetRelationName += (sender, args) =>
            // {
            //     args.RelationName = "Unit Stats";
            // };
        }

        private void gvRowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            var colName = e.Column.FieldName == "value";
            if (e.Column.FieldName == "value")
            {
                e.Appearance.BackColor = Color.Aqua;
            }
        }

        // Highlight GRIDVIEW when value increased
        private void gvHighlight(GridView gv)
        {
            GridFormatRule highlight(string ruleName,
                string iconName,
                string colorFill,
                FormatConditionDataUpdateTrigger trigger)
            {
                GridFormatRule gridFormatRule = new GridFormatRule();
                FormatConditionRuleDataUpdate dataUpdate = new FormatConditionRuleDataUpdate();

                gridFormatRule.Column = gv.Columns["value"];
                gridFormatRule.Name = ruleName;
                dataUpdate.HighlightTime = 500;
                dataUpdate.Icon.PredefinedName = iconName;
                dataUpdate.PredefinedName = colorFill;
                dataUpdate.Trigger = trigger;
                gridFormatRule.Rule = dataUpdate;
                gridFormatRule.ApplyToRow = true;
                return gridFormatRule;
            }

            ;
            // foreach (GridColumn gvColumn in gv.Columns)
            // {
            //     gridFormatRule.Column = gvColumn;
            // }
            // gridFormatRule.Column = "value";
            gv.FormatRules.Add(highlight("Format1",
                "Flags3_1.png",
                "Green Fill",
                FormatConditionDataUpdateTrigger.ValueIncreased));
            gv.FormatRules.Add(highlight("Format2",
                "Flags3_1.png",
                "Red Fill",
                FormatConditionDataUpdateTrigger.ValueDecreased));
        }

        // When editor is shown, if editable is false, couldnt get edited
        private void gvShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = (GridView) sender;
            bool isEditable = (bool) view.GetRowCellValue(view.FocusedRowHandle, "editable");
            e.Cancel = !isEditable;
        }

        // When row validated write to MEM
        private void gvValidateRow(object sender, ValidateRowEventArgs e)
        {
            GridView view = (GridView) sender;
            if (view.FocusedColumn != view.Columns["value"]) return; 
            var rowValue = view.GetRowCellValue(e.RowHandle, "value");
            var rowName = view.GetRowCellValue(e.RowHandle, "name");
            rw.SRWrite(rowName.ToString(), rowValue.ToString());
        }

        private void UpdateDataGridView(XtraTabPage xtab, GridView gv, IList<Feature> f)
        {
            if (!jsonReader.activeTrainer.GameValidated) return;

            for (int i = 0; i < f.Count; i++)
            {
                if(!f[i].enabled) continue;
                if (f[i].freeze)
                {
                    // DIRTY HACK
                    // if(f[i].name != "ArmyExperience") 
                    rw.SRFreeze(f[i].name, f[i].value, tglFreezeAllowIncrease.Checked);
                    f[i].value = rw.SRRead(f[i].name);
                }
                else
                {
                    // if(rw.SRRead(f[i].name).GetType().ToString() == "ïnt") 
                    // f[i].value = rw.SRRead(f[i].name).ToString();
                    // else
                    if (rw.SRRead(f[i].name).GetType().ToString() == "byte")
                        f[i].value = rw.SRRead(f[i].name);
                    else
                        f[i].value = rw.SRRead(f[i].name).ToString();
                    // gv.PostEditor();
                }
            }

            if (xtabMainControl.SelectedTabPage != xtab) return;
            for (int j = 0; j < gv.DataRowCount; j++)
            {
                // var rowName = gv.GetRowCellValue(j, "name");
                // var rowGridId = gv.GetRowCellValue(j, "gridId");
                
                if (gv.GetRowCellValue(j, "gridId") != (object) j)
                    gv.SetRowCellValue(j, gv.Columns["gridId"], j);
                gv.RefreshRowCell(j, gv.Columns["value"]);
                gv.RefreshRowCell(j, gv.Columns["original"]);
                gv.RefreshRowCell(j, gv.Columns["freeze"]);
                // var rNameValue = gv.GetDataRow(j)["name"];
                // var rValueValue = gv.GetDataRow(j)["value"];
                // var rFreeze = gv.GetDataRow(j)["freeze"];
                // if((bool)rFreeze)
                //     rw.SRFreeze(rNameValue.ToString(), rValueValue.ToString());
            }
            // gv.PostEditor();

            // gv.PostEditor();
            // gv.RefreshData();
        }

        private void UpdateUnitHistoryList(IList<Feature> currentUnitStats)
        {
            if (jsonReader.activeTrainer.GameValidated)
                UnitHistoryList.Instance.AddIfNotExists(currentUnitStats);
            var unitAddress = rw.GetCode(SRMain.Instance.pointerStore("ArmyCurrentStrength"));
            var unitName = jsonReader.getUnitName(currentUnitStats);
            var unitId = jsonReader.getUnitName(currentUnitStats);
            var ustats = UnitHistoryList.Instance.GetUnitOriginalValueByName(unitName);
            // _unitTracker.AddToObserve(new Unit(unitAddress.ToString(), unitId, unitName, currentUnitStats));

            for (int i = 0; i < currentUnitStats.Count; i++)
            {
                if (ustats != null)
                {
                    var original = ustats[i].value;
                    if (original != null) currentUnitStats[i].original = original;
                }
            }
            
            if (!ceModePersistent.Checked) return;
            if(SRMemento.Instance.MakePersistent(rw, currentUnitStats))
                gvPersistentUnit.RefreshData();
        }
        private void MissileMadness()
        {
            if (ceMissiles.Checked && Convert.ToInt32(seMissiles.EditValue) >= 0)
            {
                var intMissile = Convert.ToInt32(seMissiles.EditValue);
                if (intMissile < int.MaxValue)
                {
                    var missileValue = intMissile.ToString();
                    rw.SRWrite("ArmyMissileAvailableStorageQuantity", missileValue);
                    rw.SRWrite("ArmyMissileAvailableCargoQuantity", missileValue);
                    rw.SRWrite("ArmyMissileStrategicPoolAssigned", missileValue);
                    rw.SRWrite("ArmyMissileStrategicPoolReserve", missileValue);
                }
            }
        }

        private void SpecialOption()
        {
            if (cbADayBuild.Checked)
                rw.SRWrite("ADayBuild",
                    rw.SRRead("ADayBuild", true) >= 1 ? "1" : "0.999");
            if (cbADayArmy.Checked)
                rw.SRWrite("ADayArmy", "0.001");
            if (cbADayResearch.Checked)
            {
                rw.SRWrite("ADayResearchClick", "0.001");
                rw.SRWrite("ADayResearchTooltip", "0.001");
            }

            if (ceSatComm.Checked)
                rw.SRWrite("SatelliteCommCoverage", "1");
            if (ceSatMilDef.Checked)
                rw.SRWrite("SatelliteMissileDefenseCoverage", "1");
            if (ceSatRecon.Checked)
                rw.SRWrite("SatelliteReconCoverage", "1");

            switch (rgResearchEfficiency.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    rw.SRWrite("ResearchEfficiency", "2");
                    break;
                case 2:
                    rw.SRWrite("ResearchEfficiency", "4");
                    break;
                case 3:
                    rw.SRWrite("ResearchEfficiency", "8");
                    break;
                case 4:
                    rw.SRWrite("ResearchEfficiency", "100");
                    break;
                default:
                    break;
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            // rw.IsTrainerNeedRestart();
            GvPersistentDetailRuntime();
            // string unitName = rw.SRRead("UnitName");
            // string unitID = rw.SRRead("UnitID");
            string unitSelected = jsonReader.feature("UnitSelected", jsonReader.FeaturesWarfare).value;
            string unitDeployed = jsonReader.feature("UnitDeployed", jsonReader.FeaturesWarfare).value;
            string unitReserved = jsonReader.feature("UnitReserve", jsonReader.FeaturesWarfare).value;
            string unitBattleGroup = jsonReader.feature("UnitBattleGroup", jsonReader.FeaturesWarfare).value;
            int unitPDeployed = (unitDeployed.StrToInt());
            int unitPReserved = unitReserved.StrToInt();
            int unitTotal = unitPDeployed + unitPReserved;

            // Selected/Battle Groups For Show
            memoEdit1.EditValue = $@"Selected: {unitSelected}
Deployed: {unitDeployed} ({NumericExtension.SafePercentage(unitPDeployed, unitTotal)})
Reserved: {unitReserved} ({NumericExtension.SafePercentage(unitPReserved, unitTotal)})
Battle Group: {unitBattleGroup}";

            SpecialOption();
            MissileMadness();
            // countryInfoTable.UpdateTable();
            UpdateDataGridView(xtabResources, gvResources, jsonReader.FeaturesResources);
            UpdateDataGridView(xtabCountry, gvCountry, jsonReader.FeaturesCountry);
            UpdateDataGridView(xtabWarfare, gvWarfare, jsonReader.FeaturesWarfare);
            UpdateUnitHistoryList(jsonReader.FeaturesWarfare); 
            _isNaval = rw.SRIsNaval(Convert.ToInt32(UnitClass.value));

            // UpdateUnitHistoryList(gvUnitHistoryList);
            // gvUnitHistoryList.DataSource = UnitHistoryList.Instance.unitIdList;
            // gvUnitHistory.DataSource = gcUnitHistoryList
            // So pointer will fetch after updated data grid

            // updateSpinEdit(ceUnitStrength.Checked, seUnitStrActual);

            // update Spinedit EDIT VALUE
            // updateSpinEdit(ceUnitStrength.Checked, seUnitStrCurrent);
            // updateSpinEdit(ceUnitFuel.Checked, seUnitFuel);
            // updateSpinEdit(ceUnitSupplies.Checked, seUnitSupplies);

            // Unit Movement/target/class Refresh every tick

            // ARMY - Class updater and changer. 
            UnitClassLookUpEditHandler("UnitMovementType", lookBoxUnitMovementType);
            UnitClassLookUpEditHandler("UnitTargetType", lookBoxUnitTargetType);
            UnitClassLookUpEditHandler("UnitClass", lookBoxUnitClass);


            // Reader unified
            ArmyControlReader();
            ArmyControlWriter();
            EnemyReader();
            EnemyWriter();

            // Set specific spinedit to display % when naval is selected
            SetSpinEditDisplayPercentage(seUnitStrActual, _isNaval);
            SetSpinEditDisplayPercentage(seUnitStrCurrent, _isNaval);

            // READ ARMY ACTIVE STAFF REES
            // UnitReader(seStaffActive, jsonReader.FeaturesWarfare);
            // UnitReader(seStaffReserve, jsonReader.FeaturesWarfare);
            // UnitReader(seUnitFuel, jsonReader.FeaturesWarfare);
            // UnitReader(seUnitGas, jsonReader.FeaturesWarfare);

            // This update Army Naval Health blablabla
            // calculateHealth();

            // Multiplayer Protection
            MultiplayerProtection();
        }

        // Added protection for multiplayer use to fail
        public void MultiplayerProtection()
        {
            if (rw.SRRead("MultiplayerReadyState", true, jsonReader.FeaturesSpecial) != 1) return;
            mainTimer.Enabled = false;
            XtraMessageBox.Show("No Multiplayer! sorry :)");
            Application.Exit();
        }

        public void ArmyControlReader()
        {
            // seStaffActive.ismo

            Int64 converter(string val)
            {
                return Convert.ToInt64(decimal.Parse(val));
            }

            // Active Staff
            seStaffActive.EditValue = ArmyActiveStaff.value;
            if (!seStaffReserve.IsEditorActive)
                seStaffReserve.EditValue = ArmyReserve.value;

            if (!seUnitStrCurrent.IsEditorActive && !ceUnitStrength.Checked)
                seUnitStrCurrent.EditValue = converter(ArmyCurrentStrength.value);
            if (!seUnitStrActual.IsEditorActive)
                seUnitStrActual.EditValue = converter(ArmyActualStrength.value);
            // }

            // Fuel Editor
            if (!seUnitFuel.IsEditorActive && !ceUnitFuel.Checked)
                seUnitFuel.EditValue = converter(ArmyFuel.value);
            seUnitActualFuel.EditValue =
                (int) (decimal.Parse(ArmyCurrentStrength.value) * decimal.Parse(UnitFuelCapacity.value));

            // Supplies Editor
            if (!seUnitSupplies.IsEditorActive && !ceUnitFuel.Checked)
                seUnitSupplies.EditValue = converter(ArmySupply.value);
            seUnitActualSupplies.EditValue =
                (int) (decimal.Parse(ArmyCurrentStrength.value) * decimal.Parse(UnitSuppliesCapacity.value));

            // Health Bars
            showHealthBar();

            // Morale Bars
            showBar(pbArmyMoraleBar, ArmyMorale, 100);
            // Enemy(() => showBar(pbArmyMoraleBar, EnemyArmyMorale, 100));

            // Efficiency Bar
            showBar(pbArmyEfficiencyBar, ArmyEfficiency, 120);
            
            // Entrenchment Bar
            showBar(pbArmyEntrenchmentBar, ArmyEntrenchment, 120);

            // Experience Stars
            showExperienceStar(ArmyEfficiency);
        }

        private void showExperienceStar(Feature armyexp)
        {
            if (ceExperience.Checked) return;
            if (armyexp.value.StrToDouble() < 0.25)
                rtExperience.Rating = 0;
            if (ArmyExperience.value.StrToDouble().IsBetween(0.25, 1))
                rtExperience.Rating = 1;
            if (ArmyExperience.value.StrToDouble().IsBetween(1, 2.25))
                rtExperience.Rating = 2;
            if (ArmyExperience.value.StrToDouble().IsBetween(2.24, 99999))
                rtExperience.Rating = 3;
        }

        private decimal ConvertedExperienceRating()
        {
            return rtExperience.Rating == 3 ? 5 : rtExperience.Rating;
        }

        private void Hover(Action a)
        {
            if (ceModeHover.Checked && SRMain.Instance.FeatureArmyEnemyEnabled)
                a();
        }

        private void EnemyWriter()
        {
            // if (!checkEdit1.Checked) return;
            // if (ceMorale.Checked)
            //     EnemyArmyMorale.WriteDecimalTo(rw, 10);
            // if (ceExperience.Checked)
            //     EnemyArmyExperience.WriteDecimalTo(rw, 10);
            // if (ceEfficiency.Checked)
            //     EnemyArmyEfficiency.WriteDecimalTo(rw, 10);
        }
        private void EnemyReader()
        {
                // EnemyArmyCurrentStrength.SetFromRead(rw, false);
                // EnemyArmyActualStrength.SetFromRead(rw, false);
                // EnemyArmyCurrentStrength.SetFromRead(rw, false);
                // EnemyArmyExperience.SetFromRead(rw, false);
                // EnemyArmyMorale.SetFromRead(rw, false);
                // EnemyArmyFuel.SetFromRead(rw, false);
                // EnemyArmySupply.SetFromRead(rw, false);
                // EnemyArmyEfficiency.SetFromRead(rw, false);
        }

        private void ArmyControlWriter()
        {
            if (SRMemento.Instance.UnitIsPersistent("ArmyCurrentStrength".GetPointer(rw))) return;
            if (SRMemento.Instance.UnitIsPersistent("HoverArmyCurrentStrength".GetPointer(rw))) return;
            
            if (ceMorale.Checked)
            {
                gvWarfare.gvSetRowCellValue("value", "ArmyMorale", "5", true);
                Hover(() => gvWarfare.gvSetRowCellValue("value", "HoverArmyMorale", "5", true));
                
                "ArmyMorale".GetFeature().WriteTo(rw, "5");
                Hover(() => "HoverArmyMorale".GetFeature().WriteTo(rw, "5"));
            }

            // rw.SRWrite("ArmyMorale", "10.0");
            if (ceExperience.Checked)
            {
                var rate = ConvertedExperienceRating().ToString(CultureInfo.InvariantCulture);
                gvWarfare.gvSetRowCellValue("value", "ArmyExperience", rate, true);
                Hover(() => gvWarfare.gvSetRowCellValue("value", "HoverArmyExperience", rate, true));
                "ArmyExperience".GetFeature().WriteTo(rw, rate);
                Hover(() => "HoverArmyExperience".GetFeature().WriteTo(rw, rate));
            }

            // rw.SRWrite("ArmyExperience", "10.0");
            if (ceEfficiency.Checked)
            {
                gvWarfare.gvSetRowCellValue("value", "ArmyEfficiency", "5", true);
                Hover(() => gvWarfare.gvSetRowCellValue("value", "HoverArmyEfficiency", "5", true));
                
                "ArmyEfficiency".GetFeature().WriteTo(rw, "5");
                Hover(() => "HoverArmyEfficiency".GetFeature().WriteTo(rw, "5"));
            }

            if (ceEntrenchment.Checked)
            {
                gvWarfare.gvSetRowCellValue("value", "ArmyEntrenchment", "5", true);
                Hover(() => gvWarfare.gvSetRowCellValue("value", "HoverArmyEntrenchment", "5", true));
                
                "ArmyEntrenchment".GetFeature().WriteTo(rw, "5");
                Hover(() => "HoverArmyEntrenchment".GetFeature().WriteTo(rw, "5"));
            }

            // rw.SRWrite("ArmyEfficiency", "10.0");

            bool IsChecked(WarfareArmyEnum e)
            {
                return checkedListBoxControl1.Items[e].CheckState == CheckState.Checked;
            }

            // Init For special 
            var actualStrength = ArmyActualStrength.value.StrToDecimal();
            var actualGas = (ArmyCurrentStrength.value.StrToDecimal() * UnitFuelCapacity.value.StrToDecimal());
            var actualSupply = (ArmyCurrentStrength.value.StrToDecimal() * UnitSuppliesCapacity.value.StrToDecimal());
            
            
            var hoverActualStrength = HoverArmyActualStrength.value.StrToDecimal();
            var hoverActualGas = (HoverArmyCurrentStrength.value.StrToDecimal() * HoverUnitFuelCap.value.StrToDecimal());
            var hoverActualSupply = (HoverArmyCurrentStrength.value.StrToDecimal() * HoverUnitSupplyCap.value.StrToDecimal());


            // Multiplier
            var supplyGasMultiplier = 1;
            var strMultiplier = 1;


            // Heal
            if (IsChecked(WarfareArmyEnum.Heal))
            {
                if (ArmyCurrentStrength.value.StrToDecimal() < ArmyActualStrength.value.StrToDecimal())
                    rw.SRWrite(ArmyCurrentStrength.name, (ArmyActualStrength.value.StrToDecimal()).ToString());
                if (ArmyFuel.value.StrToDecimal() < actualGas)
                    rw.SRWrite(ArmyFuel.name, actualGas.ToString());
                if (ArmySupply.value.StrToDecimal() < actualSupply)
                    rw.SRWrite(ArmySupply.name, actualSupply.ToString());

                Hover(() =>
                {
                    if (HoverArmyCurrentStrength.value.StrToDecimal() < HoverArmyActualStrength.value.StrToDecimal())
                        HoverArmyCurrentStrength.WriteTo(rw, HoverArmyActualStrength.value);
                    if (HoverArmyFuel.value.StrToDecimal() < actualGas)
                        HoverArmyFuel.WriteTo(rw, HoverArmyActualStrength.value);
                    if (HoverArmySupply.value.StrToDecimal() < actualSupply)
                        HoverArmySupply.WriteTo(rw, HoverArmyActualStrength.value);
                });
            }

            // Rambo
            if (IsChecked(WarfareArmyEnum.Rambo))
            {
                // ceEfficiency.Checked = true;
                // ceExperience.Checked = true;
                // ceMorale.Checked = true;
                // if (!IsChecked(WarfareArmyEnum.Rambo))
                // {
                //     ceEfficiency.Checked = false;     
                //     ceExperience.Checked = false;
                //     ceMorale.Checked = false;
                // }
            }

            // 2x Str
            if (IsChecked(WarfareArmyEnum.Str2x))
            {
                strMultiplier = strMultiplier + 2;
                rw.SRWrite(ArmyCurrentStrength.name, (actualStrength * strMultiplier).ToString());
                Hover(() => HoverArmyCurrentStrength.WriteTo(rw, hoverActualStrength * strMultiplier));
            }

            // 4x Str
            if (IsChecked(WarfareArmyEnum.Str4x))
            {
                strMultiplier = strMultiplier + 4;
                rw.SRWrite(ArmyCurrentStrength.name, (actualStrength * strMultiplier).ToString());
                Hover(() => HoverArmyCurrentStrength.WriteTo(rw, hoverActualStrength * strMultiplier));
            }

            // 2x SupplyGas
            if (IsChecked(WarfareArmyEnum.GasSupply2x))
            {
                supplyGasMultiplier = supplyGasMultiplier + 2;
                rw.SRWrite(ArmyFuel.name, (actualGas * supplyGasMultiplier).ToString("N"));
                rw.SRWrite(ArmySupply.name, (actualSupply * supplyGasMultiplier).ToString("N"));
                
                Hover(() =>
                {
                    rw.SRWrite(HoverArmyFuel.name, (hoverActualGas * supplyGasMultiplier).ToString("N"));
                    rw.SRWrite(HoverArmySupply.name, (hoverActualSupply * supplyGasMultiplier).ToString("N"));
                });
            }

            // 4x SupplyGas
            if (IsChecked(WarfareArmyEnum.GasSupply4x))
            {
                supplyGasMultiplier = supplyGasMultiplier + 4;
                rw.SRWrite(ArmyFuel.name, (actualGas * supplyGasMultiplier).ToString("N"));
                rw.SRWrite(ArmySupply.name, (actualSupply * supplyGasMultiplier).ToString("N"));
                
                Hover(() =>
                {
                    rw.SRWrite(HoverArmyFuel.name, (hoverActualGas * supplyGasMultiplier).ToString("N"));
                    rw.SRWrite(HoverArmySupply.name, (hoverActualSupply * supplyGasMultiplier).ToString("N"));
                });
            }
        }

        private void showBar(ProgressBarControl pbc, Feature source, int MaxValue)
        {
            int realVal = (int) (Convert.ToDecimal(source.value) * 100);
            if (realVal >= MaxValue) realVal = MaxValue;
            pbc.Position = realVal;
            pbc.Properties.Maximum = MaxValue;
        }

        // Bar Override Deactivate
        protected void FormDeactivate(object sender, EventArgs e)
        {
            if (bar2.IsFloating) //your condition  
            {
                ISupportWindowActivate act = bar2 as ISupportWindowActivate;
                if (act != null) act.Activate();
            }
        }

        private void bar2_DockChanged(object sender, EventArgs e)
        {
            // if (bar2.IsFloating)
            // {
            //     this.TopMost = barTglOnTop.Checked;
            // }
            // else
            // {
            //     this.TopMost = barTglOnTop.Checked;
            // }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            gcUnitHistoryList.DataSource = UnitHistoryList.Instance.UnitList;
        }

        private void setSpinEditorEvent(SpinEdit se)
        {
            se.Properties.ValidateOnEnterKey = true;
            se.Validating += spinValidatingEvent;
            se.MouseEnter += (sender, args) =>
            {
                // se.Focus();
            };
            se.LostFocus += (sender, args) =>
            {
                // SpinEdit se = sender as SpinEdit;
            };
            se.Spin += setSpinEvent;
        }

        private void spinValidatingEvent(object sender, CancelEventArgs e)
        {
            SpinEdit se = (SpinEdit) sender;
            Debug.WriteLine($"SpinEvent Validating: before swWrite, {se.Text}");
            rw.SRWrite(se.Tag.ToString(), se.EditValue.ToString());
        }

        private void setSpinEvent(object sender, SpinEventArgs e)
        {
            SpinEdit se = (SpinEdit) sender;
            // Feature v = jsonReader.safeFeatureSearch(se.Tag.ToString(), warfareList);
            // se.EditValue = v.value;
            // // MessageBox.Show($"SE VALUE {se.Value}");
            // // MessageBox.Show($"v VALUE {v.value}");
            // // MessageBox.Show($"se EDIT VALUE {se.EditValue}");
            // v.value = se.EditValue.ToString();
            // se.Properties.BeginUpdate();
            Debug.WriteLine($"SpinEvent: before swWrite, {se.Text}");
            rw.SRWrite(se.Tag.ToString(), se.EditValue.ToString());
            // se.Properties.EndUpdate();
        }

        private void seModifiedEvent(object sender, EventArgs e)
        {
            SpinEdit se = (SpinEdit) sender;
            rw.SRWrite(se.Tag.ToString(), se.EditValue.ToString());
            //if (se.Tag.ToString)
            //}
        }

        private void ceCheckStateChangedEvent(object sender, EventArgs e)
        {
            CheckEdit ce = sender as CheckEdit;

            List<Feature> tags = ce.Tag.CastTo<List<Feature>>();
            for (int i = 0; i < tags.Count; i++)
            {
                if (ce.Checked && tags[i].freeze) continue;
                tags[i].freeze = ce.Checked;
                // if (tags[i].category == "Warfare")
                // gvWarfare.SetRowCellValue(tags[i].gridId, "freeze", ce.Checked);
                // var gvFreeze = gvWarfare.GetRowCellValue(tags[i].gridId, "freeze");
                // rw.SRWrite(tags[i].name, tags[i].value);
                // rw.SRFreeze(tags[i].name, tags[i].value);
            }
        }

        private void barBtBestFitColumn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gvCountry.BestFitColumns();
            gvWarfare.BestFitColumns();
            gvResources.BestFitColumns();
        }

        private void xtraTabControl1_Selected(object sender, DevExpress.XtraTab.TabPageEventArgs e)
        {
            gvCountry.BestFitColumns();
            gvCountry.OptionsView.ColumnAutoWidth = false;
            gvCountry.Columns["value"].OptionsColumn.FixedWidth = true;

            gvWarfare.BestFitColumns();
            gvWarfare.Columns["displayName"].OptionsColumn.FixedWidth = true;
            gvWarfare.Columns["original"].OptionsColumn.FixedWidth = true;
            gvWarfare.Columns["value"].OptionsColumn.FixedWidth = true;

            gvResources.BestFitColumns();
            gvResources.Columns["value"].OptionsColumn.FixedWidth = true;
        }

        private void barEditItem3_EditValueChanged(object sender, EventArgs e)
        {
            // TrackBarControl trackBar = sender as TrackBarControl;
            if (barEditItem3.EditValue != null)
            {
                var val = Convert.ToInt32(barEditItem3.EditValue);
                this.Opacity = (double) val / 100;
            }
        }

        private void barTglOnTop_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TopMost = barTglOnTop.Checked;
            Properties.Settings.Default.UserAlwaysOnTop = barTglOnTop.Checked;
        }

        private void skinDropDownButtonItem1_DownChanged(object sender, ItemClickEventArgs e)
        {
            SkinDropDownButtonItem skin = sender as SkinDropDownButtonItem;
            Properties.Settings.Default.UserThemes = LookAndFeel.ActiveSkinName;
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            // _form1 = new Form1();
            // _form1.Show();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            // rw.SRFreezePersistent("ArmyMorale", true, "0,1");
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void memoEdit2_EditValueChanged(object sender, EventArgs e)
        {
        }

        private void barBtnDonationBottom_ItemClick(object sender, ItemClickEventArgs e)
        {
        }
    }
}