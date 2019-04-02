using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalAnarchyGameServer.Weapons {
    public class M4 : BaseWeapon {
        public M4() {
            weaponName = "M4 Carbine";
            weaponID = 1;
            damage = 100;
            ammoPerClip = 30;
            currentAmmo = ammoPerClip;
            fireRate = 0.5f;

            ammoType = AmmoTypes.fiveFiveSix;

        }
    }
}
