// Decompiled with JetBrains decompiler
// Type: FistVR.OmniDisc
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OmniDisc : MonoBehaviour, IFVRDamageable
  {
    private bool m_isDestroyed;
    private OmniSpawner_Discs m_spawner;
    public OmniSpawnDef_Discs.DiscType Type;
    private OmniSpawnDef_Discs.DiscMovementPattern m_movePattern;
    private OmniSpawnDef_Discs.DiscMovementStyle m_moveStyle;
    private Vector3 m_startPos;
    private Vector3 m_endPos;
    private Quaternion m_startRot;
    private Quaternion m_endRot;
    private bool m_isMovingIntoPosition;
    private float m_moveLerp;
    private float m_moveSpeed = 1f;
    private int pointsForNormal = 100;
    private int pointsForNoShoot = -50;
    private int pointsForArmored = 1000;
    private int pointsForBullCenter = 200;
    private int pointsForBullInnerRing = 50;
    private int pointsForBullOuterRing = 10;
    private float m_startingArmorLife = 4000f;
    private float ArmoredPointsLife = 4000f;
    private Vector2 m_spawnBounds;
    private float m_tick;
    private Vector3 m_lerpPointA;
    private Vector3 m_lerpPointB;
    public Rigidbody[] Shards;
    private Renderer m_rend;
    public GameObject[] PFXPrefabs;

    public void Init(
      OmniSpawner_Discs spawner,
      Vector3 startPos,
      Vector3 endPos,
      Quaternion startRot,
      Quaternion endRot,
      OmniSpawnDef_Discs.DiscMovementPattern pattern,
      OmniSpawnDef_Discs.DiscMovementStyle moveStyle,
      Vector2 spawnbounds,
      float moveSpeed)
    {
      this.m_spawner = spawner;
      this.m_startPos = startPos;
      this.m_endPos = endPos;
      this.m_startRot = startRot;
      this.m_endRot = endRot;
      this.m_isMovingIntoPosition = true;
      this.m_movePattern = pattern;
      this.m_spawnBounds = spawnbounds;
      this.m_moveStyle = moveStyle;
      this.m_rend = this.GetComponent<Renderer>();
      this.m_moveSpeed = moveSpeed;
      this.InitTick();
    }

    private void InitTick()
    {
      this.m_lerpPointA = this.m_endPos;
      this.m_lerpPointB = this.m_endPos;
      switch (this.m_movePattern)
      {
        case OmniSpawnDef_Discs.DiscMovementPattern.OscillateX:
          this.m_lerpPointB.x = -this.m_lerpPointB.x;
          break;
        case OmniSpawnDef_Discs.DiscMovementPattern.OscillateY:
          this.m_lerpPointB.y = (float) (-((double) this.m_lerpPointB.y - 1.25) + 1.25);
          break;
        case OmniSpawnDef_Discs.DiscMovementPattern.OscillateZ:
          this.m_lerpPointB.z *= 2f;
          break;
        case OmniSpawnDef_Discs.DiscMovementPattern.OscillateXY:
          this.m_lerpPointB.x = -this.m_lerpPointB.x;
          this.m_lerpPointB.y = (float) (-((double) this.m_lerpPointB.y - 1.25) + 1.25);
          break;
        case OmniSpawnDef_Discs.DiscMovementPattern.ClockwiseRot:
          this.m_lerpPointB.y -= 1.25f;
          break;
        case OmniSpawnDef_Discs.DiscMovementPattern.CounterClockwiseRot:
          this.m_lerpPointB.y -= 1.25f;
          break;
      }
    }

    private void Update()
    {
      if (this.m_isMovingIntoPosition)
      {
        if ((double) this.m_moveLerp < 1.0)
          this.m_moveLerp += Time.deltaTime * 5f;
        else
          this.m_moveLerp = 1f;
        this.transform.position = Vector3.Lerp(this.m_startPos, this.m_endPos, this.m_moveLerp);
        this.transform.rotation = Quaternion.Slerp(this.m_startRot, this.m_endRot, this.m_moveLerp);
        if ((double) this.m_moveLerp < 1.0)
          return;
        this.m_isMovingIntoPosition = false;
      }
      else
      {
        Vector3 vector3_1 = Vector3.zero;
        float num = 1f;
        switch (this.m_moveStyle)
        {
          case OmniSpawnDef_Discs.DiscMovementStyle.Linear:
            this.m_tick += Time.deltaTime * this.m_moveSpeed;
            if ((double) this.m_tick > 2.0)
              this.m_tick -= 2f;
            vector3_1 = Vector3.Lerp(this.m_lerpPointA, this.m_lerpPointB, (double) this.m_tick >= 1.0 ? 2f - this.m_tick : this.m_tick);
            break;
          case OmniSpawnDef_Discs.DiscMovementStyle.Sinusoidal:
            this.m_tick += Time.deltaTime * this.m_moveSpeed;
            vector3_1 = Vector3.Lerp(this.m_lerpPointA, this.m_lerpPointB, (float) ((double) Mathf.Sin((float) ((double) this.m_tick * 3.14159274101257 * 0.5 - 1.57079637050629)) * 0.5 + 0.5));
            break;
          case OmniSpawnDef_Discs.DiscMovementStyle.Rotational:
            this.m_tick += Time.deltaTime * 0.25f * this.m_moveSpeed;
            if ((double) this.m_tick > 1.0)
              --this.m_tick;
            num = 1f;
            vector3_1 = Quaternion.Euler(0.0f, 0.0f, this.m_tick * 360f * (this.m_movePattern != OmniSpawnDef_Discs.DiscMovementPattern.ClockwiseRot ? -1f : 1f)) * this.m_lerpPointB + Vector3.up * 1.25f;
            break;
          case OmniSpawnDef_Discs.DiscMovementStyle.RotationalSwell:
            Vector3 vector3_2 = new Vector3(this.m_lerpPointB.x, this.m_lerpPointB.y, 0.0f);
            float z = this.m_lerpPointB.z;
            float magnitude = vector3_2.magnitude;
            Vector3 normalized = vector3_2.normalized;
            this.m_tick += Time.deltaTime * 0.25f * this.m_moveSpeed;
            if ((double) this.m_tick > 1.0)
              --this.m_tick;
            num = 1f;
            Vector3 vector3_3 = Quaternion.Euler(0.0f, 0.0f, this.m_tick * 360f * (this.m_movePattern != OmniSpawnDef_Discs.DiscMovementPattern.ClockwiseRot ? -1f : 1f)) * normalized * (float) (((double) Mathf.Sin((float) ((double) this.m_tick * 3.14159274101257 * 2.0)) + 1.0) * 0.25 + 0.5) * magnitude;
            vector3_3.z = z;
            vector3_1 = vector3_3 + Vector3.up * 1.25f;
            break;
        }
        this.transform.position = vector3_1;
      }
    }

    private void DeployShards(Vector3 point)
    {
      for (int index = 0; index < this.Shards.Length; ++index)
      {
        this.Shards[index].gameObject.SetActive(true);
        this.Shards[index].transform.SetParent((Transform) null);
        this.Shards[index].AddExplosionForce(10f, point, 2f, 0.1f, ForceMode.Impulse);
      }
    }

    private void DeployPFX(int i) => Object.Instantiate<GameObject>(this.PFXPrefabs[i], this.transform.position, this.transform.rotation);

    public void Damage(FistVR.Damage dam)
    {
      if (this.m_isMovingIntoPosition)
        return;
      switch (this.Type)
      {
        case OmniSpawnDef_Discs.DiscType.Normal:
          if (this.m_isDestroyed)
            break;
          this.m_isDestroyed = true;
          this.m_spawner.AddPoints(this.pointsForNormal);
          this.m_spawner.ClearDisc(this);
          this.m_spawner.Invoke("PlaySuccessSound", 0.15f);
          this.DeployShards(dam.point);
          Object.Destroy((Object) this.gameObject);
          break;
        case OmniSpawnDef_Discs.DiscType.NoShoot:
          if (this.m_isDestroyed)
            break;
          this.m_isDestroyed = true;
          this.m_spawner.AddPoints(this.pointsForNoShoot);
          this.m_spawner.ClearDisc(this);
          this.m_spawner.Invoke("PlayFailureSound", 0.15f);
          this.DeployShards(dam.point);
          Object.Destroy((Object) this.gameObject);
          break;
        case OmniSpawnDef_Discs.DiscType.Armored:
          if (dam.Class != FistVR.Damage.DamageClass.Projectile)
            break;
          this.ArmoredPointsLife -= dam.Dam_TotalKinetic;
          this.m_rend.material.SetColor("_Color", Color.Lerp(new Color(0.2f, 0.4f, 1f, 1f), Color.white, this.ArmoredPointsLife / this.m_startingArmorLife));
          if ((double) this.ArmoredPointsLife > 0.0 || this.m_isDestroyed)
            break;
          this.m_isDestroyed = true;
          this.m_spawner.AddPoints(this.pointsForArmored);
          this.m_spawner.ClearDisc(this);
          this.m_spawner.Invoke("PlaySuccessSound", 0.15f);
          this.DeployShards(dam.point);
          Object.Destroy((Object) this.gameObject);
          break;
        case OmniSpawnDef_Discs.DiscType.RedRing:
          if (this.m_isDestroyed)
            break;
          this.m_isDestroyed = true;
          if ((double) Vector3.Distance(dam.point, this.transform.position) > 0.25)
          {
            this.m_spawner.AddPoints(this.pointsForNoShoot);
            this.m_spawner.Invoke("PlayFailureSound", 0.15f);
            this.DeployPFX(0);
          }
          else
          {
            this.m_spawner.AddPoints(this.pointsForNormal);
            this.m_spawner.Invoke("PlaySuccessSound", 0.15f);
            this.DeployPFX(1);
          }
          this.m_spawner.ClearDisc(this);
          Object.Destroy((Object) this.gameObject);
          break;
        case OmniSpawnDef_Discs.DiscType.Bullseye:
          if (this.m_isDestroyed)
            break;
          this.m_isDestroyed = true;
          if ((double) Vector3.Distance(dam.point, this.transform.position) > 0.300000011920929)
          {
            this.m_spawner.AddPoints(this.pointsForBullOuterRing);
            this.DeployPFX(2);
          }
          else if ((double) Vector3.Distance(dam.point, this.transform.position) > 0.100000001490116)
          {
            this.m_spawner.AddPoints(this.pointsForBullInnerRing);
            this.DeployPFX(1);
          }
          else
          {
            this.m_spawner.AddPoints(this.pointsForBullCenter);
            this.DeployPFX(0);
          }
          this.m_spawner.ClearDisc(this);
          this.m_spawner.Invoke("PlaySuccessSound", 0.15f);
          this.DeployShards(dam.point);
          Object.Destroy((Object) this.gameObject);
          break;
      }
    }
  }
}
