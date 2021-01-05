// Decompiled with JetBrains decompiler
// Type: FistVR.wwTargetShatterable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwTargetShatterable : wwTarget
  {
    private bool m_isShattered;
    public Rigidbody RB;
    public Rigidbody[] Shards;
    public GameObject[] Spawns;
    public bool DoSpawnsAlignWithStrikerDir;
    public bool DoSpawnsRotateRandom;
    public float explosionMultiplier = 1f;
    public FVRObject ObjectWrapper;
    public bool IsTNT;
    public Vector3 compareTo;
    public float DistTrigger;
    public bool UsesPoints;
    public float PointsLife = 1200f;
    public bool IgnoresCollisionDamage;

    public override void TargetStruck(Damage dam, bool sendStruckEvent)
    {
      base.TargetStruck(dam, sendStruckEvent);
      if (this.UsesPoints)
      {
        this.PointsLife -= dam.Dam_TotalKinetic;
        if ((double) this.PointsLife > 0.0)
          return;
      }
      if (this.m_isShattered)
        return;
      if (this.hasManager)
        this.Manager.PrimeForRespawn((wwTarget) this, this.m_originalPos, this.m_originalRot, this.m_originalScale, this.DoesRescale);
      this.m_isShattered = true;
      this.Shatter(dam);
    }

    public void OnCollisionEnter(Collision col)
    {
      if ((double) col.relativeVelocity.magnitude <= 3.0 || this.IgnoresCollisionDamage)
        return;
      this.TargetStruck(new Damage()
      {
        strikeDir = col.relativeVelocity.normalized,
        point = col.contacts[0].point,
        hitNormal = col.contacts[0].normal,
        Class = Damage.DamageClass.Environment,
        Dam_Blunt = col.relativeVelocity.magnitude * 100f
      }, false);
    }

    private void Shatter(Damage dam)
    {
      if (this.IsTNT && (double) Vector3.Distance(this.transform.position, this.compareTo) <= (double) this.DistTrigger && this.hasManager)
        this.Manager.Manager.ExplodeBullet();
      Vector3 vector3 = Vector3.ClampMagnitude(dam.strikeDir * dam.Dam_Blunt * 0.01f, 100f);
      for (int index = 0; index < this.Shards.Length; ++index)
      {
        if ((Object) this.RB != (Object) null)
        {
          this.Shards[index].velocity = this.RB.velocity;
          this.Shards[index].angularVelocity = this.RB.angularVelocity;
        }
        this.Shards[index].gameObject.SetActive(true);
        this.Shards[index].transform.SetParent((Transform) null);
        this.Shards[index].AddForceAtPosition(this.explosionMultiplier * vector3 * (1f / (float) this.Shards.Length), dam.point, ForceMode.Impulse);
      }
      for (int index = 0; index < this.Spawns.Length; ++index)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.Spawns[index], this.transform.position, this.transform.rotation);
        if (this.DoSpawnsAlignWithStrikerDir)
          gameObject.transform.rotation = Quaternion.LookRotation(dam.strikeDir);
        else if (this.DoSpawnsRotateRandom)
          gameObject.transform.rotation = Random.rotation;
      }
      Object.Destroy((Object) this.gameObject);
    }
  }
}
