// Decompiled with JetBrains decompiler
// Type: FistVR.TestingGun
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TestingGun : MonoBehaviour
  {
    public GameObject BulletPrefab;
    public float MuzzleVel;

    private void Update()
    {
      if (!Input.GetKeyDown(KeyCode.F))
        return;
      Object.Instantiate<GameObject>(this.BulletPrefab, this.transform.position, this.transform.rotation).GetComponent<BallisticProjectile>().Fire(this.transform.forward, (FVRFireArm) null);
    }
  }
}
