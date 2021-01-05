// Decompiled with JetBrains decompiler
// Type: AmplifyColorVolume2D
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[RequireComponent(typeof (BoxCollider2D))]
[AddComponentMenu("Image Effects/Amplify Color Volume 2D")]
public class AmplifyColorVolume2D : AmplifyColorVolumeBase
{
  private void OnTriggerEnter2D(Collider2D other)
  {
    AmplifyColorTriggerProxy2D component = other.GetComponent<AmplifyColorTriggerProxy2D>();
    if (!((Object) component != (Object) null) || !component.OwnerEffect.UseVolumes || ((int) component.OwnerEffect.VolumeCollisionMask & 1 << this.gameObject.layer) == 0)
      return;
    component.OwnerEffect.EnterVolume((AmplifyColorVolumeBase) this);
  }

  private void OnTriggerExit2D(Collider2D other)
  {
    AmplifyColorTriggerProxy2D component = other.GetComponent<AmplifyColorTriggerProxy2D>();
    if (!((Object) component != (Object) null) || !component.OwnerEffect.UseVolumes || ((int) component.OwnerEffect.VolumeCollisionMask & 1 << this.gameObject.layer) == 0)
      return;
    component.OwnerEffect.ExitVolume((AmplifyColorVolumeBase) this);
  }
}
