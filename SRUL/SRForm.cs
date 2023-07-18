using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using AdsJumboWinForm;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Text;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTab;
using GlobalHotKey;
using SRUL.Annotations;
using SRUL.Properties;
using SRUL.Types;
using SRUL.UnitTracker;
using TB.ComponentModel;
using ProgressBar = System.Windows.Forms.ProgressBar;
using Timer = System.Windows.Forms.Timer;

namespace SRUL
{
    // TODO: Refactor SharedEvent to own Class, so it willnt shown as cluttered
    // TODO: Form Class supposed for using to init and placement

    public partial class MainForm : DevExpress.XtraBars.ToolbarForm.ToolbarForm
    {
           #region Properties

    // Information countryInfoTable = new Information(Loader.Rw);

    // [External] Deps
    private HotKeyManager _hotkeys;
    
    // [Internal] Deps
    private SRMiniWindow? _miniWindow;
    private SRPlayersControl _srPlayerControls = SRPlayersControl.Instance;
    public readonly SRViews SRViews;
    public SRMain JsonReader = SRLoaderForm._srLoader.jr;
    public SRLoader Loader = SRLoaderForm._srLoader;
    public SRReadWrite rw = SRLoaderForm._srLoader.rw;
    public SRRefresher Refresher = SRRefresher.CreateInstance();
    public new SREvents Events = SREvents.CreateInstance();
    public SRMemento Memento = SRMemento.Instance; 
    public TrackedUnitController TrackedUnitsController = new TrackedUnitController();
    public SpecialCheats SpecialCheats = new SpecialCheats();

