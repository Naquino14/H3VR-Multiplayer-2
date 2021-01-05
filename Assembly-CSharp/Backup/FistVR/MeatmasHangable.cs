// Decompiled with JetBrains decompiler
// Type: FistVR.MeatmasHangable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MeatmasHangable : FVRPhysicalObject
  {
    private SaveableTreeSystem Tree;
    public HangableDef Def;

    protected override void Awake() => base.Awake();

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      if (!((Object) this.Tree != (Object) null))
        return;
      if ((double) Vector3.Distance(new Vector3(this.transform.position.x, 0.0f, this.transform.position.z), new Vector3(this.Tree.transform.position.x, 0.0f, this.Tree.transform.position.z)) <= 2.5)
        this.SetIsKinematicLocked(true);
      else
        this.SetIsKinematicLocked(false);
    }
  }
}
