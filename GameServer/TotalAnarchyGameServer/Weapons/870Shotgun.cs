using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalAnarchyGameServer.Weapons
{
    public class _870Shotgun : BaseWeapon
    {
        public _870Shotgun(){
            weaponName = "870Shotgun";
            weaponID = 2;
            damage = 100;
            ammoPerClip = 8;
            currentAmmo = ammoPerClip;
            fireRate = 0.5f;
            ammoType = AmmoTypes.shotgun;
        }
    }
}
