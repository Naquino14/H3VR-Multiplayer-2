// Decompiled with JetBrains decompiler
// Type: AmplifyColorVolume
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[RequireComponent(typeof (BoxCollider))]
[AddComponentMenu("Image Effects/Amplify Color Volume")]
public class AmplifyColorVolume : AmplifyColorVolumeBase
{
  private void OnTriggerEnter(Collider other)
  {
    AmplifyColorTriggerProxy component = other.GetComponent<AmplifyColorTriggerProxy>();
    if (!((Object) component != (Object) null) || !component.OwnerEffect.UseVolumes || ((int) component.OwnerEffect.VolumeCollisionMask & 1 << this.gameObject.layer) == 0)
      return;
    component.OwnerEffect.EnterVolume((AmplifyColorVolumeBase) this);
  }

  private void OnTriggerExit(Collider other)
  {
    AmplifyColorTriggerProxy component = other.GetComponent<AmplifyColorTriggerProxy>();
    if (!((Object) component != (Object) null) || !component.OwnerEffect.UseVolumes || ((int) component.OwnerEffect.VolumeCollisionMask & 1 << this.gameObject.layer) == 0)
      return;
    component.OwnerEffect.ExitVolume((AmplifyColorVolumeBase) this);
  }
}