    private Key _userHotkey = (Key)Enum.Parse(typeof(Key), Properties.Settings.Default.SpecialTABRailRoadHotKey.ToString("0"));
    private ModifierKeys _userKeymodifier = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), Settings.Default.SpecialTABRailRoadModifierHotKey.ToString("0"));

    // private SRPlayersControl.CustomPopupMenu _popupMenuCustom = new SRPlayersControl.CustomPopupMenu();

    // Inherit from Loader Class
    // private JSONReader jsonReader = SRLoader.LoaderInstance.jr;
    // private SRReadWrite rw = SRLoaderForm._sr.LoaderInstance.rw;

    public SrSoldierCamp Camp = new SrSoldierCamp();

    // Observer pattern, not used anymore. using memento instead.
    // private UnitTracker _unitTracker = new UnitTracker();
    // private UnitReporter _unitReporter = new UnitReporter("# Unit");

    // Gridview Initiliazation 2022 commented
    //private int gvCurrentRow;
    //private bool gvCurrentEnabledState;
    //private bool gvCurrentRowName;

    internal IList<Feature> warfareList;

    #region  [Special] Railroad;
    // Capturer
    internal string? StaticRailPointer;
    internal string? StaticRoadPointer;
    internal string? StaticRailCostPointer;
    internal string? StaticRoadCostPointer;
    internal string? StaticRailIGNeededPointer;
    internal string? StaticRoadIGNeededPointer;
    internal string? StaticRailOriginalCostValue;
    internal string? StaticRoadOriginalCostValue;
    internal string? StaticRoadOriginalValue = null;
    internal string? StaticRailOriginalValue = null;
    internal string? StaticRoadIGNeededOriginalValue = null;
    internal string? StaticRailIGNeededOriginalValue = null;
    #endregion

    #region [Warfare] Army
    public Feature ArmyEfficiency;
    public Feature ArmyEntrenchment;
    public Feature ArmyActualStrength;
    public Feature ArmyCurrentStrength;
    public Feature ArmyMorale;
    public Feature ArmyExperience;
    public Feature ArmyFuel;
    public Feature ArmySupply;
    public Feature UnitFuelCapacity;
    public Feature UnitSuppliesCapacity;
    public Feature ArmyActiveStaff;
    public Feature ArmyReserve;
    public Feature UnitClass;
    #endregion

    #region [Warfare] Hover Mode
    protected internal readonly Feature HoverArmyActualStrength = "HoverArmyActualStrength".GetFeature();
    protected internal readonly Feature HoverArmyCurrentStrength = "HoverArmyCurrentStrength".GetFeature();
    protected internal readonly Feature HoverArmyEfficiency = "HoverArmyEfficiency".GetFeature();
    protected internal readonly Feature HoverArmyExperience = "HoverArmyExperience".GetFeature();
    protected internal readonly Feature HoverArmyEntrenchment = "HoverArmyEntrenchment".GetFeature();
    protected internal readonly Feature HoverArmyMorale = "HoverArmyMorale".GetFeature();
    protected internal readonly Feature HoverArmyFuel = "HoverArmyFuel".GetFeature();
    protected internal readonly Feature HoverUnitFuelCap = "HoverUnitFuelCapacity".GetFeature();
    protected internal readonly Feature HoverArmySupply = "HoverArmySupply".GetFeature();
    protected internal readonly Feature HoverUnitSupplyCap = "HoverUnitSuppliesCapacity".GetFeature();
    #endregion

    #region [Special] Satellite
    protected internal readonly Feature SatelliteCommCoverage = "SatelliteCommCoverage".GetFeature();
    protected internal readonly Feature SatelliteReconCoverage = "SatelliteReconCoverage".GetFeature();
    protected internal readonly Feature SatelliteMissileDefenseCoverage = "SatelliteMissileDefenseCoverage".GetFeature();
    #endregion

    #region [Special] 1 Day
    protected internal readonly Feature ADayBuild = "ADayBuild".GetFeature();
    protected internal readonly Feature ADayArmy = "ADayArmy".GetFeature();
    protected internal readonly Feature ADayResearchClick = "ADayResearchClick".GetFeature();
    protected internal readonly Feature ADayResearchTooltip = "ADayResearchTooltip".GetFeature();
    protected internal readonly Feature ResearchEfficiency = "ResearchEfficiency".GetFeature();
    #endregion

    // [Warfare] Missile Multillier
    #region [Warfare] Missile Multiplier
    protected internal readonly Feature ArmyMissileAvailableStorageQuantity = "ArmyMissileAvailableStorageQuantity".GetFeature();
    protected internal readonly Feature ArmyMissileAvailableCargoQuantity = "ArmyMissileAvailableCargoQuantity".GetFeature();
    protected internal readonly Feature ArmyMissileStrategicPoolAssigned = "ArmyMissileStrategicPoolAssigned".GetFeature();
    protected internal readonly Feature ArmyMissileStrategicPoolReserve = "ArmyMissileStrategicPoolReserve".GetFeature();
    #endregion

    #region [Warfare] Garrison
    protected internal readonly Feature BuildingGarrisonActive = "BuildingGarrisonActive".GetFeature();
    protected internal readonly Feature BuildingGarrisonEfficiency = "BuildingGarrisonEfficiency".GetFeature();
    #endregion

    // private SRAdsManager srAdsManager = SRLoader.srAdsManager;

    internal bool IsNaval;
    internal HandmadeFeatures HandmadeFeature = HandmadeFeatures.Instance;

    // [Internal] Indicator
    public bool IsFullScreen;

    #endregion

    #region [Warfare] Army Editor Property Region
    private double _actualStrength = 0;
    private double _actualGas = 0;
    private double _actualSupply = 0;
    private double _hoverActualStrength = 0;
    private double _hoverActualGas = 0;
    private double _hoverActualSupply = 0;
    private double _supplyGasMultiplier = 0;
    private double _strMultiplier = 0;
    #endregion

    #region SoundPlayer
    SoundPlayer _player = new SoundPlayer();
    bool disposing = false;

    #endregion
    //
    // private DevExpress.XtraBars.ToolbarForm.ToolbarFormControl toolbarFormControl1;
    // private DevExpress.XtraBars.ToolbarForm.ToolbarFormManager toolbarFormManager1;
    // private IContainer components;
    // private BarDockControl barDockControlTop;
    // private BarDockControl barDockControlBottom;
    // private BarDockControl barDockControlLeft;
    // private BarDockControl barDockControlRight;
    public Settings Settings = Properties.Settings.Default;
    private ProgressBarControl[] _pbUsages;
    private Timer? adsTimer;

        #region Constructor
        public MainForm()
        {
            InitializeComponent();
            xtabMainControl.Transition.AllowTransition = DefaultBoolean.False;

            this.Disposed += (sender, args) =>
            { 
                Dispose();   
            };
            SRViews = SRViews.CreateInstance();
            // bannerAdsSpecialTab.ShowAd(468, 60, Settings.ApplicationAdsKey);
            // interstitialAdAllScreen.ShowInterstitialAd(Settings.Default.ApplicationAdsKey);
            // SRAds.Instance.ShowAd(bannerAdsCountryTab, 468, 60);
            // // SRAds.Instance.ShowAd(bannerAdsResourcesTab, 468, 60);
            // SRAds.Instance.ShowAd(bannerAdsProfileBox1, 300, 250);
            // SRAds.Instance.ShowAd(bannerAdsProfileBox2, 300, 250);
            // SRAds.Instance.ShowAd(bannerAdsResourceTab, 468, 60);
            // SRAds.Instance.ShowAd(interstitialAdAllScreen, true, 25000);
            // // SRAds.Instance.ShowAd(interstitialAd2, true, 25000);
            // // SRAds.Instance.ShowAd(interstitialAd3, true, 25000);
            // SRAds.Instance.ShowAd(interstitialAdFullScreen, true, 25000);
            //
            // bannerAdsProfileBox2.Visible = false;
            // bannerAdsProfileBox1.Visible = false;

            // Facilities Grid for donators
            // Width:
            // Heigth:

            // srAdsManager = new SRAdsManager();
            // srAdsManager.AddAd(new ManagedInterstitialAd(interstitialAdFullScreen, false));
            // srAdsManager.AddAd(new ManagedInterstitialAd(interstitialAdAllScreen, false));
            // srAdsManager.Start();
            // srAdsManager.AddAd(new ManagedBannerAd(bannerAdsCountryTab, 468, 60));
            // srAdsManager.AddAd(new ManagedBannerAd(bannerAdsProfileBox1, 300, 250));
            // srAdsManager.AddAd(new ManagedBannerAd(bannerAdsProfileBox2, 300, 250));
            // srAdsManager.AddAd(new ManagedBannerAd(bannerAdsResourceTab, 468, 60));
            // srAdsManager.AddAd(new ManagedBannerAd(bannerAdsSpecialTab, 468, 60));
            // srAdsManager.AddAd(new ManagedBannerAd(bannerAdsProfileBoxBottom, 468, 60));
            // srAdsManager.StartAll();


            TDeviceApproval? approval = SRLoader.SRDeviceApproval;
            bool tApproval = false;
            if (approval == null)
                tApproval = false;
            else
                tApproval = approval.Approval;
            
            BannerAds CreateBannerAds(BannerAds ad, Point location, Size size, int hAds, int wAds)
            {
                if (ad.IsDisposed)
                {
                    var newAd = new BannerAds();
                    newAd.Name = ad.Name;
                    newAd.Location = location;
                    newAd.Size = size;
                    newAd.HeightAd = hAds;
                    newAd.WidthAd = wAds;
                    newAd.Dock = ad.Dock;
                    newAd.TabIndex = ad.TabIndex;
                    newAd.ApplicationId = Settings.ApplicationAdsKey;
                    return newAd;
                }
                return ad;
            }

            void BannerAdsCreatorWrapper(bool visibility, ref BannerAds ad, Control ctr, Point pt, Size sz, int hAds, int wAds)
            {
                if (!visibility) {
                    AdsDisposer(ref ad);
                }
                else
                {
                    ad = CreateBannerAds(ad, pt, sz, hAds, wAds);
                    if (!ctr.Controls.ContainsKey(ad.Name))
                        ctr.Controls.Add(ad);
                    ad.Show();
                    ad.BringToFront();
                    // ad.ShowAd(468, 60, Settings.ApplicationAdsKey);
                }
            }
            
            var adsCreator = new AdsCreator();
            adsCreator.RegisterACopy(bannerAdsCountryTab, 468, 60);
            adsCreator.RegisterACopy(bannerAdsSpecialTab, 468, 60);
            adsCreator.RegisterACopy(bannerAdsProfileBox1, 300, 250);
            adsCreator.RegisterACopy(bannerAdsProfileBox2, 300, 250);
            adsCreator.RegisterACopy(bannerAdsProfileBoxBottom, 468, 60);
            adsCreator.RegisterACopy(bannerAdsResourceTab, 468, 60);
            adsCreator.RegisterACopy(bannerAdsTrackedUnits, 468, 60);
            adsCreator.RegisterACopy(bannerAdsWarfareUnitEditorTab, 468, 60);
            adsCreator.RegisterACopy(interstitialAdAllScreen, 468, 60);
            adsCreator.RegisterACopy(interstitialAdFullScreen, 468, 60);
            void CreateWrapper(bool visibility, BannerAds ad, Control ctr)
            {
                if (!visibility)
                {
                    AdsDisposer(ref ad);
                }
                else
                {
                    ad = adsCreator.CreateAd<BannerAds>(ad.Name);
                    if (!ctr.Controls.ContainsKey(ad.Name))
                        ctr.Controls.Add(ad);
                    ad.Show();
                    ad.BringToFront();
                    // ad.ShowAd(300, 250, Settings.ApplicationAdsKey);
                    // ad.ShowAd(468, 60, Settings.ApplicationAdsKey);
                }
            }
            void RecreateAds(bool visibility, BannerAds ad, Control ctr)
            {
                if (visibility)
                {
                    AdsDisposer(ref ad);
                    if (ad.IsDisposed)
                    {
                        ad = adsCreator.CreateAd<BannerAds>(ad.Name);
                        if (!ctr.Controls.ContainsKey(ad.Name))
                            ctr.Controls.Add(ad);
                        ad.Show();
                    }
                }
            }

            if (!tApproval)
            {
                // Banner
                OnBannerAdsVisibleChangedWrapper(bannerAdsCountryTab, xtabCountry);
                OnBannerAdsVisibleChangedWrapper(bannerAdsSpecialTab, xtabSpecial);
                OnBannerAdsVisibleChangedWrapper(bannerAdsProfileBox1, xtabPlayer);
                OnBannerAdsVisibleChangedWrapper(bannerAdsProfileBox2, xtabPlayer);
                OnBannerAdsVisibleChangedWrapper(bannerAdsProfileBoxBottom, xtabPlayer);
                OnBannerAdsVisibleChangedWrapper(bannerAdsResourceTab, xtabResources);
                OnBannerAdsVisibleChangedWrapper(bannerAdsTrackedUnits, xtabTrackedUnit);
                OnBannerAdsVisibleChangedWrapper(bannerAdsWarfareUnitEditorTab, xtabStatEditor);

                xtabStatEditor.VisibleChanged += (sender, args) => 
                    BannerAdsCreatorWrapper(xtabStatEditor.Visible, ref bannerAdsWarfareUnitEditorTab, xtabStatEditor, 
                        new Point(0, 0), 
                        new Size(461, 60),  60, 468);
                xtabPlayer.VisibleChanged += (sender, args) =>
                {
                    
                    adsCreator.DisposeIfNotVisible(ref bannerAdsProfileBox1, xtabPlayer);
                    adsCreator.DisposeIfNotVisible(ref bannerAdsProfileBox2, xtabPlayer);
                    adsCreator.DisposeIfNotVisible(ref bannerAdsProfileBoxBottom, xtabPlayer);
                    // BannerAdsCreatorWrapper((sender as XtraTabPage).Visible, 
                    //     ref bannerAdsProfileBox1, xtabPlayer, new Point(0, 217), new Size(300, 250), 250, 300);
                    // BannerAdsCreatorWrapper((sender as XtraTabPage).Visible, 
                    //     ref bannerAdsProfileBox2, xtabPlayer, new Point(223, 217), new Size(300, 250), 250, 300);
                    // BannerAdsCreatorWrapper((sender as XtraTabPage).Visible, 
                    //     ref bannerAdsProfileBoxBottom, xtabPlayer, new Point(2, 515), new Size(468, 60), 250, 300);
                    // bannerAdsProfileBox1.BringToFront();
                    // bannerAdsProfileBox2.BringToFront();
                    // bannerAdsProfileBoxBottom.BringToFront();
                };
                xtabCountry.VisibleChanged += (sender, args) => adsCreator.DisposeIfNotVisible(ref bannerAdsCountryTab, panelCountry);
                    // BannerAdsCreatorWrapper((sender as XtraTabPage).Visible, ref bannerAdsCountryTab, panelCountry, 
                    // new Point(2, 485), 
                    // new Size(459, 60),  60, 468);
                xtabResources.VisibleChanged += (sender, args) => adsCreator.DisposeIfNotVisible(ref bannerAdsResourceTab, panelResources);
                    // BannerAdsCreatorWrapper((sender as XtraTabPage).Visible, ref bannerAdsResourceTab, panelResources, 
                    // new Point(2, 512), 
                    // new Size(459, 60),  60, 468);
                xtabTrackedUnit.VisibleChanged += (sender, args) => adsCreator.DisposeIfNotVisible(ref bannerAdsTrackedUnits, xtabTrackedUnit);
                    // BannerAdsCreatorWrapper((sender as XtraTabPage).Visible, ref bannerAdsTrackedUnits, xtabTrackedUnit, 
                    //     new Point(0, 0), 
                    //     new Size(461, 61),  60, 468);
                xtabSpecial.VisibleChanged += (sender, args) => adsCreator.DisposeIfNotVisible(ref bannerAdsSpecialTab, panelControl5);
                    // BannerAdsCreatorWrapper((sender as XtraTabPage).Visible, ref bannerAdsSpecialTab, panelControl5, 
                    //     new Point(0, 0), 
                    //     new Size(461, 61),  60, 468);

                // Intersittial
                var periodTimeSpan = TimeSpan.FromMinutes(2);
                adsTimer = new Timer();
                adsTimer.Interval = (int)periodTimeSpan.TotalMilliseconds;
                
                var adsCleaner = new Timer();
                adsCleaner.Interval = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
                adsCleaner.Tick += (sender, args) =>
                {
                    if (!_interstitialDisposing) return;
                    if (interstitialAdFullScreen.Visible) return;
                    if (interstitialAdAllScreen.Visible) return;
                    
                    AdsDisposer(ref interstitialAdFullScreen);
                    AdsDisposer(ref interstitialAdAllScreen);
                    _interstitialDisposing = false;
                    adsCleaner.Stop();
                };
                
                
                void AdsManager(InterstitialAd ad) => ad.VisibleChanged += OnAdOnVisibleChanged;
                AdsManager(interstitialAdFullScreen);

                interstitialAdAllScreen.ShowInterstitialAd(Settings.ApplicationAdsKey);
                interstitialAdFullScreen.ShowInterstitialAd(Settings.ApplicationAdsKey);
                var self = this;
                adsTimer.Tick += (sender, args) =>
                {
                    // interstitialAdAllScreen.ShowInterstitialAd(Settings.ApplicationAdsKey);
                    // interstitialAdFullScreen.ShowInterstitialAd(Settings.ApplicationAdsKey);
                    
                    adsCreator.DisposeAndCreate(ref interstitialAdAllScreen, toolbarFormControl1);
                    interstitialAdAllScreen.ShowInterstitialAd(Settings.ApplicationAdsKey);
                    
                    adsCreator.DisposeAndCreate(ref interstitialAdFullScreen, self);
                    interstitialAdFullScreen.ShowInterstitialAd(Settings.ApplicationAdsKey);
                    
                    adsCleaner.Start();

                    adsCreator.DisposeAndCreate(ref bannerAdsProfileBox1, xtabPlayer);
                    adsCreator.DisposeAndCreate(ref bannerAdsProfileBox2, xtabPlayer);
                    adsCreator.DisposeAndCreate(ref bannerAdsProfileBoxBottom, xtabPlayer);
                    adsCreator.DisposeAndCreate(ref bannerAdsCountryTab, panelCountry);
                    adsCreator.DisposeAndCreate(ref bannerAdsSpecialTab, panelControl5);
                    adsCreator.DisposeAndCreate(ref bannerAdsResourceTab, panelResources);
                    adsCreator.DisposeAndCreate(ref bannerAdsTrackedUnits, xtabTrackedUnit);
                    adsCreator.DisposeAndCreate(ref bannerAdsWarfareUnitEditorTab, xtabStatEditor);
                };
                adsTimer.Enabled = true;
                adsTimer.Start();
            } else {
                gcCountry.Dock = DockStyle.Fill;
                gcFacility.Dock = DockStyle.Fill;
                gcResources.Dock = DockStyle.Fill;
                gcWarfare.Dock = DockStyle.Fill;
                gcPersistentUnit.Dock = DockStyle.Fill;
                
                AdsDisposer(ref bannerAdsCountryTab);
                AdsDisposer(ref bannerAdsSpecialTab);
                AdsDisposer(ref bannerAdsProfileBox1);
                AdsDisposer(ref bannerAdsProfileBox2);
                AdsDisposer(ref bannerAdsResourceTab);
                AdsDisposer(ref bannerAdsProfileBoxBottom);
                AdsDisposer(ref bannerAdsTrackedUnits);
                AdsDisposer(ref bannerAdsWarfareUnitEditorTab);
                AdsDisposer(ref interstitialAdFullScreen);
                AdsDisposer(ref interstitialAdAllScreen);

                if(interstitialAdFullScreen.IsDisposed)
                    XtraMessageBox.Show("Thank you For Your Support!", "Thank You", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            void HotkeyRegistration()
            {
                _hotkeys.Register(_userHotkey, _userKeymodifier);
                _hotkeys.KeyPressed += (sender, args) =>
                {
                    if (args.HotKey.Key == _userHotkey)
                        RailRoadHotkeyWriteGuard();
                };
            }
            
            _hotkeys = new HotKeyManager();
            try
            {
                HotkeyRegistration();
            }
            catch (Exception e)
            {
                _hotkeys.Unregister(_userHotkey, _userKeymodifier);
                HotkeyRegistration();
            }
            
            // Set first read.
            foreach (var feat in JsonReader.FeatureIndexedStore)
            {
                
                // if(ListOfSortedRow.SRGridIncludedFeatures.Contains(feat.Value.name)) 
                //     feat.Value.ReadSafeVoid();
            }
            // SRControls.Instance.Register(new SRControl(checkedListBoxControl1));

            // Check screen
            IsFullScreen = WindowTracker.IsFullscreen(Loader.Selected.GameProcess.MainWindowHandle,
                Screen.FromHandle(Loader.Selected.GameProcess.MainWindowHandle));

            // Bar bottom Guard;
            var whitelisted = Loader.CheckForApproval() ? "Donator" : "Active";
            barsiBtmTrainerStatus.Caption = JsonReader.Data.SRFStatus ? whitelisted : "Inactive";

            // Set Data Source for each Category control
            gcCountry.DataSource = SRMain.Instance.FeaturesCountry;
            gcResources.DataSource = SRMain.Instance.FeaturesResources;
            gcWarfare.DataSource = SRMain.Instance.FeaturesWarfare;
            gcFacility.DataSource = SRMain.Instance.FeaturesSpecial;

            gcCountry.ForceInitialize();
            gcResources.ForceInitialize();
            gcWarfare.ForceInitialize();
            gcFacility.ForceInitialize();
            
            SRViews.Instance.InitializeGridViews(new []
            {
                gvCountry, 
                gvResources, 
                gvWarfare, 
                gvFacility
            });
            
            Deactivate += FormDeactivate;

            // _unitReporter.Subscribe(_unitTracker);
            // Init Special Option Satellite value for cb caption
            ceSatComm.Text = "Communication Coverage " +
                             (float.Parse(JsonReader.feature("SatelliteCommCoverage").value) * 100) + "%";
            ceSatMilDef.Text = "Missile Defense Coverage " +
                               (float.Parse(JsonReader.feature("SatelliteMissileDefenseCoverage").value) * 100) + "%";
            ceSatRecon.Text = "Reconnaisance Coverage " +
                              (float.Parse(JsonReader.feature("SatelliteReconCoverage").value) * 100) + "%";

            // Init History Data Source from memento
            gcModifiedUnit.DataSource = SRMemento.Instance.UnitHistoryList;
            gcModifiedUnit.ForceInitialize();
            gvModifiedUnit.Columns["RowId"].Visible = false;
            gvModifiedUnit.Columns["UnitAddress"].Visible = false;
            gvModifiedUnit.Columns["ModifiedStats"].Visible = false;
            gvModifiedUnit.Columns["UnitStats"].Visible = false;
            gvModifiedUnit.Columns["UnitName"].OptionsColumn.AllowEdit = false;
            gvModifiedUnit.Columns["UnitId"].OptionsColumn.AllowEdit = false;
            gvModifiedUnit.Columns["RowId"].OptionsColumn.AllowEdit = false;
            gvModifiedUnit.BestFitColumns();
            
            GvMasterDetail(gcModifiedUnit, gvModifiedUnit);
            GvLoad(gvModifiedUnit);
            GvLoad(gvPersistentUnit);
            
            
            SetControlBindingToSettings(ceResourceDisplayFormat, "EditValue", "ResourceTABShowVolumeMetrics");
            SetControlBindingToSettings(ceResourceUseKMBFormatting, "EditValue", "ResourceTABShowKMBFormatting");
            // SetControlBindingToSettings(Settings_General_Warfare_InfoDisplayPercentage, "BindableChecked",
            //     "WarfareTABShowResourceAsPercentage");
            // SetControlBindingToSettings(Settings_General_Warfare_InfoDailyAnnually, "BindableChecked",
            //     "WarfareTABShowResourceAsDailyAnnually");

            Settings.Default.PropertyChanged += Default_PropertyChanged;
            
            // This line of code is generated by Data Source Configuration Wizard
            // Fill the JsonDataSource
            // jsonDataSource1.Fill();
        }

        private bool _interstitialDisposing = false;
        private void OnAdOnVisibleChanged(object sender, EventArgs args)
        {
            var sdr = (sender as InterstitialAd);
            if(sdr == null) return;
            if (adsTimer == null) return;
            if (sdr.Visible)
            {
                adsTimer.Stop();
                _interstitialDisposing = false;
            }
            else
            {
                adsTimer.Start();
                if (adsTimer.Interval <= 300000) adsTimer.Interval += (int)TimeSpan.FromSeconds(20).TotalMilliseconds;
                _interstitialDisposing = true;
            }
        }

        private void OnBannerAdsVisibleChangedWrapper(BannerAds ad, XtraTabPage page)
        {
            ad.VisibleChanged += (o, e) =>
                OnBannerAdsVisibleChanged(o, e, page);
        }
        private void OnBannerAdsVisibleChanged(object sender, EventArgs args, XtraTabPage page)
        {
            var sdr = (sender as BannerAds);
            if(sdr == null) return;
            if (page.Visible) return;
            sdr.Dispose();
            GC.SuppressFinalize(this);
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForFullGCComplete();
        }

        #endregion
        public void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Settings.Default.Save();
        }
        #region First Load after constructor
        
        private void XtraForm1_Load(object sender, EventArgs e) 
        {
        // Default tabs
        xtabMainControl.SelectedTabPage = xtabAbout;
        PlayedTime();
        SRViews.InitializeView(this);

        
        // TODO: Ads Activation, proper implementation, this is quick stuff.
        // bannerAds1.ShowAd(468, 60, "");
        // bannerAds2.ShowAd(468, 60, "");
        // bannerAds3.ShowAd(468, 60, "");
        // ads.ShowAd(bannerAds1, 468, 60);
        // ads.ShowAd(bannerAds2, 468, 60);
        // ads.ShowAd(bannerAds3, 468, 60);
        // ads.ShowAd(interstitialAd1, true, 30000);

        
        // Themes Shettings;
        // Properties.Settings.Default.UserThemes = UserLookAndFeel.Default.theme;
        // Properties.Settings.Default.UserThemes = Settings.UserSkin;
        // ApplicationThemeHelper.SaveApplicationThemeName();

        // Players Profile

        Task.Factory.StartNew(async (feature) =>
        {
            await SRLoaderForm._srLoader.PostSteamProfile().ConfigureAwait(false);
            if (SRLoader.SteamPlayerProfile != null)
                picPlayerAvatarBox.ImageLocation = SRLoader.SteamPlayerProfile.Avatarmedium;
            lblPlayerPersona.Text =
                string.Format((string)"Hello, {0}", ((feature as Feature).Read(rw)));
        }, "PlayerSteamPersona".GetFeature());

        // Loader.GetSRPlayers().GetAwaiter().GetResult();
        // if (!(await PostSteamProfile().ConfigureAwait(false)))
        //     await GetSRPlayers().ConfigureAwait(false);
        // Init Players Tab control
        _srPlayerControls.InitControls(gcPlayers, tileView1);
        SteamRunAPI.BindEvent(btnUserGameValidation, SteamRunAPI.GameValidate);
        SteamRunAPI.BindEvent(btnUserExitSteam, SteamRunAPI.ExitSteam);
        SteamRunAPI.BindEvent(btnUserOpenProfile, SteamRunAPI.DeveloperProfileURI);
        SteamRunAPI.BindEvent(btnUserOpenContentEditor, SteamRunAPI.OpenContentEditor);
        SteamRunAPI.BindEvent(btnUserOpenMapEditor, SteamRunAPI.OpenMapEditor);
        SteamRunAPI.BindEvent(btnUserOpenWorkshop, SteamRunAPI.OpenWorkshop);
        SteamRunAPI.BindEvent(btnUserOpenGameHUB, SteamRunAPI.OpenGameHub);
        SteamRunAPI.BindEvent(btnUserOpenProfile, SteamRunAPI.OpenProfile);
        SteamRunAPI.BindEvent(btnUserOpenInventory, SteamRunAPI.OpenInventory);

        HandmadeFeature.Populate();
        checkedListBoxControl1.DataSource = HandmadeFeatures.Instance.BindingSource();
        checkedListBoxControl1.DisplayMember = "enums";
        checkedListBoxControl1.ValueMember = "freeze";

        // Set this props to equals value of feature. 
        // foreach (var warfare in WarfareArrayUtils.ArmyFeatureList)
        // {
        //     if (this.GetType().GetField(warfare) == null) continue;
        //     this.GetType().GetField(warfare).SetValue(this, warfare.GetFeature());
        //     // var propertyName = this.GetType().GetFields("Name").GetValue(warfare, null).ToString();
        //     // if (warfare == ) continue;
        //     // // otherwhise set value
        //     // this.GetType().GetProperty(propertyName).SetValue(this, warfare.GetFeature(), null);
        // }


        warfareList = JsonReader.seekWarfareVariable(WarfareArrayUtils.ArmyFeatureList);
        ArmyEfficiency = "ArmyEfficiency".GetFeature(JsonReader.FeaturesWarfare);
        ArmyEntrenchment = "ArmyEntrenchment".GetFeature(JsonReader.FeaturesWarfare);
        ArmyActualStrength = "ArmyActualStrength".GetFeature(JsonReader.FeaturesWarfare);
        ArmyCurrentStrength = "ArmyCurrentStrength".GetFeature(JsonReader.FeaturesWarfare);
        ArmyMorale = "ArmyMorale".GetFeature(JsonReader.FeaturesWarfare);
        ArmyExperience = "ArmyExperience".GetFeature(JsonReader.FeaturesWarfare);
        ArmyFuel = "ArmyFuel".GetFeature(JsonReader.FeaturesWarfare);
        ArmySupply = "ArmySupply".GetFeature(JsonReader.FeaturesWarfare);
        UnitFuelCapacity = "UnitFuelCapacity".GetFeature(JsonReader.FeaturesWarfare);
        UnitSuppliesCapacity = "UnitSuppliesCapacity".GetFeature(JsonReader.FeaturesWarfare);
        ArmyActiveStaff = "ArmyActiveStaff".GetFeature(JsonReader.FeaturesWarfare);
        ArmyReserve = "ArmyReserve".GetFeature(JsonReader.FeaturesWarfare);
        UnitClass = "UnitClass".GetFeature(JsonReader.FeaturesWarfare);

        // Load Steam Player Info
        barItemPlayerOnline.Caption = "Players: " + Loader.SteamPlayerCount;
        void OnBarBtnSteamPlayerRefreshOnItemClick(object o, ItemClickEventArgs args)
        {
            Task.Run(() =>
            {
                barItemPlayerOnline.Caption = $@"Players: .";
                Thread.Sleep(200);
                barItemPlayerOnline.Caption = $@"Players: ..";
                Thread.Sleep(200);
                barItemPlayerOnline.Caption = $@"Players: ...";
                Thread.Sleep(200);
                barItemPlayerOnline.Caption = $@"Players: {Loader.GetSteamPlayerCount().Result.ToString()}";
            });
        }
        barBtnSteamPlayerRefresh.ItemClick += OnBarBtnSteamPlayerRefreshOnItemClick;

        ceModeHover.CheckStateChanged += (o, args) => { JsonReader.FeatureArmyEnemyEnabled = ceModeHover.Checked; };

        // this.LookAndFeel.SetSkinStyle(SkinStyle.VisualStudio2013Dark);
        // var s = skinPaletteDropDownButtonItem1.DropDownControl as GalleryDropDown
        // Enablind timer after datasource set
        mainTimer.Enabled = true;
        
        // Set default IsNaval.
        // if (!string.IsNullOrEmpty(UnitClass.value))
            IsNaval = Events.IsNaval;
            // IsNaval = rw.SRIsNaval(Convert.ToInt32(UnitClass.value));

        // Set Checkbox Bindings
        CeSetDataBindings(cbADayArmy, ADayArmy);
        CeSetDataBindings(cbADayBuild, ADayBuild);
        CeSetDataBindings(ceSatComm, SatelliteCommCoverage);
        CeSetDataBindings(ceSatMilDef, SatelliteMissileDefenseCoverage);
        CeSetDataBindings(ceSatRecon, SatelliteReconCoverage);
        CeSetDataBindings(ceEfficiency, ArmyEfficiency);
        CeSetDataBindings(ceEntrenchment, ArmyEntrenchment);
        CeSetDataBindings(ceExperience, ArmyExperience);
        CeSetDataBindings(ceMorale, ArmyMorale);
        CeSetDataBindings(ceUnitFuel, ArmyFuel);
        CeSetDataBindings(ceUnitStrength, ArmyCurrentStrength);
        CeSetDataBindings(ceUnitSupplies, ArmySupply);
        CeSetDataBindings(ceGarrisonEfficiency, BuildingGarrisonEfficiency);
        CeSetDataBindings(ceGarrisonInstant, BuildingGarrisonActive);

        
        //SpinEdit
        SetControlTag(seStaffActive, "ArmyActiveStaff");
        SetControlTag(seStaffReserve, "ArmyReserve");
        SetControlTag(seNavalStrActual, "ArmyActualStrength");
        SetControlTag(seNavalStrCurrent, "ArmyCurrentStrength");
        SetControlTag(seUnitStrActual, "ArmyActualStrength");
        SetControlTag(seUnitStrCurrent, "ArmyCurrentStrength");
        SetControlTag(seUnitFuel, "ArmyFuel");
        SetControlTag(seUnitSupplies, "ArmySupply");
        //CheckEdit
        SetControlTag(ceEfficiency, "ArmyEfficiency");
        SetControlTag(ceExperience, "ArmyExperience");
        SetControlTag(ceMorale, "ArmyMorale");
        // setContorlTag(ceNavalStrength, "ArmyCurrentStrength,ArmyActualStrength");
        SetControlTag(ceUnitFuel, "ArmyFuel");
        SetControlTag(ceUnitStrength, "ArmyCurrentStrength");
        SetControlTag(ceUnitSupplies, "ArmySupply");
        // setSpinEdit(new SpinEdit[]{seNavalStrActual, seNavalStrCurrent}, ceNavalStrength.Tag.CastTo<List<Feature>>());
        // setSpinEdit(new []{seUnitStrActual, seUnitStrCurrent}, ceUnitStrength.Tag.CastTo<List<Feature>>());

        // When changed then changed
        SetSpinEditorEvent(seUnitStrActual);
        SetSpinEditorEvent(seUnitStrCurrent);
        SetSpinEditorEvent(seNavalStrActual);
        SetSpinEditorEvent(seNavalStrCurrent);
        SetSpinEditorEvent(seUnitFuel);
        SetSpinEditorEvent(seUnitSupplies);
        SetSpinEditorEvent(seStaffReserve);
        // Set Global Event for multiple control
        // gvCountry.ValidateRow += new ValidateRowEventHandler(gvValidateRow);
        // gvResources.ValidateRow += new ValidateRowEventHandler(gvValidateRow);
        // gvWarfare.ValidateRow += new ValidateRowEventHandler(gvValidateRow);
        
        // When DATASOURCE of Gridview is changed
        gvCountry.DataSourceChanged += GvDataSourceChanged;
        gvResources.DataSourceChanged += GvDataSourceChanged;
        gvWarfare.DataSourceChanged += GvDataSourceChanged;
        gvModifiedUnit.DataSourceChanged += GvDataSourceChanged;
        gvPersistentUnit.DataSourceChanged += GvDataSourceChanged;
        
        //
        leTechEffect1.Tag = "TechEffectIndustrializationEffect1";
        leTechEffect2.Tag = "TechEffectIndustrializationEffect2";
        leTechEffect1.Validated += Events.OnTechEffectValidated;
        leTechEffect2.Validated += Events.OnTechEffectValidated;
        
        leBuildModeOverride.Tag = "BuildModeFacilityIDOverride";
        leBuildModeOverride.Validated += Events.OnTechEffectValidated;



        // Method to set gridview init events etc.
        SetGvEvents(gvCountry, JsonReader.DataCountry);
        SetGvEvents(gvResources, JsonReader.DataResources);
        SetGvEvents(gvWarfare, JsonReader.DataWarfare);
        SetGvEvents(gvFacility, JsonReader.DataSpecial);

        // Populate Combo box warfare unit value
        PopulateWarfareTypes(lookBoxUnitClass, WarfareValueDictionary.Instance.DictUnitClassType);
        PopulateWarfareTypes(lookBoxUnitMovementType, WarfareValueDictionary.Instance.DictUnitMovementType);
        PopulateWarfareTypes(lookBoxUnitTargetType, WarfareValueDictionary.Instance.DictUnitTargetType);

        // Sort Datagrid record Order based on SRUL.Types collection,
        // jsonReader.dgOrder(gvCountry);
        // jsonReader.dgOrder(gvResources);
        // jsonReader.dgOrder(gvWarfare);
        // gvWarfare.Columns["original"].Visible = true;

        //Set Row style for editable
        using (var rs = new SRStyle())
        {
            // Init Row Highlight
            rs.GvHighlight(gvCountry);
            rs.GvHighlight(gvResources);
            rs.GvHighlight(gvWarfare);
            rs.GvHighlight(gvFacility);
            
            // Init Row Style
            rs.gvRowStyle(gvCountry);
            rs.gvRowStyle(gvResources);
            rs.gvRowStyle(gvWarfare);
            rs.gvRowStyle(gvFacility);
        }

        // gvCountry.FormatRules[0].Column = gvCountry.Columns["value"];

        // Custom Text Display Tonnes, $ sign, % when row is displayed
        // gvCountry.CustomColumnDisplayText += gvCustomColumnDisplayText;
        // gvWarfare.CustomColumnDisplayText += gvCustomColumnDisplayText;
        // gvResources.CustomColumnDisplayText += gvCustomColumnDisplayText;

        // gvCountry.CustomColumnSort += OnCustomColumnSort;

        // For Cell Editing custom, spinedit etc in a row.
        AddRepositoryItem(JsonReader.FeaturesCountry, gcCountry, gvCountry);
        AddRepositoryItem(JsonReader.FeaturesResources, gcResources, gvResources);
        AddRepositoryItem(JsonReader.FeaturesWarfare, gcWarfare, gvWarfare);
        AddRepositoryItem(JsonReader.FeaturesSpecial, gcFacility, gvFacility);

        // Flash Helper

        // SE percentage
        // SetSpinEditDisplayPercentage(seNavalStrActual);
        // SetSpinEditDisplayPercentage(seNavalStrCurrent);
        SetSpinEditDisplayPercentage(seNavalStrActual, true);
        SetSpinEditDisplayPercentage(seNavalStrCurrent, true);
        // pbUnitHealthBar.custom

        barBtnHealAllUnit.ItemClick += (o, e) =>
        {
            SRMemento.Instance.HealAllUnits();
        };

        SETTINGS_INIT_ALWAYSONTOP();
        
        // Custom Display for HEALTH Bar, supposed to display overload value
        pbUnitHealthBar.Tag = "ArmyCurrentStrength";
        pbUnitFuelBar.Tag = "ArmyFuel";
        pbUnitSupplyBar.Tag = "ArmySupply";
        pbUnitHealthBar.CustomDisplayText += SetProgressBarDisplayOverload;
        pbUnitFuelBar.CustomDisplayText += SetProgressBarDisplayOverload;
        pbUnitSupplyBar.CustomDisplayText += SetProgressBarDisplayOverload;

        pbWarfarePetrolProduction.CustomDisplayText += ProgressBarWarfareResourceDisplay;
        pbWarfarePetrolStock.CustomDisplayText += ProgressBarWarfareResourceDisplay;
        pbWarfareMGStock.CustomDisplayText += ProgressBarWarfareResourceDisplay;
        pbWarfareMGProduction.CustomDisplayText += ProgressBarWarfareResourceDisplay;

        // Special Warfare OptioncheckedListBoxControl1
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
            
            checkedListBoxControl1.SetItemValue(args.State == CheckState.Checked, args.Index);
            // ((HandmadeFeature)checkedListBoxControl1.SetItemValue(args.Index)).freeze = args.State == CheckState.Checked;
        };

        // 2.0.0.0 Unit history and Persistent
        btnRemovePersistentUnit.Click += (s, args) =>
        {
            // gvWarfare.Columns["gridId"].FieldNameSortGroup = "value";
            if (gvPersistentUnit.RowCount < 1) return;
            // var selectedRowIndex = gvPersistentUnit.GetSelectedRows()[0];
            // var rowValue = gvPersistentUnit.GetRowCellValue(selectedRowIndex, "Address");
            try
            {
                gvPersistentUnit.BeginUpdate();
                for (int i = 0; i < gvPersistentUnit.DataRowCount; i++)
                {
                    if (!gvPersistentUnit.IsRowSelected(i)) continue; 
                        SRMemento.Instance.RemovePersistantUnit(gvPersistentUnit, 
                            gvPersistentUnit.GetRowCellValue(i, 
                                "Address").ToString());
                }
            }
            finally
            {
                gvPersistentUnit.EndUpdate(); 
                gcPersistentUnit.RefreshDataSource();
            }
            // gcPersistentUnit.DataSource = SRMemento.Instance.UnitPersistentList;
        };

        btnRemoveAllPersistentUnit.Click += (s, args) =>
        {
            // if (gvPersistentUnit.RowCount < 1) return;
            // SRMemento.Instance.RemoveAllPersistantUnit(gvPersistentUnit);
            // gcPersistentUnit.RefreshDataSource();
        };

        btnMakePersistent.Click += (o, args) =>
        {
            if (!TrackedUnitsController.IsUnitTracked("ArmyCurrentStrength".GetPointerUIntPtr(rw)))
            {
                TrackedUnitsController.MakePersistent(rw, JsonReader.FeaturesWarfare, Settings.WarfareTABPersistantAutoFreeze);
                // Camp.AddTrackedSoldier("ArmyCurrentStrength".GetPointer(Rw), "");
            }
            else
            {
                TrackedUnitsController.RemoveSelectedTrackedUnit(gvPersistentUnit);
            }
            gcPersistentUnit.RefreshDataSource();
        };

        void BtnRemoveTrackedUnit()
        {
            // if (!SRMemento.Instance.UnitIsPersistent("ArmyCurrentStrength".GetPointer(rw)))
            //     SRMemento.Instance.RemovePersistantUnit(gvPersistentUnit, "ArmyCurrentStrength".GetPointer(rw));
            // gcPersistentUnit.RefreshDataSource();
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

        Settings_TrainerBtnReloadTrainerData.ItemClick += (o, args) => { Loader.ReloadTrainerData(); };

        // Restore Original Stat Value
        btnRestoreUnitToOriginal.Click += (o, args) =>
        {
            if (gvModifiedUnit.RowCount < 1) return;
            var selectedRowIndex = gvModifiedUnit.GetSelectedRows()[0];
            var address = gvModifiedUnit.GetRowCellValue(selectedRowIndex, "UnitAddress");
            if (SRMemento.Instance.RestoreToOriginal(address.ToString(), JsonReader.FeaturesWarfare, rw))
            {
                gcModifiedUnit.RefreshDataSource();
                // gcModifiedUnit.del
            }
        };
        
        // Facilities Spin editor Validation

        #region Facilities Control Initiliazization

        SetControlTag(seFacBuildTime, "FacilityBuildTime");
        SetControlTag(seFacCost, "FacilityCost");
        SetControlTag(seFacDefenseAir, "FacilityDefenseAir");
        SetControlTag(seFacDefenseGround, "FacilityDefenseGround");
        SetControlTag(seFacDefenseIndirect, "FaciliityDefenseIndirect");
        SetControlTag(seFacDefenseClose, "FacilityDefenseClose");
        SetControlTag(seFacSupplyLevel, "FacilitySupplyLevel");
        SetControlTag(seFacStaff, "FacilityStaff");
        SetControlTag(seFacIGNeeded, "FacilityIGNeeded");
        SetControlTag(seFacMILGNeeded, "FacilityMGNeeded");

        // Garrison
        SetControlTag(ceGarrisonEfficiency, "BuildingGarrisonEfficiency");
        SetControlTag(seGarrisonEfficiency, "BuildingGarrisonEfficiency");

        //All Socials
        //setContorlTag(seSocials, "");


        // Display as percentage
        SetSpinEditDisplayPercentage(seGarrisonEfficiency, true);
        SetSpinEditDisplayPercentage(seFacSupplyLevel, true);
        SetSpinEditDisplayPercentage(seTechEffectModifier1, true);
        SetSpinEditDisplayPercentage(seTechEffectModifier2, true);
        SetSpinEditDisplayPercentage(seSocials, true);

        // when edited
        SetSpinEditorEvent(seFacBuildTime);
        SetSpinEditorEvent(seFacCost);
        SetSpinEditorEvent(seFacDefenseAir);
        SetSpinEditorEvent(seFacDefenseGround);
        SetSpinEditorEvent(seFacDefenseIndirect);
        SetSpinEditorEvent(seFacDefenseClose);
        SetSpinEditorEvent(seFacSupplyLevel);
        SetSpinEditorEvent(seFacStaff);
        SetSpinEditorEvent(seFacIGNeeded);
        SetSpinEditorEvent(seFacMILGNeeded);
        SetSpinEditorEvent(seGarrisonEfficiency);
        //setSpinEditorEvent(seSocials);

        #endregion

        #region Tech Effect

        PopulateLookUpEditDataSource(leTechEffect1, TechEffects.TechDictionarySource);
        PopulateLookUpEditDataSource(leTechEffect2, TechEffects.TechDictionarySource);
        PopulateLookUpEditDataSource(leBuildModeOverride, BuildingList.Facilities);

        SetControlTag(seTechEffectModifier1, "TechEffectIndustrializationModifier1");
        SetControlTag(seTechEffectModifier2, "TechEffectIndustrializationModifier2");
        SetSpinEditorEvent(seTechEffectModifier1);
        SetSpinEditorEvent(seTechEffectModifier2);
        
        // SetCheckEditEvents(ceCheatAllUnits, "CheatAllUnits", "2", "0");
        // SetCheckEditEvents(ceCheatFogOfWar, "CheatFogOfWar", "200", "201");
        

        // SetCheckEditEvents(ceCheatFogOfWar, "CheatFogOfWar", "136", "137");

        #endregion

        UnitModelSizeWriter();
        
        // Makes clicking resource ICON change daily/annually figure
        labelControl33.Click += OnLabelControl3233OnClick;
        labelControl32.Click += OnLabelControl3233OnClick;
        labelControl33.Text = Settings.WarfareTABShowResourceAsDailyAnnually ? "Annual" : "Daily";
        labelControl32.Text = Settings.WarfareTABShowResourceAsDailyAnnually ? "Annual" : "Daily";
        // Makes clicking warfare resource display progressbar change representation figure
        pbWarfarePetrolProduction.Click += (o, __) => 
            ChangeWarfareTabShowResourceAsPercentage(null);
        pbWarfarePetrolStock.Click += (o, __) => 
            ChangeWarfareTabShowResourceAsPercentage(null);
        pbWarfareMGStock.Click += (o, __) => 
            ChangeWarfareTabShowResourceAsPercentage(null);
        pbWarfareMGProduction.Click += (o, __) => 
            ChangeWarfareTabShowResourceAsPercentage(null);
        
        pbWarfarePetrolStock.Properties.Step = 1;
        pbWarfarePetrolProduction.Properties.Step = 1;
        pbWarfareMGStock.Properties.Step = 1;
        pbWarfareMGProduction.Properties.Step = 1;

            // when sub tab selected, modify columns
        xtabSubWarfare.Selected += (o, args) =>
        {
            gvPersistentUnit.BestFitColumns();
            if(gvPersistentUnit.Columns.ColumnByFieldName("RowId") != null) 
                gvPersistentUnit.Columns["RowId"].OptionsColumn.FixedWidth = true; 
            
            gvModifiedUnit.BestFitColumns();
            if(gvModifiedUnit.Columns.ColumnByFieldName("UnitId") != null) 
                gvModifiedUnit.Columns["UnitId"].OptionsColumn.FixedWidth = true;
        };

        rtExperience.EditValueChanged += (o, args) =>
        {
            ArmyExperience.WriteTo(rw, rtExperience.Rating.ConvertRating().ToString());
        };
        
        _pbUsages = new[]
        {
            pbWarfarePetrolProduction,
            pbWarfareMGStock,
            pbWarfarePetrolStock,
            pbWarfareMGProduction
        };

        // GridColumn firstGroupColumn = gvResources.SortInfo[0].Column;
        // GridSummaryItem summaryItemMaxOrderSum = gvResources.GroupSummary.Add(SummaryItemType.Max, 
        //     "OrderSum", null, 
        //     "(MAX Order Sum = {0:c2})");
        // GroupSummarySortInfo[] groupSummaryToSort = { new GroupSummarySortInfo(summaryItemMaxOrderSum, firstGroupColumn, ColumnSortOrder.Ascending) };
        // gridView.GroupSummarySortInfo.ClearAndAddRange(groupSummaryToSort);

        // gvResources.s;
        //
        // gvResources.SortInfo.ClearAndAddRange(new[]
        // {
        //     new GridMergedColumnSortInfo(
        //         new [] { gvResources.Columns["value"], gvResources.Columns["value"]  },
        //         new [] { ColumnSortOrder.Ascending }),
        //     new GridColumnSortInfo(gvResources.Columns["value"], ColumnSortOrder.Ascending)
        // });

        Settings_GeneralBsiMiscellaneous.AddItem(new BarHeaderItem { Caption = "Warfare" });
        SettingsBarSubmenuAddItem(Settings_GeneralBsiMiscellaneous, new BarToggleSwitchItem()
        {
            Name = "Settings_General_Warfare_InfoDisplayPercentage",
            Caption = "[Info] Percentage Display",
            Checked = Settings.WarfareTABShowResourceAsPercentage,
        }, "WarfareTABShowResourceAsPercentage", "BindableChecked");
        SettingsBarSubmenuAddItem(Settings_GeneralBsiMiscellaneous, new BarToggleSwitchItem()
        {
            Name = "Settings_General_Warfare_InfoDailyAnnually",
            Caption = "[Info] Daily/Annually",
            Checked = Settings.WarfareTABShowResourceAsDailyAnnually,
        }, "WarfareTABShowResourceAsDailyAnnually", "BindableChecked");
        SettingsBarSubmenuAddItem(Settings_GeneralBsiMiscellaneous, new BarToggleSwitchItem() {
            Name = "Settings_General_Warfare_UnitPersistAutoFreeze",
            Caption = "[Unit] Persist Auto Freeze",
            Checked = Settings.WarfareTABPersistantAutoFreeze,
        }, "WarfareTABPersistantAutoFreeze", "BindableChecked");

        // ceModePersistent.EditValue;
        SetControlBindingToSettings(
            ceModePersistent,
            "checked",
            "WarfareTABModeTracking");
        SetControlBindingToSettings(
            ceModeHover,
            "checked",
            "WarfareTABModeHover");
        SetControlBindingToSettings(
            Settings_FormSetOpacity, 
            "EditValue",
            "TrainerOpacity");
        SetControlBindingToSettings(
            ceRailRoadDaysToBuild, 
            "checked",
            "SpecialTABRailRoadDaysToBuild");
        SetControlBindingToSettings(
            Settings_FormToogleOnTop, 
            "checked",
            "UserAlwaysOnTop");
        
        // TODO: Refactor, quick hax.
        ceRailRoadDaysToBuild.CheckedChanged += (o, args) =>
        {
            if (ceRailRoadDaysToBuild.Checked) return;
            StaticRailPointer = null;
                StaticRoadPointer = null;
            StaticRailCostPointer = null;
                StaticRoadCostPointer = null;
            StaticRailOriginalCostValue = null;
                StaticRoadOriginalCostValue = null;
            StaticRoadOriginalValue= null;
                StaticRailOriginalValue = null;

                StaticRoadIGNeededOriginalValue = null;
                StaticRoadIGNeededPointer = null;
                StaticRailIGNeededPointer = null;
                StaticRailIGNeededOriginalValue = null;
        };
        
        InitInfo();
        
        sbResourceAddMillionToAllResources.Click += (o, args) =>
        {
            foreach (var resource in JsonReader.ResourcesIndexedFeatures)
            {
                if (!resource.Value.displayName.Equals("Stock")) continue;
                if (!resource.Value.value.As<float>().HasValue) continue;
                float hundred = resource.Value.value.As<float>().Value + 500000;
                if(!float.IsNaN(hundred)) 
                    resource.Value.WriteTo(rw, hundred.ToString());
            }
        };
        
        seUnitStrCurrent.Properties.ValidateOnEnterKey = true;
        seUnitSupplies.Properties.ValidateOnEnterKey = true;
        seUnitFuel.Properties.ValidateOnEnterKey = true;
        seUnitStrActual.Properties.ValidateOnEnterKey = true;
        seGarrisonEfficiency.Properties.ValidateOnEnterKey = true;
        
        // TrackedUnitsController.SetMasterDetailControlViews(
        //     ref gcPersistentUnit,
        //     ref gvPersistentUnit,
        //     ref gvPersistentUnitStats);
            
        TrackedUnitsController.Initialize(
            gcPersistentUnit, 
            gvPersistentUnit, 
            gvPersistentUnitStats);
        
        SpecialCheats.InitView(ref ceListBoxCheats);

        bool Reloader()
        {
            var staticAddr = SRMain.Instance.FeaturePointerStore["Treasury"];
            var dynamicAddr = SRMain.Instance.FeaturePointerStoreRaw?["Treasury"];
            if(dynamicAddr == null) return false;
            if(staticAddr == null) return false;
            var staticValue = rw.Read("float", staticAddr);
            var dynamicValue = rw.Read("float", dynamicAddr);
            
            if (staticValue != dynamicValue) {
                if (float.IsNaN(staticValue)) return false;
                if (!rw.MemoryAvailable(dynamicAddr)) return false; 
                Loader.ReloadTrainerData(); 
                return true;
            }

            return false;
        }
        
        int _lock = 0; 
        Refresher.InitializeTimer((o) =>
        {
            // gcWarfare.Enabled = !String.IsNullOrEmpty("UnitName".GetFeature(JsonReader.FeaturesWarfare).value);

            UpdateDataGridView(gvResources, JsonReader.ResourcesIndexedFeatures); 
            UpdateDataGridView(gvCountry, JsonReader.CountryIndexedFeatures); 
            UpdateDataGridView(gvWarfare, JsonReader.WarfareIndexedFeatures); 
            UpdateDataGridView(gvFacility, JsonReader.SpecialIndexedFeatures);
            
            if(xtabMainControl.SelectedTabPage == xtabWarfare)
                RefreshDataGridRowCell(gvWarfare);
            if(xtabMainControl.SelectedTabPage == xtabSpecial)
                RefreshDataGridRowCell(gvFacility);
            if(xtabMainControl.SelectedTabPage == xtabResources)
                RefreshDataGridRowCell(gvResources);
            if(xtabMainControl.SelectedTabPage == xtabCountry) 
                RefreshDataGridRowCell(gvCountry);

            if (Reloader())
            {
                ceRailRoadDaysToBuild.Checked = !ceRailRoadDaysToBuild.Checked;
                ceRailRoadDaysToBuild.Checked = !ceRailRoadDaysToBuild.Checked;
                PlaySound(Resources.GunReload);
            }
                // specialCheats.Writer();

            if (gvPersistentUnit.IsEditing) return;
            TrackedUnitsController.RefreshMasterTimer(gvPersistentUnit, rw); 
            TrackedUnitsController.RefreshDetailTimer(gvPersistentUnitStats, rw);

            if (_miniWindow == null) return;
            TrackedUnitsController.RefreshMasterTimer(_miniWindow.gvPersistentUnit, rw);
            TrackedUnitsController.RefreshDetailTimer(_miniWindow.gvPersistentUnitStats, rw);
            
        });
    }

        private void OnLabelControl3233OnClick(object _, EventArgs __)
        {
            var bs =
                Settings_GeneralBsiMiscellaneous.Manager.Items["Settings_General_Warfare_InfoDailyAnnually"] as
                    BarToggleSwitchItem;
            bs.BindableChecked = !bs.BindableChecked;
            labelControl33.Text = Settings.WarfareTABShowResourceAsDailyAnnually ? "Annual" : "Daily";
            labelControl32.Text = Settings.WarfareTABShowResourceAsDailyAnnually ? "Annual" : "Daily";
        }

        private void ChangeWarfareTabShowResourceAsPercentage(ProgressBar pb)
        {
            var bs = 
                Settings_GeneralBsiMiscellaneous.Manager.Items["Settings_General_Warfare_InfoDisplayPercentage"]
                as BarToggleSwitchItem;
            bs.BindableChecked = !bs.BindableChecked;
            foreach (var pbDisplay in _pbUsages) {
                pbDisplay.Refresh();
                pbDisplay.PerformStep();
            }
        }

        #endregion
        public void SetControlBindingToSettings(object ce, string propertyName, string settingName)
        {
            if (ce.GetType() == typeof(BarToggleSwitchItem))
            {
                ((BarItem)ce).DataBindings.Add(propertyName, Settings, 
                    settingName, 
                    false, 
                    DataSourceUpdateMode.OnPropertyChanged);
            }
            
            if (ce.GetType() == typeof(CheckEdit))
            {
                    ((CheckEdit)ce).DataBindings.Add(propertyName, Settings, 
                    settingName, 
                    false, 
                    DataSourceUpdateMode.OnPropertyChanged);   
            }
            // ce.CheckedChanged += (sender, e) =>
            // {
            //     if (ce.EditValue == null) return;
            //     Properties.Settings.Default.Save();
            // };
        }
        private void InitInfo()
        {
            // SRInfo.Instance.SRDonationButton(btnDonate, TrainerEnum.Paypal);
            // SRInfo.Instance.SRDonationButton(btnDonorBoxDonate, TrainerEnum.DonorBox);
            btnDonateLinksPopup.Click += (o, args) => SRInfo.Instance.SRGenerateLinksPopup();
            SRInfo.Instance.SRBarDonationHeader(barBtnDonation);
            SRInfo.Instance.SRBarDonationButton(barBtnDonationBottom);
            barHeaderVersion.Caption = Loader.CurrentProductVersion;
            barStaticItem1.Caption = $@"Game Version: {JsonReader.activeTrainer.GameVersion}";

            SRInfo.Instance.SRProductInformation(reInfo);
            SRInfo.Instance.SRChangeLog(reInfoChangelog, JsonReader.Data.SRFChangelog);
            SRInfo.Instance.SRLoadCheatTable(reExtraCheatTable);
        }
        internal void GvLoad(GridView gv)
        {
            gv.OptionsMenu.EnableColumnMenu = false;
            gv.OptionsMenu.EnableFooterMenu = false;
            gv.OptionsCustomization.AllowSort = false;
            gv.OptionsView.ColumnAutoWidth = true;
            gv.OptionsView.ShowVerticalLines = DefaultBoolean.True;
            // gv.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gv.OptionsDetail.SmartDetailExpandButtonMode = DetailExpandButtonMode.AlwaysEnabled;
            // initFlashCell(gv);
        }
        
        public object this[string propertyName] 
        {
            get
            {
                // probably faster without reflection:
                // like:  return Properties.Settings.Default.PropertyValues[propertyName] 
                // instead of the following
                Type myType = typeof(MainForm);                   
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                return myPropInfo.GetValue(this, null);
            }
            set
            {
                Type myType = typeof(MainForm);                   
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                myPropInfo.SetValue(this, value, null);
            }
        }

        void SettingsBarSubmenuAddItem(BarSubItem subMenu, BarToggleSwitchItem bts, string settingName, string propName)
        {
            bts.DataBindings.Add(propName, Settings, settingName, false, DataSourceUpdateMode.OnPropertyChanged);
            if(!subMenu.ContainsItem(bts)) 
                subMenu.AddItem(bts);
        }
        void SettingsAddButton(BarManager bm, BarItem bi, string name, string caption, string tooltip, string image)
        {
            bi.Caption = caption;
            bi.Name = name;
            bi.Hint = tooltip;
            bi.ImageOptions.Image = image == null ? null : Image.FromFile(image);
            bm.Items.Add(bi);
        }

        void ShowHealthBar()
        {
            if (JsonReader.feature("UnitName").value == "")
            {
                pbUnitHealthBar.Position = 0;
                pbUnitFuelBar.Position = 0;
                pbUnitSupplyBar.Position = 0;
                return;
            }
   
            // V3
            var unitSupply = double.Parse(ArmySupply.value == String.Empty ? 0.ToString() : ArmySupply.value);
            var unitFuel = double.Parse(ArmyFuel.value == String.Empty ? 0.ToString() : ArmyFuel.value);
            var unitFuelCap = double.Parse(UnitFuelCapacity.value == String.Empty ? 0.ToString() : UnitFuelCapacity.value);
            var unitSupplyCap = double.Parse(UnitSuppliesCapacity.value == String.Empty ? 0.ToString() : UnitSuppliesCapacity.value);
            var unitCurrent = double.Parse(ArmyCurrentStrength.value == String.Empty ? 0.ToString() : ArmyCurrentStrength.value);
            var unitActual = double.Parse(ArmyActualStrength.value == String.Empty ? 0.ToString() : ArmyActualStrength.value);
            
            // pbUnitHealthBar.Tag = "Health";
            // pbUnitFuelBar.Tag = "Fuel";
            // pbUnitSupplyBar.Tag = "Supply";

            if (Events.IsNaval)
            {
                unitActual *= 100;
                unitCurrent *= 100;
                unitSupply *= 100;
            }
            pbUnitHealthBar.Properties.Maximum = (int) (unitActual * 1);
            pbUnitFuelBar.Properties.Maximum = (int) (unitCurrent * unitFuelCap);
            pbUnitSupplyBar.Properties.Maximum = (int) (unitCurrent * unitSupplyCap);

            // Should be multiplied because maximum properties altered *100 if unit is naval, so the position stays.
            if (Events.IsNaval)
            {
                unitFuel *= 100;
            }

            pbUnitHealthBar.EditValue = ((int) unitCurrent).ToString(); // 12
            pbUnitFuelBar.EditValue = ((int) unitFuel).ToString();
            pbUnitSupplyBar.EditValue = ((int) unitSupply).ToString();
            // pbUnitHealthBar.Tag = UnitF;
        }

        void PlayedTime()
        {
            var watch = new System.Windows.Forms.Timer();
            TimeSpan elapsed = DateTime.UtcNow - ActiveTrainer.Instance.GameProcess.StartTime.ToUniversalTime();
            watch.Tick += (o, args) =>
            {
                // var elapsed = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();
                if (ActiveTrainer.Instance.GameProcess == null) return;
                if (!ActiveTrainer.Instance.GameProcess.HasExited)
                { 
                    elapsed = DateTime.UtcNow - ActiveTrainer.Instance.GameProcess.StartTime.ToUniversalTime(); 
                    var sFormat = elapsed.ToString(@"hh\:mm\:ss") + " Playtime";
                    lblUserClock.Text = sFormat;   
                }
            };
            watch.Enabled = true;
        }

        internal void ProgressBarWarfareResourceDisplay(object sender, CustomDisplayTextEventArgs e)
        {
            ProgressBarControl? pbc = sender as ProgressBarControl;
            if (pbc == null) return;
            var tag = pbc.Tag;
            if (tag == null) return;
            
            e.DisplayText = tag.ToString();
            // if (pbc.Position == pbc.Properties.Maximum)
            // {
            //     pbc.Position -= 1;
            // }
        }
        internal void SetProgressBarDisplayOverload(object sender, CustomDisplayTextEventArgs e)
        {
            ProgressBarControl? pbc = sender as ProgressBarControl;
            if (pbc == null) return;
            Feature? feature = pbc.Tag.ToString().GetFeature(SREnum.CategoryName.Warfare);
            if (feature == null) return;
            
            // Return if feature aren't loaded
            if (String.IsNullOrEmpty(feature.value)) return;

            // Multiply by 100 if naval unit, due to max position altered
            var division = (Events.IsNaval ? Convert.ToDecimal(feature.value) * 100 
                : Convert.ToDecimal(feature.value)).SafeDecimalDivision(Convert.ToDecimal(pbc.Properties.Maximum));
            var p = (division * 100);
                e.DisplayText = String.Format("{0:P1}%", p.ToString("0"));
                if (pbc.Position == pbc.Properties.Maximum)
                {
                    if (feature.name != "ArmyCurrentStrength") return;
                    pbc.Position -= 1;
                    // pbc.Refresh();
                }
        }

        internal void SetSpinEditPropertiesIncrementConditionally(SpinEdit se)
        {
            if(Events.IsNaval)
                if (se.Tag.ToString() == "ArmyCurrentStrength"
                    || se.Tag.ToString() == "ArmyActualStrength")
                    se.Properties.Increment = 0.1m;
                else
                    se.Properties.Increment = 5m;
            
            if (se.Tag.ToString() != "ArmyCurrentStrength")
                se.Properties.Increment = 5m;
            else if (se.Tag.ToString() != "ArmyActualStrength")
                se.Properties.Increment = 5m;
            else
                se.Properties.Increment = 10m;
        }
        internal void SetSpinEditDisplayPercentage(SpinEdit se, bool isNaval)
        {
            if (isNaval)
            {
                se.Properties.IsFloatValue = true;
                se.Properties.DisplayFormat.FormatType = FormatType.Numeric;
                se.Properties.DisplayFormat.FormatString = "P1";
                se.Properties.Mask.EditMask = "p";
                if (se.Properties.Increment == 1m) return;
                se.Properties.Increment = 1m;
            }
            else
            {
                se.Properties.IsFloatValue = false;
                se.Properties.Mask.EditMask = default;
                se.Properties.DisplayFormat.FormatType = default;
                se.Properties.DisplayFormat.FormatString = default;
                if (se.Properties.Increment == 10) return;
                se.Properties.Increment = 10;
            }
            
        }

        public void SetCheckEditEvents(CheckEdit ce, string featureName, string trueValue, string falseValue)
        {
            ce.CheckedChanged += (o, args) =>
            {
                rw.SRWrite(featureName, ce.Checked ? trueValue : falseValue);
            };
        }
        

        /// <summary> The SetGvEvents function sets up the events for a GridView control.
        /// It is called by the constructor of the SRGridView class.</summary>
        ///
        /// <param name="gv"> Gridview</param>
        /// <param name="Category"> /// category of the srul.types collection, 
        /// </param>
        ///
        /// <returns> Void.</returns>
        
        
        Dictionary<string, Image?> _gridIcons = new Dictionary<string, Image?>();
        internal void SetGvEvents(GridView gv, Category category)
        {
            void OnGvOnCustomDrawCell(object s, RowCellCustomDrawEventArgs e)
            {
                // if (e.Column.VisibleIndex != 2) return;
                // e.Cache.FillRectangle(Color.Red, e.Bounds);
                // e.Appearance.DrawString(e.Cache, e.DisplayText, e.Bounds);

                if (e.Column.FieldName == "displayName" 
                    && ListOfSortedRow.rowDisplayNameText.Contains(e.DisplayText))
                {
                    Image? img = null;
                    if (!_gridIcons.ContainsKey(e.DisplayText))
                    {
                        _gridIcons.Add(e.DisplayText, Resources.ResourceManager.GetObject(e.DisplayText) as Image);
                        img = _gridIcons[e.DisplayText];
                    }
                    else img = _gridIcons[e.DisplayText];
                    Rectangle r = e.Bounds;
                    r.X += e.Bounds.Height + 5 * 2;
                    r.Width -= (e.Bounds.Height + 5 * 2);
                    e.Cache.DrawImage(img, JsonReader.DefinedRectangle(e.Bounds.X + 5, e.Bounds.Y, e.Bounds.Height + 2, e.Bounds.Height + 2));
                    e.Appearance.DrawString(e.Cache, e.DisplayText, r);
                    e.Handled = true;
                }
                else
                {
                    e.DefaultDraw();
                }
                // if (e.Column.FieldName == "displayName" && ListOfSortedRow.rowDisplayNameText.Contains(e.DisplayText))
                // {
                //     // if (xtabMainControl.SelectedTabPage == xtabResources)
                //         // for (int i = 0; i < ListOfSortedRow.ResourceIconNames.Length; i++)
                //         // {
                //         //     if (gv.GetRowCellValue(e.RowHandle, "name").ToString().Contains(ListOfSortedRow.ResourcesNameFieldName[i]))
                //         //     {
                //         //         r = e.Bounds;
                //         //         r.X += e.Bounds.Height + 5 * 2;
                //         //         r.Width -= (e.Bounds.Height + 5 * 2);
                //         //         e.Cache.DrawImage(((Image)Resources.ResourceManager.GetObject(ListOfSortedRow.ResourceIconNames[i]))!, 
                //         //             JsonReader.DefinedRectangle(e.Bounds.X + 5, e.Bounds.Y + 5, e.Bounds.Height + 2, e.Bounds.Height + 2));
                //         //         e.Appearance.DrawString(e.Cache, e.DisplayText, r);
                //         //         e.Handled = true;
                //         //     }
                //         // }
                //         view.BeginUpdate();
                //         Rectangle r;
                //         r = e.Bounds;
                //         r.X += e.Bounds.Height + 5 * 2;
                //         r.Width -= (e.Bounds.Height + 5 * 2);
                //         e.Cache.DrawImage((Image)Resources.ResourceManager.GetObject(e.DisplayText), 
                //             new Rectangle(e.Bounds.X + 5, e.Bounds.Y + 5, e.Bounds.Height + 2, e.Bounds.Height + 2));
                //         // e.Cache.DrawImage(Resources.ResourceManager.GetObject($"{e.DisplayText}") as Image, new Rectangle(e.Bounds.X + 5, e.Bounds.Y, e.Bounds.Height + 2, e.Bounds.Height + 2));
                //         e.Appearance.DrawString(e.Cache, e.DisplayText, r);
                //         e.Handled = true;
                //         // using (var sImage = (Image)Resources.ResourceManager.GetObject(e.DisplayText))
                //         // {
                //         // }
                //         view.EndUpdate();
                // }
                // else
                // {
                //     e.DefaultDraw();
                // }
            }
            // Set Global Event for multiple control, SR Write when validated.
            gv.ValidateRow += GvValidateRow;
            // When editor is showing, change ?
            // gv.ShowingEditor += GvShowingEditor;
            // When DATASOURCE of Gridview is changed
            gv.DataSourceChanged += GvDataSourceChanged;
            // Custom Text Display Tonnes, $ sign, % when row is displayed
            gv.CustomColumnDisplayText += GvCustomColumnDisplayText;
            // Custom Cell
            gv.CustomDrawCell += OnGvOnCustomDrawCell;
            // Set Custom Row Filter for Warfare;
            // gv.CustomRowFilter += jsonReader.gvRowFilterExclusion;
            SRViews.gvRowFilterShownOnlyIncludedFeature(gv, category);
            // Sort Datagrid record Order based on SRUL.Types collection,
            JsonReader.dgOrder(gv, category);
        }

        internal void gvCellValueChanged(object sender, CellValueChangedEventArgs e)
        {
          GridView view = sender as GridView;
            // int speed = (int) e.Value;
            view.RefreshData();
        }

        /// <summary> The FieldTypeValueValidator function checks to see if the value is a valid numeric type.
        /// If it is, then it returns true. Otherwise, false.</summary>
        ///
        /// <param name="numericType"> The type of numeric value</param>
        /// <param name="string"> </param>
        ///
        /// <returns> A boolean value</returns>
        internal bool FieldTypeValueValidator(string numericType, string val)
        {
            var value = val;
            ushort isValid = 0;
            switch (numericType)
            {
                case "int":
                    if(!int.TryParse(value, out int a))
                        return false;
                    break;
                case "byte":
                    if (!byte.TryParse(value, out byte b))
                        return false;
                    break;
                case "test2byte":
                    if (!UInt16.TryParse(value, out ushort c))
                        return false;
                    break;
                case "2byte":
                    if (!UInt16.TryParse(value, out ushort d))
                        return false;
                    break;
                case "2bytes":
                    if (!UInt16.TryParse(value, out ushort e))
                        return false;
                    break;
                case "float":
                    if (!float.TryParse(value, out float f))
                        return false;
                    break;
                case "double":
                    if (!double.TryParse(value, out double g))
                        return false;
                    break;
            }
            return true;
        } 

        static RepositoryItemSpinEdit spin = new RepositoryItemSpinEdit();
        static RepositoryItemLookUpEdit lookUp = new RepositoryItemLookUpEdit();
        static RepositoryItemLookUpEdit lookUpUnitTarget = new RepositoryItemLookUpEdit();
        static RepositoryItemLookUpEdit lookUpUnitMovement = new RepositoryItemLookUpEdit();
        static RepositoryItemCheckEdit check = new RepositoryItemCheckEdit();
        static RepositoryItemComboBox combo = new RepositoryItemComboBox();
        static RepositoryItemTextEdit edit = new RepositoryItemTextEdit();
        static RepositoryItemRichTextEdit rich = new RepositoryItemRichTextEdit();
        static RepositoryItemHypertextLabel hypertext = new RepositoryItemHypertextLabel();
        static RepositoryItemHyperLinkEdit hyperlink = new RepositoryItemHyperLinkEdit();
        static RepositoryItemButtonEdit btn = new RepositoryItemButtonEdit();
        object[] repositoryItemCollection = new object[]
        {
            combo, 
            check, 
            spin, 
            lookUp, 
            lookUpUnitTarget,
            lookUpUnitMovement,
            edit,
            rich, 
            hypertext,
            hyperlink,
            btn,
        };
        public void AddRepositoryItem([NotNull] IList<Feature> fs, GridControl gc, GridView gv)
        {
            if (fs == null) throw new ArgumentNullException(nameof(fs));
            
            gc.RepositoryItems.AddRange(new RepositoryItem[] {
                combo, 
                check, 
                spin, 
                lookUp, 
                lookUpUnitTarget,
                lookUpUnitMovement,
                edit, 
                rich, 
                hypertext,
                hyperlink,
                btn,
            });

            // Set Spin Properties
            spin.AllowMouseWheel = true;
            spin.AllowNullInput = DefaultBoolean.False;
            spin.ValidateOnEnterKey = true;

            // Editing All GRID VIEW with spinEditor
            void GvCustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
            {
                GridView? gView = sender as GridView;
                ColumnView? cView = sender as ColumnView;

                var vrow = gView.GetRow(e.RowHandle) as Feature;
                
                if(e.RowHandle < 0)
                {
                    return;
                }

                var formatType = vrow.format;
                var valueType = vrow.type;
                var name = vrow.name;
                var value = vrow.value;
                // var formatType = (string) gView.GetListSourceRowCellValue(e.RowHandle, "format");
                // var valueType = (string) gView.GetListSourceRowCellValue(e.RowHandle, "type");
                // var name = (string) gView.GetListSourceRowCellValue(e.RowHandle, "name");
                // var value = (string) gView.GetListSourceRowCellValue(e.RowHandle, "value");
                // if (name == "ProductionAgricultureAnnually") return;
                
                //Check Register
                spin.Mask.EditMask = default;

                RepositoryItemLookUpEdit unitClassStore = null;

                /// BARRIER
                if (e.Column.FieldName != "value") return;
                if (formatType.Contains("UNITCLASS_TYPES"))
                    unitClassStore = lookUp;
                if (formatType.Contains("UNITMOVEMENT_TYPES"))
                    unitClassStore = lookUpUnitMovement;
                if (formatType.Contains("UNITTARGET_TYPES"))
                    unitClassStore = lookUpUnitTarget;
                
                switch (valueType)
                {
                    case "float":
                        spin.IsFloatValue = true;
                        break;
                    default:
                        spin.IsFloatValue = false;
                        break;
                }
                
                switch (formatType)
                {
                    case var a when a.Contains("UNITCLASS_TYPES"):
                    case var b when b.Contains("UNITMOVEMENT_TYPES"):
                    case var c when c.Contains("UNITTARGET_TYPES"):
                        if (unitClassStore == null) return;
                        if (unitClassStore.DataSource == null) {
                            unitClassStore.DataSource = JsonReader.GetFormatData(name.GetFeature());
                            unitClassStore.DisplayMember = "Value";
                            unitClassStore.ValueMember = "Key";
                            unitClassStore.PopulateColumns();
                        }
                        e.RepositoryItem = unitClassStore;
                        break;
                    case "opinion":
                        var t = WorldMarketOpinion.Instance.List;
                        if (lookUp.DataSource == null)
                        {
                             lookUp.DataSource = t;
                             // lookUp.Columns.Add(new LookUpColumnInfo("test", "test"));
                             lookUp.DisplayMember = "sentiment";
                             lookUp.ValueMember = "value";
                             lookUp.PopulateColumns();
                             lookUp.Columns["value"].Visible = false;
                        }
                         e.RepositoryItem = lookUp;
                         break;
                    case "currency":
                        if (name.ToLower() == "treasury")
                        {
                            // spin.Mask.EditMask = "N";
                            spin.Increment = 1000000000000;
                        }
                        else spin.Increment = 2000;
                        e.RepositoryItem = spin;
                        break;
                    case "percentage":
                        spin.Mask.EditMask = "###########.0#%";
                        // var test = value.As<float>().Value;
                        // var tst = test.IsBetween(1, 100);
                        // var ss = tst;
                       
                        // if (value.As<double>().Value.IsBetween(0, 0.1))
                        //     spin.Increment = (decimal)float.Parse("0.01");
                        //  else if (value.As<double>().Value.IsBetween(0.1, 1))
                        //     spin.Increment = (decimal) float.Parse("0.05");
                        //  else if (value.As<double>().Value.IsBetween(1, 10))
                        //     spin.Increment = (decimal) float.Parse("0.10");
                        //  else if (value.As<double>().Value.IsBetween(10, 100))
                        //     spin.Increment = (decimal) float.Parse("1");
                        //  else 
                            spin.Increment = (decimal) float.Parse("0.10");
                        e.RepositoryItem = spin;
                        break;
                    default:
                        e.RepositoryItem = spin;
                        // spin.Increment = (decimal) float.Parse("0,1");
                        break;
                }
                if (formatType.Contains("volumes"))
                {
                    spin.Mask.EditMask = "####,###,###,###,###";
                    spin.Increment = 1000000;
                }

                // void editValueChanging(object o, ChangingEventArgs args)
                // {
                //     if (formatType.Contains("percentage"))
                //     {
                //         if (args.NewValue.As<double>().Value.IsBetween(0, 0.1))
                //             (o as RepositoryItemSpinEdit).Increment = (decimal)float.Parse("0,01");
                //         else if (args.NewValue.As<double>().Value.IsBetween(0.1, 1))
                //             (o as RepositoryItemSpinEdit).Increment = (decimal) float.Parse("0,1");
                //         else if (args.NewValue.As<double>().Value.IsBetween(1, 10))
                //             (o as RepositoryItemSpinEdit).Increment = (decimal) float.Parse("1");
                //         else if (args.NewValue.As<double>().Value.IsBetween(10, 100))
                //             (o as RepositoryItemSpinEdit).Increment = (decimal) float.Parse("2");
                //         else 
                //             (o as RepositoryItemSpinEdit).Increment = (decimal) float.Parse("0,1");
                //     }
                // }
                //
                // spin.EditValueChanging += editValueChanging;


                // if(formatType.ToLower() != "opinion") 
                // if(formatType.ToLower() != "opinion") 
                //     e.RepositoryItem = spin;
                
                // if (name == "WorldMarketOpinion")
                // {
                //     e.RepositoryItem = lookUp;
                // }
                
                // Make freeze checkbox immediately reflect to change.
                if (gv.FocusedColumn == gv.Columns["freeze"])
                {
                    e.RepositoryItem = check;
                }
            }
            
            //Check spin
            //set checkproerty
            check.CheckedChanged += (s, e) =>
            {
                gv.PostEditor();
            };

            // Spin repisotry validation;
            spin.Validating += (sender, args) =>
            {
                // gv.PostEditor();
            };

            /// <summary> The SaveToHistory function saves the current unit to the history list.</summary>
            ///
            /// <param name="unit"> The unit to save</param>
            /// <param name="string"> The string.</param>
            /// <param name="string"> The string to be converted.</param>
            ///
            /// <returns> A boolean value indicating whether or not the unit was successfully added to the history list.</returns>
            void SaveToHistory(IList<Feature> unit, string statName, string statValue)
            {
                // var unitName = unit.GetFeature("UnitName").value;
                if (unit[0].category.ToLower() != "warfare") return;
                if (SRMemento.Instance.SaveToHistory(unit, rw))
                {
                    SRMemento.Instance.AddModifiedStat(rw, statName, statValue);
                    gvModifiedUnit.RefreshData(); 
                }
                else
                {
                    var unitName = unit.GetFeatureByName("UnitName").value;
                    if (SRMemento.Instance.UnitHistoryList.Any(s => s.UnitName == unitName))
                        SRMemento.Instance.AddModifiedStat(rw, statName, statValue);
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
                string fieldName = string.Empty;
                GridView gView = sender as GridView;
                
                EditFormValidateEditorEventArgs ea = args as EditFormValidateEditorEventArgs;
                if (ea == null)
                    fieldName = _view.FocusedColumn.FieldName;
                else
                    fieldName = ea.Column.FieldName;
                
                
                if (fieldName == "value")
                {
                    var name = gView.GetRowCellValue(gView.FocusedRowHandle, "name");
                    if (String.IsNullOrEmpty(name.ToString())) return;

                    var type = name.ToString().GetFeature().type;
                    args.Valid = FieldTypeValueValidator(type, args.Value.ToString());
                    
                    if(!args.Valid)
                        args.ErrorText = "Value is not valid";

                    if (args.Valid)
                    {
                        SaveToHistory(fs, name.ToString(), args.Value.ToString());
                        rw.SRWrite(name.ToString(), args.Value.ToString());
                        gv.PostEditor();
                    }
                    
                    // DateTime? orderDate = view.GetRowCellValue(view.FocusedRowHandle, colOrderDate) as DateTime?;
                    // if (requiredDate < orderDate) {
                    //     e.Valid = false;
                    //     e.ErrorText = "Required Date is earlier than the order date";
                    // }
                }
            };
            
            gv.ValidateRow += (sender, args) =>
            {
                Feature obj = args.Row as Feature;
                args.Valid = FieldTypeValueValidator(obj.type, obj.value);
                args.ErrorText = "Value Was too Large";
            };
            
            // This event is raised if the row fails validation or cannot be saved to the data source due to database restrictions
            gv.InvalidRowException += (s, e) => {
                //The ExceptionMode parameter controls the actual response to the error
                e.ExceptionMode = ExceptionMode.DisplayError;
            };
            gv.Columns["wiki"].ColumnEdit = hypertext;
            gv.Columns["wiki"].OptionsColumn.AllowEdit = true;
            gv.Columns["wiki"].OptionsColumn.ReadOnly = true;

            gv.CustomRowCellEditForEditing += GvCustomRowCellEditForEditing;
            gv.MeasurePreviewHeight += gridView_MeasurePreviewHeight;
            gv.CustomDrawRowPreview += (sender, args) =>
            {
                GridView view = sender as GridView;
                // view.repo
                StringPainter.Default.DrawString(args.Cache, args.Appearance,
                    view.GetRowPreviewDisplayText(args.RowHandle), args.Bounds);
                args.Handled = true;
            };
        }

        void gridView_MeasurePreviewHeight(object sender, RowHeightEventArgs e)
        {
            GridView view = sender as GridView;
            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
            try
            {
                var gCache = viewInfo.GInfo.AddGraphicsCache(null);
                int mWidth = viewInfo.ViewRects.ColumnPanelWidth - viewInfo.GetPreviewIndent() - viewInfo.ViewRects.IndicatorWidth;
                int htmlTextHeight = StringPainter.Default.Calculate(cache: gCache, appearance: viewInfo.PaintAppearance.Preview, text: view.GetRowPreviewDisplayText(e.RowHandle), maxWidth: mWidth).Bounds.Height;
                e.RowHeight = htmlTextHeight + viewInfo.CellPadding.Top + viewInfo.CellPadding.Bottom;
            }
            finally
            {
                viewInfo.GInfo.ReleaseGraphics();
            }
        }
        
        private WorldUNOpinion ConvertWorldOpinion(float opinion)
        {
            var result = WorldUNOpinion.Unknown;
            return
                opinion.FloatBetween(0, 0.10, true)
                    ? WorldUNOpinion.Outraged
                    : opinion.FloatBetween(0.11, 0.30, true)
                        ? WorldUNOpinion.Dissaproving
                        : opinion.FloatBetween(0.31, 0.40, true)
                            ? WorldUNOpinion.Indifferent
                            : opinion.FloatBetween(0.41, 0.50, true)
                                ? WorldUNOpinion.Satisfied
                                : opinion.FloatBetween(0.51, 0.60, true)
                                    ? WorldUNOpinion.Pleased
                                    : opinion.FloatBetween(0.61, 1000, true)
                                        ? WorldUNOpinion.Delighted : WorldUNOpinion.Unknown; 
            // if (opinion <= 0.10)
            //     return WorldUNOpinion.Outraged;
            // else if (opinion < 0.30)
            //     return WorldUNOpinion.Dissaproving;
            // else if (opinion < 0.40)
            //     return WorldUNOpinion.Concerned;
            // else if (opinion < 0.50)
            //     return WorldUNOpinion.Indifferent;
            // else if (opinion < 0.60)
            //     return WorldUNOpinion.Satisfied;
            // else if (opinion < 0.70)
            //     return WorldUNOpinion.Pleased;
            // else
            //     return opinion > 0.70 ? WorldUNOpinion.Delighted : WorldUNOpinion.Unknown;
        }

        private ColumnView? _view;
        private decimal rFigure(decimal s)
        {
            return !toggleSwitchResourceAnnuallyDaily.IsOn
                ? (s / 365)
                : s;
        }
        // private CultureInfo ciUsa = new CultureInfo("en-US");
        private void GvCustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            var _gv = sender as GridView;
            // var dt = _gv.GetRow(e.) as Feature;
            _view = sender as ColumnView;
            var name = (string) _view.GetListSourceRowCellValue(e.ListSourceRowIndex, "name");
            var formatType = (string) _view.GetListSourceRowCellValue(e.ListSourceRowIndex, "format");
            var type = (string) _view.GetListSourceRowCellValue(e.ListSourceRowIndex, "type");
            // var gridId = (string) view.GetListSourceRowCellValue(e.ListSourceRowIndex, "gridId");
            var displayName = (string) _view.GetListSourceRowCellValue(e.ListSourceRowIndex, "displayName");
            var category = (string) _view.GetListSourceRowCellValue(e.ListSourceRowIndex, "category");
            var value = _view.GetListSourceRowCellValue(e.ListSourceRowIndex, "value");
            var original = _view.GetListSourceRowCellValue(e.ListSourceRowIndex, "original");
            if (value == null) return;
            // Display subcategory
            if (e.Column.FieldName == "subCategory")
                e.DisplayText = name.GetSubCategory()?.categoryDisplayName;

            if (e.Column.FieldName == "value")
            {
                if (!(bool)_view.GetListSourceRowCellValue(e.ListSourceRowIndex, "IsReadable"))
                    e.DisplayText = "Unavailable";
                if(name == "UnitID") 
                    if((string)_view.GetListSourceRowCellValue(e.ListSourceRowIndex, "value") == "65534") 
                        e.DisplayText = "Unavailable";
            }
            
            
            if (e.Column.FieldName == "displayName")
            {
                if (String.IsNullOrEmpty(value.ToString())) return;
                // if (original is null) return;
                if (value.Equals("????")) return;
                // if (original.Equals("????")) return;
                string str;
                switch (name)
                {
                    case { } a when a.Contains("Demand") && a.Contains("Annually"):
                        str = name.Replace(@"Demand", "Production");
                        e.DisplayText = String.Format("{0} ({1}) / Production", displayName,
                            NumericExtension.SafePercentage(
                                value.To<decimal>(), 
                                str.GetFeature(SREnum.CategoryName.Resources)!.value
                                    .To<decimal>(), "P2"));
                        break;
                    case { } a when a.Contains("Production") && a.Contains("Annually"):
                        str = name.Replace(@"Production", "Demand");
                        e.DisplayText = String.Format("{0} ({1} filling demands)", displayName,
                            NumericExtension.SafePercentage(
                                value.To<decimal>(), 
                                str.GetFeature(SREnum.CategoryName.Resources)!.value
                                    .To<decimal>(), "P2"));
                        break;
                    case { } a when a.Contains("ProductionCapacity"):
                        str = $"{name.Replace(@"Capacity", "")}Annually";
                        var used = value.To<decimal>() - str.GetFeature(SREnum.CategoryName.Resources)!.value
                            .To<decimal>();
                        var isAtFullCapacity = used == 0 ? "{0} (FULL)" : "{0} ({1} Left)";
                        e.DisplayText = String.Format(isAtFullCapacity, displayName,
                            NumericExtension.SafePercentage(
                                used, 
                                value.To<decimal>(), "P2"));
                        break;
                    case { } a when a.Contains("Price") && !a.Contains("Base") && !a.Contains("Full")
                                    && category == "Resources":
                        str = name.Replace(@"Price", "Cost");
                        var cost = str.GetFeature(SREnum.CategoryName.Resources).value.To<decimal>();
                        if(value.To<decimal>() < cost)
                            e.DisplayText = String.Format("{0} -({1})", displayName, 
                                // (value.To<decimal>() - cost)
                                // .ToSemanticRepresentation(false), 
                                NumericExtension.SafePercentage(
                                    (value.To<decimal>() - cost), 
                                    value.To<decimal>(), "P0"));
                        else 
                            e.DisplayText = String.Format("{0} +({1} Markup)", displayName, 
                                // (value.To<decimal>() - cost)
                                // .ToSemanticRepresentation(false), 
                                NumericExtension.SafePercentage(
                                    (value.To<decimal>() - cost), 
                                    value.To<decimal>(), "P0"));
                        break;
                }
            }
            else
            {
                var col = new string[2] { "value", "original" };
                if (!col.Contains(e.Column.FieldName)) return;
                // if (e.Column.Caption != "value") return;
                if (e.Value == null) return;
                if (e.Value.Equals("????")) return;
                if ((string)e.Value == "") return;
                if (type == "string" || type == "2bytes") return;
                
                
                // For Annual daily switch
                decimal formattedValue = 0;
                if (!value.As<decimal>().HasValue) return;
                if ( toggleSwitchResourceAnnuallyDaily.IsOn 
                     && name.Contains("Annually") 
                   )
                    Decimal.TryParse((Convert.ToSingle(value) / 365).ToString("N"), out formattedValue);
                else
                    Decimal.TryParse((value).ToString(), out formattedValue);
                
                // For production capacity display text.
                if(toggleSwitchResourceAnnuallyDaily.IsOn && name.Contains($"ProductionCapacity"))
                    Decimal.TryParse((Convert.ToSingle(value) / 365).ToString("N"), out formattedValue);
                
                // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings?redirectedfrom=MSDN
                switch (formatType)
                {
                    case var a when a.Contains("UNITCLASS_TYPES"):
                    case var b when b.Contains("UNITMOVEMENT_TYPES"):
                    case var c when c.Contains("UNITTARGET_TYPES"):
                        // if (original.To<Int32>() >  JsonReader.GetFormatData(name.GetFeature()).Count) break;
                        if (e.Column.FieldName == "value")
                            e.DisplayText = JsonReader.GetFormatData(name.GetFeature())[Convert.ToInt32(value)];
                        if (e.Column.FieldName == "original")
                            e.DisplayText = JsonReader.GetFormatData(name.GetFeature())[Convert.ToInt32(original)];
                        break;
                    case "opinion":
                        e.DisplayText = ConvertWorldOpinion((float) formattedValue).ToString();
                        break;
                    case "currency":
                        // e.DisplayText = String.Format(ciUSA,"{0:C6}M", formattedValue);
                        e.DisplayText = String.Format(new MoneyFormat(), "{0:M}", formattedValue);
                        break;
                    case "percentage":
                        e.DisplayText = String.Format("{0:P1}", formattedValue);
                        break;
                    case "volumes,Tonnes":
                        e.DisplayText = formattedValue.ToSemanticRepresentation(
                            Settings.Default.ResourceTABShowVolumeMetrics ? " - Tonnes" : "", 
                            Settings.Default.ResourceTABShowKMBFormatting);
                        break;
                    case "volumes,Barrels":
                        e.DisplayText = formattedValue.ToSemanticRepresentation(
                            Settings.Default.ResourceTABShowVolumeMetrics ? " - Barrels" : "", 
                            Settings.Default.ResourceTABShowKMBFormatting);
                        break;
                    case "volumes,kg":
                        e.DisplayText = formattedValue.ToSemanticRepresentation(
                            Settings.Default.ResourceTABShowVolumeMetrics ? " - kg" : "", 
                            Settings.Default.ResourceTABShowKMBFormatting);
                        break;
                    case "volumes,MWh":
                        e.DisplayText = formattedValue.ToSemanticRepresentation(
                            Settings.Default.ResourceTABShowVolumeMetrics ? " - MWh" : "", 
                            Settings.Default.ResourceTABShowKMBFormatting);
                        break;
                    case "volumes,m3":
                        e.DisplayText = formattedValue.ToSemanticRepresentation(
                            Settings.Default.ResourceTABShowVolumeMetrics ? " - m3" : "", 
                            Settings.Default.ResourceTABShowKMBFormatting);
                        break;
                    default:
                        if (value.ToString() == "-1")
                            e.DisplayText = "00";
                        break;
                }
                // view.SetRowCellValue(e.ListSourceRowIndex, "formattedValue", formattedValue);   
            }
        }
        
        // {
        //     ce.CheckedChanged += (sender, e) =>
        //     {
        //         var ceChecked = (CheckEdit) sender;
        //         var settingName = ceChecked.Name.Replace("ce", "");
        //         var settingValue = ceChecked.Checked ? "1" : "0";
        //         Settings.Default.SetValue(settingName, settingValue);
        //         Settings.Default.Save();
        //     };
        // }

        void SetControlTag(CheckEdit ce, string tag)
        {
            var temp = new List<Feature>();

            // If CheckBox tag contains , split
            if (tag.Contains(","))
            {
                var split = tag.Split(',');

                // Add splitted varname in tag to Temp List
                foreach (var str in split)
                {
                    temp.Add(JsonReader.feature(str));
                }

                // Apply checkbox Tag with Temporary list
                // Add Event when checkstate is changed
                ce.Tag = temp;
                ce.CheckStateChanged += CeCheckStateChangedEvent;
            }
            else
            {
                // Else, contains only 1 varname param, then do the same.
                temp.Add(JsonReader.feature(tag));
                ce.Tag = temp;
                ce.CheckStateChanged += CeCheckStateChangedEvent;
            }
        }

        // Put Features in tag to, as a bound data
        internal void SetControlTag(SpinEdit se, string tag)
        {
            se.Tag = tag;
            
            // TODO: New Update, 3/16/2022
            // se.DataBindings.Add(new Binding("EditValue", tag.GetFeature(), "value", true,
            //     DataSourceUpdateMode.OnPropertyChanged));
        }
        
        internal void SeSetDataBindings(CheckEdit childCtrl, Feature model)
        {
            childCtrl.DataBindings.Add(new Binding("EditValue", model, "value", true,
                DataSourceUpdateMode.OnPropertyChanged));
        }
        internal void SeSetDataBindings(SpinEdit childCtrl, SpinEdit parent)
        {
            childCtrl.DataBindings.Add(new Binding("EditValue", parent, "EditValue", true,
                DataSourceUpdateMode.OnPropertyChanged));
            // childCtrl.DataBindings.Add(new Binding("IsEditorActive", parent, "IsEditorActive", true,
            //     DataSourceUpdateMode.OnPropertyChanged));
            childCtrl.DataBindings.Add(new Binding("Enabled", parent, "Enabled", true,
                DataSourceUpdateMode.OnPropertyChanged));
        }
        
        internal void SetControlBindings(GridControl childCtrl, GridControl parentCtrl)
        {
            childCtrl.DataBindings.Add(new Binding("DataSource", parentCtrl, "DataSource", true,
                DataSourceUpdateMode.OnPropertyChanged));
        }
        
        internal void SetControlBindings(CheckEdit childCtrl, CheckEdit parentCtrl)
        {
            childCtrl.DataBindings.Add(new Binding("Checked", parentCtrl, "Checked", true,
                DataSourceUpdateMode.OnPropertyChanged));
            childCtrl.DataBindings.Add(new Binding("EditValue", parentCtrl, "EditValue", true,
                DataSourceUpdateMode.OnPropertyChanged));
            childCtrl.DataBindings.Add(new Binding("Enabled", parentCtrl, "Enabled", true,
                DataSourceUpdateMode.OnPropertyChanged));
            childCtrl.DataBindings.Add(new Binding("ReadOnly", parentCtrl, "ReadOnly", true,
                DataSourceUpdateMode.OnPropertyChanged));
            childCtrl.DataBindings.Add(new Binding("ForeColor", parentCtrl, "ForeColor", true,
                DataSourceUpdateMode.OnPropertyChanged));
            childCtrl.DataBindings.Add(new Binding("BackColor", parentCtrl, "BackColor", true,
                DataSourceUpdateMode.OnPropertyChanged));
        }
        
        // internal void SetControlBindings(GridView childCtrl, GridView parentCtrl)
        // {
        //     childCtrl.DataBindings.Add(new Binding("Checked", model, "freeze", true,
        //         DataSourceUpdateMode.OnPropertyChanged));
        // }
        
        internal void CeSetDataBindings(CheckEdit childCtrl, Feature model)
        {
            childCtrl.DataBindings.Add(new Binding("Checked", model, "freeze", true,
                DataSourceUpdateMode.OnPropertyChanged));
        }
        
        internal void CeGenericBinding(CheckEdit childCtrl)
        {
            childCtrl.DataBindings.Add(new Binding("Enabled", 
                childCtrl.Tag, 
                "enabled", 
                true,
                DataSourceUpdateMode.OnPropertyChanged));
        }

        // Set Spin Edit
        internal void SetSpinEdit(SpinEdit[] se, IList<Feature> f)
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

        internal void FacilitiesLookUpEditHandler(string featName, LookUpEdit le)
        {
            var dtsVal = le.Properties.GetDataSourceValue("Value", Int32.Parse(rw.SRRead(featName).ToString())-1);
            if (le.Focused)
            {
                if (le.EditValue != null && le.ItemIndex != null)
                {
                    // le.focu
                    rw.SRWrite(featName, (le.ItemIndex+1).ToString(), "byte");
                    // labelControl6.Text = le.EditValue.ToString();
                }
            }
            else
            {
                // Dillematic, supposed take from json which coupled, or just the varname but coupled to r/w
                if (JsonReader.feature(featName).value != null)
                {
                    le.EditValue = dtsVal;
                }
            }
        }

        internal void UnitClassLookUpEditHandler(string featName, LookUpEdit le)
        {
            if (!featName.GetFeature().enabled) return;
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
                if (JsonReader.feature(featName).value != null)
                {
                    le.EditValue = dtsVal;
                }
            }
        }

        // void leValidated(object sender, validated)
        internal void OnCustomColumnSort(object sender, CustomColumnSortEventArgs e)
        {
            // if (e.Column.FieldName == "displayName" || e.Column.FieldName == "display Name")
            // {
            //     int dayIndex1 = getIndexing((string)e.Value1);
            //     int dayIndex2 = 0;
            //     e.Result = dayIndex1.CompareTo(dayIndex2);
            //     e.Handled = true;
            // }
        }

        internal void PopulateWarfareTypes(LookUpEdit leControl, Dictionary<int, string> dict)
        {
            // var a = new WarfareValueDictionary();
            leControl.Properties.DataSource = dict;
            leControl.Properties.DisplayMember = "Value";
            leControl.Properties.ValueMember = "Key";
            // leControl.EditValue = 0;
            leControl.ItemIndex = 0;
        }

        internal void PopulateLookUpEditDataSource(LookUpEdit leControl, Dictionary<string, int> dict)
        {
            // var a = new WarfareValueDictionary();
            leControl.Properties.DataSource = dict;
            leControl.Properties.DisplayMember = "Key";
            leControl.Properties.ValueMember = "Value";
            // leControl.EditValue = 0;
            leControl.ItemIndex = 0;
        }

        internal void gvCountry_CustomColumnSort(object sender,
            DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
        {
            if (e.Column.FieldName == "displayName")
            {
                e.Handled = true;
            }
        }

        //When DAtasource changed by updatedaragridview()
        internal void GvDataSourceChanged(object sender, EventArgs e)
        {
            GridView view = (GridView) sender;
            // GvHighlight(view);
            view.PostEditor();
            // view.refre();
            // view.refre
        }

        
        internal void GvPersistentDetailRuntime()
        {
            ceNavalUnitIndicator.Checked = Events.IsNaval;
            if(!ceNavalUnitIndicator.ReadOnly) 
                ceNavalUnitIndicator.ReadOnly = true;

            if (ceNavalUnitIndicator.Checked)
                ceNavalUnitIndicator.ForeColor = Color.LimeGreen;
            else 
                ceNavalUnitIndicator.ForeColor = Color.DimGray;
            
            if(!cePersistentUnitIndicator.ReadOnly) 
                cePersistentUnitIndicator.ReadOnly = true;
            
            
            // TODO: To prevent change the same style over and over 
            // Subscribe to events?
            // _lastId = "UnitID".GetFeature().value;
        }
        
        internal void PlaySound(UnmanagedMemoryStream soundName)
        {
            using (var sound = new SoundPlayer(soundName))
            { 
                sound.Play();
            }
            // sound.Dispose();
        }

        private HashSet<int> _masterRowExpanded = new HashSet<int>();
        internal void GvMasterDetail(GridControl gc, GridView gv)
        {
            // gv.OptionsDetail.ShowDetailTabs = false;
            gv.MasterRowCollapsed += (sender, e) =>
            {
                _masterRowExpanded.Remove(e.RowHandle);

                // if (gvPersistentUnit.IsVisible && !gvPersistentUnit.IsEditing)
                //     if (!gvPersistentUnit.IsDetailView &&)
                        gvPersistentUnit.PostEditor();
            };
            gv.MasterRowExpanded += (sender, e) =>
            {
                // GridView dView = gv.GetDetailView(e.RowHandle, (sender as GridView).GetVisibleDetailRelationIndex(e.RowHandle)) as GridView;
                // SRMemento.Instance.InitCustomPersistentEditor(rw, gc, dView, gv);
                _masterRowExpanded.Add(e.RowHandle);
                // SRMemento._editor = dView;
            };
            // gv.DataSourceChanged += (sender, args) =>
            // {
            //     if(_masterRowExpanded.Count > 0)
            //         foreach (var row in _masterRowExpanded)
            //         {
            //             gv.ExpandMasterRow(row);
            //         }
            // };
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

        internal void GvRowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            var colName = e.Column.FieldName == "value";
            if (e.Column.FieldName == "value")
            {
                e.Appearance.BackColor = Color.Aqua;
            }
        }

        // Replaced with in SRStyle
        // When editor is shown, if editable is false, couldnt get edited
        // internal void GvShowingEditor(object sender, CancelEventArgs e)
        // {
        //     GridView view = (GridView) sender;
        //     bool isEditable = (bool) view.GetRowCellValue(view.FocusedRowHandle, "editable");
        //     e.Cancel = !isEditable;
        // }

        // When row validated write to MEM
        internal void GvValidateRow(object sender, ValidateRowEventArgs e)
        {
            GridView view = (GridView) sender;
            if (view.FocusedColumn != view.Columns["value"]) return; 
            view.BeginDataUpdate();
            var rowValue = view.GetRowCellValue(e.RowHandle, "value");
            var rowName = view.GetRowCellValue(e.RowHandle, "name");
            var rowType = view.GetRowCellValue(e.RowHandle, "type");
            e.Valid = FieldTypeValueValidator(rowType.ToString(), rowValue.ToString());
            if(e.Valid) rw.SRWrite(rowName.ToString(), rowValue.ToString());
            view.EndDataUpdate();
        }

        private object _gvIndex = new object();

        protected void UpdateDataGridView(GridView gv, Dictionary<string, Feature> featuresDictionary)
        {
            if (SRGuard.Allowed() is not true) return;
            if (disposing) return;
            
            foreach (var f in featuresDictionary)
            {
                if (f.Value.enabled is not true) continue;

                if (f.Value.name == "UnitMilitaryGoodsNeeded")
                {
                    var idx = Convert.ToInt32("UnitClass".GetFeature("Warfare")?.value);
                    if (WarfareValueDictionary.Instance.DictUnitClassType.Count > 0)
                    {
                        var str = JsonReader
                            .SubCategoriesIndexedStore["DSUBCOUNTRY_BATTALIONSIZE"]
                            .categoryIncludedFeatures[idx].GetFeature().value;
                        var calc = str.As<decimal>().OrDefault() *
                                   Convert.ToDecimal("UnitWeight".GetFeature(SREnum.CategoryName.Warfare)?.value);
                        if (rw.SRIsNaval(idx))
                            calc /= 2;
                        f.Value.value = String.Format("{0:0}", calc);
                    }
                    continue;
                }
                
                if (!f.Value.category.Equals("Warfare"))
                    if (!ListOfSortedRow.SRGridIncludedFeatures.Contains(f.Value.name))
                        if(f.Value.category.Equals("Special")) 
                            continue;

                // Freeze
                if(f.Value.freeze)
                    rw.SRFreeze(f.Value.name, f.Value.value, Settings_TrainerToogleFreezeAllowIncrease.Checked);
                else f.Value.SetWithValueFromMemory();

                // {
                //     if (f.Value.type.Equals("byte")) 
                //         f.Value.value = rw.SRRead(f.Value.name);
                //     else
                //         f.Value.value = rw.SRRead(f.Value.name).ToString();
                //     // f.Value.ReadSafeVoid().ToString();
                //     //             f[i].value = rw.SRRead(f[i].name);
                //     //         else
                //     //             f[i].value = rw.SRRead(f[i].name).ToString();
                // }

            }

            // TODO: Replace with dictionary
            // for (int i = 0; i < f.Count; i++)
            // {
            //     // If isnt enabled dont even readd the address
            //     if(f[i].enabled is not true) continue;
            //     
            //     // USED SO Special Tab that doesnt belong in facility tab doesn't get cosntantly updated.
            //     // if category not "warfare" dont add to gridview
            //     if (!f[i].category.Equals("warfare", StringComparison.InvariantCultureIgnoreCase))
            //     {
            //         // check if FEATURE is in Included Feature list
            //         // And if category "resources" skip loop
            //         // Remember, SRGridIncludedFeature doesn't contains all warfare featured category.
            //         if (!ListOfSortedRow.SRGridIncludedFeatures.Contains(f[i].name))
            //         {
            //             if(f[i].category.Equals("special", StringComparison.InvariantCultureIgnoreCase))
            //                 continue;
            //         }
            //     }
            //
            //     // Warfare alwoys go here
            //     if (f[i].freeze is true)
            //     {
            //         //TODO: Freeze should behave as static value, value shouldnt get mutated when selection change;
            //         // DIRTY HACK
            //         rw.SRFreeze(f[i].name, f[i].value, Settings_TrainerToogleFreezeAllowIncrease.Checked);
            //         // f[i].value = rw.SRRead(f[i].name);
            //     }
            //     else
            //     {
            //         // f[i].value = rw.SRRead(f[i].name);
            //         // TODO : 7/6/2022: Examine this
            //         if (f[i].type.Equals("byte"))
            //             f[i].value = rw.SRRead(f[i].name);
            //         else
            //             f[i].value = rw.SRRead(f[i].name).ToString();
            //         // gv.PostEditor();
            //     }
            // }
        }

        public void RefreshDataGridRowCell(GridView gv)
        {
            // TODO: DPA Issues
            // SET GridId to FeatureList row, so it can be assesed later.
            // TODO: This issues perist, it could makes FormattedRowCell not work. probably at initialization to set gridId
            // if (!allower) return;

            for (int j = 0; j < gv.RowCount; j++)
            {
                if (gv.IsEditorFocused) continue;
                    gv.RefreshRow(j);
                // if(gv.GetVisibleIndex(gv.rowhan))
                // if(gv.refre)
                // gv.RefreshRowCell(j, gv.Columns["freeze"]);
                // gv.RefreshRowCell(j, gv.Columns["value"]);
                // gv.RefreshRowCell(j, gv.Columns["name"]);
            }
            // for (int j = 0; j < gv.DataRowCount; j++)
            // {
            //     // if (gv.GetRowCellValue(j, "gridId") != j.To<object>())
            //     //     gv.SetRowCellValue(j, gv.Columns["gridId"], j);
            //
            //     // gv.RefreshRowCell(j, gv.Columns["value"]);
            //     // gv.RefreshRowCell(j, gv.Columns["original"]);
            //     // gv.RefreshRowCell(j, gv.Columns["freeze"]);
            //     if(!gv.IsEditorFocused) 
            //         gv.RefreshRow(j);
            // }
        }
        
        // Updating Hidden History Grid every click.
        // Timer bound
        // private string _addressGuard = "";
        // private string _needleAddressGuard = "";
        private UIntPtr _needleAddressUIntPtrGuard = UIntPtr.Zero;
        internal void UpdateUnitHistoryList(IList<Feature> currentUnitStats)
        {
            // if (ceDefenseProduction.Checked) return;
            // string needleAddress = null;
            UIntPtr needleAddressUIntPtr = UIntPtr.Zero;
            // Check if GameValidated first
            if (!JsonReader.activeTrainer.GameValidated) return;
            UnitHistoryList.Instance.AddIfNotExists(currentUnitStats);
            
            
            // TODO: Fix this damn pointer, pointed to the current selected unit
            // var unitAddress = rw.GetCode(JsonReader.PointerStore("ArmyCurrentStrength"));
            var unitName = JsonReader.getUnitName(currentUnitStats);
            if (unitName.Equals("????"))
            {
                return;
            }
            var unitHistoryStats = UnitHistoryList.Instance.GetUnitOriginalValueByName(unitName);
            // _unitTracker.AddToObserve(new Unit(unitAddress.ToString(), unitId, unitName, currentUnitStats));
            
            // TODO: this fixs the issue to not overlapping the grid refresher when the unit is not selected.
            var cur = UIntPtr.Zero;
            if (ceDefenseSelection.Checked)
            {
                cur = "ArmyCurrentStrength".GetPointerUIntPtr(rw);
                Events.IsUnitSelected = cur;
            }
            else
            {
                cur = "UnitWeight".GetPointerUIntPtr(rw);
            }
            
            if (!cur.IsValid())
            {
                foreach (var feat in ListOfSortedRow.WarfareIncludedFeatureList)
                {
                    if (feat.Equals("UnitID")) continue;
                    var cache = feat.GetFeatureFromCache(JsonReader.WarfareIndexedFeatures);
                    cache.value = "0";
                    cache.original = "0";
                }
                return;
            }
            
            
            for (int i = 0; i < currentUnitStats.Count; i++)
            {
                if (unitHistoryStats == null)
                {
                    if(String.Empty == unitName)
                        currentUnitStats[i].original = "0";
                    continue;
                }
                var original = unitHistoryStats?[i].value;
                if (!String.IsNullOrEmpty(original)) 
                    if(unitHistoryStats?[i].name == currentUnitStats[i].name)
                        if (original != null)
                            currentUnitStats[i].original = original;
            }
            
            // taking from collection, if the same as the last then return;
            // needleAddress = Memento.GetTrackedUnitPointerCollection(ArmyCurrentStrength.GetPointer(rw));
            needleAddressUIntPtr = Memento.GetTrackedUnitPointerCollection(ArmyCurrentStrength.GetPointerUIntPtr(rw));

            if (needleAddressUIntPtr == _needleAddressUIntPtrGuard)
                return;

            foreach (var pStat in ListOfSortedRow.PersistentUnitIncludedStats)
            {
                Memento.GetTrackedUnitPointerCollection(pStat.GetPointerUIntPtr(rw));
            }
            
            
            // Making unit persistent
            // if (_addressGuard.Equals(needleAddress))
            // {
            //     return;
            // }
            // if (ArmyCurrentStrength.CanWrite(rw))
            // {

            if (!Settings.WarfareTABModeTracking) return;
            if (!TrackedUnitsController.IsUnitTracked(needleAddressUIntPtr))
            {
                if (TrackedUnitsController.MakePersistent(rw, JsonReader.FeaturesWarfare,
                        Settings.WarfareTABPersistantAutoFreeze))
                {
                    gvPersistentUnit.RefreshData();
                    if(_miniWindow != null)
                        _miniWindow.gvPersistentUnit.RefreshData();
                    // _needleAddressGuard = needleAddress;
                    // set so it doesn't return here if the same.
                    _needleAddressUIntPtrGuard = needleAddressUIntPtr;
                    // _addressGuard = needleAddress;
                }
            }
            // }
        }
        internal void MissileMadness()
        {
            if (ceMissiles.Checked && Convert.ToInt32(seMissiles.EditValue) >= 0)
            {
                var intMissile = Convert.ToInt32(seMissiles.EditValue);
                if (intMissile < int.MaxValue)
                {
                    var missileValue = intMissile.ToString();
                    ArmyMissileAvailableCargoQuantity.WriteTo(rw, missileValue);
                    ArmyMissileAvailableStorageQuantity.WriteTo(rw, missileValue);
                    ArmyMissileStrategicPoolAssigned.WriteTo(rw, missileValue);
                    ArmyMissileStrategicPoolReserve.WriteTo(rw, missileValue);
                }
            }
        }

        internal void SpecialOption()
        {
            ADayBuild
                .Use(rw)
                .WithTheValueOf(cbADayBuild.Checked)
                .AndTheValueOf((float)rw.SRRead("ADayBuild", true, null, false) < 1f)
                .WriteThis(null, "1");
            
            ADayArmy.IfTrue(cbADayArmy.Checked).SafelyWriteTo(rw, null);

            ADayResearchClick.IfTrue(cbADayResearch.Checked).SafelyWriteTo(rw, null);
            ADayResearchTooltip.IfTrue(cbADayResearch.Checked).SafelyWriteTo(rw, null);

            SatelliteCommCoverage.IfTrue(ceSatComm.Checked).SafelyWriteTo(rw, null);
            SatelliteMissileDefenseCoverage.IfTrue(ceSatMilDef.Checked).SafelyWriteTo(rw, null);
            SatelliteReconCoverage.IfTrue(ceSatRecon.Checked).SafelyWriteTo(rw, null);
            
            ResearchEfficiency
                .Use(rw)
                .WithInt(rgResearchEfficiency.SelectedIndex)
                .AndIfItsEqualTo(1)
                .WriteThis("1")
                .AndIfItsEqualTo(2)
                .WriteThis("4")
                .AndIfItsEqualTo(3)
                .WriteThis("8")
                .AndIfItsEqualTo(4)
                .WriteThis("100");
        }


        private decimal DailyAnnualFigure(decimal s)
        {
            return !Settings.Default.WarfareTABShowResourceAsDailyAnnually
                ? (s / 365)
                : s;
        }

        private void coloring(ProgressBarControl pb, decimal d)
        {
            var color = Color.Gray;
            if (d.IsBetween(0m, 0.199m))
                color = Color.Lime;
            if (d.IsBetween(0.2m, 0.299m))
                color = Color.LimeGreen;
            if (d.IsBetween(0.3m, 0.49m))
                color = Color.GreenYellow;
            if (d.IsBetween(0.5m, 0.79m))
                color = Color.Gold;
            if (d.IsBetween(0.8m, 0.99m))
                color = Color.Orange;
            if (d >= 1)
                color = Color.Red;

            if (pb.Properties.Appearance.BorderColor != color)
                pb.Properties.Appearance.BorderColor = color;
        }

        private Random _rnd = new Random();

        private void PbUsageDisplay(ProgressBarControl pb, int max, int current) 
        {
            pb.Properties.Maximum = max.IfZero(100);
            pb.Position = current.IfZero(max, _rnd.Next(98,100));
            pb.PerformStep();
        }
        
        // TODO: Just a preview, rewrite this into smaller and cleaner and perf wise code.
        private void WarfareResoureDisplay()
        {
            decimal FormatValue (string value) { return Convert.ToDecimal(value == "" ? "0" : value); }
            decimal.TryParse(JsonReader.feature("StockPetroleum", JsonReader.FeaturesResources).value, 
                out decimal stockPetrol);
            decimal.TryParse(JsonReader.feature("StockMilitaryGoods", JsonReader.FeaturesResources).value, 
                out decimal stockMilitaryGoods);
           
            decimal.TryParse(JsonReader.feature("ArmyFuel", JsonReader.FeaturesWarfare).value, out decimal 
                armyFuel);
            decimal.TryParse(JsonReader.feature("ArmySupply", JsonReader.FeaturesWarfare).value, out decimal 
                armySupply);
            
            decimal.TryParse(JsonReader.feature("DemandPetroleumAnnually", JsonReader.FeaturesResources).value, 
                out decimal actualUsePetroleumAnnually);
            decimal.TryParse(JsonReader.feature("DemandMilitaryGoodsAnnually", JsonReader.FeaturesResources).value, 
                out decimal actualUseMilitaryGoodsAnnually);
           
            decimal.TryParse(JsonReader.feature("ProductionPetroleumAnnually", JsonReader.FeaturesResources).value, 
                out decimal productionPetrolAnnually);
            decimal.TryParse(JsonReader.feature("ProductionMilitaryGoodsAnnually", JsonReader.FeaturesResources).value, 
                out decimal productionMilitaryGoodsAnnually);
            
            // string formattedPetrol = string.Format("{0:N} - Barrels", FormatValue(stockPetrol.ToString()));
            // string formattedMilitaryGoods = string.Format("{0:N} - Tonnes", FormatValue(stockMilitaryGoods.ToString()));
            // var WarfareInformationDisplay = false;
            // string formattedPetrol = stockPetrol.ToSemanticRepresentation();
            // string formattedMilitaryGoods = stockMilitaryGoods.ToSemanticRepresentation();
            // string semanticPetrolDemands = (actualUsePetroleumAnnually).ToSemanticRepresentation();
            // string semanticMGDemands = (actualUseMilitaryGoodsAnnually).ToSemanticRepresentation();

            var formattedProductionPerol = NumericExtension.SafePercentage(
                DailyAnnualFigure(actualUsePetroleumAnnually),
                DailyAnnualFigure(productionPetrolAnnually));
            var formattedMilitaryGoodsAnnually = NumericExtension.SafePercentage(
                DailyAnnualFigure(actualUseMilitaryGoodsAnnually), 
                DailyAnnualFigure(productionMilitaryGoodsAnnually));
            var percentageUseOfPetrol = NumericExtension.SafePercentage(
                DailyAnnualFigure(actualUsePetroleumAnnually), 
                stockPetrol);
            var percentageUseOfMilitaryGoods = NumericExtension.SafePercentage(
                DailyAnnualFigure(actualUseMilitaryGoodsAnnually), 
                stockMilitaryGoods);

            // labelControl32.Text = Settings.WarfareTABShowResourceAsDailyAnnually ? "Annual" : "Daily";
            // labelControl33.Text = Settings.WarfareTABShowResourceAsDailyAnnually ? "Annual" : "Daily";
            
            coloring(pbWarfarePetrolProduction, formattedProductionPerol == 0 ? 1 : formattedProductionPerol);
            coloring(pbWarfarePetrolStock, percentageUseOfPetrol == 0 ? 100 : percentageUseOfPetrol);
            coloring(pbWarfareMGProduction, formattedMilitaryGoodsAnnually == 0 ? 1 : formattedMilitaryGoodsAnnually);
            coloring(pbWarfareMGStock, percentageUseOfMilitaryGoods == 0 ? 1 : percentageUseOfMilitaryGoods);
            
            var petrolProduction =
                $"{DailyAnnualFigure(actualUsePetroleumAnnually).ToSemanticRepresentation()}/{DailyAnnualFigure(productionPetrolAnnually).ToSemanticRepresentation()}";
            var petrolStock =
                $"{DailyAnnualFigure(actualUsePetroleumAnnually).ToSemanticRepresentation()}/{stockPetrol.ToSemanticRepresentation()}";
            var mgProduction =
                $"{DailyAnnualFigure(actualUseMilitaryGoodsAnnually).ToSemanticRepresentation()}/{DailyAnnualFigure(productionMilitaryGoodsAnnually).ToSemanticRepresentation()}";
            var mgStock =
                $"{DailyAnnualFigure(actualUseMilitaryGoodsAnnually).ToSemanticRepresentation()}/{stockMilitaryGoods.ToSemanticRepresentation()}";
            
                var stockMax = ((int)(stockPetrol / 100));
                var stockCurrent = ((int)DailyAnnualFigure(actualUsePetroleumAnnually / 100));
                PbUsageDisplay(pbWarfarePetrolStock,stockMax, stockCurrent);

                var productionMax = ((int)DailyAnnualFigure(productionPetrolAnnually / 100));
                var productionCurrent = ((int)DailyAnnualFigure(actualUsePetroleumAnnually / 100));
                PbUsageDisplay(pbWarfarePetrolProduction, productionMax, productionCurrent);
                
                var MGProductionMax = ((int)DailyAnnualFigure(productionMilitaryGoodsAnnually / 100));
                var MGProdcutionCurrent = ((int)DailyAnnualFigure(actualUseMilitaryGoodsAnnually / 100));
                PbUsageDisplay(pbWarfareMGProduction,  MGProductionMax, MGProdcutionCurrent);

                var MGStockMax = ((int)(stockMilitaryGoods / 100));
                var MGStockCurrent = ((int)DailyAnnualFigure(actualUseMilitaryGoodsAnnually / 100));
                PbUsageDisplay(pbWarfareMGStock,MGStockMax, MGStockCurrent);

                pbWarfarePetrolProduction.Tag = !Settings.WarfareTABShowResourceAsPercentage ? petrolProduction : formattedProductionPerol.ToString("P2");
                pbWarfarePetrolStock.Tag = !Settings.WarfareTABShowResourceAsPercentage ? petrolStock : percentageUseOfPetrol.ToString("P2");
                pbWarfareMGProduction.Tag = !Settings.WarfareTABShowResourceAsPercentage ? mgProduction : formattedMilitaryGoodsAnnually.ToString("P2");
                pbWarfareMGStock.Tag = !Settings.WarfareTABShowResourceAsPercentage ? mgStock : percentageUseOfMilitaryGoods.ToString("P2");
        }
        

        public void BuildModeOverride(LookUpEdit ctrl, bool allower)
        {
            if (!allower) return;
                if(ctrl.ItemIndex != -1 )
                {
                    "BuildModeFacilityID".GetFeature()?.WriteTo(rw, ctrl.EditValue.ToString());
                    "BuildModeFacilityIDOverride".GetFeature()?.WriteTo(rw, ctrl.EditValue.ToString());
                }

            ctrl.ItemIndex 
                = leBuildModeOverride
                    .Properties
                    .GetDataSourceRowIndex(
                        "Value", 
                        "BuildModeFacilityIDOverride".GetFeature().Read(rw).ToString());
        }

        public void UnitModelSizeReader()
        {
            //Debug> labelControl28.Text = zoomTrackBarUnitModelSize.EditValue.ToString();
            if (zoomTrackBarUnitModelSize.IsEditorActive) return;
            decimal realVal = 1;

            if (decimal.TryParse("UnitModelSize".GetFeature().Read(rw), out realVal))
            {
                if (realVal > 0)
                {
                    zoomTrackBarUnitModelSize.Value = (int)(realVal * 100);
                }
            }
        }

        public void UnitModelSizeWriter()
        {
            zoomTrackBarUnitModelSize.Properties.Maximum = 200;
            zoomTrackBarUnitModelSize.Properties.Middle = 100;
            zoomTrackBarUnitModelSize.Properties.Minimum = 1;

            // RGA
            zoomTrackBarUnitModelSize.ValueChanged += (object sender, EventArgs e) =>
            {
                var zbar = sender as ZoomTrackBarControl;
                var vl = (decimal)zbar.Value / 100;
                "UnitModelSize".GetFeature().WriteTo(rw, vl.ToString(CultureInfo.InvariantCulture));
            };
        }

        // Added protection for multiplayer use to fail
        public void MultiplayerProtection()
        {
            if (rw.SRRead("MultiplayerReadyState", true, JsonReader.FeaturesSpecial) != 1) return;
            mainTimer.Enabled = false;
            XtraMessageBox.Show("No Multiplayer! sorry :)");
            Application.Exit();
        }

        private void ShowExperienceStar(Feature armyexp)
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
        
        internal void Hover(Action a)
        {
            if (ceModeHover.Checked && JsonReader.FeatureArmyEnemyEnabled)
                a();
        }

        internal void ControlSpinEditSafeRead(SpinEdit se)
        {
            if (se is { IsEditorActive: false, Tag: { } })
                se.EditValue = se.Tag.ToString().GetFeature().Read(rw);
        }

        // private int _techEffectWriterCounter = 0;
        // internal void TechEffectWriter()
        // {
        //     if(!leTechEffect1.ItemIndex.Equals(-1))
        //         "TechEffectIndustrializationEffect1".GetFeature().WriteTo(rw, leTechEffect1.EditValue.ToString());
        //     if(!leTechEffect2.ItemIndex.Equals(-1)) 
        //         "TechEffectIndustrializationEffect2".GetFeature().WriteTo(rw, leTechEffect2.EditValue.ToString());
        //     _techEffectWriterCounter++;
        // }

        internal void TechEffectReader()
        {
            ControlSpinEditSafeRead(seTechEffectModifier1);
            ControlSpinEditSafeRead(seTechEffectModifier2);
        }

        #region Facility Reader/Writer [Deprecated]
        internal void FacilitiesReader()
        {
            // Facilities
            lblFacID.Text = rw.SRRead("FacilityID");
            lblFacName.Text = rw.SRRead("FacilityName");
            // ControlSpinEditSafeRead(seFacBuildTime);
            // ControlSpinEditSafeRead(seFacCost);
            // ControlSpinEditSafeRead(seFacIGNeeded);
            // ControlSpinEditSafeRead(seFacMILGNeeded);
            // ControlSpinEditSafeRead(seFacSupplyLevel);
            // ControlSpinEditSafeRead(seFacDefenseAir);
            // ControlSpinEditSafeRead(seFacDefenseGround);
            // ControlSpinEditSafeRead(seFacDefenseIndirect);
            // ControlSpinEditSafeRead(seFacDefenseClose);
            // ControlSpinEditSafeRead(seFacStaff);
            // ControlSpinEditSafeRead(seGarrisonEfficiency);
        }
        internal void FacilitiesWriter()
        {
           if(ceGarrisonInstant.Checked)
               if(BuildingGarrisonActive.CanWrite(rw)) 
                   gvFacility.gvSetRowCellValue("value", "BuildingGarrisonActive", "8", true);
                   // BuildingGarrisonActive.WriteTo(rw, "8");
           if(ceGarrisonEfficiency.Checked)
               if(BuildingGarrisonEfficiency.CanWrite(rw)) 
                   gvFacility.gvSetRowCellValue("value", "BuildingGarrisonEfficiency", seGarrisonEfficiency.EditValue.ToString(), true);
                   // BuildingGarrisonEfficiency.WriteTo(rw, seGarrisonEfficiency.EditValue);
        }
        #endregion
        
        #region Army Reader/Writer
        internal void ArmyControlWriter()
        {
            // Check If Unit is in persistent/tracked mode
            if (TrackedUnitsController.IsUnitTracked("ArmyCurrentStrength".GetPointerUIntPtr(rw))) 
                if (TrackedUnitsController.IsUnitTracked("HoverArmyCurrentStrength".GetPointer(rw))) 
                    return;
            
            ArmyEfficiency.freeze = ceEfficiency.Checked;
            ArmyExperience.freeze = ceExperience.Checked;
            ArmyMorale.freeze = ceMorale.Checked;
            ArmySupply.freeze = ceUnitSupplies.Checked;
            ArmyCurrentStrength.freeze = ceUnitStrength.Checked;
            ArmyFuel.freeze = ceUnitFuel.Checked;
            
            HoverArmyEfficiency.freeze = ceEfficiency.Checked && ceModeHover.Checked;
            HoverArmyExperience.freeze = ceExperience.Checked && ceModeHover.Checked;
            HoverArmyMorale.freeze = ceMorale.Checked && ceModeHover.Checked;
            HoverArmySupply.freeze = ceUnitSupplies.Checked && ceModeHover.Checked;
            HoverArmyCurrentStrength.freeze = ceUnitStrength.Checked && ceModeHover.Checked;
            HoverArmyFuel.freeze = ceUnitFuel.Checked && ceModeHover.Checked;

            // If Morale Checkbox is checked, set Morale to unit
            if (ceMorale.Checked)
            {
                gvWarfare.gvSetRowCellValue("value", "ArmyMorale", "5", true);
                Hover(() => gvWarfare.gvSetRowCellValue("value", HoverArmyMorale.name, "5", true));
                // ArmyMorale.WriteTo(rw, "5");
                // Hover(() => HoverArmyMorale.WriteTo(rw, "5"));
            }
            
            if (ceUnitFuel.Checked)
            {
                gvWarfare.gvSetRowCellValue("value", seUnitFuel.Tag.ToString(), seUnitFuel.EditValue.ToString(), true);
                Hover(() => gvWarfare.gvSetRowCellValue("value",HoverArmyFuel.name, seUnitFuel.EditValue.ToString(), true));
                // ArmyFuel.WriteTo(rw, seUnitFuel.EditValue.ToString());
                // Hover(() => HoverArmyFuel.WriteTo(rw, seUnitFuel.EditValue.ToString()));
            }
            
            if (ceUnitStrength.Checked)
            {
                
                gvWarfare.gvSetRowCellValue("value", seUnitStrCurrent.Tag.ToString(), seUnitStrCurrent.EditValue.ToString(), true);
                Hover(() => gvWarfare.gvSetRowCellValue("value", HoverArmyCurrentStrength.name, seUnitStrCurrent.EditValue.ToString(), true));
                // HoverArmyCurrentStrength.WriteTo(rw, seUnitStrCurrent.EditValue.ToString());
                // Hover(() => HoverArmyCurrentStrength.WriteTo(rw, seUnitStrCurrent.EditValue.ToString()));
            }
            
            if (ceUnitSupplies.Checked)
            {
                gvWarfare.gvSetRowCellValue("value", seUnitSupplies.Tag.ToString(), seUnitSupplies.EditValue.ToString(), true);
                Hover(() => gvWarfare.gvSetRowCellValue("value", HoverArmySupply.name, seUnitSupplies.EditValue.ToString(), true));
                // ArmySupply.WriteTo(rw, seUnitSupplies.EditValue.ToString());
                // Hover(() => HoverArmySupply.WriteTo(rw, seUnitSupplies.EditValue.ToString()));
                
            }

            // rw.SRWrite("ArmyMorale", "10.0");
            if (ceExperience.Checked)
            {
                var rate = rtExperience.Rating.ConvertRatingForEdit().ToString(CultureInfo.InvariantCulture);
                gvWarfare.gvSetRowCellValue("value", ArmyExperience.name, rate, true);
                // ArmyExperience.WriteTo(rw, rate);
                
                Hover(() => gvWarfare.gvSetRowCellValue("value", HoverArmyExperience.name, rate, true));
                // Hover(() => HoverArmyExperience.WriteTo(rw, rate));
            }

            // rw.SRWrite("ArmyExperience", "10.0");
            if (ceEfficiency.Checked)
            {
                gvWarfare.gvSetRowCellValue("value", ArmyEfficiency.name, "5", true);
                // ArmyEfficiency.WriteTo(rw, "5");
                
                Hover(() => gvWarfare.gvSetRowCellValue("value", HoverArmyEfficiency.name, "5", true));
                // Hover(() => HoverArmyEfficiency.WriteTo(rw, "5"));
            }

            if (ceEntrenchment.Checked)
            {
                gvWarfare.gvSetRowCellValue("value", ArmyEntrenchment.name, "5", true);
                // ArmyEntrenchment.WriteTo(rw, "5");
                
                Hover(() => gvWarfare.gvSetRowCellValue("value", HoverArmyEntrenchment.name, "5", true));
                // Hover(() => HoverArmyEntrenchment.WriteTo(rw, "5"));
            }

            // rw.SRWrite("ArmyEfficiency", "10.0");

            bool IsChecked(HandmadeEnums e)
            {
                string s = Enum.GetName(typeof(HandmadeEnums), e);
                int index = checkedListBoxControl1.FindItem(0, true, delegate(ListBoxFindItemArgs ee) {
                    ee.IsFound = s.Equals(ee.DisplayText);
                });
                // TODO: something is wrong with this bitch
                return HandmadeFeatures.Instance.Lists[index].freeze;
                // return checkedListBoxControl1.GetItemCheckState(index) == CheckState.Checked;
                // return false;
                // return checkedListBoxControl1.Items[e].CheckState == CheckState.Checked;
            }

            // Init For special 
            _actualStrength = ArmyActualStrength.value.StrToDouble();
            _actualGas = (ArmyCurrentStrength.value.StrToDouble() * UnitFuelCapacity.value.StrToDouble());
            _actualSupply = (ArmyCurrentStrength.value.StrToDouble() * UnitSuppliesCapacity.value.StrToDouble());

            _hoverActualStrength = HoverArmyActualStrength.value.StrToDouble();
            _hoverActualGas = (HoverArmyCurrentStrength.value.StrToDouble() * HoverUnitFuelCap.value.StrToDouble());
            _hoverActualSupply = (HoverArmyCurrentStrength.value.StrToDouble() * HoverUnitSupplyCap.value.StrToDouble());
            
            // Multiplier
            _supplyGasMultiplier = 1;
            _strMultiplier = 1;


            // Heal
            if (IsChecked(HandmadeEnums.Heal))
            {
                if (ArmyCurrentStrength.value.StrToDouble() < ArmyActualStrength.value.StrToDouble())
                    rw.SRWrite(ArmyCurrentStrength.name, (ArmyActualStrength.value.StrToDouble()).ToString());
                if (ArmyFuel.value.StrToDouble() < _actualGas)
                    rw.SRWrite(ArmyFuel.name, _actualGas.ToString());
                if (ArmySupply.value.StrToDouble() < _actualSupply)
                    rw.SRWrite(ArmySupply.name, _actualSupply.ToString());

                Hover(() =>
                {
                    if (HoverArmyCurrentStrength.value.StrToDouble() < HoverArmyActualStrength.value.StrToDouble())
                        HoverArmyCurrentStrength.WriteTo(rw, HoverArmyActualStrength.value);
                    if (HoverArmyFuel.value.StrToDouble() < _actualGas)
                        HoverArmyFuel.WriteTo(rw, HoverArmyActualStrength.value);
                    if (HoverArmySupply.value.StrToDouble() < _actualSupply)
                        HoverArmySupply.WriteTo(rw, HoverArmyActualStrength.value);
                });
            }

            // Rambo
            if (IsChecked(HandmadeEnums.Rambo))
            {
                ceEfficiency.Checked = true;
                ceExperience.Checked = true;
                ceMorale.Checked = true;
                if(!ceEfficiency.Checked && !ceExperience.Checked && !ceMorale.Checked)
                    HandmadeFeature.SetHandmadeState(HandmadeEnums.Rambo, false);
                // if (!IsChecked(HandmadeEnums.Rambo))
                // {
                //     ceEfficiency.Checked = false;     
                //     ceExperience.Checked = false;
                //     ceMorale.Checked = false;
                // }
            }

            // 2x Str
            if (IsChecked(HandmadeEnums.Str2x))
            {
                _strMultiplier = _strMultiplier + 2;
                rw.SRWrite(ArmyCurrentStrength.name, (_actualStrength * _strMultiplier).ToString("N"));
                Hover(() =>
                {
                    var r = HoverArmyActualStrength.Read();
                    if(r != null)
                    HoverArmyCurrentStrength.SafeWrite(rw, (r.value.StrToDouble() * _strMultiplier).ToString("N"));
                });
            }

            // 4x Str
            if (IsChecked(HandmadeEnums.Str4x))
            {
                _strMultiplier = _strMultiplier + 4;
                rw.SRWrite(ArmyCurrentStrength.name, (_actualStrength * _strMultiplier).ToString("N"));
                Hover(() =>
                    {
                        var r = HoverArmyActualStrength.Read();
                        if(r != null)
                            HoverArmyCurrentStrength.SafeWrite(rw, (r.value.StrToDouble() * _strMultiplier).ToString("N"));
                    });
            }

            // 2x SupplyGas
            if (IsChecked(HandmadeEnums.GasSupply2x))
            {
                _supplyGasMultiplier = _supplyGasMultiplier + 2;
                rw.SRWrite(ArmyFuel.name, (_actualGas * _supplyGasMultiplier).ToString("N"));
                rw.SRWrite(ArmySupply.name, (_actualSupply * _supplyGasMultiplier).ToString("N"));
                
                Hover(() =>
                {
                    if (_hoverActualGas > 0) 
                        rw.SRWrite(HoverArmyFuel.name, (_hoverActualGas * _supplyGasMultiplier).ToString("N"));
                    if (_hoverActualSupply > 0) 
                        rw.SRWrite(HoverArmySupply.name, (_hoverActualSupply * _supplyGasMultiplier).ToString("N"));
                });
            }

            // 4x SupplyGas
            if (IsChecked(HandmadeEnums.GasSupply4x))
            {
                
                _supplyGasMultiplier = _supplyGasMultiplier + 4;
                rw.SRWrite(ArmyFuel.name, (_actualGas * _supplyGasMultiplier).ToString("N"));
                rw.SRWrite(ArmySupply.name, (_actualSupply * _supplyGasMultiplier).ToString("N"));
                
                Hover(() =>
                {
                    if (_hoverActualGas > 0) 
                        rw.SRWrite(HoverArmyFuel.name, (_hoverActualGas * _supplyGasMultiplier).ToString("N"));
                    if (_hoverActualSupply > 0) 
                        rw.SRWrite(HoverArmySupply.name, (_hoverActualSupply * _supplyGasMultiplier).ToString("N"));
                });
            }
        }

        internal void ArmyActiveStaffWrapped()
        {
            // Active Staff
            seStaffActive.EditValue = ArmyActiveStaff.value.ValueFilter();
            if (!seStaffReserve.IsEditorActive)
                seStaffReserve.EditValue = ArmyReserve.value.ValueFilter();
            
            // seUnitStrCurrent.Properties.IsFloatValue = true;
            // seUnitStrCurrent.Properties.Increment = IsNaval ? 0.01m : 1m;

            if (!seUnitStrCurrent.IsEditorActive && !ceUnitStrength.Checked)
                seUnitStrCurrent.EditValue = Events.IsNaval ? ArmyCurrentStrength.value : ArmyCurrentStrength.value.ConvertToInt64().ToString();
            if (!seUnitStrActual.IsEditorActive)
                seUnitStrActual.EditValue = Events.IsNaval ? ArmyActualStrength.value : ArmyActualStrength.value.ConvertToInt64().ToString();
        }
        internal void ArmyFuelEditorWrapped()
        {
            // Fuel Editor
            if (!seUnitFuel.IsEditorActive && !ceUnitFuel.Checked)
                seUnitFuel.EditValue = ArmyFuel.value.ConvertToInt64().ToString();;
            
            if (ArmyCurrentStrength.value != string.Empty && UnitFuelCapacity.value != string.Empty)
                seUnitActualFuel.EditValue = 
                    (decimal.Parse(ArmyCurrentStrength.value) * decimal.Parse(UnitFuelCapacity.value)).ToString("0");
            
        }
        internal void ArmySuppliesEditorWrapped()
        {
            // Supplies Editor
            if (!seUnitSupplies.IsEditorActive && !ceUnitSupplies.Checked)
                seUnitSupplies.EditValue = ArmySupply.value.ConvertToInt64().ToString();;
            
            if (ArmyCurrentStrength.value != string.Empty && UnitSuppliesCapacity.value != string.Empty)
                seUnitActualSupplies.EditValue =
                    (decimal.Parse(ArmyCurrentStrength.value) * decimal.Parse(UnitSuppliesCapacity.value)).ToString("0");
        }
        public void ArmyControlReader()
        {
            ArmyActiveStaffWrapped();
            ArmyFuelEditorWrapped();
            ArmySuppliesEditorWrapped();

            // Health Bars
            ShowHealthBar();

            // Morale Bars
            if(ArmyMorale.value.Length <  5)
                ShowBar(pbArmyMoraleBar, ArmyMorale, 100);

            // Efficiency Bar
            if(ArmyEfficiency.value.Length <  5)
                ShowBar(pbArmyEfficiencyBar, ArmyEfficiency, 120);
            
            // Entrenchment Bar
            if(ArmyEntrenchment.value.Length <  5) 
                ShowBar(pbArmyEntrenchmentBar, ArmyEntrenchment, 120);

            // Experience Stars
            ShowExperienceStar(ArmyEfficiency);
        }
        #endregion
        internal void ShowBar(ProgressBarControl pbc, Feature source, int maxValue)
        {
            double inputParsed;
            if (!double.TryParse(source.value, out inputParsed)) return;
            if ((inputParsed * 100) >= int.MaxValue) return;
            int realVal = (int) (inputParsed * 100);
            if (realVal >= maxValue) realVal = maxValue;
            pbc.Position = realVal;
            pbc.Properties.Maximum = maxValue;
        }

        // Bar Override Deactivate
        protected void FormDeactivate(object sender, EventArgs e)
        {
            if (bar2.IsFloating) //your condition  
            {
                ISupportWindowActivate act = bar2;
                if (act != null) act.Activate();
            }
        }

        // Debug Purpose
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            gcUnitHistoryList.DataSource = UnitHistoryList.Instance.UnitList;
        }

        internal void SetSpinEditorEvent(SpinEdit se)
        {
            // se.Properties.ValidateOnEnterKey = true;
            // se.Properties
            // se.Properties.IsFloatValue = true;
            se.Validating += SpinValidatingEvent;
            se.MouseEnter += (sender, args) => { };
            se.LostFocus += (sender, args) => { };
            se.Spin += SetSpinEvent;
        }

        // Spin Editor Validating Event, Reading from SPIN.TAG to check FeatureName, 
        private void SpinValidatingEvent(object sender, CancelEventArgs e)
        {
            SpinEdit se = (SpinEdit) sender;
            if (rw.SRWrite(se.Tag.ToString(), se.EditValue.ToString()))
                // play sound from resources
                PlaySound(Properties.Resources.AudioClick);
        }

        // Set Spin Event, basically the same as above.
        private void SetSpinEvent(object sender, SpinEventArgs e)
        {
            var se = (SpinEdit) sender;
            // Debug.WriteLine($"SpinEvent: before swWrite, {se.Text}");
            if (string.IsNullOrEmpty(se.Tag.ToString())) return;
            // se.Properties.BeginUpdate();
            rw.SRWrite(se.Tag.ToString(), se.EditValue.ToString());
            // se.Properties.EndUpdate();
        }

        private void SeModifiedEvent(object sender, EventArgs e)
        {
            SpinEdit se = (SpinEdit) sender;
            rw.SRWrite(se.Tag.ToString(), se.EditValue.ToString());
            //if (se.Tag.ToString)
            //}
        }

        // Freeze Feature in tags if its checked.
        private void CeCheckStateChangedEvent(object sender, EventArgs e)
        {
            var ce = sender as CheckEdit;

            var tags = (List<Feature>)ce.Tag;
            for (var i = 0; i < tags.Count; i++)
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
            gvPersistentUnit.BestFitColumns();
            gvModifiedUnit.BestFitColumns();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void xtraTabControl1_Selected(object sender, DevExpress.XtraTab.TabPageEventArgs e)
        {
            gvCountry.BestFitColumns();
            // gvCountry.OptionsView.ColumnAutoWidth = false;
            // gvCountry.Columns["value"].OptionsColumn.FixedWidth = true;

            gvWarfare.BestFitColumns();
            // gvWarfare.Columns["displayName"].OptionsColumn.FixedWidth = true;
            // gvWarfare.Columns["original"].OptionsColumn.FixedWidth = true;
            // gvWarfare.Columns["value"].OptionsColumn.FixedWidth = true;

            gvResources.BestFitColumns();
            // gvResources.Columns["value"].OptionsColumn.FixedWidth = true;
        }

        private void barEditItem3_EditValueChanged(object sender, EventArgs e)
        {
            // TrackBarControl trackBar = sender as TrackBarControl;
            if (Settings_FormSetOpacity.EditValue != null)
            {
                var val = Convert.ToInt32(Settings_FormSetOpacity.EditValue);
                this.Opacity = (double) val / 100;
            }
        }

        private void SETTINGS_INIT_ALWAYSONTOP()
        {
            TopMost = Settings.UserAlwaysOnTop;
        }

        private void barTglOnTop_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TopMost = Settings_FormToogleOnTop.Checked;
            Settings.UserAlwaysOnTop = Settings_FormToogleOnTop.Checked;
        }

        // TODO: 3.0.0.0 temporarily disabled for deployment purpose
        // private void skinDropDownButtonItem1_DownChanged(object sender, ItemClickEventArgs e)
        // {
        //     SkinDropDownButtonItem skin = sender as SkinDropDownButtonItem;
        //     Properties.Settings.Default.UserThemes = LookAndFeel.ActiveSkinName;
        // }

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
            mainTimer.Enabled = false;
            Application.ExitThread();
            Environment.Exit(0);
        }


        private void _miniWindow_OnShown(object o, EventArgs args)
        {
            barMiniWindowBtn.Caption = $@"Exit In-game Mini Editor";
        }
        private void _miniWindow_Disposed(object o, EventArgs args)
        {
            barMiniWindowBtn.Caption = $@"Open In-game Mini Editor";
        }
        
        private void _miniWindow_FormClosing(object o, FormClosingEventArgs args)
        {
            // SRMiniWindow s = o as SRMiniWindow ?? throw new InvalidOperationException();
            var ctrls = new List<Control>(Enumerable.Cast<Control>(_miniWindow.Controls));
            foreach (var c in ctrls)
            {
                c.Dispose();
            }
            // s.gvPersistentUnit.Dispose();
            // s.gvFacility.Dispose();
            // s.gvPersistentUnitStats.Dispose();
            // s.gvCountry.Dispose();
            _miniWindow.Controls.Clear();
            WindowTracker.Unhook();
            _miniWindow.MiniWindows_Dispose();
            _miniWindow.Shown -= _miniWindow_OnShown;
            _miniWindow.Disposed -= _miniWindow_Disposed;
            _miniWindow.FormClosing -= _miniWindow_FormClosing;
            GC.SuppressFinalize(_miniWindow);
            _miniWindow = null;
        }
        #region Mini Editor
        XtraForm SpawnMiniEditor()
        {
            if(Loader.Selected.GameProcess == null)
            {
                XtraMessageBox.Show("Please start the game first.");
                return null;
            }
            if (_miniWindow != null)
            {
                _miniWindow.Close();
                _miniWindow = null;
            }
            _miniWindow = new SRMiniWindow(this);
            // _miniWindow.Shown -= _miniWindow_OnShown;
            _miniWindow.Shown += _miniWindow_OnShown;
            // _miniWindow.Disposed -= _miniWindow_Disposed;
            _miniWindow.Disposed += _miniWindow_Disposed;
            // _miniWindow.FormClosing -= _miniWindow_FormClosing;
            _miniWindow.FormClosing += _miniWindow_FormClosing;
            return _miniWindow;
        }

        private void barMiniWindowBtn_ItemClick(object sender, ItemClickEventArgs e)
        {
            // if MiniWindow is null, HookChildWindow
            // if MiniWindow is not null, UnhookChildWindow
            if (IsFullScreen)
            {
                XtraMessageBox.Show("Please start the game in windowed mode.");
                return;
            };
            
            if (_miniWindow == null)
            {
                WindowTracker.HookChildWindow(Loader.Selected.GameProcess, this, SpawnMiniEditor());
            }
            else
            {
                WindowTracker.Unhook();
                _miniWindow.Close();
                _miniWindow = null;
            }
            
            // if (!IsFullScreen)
            // {
            //     WindowTracker.HookChildWindow(Loader.Selected.GameProcess, this, SpawnMiniEditor());
            // }
            // else
            // {
            //     WindowTracker.Unhook();
            //     _MiniWindow.Dispose();
            // }
        }

        #endregion
        private void seSocials_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void AdsDisposer<T>(ref T ad) where T : UserControl
        {
            if (ad == null) return;
            if (!ad.IsDisposed)
            {
                ad.Visible = false;
                ad.Dispose();
                GC.SuppressFinalize(this);
                System.GC.Collect(0, GCCollectionMode.Forced);
                System.GC.WaitForFullGCComplete();
            }
        }
        
        private void AdsDisposer<T>(ref T ad, GridControl? gc) where T : UserControl
        {
            
            if (!ad.IsDisposed)
            {
                if(gc != null)
                    gc.Dock = DockStyle.Fill;
                ad.Visible = false;
                if(this.Controls.ContainsKey(ad.Name)) 
                    this.Controls.Remove(ad);
                ad.Dispose();
            }
        }
        
        private void Dispose()
        {
            disposing = true;
            
            AdsDisposer(ref bannerAdsCountryTab);
            AdsDisposer(ref bannerAdsSpecialTab);
            AdsDisposer(ref bannerAdsProfileBox1);
            AdsDisposer(ref bannerAdsProfileBox2);
            AdsDisposer(ref bannerAdsResourceTab);
            AdsDisposer(ref bannerAdsProfileBoxBottom);
            AdsDisposer(ref bannerAdsTrackedUnits);
            AdsDisposer(ref bannerAdsWarfareUnitEditorTab);
            AdsDisposer(ref interstitialAdFullScreen);
            AdsDisposer(ref interstitialAdAllScreen);
            
            bannerAdsCountryTab = null;
            bannerAdsSpecialTab = null;
            bannerAdsProfileBox1 = null;
            bannerAdsProfileBox2 = null;
            bannerAdsResourceTab = null;
            bannerAdsProfileBoxBottom = null;
            bannerAdsTrackedUnits = null;
            bannerAdsWarfareUnitEditorTab = null;
            interstitialAdFullScreen = null;
            interstitialAdAllScreen = null;
            
            if(adsTimer != null) 
                adsTimer.Dispose();

            checkedListBoxControl1.Dispose();
            gcWarfare.Dispose();
            gcFacility.Dispose();
            gcCountry.Dispose();
            gcPersistentUnit.Dispose();
            gcResources.Dispose();
            JsonReader.Dispose();
            base.Dispose();
        }

        protected internal void GroupedControlToFreezeList(string valueContainSpinEdit, bool conditional, string[] featureNameList)
        {
            var stringValue = valueContainSpinEdit;
            if (conditional)
            {
                for (int i = 0; i < featureNameList.Length; i++)
                {
                    featureNameList[i].GetFeature().value = stringValue;
                    featureNameList[i].GetFeature().freeze = true;
                }
            }
            else
            {
                for (int i = 0; i < featureNameList.Length; i++)
                {
                    featureNameList[i].GetFeature().freeze = false;
                }
            }
        }

        protected internal void GroupedControlToFreezeList(bool conditional, string[] featureNameList)
        {
            for (int i = 0; i < featureNameList.Length; i++)
            {
                featureNameList[i].GetFeature().freeze = conditional;
            }
        }
        
        protected internal void FreezeByDisplayName(Dictionary<string, Feature> collection, string needle, bool conditional)
        {
            foreach (var feature in collection)
            {
                if (!feature.Value.displayName.Equals(needle)) continue; 
                feature.Value.freeze = conditional;
            }
        }

        // TODO: Revise to reflect metadata abstraction
        private void cbtnSocials_CheckedChanged(object sender, EventArgs e)
        {
            GroupedControlToFreezeList(seSocials.EditValue.ToString(), 
                cbtnSocials.Checked,
                JsonReader.DataCountry.GetSubCategoryIncludedFeaturesById(1));
        }

        private void barCbShowDescription_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            BarCheckItem self = sender as BarCheckItem;
            gvCountry.OptionsView.ShowPreview = self.Checked;
            gvResources.OptionsView.ShowPreview = self.Checked;
            gvWarfare.OptionsView.ShowPreview = self.Checked;
            gvCountry.BestFitColumns(true);
            gvResources.BestFitColumns(true);
            gvWarfare.BestFitColumns(true);
        }

        private void toolbarFormControl1_Click(object sender, EventArgs e)
        {

        }

        private void btnResourcesFreezeAll_Click(object sender, EventArgs e)
        {
            FreezeByDisplayName(JsonReader.ResourcesIndexedFeatures, "Stock", true);
        }

        private void btnResourcesUnfreeze_Click(object sender, EventArgs e)
        {
            
            FreezeByDisplayName(JsonReader.ResourcesIndexedFeatures, "Stock", false);
        }

        #region RailRoad Build
        
         public void RailRoadBridgeCapturer()
         {
             if (!Settings.SpecialTABRailRoadDaysToBuild)
             {
                 return;
             }
            // if (StaticRoadPointer != null && StaticRailPointer != null) return;
            string id = "HoverFacilityID".GetFeature(JsonReader.FeaturesSpecial).Read(rw).ToString();
            var hoverFacilityBuildTimePointer = JsonReader.PointerStore("HoverFacilityBuildTime");
            var hoverFacilityCost = JsonReader.PointerStore("HoverFacilityCost");
            var hoverFacilityIGNeeded = JsonReader.PointerStore("HoverFacilityIGNeeded");
            switch (id)
            {
                case "21903": // Get Road Pointer and Value
                    StaticRoadPointer = rw.GetCode(hoverFacilityBuildTimePointer).ToUInt32().ToString("X");
                    if(StaticRoadOriginalValue == null)
                        StaticRoadOriginalValue = rw.ReadFloat(StaticRoadPointer, "", false).ToString("N"); // !w.ReadFloat(hoverFacilityBuildTimePointer).ToString();
                    
                    StaticRoadCostPointer = rw.GetCode(hoverFacilityCost).ToUInt32().ToString("X");
                    if(StaticRoadOriginalCostValue == null) 
                        StaticRoadOriginalCostValue = rw.ReadFloat(StaticRoadCostPointer, "", false).ToString("N");
                    
                    StaticRoadIGNeededPointer = rw.GetCode(hoverFacilityIGNeeded).ToUInt32().ToString("X");
                    if(StaticRoadIGNeededOriginalValue == null) 
                        StaticRoadIGNeededOriginalValue = rw.ReadFloat(StaticRoadIGNeededPointer, "", false).ToString("N");
                    break;
                case "21904": // Get Rail Pointer and Value
                    StaticRailPointer = rw.GetCode(hoverFacilityBuildTimePointer).ToUInt32().ToString("X");
                    if(StaticRailOriginalValue == null)
                        StaticRailOriginalValue = rw.ReadFloat(StaticRailPointer, "", false).ToString("N");; //!rw.ReadFloat(StaticRailPointer).ToString();
                    
                    StaticRailCostPointer = rw.GetCode(hoverFacilityCost).ToUInt32().ToString("X");
                    if(StaticRailOriginalCostValue == null)
                        StaticRailOriginalCostValue = rw.ReadFloat(StaticRailCostPointer, "", false).ToString("N");
                    
                    StaticRailIGNeededPointer = rw.GetCode(hoverFacilityIGNeeded).ToUInt32().ToString("X");
                    if(StaticRailIGNeededOriginalValue == null) 
                        StaticRailIGNeededOriginalValue = rw.ReadFloat(StaticRailIGNeededPointer).ToString("N");
                    break;
            }
        }

        private bool WriteToMemoryAndPlaySound(string? pointer, string ifTrue,  string ifFalse)
        {
            if (ifFalse == null) throw new ArgumentNullException(nameof(ifFalse));
            
            // Current value of rail/road
            var rFloat = rw.ReadFloat(pointer, "", false).ToString("N");
            
            // If current value is the same (means activated)
            if (rFloat.Equals(ifFalse)) {
                rw.WriteMemory(pointer, "float", ifTrue);
                PlaySound(Properties.Resources.Activated);
                return true;
            }
            
            // Reset value to original
            rw.WriteMemory(pointer, "float", ifFalse);
            PlaySound(Properties.Resources.Deactivated);
            return false;
        }

        // play a sound file, if hotkeys alt is pressed
        private void RailRoadHotkeyWriteGuard()
        {
            // PlaySound(Properties.Resources.Activated);
            if (StaticRoadOriginalValue is null) return;
            if (StaticRoadIGNeededOriginalValue is null) return;
            if (StaticRoadOriginalCostValue is null) return;
            
            if (StaticRailOriginalValue is null) return;
            if (StaticRailIGNeededOriginalValue is null) return;
            if (StaticRailOriginalCostValue is null) return;

            
            // Build time
            if (StaticRailPointer is null) return;
            WriteToMemoryAndPlaySound(StaticRoadPointer, "0.10", StaticRoadOriginalValue);
            if (StaticRoadPointer is null) return;
            WriteToMemoryAndPlaySound(StaticRailPointer, "0.10", StaticRailOriginalValue);
            
            // Cost
            if (StaticRoadCostPointer is null) return;
            WriteToMemoryAndPlaySound(StaticRoadCostPointer, "0.00", StaticRoadOriginalCostValue);
            if (StaticRailCostPointer is null) return;
            WriteToMemoryAndPlaySound(StaticRailCostPointer, "0.00", StaticRailOriginalCostValue);
            
            // IG Needed
            if (StaticRoadIGNeededPointer is null) return;
            WriteToMemoryAndPlaySound(StaticRoadIGNeededPointer, "0.01", StaticRoadIGNeededOriginalValue);
            if (StaticRailIGNeededPointer is null) return;
            WriteToMemoryAndPlaySound(StaticRailIGNeededPointer, "0.01", StaticRailIGNeededOriginalValue);

        }

        #endregion
        

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _hotkeys.Unregister(_userHotkey, _userKeymodifier);
            
            // _hotkeys.Unregister(Key.B, System.Windows.Input.ModifierKeys.Alt);
            _hotkeys.Dispose();
            
            Settings.UserSkin = UserLookAndFeel.Default.SkinName;
            
            if (_miniWindow == null) return;
            _miniWindow.Close();
            _miniWindow = null;
        }

        private readonly string[] _excludedFeatureName = { "UnitID" };
        private void ceDefenseSelection_CheckedChanged(object sender, EventArgs e)
        {
            if (!ceDefenseSelection.Checked) return;
            groupControlSelectedUnit.Enabled = true;
            checkedListBoxControl1.Enabled = true;
            ceModePersistent.Enabled = true;
            ceModeHover.Enabled = true;
            
            // Mutating pointerId of all included features
            JsonReader.MutateFeaturePointer(rw, 
                    ListOfSortedRow.WarfareIncludedFeatureList
                        .Except(_excludedFeatureName).ToArray(),24, 3);
            
            // Mutate UnitID
            JsonReader.MutateFeaturePointer(rw, 
                _excludedFeatureName,32, 13);
            
            // Rebuild Pointer
            
            FeatureChangeState(ListOfSortedRow.WarfareExcludedFeatureList, true);
            // gcWarfare.Dispose();
            gcWarfare.DataSource = SRMain.Instance.FeaturesWarfare;
            gcWarfare.RefreshDataSource();
        }

        private SREnum.TabSelection _tabSelection = SREnum.TabSelection.Land;
        public void ceDefenseProduction_Refresher()
        {
            var tabselect = "GameTabSelection".GetFeature().Read().value;
            if (!String.IsNullOrEmpty(tabselect)) 
                _tabSelection = SREnum.ConvertTabSelectionToString(tabselect);
            // "GameTabSelection".GetFeature(JsonReader.FeaturesSpecial).WriteTo(rw, "5");

            if (_tabSelection != SREnum.TabSelection.DefenseProduction)
            {
                ceDefenseSelection.Checked = true;
                ceDefenseProduction.ReadOnly = true;
                ceDefenseProduction.Properties.AppearanceReadOnly.ForeColor = Color.LightGray;
            }
            else
            {
                ceDefenseProduction.ReadOnly = false;
                ceDefenseProduction.Properties.Appearance.ForeColor = Color.White;
            }
        }
        private void ceDefenseProduction_CheckedChanged(object sender, EventArgs e)
        {

            if (!ceDefenseProduction.Checked) return;
            
            JsonReader.SetOriginalValueToCurrentValue();
            // "GameTabSelection".GetFeature(JsonReader.FeaturesSpecial).WriteTo(rw, "5");
            groupControlSelectedUnit.Enabled = false;
            checkedListBoxControl1.Enabled = false;
            ceModePersistent.Enabled = false;
            ceModeHover.Enabled = false;

            // groupControlSelectedUnit.Visible = false;
            JsonReader.MutateFeaturePointer(rw, 
                ListOfSortedRow
                    .WarfareIncludedFeatureList
                    .Except(_excludedFeatureName).ToArray(), 3, 24);
            JsonReader.MutateFeaturePointer(rw, 
                _excludedFeatureName, 13, 32);
            FeatureChangeState(ListOfSortedRow.WarfareExcludedFeatureList, false);
            gcWarfare.DataSource = SRMain.Instance.FeaturesWarfare;
            gcWarfare.RefreshDataSource();
        }

        private void FeatureChangeState(string[] featureNameList, bool condition)
        {
            foreach (var feature in featureNameList)
            {
                JsonReader.FeatureIndexedStore[feature].enabled = condition;
            }
        }

        private void barBtnForceUpdate_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonReader.ResourceSwapColumnGroup(gvResources);
            // gvResources.Columns["displayName"].Group();
           Loader.ForceUpdate();
        }

        private void TrackingButtonStyleChanger()
        {
            var trackedRowId = TrackedUnitsController.GetTrackedRowId(rw);
            if (trackedRowId != -1)
            {
                cePersistentUnitIndicator.Text =
                    $"Tracked ({TrackedUnitsController.TrackedUnits.Count.ToString()})";
                cePersistentUnitIndicator.ForeColor = Color.LimeGreen;
                cePersistentUnitIndicator.CheckState = CheckState.Checked;

                // if (!SRGuard.TrackedBtnIsAllowedToContinue() ) return;
                btnMakePersistent.Enabled = true;
                btnMakePersistent.Text =
                    string.Format("Untrack ({0}) [{1}]", trackedRowId.ToString(), "UnitName".GetFeature().value);
                btnMakePersistent.ForeColor = Color.GhostWhite;
                btnMakePersistent.Appearance.BackColor = Color.Brown;
                btnMakePersistent.BackColor = Color.Brown;
            }
            else
            {
                cePersistentUnitIndicator.ForeColor = Color.DimGray;
                cePersistentUnitIndicator.Text = "Untracked";
                cePersistentUnitIndicator.CheckState = CheckState.Unchecked;
                // btnMakePersistent.Enabled = !string.IsNullOrEmpty("UnitName".GetFeature().value);

                // if (!SRGuard.TrackedBtnIsAllowedToContinue()) return;
                btnMakePersistent.Enabled = !("ArmyCurrentStrength".GetPointer(rw).Length < 4);
                btnMakePersistent.Text = "Track selected unit: [" +
                                         "UnitName".GetFeature(SREnum.CategoryName.Warfare)?.Read(rw) + "]";
                btnMakePersistent.ForeColor = Color.Empty;
                btnMakePersistent.Appearance.BackColor = Color.MediumSpringGreen;
                btnMakePersistent.BackColor = Color.MediumSpringGreen;

            }
        }

        // To help with allocated issues, make it with even mannered
        private CancellationTokenSource _ctsThread = new CancellationTokenSource();
        private void EventsDeclarationRunJustOnce()
        {
            Events.OnUnitIsSelected += (sender, args) =>
            {
                var o = sender as SREvents;
                TrackingButtonStyleChanger();
            };
            
            TrackedUnitsController.OnTrackedUnitsUpdated += (sender, args) =>
            {
                TrackingButtonStyleChanger();
            };

            Events.OnNavalIsSelected += (sender, args) =>
            {
                var o = sender as SREvents;
                SetSpinEditDisplayPercentage(seUnitStrActual, o.IsNaval);
                SetSpinEditDisplayPercentage(seUnitStrCurrent, o.IsNaval);
                seUnitStrCurrent.Properties.Increment = o.IsNaval ? 0.1m : 5m;
                seUnitStrActual.Properties.Increment = o.IsNaval ? 0.1m : 5m;  
                seUnitFuel.Properties.Increment = o.IsNaval ? 100m : 1m;
            };

            leBuildModeOverride.EditValueChanged += (sender, args) =>
            {
                var o = sender as LookUpEdit;
                Events.BuildModeOverrideId = o.EditValue.ToString();
            };
            Events.OnBuildModeOverrideIsChanged += (sender, _) =>
            {
                var o = sender as SREvents;
                if (leBuildModeOverride.EditValue == null) return;
                o.BuildModeOverrideId = leBuildModeOverride.EditValue.ToString();
                // BuildModeOverride(leBuildModeOverride, o.BuildModeOverride);
                // if (!allower) return;
                // if(leBuildModeOverride is { ItemIndex: -1 })
                // {
                //     "BuildModeFacilityID".GetFeature().WriteTo(rw, o.BuildModeOverrideId);
                //     "BuildModeFacilityIDOverride".GetFeature().WriteTo(rw, o.BuildModeOverrideId);
                // }
                //
                // leBuildModeOverride.ItemIndex 
                //     = leBuildModeOverride
                //         .Properties
                //         .GetDataSourceRowIndex(
                //             "Value", 
                //             "BuildModeFacilityIDOverride".GetFeature().Read(rw).ToString());
            };
        }

        private int _justOnce = 0;
        private string _unitSelected;
        private string _unitDeployed;
        private string _unitReserved;
        private string _unitBattleGroup;
        private int _unitPDeployed;
        private int _unitPReserved;
        private int _unitTotal;
        /// <summary> The timer1_Tick_1 function is called every second.</summary>
        ///
        /// <param name="sender"> </param>
        /// <param name="EventArgs"> </param>
        ///
        /// <returns> Void.</returns>
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            //
            //Check if full screen
            
            IsFullScreen = "OptionWindowedMode".GetFeature().ReadInt(rw) == 39;
            barMiniWindowBtn.Enabled = !IsFullScreen;
            
            // checkedListBoxControl1.DataSourceChanged += (o, args) =>
            // {
                // CheckedListBoxControl clbc = o as CheckedListBoxControl;
                for (int i = 0; i < checkedListBoxControl1.ItemCount; i++)
                {
                    checkedListBoxControl1.SetItemChecked(i, HandmadeFeatures.Instance.Lists[i].freeze);
                }
                
                for (int i = 0; i < ceListBoxCheats.ItemCount; i++)
                {
                    ceListBoxCheats.SetItemChecked(i, SpecialCheats.Cheats[i].Checked);
                }
            

                // Threading;
            if (_justOnce == 0)
            EventsDeclarationRunJustOnce();
            if (_justOnce == 0) 
            _justOnce += 1;
            
            // Non threading

            // rw.IsTrainerNeedRestart();
            GvPersistentDetailRuntime();
            // string unitName = rw.SRRead("UnitName");
            // string unitID = rw.SRRead("UnitID");
            _unitSelected = "UnitSelected".GetFeature(SREnum.CategoryName.Warfare).value;
            _unitDeployed = "UnitDeployed".GetFeature(SREnum.CategoryName.Warfare).value;
            _unitReserved = "UnitReserve".GetFeature(SREnum.CategoryName.Warfare).value;
            _unitBattleGroup = "UnitBattleGroup".GetFeature(SREnum.CategoryName.Warfare).value;
            _unitPDeployed = (_unitDeployed.StrToInt());
            _unitPReserved = _unitReserved.StrToInt();
            int unitTotal = _unitPDeployed + _unitPReserved;

            // Display Current usage/production of MG/Petrol
            WarfareResoureDisplay();

            // Selected/Battle Groups For Show
            memoEdit1.EditValue = $@"Selected: {_unitSelected}
