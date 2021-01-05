// Decompiled with JetBrains decompiler
// Type: WFX_Demo_DeleteAfterDelay
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class WFX_Demo_DeleteAfterDelay : MonoBehaviour
{
  public float delay = 1f;

  private void Update()
  {
    this.delay -= Time.deltaTime;
    if ((double) this.delay >= 0.0)
      return;
    Object.Destroy((Object) this.gameObject);
  }
}
