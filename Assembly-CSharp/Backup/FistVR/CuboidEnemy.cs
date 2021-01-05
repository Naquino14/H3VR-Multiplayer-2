// Decompiled with JetBrains decompiler
// Type: FistVR.CuboidEnemy
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class CuboidEnemy : MonoBehaviour, IFVRDamageable
  {
    [Header("Base Stats")]
    public int Life = 1;
    public CuboidEnemy.CuboidMoveStyle MoveStyle;
    public float LinearMoveSpeed = 0.1f;
    public float RotMoveSpeed = 0.1f;
    public int DamageOnCollision = 1;
    public int Points;
    public GameObject WarpInEffect;
    public GameObject WarpOutEffect;
    public GameObject SpawnOnHit;
    public GameObject[] SpawnOnDestruction;
    private Rigidbody rb;
    private bool m_isDestroyed;

    public virtual void Awake()
    {
      this.rb = this.GetComponent<Rigidbody>();
      this.rb.maxAngularVelocity = 20f;
      switch (this.MoveStyle)
      {
        case CuboidEnemy.CuboidMoveStyle.SimpleRotate:
          this.rb.angularVelocity = Random.onUnitSphere * this.RotMoveSpeed;
          break;
        case CuboidEnemy.CuboidMoveStyle.TowardCenter:
          this.rb.velocity = -this.transform.position * this.LinearMoveSpeed;
          this.rb.angularVelocity = Random.onUnitSphere * this.RotMoveSpeed;
          break;
        case CuboidEnemy.CuboidMoveStyle.Random:
          this.rb.velocity = Random.onUnitSphere * this.LinearMoveSpeed;
          this.rb.angularVelocity = Random.onUnitSphere * this.RotMoveSpeed;
          break;
        case CuboidEnemy.CuboidMoveStyle.Vertical:
          this.rb.velocity = new Vector3(0.0f, Mathf.Sign(-this.transform.position.y) * this.LinearMoveSpeed, 0.0f);
          this.rb.angularVelocity = new Vector3(0.0f, this.RotMoveSpeed * Random.Range(-10f, 10f), 0.0f);
          break;
        case CuboidEnemy.CuboidMoveStyle.JukeAroundPos:
          this.rb.velocity = Random.onUnitSphere * this.LinearMoveSpeed;
          break;
      }
    }

    public void Start()
    {
      if (!((Object) this.WarpInEffect != (Object) null))
        return;
      Object.Instantiate<GameObject>(this.WarpInEffect, this.transform.position, this.transform.rotation);
    }

    public void Damage(int i)
    {
      this.Life -= i;
      if (this.Life > 0)
        return;
      this.Destroy(Vector3.zero, Vector3.zero, false);
    }

    public void Damage(int i, Vector3 force, Vector3 point)
    {
      this.Life -= i;
      if (this.Life > 0)
        return;
      this.Destroy(force, point, false);
    }

    public void Damage(FistVR.Damage d)
    {
      if (d.Class == FistVR.Damage.DamageClass.Projectile)
        this.Life -= (int) d.Dam_TotalKinetic;
      if (this.Life > 0)
        return;
      this.Destroy(d.strikeDir, d.point, true);
    }

    protected void Destroy(Vector3 force, Vector3 point, bool GetPoints)
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      if (!GetPoints)
        ;
      Object.Destroy((Object) this.gameObject);
    }

    public void WarpOut()
    {
      if ((Object) this.WarpOutEffect != (Object) null)
        Object.Instantiate<GameObject>(this.WarpInEffect, this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    private void OnCollisionEnter(Collision col)
    {
      if (col.other.gameObject.layer != LayerMask.NameToLayer("ColOnlyTarget") || this.DamageOnCollision <= 0)
        return;
      this.Damage(this.DamageOnCollision);
    }

    public enum CuboidMoveStyle
    {
      SimpleRotate,
      TowardCenter,
      Random,
      Vertical,
      JukeAroundPos,
    }
  }
}
