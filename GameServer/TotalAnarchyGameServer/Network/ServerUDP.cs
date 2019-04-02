using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TotalAnarchyGameServer.Network {
    class ServerUDP {

        public static UdpClient serverListener;

        public static void InitializeNetwork() {
            serverListener = new UdpClient(5556);
            serverListener.BeginReceive(OnRecieve, null);
        }

        static void OnRecieve(IAsyncResult result) {
            try {
                IPEndPoint ipEndpoint = null;
                byte[] data = serverListener.EndReceive(result, ref ipEndpoint);
                HandleUDPData(data, ipEndpoint);

                serverListener.BeginReceive(OnRecieve, null);
            } catch (Exception e) {
                Logger.WriteError(e.ToString());
                Logger.WriteError("Player Disconnected from UDP");
                //Client has disconnected
            }
        }

        static void HandleUDPData(byte[] data, IPEndPoint endpoint) {

            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            string splitter;

            long packet = buffer.ReadLong();
            splitter = buffer.ReadString();
            int connectionID = buffer.ReadInteger();

            for (int i = 0; i < General.client.Length; i++) {
                if (General.client[i].connectionID == connectionID && General.client[i].endpoint == null) {
                    General.client[i].endpoint = endpoint;
                    break;
                }
            }

            switch (packet) {
                case 3:
                    PACKET_PLAYER_POSITION(connectionID, data);
                    break;
            }
        }


        public static void SendTo(IPEndPoint endpoint, byte[] data) {
            serverListener.Send(data, data.Length, endpoint);
            Logger.WriteDebug("Sent data to: " + endpoint);
        }

        public static void SendToAll(byte[] data) {
            for (int i = 0; i < General.client.Length; i++) {
                if (General.client[i].endpoint != null) {
                    SendTo(General.client[i].endpoint, data);
                }
            }
        }

        public static void SendToAllBut(int connectionID, byte[] data) {
            for (int i = 0; i < Constants.MAX_PLAYERS; i++) {
                if (connectionID != i) {
                    if (General.client[i].endpoint != null) {
                        //Logger.WriteDebug("Sending Position Data To: " + i);
                        SendTo(General.client[i].endpoint, data);
                    }
                }
            }
        }

        public static void SendToAllInLobbyBut(Lobby lobby, int connectionID, byte[] data) {
            for (int i = 0; i < lobby.clientsInLobby.Count; i++) {
                if (lobby.clientsInLobby[i].endpoint != null && lobby.clientsInLobby[i].connectionID != connectionID) {
                    SendTo(lobby.clientsInLobby[i].endpoint, data);
                }
            }
        }

        public static void SendToAllInLobby(Lobby lobby, byte[] data) {
            for (int i = 0; i < lobby.clientsInLobby.Count; i++) {
                if (lobby.clientsInLobby[i].endpoint != null) {
                    //Logger.WriteDebug("Sending data to: " + lobby.clientsInLobby[i].endpoint);
                    SendTo(lobby.clientsInLobby[i].endpoint, data);
                }
            }  
        }

        static void PACKET_PLAYER_POSITION(int connectionID, byte[] data) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            string splitter;

            long packet = buffer.ReadLong();
            splitter = buffer.ReadString();
            int ID = buffer.ReadInteger();
            splitter = buffer.ReadString();
            float posX = buffer.ReadFloat();
            splitter = buffer.ReadString();
            float posY = buffer.ReadFloat();
            splitter = buffer.ReadString();
            float posZ = buffer.ReadFloat();

            buffer.Dispose();

            for (int i = 0; i < General.client.Length; i++) {
                if (General.client[i].connectionID == connectionID) {

                    General.client[i].playerCharacter.posx = posX;
                    General.client[i].playerCharacter.posy = posY;
                    General.client[i].playerCharacter.posz = posZ;
                    return;
                }
            } 
        }
    }
}
