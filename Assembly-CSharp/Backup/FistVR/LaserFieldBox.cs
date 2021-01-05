// Decompiled with JetBrains decompiler
// Type: FistVR.LaserFieldBox
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class LaserFieldBox : MonoBehaviour, IFVRDamageable
  {
    public LaserField Field;
    public float Life;
    public GameObject SpawnOnDestroy;
    private bool m_isDestroyed;

    public void Damage(FistVR.Damage d)
    {
      this.Life -= d.Dam_TotalKinetic;
      if ((double) this.Life > 0.0)
        return;
      this.Boom();
    }

    private void Boom()
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      Object.Instantiate<GameObject>(this.SpawnOnDestroy, this.transform.position, this.transform.rotation);
      this.Field.ShutDown();
      Object.Destroy((Object) this.gameObject);
    }
  }
}
