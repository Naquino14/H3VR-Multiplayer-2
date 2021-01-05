// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_CameraMask
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR
{
  [ExecuteInEditMode]
  public class SteamVR_CameraMask : MonoBehaviour
  {
    private void Awake()
    {
      Debug.Log((object) "<b>[SteamVR]</b> SteamVR_CameraMask is deprecated in Unity 5.4 - REMOVING");
      Object.DestroyImmediate((Object) this);
    }
  }
}
