using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using SmartAssembly.Attributes;
using SRUL.Annotations;
using SRUL.Types;

namespace SRUL
{
    [ForceObfuscate(true)]
    public sealed class SRMain : IDisposable, INotifyPropertyChanged
    {
        private readonly Array _sortedList;
        public Root Data { get; set; }
        internal ActiveTrainer activeTrainer { get; set; } = ActiveTrainer.Instance;
        private IList<Game> DataGames { get; set; }
        private Game DataGame { get; set; }
        internal Version DataVersion { get; set; }
        private IList<Pointer> DataPointer { get; set; }
        internal Category DataCountry { get; set; }
        internal Category DataResources { get; set; }
        internal Category DataWarfare { get; set; }
        internal Category DataSpecial { get; set; }

        internal IList<SubCategory> DataSubCategories { get; set; }

        public IList<Category> DataCategories { get; set; }
        public IList<Feature> FeaturesCountry { get; private set; }
        public IList<Feature> FeaturesResources { get; private set; }
        public IList<Feature> FeaturesWarfare { get; private set; }
        public IList<Feature> FeaturesSpecial { get; private set; }
        
        public Dictionary<string, SubCategory> SubCategoriesIndexedStore;
        // To FIX DPA we just init what needed, so doesnt have to process the pointer store
        private IList<Feature> Features;
        public Dictionary<string, string>? FeaturePointerStore;
        public Dictionary<string, string>? FeaturePointerStoreRaw;
        public Dictionary<string, string>? FeaturePointerStoreUintPTR;
        public Dictionary<string, int> FeatureIndexStore;
        public Dictionary<string, Feature> FeatureIndexedStore;
        
        public Dictionary<string, Feature> WarfareIndexedFeatures;
        public Dictionary<string, Feature> ResourcesIndexedFeatures;
        public Dictionary<string, Feature> CountryIndexedFeatures;
        public Dictionary<string, Feature> SpecialIndexedFeatures;
        public bool FeatureArmyEnemyEnabled;
        public bool IsInDefenseEditorMode { get; set; } = false;

        // To Vaoid Allocation
        private Feature _tempFeature;
        
        // 

        private static Lazy<SRMain> JsonReader = null;
        // dynamic stuff = JObject.Load()
        private SRMain(Root rt)
        {
            activeTrainer = ActiveTrainer.Instance;
            // activeTrainer.GameValidated = false;
            Load(rt);
            // CreateDataSourceFromWeb();
        }

        public static SRMain CreateSingleton(Root rt)
        {
            if (JsonReader != null)
            {
                // MessageBox.Show("Reader have been instantiated");
                Debug.WriteLine("Reader have been instantiated");

            }
            JsonReader = new Lazy<SRMain>(() => new SRMain(rt));
            return JsonReader.Value;
        }
        
        public static SRMain Instance
        {
            get
            {
                if (JsonReader == null)
                {
                    Debug.WriteLine("Reader need to be instantiated");
                    // MessageBox.Show("Reader need to be instantiated");
                }
                return JsonReader.Value;
            }
        }

        public void SetOriginalValueToCurrentValue()
        {
            // if(ListOfSortedRow.WarfareIncludedFeatureList.Contains())
                foreach (var feature in FeaturesWarfare)
                {
                    feature.original = feature.value;
                }
        }

