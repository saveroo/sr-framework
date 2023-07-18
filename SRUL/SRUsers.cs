using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;
using Newtonsoft.Json;
using SRUL.Annotations;
using SRUL.Types;

namespace SRUL
{
    public class SRPlayer
    {
        public bool IsOnline { get; set; }

        [JsonProperty("steamid")]
        public string Steamid { get; set; } 

        [JsonProperty("communityvisibilitystate")]
        public int Communityvisibilitystate { get; set; } 

        [JsonProperty("profilestate")]
        public int Profilestate { get; set; }

        [JsonProperty("profileurl")]
        public string Profileurl { get; set; }
        [JsonProperty("avatar")][CanBeNull] public byte[] Avatar { get; set; }

        [JsonProperty("avatarmedium")][CanBeNull] public byte[] Avatarmedium { get; set; }
        [JsonProperty("personaname")]
        public string Personaname { get; set; }

        [JsonProperty("avatarhash")] [CanBeNull] public string Avatarhash { get; set; }
    }

    public class SRPlayers
    {
        public SRPlayer Me;
        public List<SRPlayer> Players;
    }

    public class DeviceApproval
    {
        public static TDeviceApproval Instance = new TDeviceApproval();
        public static async Task<TDeviceApproval> CheckForApproval(SRApi api)
        {
            var test = await api.PostDeviceApproval().ConfigureAwait(false);
            // get this async result
            if (test == null) return Instance;
            if (test.statusCode == 200)
            {
                Instance.DeviceID = test.body.DeviceID;
                Instance.Approval = test.body.Approval;
            }

            if (!SrApiConfig.OfflineMode)
                if (Instance.Approval)
                    return Instance;
            return Instance;
        }
    }

    public class TDeviceApproval
    {
        [JsonProperty("approval")]
        public bool Approval { get; set; } = false;
        public string DeviceID { get; set; } = "0";
    }
    public class TRequestDeviceApproval
    {
        public string DeviceID { get; set; }
        
    }
    public class TResponseDeviceApproval
    {
        public int statusCode { get; set; }
        public TDeviceApproval body { get; set; }
    }

    public struct SteamRunAPI
    {
        public static string OpenGameHub { get; set; } = "steam://url/GameHub/314980";
        public static string OpenWorkshop { get; set; } = "steam://url/SteamWorkshopPage/314980";
        public static string OpenProfile { get; set; } = "steam://url/SteamIDMyProfile";
        public static string OpenInventory { get; set; } = "steam://url/CommunityInventory";
        public static string OpenMapEditor { get; set; } = "steam://run/916420";
        public static string OpenContentEditor { get; set; } = "steam://run/916500";
        public static string GameValidate { get; set; } = "steam://validate/314980";
        public static string ExitSteam { get; set; } = "steam://exit";
        public static string DeveloperProfileURI { get; set; } = "https://steamcommunity.com/id/saveroo/";
        
        public static string OpenGuidesPage { get; set; } = "https://steamcommunity.com/app/314980/guides/";

        public static void BindEvent(SimpleButton sb, string runAPIProperty)
        {
            sb.Click += (sender, args) =>
            {
                Process.Start(runAPIProperty);
            };
        }
    }