Deployed: {_unitDeployed} ({NumericExtension.SafePercentage(_unitPDeployed, unitTotal, "P1")})
Reserved: {_unitReserved} ({NumericExtension.SafePercentage(_unitPReserved, unitTotal, "P1")})
Battle Group: {_unitBattleGroup}";

            SpecialOption();
            MissileMadness();
            ceDefenseProduction_Refresher();

            if (UnitClass.value != string.Empty)
            {
                Events.IsNaval = rw.SRIsNaval(Convert.ToInt32(UnitClass.value));
                IsNaval = Events.IsNaval;
            }

            Events.BuildModeOverride = ceBuildModeOverride.Checked;
            // if(ceBuildModeOverride.Checked)
                
            
            // countryInfoTable.UpdateTable();
            // Updating Data Grid View for specific class.

            // Save to global unit
            // if(jsonReader.feature("UnitName").value == "")
            // Updating unit history in datagrid DEBUG View list
            UpdateUnitHistoryList(JsonReader.FeaturesWarfare);
            SRMemento.Instance.SaveToGlobalUnitStore(JsonReader.WarfareIndexedFeatures, rw);

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

            // v3 -removed changed with GRIDVIEW
            // ARMY - Class updater and changer. 
            // UnitClassLookUpEditHandler("UnitMovementType", lookBoxUnitMovementType);
            // UnitClassLookUpEditHandler("UnitTargetType", lookBoxUnitTargetType);
            // UnitClassLookUpEditHandler("UnitClass", lookBoxUnitClass);
            // Tech Effect Handler with lookupEdit Controls
            // FacilitiesLookUpEditHandler("TechEffectIndustrializationEffect1", leTechEffect1);
            // FacilitiesLookUpEditHandler("TechEffectIndustrializationEffect2", leTechEffect2);

            // Reader unified
            ArmyControlReader();
            ArmyControlWriter();

            
            // TODO: DPA, change into events based;
            // Set specific spinedit to display % when naval is selected
            // SetSpinEditDisplayPercentage(seUnitStrActual, IsNaval);
            // SetSpinEditDisplayPercentage(seUnitStrCurrent, IsNaval);
            // seUnitStrCurrent.Properties.Increment = IsNaval ? 0.1m : 5m;
            // seUnitStrActual.Properties.Increment = IsNaval ? 0.1m : 5m;
            
            // seUnitSupplies.Properties.Increment = IsNaval ? 100m : 1m;
            //
            // SetSpinEditDisplayPercentage(seUnitFuel, false);
            // SetSpinEditDisplayPercentage(seUnitSupplies, false);

            // READ ARMY ACTIVE STAFF REES
            // UnitReader(seStaffActive, jsonReader.FeaturesWarfare);
            // UnitReader(seStaffReserve, jsonReader.FeaturesWarfare);
            // UnitReader(seUnitFuel, jsonReader.FeaturesWarfare);
            // UnitReader(seUnitGas, jsonReader.FeaturesWarfare);

            // This update Army Naval Health blablabla
            // calculateHealth();

            // V3 Removed
            FacilitiesReader();
            FacilitiesWriter();

            TechEffectReader();
            // TechEffectWriter();

            UnitModelSizeReader();
            RailRoadBridgeCapturer();

            if (Events.BuildModeOverride && Events.BuildModeOverrideId != "-1")
            {
                "BuildModeFacilityID".GetFeature().WriteTo(rw, Events.BuildModeOverrideId);
                "BuildModeFacilityIDOverride".GetFeature().WriteTo(rw, Events.BuildModeOverrideId);
            }
            
            // Multiplayer Protection
            MultiplayerProtection();
        }

        /// <summary> The gvPersistentUnitStats_CustomRowFilter function is used to hide the rows of the grid view that are not needed.
        /// The function is called when a row in the grid view is selected, and it checks if there are any excluded stats for that unit.
        /// If there are excluded stats, then it hides all of them from being displayed.</summary>
        ///
        /// <param name="sender"> </param>
        /// <param name="RowFilterEventArgs"> /// the rowfiltereventargs.
        /// </param>
        ///
        /// <returns> True if the row is visible, false otherwise.</returns>
        public void gvPersistentUnitStats_CustomRowFilter(object sender, RowFilterEventArgs e)
        {
            // GridView view = sender as GridView;
            // if (view.GetRowHandle(e.ListSourceRow) == view.FocusedRowHandle) return;
            // TrackedUnitStat mc = (view.GetRow(e.ListSourceRow) as TrackedUnitStat);
            // // e.Visible = !ListOfSortedRow.PersistentUnitExcludedStats.Contains(mc.StatName);
            // if (ListOfSortedRow.PersistentUnitExcludedStats.Contains(mc.StatName))
            // {
            //     e.Visible = false; 
            //     e.Handled = true;
            // } 
        }
        
    }
}