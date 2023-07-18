using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using SmartAssembly.Attributes;
using SRUL.Types;
using SRUL.UnitTracker;
using TB.ComponentModel;

namespace SRUL
{
    public class SRViews : IDisposable
    {
        // Properties
        public Dictionary<string?, BarToggleSwitchItem> SubCategoryToggleItemDictionary = new Dictionary<string?, BarToggleSwitchItem>();
        public Dictionary<string, bool> SubCategoryToggleCheckedDictionary = new Dictionary<string, bool>();
        private bool disposed;
        private static Lazy<SRViews> _SRViews = null;
        private MainForm _mainForm { get; set; } = null;
        
        public static SRViews CreateInstance()
        {
            if (_SRViews == null)
                _SRViews = new Lazy<SRViews>(() => new SRViews());
            return _SRViews.Value;
        }
        
        public static SRViews Instance
        {
            get
            {
                if (_SRViews == null)
                {
                    XtraMessageBox.Show("Views need to be instantiated");
                }
                return _SRViews.Value;
            }
        }

        //constructor 
        public SRViews()
        {
            //initialize the views
        }

        public void InitializeView(MainForm mainForm)
        {
            if(_mainForm == null)
                _mainForm = mainForm;
            PopulateMenuVisibilityToogles(_mainForm.Settings_SubCategoryVisibility);
            // InitInfo();
        }
        
        void SetGridIdToEveryRow(GridView view)
        {
            for (int i = 0; i < view.DataRowCount; i++)
            {
                if (view.GetRowCellValue(i, "gridId") != i.To<object>())
                    view.SetRowCellValue(i, view.Columns["gridId"], i.To<object>());
            }
        }

