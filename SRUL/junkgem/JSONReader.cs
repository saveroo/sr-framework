using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Memories;
using System.Management;
using System.Runtime.CompilerServices;
using DevExpress.DataAccess.Json;
using DevExpress.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SRUL.Annotations;
using SRUL.Types;

namespace SRUL
{
    public sealed class JSONReader : IDisposable, INotifyPropertyChanged
    {
        public SREncryptor srEncryptor = SREncryptor.Instance;

        private readonly Array _sortedList;
        public Root Data { get; set; }
        public ActiveTrainer activeTrainer { get; set; } = ActiveTrainer.Instance;
        public IList<Game> DataGames { get; set; }
        public Game DataGame { get; set; }
        public Version DataVersion { get; set; }
        public IList<Pointer> DataPointer { get; set; }
        public Category DataCountry { get; set; }
        public Category DataResources { get; set; }
        public Category DataWarfare { get; set; }
        public Category DataSpecial { get; set; }
        public IList<Feature> FeaturesCountry { get; set; }
        public IList<Feature> FeaturesResources { get; set; }
        public IList<Feature> FeaturesWarfare { get; set; }
        public IList<Feature> FeaturesSpecial { get; set; }
        public JsonDataSource DataJsonSource { get; set; }

        private static Lazy<JSONReader> JsonReader = null;
        // dynamic stuff = JObject.Load()
        private JSONReader(Root rt)
        {
            activeTrainer = ActiveTrainer.Instance;
            // activeTrainer.GameValidated = false;
            Load(rt);
            // CreateDataSourceFromWeb();
        }

        public static JSONReader CreateSingleton(Root rt)
        {
            if (JsonReader != null)
            {
                // MessageBox.Show("Reader have been instantiated");
                Debug.WriteLine("Reader have been instantiated");

            }
            JsonReader = new Lazy<JSONReader>(() => new JSONReader(rt));
            return JsonReader.Value;
        }
        
        public static JSONReader Instance
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

        public void dgOrder(GridView dgv)
        {
            dgv.Columns["id"].Visible = false;
            dgv.Columns["pointerId"].Visible = false;
            dgv.Columns["name"].Visible = false;
            dgv.Columns["formattedValue"].Visible = false;
            dgv.Columns["type"].Visible = false;
            dgv.Columns["format"].Visible = false;
            dgv.Columns["offset"].Visible = false;
            dgv.Columns["enabled"].Visible = false;
            dgv.Columns["category"].Visible = false;
            dgv.Columns["editable"].Visible = false;
            dgv.Columns["original"].Visible = false;
            dgv.Columns["gridId"].Visible = false;

            if (dgv.GetRowCellValue(0, "category").ToString().ToLower() == "warfare")
            {
                dgv.Columns["original"].Visible = true;
            }
            
            dgv.Columns["displayName"].AbsoluteIndex = 0;
            dgv.Columns["value"].AbsoluteIndex = 1;
            dgv.Columns["description"].AbsoluteIndex = 2;

            dgv.Columns["displayName"].OptionsColumn.AllowEdit = false;
            dgv.Columns["description"].OptionsColumn.AllowEdit = false;
            // dgv.Columns["description"].ReadOnly = true;
        }

        public void setReadOnlyRow(CustomRowCellEditEventArgs e)
        {
            var repoReadOnly = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            repoReadOnly.Name = "repoTextEditReadOnly";
            repoReadOnly.ReadOnly = true;
            e.RepositoryItem = repoReadOnly;
        }

        private void UpdateRow(DevExpress.XtraGrid.Views.Base.ColumnView View)
        {
            // Obtain the Price column. 
            DevExpress.XtraGrid.Columns.GridColumn col = View.Columns.ColumnByFieldName("Price");
            if (col == null) return;
            View.BeginSort();
            try
            {
                // Obtain the number of data rows. 
                int dataRowCount = View.DataRowCount;
                // Traverse data rows and change the Price field values. 
                for (int i = 0; i < dataRowCount; i++)
                {
                    object cellValue = View.GetRowCellValue(i, col);
                    double newValue = Convert.ToDouble(cellValue) * 0.9;
                    View.SetRowCellValue(i, col, newValue);
                }
            }
            finally { View.EndSort(); }
        }

