// Decompiled with JetBrains decompiler
// Type: FistVR.LAPD2019BatteryTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class LAPD2019BatteryTrigger : MonoBehaviour
  {
    public LAPD2019Battery Battery;
    public LAPD2019BatteryTriggerWell WellTrigger;

    private void OnTriggerEnter(Collider collider)
    {
      if (!((Object) this.Battery != (Object) null) || !((Object) this.Battery.QuickbeltSlot == (Object) null) || !(collider.gameObject.tag == "FVRFireArmMagazineReloadTrigger"))
        return;
      LAPD2019BatteryTriggerWell component = collider.gameObject.GetComponent<LAPD2019BatteryTriggerWell>();
      if (!((Object) component != (Object) null))
        return;
      this.WellTrigger = component;
    }

    private void OnTriggerExit(Collider collider)
    {
      if (!((Object) this.WellTrigger != (Object) null) || !(collider.gameObject.tag == "FVRFireArmMagazineReloadTrigger") || !((Object) collider.gameObject.GetComponent<LAPD2019BatteryTriggerWell>() == (Object) this.WellTrigger))
        return;
      this.WellTrigger = (LAPD2019BatteryTriggerWell) null;
    }

    private void Update()
    {
      if (!((Object) this.WellTrigger != (Object) null))
        return;
      this.LoadMag();
    }

    private void LoadMag()
    {
      if (!this.WellTrigger.Gun.LoadBattery(this.Battery))
        return;
      Object.Destroy((Object) this.Battery.gameObject);
    }
  }
}