        public void InitializeGridViews(GridView[] views)
        {
            foreach (var view in views)
            {
                if(view.Columns.Count < 1) continue;
                if(view.Columns.ColumnByFieldName("freeze") != null) 
                    view.Columns["freeze"].Caption = "Lock";
                if(view.Columns.ColumnByFieldName("displayName") != null) 
                    view.Columns["displayName"].Caption = "Feature";
                if(view.Columns.ColumnByFieldName("value") != null) 
                    view.Columns["value"].Caption = "Value";
                if(view.Columns.ColumnByFieldName("original") != null) 
                    view.Columns["original"].Caption = "Default";

                view.OptionsMenu.ShowConditionalFormattingItem = true;
                view.OptionsMenu.EnableColumnMenu = false;
                view.OptionsMenu.EnableFooterMenu = false;
                view.OptionsCustomization.AllowSort = false;
                
                view.OptionsView.AllowHtmlDrawGroups = true;
                view.OptionsView.ColumnAutoWidth = true;
                view.OptionsView.ShowIndicator = false;
                view.OptionsView.ShowAutoFilterRow = false;
                view.OptionsView.ShowPreview = false;
                view.OptionsView.AutoCalcPreviewLineCount = true;
                view.OptionsView.ShowVerticalLines = DefaultBoolean.True;

                view.OptionsFilter.InHeaderSearchMode = GridInHeaderSearchMode.Default;
                view.OptionsFilter.ShowInHeaderSearchResults = ShowInHeaderSearchResultsMode.None;
                view.OptionsDetail.SmartDetailExpandButtonMode = DetailExpandButtonMode.AlwaysEnabled;
                view.OptionsSelection.MultiSelect = true;
            
                // TODO: Grouping Fix
                view.Columns[Properties.Settings.Default.ResourceGroupByColumn].Group();
                view.ExpandAllGroups();
            
                // Set GridId to every row cell;
                SetGridIdToEveryRow(view);

                // Create Timer
                view.Appearance.Preview.ForeColor = Color.LightGray;
                view.PreviewFieldName = "description";
                
                view.BestFitColumns();
                
                // FOR DEBUG
                // if(view.Columns.ColumnByFieldName("BuildedAddress") != null) 
                //     view.Columns["BuildedAddress"].Visible = true;
                // view.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            }
        }

        public void gvRowFilterShownOnlyIncludedFeature(GridView gv, Category category)
        {
            string[] arr = new[]
            {
                "SUBRESOURCES_STOCK",
                "SUBRESOURCES_DEMANDS",
                "SUBRESOURCES_ACTUALUSE",
                "SUBRESOURCES_PRODUCTION",
                "SUBRESOURCES_PRODUCTIONCAPACITY",
                "SUBRESOURCES_PRODUCTIONCOST",
                "SUBRESOURCES_MARKETPRICE",
                "SUBRESOURCES_SPECIAL",
                "SUBRESOURCES_MARGIN",
                "SUBRESOURCES_BASECOST",
                "SUBRESOURCES_FULLCOST",
                "SUBRESOURCES_MAXDEMAND",
                "SUBRESOURCES_MINDEMAND",
                "SUBRESOURCES_PRODUCEDFROM",
                "SUBRESOURCES_HEXMULTIPLIER"
            };
            // Create customColumnSort
            // void gvCustomColumnSort(object sender, CustomColumnSortEventArgs e)
            // {
            //     if (e.Column.FieldName == "subCategory")
            //     {
            //         string val1 = e.Value1.ToString();
            //         string val2 = e.Value2.ToString();
            //         foreach (var t in category.subCategories)
            //         {
            //             if (val1 == val2) e.Result = 0;
            //             else
            //             {
            //                 if (val1 == t.categoryName) 
            //                     e.Result = -1;
            //                 if (val2 == t.categoryName)
            //                     e.Result = 1;
            //             }
            //         }
            //         e.Handled = true;
            //     }
            // }
            // gv.CustomColumnSort += gvCustomColumnSort;

            void exclusion(object sender, RowFilterEventArgs e)
            {
                GridView view = (GridView)sender;
                string recordName = view.GetListSourceRowCellValue(e.ListSourceRow, "name").ToString();
                bool recordVisibility = (bool)view.GetListSourceRowCellValue(e.ListSourceRow, "visible");
                string? recordSubzero = view.GetListSourceRowCellValue(e.ListSourceRow, "subCategory")?.ToString();
                var feat = view.GetRow(e.ListSourceRow) as Feature;

                if (recordSubzero == null || recordSubzero == String.Empty)
                {
                    e.Visible = false;
                    e.Handled = true;
                    return;
                }

                if (!SubCategoryToggleItemDictionary.ContainsKey(recordSubzero)) return;
                if (ListOfSortedRow.SRGridIncludedFeatures.Contains(recordName))
                    e.Visible = SubCategoryToggleItemDictionary[recordSubzero].Checked;
                
                if (ListOfSortedRow.ResourcesIncludedFeatureList.Contains(recordName))
                {
                    if (feat != null) e.Visible = feat.visible;
                    e.Handled = true;
                    return;
                }
                e.Handled = true;
                
            }
            gv.CustomRowFilter += exclusion;
            //expanding all

            
            // GriControl to show image in subactegory column
            void gcCustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs args)
            {
                var gvv = sender as GridView;
                var info = args.Info as GridGroupRowInfo;
                if (info.EditValue == null) return;
                // int quantity = Convert.ToInt32(gvv.GetGroupRowValue(args.RowHandle, info.Column));

                // var test = "0";
                // if(info.Column.FieldName == "name")
                    
                if (info.Column.FieldName == "subCategory")
                {
                    if (gvv.GetGroupRowValue(args.RowHandle, info.Column) != null)
                    {
                        if (ListOfSortedRow.ResourceIconNames.Contains(info.GroupValueText))
                        {
                            try
                            {
                                var r = info.Bounds;
                                foreach (var icon in ListOfSortedRow.ResourceIconNames)
                                {
                                    if (icon != info.GroupValueText) continue;
                                    var test = gvv.GetRowCellValue(gvv.GetDataRowHandleByGroupRowHandle(args.RowHandle), "value");
                                    // var test = SRMain.Instance.FeatureIndexedStore[$"Stock{icon.Replace(" ", "")}"]?.value;
                                    r.X += info.Bounds.Height + 5 * 2;
                                    r.Width -= (info.Bounds.Height + 5 * 2);
                                    info.Appearance.BackColor = Color.Black;
                                    info.Cache.DrawImage(
                                        Properties.Resources.ResourceManager.GetObject(icon) as Image, 
                                        SRMain.Instance.DefinedRectangle(info.Bounds.X + 5, info.Bounds.Y, info.Bounds.Height - 2, info.Bounds.Height - 2));
                                    args.Appearance.BackColor = Color.Black;
                                    info.Appearance.DrawString(args.Cache, 
                                        string.Format("{0} - (Stock: {1})", info.GroupValueText, test != null ? test : ""), r);
                                }
                            }
                            finally
                            { 
                                args.Handled = true;
                            }
                        }
                        else
                        {
                                info.Appearance.BackColor = Color.Black;
                            info.GroupText = info.GroupValueText;
                        }
                    }
                    // info.GroupText = gvv.GetGroupRowValue(args.RowHandle, info.Column).ToString();
                    // ListOfSortedRow.ResourceIconNames.Contains(info.GroupValueText);
                    // info.GroupText = info.GroupValueText;
                    // info.Appearance.Image = Resource
                    // info.GroupText = info.GroupValueText;
                        // info.GroupText = gvv.GetGroupRowValue(args.RowHandle, info.Column)?.ToString();
                }   
            }

            gv.PopupMenuShowing += OnPopupMenuShowing;
            gv.CustomDrawGroupRow += gcCustomDrawGroupRow;
        }
        
