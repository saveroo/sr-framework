// using System;
// using System.Collections.Generic;
// using LiteDB;
//
// namespace SRUL
// {
//     public struct SRFramework
//     {
//         public string SRFIdentifier { get; set; }
//         public string SRFVersion { get; set; }
//         public List<Game> games { get; set; }
//
//         public SRFramework(string srid, string srv, List<Game> gms)
//         {
//             SRFIdentifier = srid;
//             SRFVersion = srv;
//             games = gms;
//         }
//     }
//     public struct Version
//     {
//         public string GameVersion { get; set; }
//         public bool Availability { get; set; }
//         public List<Pointers> Pointers { get; set; }
//         public List<Categories> Categories { get; set; }
//
//         public Version(string gameVersion, bool availability, List<Pointers> pointer, List<Categories> categories)
//         {
//             GameVersion = gameVersion;
//             Availability = availability;
//             Pointers = pointer;
//             Categories = categories;
//         }
//     }
//
//     public struct Game
//     {
//         public string DisplayName { get; set; }
//         public string ProcessName { get; set; }
//         public IList<Version> Versions { get; set; }
//
//         public Game(string displayName, string processName, IList<Version> versions)
//         {
//             this.DisplayName = displayName;
//             this.ProcessName = processName;
//             this.Versions = versions;
//         }
//     }
//
//
//     public sealed class GameList
//     {
//         public IList<Game> Games { get; set; }
//         private static readonly Lazy<GameList> _gameList = new Lazy<GameList>(() => new GameList());
//
//         private GameList()
//         {
//             // using (var db = new LiteDatabase(@"R:\PROJECT\RELEASE\sru.db"))
//             // {
//             //     var col = db.GetCollection<Game>("GameList");
//             //
//             //     var game = new Game(
//             //         "Supreme Ruler Ultimate",
//             //         "SupremeRulerUltimate",
//             //         new List<Version> {
//             //             new Version("9, 2, 4", true)
//             //         });
//             //     col.Insert(game);
//             //
//             // }
//             this.Games = new List<Game>
//             {
//                 new Game(
//                     "Supreme Ruler Ultimate", 
//                     "SupremeRulerUltimate", 
//                     new List<Version> {
//                         new Version("9, 2, 4", true)
//                     }), 
//                 new Game(
//                     "Supreme Ruler 1936", 
//                     "SupremeRuler1936", 
//                     new List<Version> {
//                         new Version("Unavailable", false), 
//                     }),
//                 new Game(
//                     "Galactic Ruler", 
//                     "GalacticRuler", 
//                     new List<Version> {
//                         new Version("x, x, x", false), 
//                     }),
//             };
//         }
//
//         public static GameList Instance
//         {
//             get { return _gameList.Value; }
//         }
//         
//     }
// }