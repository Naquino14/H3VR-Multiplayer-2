// Decompiled with JetBrains decompiler
// Type: FistVR.BevelCubeTarget
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class BevelCubeTarget : MonoBehaviour, IFVRDamageable
  {
    public BevelCubeTarget.BevelCubeTargetMovementType MoveType;
    public GameObject DestroyEffect;
    private bool m_isDestroyed;
    public GameObject OnDieMessageTarget;
    public int MessageNum;
    public Cubegame Game;
    public int Points;
    public bool DoesSuicide;
    public int Life;
    private Rigidbody rb;
    public float VelMultiplier = 0.3f;
    public float RotMultiplier = 0.1f;

    public void Start()
    {
      this.Game = GameObject.Find("CubeGame").GetComponent<Cubegame>();
      this.rb = this.GetComponent<Rigidbody>();
      switch (this.MoveType)
      {
        case BevelCubeTarget.BevelCubeTargetMovementType.TowardCenter:
          Debug.Log((object) "Toward Center");
          this.rb.velocity = -this.transform.position * this.VelMultiplier;
          this.rb.angularVelocity = Random.onUnitSphere * this.RotMultiplier;
          break;
        case BevelCubeTarget.BevelCubeTargetMovementType.Vertical:
          Vector3 vector3 = -this.transform.position * this.VelMultiplier;
          vector3.x = 0.0f;
          vector3.z = 0.0f;
          this.rb.velocity = vector3;
          this.rb.angularVelocity = new Vector3(0.0f, this.RotMultiplier * Random.Range(-3f, 3f), 0.0f);
          break;
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
      this.m_isDestroyed = true;
      if (getPoints)
        this.Game.ScorePoints((float) this.Points);
      Object.Instantiate<GameObject>(this.DestroyEffect, this.transform.position, this.transform.rotation).GetComponent<Rigidbody>().velocity = this.rb.velocity;
      Object.Destroy((Object) this.gameObject);
    }

    private void OnCollisionEnter(Collision col)
    {
      if (!this.DoesSuicide || col.other.gameObject.layer != LayerMask.NameToLayer("ColOnlyTarget"))
        return;
      this.Boom(false);
    }

    public enum BevelCubeTargetMovementType
    {
      TowardCenter,
      Random,
      Vertical,
    }
  }
}
