using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalAnarchyGameServer.Network;
using TotalAnarchyGameServer.Inventory;

namespace TotalAnarchyGameServer.Player {
    public class PlayerCharacter {

        public int currentHealth;
        public int maxHealth = 100;

        public bool isDead = false;

        public float posx;
        public float posy;
        public float posz;

        public float rotx;
        public float roty;
        public float rotz;
        public float rotw;

        public PlayerInventory playerInventory = new PlayerInventory();

        public PlayerCharacter() {
            currentHealth = maxHealth;
            isDead = false;
        }

        public void TakeDamage(int DamageAmount, Clients client) {
            currentHealth -= DamageAmount;

            if (currentHealth <= 0) {
                Die(client);
            }
        }

        public void Die(Clients client) {
            isDead = true;
            ServerTCP.PlayerDied(client);
            client.CloseSocket();
        }
    }
}
