// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.DestroyOnTriggerEnter
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class DestroyOnTriggerEnter : MonoBehaviour
  {
    public string tagFilter;
    private bool useTag;

    private void Start()
    {
      if (string.IsNullOrEmpty(this.tagFilter))
        return;
      this.useTag = true;
    }

    private void OnTriggerEnter(Collider collider)
    {
      if (this.useTag && (!this.useTag || !(collider.gameObject.tag == this.tagFilter)))
        return;
      Object.Destroy((Object) collider.gameObject.transform.root.gameObject);
    }
  }
}
