// Decompiled with JetBrains decompiler
// Type: FistVR.MM_EAPACrate
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MM_EAPACrate : MonoBehaviour, IFVRDamageable
  {
    public Transform[] SpawnPoints;
    public Rigidbody[] Shards;
    private bool m_isDestroyed;
    public GameObject ShatterFX_Prefab;
    public Transform ShatterFX_Point;
    private GameObject go1;
    private GameObject go2;
    private GameObject go3;
    private GameObject go4;
    private GameObject go5;

    public void Damage(FistVR.Damage d)
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      this.Destroy();
    }

    public void SetGOs(GameObject g1, GameObject g2, GameObject g3, GameObject g4, GameObject g5)
    {
      this.go1 = g1;
      this.go2 = g2;
      this.go3 = g3;
      this.go4 = g4;
      this.go5 = g5;
    }

    private void Destroy()
    {
      Object.Instantiate<GameObject>(this.ShatterFX_Prefab, this.ShatterFX_Point.position, this.ShatterFX_Point.rotation);
      for (int index = 0; index < this.Shards.Length; ++index)
      {
        this.Shards[index].transform.SetParent((Transform) null);
        this.Shards[index].gameObject.SetActive(true);
      }
      if ((Object) this.go1 != (Object) null)
        Object.Instantiate<GameObject>(this.go1, this.SpawnPoints[0].position, this.SpawnPoints[0].rotation);
      if ((Object) this.go2 != (Object) null)
        Object.Instantiate<GameObject>(this.go2, this.SpawnPoints[0].position, this.SpawnPoints[0].rotation);
      if ((Object) this.go3 != (Object) null)
        Object.Instantiate<GameObject>(this.go3, this.SpawnPoints[0].position, this.SpawnPoints[0].rotation);
      if ((Object) this.go4 != (Object) null)
        Object.Instantiate<GameObject>(this.go4, this.SpawnPoints[0].position, this.SpawnPoints[0].rotation);
      if ((Object) this.go5 != (Object) null)
        Object.Instantiate<GameObject>(this.go5, this.SpawnPoints[0].position, this.SpawnPoints[0].rotation);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
