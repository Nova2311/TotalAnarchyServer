public enum ServerPackets {
    SWelcomeMessage = 1,
    SInGame,
    SPlayerData,
    SPlayerPosition,
    SPlayerRotation,
    SWorldWeapons,
    SWorldAmmo,
    SPlayerPickedUpItem,
    SUpdateAmmo,
    SPlayerShot,
    SPlayerSwitchedWeapon,
    SPlayerDied,
    SPlayerDisconnected,
}

public enum ClientPackets {
    CPlayerPosition = 1,
    CPlayerRotation,
    CPlayerLoaded,
    CRegisterPlayer,
    CMatchmakeRequest,
    CRequestWorldObjects,
    CPickedUpItem,
    CPlayerReload,
    CPlayerShoot,
    CDamagedEnemy,
    CSwitchWep,

}

public enum ItemType {
    Ammo = 1,
    Weapon,
}

public enum AmmoType {
    pistol = 1,
    shotgun,
    rifle,
}