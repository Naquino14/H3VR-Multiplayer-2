// Decompiled with JetBrains decompiler
// Type: FistVR.TR_GrinderDamageTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TR_GrinderDamageTrigger : MonoBehaviour
  {
    public TR_GrinderContoller GController;
    public TR_Grinder Grinder;

    private void OnTriggerEnter(Collider col)
    {
      if (!((Object) col.attachedRigidbody != (Object) null))
        return;
      ShatterableMeat component = col.gameObject.GetComponent<ShatterableMeat>();
      if ((Object) component != (Object) null)
      {
        if (!component.Explode())
          return;
        this.Grinder.EmitEvent(col.transform.position);
        this.GController.DamageGrinder();
      }
      else
        col.attachedRigidbody.AddForce(-this.transform.right * 5f, ForceMode.Impulse);
    }
  }
}
