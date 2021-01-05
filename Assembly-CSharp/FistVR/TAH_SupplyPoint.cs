// Decompiled with JetBrains decompiler
// Type: FistVR.TAH_SupplyPoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TAH_SupplyPoint : MonoBehaviour
  {
    public Transform PlayerSpawnPoint;
    public Transform SpawnPos_CrateLarge;
    public Transform SpawnPos_CrateSmall;
    public TAH_WeaponCrate CrateSmall;
    public TAH_WeaponCrate CrateLarge;
    public Transform SpawnPoint_Large1;
    public Transform SpawnPoint_Large2;
    public Transform SpawnPoint_MeleeWeapon;
    public Transform SpawnPos_PowerUp;
    public Transform[] BotSpawnPoints;
    public Transform[] BotAttackSpawnPoints;
    public wwBotWurstNavPointGroup NavGroup;
  }
}
