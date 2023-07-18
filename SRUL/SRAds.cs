using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdsJumboWinForm;
using DevExpress.XtraBars.ToolbarForm;
using SRUL.Properties;

namespace SRUL
{
    public struct AdsProperties
    {
        public Point Location { get; set; }
        public Size Size { get; set; }
        public int TabIndex { get; set; }
        public string ApplicationId { get; set; }
        public string Name { get; set; }
        public DockStyle Dock { get; set; }
        
        public int WidthAd { get; set; }
        public int HeightAd { get; set; }
    }
    public class AdsCreator
    { 
        public static List<AdsProperties> AdsList;
        public AdsCreator()
        {
            AdsList = new List<AdsProperties>();
        }

        public void DisposeAndCreate<T>(ref T ad, Control ctr) where T: UserControl
        {
            if (ctr.Visible)
            {
                AdsDisposer(ref ad);
                if (ad.IsDisposed)
                {
                    ad = CreateAd<T>(ad.Name);
                    if (!ctr.Controls.ContainsKey(ad.Name))
                        ctr.Controls.Add(ad);

                    if (typeof(T) != typeof(InterstitialAd))
                        ad.Show();
                    
                    ad.BringToFront();
                    // {
                    //     var ads = (ad as InterstitialAd);
                    //     if (ads == null) return; 
                    //     ads.ShowInterstitialAd(Properties.Settings.Default.ApplicationAdsKey);
                    // }
                    // else 
                }
            }
        }
        
        public void DisposeIfNotVisible<T>(ref T ad, in Control ctr) where T: UserControl
        {
            if (!ctr.Visible)
            {
                if(ctr.Controls.ContainsKey(ad.Name))
                    ctr.Controls.RemoveByKey(ad.Name);
                AdsDisposer(ref ad);
            }
            else if (ctr.Visible)
            {
                if (ad == null) return;
                if (ad.IsDisposed)
                {
                    ad = CreateAd<T>(ad.Name);
                    if (!ctr.Controls.ContainsKey(ad.Name))
                        ctr.Controls.Add(ad);

                    if (typeof(T) != typeof(InterstitialAd))
                        ad.Show();
                    
                    ad.BringToFront();
                }
            }
        }
        
        public void AdsDisposer<T>(ref T ad) where T : UserControl
        {
            if (!ad.IsDisposed)
            {
                ad.Visible = false;
                ad.Dispose();
                GC.SuppressFinalize(this);
                System.GC.Collect();
                // System.GC.Collect(0, GCCollectionMode.Forced);
                System.GC.WaitForFullGCComplete();
            }
        }

        public void AttachEvents<T>(EventHandler evt, ref T ad)
        {
            switch (typeof(T))
            {
                case { } t when t == typeof(InterstitialAd):
                    (ad as InterstitialAd).VisibleChanged -= evt;
                    (ad as InterstitialAd).VisibleChanged += evt;
                    break;
            }
        }
        public void RegisterACopy<T>(T ad, int w, int h) where T : UserControl
        {
            foreach(var listedAd in AdsList)
                if (listedAd.Name == ad.Name)
                    return;
            
            var props = new AdsProperties();
            props.Name = ad.Name;
            props.ApplicationId = Properties.Settings.Default.ApplicationAdsKey;
            props.Location = ad.Location;
            props.Size = ad.Size;
            props.Dock = ad.Dock;
            props.TabIndex = ad.TabIndex;
            props.WidthAd = w;
            props.HeightAd = h;
            AdsList.Add(props);
        }

