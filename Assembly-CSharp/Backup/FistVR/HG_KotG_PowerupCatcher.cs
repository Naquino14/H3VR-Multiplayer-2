// Decompiled with JetBrains decompiler
// Type: FistVR.HG_KotG_PowerupCatcher
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class HG_KotG_PowerupCatcher : MonoBehaviour
  {
    public HG_ModeManager_KingOfTheGrill M;
    public GameObject SpawnOnPowerup;

    private void OnTriggerEnter(Collider col) => this.TestCollider(col);

    private void TestCollider(Collider col)
    {
      if ((Object) col.attachedRigidbody == (Object) null)
        return;
      RW_Powerup component = col.attachedRigidbody.GetComponent<RW_Powerup>();
      if ((Object) component == (Object) null || (Object) component.QuickbeltSlot != (Object) null || !component.Cooked)
        return;
      this.M.DepositPowerUp(col.attachedRigidbody.GetComponent<RW_Powerup>().PowerupType);
      Object.Instantiate<GameObject>(this.SpawnOnPowerup, this.transform.position, this.transform.rotation);
      Object.Destroy((Object) col.attachedRigidbody.gameObject);
    }
  }
}
