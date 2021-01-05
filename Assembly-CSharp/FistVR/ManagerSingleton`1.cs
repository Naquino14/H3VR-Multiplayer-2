// Decompiled with JetBrains decompiler
// Type: FistVR.ManagerSingleton`1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ManagerSingleton<T> : MonoBehaviour where T : MonoBehaviour
  {
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
      if ((Object) ManagerSingleton<T>.Instance == (Object) null)
        ManagerSingleton<T>.Instance = (object) this as T;
      else
        Debug.LogError((object) "wtf Anton MANAGER IN SCENE BEEEP BEEEEP BEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEPPPP;");
    }
  }
}