        public void dgOrder(GridView dgv, Category category)
        {
           
            foreach (GridColumn column in dgv.Columns)
            {
                if (category.categoryShownColumns.Any(x => x.columnName == column.FieldName))
                {
                    var categoryColumn = category.categoryShownColumns.FirstOrDefault(x => x.columnName == column.FieldName);
                    dgv.Columns[categoryColumn.columnName].Visible = true;

                    if(categoryColumn.columnMinWidth != 0) 
                        dgv.Columns[categoryColumn.columnName].MinWidth = categoryColumn.columnMinWidth;
                    if (categoryColumn.columnMaxWidth != 0) 
                        dgv.Columns[categoryColumn.columnName].MaxWidth = categoryColumn.columnMaxWidth;
                    dgv.Columns[categoryColumn.columnName].OptionsColumn.AllowEdit = categoryColumn.columnAllowEdit;
                    dgv.Columns[categoryColumn.columnName].OptionsFilter.AllowFilter = false;
                }
                else
                if (category.categoryShownColumns.All(x => x.columnName != column.FieldName))
                {
                    dgv.Columns[column.FieldName].Visible = false;
                }
                
                // dgv.BeginUpdate();
                // dgv.EndUpdate();
            }
            
            
            // For debug
            // if(dgv.Columns.ColumnByFieldName("BuildedAddress") != null) 
            //     dgv.Columns["BuildedAddress"].Visible = true;

            
            // GridFormatRule Highlight(string ruleName,
            //     string iconName,
            //     string colorFill,
            //     FormatConditionDataUpdateTrigger trigger)
            // {
            //     GridFormatRule gridFormatRule = new GridFormatRule();
            //     FormatConditionRuleDataUpdate dataUpdate = new FormatConditionRuleDataUpdate();
            //
            //     dataUpdate.HighlightTime = 500;
            //     dataUpdate.Icon.PredefinedName = iconName;
            //     dataUpdate.PredefinedName = colorFill;
            //     dataUpdate.Trigger = trigger;
            //     dataUpdate.AllowAnimation = DefaultBoolean.True;
            //
            //     gridFormatRule.Name = ruleName;
            //     gridFormatRule.Rule = dataUpdate;
            //     gridFormatRule.ApplyToRow = true;
            //     gridFormatRule.ColumnApplyTo = gridFormatRule.Column = dgv.Columns.ColumnByFieldName("value");
            //
            //     return gridFormatRule;
            // }
            // dgv.BeginUpdate();
            //
            // // foreach (GridColumn gvColumn in gv.Columns)
            // // {
            // //     gridFormatRule.Column = gvColumn;
            // // }
            // // gridFormatRule.Column = "value";
            // dgv.FormatConditions.BeginUpdate();
            // dgv.FormatRules.Add(Highlight("FormatIncreased",
            //     "Arrows3_1.png",
            //     "Green Fill",
            //     FormatConditionDataUpdateTrigger.ValueIncreased));
            // dgv.FormatRules.Add(Highlight("FormatDecreased",
            //     "Arrows3_3.png",
            //     "Red Fill",
            //     FormatConditionDataUpdateTrigger.ValueDecreased));
            // dgv.FormatConditions.EndUpdate();
            // dgv.GroupSummary.Add(new GridGroupSummaryItem(SummaryItemType.Max, 
            //     "id", 
            //     null,
            //     "{0}"));
            // dgv.EndUpdate();
        }

        public void setReadOnlyRow(CustomRowCellEditEventArgs e)
        {
            var repoReadOnly = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            repoReadOnly.Name = "repoTextEditReadOnly";
            repoReadOnly.ReadOnly = true;
            e.RepositoryItem = repoReadOnly;
        }

        private void UpdateRow(ColumnView view)
        {
            // Obtain the Price column. 
            var col = view.Columns.ColumnByFieldName("Price");
            if (col == null) return;
            view.BeginSort();
            try
            {
                // Obtain the number of data rows. 
                int dataRowCount = view.DataRowCount;
                // Traverse data rows and change the Price field values. 
                for (int i = 0; i < dataRowCount; i++)
                {
                    object cellValue = view.GetRowCellValue(i, col);
                    double newValue = Convert.ToDouble(cellValue) * 0.9;
                    view.SetRowCellValue(i, col, newValue);
                }
            }
            finally { view.EndSort(); }
        }

        public bool SelectedTrainer(string gn, string gv)
        {
            activeTrainer.GameName = gn;
            activeTrainer.GameVersion = gv;
            if (DataGames.Any(s => s.DisplayName == gn))
            {
                activeTrainer.GameValidated = true;
                Load();
                return activeTrainer.GameValidated;
            }
            activeTrainer.GameValidated = false;
            return activeTrainer.GameValidated;
        }
        // to sort after deserialization
        private List<Feature> gvSortList(IList<Feature> feature, string[] sortedList)
        {
            return feature.OrderBy(x =>
                {
                    var index = Array.IndexOf(sortedList, x.name);
                    return index < 0 ? int.MaxValue : index;
                }).ToList();
        }