        public bool SelectedTrainer(string gn, string gv)
        {
            activeTrainer.GameName = gn;
            activeTrainer.GameVersion = gv;
            if (DataGames.Where(s => s.DisplayName == gn).Any())
            {
                activeTrainer.GameValidated = true;
                Load();
                return activeTrainer.GameValidated;
            }
            activeTrainer.GameValidated = false;
            return activeTrainer.GameValidated;
        }
        private JsonDataSource CreateDataSourceFromWeb()
        {
            var jsonDataSource = new JsonDataSource();
            //Specify the data source location 
            jsonDataSource.JsonSource = new UriJsonSource(new Uri("https://srframework.vercel.app/SRFeature.json"));
            // jsonDataSource.JsonSource = new UriJsonSource(new Uri("http://northwind.servicestack.net/customers.json"));

            jsonDataSource.Fill();
            //jsonDataSource.FillAsync();
            DataJsonSource = jsonDataSource;
            return jsonDataSource;
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
        public void Load(Root rt = null)
        {
            // const string fileName = @"R:\PROJECT\VS17\SRUL\SRUL\Transformer\api\SRFeature.json";
            // Data = JsonConvert.DeserializeObject<Root>(File.ReadAllText(fileName));
            if (rt != null)
            {
                Data = rt;
                DataGames = Data.Games;
            }
            // if (!activeTrainer.GameValidated)
            // {
            //     Data = JsonConvert.DeserializeObject<Root>(srEncryptor.decrypted);
            //     DataGames = Data.Games;
            // }
            // MessageBox.Show(activeTrainer.ToString());

            if (activeTrainer.GameProcess != null)
            {
                DataGame = Data.Games.FirstOrDefault(s => s.DisplayName == ActiveTrainer.Instance.GameName);
                DataVersion = DataGame.Versions.First(s => s.Availability && s.GameVersion == activeTrainer.GameVersion);

                DataCountry = DataVersion.Categories[0];
                DataResources = DataVersion.Categories[1];
                DataWarfare = DataVersion.Categories[2];
                DataSpecial = DataVersion.Categories[3];

                FeaturesCountry = gvSortList(DataCountry.features, ListOfSortedRow.countryRowOrderList);
                FeaturesResources = DataResources.features;
                FeaturesWarfare = gvSortList(DataWarfare.features, ListOfSortedRow.warfareRowOrderList);
                FeaturesSpecial = DataSpecial.features;
                DataPointer = DataVersion.Pointers;
            }
            // var jsonString = File.ReadAllText(fileName);
            // Data = JObject.Parse(jsonString);
            // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(jsonString);
            //
        }

        int getIndexing(string[] orderedArray,string val)
        {
            return Array.FindIndex(orderedArray, id => id.Contains(val));
        }

        public string pointerStore(string varName)
        {
            Feature offs;
            Func<Feature, bool> search = ss => string.Equals(ss.name, varName, StringComparison.OrdinalIgnoreCase);
            offs = FeaturesResources.FirstOrDefault(search) ?? FeaturesCountry.FirstOrDefault(search) ??
                   FeaturesWarfare.FirstOrDefault(search) ?? FeaturesSpecial.FirstOrDefault(search);
            var p = DataPointer.First(s => s.id == offs.pointerId).pointer;
            var offset = String.IsNullOrEmpty(offs.offset) ? "" : "," + offs.offset;
            return DataGame.ProcessName + "+" + p + offset;
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

        // public Feature getUnitId(string id)
        // {
        //     Feature unit;
        //     unit = FeaturesWarfare.Contains(predicate => predicate.id == id);
        // }
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
        public Feature feature(string varName, IList<Feature> from = null)
        {
            Feature offs;
            Func<Feature, bool> search = ss => string.Equals(ss.name, varName, StringComparison.OrdinalIgnoreCase);
            if (from == null)
                offs = FeaturesResources.FirstOrDefault(search) ?? FeaturesCountry.FirstOrDefault(search) ??
                    FeaturesWarfare.FirstOrDefault(search) ?? FeaturesSpecial.FirstOrDefault(search);
            else
                offs = from.First(search);
            return offs;
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
            IList<Feature> lf;
            IList<Feature> tmp = new List<Feature>();
            foreach (var v in varList)
            {
                var tmp2 = FeaturesWarfare.Where(s => v == s.name);
                if (tmp2.Any())
                {
                    tmp.Add(tmp2.First());
                }
            }
            lf = tmp;
            return lf;
        }
        public Feature safeFeatureSearch(string varName, IList<Feature> from = null)
        {
            dynamic offs = false;
            if (from == null)
            {
                var foo = new List<IList<Feature>>();
                foo.Add(FeaturesCountry);
                foo.Add(FeaturesResources);
                foo.Add(FeaturesWarfare);
                foo.Add(FeaturesSpecial);
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
       

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            MessageBox.Show("Changed sdasd");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}