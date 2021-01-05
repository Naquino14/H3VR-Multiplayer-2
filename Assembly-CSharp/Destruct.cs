// Decompiled with JetBrains decompiler
// Type: Destruct
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class Destruct : MonoBehaviour
{
  public GameObject fracturedPrefab;
  public GameObject smokeParticle;
  public AudioClip shatter;

  private void Start()
  {
    if (!((Object) this.fracturedPrefab == (Object) null))
      return;
    Debug.LogError((object) ("Fractured prefab not assigned for object: " + this.gameObject.name));
  }
}