        public T CreateAd<T>(string name) 
            where T : UserControl
        {
            AdsProperties prop = default; 
            foreach (var ad in AdsList) 
            {
                if (ad.Name != name) continue;
                prop = ad;
                break;
            }

            switch (typeof(T))
            {
                case var a when a == typeof(BannerAds):
                    var bannerAds = new BannerAds();
                    bannerAds.Name = prop.Name;
                    bannerAds.Location = prop.Location;
                    bannerAds.Size = prop.Size;
                    bannerAds.HeightAd = prop.HeightAd;
                    bannerAds.WidthAd = prop.WidthAd;
                    bannerAds.Dock = prop.Dock;
                    bannerAds.TabIndex = prop.TabIndex;
                    bannerAds.ApplicationId = prop.ApplicationId;
                    return bannerAds as T ?? throw new InvalidOperationException();
                case var b when b == typeof(InterstitialAd):
                    var interstitialAd = new InterstitialAd();
                    interstitialAd.ApplicationId = prop.ApplicationId;
                    interstitialAd.Name = prop.Name;
                    interstitialAd.Location = prop.Location;
                    interstitialAd.Size = prop.Size;
                    interstitialAd.BackColor = System.Drawing.Color.Black;
                    interstitialAd.Visible = false;
                    interstitialAd.TabIndex = prop.TabIndex;
                    return interstitialAd as T ?? throw new InvalidOperationException();
            }

            throw new InvalidOperationException();
        }

        public InterstitialAd Create(Timer adsTimer)
        {
            InterstitialAd allScreenAds = new InterstitialAd();
            allScreenAds.ApplicationId = Settings.Default.ApplicationAdsKey;
            allScreenAds.BackColor = System.Drawing.Color.Black;
            allScreenAds.Location = new System.Drawing.Point(304, -11);
            allScreenAds.Name = "allScreenAds";
            allScreenAds.Size = new System.Drawing.Size(52, 59);
            allScreenAds.TabIndex = 0;
            allScreenAds.Visible = false;
            allScreenAds.VisibleChanged += (sender, args) =>
            {
                if ((sender as InterstitialAd).Visible)
                    adsTimer.Stop();
                else
                {
                    adsTimer.Start();
                    if(adsTimer.Interval <= 300000) 
                        adsTimer.Interval += (int)TimeSpan.FromSeconds(20).TotalMilliseconds;
                }
            };
            return allScreenAds;
        }
    }
    public interface IAds<T> where T : UserControl
    {
        int SeenTimes { get; set; }
        DateTime LastSeen { get; set; }
        bool VisibleState { get; set; }
        public T Ad { get; set; }
    }
    public class ManagedBannerAd : IAds<BannerAds>
    {
        private bool _active;

