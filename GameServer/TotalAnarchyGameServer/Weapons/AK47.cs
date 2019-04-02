using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalAnarchyGameServer.Weapons{
    public class AK47 : BaseWeapon{

        public AK47() { 
            weaponName = "AK47";
            weaponID = 0;
            damage = 100;
            ammoPerClip = 30;
            currentAmmo = ammoPerClip;
            fireRate = 0.5f;
            ammoType = AmmoTypes.sevenSixTwo;
        }
    }
}
