// Decompiled with JetBrains decompiler
// Type: FistVR.DodecaLauncher
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class DodecaLauncher : MonoBehaviour, IFVRDamageable
  {
    public DodecaMissile[] Missiles;
    public GameObject DestroyEffect;
    private bool m_isDestroyed;
    public GameObject OnDieMessageTarget;
    public int MessageNum;
    public Cubegame Game;
    public int Points;
    public int Life;
    private Rigidbody rb;
    private bool m_isLaunching = true;
    private float m_launchTick = 2f;

    public void Awake()
    {
      this.Game = GameObject.Find("CubeGame").GetComponent<Cubegame>();
      this.rb = this.GetComponent<Rigidbody>();
      this.rb.angularVelocity = Random.onUnitSphere * 15f;
    }

    public void Update()
    {
      if (!this.m_isLaunching)
        return;
      if ((double) this.m_launchTick > 0.0)
      {
        this.m_launchTick -= Time.deltaTime;
      }
      else
      {
        this.m_launchTick = 3f;
        for (int index1 = 0; index1 < 1; ++index1)
        {
          for (int index2 = 0; index2 < this.Missiles.Length; ++index2)
          {
            bool flag = false;
            if (!this.Missiles[index2].IsLaunched)
            {
              this.Missiles[index2].Launch();
              flag = true;
            }
            if (index2 == this.Missiles.Length - 1)
              this.m_isLaunching = false;
            if (flag)
              break;
          }
        }
      }
    }

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

    public void Boom(bool getPoints)
    {
      if (this.m_isDestroyed)
        return;
      this.Game.TargetDown();
      for (int index = 0; index < this.Missiles.Length; ++index)
      {
        if ((Object) this.Missiles[index] != (Object) null && !this.Missiles[index].IsLaunched)
          this.Missiles[index].MisFire();
      }
      this.m_isDestroyed = true;
      if (getPoints)
        this.Game.ScorePoints((float) this.Points);
      Object.Instantiate<GameObject>(this.DestroyEffect, this.transform.position, this.transform.rotation).GetComponent<Rigidbody>().velocity = this.rb.velocity;
      Object.Destroy((Object) this.gameObject);
    }
  }
}
