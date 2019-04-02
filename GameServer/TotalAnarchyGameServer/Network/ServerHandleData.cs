using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

namespace TotalAnarchyGameServer.Network {
    public class ServerHandleData {
        private delegate void Packet_(long connectionID, byte[] data);
        static Dictionary<long, Packet_> packets;
        static long pLength;

        public static void InitializePackets() {
            Logger.WriteInfo("Initializing Network Packets...");
            packets = new Dictionary<long, Packet_> {
                { (long)ClientPackets.CPlayerPosition, PACKET_PLAYER_POSITION },
                { (long)ClientPackets.CPlayerRotation, PACKET_PLAYER_ROTATION },
                { (long)ClientPackets.CPlayerLoaded, PACKET_PLAYER_LOADED },
                { (long)ClientPackets.CMatchmakeRequest, PACKET_MATCHMAKE },
                { (long)ClientPackets.CRegisterPlayer, PACKET_PLAYER_REGISTER },
                { (long)ClientPackets.CRequestWorldObjects, PACKET_REQUEST_WORLD_OBJECTS },
                { (long)ClientPackets.CPickedUpItem, PACKET_PLAYER_PICKED_UP_ITEM },
                { (long)ClientPackets.CPlayerShoot, PACKET_PLAYER_SHOOT },
                { (long)ClientPackets.CPlayerReload, PACKET_PLAYER_RELOAD },
                { (long)ClientPackets.CSwitchWep, PACKET_PLAYER_EQUIPPED_WEAPON },
                { (long)ClientPackets.CDamagedEnemy, PACKET_PLAYER_DAMAGED },
            };
        }

        public static void HandleData(long connectionID, byte[] data) {
            byte[] Buffer;
            Buffer = (byte[])data.Clone();


            if (General.client[connectionID].playerBuffer == null) {
                General.client[connectionID].playerBuffer = new ByteBuffer();
            }
            General.client[connectionID].playerBuffer.WriteBytes(Buffer);

            if (General.client[connectionID].playerBuffer.Count() == 0) {
                General.client[connectionID].playerBuffer.Clear();
                return;
            }

            if (General.client[connectionID].playerBuffer.Length() >= 8) {
                pLength = General.client[connectionID].playerBuffer.ReadLong(false);
                if (pLength <= 0) {
                    General.client[connectionID].playerBuffer.Clear();
                    return;
                }
            }

            while (pLength > 0 & pLength <= General.client[connectionID].playerBuffer.Length() - 8) {
                if (pLength <= General.client[connectionID].playerBuffer.Length() - 8) {
                    General.client[connectionID].playerBuffer.ReadLong();
                    data = General.client[connectionID].playerBuffer.ReadBytes((int)pLength);
                    HandleDataPackets(connectionID, data);
                }

                pLength = 0;

                if (General.client[connectionID].playerBuffer.Length() >= 8) {
                    pLength = General.client[connectionID].playerBuffer.ReadLong(false);

                    if (pLength < 0) {
                        General.client[connectionID].playerBuffer.Clear();
                        return;
                    }
                }
            }
        }

        static void HandleDataPackets(long connectionID, byte[] data) {
            //Logger.WriteInfo(data.Length.ToString());
            long packetIdentifier;
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            packetIdentifier = buffer.ReadLong();

            buffer.Dispose();

            if (packets.TryGetValue(packetIdentifier, out Packet_ packet)) {
                packet.Invoke(connectionID, data);
            }
        }

        #region RecievePackets
        static void PACKET_PLAYER_POSITION(long connectionID, byte[] data) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            long packet = buffer.ReadLong();
            int ID = buffer.ReadInteger();

            float x = buffer.ReadFloat();
            float y = buffer.ReadFloat();
            float z = buffer.ReadFloat();

            for (int i = 0; i < General.client.Length; i++) {
                if (General.client[i].connectionID == ID) {
                    General.client[i].playerCharacter.posx = x;
                    General.client[i].playerCharacter.posy = y;
                    General.client[i].playerCharacter.posz = z;
                    ServerTCP.PlayerPositions(General.client[i]);
                    return;
                }
            }
        }
        static void PACKET_PLAYER_ROTATION(long connectionID, byte[] data) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            long packet = buffer.ReadLong();
            int ID = buffer.ReadInteger();

            float x = buffer.ReadFloat();
            float y = buffer.ReadFloat();
            float z = buffer.ReadFloat();
            float w = buffer.ReadFloat();