    public class SRPlayersControl
    {
        private GridControl gc { get; set; }
        private TileView gv { get; set; }
        public string[] excluded = new[] {
            "Steamid", "Communityvisibilitystate", "Profilestate", 
            "Profileurl", "Avatarhash", "Avatarmedium"
        };
        private static readonly Lazy<SRPlayersControl> _instance = new Lazy<SRPlayersControl>(() => new SRPlayersControl());
        public static SRPlayersControl Instance => _instance.Value;
        void visitProfile(int index)
        {
            System.Diagnostics.Process.Start(SRLoader.SRPlayers.Players[index].Profileurl);
        }
        public void InitControls(GridControl gcControl, 
            TileView gvControl)
        {
            this.gc = gcControl;
            this.gv = gvControl;
            if (SRLoader.SRPlayers == null) return;

            gc.DataSource = SRLoader.SRPlayers.Players;
            // foreach (var col in excluded)
            // {
            //     this.gv.Columns[col].Visible = false;
            // }

            gv.Columns["IsOnline"].AbsoluteIndex = 9;
            gv.Columns["Avatar"].AbsoluteIndex = 8;
            gv.Columns["Personaname"].AbsoluteIndex = 7;
            // gv.OptionsView.RowAutoHeight = true;
            // gv.OptionsView.EnableAppearanceEvenRow = true;
            // gvPlayers.OptionsView. = true;

            ((TileViewItemElement)gv.TileTemplate[0]).Column = gv.Columns["Avatar"];
            ((TileViewItemElement)gv.TileTemplate[0]).TextVisible = false;
            ((TileViewItemElement)gv.TileTemplate[1]).Column = gv.Columns["Personaname"];
            ((TileViewItemElement)gv.TileTemplate[3]).Column = gv.Columns["IsOnline"];
            ((TileViewItemElement)gv.TileTemplate[3]).TextVisible = false;
            gv.ItemCustomize += (sender, args) =>
            {
                var isOnline = args.Item.Elements[3].Text;
                var status = isOnline == "true" ? "apply" : "cancel";
                args.Item.Elements[3].Image =
                    DevExpress.Images.ImageResourceCache.Default.GetImage($"images/actions/Colored/${status}_32x32.png");
            };
            gv.ItemClick += (object sdr, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs me) =>
            {
                var index = gv.GetDataSourceRowIndex(me.Item.RowHandle);
                TileView view = sdr as TileView;
                Point pt = view.GridControl.PointToClient(Control.MousePosition);
                TileViewHitInfo hitInfo = view.CalcHitInfo(pt);
                if (hitInfo.ItemInfo != null)
                {
                    var idx = me.Item.RowHandle;
                    // string text = elementInfo.Element.Text;
                    // MessageBox.Show(SRLoader.SRPlayers.Players[e.Item.RowHandle].Profileurl);
                    TileItemElementViewInfo elementInfo = hitInfo.ItemInfo.Elements.FirstOrDefault(t => t.EntireElementBounds.Contains(pt));
                    if (elementInfo != null)
                    {
                        if (elementInfo.Text == "Visit profile")
                            visitProfile(idx);
                        if (elementInfo.Text == "Add friend")
                            Process.Start($"steam://friends/add/{SRLoader.SRPlayers.Players[idx].Steamid}");
                        // ((ContextButton) tileView1.ContextButtons[0]).Caption = text;
                    }
                }
            };
        }

        private List<Feature> gvSortList(IList<Feature> feature, string[] sortedList)
        {
            return feature.OrderBy(x =>
            {
                var index = Array.IndexOf(sortedList, x.name);
                return index < 0 ? int.MaxValue : index;
            }).ToList();
        }

        public void gvRowFilterExclusion(object sender, RowFilterEventArgs e)
        {
            GridView view = (GridView)sender;
            string recordName = view.GetListSourceRowCellValue(e.ListSourceRow, "name").ToString();
            string recordCategory = view.GetListSourceRowCellValue(e.ListSourceRow, "category").ToString();
            if (recordCategory.ToLower() == "warfare")
            {
                if (ListOfSortedRow.warfareExcludedInDataView.Contains(recordName))
                {
                    e.Visible = false;
                    e.Handled = true;
                }
            }
        }

        public class CustomPopupMenu : DevExpress.XtraBars.PopupMenu
        {
            public Point BottomRightCorner { get; internal set; }
            public CustomPopupMenu() { }
            public CustomPopupMenu(IContainer container) : base(container) { }
            public CustomPopupMenu(BarManager manager) : base(manager) { }

            protected override PopupMenuBarControl CreatePopupControl(BarManager manager)
            {
                return new CustomPopupMenuBarControl(manager, this);
            }

            public bool ShowTileItemPopup(TileItem tileItem)
            {
                var tileBounds = (tileItem as DevExpress.Utils.VisualEffects.ISupportAdornerElement).Bounds;
                var tileView = (tileItem.Group as DevExpress.XtraGrid.Views.Tile.TileViewGroup).View;
                BottomRightCorner = tileView.GridControl.PointToScreen(new Point(tileBounds.Right, tileBounds.Bottom));

                base.ShowPopup(Point.Empty);
                return true;
            }
        }

        public class CustomPopupMenuBarControl : PopupMenuBarControl
        {
            public CustomPopupMenuBarControl(BarManager manager, CustomPopupMenu customPopupMenu)
                : base(manager, customPopupMenu) { }

            bool popupFormIsReady;
            public override void OpenPopup(LocationInfo locInfo, IPopup parentPopup)
            {
                popupFormIsReady = false;
                base.OpenPopup(locInfo, parentPopup);

                var desiredLocation = (Menu as CustomPopupMenu).BottomRightCorner;
                desiredLocation.X -= Form.Width;
                Form.Location = desiredLocation;
                popupFormIsReady = true;
                ShowForm();
            }

            protected override void ShowForm()
            {
                if (popupFormIsReady)
                    base.ShowForm();
            }
        }

    }
}