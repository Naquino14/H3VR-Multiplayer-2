// Decompiled with JetBrains decompiler
// Type: FistVR.CubeGameInfiniteAmmoToggle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class CubeGameInfiniteAmmoToggle : MonoBehaviour
  {
    public OptionsPanel_ButtonSet OBS_Ammo;

    private void Start() => this.OBS_Ammo.SetSelectedButton(0);

    public void SetAmmo(bool b)
    {
      if (b)
        GM.CurrentSceneSettings.IsAmmoInfinite = true;
      else
        GM.CurrentSceneSettings.IsAmmoInfinite = false;
    }
  }
}
