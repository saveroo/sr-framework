using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SRUL.Annotations;

namespace SRUL.Models
{
    class SRU924 : INotifyPropertyChanged
    {
        public SRU924(bool enabled, bool editable, bool freeze, string name, string type, string description, string oldvalue, string currentvalue, string formattedValue, string offset)
        {
            this.freeze = freeze;
            this.editable = editable;
            this.name = name;
            this.type = type;
            this.description = description;
            this.oldvalue = oldvalue;
            this.currentvalue = currentvalue;
            this.formattedValue = formattedValue;
            this.offset = offset;
            this.enabled = enabled;
        }
        private bool freeze;
        private bool editable;
        private string name;
        private string type;
        private string description;
        private string oldvalue;
        private string currentvalue;
        private string formattedValue;
        private string offset;
        private bool enabled;

        public string Name { get; set; }
        public string DataType { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
