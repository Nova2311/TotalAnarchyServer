using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalAnarchyGameServer.Weapons {
    public class Rifle : Weapon {

        public Rifle() {
            weaponName = "Rifle";
            weaponID = 0;
            ammoPerClip = 20; 
            currentAmmo = ammoPerClip;
            ammoType = AmmoType.rifle;
        }


    }
}
