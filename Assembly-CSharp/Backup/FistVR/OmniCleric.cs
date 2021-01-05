// Decompiled with JetBrains decompiler
// Type: FistVR.OmniCleric
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OmniCleric : MonoBehaviour, IFVRDamageable
  {
    private float m_life = 600f;
    private bool m_isDestroyed;
    private float m_headRadius = 0.14f;
    private OmniSpawner_Cleric m_spawner;
    public OmniSpawnDef_Cleric.TargetLocation m_pos;
    private Vector3 m_startPos;
    private Vector3 m_endPos;
    private bool m_isMovingIntoPosition;
    private float m_moveLerp;
    private float m_moveSpeed = 10f;
    public Rigidbody[] Shards_Top;
    public Rigidbody[] Shards_Middle;
    public GameObject[] PFXPrefabs;
    public Transform TR_HeadSpot;
    private bool m_doesFire;
    public Transform ShotRoot;
    public Transform ShotBeam;
    public Renderer BeamRenderer;
    public Color StartColor;
    public Color EndColor;
    private float m_beamTick;
    private float m_beamMax;
    private Vector3 m_beamStart = new Vector3(0.025f, 0.025f, 6f);
    private Vector3 m_beamEnd = new Vector3(0.005f, 0.005f, 6f);
    private bool m_hasFired;
    private bool m_isTickingDown;
    public ParticleSystem MuzzleFire;
    public LayerMask ShotMask;
    private RaycastHit m_shotHit;
    public AudioSource GunShotSound;
    public AudioClip[] GunShotClips;

    public void Init(
      OmniSpawner_Cleric spawner,
      Transform spawnPoint,
      bool doesFire,
      float FiringTime,
      OmniSpawnDef_Cleric.TargetLocation PositionIndex)
    {
      this.m_spawner = spawner;
      this.m_startPos = spawnPoint.position;
      this.m_endPos = this.m_startPos;
      this.m_endPos.y += 2f;
      this.m_isMovingIntoPosition = true;
      this.m_moveLerp = 0.0f;
      this.m_doesFire = doesFire;
      this.m_pos = PositionIndex;
      this.m_isTickingDown = true;
      this.m_beamTick = Random.Range(FiringTime, FiringTime * 1.1f);
      this.m_beamMax = this.m_beamTick;
    }

    private void Update()
    {
      if (this.m_isMovingIntoPosition)
      {
        if ((double) this.m_moveLerp < 1.0)
          this.m_moveLerp += Time.deltaTime * this.m_moveSpeed;
        else
          this.m_moveLerp = 1f;
        this.transform.position = Vector3.Lerp(this.m_startPos, this.m_endPos, this.m_moveLerp);
        if ((double) this.m_moveLerp < 1.0)
          return;
        this.m_isMovingIntoPosition = false;
        this.ShotBeam.LookAt(GM.CurrentPlayerBody.Torso, Vector3.up);
        this.ShotBeam.Rotate(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0.0f));
      }
      else
      {
        if (!this.m_doesFire || !this.m_isTickingDown)
          return;
        if ((double) this.m_beamTick > 0.0)
        {
          if (!this.ShotBeam.gameObject.activeSelf)
            this.ShotBeam.gameObject.SetActive(true);
          float t = (float) (1.0 - (double) this.m_beamTick / (double) this.m_beamMax);
          this.ShotBeam.localScale = Vector3.Lerp(this.m_beamStart, this.m_beamEnd, t);
          this.BeamRenderer.material.SetColor("_Color", Color.Lerp(this.StartColor, this.EndColor, t));
          this.m_beamTick -= Time.deltaTime;
          this.m_hasFired = false;
        }
        else
        {
          if (this.m_hasFired)
            return;
          if (this.ShotBeam.gameObject.activeSelf)
            this.ShotBeam.gameObject.SetActive(false);
          this.m_hasFired = true;
          this.GunShotSound.PlayOneShot(this.GunShotClips[Random.Range(0, this.GunShotClips.Length)], 1.2f);
          this.MuzzleFire.Emit(3);
          FXM.InitiateMuzzleFlash(this.ShotBeam.position, this.ShotBeam.forward, 3f, Color.white, 3f);
          if (Physics.Raycast(this.ShotBeam.position, this.ShotBeam.forward, out this.m_shotHit, 6f, (int) this.ShotMask, QueryTriggerInteraction.Collide) && (Object) this.m_shotHit.collider.gameObject.GetComponent<FVRPlayerHitbox>() != (Object) null)
          {
            GM.CurrentPlayerBody.HitEffect();
            this.m_spawner.AddPoints(-500);
          }
          this.m_beamTick = this.m_beamMax;
          this.ShotBeam.LookAt(GM.CurrentPlayerBody.Torso, Vector3.up);
          this.ShotBeam.Rotate(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0.0f));
        }
      }
    }

    public void Damage(FistVR.Damage dam)
    {
      if (this.m_isMovingIntoPosition || dam.Class != FistVR.Damage.DamageClass.Projectile)
        return;
      bool isHeadShot = false;
      if ((double) Vector3.Distance(dam.point, this.TR_HeadSpot.transform.position) <= (double) this.m_headRadius)
      {
        isHeadShot = true;
        this.m_life = 0.0f;
      }
      else
        this.m_life -= dam.Dam_TotalKinetic;
      if ((double) this.m_life > 0.0)
        return;
      this.Destroy(isHeadShot, dam);
    }

    private void Destroy(bool isHeadShot, FistVR.Damage dam)
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      for (int index = 0; index < this.PFXPrefabs.Length; ++index)
        Object.Instantiate<GameObject>(this.PFXPrefabs[index], dam.point, Quaternion.identity);
      if (isHeadShot)
      {
        for (int index = 0; index < this.Shards_Top.Length; ++index)
        {
          this.Shards_Top[index].transform.SetParent((Transform) null);
          this.Shards_Top[index].gameObject.SetActive(true);
          this.Shards_Top[index].AddForceAtPosition(dam.strikeDir * dam.Dam_TotalKinetic * 0.01f, dam.point, ForceMode.Impulse);
          this.Shards_Top[index].AddExplosionForce(2f, dam.point, 2f, 0.1f, ForceMode.Impulse);
        }
      }
      else
      {
        for (int index = 0; index < this.Shards_Middle.Length; ++index)
        {
          this.Shards_Middle[index].transform.SetParent((Transform) null);
          this.Shards_Middle[index].gameObject.SetActive(true);
          this.Shards_Middle[index].AddForceAtPosition(dam.strikeDir * dam.Dam_TotalKinetic * 0.01f, dam.point, ForceMode.Impulse);
          this.Shards_Middle[index].AddExplosionForce(2f, dam.point, 1f, 0.1f, ForceMode.Impulse);
        }
      }
      this.m_spawner.ClearCleric(this, isHeadShot);
    }
  }
}
