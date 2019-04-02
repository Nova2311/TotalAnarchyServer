using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalAnarchyGameServer.Weapons {
    public class L96Sniper : BaseWeapon {
        public L96Sniper() {
            weaponName = "L96Sniper";
            weaponID = 3;
            damage = 100;
            ammoPerClip = 5;
            currentAmmo = ammoPerClip;
            fireRate = 0.5f;
            ammoType = AmmoTypes.sevenSixTwo;

        }
    }
}

