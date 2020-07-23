using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;

namespace SRUL.Models
{
    class SRU924List : ObservableCollection<SRU924>
    {
        public SRU924List()
        {
            Add(new SRU924(true, true, false, 
                "CountryPopulation", 
                "float", 
                "", 
                "", 
                "", 
                "", 
                "0x7AF0"));
            Add(new SRU924(true, true, false,
                "Immigration",
                "float",
                "",
                "",
                "",
                "",
                "0x7B14"));
            Add(new SRU924(true, true, false,
                "CountryBirth",
                "float",
                "",
                "",
                "",
                "",
                "0x7B1C"));
            Add(new SRU924(true, true, false,
                "CountryDeath",
                "float",
                "",
                "",
                "",
                "",
                "0x7B20"));
        }

        public IEnumerable<string> GetFieldNames()
        {
            return new string[] { "name", "type", "offset" };
        }
    }
}
