// Decompiled with JetBrains decompiler
// Type: FistVR.OmniMortar
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OmniMortar : MonoBehaviour, IFVRDamageable
  {
    private OmniSpawner_Mortar m_spawner;
    public Rigidbody RB;
    public Rigidbody[] Shards;
    private bool m_isDestroyed;

    private void Awake() => this.RB.maxAngularVelocity = 25f;

    public void Configure(OmniSpawner_Mortar spawner, Vector3 pos, Vector3 forward, float vel)
    {
      this.m_spawner = spawner;
      this.transform.position = pos;
      this.RB.angularVelocity = Random.Range(1f, 5f) * Random.onUnitSphere;
      this.RB.velocity = forward * vel;
    }

    private void FixedUpdate()
    {
      this.RB.velocity += new Vector3(0.0f, -9.81f, 0.0f) * Time.deltaTime;
      if ((double) this.transform.position.y > -100.0 || this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      this.m_spawner.MortarExpired(this);
    }

    public void Damage(FistVR.Damage dam)
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      this.m_spawner.AddPoints(100);
      for (int index = 0; index < this.Shards.Length; ++index)
      {
        this.Shards[index].gameObject.SetActive(true);
        this.Shards[index].transform.SetParent((Transform) null);
        this.Shards[index].maxAngularVelocity = 25f;
        this.Shards[index].velocity = this.RB.velocity;
        this.Shards[index].angularVelocity = this.RB.angularVelocity;
        this.Shards[index].AddExplosionForce(Random.Range(5f, 20f), dam.point, 1f, 0.1f, ForceMode.Impulse);
      }
      this.m_spawner.MortarHit(this);
    }
  }
}
