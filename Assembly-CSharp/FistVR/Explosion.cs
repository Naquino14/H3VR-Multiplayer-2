// Decompiled with JetBrains decompiler
// Type: FistVR.Explosion
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class Explosion : MonoBehaviour
  {
    public int MaxChecksPerFrame = 10;
    public float ExplosionRadius;
    public float ExplosiveForce;
    public bool UsesExplosiveForceOverride;
    public Transform DirOverride;
    public LayerMask Mask_Explosion;
    public LayerMask Mask_Blockers;
    public bool UsesBlockers = true;
    private Collider[] m_colliders;
    private HashSet<Rigidbody> hitRBs = new HashSet<Rigidbody>();
    private List<Rigidbody> hitRBsList = new List<Rigidbody>();
    private int currentIndex;
    private bool m_hasExploded;
    public bool HasSecondaries;
    public GameObject SecondaryPrefab;
    public int NumSecondaries;
    public float SecondaryVelocity;
    public float LegacyUpForce = 0.2f;
    public GameObject ShrapnelPrefab;
    public int ShrapnelAmount;
    public int ShrapnelPerFrame;
    private int ShrapnelSoFar;
    public Vector2 ShrapnelVelocityRange;
    public bool DoesRadiusDamage;
    public AnimationCurve DamageCurve;
    public float PointsDamageMax;
    public AnimationCurve DamageCurve_Stun;
    public float StunDamageMax = 0.5f;
    public AnimationCurve DamageCurve_Blind;
    public float BlindDamageMax;
    public AnimationCurve DamageCurve_EMP;
    public float EMPDamageMax;
    public bool DoesIgnite;
    public AnimationCurve IgnitionChanceCurve;
    public int IFF;
    public bool CanGenerateRocketJump;
    public Vector2 MinMaxRocketJumpRange = new Vector2(1.5f, 2.5f);
    public float RocketJumpVelocity = 20f;
    public bool UsesNormalizedForce;

    private void Awake() => this.Invoke("Explode", 0.05f);

    private void OnDestroy()
    {
      this.hitRBs.Clear();
      this.hitRBsList.Clear();
    }

    private void Explode()
    {
      if (this.m_hasExploded)
        return;
      this.m_hasExploded = true;
      GM.CurrentSceneSettings.PingReceivers(this.transform.position);
      if ((double) this.ExplosionRadius > 0.00999999977648258)
      {
        this.m_colliders = Physics.OverlapSphere(this.transform.position, this.ExplosionRadius, (int) this.Mask_Explosion);
        if ((double) this.ExplosiveForce > 0.0)
        {
          for (int index = 0; index < this.m_colliders.Length; ++index)
          {
            if ((Object) this.m_colliders[index].attachedRigidbody != (Object) null && this.hitRBs.Add(this.m_colliders[index].attachedRigidbody))
              this.hitRBsList.Add(this.m_colliders[index].attachedRigidbody);
          }
        }
      }
      if (this.DoesRadiusDamage)
      {
        float num1 = 0.5f;
        switch (SM.GetSoundEnvironment(this.transform.position))
        {
          case FVRSoundEnvironment.InsideNarrow:
          case FVRSoundEnvironment.InsideLarge:
            num1 = 1f;
            break;
          case FVRSoundEnvironment.InsideSmall:
            num1 = 1.5f;
            break;
          case FVRSoundEnvironment.InsideNarrowSmall:
          case FVRSoundEnvironment.InsideMedium:
            num1 = 1.2f;
            break;
        }
        Vector3 position = this.transform.position;
        if (this.UsesExplosiveForceOverride)
          position = this.DirOverride.position;
        float num2 = 0.0f;
        float num3 = 0.0f;
        float num4 = 0.0f;
        for (int index = 0; index < this.m_colliders.Length; ++index)
        {
          if ((Object) this.m_colliders[index] != (Object) null)
          {
            IFVRDamageable component1 = this.m_colliders[index].gameObject.GetComponent<IFVRDamageable>();
            if (component1 == null && (Object) this.m_colliders[index].attachedRigidbody != (Object) null)
              component1 = this.m_colliders[index].attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
            if ((Object) component1 != (Object) null)
            {
              Vector3 vector3 = this.m_colliders[index].transform.position - position;
              Vector3 normalized = vector3.normalized;
              bool flag = false;
              RaycastHit hitInfo;
              if (this.UsesBlockers && Physics.Raycast(position, normalized, out hitInfo, vector3.magnitude, (int) this.Mask_Blockers) && !hitInfo.collider.gameObject.CompareTag("noOcclude"))
                flag = true;
              float magnitude = vector3.magnitude;
              if (this.DoesIgnite)
              {
                FVRIgnitable component2 = this.m_colliders[index].gameObject.GetComponent<FVRIgnitable>();
                if ((Object) component2 != (Object) null && (double) Random.Range(0.0f, 1f) > (double) this.IgnitionChanceCurve.Evaluate(magnitude))
                  FXM.Ignite(component2, 1f);
              }
              float num5 = this.DamageCurve.Evaluate(magnitude) * num1;
              if ((double) this.StunDamageMax > 0.0)
                num2 = this.DamageCurve_Stun.Evaluate(magnitude) * num1;
              if ((double) this.BlindDamageMax > 0.0)
                num3 = this.DamageCurve_Blind.Evaluate(magnitude) * num1;
              if ((double) this.EMPDamageMax > 0.0)
                num4 = this.DamageCurve_EMP.Evaluate(magnitude) * num1;
              if (flag)
              {
                num5 *= 0.01f;
                num2 *= 0.2f;
                num3 *= 0.01f;
                num4 *= 0.3f;
              }
              Damage dam = new Damage();
              dam.Dam_Piercing = (float) ((double) this.PointsDamageMax * (double) num5 * 0.100000001490116);
              dam.Dam_Blunt = (float) ((double) this.PointsDamageMax * (double) num5 * 0.899999976158142);
              dam.Dam_TotalKinetic = this.PointsDamageMax * num5;
              dam.Dam_Blinding = this.BlindDamageMax * num3;
              dam.Dam_Stunning = this.StunDamageMax * num2;
              dam.Dam_EMP = this.EMPDamageMax * num4;
              dam.hitNormal = -vector3;
              dam.point = this.m_colliders[index].transform.position;
              dam.strikeDir = normalized;
              dam.Class = Damage.DamageClass.Explosive;
              dam.damageSize = 1f;
              dam.Source_IFF = this.IFF;
              if (!this.CanGenerateRocketJump)
                component1.Damage(dam);
              else if ((Object) this.m_colliders[index].gameObject.GetComponent<FVRPlayerHitbox>() != (Object) null)
              {
                if (GM.MFFlags.PlayerSetting_BlastJumpingSelfDamage == MF_BlastJumpingSelfDamage.Arcade)
                {
                  dam.Dam_Piercing *= 0.04f;
                  dam.Dam_Blunt *= 0.04f;
                  dam.Dam_TotalKinetic *= 0.04f;
                  dam.Dam_Blinding *= 0.04f;
                  dam.Dam_Stunning *= 0.04f;
                }
                else if (GM.MFFlags.PlayerSetting_BlastJumpingSelfDamage == MF_BlastJumpingSelfDamage.Realistic)
                  component1.Damage(dam);
              }
              else
                component1.Damage(dam);
            }
          }
        }
      }
      if (this.HasSecondaries)
      {
        for (int index = 0; index < this.NumSecondaries; ++index)
        {
          Vector3 onUnitSphere = Random.onUnitSphere;
          Object.Instantiate<GameObject>(this.SecondaryPrefab, this.transform.position + onUnitSphere * 0.025f, Quaternion.identity).GetComponent<Rigidbody>().velocity = onUnitSphere * this.SecondaryVelocity * Random.Range(0.5f, 1f);
        }
      }
      if (!this.CanGenerateRocketJump || GM.MFFlags.PlayerSetting_BlastJumping != MF_BlastJumping.On)
        return;
      GM.CurrentMovementManager.RocketJump(this.transform.position, this.MinMaxRocketJumpRange, this.RocketJumpVelocity);
    }

    private void Update()
    {
      if (this.m_hasExploded && (Object) this.ShrapnelPrefab != (Object) null && this.ShrapnelSoFar < this.ShrapnelAmount)
      {
        for (int index = 0; index < this.ShrapnelPerFrame; ++index)
        {
          if (this.ShrapnelSoFar < this.ShrapnelAmount)
          {
            GameObject gameObject = Object.Instantiate<GameObject>(this.ShrapnelPrefab, this.transform.position, Random.rotation);
            BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
            if ((Object) component != (Object) null)
            {
              component.Fire(Random.Range(this.ShrapnelVelocityRange.x, this.ShrapnelVelocityRange.y), gameObject.transform.forward, (FVRFireArm) null);
              component.Source_IFF = this.IFF;
              ++this.ShrapnelSoFar;
            }
            ++this.ShrapnelSoFar;
          }
        }
      }
      int currentIndex = this.currentIndex;
      Vector3 position = this.transform.position;
      float explosionRadius = this.ExplosionRadius;
      if (this.UsesExplosiveForceOverride)
      {
        position = this.DirOverride.position;
        explosionRadius *= 3f;
      }
      if (this.hitRBsList.Count <= 0 || this.currentIndex >= this.hitRBsList.Count)
        return;
      for (int index = currentIndex; index < Mathf.Min(currentIndex + this.MaxChecksPerFrame, this.hitRBsList.Count); ++index)
      {
        if ((Object) this.hitRBsList[index] != (Object) null)
        {
          Vector3 vector3 = this.hitRBsList[index].position - position;
          Vector3 normalized = vector3.normalized;
          if (!Physics.Raycast(position, normalized, vector3.magnitude, (int) this.Mask_Blockers) && !this.hitRBsList[index].isKinematic)
          {
            if (this.UsesNormalizedForce)
              this.hitRBsList[index].AddExplosionForce(this.ExplosiveForce, position, explosionRadius, this.LegacyUpForce, ForceMode.VelocityChange);
            else
              this.hitRBsList[index].AddExplosionForce(this.ExplosiveForce, position, explosionRadius, this.LegacyUpForce, ForceMode.Impulse);
          }
        }
        ++this.currentIndex;
      }
    }
  }
}
