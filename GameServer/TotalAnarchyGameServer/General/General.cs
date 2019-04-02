using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TotalAnarchyGameServer.Network;
using TotalAnarchyGameServer.Player;
namespace TotalAnarchyGameServer {
    class General{

        public static List<Lobby> lobbys = new List<Lobby>(new Lobby[Constants.MAX_LOBBYS]);
        public static List<Thread> lobbyThreads = new List<Thread>(new Thread[Constants.MAX_LOBBYS]);
        public static Clients[] client = new Clients[Constants.MAX_PLAYERS];



        public static int GetTickCount(){
            return Environment.TickCount;
        }

        public static void InitializeServer(){
            int startTime = 0; int endTime = 0;
            startTime = GetTickCount();
            Logger.WriteInfo("Initializing Server...");
            //Intializing all game data arrays
            Logger.WriteInfo("Initializing Game Arrays...");
            for (int i = 0; i < Constants.MAX_PLAYERS; i++){
                client[i] = new Clients();
            }

            //Start the Networking
            Logger.WriteInfo("Initializing Network...");
            ServerHandleData.InitializePackets();
            ServerTCP.InitializeNetwork();

            Logger.WriteInfo("Initializing Lobbys...");
            lobbys.Capacity = Constants.MAX_LOBBYS;
            for (int i = 0; i < Constants.MAX_LOBBYS; i++) {
                lobbys[i] = new Lobby();
                lobbys[i].lobbyID = i;

                for (int x = 0; x < Constants.MAX_PLAYERS_PER_LOBBY; x++) {
                    lobbys[i].clientsInLobby[x] = new Clients();
                }

                lobbyThreads[i] = new Thread(lobbys[i].InitializeLobby);
                lobbyThreads[i].Start();
            }

            endTime = GetTickCount();
            Logger.WriteLog("Initialization complete. Server loaded in " + (endTime - startTime) + " ms.");
        }

        public static void Matchmake(int connectionID) {
            for (int i = 0; i < lobbys.Count; i++) {
                if (lobbys[i].connectedClients != Constants.MAX_PLAYERS_PER_LOBBY) {
                    for (int x = 0; x < client.Length; x++) {
                        if (client[x].connectionID == connectionID) {
                            lobbys[i].JoinLobby(client[x]);
                            return;
                        }
                    }
                }
            }
        }
    }
}
