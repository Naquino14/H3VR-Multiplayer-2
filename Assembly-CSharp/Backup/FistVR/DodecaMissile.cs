// Decompiled with JetBrains decompiler
// Type: FistVR.DodecaMissile
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class DodecaMissile : MonoBehaviour, IFVRDamageable
  {
    public DodecaLauncher Launcher;
    public GameObject DestroyEffect;
    private bool m_isDestroyed;
    public Cubegame Game;
    public int Points;
    public int Life;
    private Rigidbody rb;
    public bool IsLaunched;
    public Vector3 TargetPos = new Vector3(0.0f, 1.4f, 0.0f);
    public ParticleSystem SmokeSystem;

    public void Awake() => this.Game = GameObject.Find("CubeGame").GetComponent<Cubegame>();

    public void Damage(FistVR.Damage d)
    {
      if (d.Class == FistVR.Damage.DamageClass.Projectile)
        this.Life -= (int) d.Dam_TotalKinetic;
      this.rb.AddForceAtPosition(d.strikeDir * 1f, d.point);
      if (this.Life > 0)
        return;
      this.rb.velocity = d.strikeDir * 20f;
      this.Boom(true);
    }

    public void Launch()
    {
      if (this.IsLaunched)
        return;
      if ((Object) this.SmokeSystem != (Object) null)
        this.SmokeSystem.gameObject.SetActive(true);
      this.IsLaunched = true;
      this.transform.SetParent((Transform) null);
      this.rb = this.gameObject.AddComponent<Rigidbody>();
      this.rb.mass = 5f;
      this.rb.useGravity = false;
    }

    public void MisFire()
    {
      if (this.IsLaunched)
        return;
      this.SmokeSystem.gameObject.SetActive(true);
      this.IsLaunched = true;
      this.TargetPos = Random.onUnitSphere * 50f;
      this.transform.SetParent((Transform) null);
      this.rb = this.gameObject.AddComponent<Rigidbody>();
      this.rb.mass = 5f;
      this.rb.useGravity = false;
    }

    public void FixedUpdate()
    {
      if (!this.IsLaunched || !((Object) this.rb != (Object) null))
        return;
      this.rb.MoveRotation(Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(this.TargetPos - this.transform.position, Random.onUnitSphere), Time.deltaTime * 5f));
      this.rb.velocity = this.transform.forward * 10f;
    }

    public void Boom(bool getPoints)
    {
      if (this.m_isDestroyed)
        return;
      if (this.IsLaunched && (Object) this.SmokeSystem != (Object) null)
      {
        this.SmokeSystem.gameObject.transform.SetParent((Transform) null);
        this.SmokeSystem.enableEmission = false;
      }
      this.m_isDestroyed = true;
      if (getPoints)
        this.Game.ScorePoints((float) this.Points);
      GameObject gameObject = Object.Instantiate<GameObject>(this.DestroyEffect, this.transform.position, this.transform.rotation);
      if ((Object) this.rb != (Object) null)
        gameObject.GetComponent<Rigidbody>().velocity = this.rb.velocity;
      Object.Destroy((Object) this.gameObject);
    }

    private void OnCollisionEnter(Collision col)
    {
      if (!this.IsLaunched || col.other.gameObject.layer != LayerMask.NameToLayer("ColOnlyTarget") && !(col.other.gameObject.tag != "Harmless"))
        return;
      this.Boom(false);
    }
  }
}
