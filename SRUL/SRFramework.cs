using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using LiteDB;
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

    public class Feature : INotifyPropertyChanged, IObservable<Feature>
    {
        public bool freeze { get; set; } = false;
        public int id { get; set; }
        public int pointerId { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
        public string type { get; set; }
        public string category { get; set; }
        public string original { get; set; }
        public string value { get; set; }
        public string description { get; set; }
        public string formattedValue { get; set; }
        public string format { get; set; }
        public string offset { get; set; }
        public bool editable { get; set; }
        public bool enabled { get; set; }
        public int gridId { get; set; }


        public IDisposable Subscribe(IObserver<Feature> observer)
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            MessageBox.Show("ChanedEvent");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Category : INotifyPropertyChanged
    {
        public int id { get; set; }
        public int featureCount { get; set; }
        public string category { get; set; }
        public string description { get; set; }
        public int[] columnsVisibility { get; set; }
        public string[] rowOrders { get; set; }
        public string[] rowExclusion { get; set; }
        public IList<Feature> features { get; set; }

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

    public class Game
    {
        public string DisplayName { get; set; }
        public string ProcessName { get; set; }
        public IList<Version> Versions { get; set; }

    }

    public class Root    {
        public string SRFAuthor { get; set; } 
        public string SRFContact { get; set; } 
        public string SRFRepository { get; set; } 
        public IList<SRFChangelog> SRFChangelog { get; set; }
        public IList<SRFSocial> SRFSocial { get; set; } 
        public string SRFWebsite { get; set; } 
        public bool SRFStatus { get; set; } 
        public IList<string> SRFMirrorList { get; set; } 
        public string SRFName { get; set; } 
        public string SRFDescription { get; set; } 
        public string SRFIdentifier { get; set; } 
        public string SRFVersion { get; set; } 
        public string SRFRevision { get; set; } 
        public string SRFDownloadLink { get; set; } 
        public IList<string> SRFTags { get; set; } 
        public string SRFSchema { get; set; } 
        public string SRFHelp { get; set; } 
        public IList<Game> Games { get; set; } 

    }
}