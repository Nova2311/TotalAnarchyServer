using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalAnarchyGameServer.Weapons {
    class Pistol : Weapon {

        public Pistol() {
            weaponID = 2;
            weaponName = "Pistol";
            ammoPerClip = 17;
            currentAmmo = ammoPerClip;
            ammoType = AmmoType.pistol;
        }

    }
}