        // private bool _visibleState;
        public int SeenTimes { get; set; }

        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
            }
        }

        public DateTime LastSeen { get; set; }
        public BannerAds Ad { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public bool SetActive(string key, bool cond)
        {
            try
            { 
                Active = cond;
                VisibleState = true;
                Ad.ShowAd(Width, Height, key);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
        
        public bool VisibleState
        {
            get => Ad.Visible;
            set
            {
                Ad.Visible = value;
            }
        }

        public bool once { get; set; }
        
        public ManagedBannerAd(BannerAds ad, int width, int height)
        {
            Ad = ad;
            Width = width;
            Height = height;
            VisibleState = false;
            Active = false;
            this.once = true;
            SeenTimes = 0;
            LastSeen = DateTime.Now;
        }
    }
    
    public class ManagedInterstitialAd : IAds<InterstitialAd>
    {
        private bool _visibleState;
        public int SeenTimes { get; set; }
        public DateTime LastSeen { get; set; }
        public InterstitialAd Ad { get; set; }
        public bool InformToStop { get; set; }

        public bool VisibleState
        {
            get
            {
                return Ad.Visible;
            }
            set
            {
                if (Ad.Visible == value) return;
                Ad.Visible = value;
            }
        }

        public bool once { get; set; }

        public void Show(string key)
        {
            Ad.ShowInterstitialAd(key);
            LastSeen = DateTime.Now;
        }

        public ManagedInterstitialAd(InterstitialAd ad, bool runOnce)
        {
            Ad = ad;
            SeenTimes = 0;
            LastSeen = DateTime.Now;
            VisibleState = ad.Visible;
            InformToStop = false;
            once = runOnce;

            Ad.VisibleChanged += OnAdOnVisibleChanged;
        }

        private void OnAdOnVisibleChanged(object sender, EventArgs args)
        {
            VisibleState = Ad.Visible;
            if (VisibleState)
                InformToStop = true;
            else
                InformToStop = false;
            SeenTimes++;
            LastSeen = DateTime.Now;
        }
    }

    public class SRAdsManager
    {
        private Timer _timer;
        private readonly string _key = Settings.Default.ApplicationAdsKey;
        private bool Approval = false;
        private TimeSpan Interval;
        private int _interstitialAdsSeenTimes;
        private int _bannerAdsSeenTimes;
        private List<ManagedBannerAd> ManagedBannerAds { get; set; } = new List<ManagedBannerAd>();
        private List<ManagedInterstitialAd> ManagedInterstitialAds { get; set; } = new List<ManagedInterstitialAd>();
        public SRAdsManager()
        {
            _timer = new Timer();
            _timer.Tick += (o, args) => Timer_Tick(o, args);
            Interval = TimeSpan.FromMinutes(1);
            _timer.Interval = (int)Interval.TotalMilliseconds;
            _timer.Enabled = true;
        }

        public void ShowOnce()
        {
            foreach (var manager in ManagedInterstitialAds)
            {
                if (Approval) break;
                if(!manager.once) continue; 
                    manager.Show(_key);
            }
        }

        public bool AskForApproval()
        {
            // Approval = SRLoader.SRDeviceApproval.Get;
            // return Approval;
            return false;
        }
        
        public void AddAd(ManagedBannerAd ad)
        {
            ManagedBannerAds.Add(ad);
        }
        
        public void AddAd(ManagedInterstitialAd ad)
        {
            ManagedInterstitialAds.Add(ad);
            // foreach (var manager in ManagedInterstitialAds)
            // {
            //     if (manager.Ad.Name.Equals(ad.Ad.Name)) continue;
            //     ManagedInterstitialAds.Add(ad);
            // }
        }

        public void SetActive(BannerAds ad, bool active)
        {
            foreach (var manager in ManagedBannerAds)
            {
                if (manager.Ad.Equals(ad))
                    manager.Active = active;
            }
        }
        
        public void StartAll()
        {
            if (Approval) return;
            // banner
            foreach (var bannerAd in ManagedBannerAds)
            {
                bannerAd.SetActive(_key, true);
            }
            
            // interstitial
            Start();
        }
        
        public void Start()
        {
            _timer.Start();
        }
        
        public void Stop()
        {
            _timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if(Approval) Stop();
                foreach (var bannerAd in ManagedBannerAds)
                {
                    bannerAd.VisibleState = Approval;
                }
                foreach (var manager in ManagedInterstitialAds)
                {
                    if (manager.once && manager.SeenTimes < 1)
                        manager.Show(_key);
                    
                    if (!manager.InformToStop && (!manager.once && manager.SeenTimes >= 1))
                    {
                        manager.Show(_key);
                        _interstitialAdsSeenTimes++;
                        _timer.Interval = (int)Interval.TotalMilliseconds;
                    }
                    else
                    { 
                        _timer.Interval = 30000;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw exception;
            }
        }
    }
    // public class SRAds : IDisposable
    // {
    //     // public List<ManagedAds> managedAds;
    //     private int MsInterval { get; set; } = 3000; // in milliseconds
    //     private List<Timer> TimerInstance { get; set; } = new List<Timer>();
    //     private TimeSpan StartTimeSpan { get; set; } = TimeSpan.Zero; // 
    //     private TimeSpan PeriodTimeSpan { get; set; } = TimeSpan.FromSeconds(10); // 
    //     private int AdsEncountered { get; set; } = 0;
    //     internal string Key { get; } = "p8vzazwanqyo";
    //
    //     private List<AdsJumboWinForm.BannerAds> AdBannerList = new List<AdsJumboWinForm.BannerAds>();
    //     private List<AdsJumboWinForm.InterstitialAd> AdIntestitialList = new List<AdsJumboWinForm.InterstitialAd>();
    //     private static readonly Lazy<SRAds> _instance = new Lazy<SRAds>(() => new SRAds());
    //     public static SRAds Instance => _instance.Value;
    //
    //     public void showAds(string adType)
    //     {
    //         if (adType == "Banner")
    //         {
    //             foreach (var item in AdBannerList)
    //             {
    //                 item.Show();
    //             }
    //         }
    //         else if (adType == "Interstitial")
    //         {
    //             foreach (var item in AdIntestitialList)
    //             {
    //                 item.Show();
    //             }
    //         }
    //     }
    //
    //     public void createBanner()
    //     {
    //         var banner = new AdsJumboWinForm.BannerAds();
    //         banner.ApplicationId = "p8vzazwanqyo";
    //         banner.BackColor = System.Drawing.Color.White;
    //         banner.HeightAd = 73;
    //         banner.Location = new System.Drawing.Point(2, 515);
    //         banner.Name = "bannerAdsProfileBoxWidthBelow";
    //         banner.Size = new System.Drawing.Size(465, 60);
    //         banner.TabIndex = 10;
    //         banner.WidthAd = 463;
    //     }
    //     public async Task Init()
    //     {
    //         var test = await SRLoaderForm._srLoader.apis.PostDeviceApproval();
    //         SRLoader.SRDeviceApproval = test.body;
    //     }
    //     public void ShowAd(BannerAds adControl, int w, int h)
    //     {
    //         // if (SRLoader.SRDeviceApproval.Approval) return; 
    //         adControl.ShowAd(w, h, Key);
    //     }
    //
    //     public void InitAds(InterstitialAd adControl)
    //     {
    //         
    //     }
    //     public void ShowAd(InterstitialAd adControl, bool condition, int interval)
    //     {
    //         // if (SRLoader.SRDeviceApproval.Approval) return;
    //         Parallel.Invoke(() =>
    //         {
    //             adControl.ShowInterstitialAd(Key);
    //             adControl.Dispose();
    //             var t = new Timer();
    //             t.Enabled = true;
    //             t.Interval = interval;
    //             t.Tick += (sender, args) =>
    //             {
    //                 if (condition)
    //                 {
    //                     adControl.BringToFront();
    //                     adControl.ShowInterstitialAd(Key);
    //                     adControl.Dispose();
    //                     adControl.BringToFront();
    //                 }
    //             };
    //         });
    //     }
    //
    //     public void ShowInterstitialAd()
    //     {
    //
    //         AdsJumboWinForm.InterstitialAd a = new AdsJumboWinForm.InterstitialAd();
    //         a.ApplicationId = null;
    //         a.BackColor = System.Drawing.Color.Black;
    //         a.Location = new System.Drawing.Point(62, 124);
    //         a.Name = "interstitialAd1";
    //         a.Size = new System.Drawing.Size(50, 50);
    //         a.TabIndex = 33;
    //         a.Visible = false;
    //     }
    //
    //     // public void InitAdsPeriod()
    //     // {
    //     //     var t = new System.Threading.Timer(e =>
    //     //     {
    //     //         ShowInterstitialAd();
    //     //     }, null, StartTimeSpan, PeriodTimeSpan);
    //     //     TimerInstance.Add(t);
    //     // }
    //     private void ReleaseUnmanagedResources()
    //     {
    //         // TODO release unmanaged resources here
    //     }
    //
    //     public void Dispose()
    //     {
    //         ReleaseUnmanagedResources();
    //         GC.SuppressFinalize(this);
    //     }
    //
    //     ~SRAds()
    //     {
    //         ReleaseUnmanagedResources();
    //     }
    // }
}
