// Decompiled with JetBrains decompiler
// Type: FistVR.IcoSphereTarget
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class IcoSphereTarget : MonoBehaviour, IFVRDamageable
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
    private ParticleSystem DamageParts;
    public ArmorPlate[] ArmorPieces;

    public void Start()
    {
      this.DamageParts = this.GetComponent<ParticleSystem>();
      this.Game = GameObject.Find("CubeGame").GetComponent<Cubegame>();
      this.rb = this.GetComponent<Rigidbody>();
      switch (this.MoveType)
      {
        case BevelCubeTarget.BevelCubeTargetMovementType.TowardCenter:
          this.rb.velocity = -this.transform.position * this.VelMultiplier;
          this.rb.angularVelocity = Random.onUnitSphere * this.RotMultiplier;
          break;
        case BevelCubeTarget.BevelCubeTargetMovementType.Random:
          this.rb.velocity = Random.onUnitSphere * this.VelMultiplier;
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

    public void DamageParticle(Vector3 point, int num)
    {
      for (int index = 0; index < num; ++index)
        this.DamageParts.Emit(point, Random.onUnitSphere * 2f, 1.5f, 0.2f, (Color32) Color.white);
    }

    public void Damage(FistVR.Damage d)
    {
      if (d.Class == FistVR.Damage.DamageClass.Projectile)
        this.Life -= (int) d.Dam_TotalKinetic;
      this.rb.AddForceAtPosition(d.strikeDir * 1f, d.point);
      if (this.Life <= 0)
      {
        this.rb.velocity = d.strikeDir * 20f;
        this.Boom(true);
      }
      else
      {
        for (int index = 0; index < 3; ++index)
          this.DamageParts.Emit(d.point, -d.strikeDir * 5f, 1f, 0.35f, (Color32) Color.white);
      }
    }

    public void Boom(bool getPoints)
    {
      if (this.m_isDestroyed)
        return;
      for (int index = 0; index < this.ArmorPieces.Length; ++index)
      {
        if ((Object) this.ArmorPieces[index] != (Object) null)
          this.ArmorPieces[index].Detach(this.ArmorPieces[index].transform.position - this.transform.position);
      }
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
  }
}
