// Decompiled with JetBrains decompiler
// Type: FistVR.RomanCandlePayload
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RomanCandlePayload : MonoBehaviour
  {
    public Color FlashColor;

    private void Awake()
    {
      if (GM.CurrentSceneSettings.IsSceneLowLight)
        FXM.InitiateMuzzleFlash(this.transform.position, -Vector3.up, 30f, this.FlashColor, 150f);
      else
        FXM.InitiateMuzzleFlash(this.transform.position, -Vector3.up, 2f, this.FlashColor, 3f);
    }
  }
}
