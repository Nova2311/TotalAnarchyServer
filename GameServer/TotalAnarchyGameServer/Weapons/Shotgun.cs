using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalAnarchyGameServer.Weapons {
    class Shotgun : Weapon {
        public Shotgun() {
            weaponID = 1;
            weaponName = "Shotgun";
            ammoPerClip = 7;
            currentAmmo = ammoPerClip;
            ammoType = AmmoType.shotgun;
        }
    }
}
