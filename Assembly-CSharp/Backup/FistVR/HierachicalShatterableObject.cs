// Decompiled with JetBrains decompiler
// Type: FistVR.HierachicalShatterableObject
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class HierachicalShatterableObject : FVRShatterableObject
  {
    [Header("Hierachichal elements")]
    [HideInInspector]
    private List<HierachicalShatterableObject> Parents = new List<HierachicalShatterableObject>();
    public HierachicalShatterableObject[] Children;
    public HierachicalShatterableObject Root;
    public HierachicalShatterableObject[] RootsMasterList;

    public override void Awake()
    {
      base.Awake();
      for (int index = 0; index < this.Children.Length; ++index)
      {
        if (!this.Children[index].Parents.Contains(this))
          this.Children[index].Parents.Add(this);
      }
    }

    private void DetachAllChildrenIfDisconnected(Vector3 point, Vector3 force, int points)
    {
      for (int index = 0; index < this.Children.Length; ++index)
      {
        if (!((Object) this.Children[index] != (Object) null))
          ;
        if ((Object) this.Children[index] != (Object) null)
          this.Children[index].DetachAllChildrenIfDisconnected(this.transform.position, force * 0.8f, 2);
      }
      this.GoNonKinematic(point, force);
    }

    public override void Destroy(Vector3 damagePoint, Vector3 damageDir)
    {
      this.DetachAllChildrenIfDisconnected(damagePoint, damageDir, 5);
      base.Destroy(damagePoint, damageDir);
    }
  }
}
