using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRUL
{
    interface ISRControl
    {
        object Control { get; set; }
        string ControlName { get; set; }
        object ControlValue { get; set; }

    }

    internal class SRControls
    {
        public static SRControls Instance { get; } = new SRControls();
        private List<SRControl> Controls;

        private SRControls()
        {
            this.Controls = new List<SRControl>();
        }

        public void Register(SRControl ctrl)
        {
            this.Controls.Add(ctrl);
        }

        public void Remove(SRControl ctrl)
        {
            this.Controls.Remove(ctrl);
        }
    }

    internal class SRControl : ISRControl
    {
        public object Control { get; set; }
        public string ControlName { get; set; }
        public object ControlValue { get; set; }
        private SRControl()
        {
        }

        public void Register(object ctrl, string name, object value)
        {
            this.Control = ctrl;
            this.ControlName = name;
            this.ControlValue = value;
        }
    }
}
