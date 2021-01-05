// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFusedThrowable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRFusedThrowable : FVRPhysicalObject, IFVRDamageable
  {
    public FVRFuse Fuse;

    public override int GetTutorialState() => this.Fuse.IsIgnited() ? 1 : 0;

    public void Damage(FistVR.Damage d)
    {
      if (!((Object) this.QuickbeltSlot == (Object) null) || (double) d.Dam_Thermal <= 0.0 && (double) d.Dam_TotalKinetic <= 100.0)
        return;
      this.Fuse.Ignite(Random.Range(0.1f, 0.8f));
    }
  }
}
