// Decompiled with JetBrains decompiler
// Type: FistVR.ShatterableMeat
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ShatterableMeat : FVRPhysicalObject
  {
    public GameObject Splosion;
    public bool DoesDamage;

    public bool Explode()
    {
      if ((Object) this.m_hand != (Object) null)
        this.m_hand.EndInteractionIfHeld((FVRInteractiveObject) this);
      Object.Instantiate<GameObject>(this.Splosion, this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
      return this.DoesDamage;
    }
  }
}