         public void OnPopupMenuShowing(object sender, PopupMenuShowingEventArgs args)
        {
            GridView view = sender as GridView;
            if (args.MenuType == GridMenuType.Row)
            {
                // GridHitInfo hitInfo = view.CalcHitInfo(args.Point);
                int rowHandle = args.HitInfo.RowHandle;
                var tag = "";
                DXPopupMenu menu = args.Menu;
                menu.Appearance.Options.UseImage = false;
                menu.Appearance.Font = new Font(FontFamily.GenericMonospace, 2);
                menu.Items.Clear();
                
                //TODO: ugh hardcoded
                // Resources
                if (view.Name == "gvResources")
                    menu.Items.Add(CreateResourceSubMenuItem($"Resources", view));

                switch (view.Name)
                {
                    case "gvResources": 
                        menu.Items.Add(CreateDXSubMenuItem("Go To", view, "Resources"));
                        break;
                    case "gvWarfare": 
                        menu.Items.Add(CreateDXSubMenuItem("Go To", view, "Warfare"));
                        break;
                    case "gvCountry": 
                        menu.Items.Add(CreateDXSubMenuItem("Go To", view, "Country"));
                        break;
                    case "gvFacility": 
                        menu.Items.Add(CreateDXSubMenuItem("Go To", view, "Special"));
                        break;
                }
                menu.Items.Add(CreateGridMenuItem("Expand All Group", view, "", 0, MenuExpandGroup));
                menu.Items.Add(CreateGridMenuItem("Collapse All Group", view, "", 0, MenuCollapseGroup));
            }
        }
         void ShowResourceGridRow(object sender, EventArgs args)
         {
             DXMenuCheckItem item = sender as DXMenuCheckItem;
             RowInfo info = item.Tag as RowInfo;
             string tag = info.OptionalData;
             string newTag = tag;
             info.View.BeginUpdate();
             foreach (var feat in ListOfSortedRow.ResourcesIncludedFeatureList)
             {
                 if (feat.GetFeature().displayName.Equals(newTag))
                 {
                     ListOfSortedRow.resourceRowsVisibleState[newTag] = item.Checked;
                     info.OptionalState = ListOfSortedRow.resourceRowsVisibleState[newTag];
                     feat.GetFeature().visible = item.Checked;
                 }
             }
             info.View.RefreshData();
             info.View.EndUpdate();
         }
         
         // TODO: Refactor, decoupling, SOC
         private DXMenuItem CreateResourceSubMenuItem(string headerCaption, GridView view)
         {
             DXSubMenuItem subItem = new DXSubMenuItem(headerCaption);
             DXSubMenuItem subItemVisibility = new DXSubMenuItem("Row Visibility");
             // DXSubMenuItem subItemGroups = new DXSubMenuItem("Groups");
             foreach (var rMember in ListOfSortedRow.ResourceSubcategoryMemberListDisplayName)
                 subItemVisibility.Items.Add(CreateGridMenuCheckItem($"Show {rMember}", view, rMember, ShowResourceGridRow));
             subItem.Items.Add(subItemVisibility);
             return subItem;
         }

