using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRUL
{
    class ActionHistory
    {
        public static List<string> HistoryList = new List<string>();

        public static void addToHistory(string values)
        {
            HistoryList.Add(values);
        }
    }
}
