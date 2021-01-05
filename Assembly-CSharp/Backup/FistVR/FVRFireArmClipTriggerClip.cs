// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmClipTriggerClip
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRFireArmClipTriggerClip : MonoBehaviour
  {
    public FVRFireArmClip Clip;

    private void OnTriggerEnter(Collider collider)
    {
      if (!((Object) this.Clip != (Object) null) || !((Object) this.Clip.FireArm == (Object) null) || (!((Object) this.Clip.QuickbeltSlot == (Object) null) || !collider.gameObject.CompareTag("FVRFireArmClipReloadTriggerWell")))
        return;
      FVRFireArmClipTriggerWell component = collider.gameObject.GetComponent<FVRFireArmClipTriggerWell>();
      if (!((Object) component != (Object) null) || !((Object) component.FireArm != (Object) null) || (component.FireArm.ClipType != this.Clip.ClipType || (double) component.FireArm.ClipEjectDelay > 0.0) || !((Object) component.FireArm.Clip == (Object) null))
        return;
      this.Clip.Load(component.FireArm);
    }
  }
}