            for (int i = 0; i < General.client.Length; i++) {
                if (General.client[i].connectionID == ID) {
                    General.client[i].playerCharacter.rotx = x;
                    General.client[i].playerCharacter.roty = y;
                    General.client[i].playerCharacter.rotz = z;
                    General.client[i].playerCharacter.rotw = w;

                    ServerTCP.PlayerRotations(General.client[i]);
                    return;
                }
            }

        }
        static void PACKET_PLAYER_LOADED(long connectionID, byte[] data) {
            for (int i = 0; i < General.client.Length; i++) {
                if (General.client[i].connectionID == connectionID) {
                    General.client[i].playerLoadedIn = true;
                    Logger.WriteDebug(General.client[i].connectionID + " Has loaded into the game");
                    return;
                }
            }
        }
        static void PACKET_PLAYER_REGISTER(long connectionID, byte[] data) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            long packet = buffer.ReadLong();
            string playerUsername = buffer.ReadString();

            General.client[connectionID].username = playerUsername;
            Logger.WriteDebug("Player Registered with username:" + playerUsername);
            buffer.Dispose();
        }
        static void PACKET_MATCHMAKE(long connectionID, byte[] data) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            General.Matchmake((int)connectionID);
        }
        static void PACKET_REQUEST_WORLD_OBJECTS(long connectionID, byte[] data) {
            Logger.WriteDebug("Player Requested world objects");


            for (int i = 0; i < General.client.Length; i++) {
                if (General.client[i].connectionID == connectionID) {
                    General.lobbys[General.client[i].lobbyIDConnectedTo].RequestWorldWeapons(General.client[i]);
                    Thread.Sleep(100); //splits the 2 messages apart enough for the client to process
                    General.lobbys[General.client[i].lobbyIDConnectedTo].RequetWorldAmmo(General.client[i]);
                    return;
                }
            }
        }
        static void PACKET_PLAYER_PICKED_UP_ITEM(long connectionID, byte[] data) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            long packet = buffer.ReadLong();
            int worldID = buffer.ReadInteger();
            long itemType = buffer.ReadLong();

            for (int i = 0; i < General.client.Length; i++) {
                if (General.client[i].connectionID == connectionID) {
                    if (itemType == 2) {
                        for (int x = 0; x < General.lobbys[General.client[i].lobbyIDConnectedTo].worldWeapons.Count; x++) {
                            if (General.lobbys[General.client[i].lobbyIDConnectedTo].worldWeapons[x].worldID == worldID) {
                                General.client[i].playerCharacter.playerInventory.currentEquippedWep = General.lobbys[General.client[i].lobbyIDConnectedTo].worldWeapons[x];
                                General.lobbys[General.client[i].lobbyIDConnectedTo].worldWeapons[x].isDropped = false;
                                General.lobbys[General.client[i].lobbyIDConnectedTo].UpdateAmmo(General.client[i]);
                                break;
                            }
                        }
                    } else {
                        long ammoType = buffer.ReadLong();
                        int amount = buffer.ReadInteger();

                        for (int x = 0; x < General.lobbys[General.client[i].lobbyIDConnectedTo].worldAmmo.Count; x++) {
                            if (General.lobbys[General.client[i].lobbyIDConnectedTo].worldAmmo[x].worldID == worldID) {
                                General.lobbys[General.client[i].lobbyIDConnectedTo].worldAmmo[x].isDropped = false;

                                switch (ammoType) {
                                    case 1://Pistol Ammo
                                        Logger.WriteDebug("Picked up pistol ammo: " + amount);
                                        General.client[i].playerCharacter.playerInventory.pistolRounds += amount;
                                        break;
                                    case 2://Shotgun Ammo
                                        Logger.WriteDebug("Picked up shotgun ammo: " + amount);
                                        General.client[i].playerCharacter.playerInventory.shotgunRounds += amount;
                                        break;
                                    case 3://Rifle Ammo
                                        Logger.WriteDebug("Picked up rifle ammo: " + amount);
                                        General.client[i].playerCharacter.playerInventory.rifleRounds += amount;
                                        break;
                                }
                                General.lobbys[General.client[i].lobbyIDConnectedTo].UpdateAmmo(General.client[i]);
                                break;
                            }
                        }
                    }

                    buffer.Dispose();
                    buffer.WriteLong((long)ServerPackets.SPlayerPickedUpItem);
                    buffer.WriteInteger(worldID);

                    ServerTCP.SendToAllInLobby(General.lobbys[General.client[i].lobbyIDConnectedTo], buffer.ToArray());
                    return;
                }
            }
        }
        static void PACKET_PLAYER_SHOOT(long connectionID, byte[] data) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            long packet = buffer.ReadLong();
            string wepName = buffer.ReadString();
            for (int i = 0; i < General.client.Length; i++) {
                if (General.client[i].connectionID == connectionID) {
                    General.lobbys[General.client[i].lobbyIDConnectedTo].PlayerShoot(General.client[i], wepName);
                    return;
                }
            }
        }
        static void PACKET_PLAYER_RELOAD(long connectionID, byte[] data) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            long packet = buffer.ReadLong();
            string wepName = buffer.ReadString();

            for (int i = 0; i < General.client.Length; i++) {
                if (General.client[i].connectionID == connectionID && wepName == General.client[i].playerCharacter.playerInventory.currentEquippedWep.weaponName) {
                    General.lobbys[General.client[i].lobbyIDConnectedTo].Reload(General.client[i]);
                }
            }
        }
        static void PACKET_PLAYER_EQUIPPED_WEAPON(long connectionID, byte[] data) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            long packet = buffer.ReadLong();
            string wepName = buffer.ReadString();

            Logger.WriteDebug("Player Equipped Weapon: " + wepName);

            for (int i = 0; i < General.client.Length; i++) {
                if (General.client[i].connectionID == connectionID) {
                    General.lobbys[General.client[i].lobbyIDConnectedTo].PlayerEquippedWep(General.client[i], wepName);
                    return;
                }
            }
        }

        static void PACKET_PLAYER_DAMAGED(long connectionID, byte[] data) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            long packet = buffer.ReadLong();
            int ID = buffer.ReadInteger();
            int hitPlayer = buffer.ReadInteger();
            int damageAmount = buffer.ReadInteger();

            Logger.WriteInfo("Player:" + ID + " Hit Player: " + hitPlayer + " for: " + damageAmount);
            for (int i = 0; i < General.client.Length; i++) {
                if (hitPlayer == General.client[i].connectionID) {
                    General.client[i].playerCharacter.TakeDamage(damageAmount, General.client[i]);
                    return;
                }
            }
        }
        #endregion
    }
}
