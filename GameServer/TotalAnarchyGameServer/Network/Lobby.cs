using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TotalAnarchyGameServer.Player;
using TotalAnarchyGameServer.Weapons;
using TotalAnarchyGameServer.Weapons.Ammo;
using System.Diagnostics;

namespace TotalAnarchyGameServer.Network {
    public class Lobby {
        public List<Clients> clientsInLobby = new List<Clients>(new Clients[Constants.MAX_PLAYERS_PER_LOBBY]);
        public int lobbyID;
        public int connectedClients = 0;

        public WeaponManager weaponManager;
        public List<Weapon> worldWeapons;
        public List<Ammo> worldAmmo;

        private int worldIDs = 0;
        

        public void InitializeLobby() {
            weaponManager = new WeaponManager();
            worldWeapons = new List<Weapon>();
            worldAmmo = new List<Ammo>();
            //generate weapon and ammo positions

            GenerateWeapons();
            GenerateAmmo();
            //run the loop
            LobbyLoop();
        }

        void GenerateWeapons() {
            Random rndWeapon = new Random();
            for (int i = 0; i < Constants.WEAPON_SPAWN_POINTS; i++) {
                int weapon = rndWeapon.Next(0, WeaponManager.weapons.Count);
                Weapon newWeapon = null;
                switch (weapon) {
                    case 0:
                        newWeapon = new Rifle();
                        break;
                    case 1:
                        newWeapon = new Shotgun();
                        break;
                    case 2:
                        newWeapon = new Pistol();
                        break;
                    case 3:
                        newWeapon = new Knife();
                        break;
                    case 4:
                        newWeapon = new Grenade();
                        break;
                }

                newWeapon.isDropped = true;
                newWeapon.worldSpawn = i;
                newWeapon.worldID = worldIDs;
                worldIDs++;
                worldWeapons.Add(newWeapon); // need to make a new instance
                Logger.WriteDebug(worldIDs.ToString() + " " + newWeapon.weaponName);
            }
        }
        void GenerateAmmo() {
            Random rndAmmo = new Random();
            for (int i = 0; i < Constants.AMMO_SPAWN_POINTS; i++) {
                int ammo = rndAmmo.Next(0, WeaponManager.ammoTypes.Count);
                Ammo newAmmo = null;
                switch (ammo) {
                    case 0:
                        newAmmo = new RifleAmmo();
                        break;
                    case 1:
                        newAmmo = new PistolAmmo();
                        break;
                    case 2:
                        newAmmo = new ShotgunAmmo();
                        break;
                }

                newAmmo.isDropped = true;
                newAmmo.worldSpawn = i;
                newAmmo.worldID = worldIDs;
                worldIDs++;
                worldAmmo.Add(newAmmo); // need to make a new instance
                Logger.WriteDebug(worldIDs.ToString() + " " + newAmmo.ammoName);
            }
        }
        public void LobbyLoop() {
            while (true) {
                UpdatePlayerPositions();
                UpdatePlayerRotations();
                Thread.Sleep(Constants.LOBBY_TICK_RATE);
            }
        }

