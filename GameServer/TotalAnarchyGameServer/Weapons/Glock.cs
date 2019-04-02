using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalAnarchyGameServer.Weapons
{
    public class Glock : BaseWeapon
    {
        public Glock() {
            weaponName = "Glock";
            weaponID = 4;
            damage = 100;
            ammoPerClip = 10;
            currentAmmo = ammoPerClip;
            fireRate = 0.5f;
            ammoType = AmmoTypes.nineMM;
        }
    }
}
