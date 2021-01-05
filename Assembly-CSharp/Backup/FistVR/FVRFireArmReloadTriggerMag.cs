// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmReloadTriggerMag
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRFireArmReloadTriggerMag : MonoBehaviour
  {
    public FVRFireArmMagazine Magazine;

    private void OnTriggerEnter(Collider collider)
    {
      if (!((Object) this.Magazine != (Object) null) || !((Object) this.Magazine.FireArm == (Object) null) || (!((Object) this.Magazine.QuickbeltSlot == (Object) null) || !(collider.gameObject.tag == "FVRFireArmReloadTriggerWell")))
        return;
      FVRFireArmReloadTriggerWell component = collider.gameObject.GetComponent<FVRFireArmReloadTriggerWell>();
      bool flag = false;
      if ((Object) component != (Object) null && !this.Magazine.IsBeltBox && component.FireArm.HasBelt)
        flag = true;
      if (!((Object) component != (Object) null) || component.IsBeltBox != this.Magazine.IsBeltBox || (!((Object) component.FireArm != (Object) null) || !((Object) component.FireArm.Magazine == (Object) null)) || flag)
        return;
      FireArmMagazineType fireArmMagazineType = component.FireArm.MagazineType;
      if (component.UsesTypeOverride)
        fireArmMagazineType = component.TypeOverride;
      if (fireArmMagazineType != this.Magazine.MagazineType || (double) component.FireArm.EjectDelay > 0.0 && !((Object) this.Magazine != (Object) component.FireArm.LastEjectedMag) || !((Object) component.FireArm.Magazine == (Object) null))
        return;
      this.Magazine.Load(component.FireArm);
    }
  }
}
