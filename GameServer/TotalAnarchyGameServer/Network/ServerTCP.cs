using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

using TotalAnarchyGameServer.Player;
namespace TotalAnarchyGameServer.Network {
    class ServerTCP {
        public static TcpListener serverSocket;

        public static void InitializeNetwork() {
            serverSocket = new TcpListener(IPAddress.Any, 5555);
            serverSocket.Start();
            serverSocket.BeginAcceptTcpClient(OnClientConnect, null);
        }

        static void OnClientConnect(IAsyncResult result) {
            TcpClient client = serverSocket.EndAcceptTcpClient(result);
            serverSocket.BeginAcceptTcpClient(OnClientConnect, null);
            for (int i = 0; i < Constants.MAX_PLAYERS; i++) {
                if (General.client[i].socket == null) {
                    General.client[i].socket = client;
                    General.client[i].connectionID = i;
                    General.client[i].ip = client.Client.RemoteEndPoint.ToString();
                    
                    General.client[i].Start();

                    SendInGame(General.client[i]);
                    //SendInWorld(General.client[i]);
                    Logger.WriteLog("Connection received from " + General.client[i].ip + " | ConnectionID: " + General.client[i].connectionID);
                    return;
                }
            }
        }

        public static void SendDataTo(int connectionID, byte[] data) {
            try {
                ByteBuffer buffer = new ByteBuffer();
                buffer.WriteLong((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
                buffer.WriteBytes(data);
                General.client[connectionID].myStream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
                buffer.Dispose();
            } catch (Exception e) {
                return;
            }
        }
        public static void SendDataToAll(byte[] data) {
            for (int i = 0; i < Constants.MAX_PLAYERS; i++) {
                if (General.client[i].socket != null) {
                    SendDataTo(i, data);
                    Thread.Sleep(250);
                }
            }
        }

        public static void SendDataToAllBut(int connectionID, byte[] data) {
            for (int i = 0; i < Constants.MAX_PLAYERS; i++) {
                if (connectionID != i) {
                    if (General.client[i].socket != null && General.client[i].playerLoadedIn) {
                        SendDataTo(i, data);
                    }
                }
            }
        }

        public static void SendToAllInLobby(Lobby lobby, byte[] data) {
            try {
                for (int i = 0; i < lobby.clientsInLobby.Count; i++) {
                    if (lobby.clientsInLobby[i].socket != null) {
                        SendDataTo(lobby.clientsInLobby[i].connectionID, data);
                    }
                }
            } catch (Exception) {
            }
        }

        public static void SendToAllInLobbyBut(Lobby lobby, int connectionID, byte[] data) {
            try {
                for (int i = 0; i < lobby.clientsInLobby.Count; i++) {
                    if (connectionID != lobby.clientsInLobby[i].connectionID) { //Something happening here when you shoot after a player has died
                        SendDataTo(lobby.clientsInLobby[i].connectionID, data);
                    }
                }
            } catch (Exception e) {
                Logger.WriteError("Error Encountered: " + e.ToString());
                return;
            }

        }

        public static void SendInGame(Clients client) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SInGame); //Packet Identifier.
            buffer.WriteInteger(client.connectionID);

            SendDataTo(client.connectionID, buffer.ToArray());
            buffer.Dispose();
        }

        

        #region SendPackages

        public static byte[] PlayerData(int connectionID) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SPlayerData); //Packet Identifier.
            buffer.WriteInteger(connectionID);
            //send the players details

            //random spawn points
            Random rnd = new Random();
            int spawn = rnd.Next(0, Constants.PLAYER_SPAWN_POINTS - 1);
            buffer.WriteInteger(spawn);

            return buffer.ToArray();
        }

        public static void PlayerPositions(Clients client) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SPlayerPosition);
            buffer.WriteInteger(client.connectionID);

            buffer.WriteFloat(client.playerCharacter.posx);
            buffer.WriteFloat(client.playerCharacter.posy);
            buffer.WriteFloat(client.playerCharacter.posz);

            //SendToAllInLobbyBut(General.lobbys[client.lobbyIDConnectedTo], client.connectionID, buffer.ToArray());
            //SendDataToAllBut(client.connectionID, buffer.ToArray());
        }

        public static void PlayerRotations(Clients client) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SPlayerRotation);
            buffer.WriteInteger(client.connectionID);

            buffer.WriteFloat(client.playerCharacter.rotx);
            buffer.WriteFloat(client.playerCharacter.roty);
            buffer.WriteFloat(client.playerCharacter.rotz);
            buffer.WriteFloat(client.playerCharacter.rotw);

            //SendToAllInLobbyBut(General.lobbys[client.lobbyIDConnectedTo], client.connectionID, buffer.ToArray());
            //SendDataToAllBut(client.connectionID, buffer.ToArray());
        }

        public static void PlayerDied(Clients client) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SPlayerDied);
            buffer.WriteInteger(client.connectionID);

            SendToAllInLobby(General.lobbys[client.lobbyIDConnectedTo], buffer.ToArray());
        }

        public static void PlayerDisconnected(Clients client) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SPlayerDisconnected);
            buffer.WriteInteger(client.connectionID);

            SendToAllInLobby(General.lobbys[client.lobbyIDConnectedTo], buffer.ToArray());
        }
        #endregion
    }
}

