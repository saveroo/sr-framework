using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRUL.Types
{
    class WarfareValueDictionary
    {
        public Dictionary<int, string> DictUnitMovementType = new Dictionary<int, string>();
        public Dictionary<int, string> DictUnitTargetType = new Dictionary<int, string>();
        public Dictionary<int, string> DictUnitClassType = new Dictionary<int, string>();
        private static readonly Lazy<WarfareValueDictionary> _instance = new Lazy<WarfareValueDictionary>(() => new WarfareValueDictionary());


        public static WarfareValueDictionary Instance
        {
            get { return _instance.Value; }
        }
        public WarfareValueDictionary()
        {
            DictUnitClassType.Add(0, "Land Infantry");
            DictUnitClassType.Add(1, "Land Recon");
            DictUnitClassType.Add(2, "Land Tank");
            DictUnitClassType.Add(3, "Land Anti-Tank");
            DictUnitClassType.Add(4, "Land Artillery/MLRS/Mortar");
            DictUnitClassType.Add(5, "Land Air Defense");
            DictUnitClassType.Add(6, "Land Ground Transport/Bridging");
            DictUnitClassType.Add(7, "Air Helicopter");
            DictUnitClassType.Add(8, "Missiles (Ballistic / SLBM etc.");
            DictUnitClassType.Add(9, "Air Fighter Interceptor");
            DictUnitClassType.Add(10, "Air Fighter Attack/Bomber");
            DictUnitClassType.Add(11, "Air Fighter Multi Role");
            DictUnitClassType.Add(12, "Air Strategic Bomber");
            DictUnitClassType.Add(13, "Air Patrol/AWACS Fixed Wing/ECM");
            DictUnitClassType.Add(14, "Air Transport/Tanker");
            DictUnitClassType.Add(15, "Naval Submarine");
            DictUnitClassType.Add(16, "Naval Carrier");
            DictUnitClassType.Add(17, "Naval Capital/Battleship/Cruiser/Destroyer");
            DictUnitClassType.Add(18, "Naval Escort/Frigate/Corvette");
            DictUnitClassType.Add(19, "Naval Patrol Ship/Support ");
            DictUnitClassType.Add(20, "Naval Transport/Support");
            DictUnitClassType.Add(21, "Facility ");

            DictUnitTargetType.Add(0, "Soft");
            DictUnitTargetType.Add(1, "Hard");
            DictUnitTargetType.Add(2, "Fort");
            DictUnitTargetType.Add(3, "Close Air");
            DictUnitTargetType.Add(4, "Mid Air");
            DictUnitTargetType.Add(5, "High Air");
            DictUnitTargetType.Add(6, "Surface");
            DictUnitTargetType.Add(7, "Submerged");

            DictUnitMovementType.Add(0, "Unmounted");
            DictUnitMovementType.Add(1, "Wheel");
            DictUnitMovementType.Add(2, "Half Track");
            DictUnitMovementType.Add(3, "Tracked");
            DictUnitMovementType.Add(4, "Towed");
            DictUnitMovementType.Add(5, "Variable");
            DictUnitMovementType.Add(6, "Mech");
            DictUnitMovementType.Add(7, "Hover");
            DictUnitMovementType.Add(8, "Unused");
            DictUnitMovementType.Add(9, "Unused");
            DictUnitMovementType.Add(10, "Naval");
            DictUnitMovementType.Add(11, "Unused");
            DictUnitMovementType.Add(12, "Air Close");
            DictUnitMovementType.Add(13, "Air Mission");
        }
    }
}
