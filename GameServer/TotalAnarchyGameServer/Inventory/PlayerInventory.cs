using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalAnarchyGameServer.Weapons;
namespace TotalAnarchyGameServer.Inventory{
    public class PlayerInventory{

        public int shotgunRounds = 0;
        public int pistolRounds = 0;
        public int rifleRounds = 0;

        public Weapon currentEquippedWep = new Weapon();

    }
}
