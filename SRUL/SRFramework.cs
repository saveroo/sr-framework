using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using SmartAssembly.Attributes;
using SRUL.Annotations;

namespace SRUL
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject(myJsonResponse); 
    public class SRFChangelog    {
        public string Date { get; set; } 
        public string Version { get; set; } 
        public string Type { get; set; } 
        public string Title { get; set; } 
        public string Description { get; set; } 
        public List<string> Changes { get; set; } 

    }
    public class SRFSocial    {
        public string SocialIcon { get; set; } 
        public string SocialName { get; set; } 
        public string SocialAccount { get; set; } 
        public string SocialLink { get; set; }
    }
    
    public class Pointer
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public IList<object> categoryScope { get; set; }
        public IList<int> offsetsList { get; set; }
        public string pointer { get; set; }

    }
    
    public class Feature : ICloneable, INotifyPropertyChanged
    {
        private bool _freeze;
        private string _value;
        private string _original;
        private string? _buildedAddress;

        public bool freeze
        {
            get => _freeze;
            set
            {
                if (value == _freeze) return;
                _freeze = value;
                OnPropertyChanged();
            }
        }

        public int id { get; set; }

        public int _pointerId;
        public int pointerId
        {
            get
            {
                return _pointerId;
            }
            set
            {
                _pointerId = value;
            }
        }

        public string name { get; set; }
        public string displayName { get; set; }
        public string type { get; set; }
        public string category { get; set; }
        public string? subCategory { get; set; }

        public string original
        {
            get => _original;
            set => _original = value;
        }

        public string value
        {
            get
            {
                // if(type == "float")
                //     if(string.IsNullOrEmpty(_value)) 
                //         return "0" ?? _value;
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public string description { get; set; }
        public string wiki { get; set; }
        public string formattedValue { get; set; }
        public string format { get; set; }
        public string offset { get; set; }
        public bool editable { get; set; }
        public bool visible { get; set; } = true;
        public bool enabled { get; set; }
        public int gridId { get; set; }

        public string? BuildedAddress
        {
            get => _buildedAddress;
            set => _buildedAddress = value;
        }

        public bool IsReadable
        {
            get
            {
                return BuildedAddress != null && SRLoaderForm._srLoader.rw.MemoryAvailable(BuildedAddress);
            }
        }

        public Feature ShallowCopy()
        {
            return (Feature) MemberwiseClone();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Feature(IList<Pointer> ptrMap)
        {
            BuildAddress(ptrMap);
        }
        
        public Feature()
        {
        }
        public void BuildAddress(IList<Pointer> ptrs)
        {
            var newOffset = string.IsNullOrEmpty(offset) ? "" : $",{offset}";
            if(ptrs[pointerId-1] != null)
                foreach (var ptr in ptrs)
                {
                    if (pointerId != ptr.id) continue;
                    BuildedAddress = $"base+{ptr.pointer}{newOffset}";
                    break;
                }
        }
        
        
        public void ReadValue()
        {
            if(_buildedAddress == null) return;
            if (!SRLoaderForm._srLoader.rw.MemoryAvailable(_buildedAddress)) return;
            if (type == "float")
                value = SRLoaderForm
                    ._srLoader
                    .rw
                    .ReadFloat(_buildedAddress, "", false)
                    .ToString("N"); // <<< CECUNGUKNYAA!!
            if(type == "byte")
                value = SRLoaderForm
                    ._srLoader
                    .rw
                    .ReadByte(_buildedAddress)
                    .ToString(CultureInfo.InvariantCulture);
            if(type == "2byte")
                value = SRLoaderForm
                    ._srLoader
                    .rw
                    .Read2Byte(_buildedAddress)
                    .ToString(CultureInfo.InvariantCulture);
            if(type == "2bytes")
                value = SRLoaderForm
                    ._srLoader
                    .rw
                    .Read2Byte(_buildedAddress)
                    .ToString(CultureInfo.InvariantCulture);
            if(type == "int")
                value = SRLoaderForm
                    ._srLoader
                    .rw
                    .ReadInt(_buildedAddress)
                    .ToString(CultureInfo.InvariantCulture);
            if (type == "string")
                value = SRLoaderForm
                    ._srLoader
                    .rw
                    .ReadString(_buildedAddress);
            if (type == "double")
                value = SRLoaderForm
                    ._srLoader
                    .rw
                    .ReadDouble(_buildedAddress)
                    .ToString(CultureInfo.InvariantCulture);
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    public class SubCategoryColumn
    {
        public string columnName { get; set; }
        public int columnMaxWidth { get; set; }
        public int columnMinWidth { get; set; }
        public bool columnAllowEdit { get; set; }

    }
    
    public class SubCategory
    {
        public int id { get; set; }
        private string _categoryName;
        private string _categoryDisplayName;

        public string categoryName
        {
            get =>  _categoryName;
            set => _categoryName = value;
        }

        public string categoryDisplayName
        {
            get => _categoryDisplayName;
            set => _categoryDisplayName = value;
        }

        public string categoryDescription { get; set; }
        public bool categoryVisibility { get; set; }
        public bool categoryExpanded { get; set; }
        public IList<string> categoryIncludedFeatures { get; set; }

    }

    public class Category : INotifyPropertyChanged
    {
        public int id { get; set; }
        public int featureCount { get; set; }
        public string category { get; set; }
        public string description { get; set; }
        [DoNotPrune]
        public IList<SubCategory> subCategories { get; set; }
        [DoNotPrune]
        public IList<SubCategoryColumn> categoryShownColumns { get; set; }
        public string[] rowOrders { get; set; }
        public string[] rowExclusion { get; set; }
        public IList<Feature> features { get; set; }

        public Category ShallowCopy()
        {
            Category category = (Category) MemberwiseClone();
            return category;
        }

        public Category DeepCopy()
        {
            Category category = this.ShallowCopy();
            category.features = features.Clone();
            return category;
        }
        public IList<Feature> GetCopyFeature()
        {
            IList<Feature> other = (IList<Feature>) MemberwiseClone();
            // ... then clone the nested class.
            return other;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    public class Version
    {
        public string GameVersion { get; set; }
        public bool Availability { get; set; }
        public IList<Pointer> Pointers { get; set; }
        public IList<Category> Categories { get; set; }

    }

    public class SRFormatType
    {
        public int FormatID { get; set; }
        public string FormatName { get; set; }
        public string FormatDisplayName { get; set; }
        public string[] FormatSchema { get; set; }
        public IList<IList<string>> FormatData { get; set; }
    }
    
    public class Game
    {
        public string DisplayName { get; set; }
        public string ProcessName { get; set; }
        [JsonProperty ("FormatTypes")]
        public IList<SRFormatType> SRFormatTypes { get; set; }
        public IList<Version> Versions { get; set; }

    }

    // ReSharper disable once InconsistentNaming
    public class Root  {
        public string? SRFAuthor { get; set; } 
        public string? SRFContact { get; set; } 
        public string? SRFRepository { get; set; } 
        public bool SRFMandatoryUpdate { get; set; }
        public string? SRFMandatoryUpdateMessage { get; set; }
        public string? SRFUpdateMessage { get; set; }
        public IList<SRFChangelog>? SRFChangelog { get; set; }
        public IList<SRFSocial>? SRFSocial { get; set; } 
        public string? SRFWebsite { get; set; } 
        public bool SRFStatus { get; set; } 
        public IList<string>? SRFMirrorList { get; set; } 
        public string? SRFName { get; set; } 
        public string? SRFDescription { get; set; } 
        public string? SRFIdentifier { get; set; } 
        public string[]? SRFKnownBugs { get; set; } 
        public string[]? SRFTodos { get; set; } 
        public string[]? SRFRevisionLogs { get; set; }
        public string? SRFVersion { get; set; } 
        public string? SRFRevision { get; set; } 
        public string? SRFDownloadLink { get; set; } 
        public IList<string>? SRFTags { get; set; } 
        public string? SRFSchema { get; set; } 
        public string? SRFHelp { get; set; } 
        public IList<Game> Games { get; set; } 

    }
}