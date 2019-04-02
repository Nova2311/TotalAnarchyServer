using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalAnarchyGameServer.Weapons {
    public class WeaponManager {

        public static List<Weapon> weapons = new List<Weapon>();
        public static List<Ammo.Ammo> ammoTypes = new List<Ammo.Ammo>();

        public WeaponManager() {
            weapons.Add(new Rifle());
            weapons.Add(new Shotgun());
            weapons.Add(new Pistol());
            weapons.Add(new Knife());
            weapons.Add(new Grenade());

            ammoTypes.Add(new Ammo.RifleAmmo());
            ammoTypes.Add(new Ammo.PistolAmmo());
            ammoTypes.Add(new Ammo.ShotgunAmmo());
        }
    }
}
