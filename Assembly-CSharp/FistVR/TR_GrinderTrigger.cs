// Decompiled with JetBrains decompiler
// Type: FistVR.TR_GrinderTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TR_GrinderTrigger : MonoBehaviour
  {
    private void OnTriggerStay(Collider col)
    {
      FVRPlayerHitbox component = col.GetComponent<FVRPlayerHitbox>();
      if (!((Object) component != (Object) null))
        return;
      component.Damage(new DamageDealt()
      {
        force = Vector3.zero,
        PointsDamage = 2000f,
        hitNormal = Vector3.zero,
        IsInside = false,
        MPa = 1f,
        MPaRootMeter = 1f,
        point = this.transform.position,
        ShotOrigin = (Transform) null,
        strikeDir = Vector3.zero,
        uvCoords = Vector2.zero,
        IsInitialContact = true
      });
    }
  }
}
