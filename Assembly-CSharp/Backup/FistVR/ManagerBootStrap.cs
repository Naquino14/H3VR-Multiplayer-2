// Decompiled with JetBrains decompiler
// Type: FistVR.ManagerBootStrap
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public static class ManagerBootStrap
  {
    private static GameObject ManagerGO;

    public static void BootStrap()
    {
      if (!((Object) ManagerBootStrap.ManagerGO == (Object) null))
        return;
      ManagerBootStrap.ManagerGO = Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/_Managers/_GameManager"));
      Object.DontDestroyOnLoad((Object) ManagerBootStrap.ManagerGO);
    }
  }
}
