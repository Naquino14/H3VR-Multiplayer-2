// Decompiled with JetBrains decompiler
// Type: FistVR.FireArmRoundClass
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FistVR
{
  public enum FireArmRoundClass
  {
    Ball = 0,
    FMJ = 1,
    JHP = 2,
    SP = 3,
    Tracer = 4,
    AP = 5,
    Incendiary = 6,
    APIncendiary = 7,
    Spitzer = 8,
    BTSP = 9,
    HighVelHP = 10, // 0x0000000A
    HyperVelHP = 11, // 0x0000000B
    Slug = 12, // 0x0000000C
    BuckShot00 = 13, // 0x0000000D
    BuckShot000 = 14, // 0x0000000E
    FragExplosive = 15, // 0x0000000F
    Flechette = 16, // 0x00000010
    DragonsBreath = 17, // 0x00000011
    BuckShotNo2 = 18, // 0x00000012
    BuckShotNo4 = 19, // 0x00000013
    TripleHit = 20, // 0x00000014
    Freedomfetti = 21, // 0x00000015
    Frag12HE = 22, // 0x00000016
    Frag12FA = 23, // 0x00000017
    Flare = 24, // 0x00000018
    Cannonball = 25, // 0x00000019
    Mk211 = 26, // 0x0000001A
    BuckShotNo1 = 27, // 0x0000001B
    Double = 28, // 0x0000001C
    DSM_Frag = 30, // 0x0000001E
    DSM_Mag = 31, // 0x0000001F
    DSM_Mine = 32, // 0x00000020
    DSM_Slugger = 33, // 0x00000021
    DSM_Swarm = 34, // 0x00000022
    DSM_Tracer = 35, // 0x00000023
    DSM_TurboPenetrator = 36, // 0x00000024
    DSM_Volt = 37, // 0x00000025
    PlusP_FMJ = 40, // 0x00000028
    PlusP_JHP = 41, // 0x00000029
    PlusP_API = 42, // 0x0000002A
    NERMAL = 50, // 0x00000032
    SPESHUL = 51, // 0x00000033
    FLASHY = 52, // 0x00000034
    MEGA = 53, // 0x00000035
    BOOOMY = 54, // 0x00000036
    POINTYOWW = 55, // 0x00000037
    Mortar = 60, // 0x0000003C
    MIRV = 61, // 0x0000003D
    MegaBuckShot = 62, // 0x0000003E
    M381_HighExplosive = 70, // 0x00000046
    M397_AirBurst = 71, // 0x00000047
    M576_MPAPERS = 72, // 0x00000048
    M651_CSGAS = 73, // 0x00000049
    M781_Practice = 74, // 0x0000004A
    X214_SteelBreaker = 75, // 0x0000004B
    X477_CornerFrag = 76, // 0x0000004C
    X666_Baphomet = 77, // 0x0000004D
    X828_Aurora = 78, // 0x0000004E
    X1776_FreedomParty = 79, // 0x0000004F
    KS23_Buckshot = 90, // 0x0000005A
    KS23_Barricade = 91, // 0x0000005B
    KS23_CSGas = 92, // 0x0000005C
    KS23_Flash = 93, // 0x0000005D
    Subsonic_FMJ = 100, // 0x00000064
    Subsonic_AP = 101, // 0x00000065
    Kol_Frag = 110, // 0x0000006E
    Kol_HEAT = 111, // 0x0000006F
    Kol_Inferno = 112, // 0x00000070
    Kol_Megabuck = 113, // 0x00000071
    Kol_Smokescreen = 114, // 0x00000072
    Kol_TriFlash = 115, // 0x00000073
    RLV_HEF = 120, // 0x00000078
    RLV_HEFJ = 121, // 0x00000079
    RLV_SMK = 122, // 0x0000007A
    RLV_SF1 = 123, // 0x0000007B
    RLV_TPM = 124, // 0x0000007C
    MF366_Retort = 130, // 0x00000082
    MF366_Debuff = 131, // 0x00000083
    MF366_Salute = 132, // 0x00000084
    MF1850_Barbie = 140, // 0x0000008C
    MF1850_Drongo = 141, // 0x0000008D
    MF1850_Gobsmacka = 142, // 0x0000008E
    MF1232_Bushfire = 150, // 0x00000096
    MF1232_FunnelSpider = 151, // 0x00000097
    MFRPG_Classic = 160, // 0x000000A0
    MFRPG_RocketPop = 161, // 0x000000A1
    MFRPG_ToTheMoon = 162, // 0x000000A2
    MFRPG_RockIt = 163, // 0x000000A3
    MFRPG_CannedMeat = 164, // 0x000000A4
    MFRPG_WRONGAMMO = 165, // 0x000000A5
    MF13g_Buck = 170, // 0x000000AA
    MF13g_Slugger = 171, // 0x000000AB
    MF13g_Blooper = 172, // 0x000000AC
    MF13g_Bleeder = 173, // 0x000000AD
    MF13g_Moonshot = 174, // 0x000000AE
    a20FMJ = 180, // 0x000000B4
    a20HE = 181, // 0x000000B5
    a20HEI = 182, // 0x000000B6
    a20SAPHEI = 183, // 0x000000B7
    a20APDS = 184, // 0x000000B8
    a20AP = 185, // 0x000000B9
    MFStickyFrag = 190, // 0x000000BE
    MFStickyRobbieBurns = 191, // 0x000000BF
    MFStickyRustyNail = 192, // 0x000000C0
    MFStickyHighlandFling = 193, // 0x000000C1
  }
}
