using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalAnarchyGameServer.Weapons.Ammo {
    public class Ammo {
        public int ammoID;
        public string ammoName;
        public int amount;

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
