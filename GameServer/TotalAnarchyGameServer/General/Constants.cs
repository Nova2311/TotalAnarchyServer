using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalAnarchyGameServer {
    class Constants{
        public const int MAX_PLAYERS = 100000; //current max tested players is 16,000
        public const int MAX_LOBBYS = 1;
        public const int MAX_PLAYERS_PER_LOBBY = 100;
        public const int PLAYER_SPAWN_POINTS = 6;
        public const int WEAPON_SPAWN_POINTS = 6;
        public const int AMMO_SPAWN_POINTS = 6;
        public const int LOBBY_TICK_RATE = 20; 
    }
}
