// Decompiled with JetBrains decompiler
// Type: FistVR.PUM
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class PUM : ManagerSingleton<PUM>
  {
    public List<GameObject> PowerupVFX_Player_Positive;
    public List<GameObject> PowerupVFX_Player_Negative;
    public List<GameObject> PowerupVFXByIndex_Bot_Positive;
    public List<GameObject> PowerupVFXByIndex_Bot_Negative;
    public List<GameObject> PowerupCloud;
    public GameObject Sosig_Barfer;
    public GameObject Sosig_Cyclops;
    public GameObject Sosig_Biclops;

    protected override void Awake() => base.Awake();

    public static bool HasEffectBot(int index, bool isInverted) => isInverted ? ManagerSingleton<PUM>.Instance.PowerupVFXByIndex_Bot_Negative.Count >= index + 1 && !((Object) ManagerSingleton<PUM>.Instance.PowerupVFXByIndex_Bot_Negative[index] == (Object) null) : ManagerSingleton<PUM>.Instance.PowerupVFXByIndex_Bot_Positive.Count >= index + 1 && !((Object) ManagerSingleton<PUM>.Instance.PowerupVFXByIndex_Bot_Positive[index] == (Object) null);

    public static bool HasEffectPlayer(int index, bool isInverted) => isInverted ? index <= ManagerSingleton<PUM>.Instance.PowerupVFX_Player_Negative.Count - 1 && !((Object) ManagerSingleton<PUM>.Instance.PowerupVFX_Player_Negative[index] == (Object) null) : index <= ManagerSingleton<PUM>.Instance.PowerupVFX_Player_Positive.Count - 1 && !((Object) ManagerSingleton<PUM>.Instance.PowerupVFX_Player_Positive[index] == (Object) null);

    public static GameObject GetEffect(int index, bool isInverted) => isInverted ? ManagerSingleton<PUM>.Instance.PowerupVFXByIndex_Bot_Negative[index] : ManagerSingleton<PUM>.Instance.PowerupVFXByIndex_Bot_Positive[index];

    public static GameObject GetEffectPlayer(int index, bool isInverted) => isInverted ? ManagerSingleton<PUM>.Instance.PowerupVFX_Player_Negative[index] : ManagerSingleton<PUM>.Instance.PowerupVFX_Player_Positive[index];
  }
}
