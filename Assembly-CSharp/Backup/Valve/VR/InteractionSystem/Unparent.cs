// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Unparent
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class Unparent : MonoBehaviour
  {
    private Transform oldParent;

    private void Start()
    {
      this.oldParent = this.transform.parent;
      this.transform.parent = (Transform) null;
      this.gameObject.name = this.oldParent.gameObject.name + "." + this.gameObject.name;
    }

    private void Update()
    {
      if (!((Object) this.oldParent == (Object) null))
        return;
      Object.Destroy((Object) this.gameObject);
    }

    public Transform GetOldParent() => this.oldParent;
  }
}