        // Deprecated, will be legacy soon
        public void gvRowFilterExclusion(object sender, RowFilterEventArgs e)
        {
            GridView view = (GridView) sender;
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
        
        // Main Data Builder, after SRLoader showing trainer;
        public void Load(Root rt = null, SRReadWrite rw = null)
        {
            // const string fileName = @"R:\PROJECT\VS17\SRUL\SRUL\Transformer\api\SRFeature.json";
            // Data = JsonConvert.DeserializeObject<Root>(File.ReadAllText(fileName));
            if (rt != null)
            {
                Data = rt;
                DataGames = Data.Games;
            }

            if (activeTrainer.GameProcess != null)
            {
                // NO LINQbchz

                try
                {
                    foreach (var game in Data.Games)
                    {
                        if(game.DisplayName == activeTrainer.GameName) {
                            DataGame = game;
                            break;
                        }
                    }
                    foreach (var version in DataGame.Versions)
                    {
                        if(!(version.Availability)) continue; 
                        if (version.GameVersion == activeTrainer.GameVersion) {
                            DataVersion = version; 
                            break; }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Main Data can't be mapped", e);
                }
                // DataGame = Data.Games.FirstOrDefault(s => s.DisplayName == ActiveTrainer.Instance.GameName);
                // DataVersion = DataGame.Versions.First(s => s.Availability && s.GameVersion == activeTrainer.GameVersion);
                DataCategories = DataVersion.Categories;

                // Naming convention for the data Categorized Features;
                DataCountry = DataCategories[0];
                DataResources = DataCategories[1];
                DataWarfare = DataCategories[2];
                DataSpecial = DataCategories[3];

                
                // Create Experimental Schema From current units then calculate new use new pointer to point out.
                // CreateMissileSchema();
                
                // Populate Subcategories
                try
                {
                    DataSubCategories = DataCountry
                        .subCategories
                        .Concat(DataResources.subCategories)
                        .Concat(DataWarfare.subCategories)
                        .Concat(DataSpecial.subCategories)
                        .ToList();

                    // Make subcategory Cache
                    // SubCategoriesIndexedStore = DataSubCategories.ToDictionary
                    //     <SubCategory, string, SubCategory>(category => category.categoryName, category => category);
                    SubCategoriesIndexedStore = new Dictionary<string, SubCategory>(DataSubCategories.Count);
                    foreach (var subCategory in DataSubCategories)
                    {
                        SubCategoriesIndexedStore.Add(subCategory.categoryName, subCategory);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("SubCategories can't be mapped", e);
                }

                // if (DataCountry.rowOrders != null)
                //     ListOfSortedRow.CountryIncludedFeatureList = DataCountry.rowOrders;
                // if (DataWarfare.rowOrders != null)
                //     ListOfSortedRow.WarfareIncludedFeatureList = DataWarfare.rowOrders;
                // if (DataWarfare.rowExclusion != null)
                //     ListOfSortedRow.warfareExcludedInDataView = DataWarfare.rowExclusion;
                // if (DataSpecial.rowOrders != null)
                //     ListOfSortedRow.facilityRowOrderList = DataSpecial.rowOrders;
                // if (DataSpecial.rowExclusion != null && DataSpecial.subCategories.Count > 0)
                //     ListOfSortedRow.FacilityIncludedFeatureList = 
                //         DataSpecial.subCategories[0].categoryIncludedFeatures
                //             .Union(DataSpecial.subCategories[1].categoryIncludedFeatures)
                //             .Union(DataSpecial.subCategories[1].categoryIncludedFeatures)
                //             .Union(DataSpecial.subCategories[1].categoryIncludedFeatures).ToArray();

                // Set Included Feature List.
                ListOfSortedRow.CountryIncludedFeatureList = ListOfSortedRow
                    .CountryIncludedFeatureList.GetIncludedFeatures(DataCountry);
                ListOfSortedRow.ResourcesIncludedFeatureList = ListOfSortedRow
                    .ResourcesIncludedFeatureList.GetIncludedFeatures(DataResources);
                ListOfSortedRow.WarfareIncludedFeatureList = ListOfSortedRow
                    .WarfareIncludedFeatureList.GetIncludedFeatures(DataWarfare);
                ListOfSortedRow.FacilityIncludedFeatureList = ListOfSortedRow
                    .FacilityIncludedFeatureList.GetIncludedFeatures(DataSpecial);

                ListOfSortedRow.SRGridIncludedFeatures = ListOfSortedRow.CountryIncludedFeatureList
                    .Concat(ListOfSortedRow.WarfareIncludedFeatureList)
                    .Concat(ListOfSortedRow.ResourcesIncludedFeatureList)
                    .Concat(ListOfSortedRow.FacilityIncludedFeatureList)
                    .Distinct()
                    .ToArray();

                ListOfSortedRow.CountryExcludedFeatureList = ListOfSortedRow
                    .CountryExcludedFeatureList.GetSetExcludedFeatures(DataCountry, ListOfSortedRow.SRGridIncludedFeatures);
                ListOfSortedRow.ResourcesExcludedFeatureList = ListOfSortedRow
                    .ResourcesExcludedFeatureList.GetSetExcludedFeatures(DataResources, ListOfSortedRow.SRGridIncludedFeatures);
                ListOfSortedRow.WarfareExcludedFeatureList = ListOfSortedRow
                    .WarfareExcludedFeatureList.GetSetExcludedFeatures(DataWarfare, ListOfSortedRow.SRGridIncludedFeatures);
                ListOfSortedRow.FacilityExcludedFeatureList = ListOfSortedRow
                    .FacilityExcludedFeatureList.GetSetExcludedFeatures(DataSpecial, ListOfSortedRow.SRGridIncludedFeatures);

                ListOfSortedRow.SRGridExcludedFeatures = ListOfSortedRow.WarfareIncludedFeatureList
                    .Concat(ListOfSortedRow.ResourcesIncludedFeatureList)
                    .Concat(ListOfSortedRow.CountryIncludedFeatureList)
                    .Concat(ListOfSortedRow.FacilityIncludedFeatureList)
                    .Distinct()
                    .ToArray();


                // For show hide, populate resources visibility dictionary
                foreach (var displayName in ListOfSortedRow.ResourceSubcategoryMemberListDisplayName)
                {
                    if(!ListOfSortedRow.resourceRowsVisibleState.ContainsKey(displayName))
                        ListOfSortedRow.resourceRowsVisibleState.Add($"{displayName}", true);
                }

                // Sorting list and return the FeatureData after
                FeaturesCountry = gvSortList(DataCountry.features, ListOfSortedRow.CountryIncludedFeatureList);
                FeaturesWarfare = gvSortList(DataWarfare.features, ListOfSortedRow.WarfareIncludedFeatureList);
                FeaturesSpecial = gvSortList(DataSpecial.features, ListOfSortedRow.FacilityIncludedFeatureList);
                
                // Map Sub-Category Into Feature Fields
                MapSubCategoryIntoFeatures(DataCountry);
                MapSubCategoryIntoFeatures(DataResources);
                MapSubCategoryIntoFeatures(DataWarfare);
                MapSubCategoryIntoFeatures(DataSpecial);
                
                FeaturesResources = DataResources.features.OrderBy(f => f.subCategory).ToList();


                // TODO: Part 1, to make a blog, how i managed to get rid of 8000mb Small object heap DPA analysis
                // Set capacity for predicted size
                int capacity = FeaturesCountry.Count;
                capacity += FeaturesResources.Count;
                capacity += FeaturesWarfare.Count;
                capacity += FeaturesSpecial.Count;
                
                // Join Every features in category into 1 single List.

                try
                {
                    Features = new List<Feature>(capacity);
                    Features = FeaturesCountry
                        .Concat(FeaturesResources)
                        .Concat(FeaturesWarfare)
                        .Concat(FeaturesSpecial)
                        .ToList();
                    foreach (var feature in Features)
                    {
                        feature.BuildAddress(DataVersion.Pointers);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw new Exception("Features can't be mapped", e);
                }

                // Set WIKI fields.
                ExtractURLFromFeatureDescriptionProcedure(Features);
                
                FeaturePointerStore = new Dictionary<string, string>(Features.Count);
                FeaturePointerStoreRaw = new Dictionary<string, string>(Features.Count);
                FeatureIndexStore = new Dictionary<string, int>(Features.Count);
                FeatureIndexedStore = new Dictionary<string, Feature>(Features.Count);

                WarfareIndexedFeatures = new Dictionary<string, Feature?>(FeaturesWarfare.Count);
                SpecialIndexedFeatures = new Dictionary<string, Feature?>(FeaturesSpecial.Count);
                ResourcesIndexedFeatures = new Dictionary<string, Feature?>(FeaturesResources.Count);
                CountryIndexedFeatures = new Dictionary<string, Feature?>(FeaturesCountry.Count);
                

                // Creating pointer store with predicted size.
                DataPointer = DataVersion.Pointers;
                
                FeaturePointerStore = PointerDictionaryMapper(rw, Features);
            }
        }

        public void ResourceSwapColumnGroup(GridView gv)
        {
            string[] newName =
            {
                "Stock",
                "Demands",
                "ActualUse",
                "Production",
                "ProductionCapacity",
                "ProductionCost",
                "MarketPrice",
                "Margin",
                "BaseCost",
                "FullCost",
                "NodeProduction",
                "CityProduction",
                "MaxDemand",
                "MinDemand",
            }; // 14
            // var results = from f in features
            //     group p.car by p.PersonId into g
            //     select new { PersonId = g.Key, Cars = g.ToList() };
            // var groupedFeatures = FeaturesResources.GroupBy(f => f.displayName);
            // Replace feature displayName with groupedFeatures Key
            // var featureNameList = from f in FeaturesResources 
            //     group f by f.displayName into g
            //     select new { displayName = g.Key, features = g.ToList() };
            // string pattern = @"(Stock|Demand|Production|ActualUse|ProductionCapacity|)";
            // RegexOptions options = RegexOptions.Singleline; Match m = Regex.Match(input, pattern, options);
            // list.RemoveRange(list.Count - 2, list.Count - 1);
            string pattern = @"(.*)\[url\](.*)\[\/url\](.*)";
            Regex regex = new Regex(pattern);
                var Ficer = SystemExtension.SkipLast(FeaturesResources, 2);
            try
            {
                foreach (var (value, i) in Ficer.Select((value, i) => (value, i)))
                {
                    for (int j = 0; j < newName.Length; j++)
                    {
                        if (value.name.Contains(newName[j]))
                        {
                            value.subCategory = value.displayName;
                            value.displayName = string.Format("{0}{1}", newName[j], value.displayName);
                        }
                    }
                }
            }
            finally
            {
                Ficer = null;
                newName = null;
                // do nothing;
            }

            // // replace feature displayName in groupedfeature displayName with new name
            // foreach (var group in groupedFeatures)
            // {
            //     var snewName = group.Key.Replace("Resources", "Resources_");
            //     foreach (var feature in group)
            //     {
            //         feature.displayName = snewName;
            //     }
            // }
            // // Add prefix to feature displayName
            // foreach (var feature in FeaturesResources)
            // {
            //     feature.displayName = "Resources" + feature.displayName;
            // }
            
        }
        
        Rectangle _rect = new Rectangle();
        public Rectangle DefinedRectangle(int X, int Y, int Width, int Height)
        {
            _rect.X = X;
            _rect.Y = Y;
            _rect.Width = Width;
            _rect.Height = Height;
            return _rect;
        }

        private Dictionary<int, string> _temp;
        public Dictionary<int, string> GetFormatData(Feature feature, int formatIndex = 0)
        {
            Dictionary<int, string> temp = new Dictionary<int, string>();
            string[] formatCollection = { "" };
            foreach (var formatType in DataGame.SRFormatTypes)
            {
                if (feature.format.Contains(","))
                    formatCollection = feature.format.Split(',');
                else
                    formatCollection = new []{feature.format};
                if (!String.Equals(formatType.FormatName, formatCollection[formatIndex], StringComparison.CurrentCultureIgnoreCase)) continue;
                foreach (var s in formatType.FormatData)
                {
                    temp.Add(Convert.ToInt32(s[1]), s[0]);
                }
            }

            return temp;
        }
        private void ExtractURLFromFeatureDescriptionProcedure(IList<Feature> featureListToBeMutated)
        {
            // Regex with this pattern will match the following: "[url]https://www.google.com[/url]"
            string pattern = @"(.*)\[url\](.*)\[\/url\](.*)";
            Regex regex = new Regex(pattern);
            foreach (var feature in featureListToBeMutated)
            {
                if (regex.IsMatch(feature.description))
                {
                    feature.wiki = $"<a href='{regex.Match(feature.description).Result("$2")}'>[WIKI]</a>";
                    feature.description = regex.Replace(feature.description, "$1$3");
                }
            } 
        }

        private string BuildAPointerString(SRReadWrite memoryReaderWriter, string pointer, string offset, bool calculateAddress)
        {
            var _newOffset = string.IsNullOrEmpty(offset) ? "" : $",{offset}";
            return calculateAddress 
                ? memoryReaderWriter.GetCode($"base+{pointer}{_newOffset}").ToUInt32().ToString("X") 
                : ($"base+{pointer}{_newOffset}");
        }
        private UIntPtr BuildAPointerString(SRReadWrite memoryReaderWriter, string pointer, string offset)
        {
            var _newOffset = string.IsNullOrEmpty(offset) ? "" : $",{offset}";
            return memoryReaderWriter.GetCode($"base+{pointer}{_newOffset}");
        }
        
        // Iterate, build pointer, and store it into respective its dictionary 
        private Dictionary<string, string> PointerDictionaryMapper(SRReadWrite memoryReaderWriter, IList<Feature> features)
        {
            string buildedPointer;
            UIntPtr ptrPointer;
            
            var localDictionary = new Dictionary<string, string>(features.Count);

            //Loop thru feature list to build the pointer based on pointerID.
            for (int i = 0; i < features.Count; i++)
            {
                // Get Pointer From DAta
                var pointer = DataPointer[features[i].pointerId-1].pointer;
                // Build pointer String
                buildedPointer = BuildAPointerString(memoryReaderWriter, pointer, features[i].offset, false);
                // Generate Real Pointer Address to be inserted in Dictionary
                if (features[i].category.ToLower() is not "warfare" & features[i].category.ToLower() != "special")
                    localDictionary.Add(features[i].name, BuildAPointerString(memoryReaderWriter, pointer, features[i].offset, true));
                else 
                    localDictionary.Add(features[i].name , buildedPointer); // use base, to reduce process
                // Used to compare to check if user load another save game.

                if(!WarfareIndexedFeatures.ContainsKey(features[i].name) && features[i].category.Equals("Warfare"))
                    WarfareIndexedFeatures.Add(features[i].name, features[i]);
                if(!SpecialIndexedFeatures.ContainsKey(features[i].name)&& features[i].category.Equals("Special"))
                    SpecialIndexedFeatures.Add(features[i].name, features[i]);
                if(!CountryIndexedFeatures.ContainsKey(features[i].name)&& features[i].category.Equals("Country"))
                    CountryIndexedFeatures.Add(features[i].name, features[i]);
                if(!ResourcesIndexedFeatures.ContainsKey(features[i].name)&& features[i].category.Equals("Resources"))
                    ResourcesIndexedFeatures.Add(features[i].name, features[i]);

                ptrPointer = BuildAPointerString(memoryReaderWriter, pointer, features[i].offset);
                if(!FeaturePointerStoreRaw.ContainsKey(features[i].name))
                    FeaturePointerStoreRaw.Add(features[i].name, buildedPointer);
                // not used atm to avoid DPA
                // Since memory.getCode will generate lots of string debris
                // use base instead, since the processName is base module (0x400000)
                // FeaturePointerStore.Add(f.name ,DataGame.ProcessName + "+" + pointer + offset);
                
                if(!FeatureIndexStore.ContainsKey(features[i].name)) 
                    FeatureIndexStore.Add(features[i].name, i);
                
                if(!FeatureIndexedStore.ContainsKey(features[i].name)) 
                    FeatureIndexedStore.Add(features[i].name, features[i]);
            }
            
            return localDictionary;
        }
        
        private void CreateMissileSchema()
        {
            foreach (var warfareSubcategory in DataWarfare.subCategories)
            {
                if (warfareSubcategory.categoryName == "5. Unit/Missile - Stats Editor [WIP]")
                {
                    var includedFeatures = warfareSubcategory.categoryIncludedFeatures;
                    for (int i = 0; i < includedFeatures.Count; i++)
                    {
                        if(DataWarfare.features.All(s => s.name != includedFeatures[i])) continue;
                        var parentUnit = DataWarfare.features.Single(s => s.name == includedFeatures[i]);
                        _tempFeature = new Feature()
                        {
                            id = Int32.Parse($"999{i}999"),
                            pointerId = 24,
                            freeze = parentUnit.freeze,
                            name = parentUnit.name.Replace("Unit", "MissileStat"),
                            displayName = parentUnit.name.Replace("Unit", "Missile"),
                            type = parentUnit.type,
                            category = "Warfare",
                            subCategory = "",
                            description = "To activate go to [Defense - Production] > Missile > Add to Queue > Select it > See editor ",
                            original = parentUnit.original,
                            value = parentUnit.value,
                            formattedValue = parentUnit.formattedValue,
                            format = parentUnit.format,
                            offset = parentUnit.offset,
                            editable = parentUnit.editable,
                            enabled = parentUnit.enabled,
                            wiki = parentUnit.wiki
                        };
                        includedFeatures[i] = _tempFeature.name;
                        DataWarfare.features.Add(_tempFeature);
                    }
                }
                
            }
        }

        // private string _mapTemp;

        private void MapSubCategoryIntoFeatures(Category category)
        {
            for (int i = 0; i < category.subCategories.Count; i++)
            {
                for (int j = 0; j < category.features.Count; j++)
                {
                    if (!category.subCategories[i].categoryIncludedFeatures.Contains(category.features[j].name)) continue;
                    category.features[j].subCategory = category.subCategories[i].categoryName;
                }
            }
            // foreach (var subCategory in category.subCategories)
            // {
            //     // _mapTemp = subCategory.categoryName;
            //     foreach (var feature in category.features)
            //     {
            //         // if (feature.category != category.category) return;
            //         // feature.subCategory = $"{subCategory.id.ToString()}.{subCategory.categoryName}";
            //         // _mapTemp = feature.subCategory;
            //     }
            //     // subCategory.categoryName = _mapTemp;
            // }
        }

        
        public void MutateFeaturePointer(SRReadWrite memoryReaderWriter, 
            string[] includedFeature,
            int needlePointerId,
            int newPointerId)
        {
            foreach (var feat in FeaturesWarfare)
            {
                var feature = feat;
                if (includedFeature.Contains(feature.name))
                {
                    if(feature.pointerId != needlePointerId) continue;
                    if (FeaturePointerStore.ContainsKey(feature.name))
                    {
                        if (feature.name == "UnitID" && feature.pointerId == 13)
                            feature.pointerId = 32;
                        else if (feature.name == "UnitID" && feature.pointerId == 32)
                            feature.pointerId = 13;
                        else
                            feature.pointerId = newPointerId;

                        // Rebuild pointer member 
                        feature.BuildAddress(DataVersion.Pointers);
                        FeaturePointerStore[feature.name] = 
                            BuildAPointerString(memoryReaderWriter, 
                                DataPointer[feature.pointerId-1].pointer, 
                                feature.offset, 
                                false);
                        FeatureIndexedStore[feature.name] = feature;
                        WarfareIndexedFeatures[feature.name] = feature;
                    }
                }
            }
        }
        
        int getIndexing(string[] orderedArray,string val)
        {
            return Array.FindIndex(orderedArray, id => id.Contains(val));
        }

        public string PointerStore(string varName)
        {
            return FeaturePointerStore[varName];
            // TODO: Part 1, to make a blog, how i managed to get rid of 8000mb Small object heap DPA analysis
            // Feature offs;
            // Func<Feature, bool> search = ss => string.Equals(ss.name, varName, StringComparison.OrdinalIgnoreCase);
            // offs = FeaturesResources.FirstOrDefault(search) ?? FeaturesCountry.FirstOrDefault(search) ??
            //        FeaturesWarfare.FirstOrDefault(search) ?? FeaturesSpecial.FirstOrDefault(search);
            // var p = DataPointer.First(s => s.id == offs.pointerId).pointer;
            // var offset = String.IsNullOrEmpty(offs.offset) ? "" : "," + offs.offset;
            // return DataGame.ProcessName + "+" + p + offset;
            
            // Feature offs;
            // offs = FeaturesResources
            //    .Concat(FeaturesCountry)
            //    .Concat(FeaturesWarfare)
            //    .Concat(FeaturesSpecial)
            //    .First(ss => ss.name == varName);
            //
            // var p = DataPointer.First(s => 
            //     s.id == offs.pointerId).pointer;
            // var offset = String.IsNullOrEmpty(offs.offset) ? "" : "," + offs.offset;
            //
            // return DataGame.ProcessName + "+" + p + offset;
        }
        
        public string getUnitId()
        {
            return FeaturesWarfare.First(f => f.name == "UnitID").value;
        }
        public string getUnitName()
        {
            return FeaturesWarfare.First(f => f.name == "UnitName").value;
        }
        public string getUnitName(IList<Feature> fList)
        {
            // return fList.AsParallel().First(s => s.name == "UnitName").value ?? "";
            return fList.Any(s => s.name == "UnitName") ? FeaturesWarfare.First(f => f.name == "UnitName").value : "";
        }
        public string getUnitId(IList<Feature> fList)
        {
            return fList.Any(s => s.name == "UnitID") ? FeaturesWarfare.First(f => f.name == "UnitID").value : "";
        }

        public Feature FeatureByCategoryAndId(string category, int id)
        {
            Feature feat;
            if (string.Equals(category, "warfare", StringComparison.OrdinalIgnoreCase))
                feat = FeaturesWarfare.First(s => s.id == id);
            else if (string.Equals(category, "country", StringComparison.OrdinalIgnoreCase))
                feat = FeaturesCountry.First(s => s.id == id);
            else if (string.Equals(category, "special", StringComparison.OrdinalIgnoreCase))
                feat = FeaturesSpecial.First(s => s.id == id);
            else
                feat = FeaturesResources.First(s => s.id == id);
            return feat;
        }
        
        public Feature FeatureByCategoryAndName(string category, string varname)
        {
            Feature feat;
            if (string.Equals(category, "warfare", StringComparison.OrdinalIgnoreCase))
                feat = FeaturesWarfare.First(s => s.name == varname);
            else if (string.Equals(category, "country", StringComparison.OrdinalIgnoreCase))
                feat = FeaturesCountry.First(s => s.name == varname);
            else if (string.Equals(category, "special", StringComparison.OrdinalIgnoreCase))
                feat = FeaturesSpecial.First(s => s.name == varname);
            else 
                feat = FeaturesResources.First(s => s.name == varname);
            return feat;
        }
        public Feature? feature(string varName, IList<Feature> from = null)
        {
            if(FeatureIndexedStore.ContainsKey(varName)) 
                   return FeatureIndexedStore[varName];
            return null;
            // throw new Exception("Something went wrong with metadata");
            // Feature offs;
            // Func<Feature, bool> search = ss => string.Equals(ss.name, varName, StringComparison.OrdinalIgnoreCase);
            // if (from == null)
            //     offs = FeaturesResources.FirstOrDefault(search) ?? FeaturesCountry.FirstOrDefault(search) ??
            //         FeaturesWarfare.FirstOrDefault(search) ?? FeaturesSpecial.FirstOrDefault(search);
            // else
            //     offs = from.First(search);
            // return offs;
            // if(from == null)
            //     offs = FeaturesResources
            //         .Concat(FeaturesCountry)
            //         .Concat(FeaturesWarfare)
            //         .Concat(FeaturesSpecial)
            //         .First(ss => ss.name.ToLower() == varName.ToLower());
            // else 
            //     offs = from.First(ss => ss.name.ToLower() == varName.ToLower());
            // return offs;
        }

        public IList<Feature> seekWarfareVariable(string[] varList)
        {
            IList<Feature> tmp = new List<Feature>();
            foreach (var v in varList)
            {
                var tmp2 = FeaturesWarfare.Where(s => v == s.name);
                if (tmp2.Any())
                {
                    tmp.Add(tmp2.First());
                }
            }
            var lf = tmp;
            return lf;
        }
        public Feature safeFeatureSearch(string varName, IList<Feature> from = null)
        {
            dynamic offs = false;
            if (from == null)
            {
                var foo = new List<IList<Feature>>
                {
                    FeaturesCountry,
                    FeaturesResources,
                    FeaturesWarfare,
                    FeaturesSpecial
                };
                //
                // var c1 = FeaturesCountry.Count;
                // var c2 = FeaturesResources.Count;
                // var c3 = FeaturesWarfare.Count;
                // var c4 = FeaturesSpecial.Count;
                // var bar = new List<Feature>(c1+c2+c3+c4);
                // int count = 0;
                //
                // AddToBelow:
                // if (count < foo.Count)
                // {
                //     bar.AddRange(foo[count++]);
                //     goto AddToBelow;
                // }
                foreach (var ftss in foo)
                {
                    foreach (var f in ftss)
                    {
                        if (f.name.ToLower() == varName.ToLower())
                        {
                            offs = f;
                        }
                    }
                }
            }
            else
            {
                foreach (var ft in from)
                {
                    if (ft.name.ToLower() == varName.ToLower())
                    {
                        offs = ft;
                        break;
                    }
                }
            } 
            return offs;
        }

        public void increasePopulationProgrammatically()
        {

        }       
        
        public void Dispose()
        {
            if (FeaturePointerStoreRaw != null)
            {
                FeaturePointerStoreRaw.Clear();
                FeaturePointerStoreRaw = null;
            }
            if(CountryIndexedFeatures != null)
            {
                CountryIndexedFeatures.Clear();
                CountryIndexedFeatures = null;
            }
            if(ResourcesIndexedFeatures != null)
            {
                ResourcesIndexedFeatures.Clear();
                ResourcesIndexedFeatures = null;
            }
            if(WarfareIndexedFeatures != null)
            {
                WarfareIndexedFeatures.Clear();
                WarfareIndexedFeatures = null;
            }
            if(SpecialIndexedFeatures != null)
            {
                SpecialIndexedFeatures.Clear();
                SpecialIndexedFeatures = null;
            }
            if (FeatureIndexedStore != null)
            {
                FeatureIndexedStore.Clear();
                FeatureIndexedStore = null;
            }
            if (FeaturesCountry != null)
            {
                FeaturesCountry.Clear();
                FeaturesCountry = null;
            }
            if (FeaturesResources != null)
            {
                FeaturesResources.Clear();
                FeaturesResources = null;
            }
            if (FeaturesWarfare != null)
            {
                FeaturesWarfare.Clear();
                FeaturesWarfare = null;
            }
            if (FeaturesSpecial != null)
            {
                FeaturesSpecial.Clear();
                FeaturesSpecial = null;
            }
            GC.SuppressFinalize(this);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            MessageBox.Show("Changed");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}