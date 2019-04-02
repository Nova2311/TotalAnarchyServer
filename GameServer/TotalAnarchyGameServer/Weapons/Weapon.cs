using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TotalAnarchyGameServer.Weapons {
    public class Weapon {

        public AmmoType ammoType;
        public int weaponID;
        public string weaponName;
        public int currentAmmo;
        public int ammoPerClip;    

        public bool isDropped = true;
        public int worldSpawn;
        public int worldID;
        public float posX;
        public float posY;
        public float posZ;

        public float rotX;
        public float rotY;
        public float rotZ;

    }
}