        public void JoinLobby(Clients client) {
            for (int i = 0; i < clientsInLobby.Count; i++) {
                if (clientsInLobby[i].socket == null) {
                    clientsInLobby[i] = client;
                    break;
                }
            }

            //Join this game
            client.lobbyIDConnectedTo = lobbyID;
            connectedClients++;
            Logger.WriteDebug("Player Joined lobby with ID: " + client.connectionID.ToString());
            SendWelcomeMessage(client);
            SendInWorld(client);
        }               
        public void SendWelcomeMessage(Clients client) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SWelcomeMessage); //Packet Identifier.
            buffer.WriteInteger(client.connectionID);
            buffer.WriteString("Welcome to Total Anarchy. You are connected to Development Server. Lobby No: " + lobbyID);
            ServerTCP.SendDataTo(client.connectionID, buffer.ToArray());
            buffer.Dispose();
        }
        public void SendInWorld(Clients client) {
            for (int i = 0; i < clientsInLobby.Count; i++) {
                if (i != client.connectionID && clientsInLobby[i].socket != null) { //gets all the other players
                    ServerTCP.SendDataTo(client.connectionID, ServerTCP.PlayerData(clientsInLobby[i].connectionID));
                }
            }
            //sends the connectionID player data to everyone including himself
            ServerTCP.SendToAllInLobby(this, ServerTCP.PlayerData(client.connectionID));
        }

        public byte[] LobbyPlayerData(Clients client) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SPlayerData);
            buffer.WriteInteger(connectedClients);

            Logger.WriteDebug(connectedClients.ToString());
            for (int i = 0; i < clientsInLobby.Count; i++) {
                if (clientsInLobby[i] != client && clientsInLobby[i].socket != null) { //downloads all players currently on the lobby
                    buffer.WriteInteger(clientsInLobby[i].connectionID);
                    //random spawn points
                    Random rnd = new Random();
                    int spawn = rnd.Next(0, Constants.PLAYER_SPAWN_POINTS - 1);
                    buffer.WriteInteger(spawn);

                    buffer.WriteFloat(clientsInLobby[i].playerCharacter.posx);
                    buffer.WriteFloat(clientsInLobby[i].playerCharacter.posy);
                    buffer.WriteFloat(clientsInLobby[i].playerCharacter.posz);
                    buffer.WriteFloat(clientsInLobby[i].playerCharacter.rotx);
                    buffer.WriteFloat(clientsInLobby[i].playerCharacter.roty);
                    buffer.WriteFloat(clientsInLobby[i].playerCharacter.rotz);
                    buffer.WriteFloat(clientsInLobby[i].playerCharacter.rotw);

                } else {
                    buffer.WriteInteger(clientsInLobby[i].connectionID);
                    //random spawn points
                    Random rnd = new Random();
                    int spawn = rnd.Next(0, Constants.PLAYER_SPAWN_POINTS - 1);
                    buffer.WriteInteger(spawn);
                }

            }
            return buffer.ToArray();
        }

        public void UpdatePlayerPositions() {
            //Logger.WriteDebug("Updating Player Positions");
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SPlayerPosition);
            buffer.WriteInteger(connectedClients);
            for (int i = 0; i < clientsInLobby.Count; i++) {
                if (clientsInLobby[i].socket != null) {
                    buffer.WriteInteger(clientsInLobby[i].connectionID);
                    buffer.WriteFloat(clientsInLobby[i].playerCharacter.posx);
                    buffer.WriteFloat(clientsInLobby[i].playerCharacter.posy);
                    buffer.WriteFloat(clientsInLobby[i].playerCharacter.posz);
                }
            }

            for (int i = 0; i < clientsInLobby.Count; i++) {
                if (clientsInLobby[i].socket != null && clientsInLobby[i].playerLoadedIn) {
                    ServerTCP.SendDataTo(clientsInLobby[i].connectionID, buffer.ToArray());
                }
            }
        }
        public void UpdatePlayerRotations() {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SPlayerRotation);
            buffer.WriteInteger(connectedClients);
            for (int i = 0; i < clientsInLobby.Count; i++) {
                if (clientsInLobby[i].socket != null) {
                    buffer.WriteInteger(clientsInLobby[i].connectionID);
                    buffer.WriteFloat(clientsInLobby[i].playerCharacter.rotx);
                    buffer.WriteFloat(clientsInLobby[i].playerCharacter.roty);
                    buffer.WriteFloat(clientsInLobby[i].playerCharacter.rotz);
                    buffer.WriteFloat(clientsInLobby[i].playerCharacter.rotw);
                }
            }

            for (int i = 0; i < clientsInLobby.Count; i++) {
                if (clientsInLobby[i].socket != null && clientsInLobby[i].playerLoadedIn) {
                    ServerTCP.SendDataTo(clientsInLobby[i].connectionID, buffer.ToArray());
                }
            }

        }
        public void RequestWorldWeapons(Clients client) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SWorldWeapons);
            for (int i = 0; i < worldWeapons.Count; i++) {
                if (worldWeapons[i].isDropped) {
                    buffer.WriteInteger(worldWeapons[i].worldID);
                    buffer.WriteInteger(worldWeapons[i].weaponID);
                    buffer.WriteString(worldWeapons[i].weaponName);
                    buffer.WriteInteger(worldWeapons[i].worldSpawn);
                    Thread.Sleep(100);
                }
            }

            Logger.WriteDebug("Sending Weapon Values to: " + client.connectionID);
            ServerTCP.SendDataTo(client.connectionID, buffer.ToArray());
        }
        public void RequetWorldAmmo(Clients client) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SWorldAmmo);
            for (int i = 0; i < worldAmmo.Count; i++) {
                if (worldAmmo[i].isDropped) {
                    buffer.WriteInteger(worldAmmo[i].worldID);
                    buffer.WriteInteger(worldAmmo[i].ammoID);
                    buffer.WriteString(worldAmmo[i].ammoName);
                    buffer.WriteInteger(worldAmmo[i].worldSpawn);
                }
            }
            ServerTCP.SendDataTo(client.connectionID, buffer.ToArray());
        }
        public void PlayerShoot(Clients client, string WepName) {
            ByteBuffer buffer = new ByteBuffer();
            try {
                if (WepName == client.playerCharacter.playerInventory.currentEquippedWep.weaponName) {
                    if (client.playerCharacter.playerInventory.currentEquippedWep.currentAmmo == 0) {
                        return;
                    }
                    client.playerCharacter.playerInventory.currentEquippedWep.currentAmmo--;
                }


                ServerTCP.SendToAllInLobbyBut(this, client.connectionID, buffer.ToArray());

                //update the ammo for the player
                UpdateAmmo(client);
            } catch (Exception) {
            }
        }
        public void Reload(Clients client) {

            switch (client.playerCharacter.playerInventory.currentEquippedWep.ammoType) {
                case AmmoType.pistol:
                    client.playerCharacter.playerInventory.currentEquippedWep.currentAmmo = client.playerCharacter.playerInventory.currentEquippedWep.ammoPerClip;
                    client.playerCharacter.playerInventory.pistolRounds -= client.playerCharacter.playerInventory.currentEquippedWep.ammoPerClip;
                    break;
                case AmmoType.shotgun:
                    client.playerCharacter.playerInventory.currentEquippedWep.currentAmmo++;
                    client.playerCharacter.playerInventory.shotgunRounds--;
                    break;
                case AmmoType.rifle:
                    client.playerCharacter.playerInventory.currentEquippedWep.currentAmmo = client.playerCharacter.playerInventory.currentEquippedWep.ammoPerClip;
                    client.playerCharacter.playerInventory.rifleRounds -= client.playerCharacter.playerInventory.currentEquippedWep.ammoPerClip;
                    break;
            }
            UpdateAmmo(client);
        }
        public void UpdateAmmo(Clients client) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.SUpdateAmmo);

            if (client.playerCharacter.playerInventory.currentEquippedWep == null) {
                buffer.WriteString("-");
                buffer.WriteString("-");
            } else {
                buffer.WriteString(client.playerCharacter.playerInventory.currentEquippedWep.currentAmmo.ToString());

                switch (client.playerCharacter.playerInventory.currentEquippedWep.ammoType) {
                    case AmmoType.pistol:
                        buffer.WriteString(client.playerCharacter.playerInventory.pistolRounds.ToString());
                        break;
                    case AmmoType.shotgun:
                        buffer.WriteString(client.playerCharacter.playerInventory.shotgunRounds.ToString());
                        break;
                    case AmmoType.rifle:
                        buffer.WriteString(client.playerCharacter.playerInventory.rifleRounds.ToString());
                        break;
                }
            }
            ServerTCP.SendDataTo(client.connectionID, buffer.ToArray());
        }
        public void PlayerEquippedWep(Clients client, string wepToEquip) {
            for (int i = 0; i < WeaponManager.weapons.Count; i++) {
                if (WeaponManager.weapons[i].weaponName == wepToEquip) {
                    client.playerCharacter.playerInventory.currentEquippedWep = WeaponManager.weapons[i];
                    //Tell other clients in lobby
                    ByteBuffer buffer = new ByteBuffer();
                    buffer.WriteLong((long)ServerPackets.SPlayerSwitchedWeapon);
                    buffer.WriteInteger(client.connectionID);
                    buffer.WriteString(wepToEquip);

                    ServerTCP.SendToAllInLobbyBut(this, client.connectionID, buffer.ToArray());
                    Logger.WriteDebug("Player equipped wep");
                    return;
                }
            }

        }
    
    }
}

