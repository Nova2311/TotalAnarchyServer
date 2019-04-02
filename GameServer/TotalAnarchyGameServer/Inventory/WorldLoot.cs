using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TotalAnarchyGameServer.Weapons;

namespace TotalAnarchyGameServer.Inventory {
    public class WorldLoot {

        public static List<BaseWeapon> worldWeapons;
        public List<Item> worldItems;

        public WorldLoot() {
            worldWeapons = new List<BaseWeapon>();
            worldItems = new List<Item>();
        }

        public static void GenerateWeaponLocations() {
            Random rndWeapon = new Random();
            for (int i = 0; i < Constants.WEAPON_SPAWN_POINTS; i++) {
                int weapon = rndWeapon.Next(0, WeaponManager.weapons.Count);
                BaseWeapon newWeapon = null;
                switch (weapon) {
                    case 0:
                        newWeapon = new AK47();
                        break;
                    case 1:
                        newWeapon = new M4();
                        break;
                    case 2:
                        newWeapon = new _870Shotgun();
                        break;
                    case 3:
                        newWeapon = new L96Sniper();
                        break;
                    case 4:
                        newWeapon = new Glock();
                        break;
                }

                newWeapon.weaponID = WeaponManager.weapons[weapon].weaponID;
                newWeapon.weaponName = WeaponManager.weapons[weapon].weaponName;
                newWeapon.damage = WeaponManager.weapons[weapon].damage;
                newWeapon.ammoPerClip = WeaponManager.weapons[weapon].ammoPerClip;
                newWeapon.fireRate = WeaponManager.weapons[weapon].fireRate;
                newWeapon.isDropped = true;
                newWeapon.worldSpawn = i;
                newWeapon.worldID = i;
                  
                worldWeapons.Add(newWeapon); // need to make a new instance

                Logger.WriteDebug(weapon.ToString() + " " + newWeapon.weaponName);
            }

            Serializer.Serialize(worldWeapons);
        }

        public void GenerateItemLocations() {

        }

    }
}
