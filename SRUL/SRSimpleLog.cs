using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRUL
{
    public struct LogStruct
    {
        public string Title;
        public string Message;
    }

    public static class SRSimpleLog
    {
        public static List<LogStruct> Logs; 
        public static void RecordLog(string title, string message)
        {
            if(Logs == null)
                Logs = new List<LogStruct>();
            Logs.Add(new LogStruct()
            {
                Title = title,
                Message = message,
            });
            ShowLatestLogs();
        }

        public static string ShowLatestLogs()
        {
            var logs = $"[{Logs.Last().Title}]: {Logs.Last().Message}";
            Debug.WriteLine(logs);
            return logs;
        }
    }
}
