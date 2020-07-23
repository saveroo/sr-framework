using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace SRUL.Models
{
    class Game
    {
        public int GameID { get; set; }
        public string DisplayName { get; set; }
        public string ProcessName { get; set; }
        public IList<Version> Versions { get; set; }
        public bool IsActive { get; set; }
    }

    class tGame
    {
        public tGame()
        {
            // using (var db = new LiteDatabase(@"R:\PROJECT\RELEASE\srhelper.db"))
            // {
            //     var col = db.GetCollection<Version>("Versions");
            //
            //     var game = new 
            // }
        }

    }

}