         private DXMenuItem CreateDXSubMenuItem(string headerCaption, GridView view, string category)
         {
             DXSubMenuItem subItem = new DXSubMenuItem(headerCaption);
             IList<Feature>? feat = view.DataSource as List<Feature>;
             if (feat == null) return subItem;

             for (int i = 0; i < SRMain.Instance.DataCategories.Count; i++)
             {
                 if(SRMain.Instance.DataCategories[i].category == category)
                     for (int j = 0; j < SRMain.Instance.DataCategories[i].subCategories.Count; j++)
                     {
                         var subcategory = SRMain.Instance.DataCategories[i].subCategories[j];
                         for (int k = 0; k < view.RowCount; k++)
                         {
                             if (view.GetRowCellValue(k, "subCategory") == subcategory.categoryName)
                             { 
                                 subItem.Items.Add(CreateGridMenuItem($"{subcategory.categoryDisplayName}", view, "", k, MenuGoToSubCategory));
                                 break;
                             }
                         }
                     }
             }
             return subItem;
         }
         
         private void MenuGoToSubCategory(object sender, EventArgs e)
         {
             DXMenuItem? view = sender as DXMenuItem;
             if (view == null) return;
             RowInfo? info = view.Tag as RowInfo;
             if (info == null) return;
             info.View.MakeRowVisible(info.RowHandle);
         }
         private void MenuCollapseGroup(object sender, EventArgs e)
         {
             DXMenuItem view = sender as DXMenuItem;
             RowInfo info = view.Tag as RowInfo;
             info.View.CollapseAllGroups();
         }
         
         private void MenuExpandGroup(object sender, EventArgs e)
         {
             DXMenuItem view = sender as DXMenuItem;
             RowInfo info = view.Tag as RowInfo;
             info.View.ExpandAllGroups();
         }
         
         private DXMenuCheckItem CreateGridMenuCheckItem(string text, GridView view, string tag, EventHandler evt)
        {
            DXMenuCheckItem item = new DXMenuCheckItem(text, ListOfSortedRow.resourceRowsVisibleState[tag], null, evt);
            RowInfo info = new RowInfo(view, 0);
            item.CloseMenuOnClick = false;
            info.OptionalState = ListOfSortedRow.resourceRowsVisibleState[tag];
            info.OptionalData = tag;
            item.Tag = info;
            return item;
        }

         private DXMenuItem CreateGridMenuItem(string caption, GridView view, string tag, int rowHandle, EventHandler evt)
         {
             DXMenuItem item = new DXMenuItem(caption, evt);
             RowInfo info = new RowInfo(view, rowHandle);
             info.OptionalData = tag;
             item.Tag = info;
             return item;
         }

        // TODO: Separation of concerns - this is the wrong place for this function.
        public void PopulateMenuVisibilityToogles(BarSubItem subMenu)
        {
            var subCategoryNames = SRMain
                .Instance
                .DataSubCategories
                .Select(s => s.categoryName).ToArray();

            // List<string> subCategoriesList = new List<string>{ };
            foreach (var subCategoryName in subCategoryNames)
            {
                if (SubCategoryToggleCheckedDictionary.ContainsKey(subCategoryName)) return;
                SubCategoryToggleCheckedDictionary.Add(subCategoryName, true);
            }
            
            foreach (var category in SRMain.Instance.DataVersion.Categories)
            {
                var headerItem = new BarHeaderItem { Caption = category.category };
                headerItem.Caption = $@"{category.category} - Visibility";
                subMenu.AddItem(headerItem);
                
                foreach (var subCategory in category.subCategories)
                {
                    var toggleItem = new BarToggleSwitchItem
                    {
                        Caption = subCategory.categoryDisplayName,
                        Name = subCategory.categoryName,
                        Checked = subCategory.categoryVisibility
                    };
                    toggleItem.CheckedChanged += (sender, args) =>
                    {
                        var item = sender as BarToggleSwitchItem;
                        SubCategoryToggleCheckedDictionary[item.Name] = item.Checked;
                        _mainForm.gvCountry.RefreshData();
                        _mainForm.gvResources.RefreshData();
                        _mainForm.gvWarfare.RefreshData();
                        _mainForm.gvFacility.RefreshData();
                    };
                    SubCategoryToggleItemDictionary.Add(subCategory.categoryName, toggleItem);
                    subMenu.AddItem(toggleItem);
                }
                // (subCategoriesList).Add(subCategory.categoryName);
                // subCategories.listsubCategory.categoryName
            }
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
            SubCategoryToggleItemDictionary.Clear();
            SubCategoryToggleCheckedDictionary.Clear();
            _mainForm = null;
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                _mainForm?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SRViews()
        {
            Dispose(false);
        }
    }
}   