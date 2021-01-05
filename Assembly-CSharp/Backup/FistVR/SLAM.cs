// Decompiled with JetBrains decompiler
// Type: FistVR.SLAM
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class SLAM : FVRPhysicalObject, IFVRDamageable
  {
    [Header("ModalStuff")]
    public SLAM.SLAMMode Mode;
    [Header("Components")]
    public Transform FlipPiece;
    public Vector2 FlipPieceRots;
    public GameObject Light_Red;
    public GameObject Light_Green;
    [Header("LaserShit")]
    public GameObject Laser_Root;
    public Transform Laser_Beam;
    public LayerMask LM_Beam;
    private float m_storedLaserLength = -1f;
    private RaycastHit m_hit;
    private bool m_isPriming;
    private float m_primeLerp;
    private bool m_isPrimed;
    [Header("SpawnOnDestroy")]
    public List<GameObject> SpawnOnDestroy;
    public Transform SpawnPoint;
    private bool m_hasDetonated;
    public List<GameObject> SpawnOnDestroy_Broad;
    public Transform SpawnPoint_Broad;
    [Header("Audio")]
    public AudioEvent AudEvent_Flip;
    public AudioEvent AudEvent_ArmButtonClick;
    public AudioEvent AudEvent_Priming;
    public AudioEvent AudEvent_Armed;
    [Header("AttachSensors")]
    public LayerMask LM_Attach;

    public override bool IsInteractable() => this.Mode != SLAM.SLAMMode.LaserArmed && base.IsInteractable();

    public override bool IsDistantGrabbable() => this.Mode != SLAM.SLAMMode.LaserArmed && base.IsDistantGrabbable();

    protected override void Start()
    {
      base.Start();
      FXM.RegisterSLAM(this);
    }

    public void SetMode(SLAM.SLAMMode m)
    {
      this.Mode = m;
      switch (this.Mode)
      {
        case SLAM.SLAMMode.Disabled:
          this.FlipPiece.localEulerAngles = new Vector3(this.FlipPieceRots.x, 0.0f, 0.0f);
          this.Light_Red.SetActive(false);
          this.Light_Green.SetActive(true);
          this.Laser_Root.SetActive(false);
          this.Size = FVRPhysicalObject.FVRPhysicalObjectSize.Medium;
          break;
        case SLAM.SLAMMode.LaserPlace:
          this.FlipPiece.localEulerAngles = new Vector3(this.FlipPieceRots.y, 0.0f, 0.0f);
          this.Light_Red.SetActive(false);
          this.Light_Green.SetActive(true);
          this.Size = FVRPhysicalObject.FVRPhysicalObjectSize.CantCarryBig;
          break;
        case SLAM.SLAMMode.LaserArmed:
          this.FlipPiece.localEulerAngles = new Vector3(this.FlipPieceRots.y, 0.0f, 0.0f);
          this.Light_Red.SetActive(true);
          this.Light_Green.SetActive(false);
          this.Laser_Root.SetActive(true);
          this.PrimeLaser();
          this.Size = FVRPhysicalObject.FVRPhysicalObjectSize.CantCarryBig;
          break;
        case SLAM.SLAMMode.ThrownArmed:
          this.FlipPiece.localEulerAngles = new Vector3(this.FlipPieceRots.x, 0.0f, 0.0f);
          this.Light_Red.SetActive(true);
          this.Light_Green.SetActive(false);
          this.Laser_Root.SetActive(false);
          this.Size = FVRPhysicalObject.FVRPhysicalObjectSize.Medium;
          break;
      }
    }

    public void TriggerFlipped(bool isFlip)
    {
      if ((Object) this.QuickbeltSlot != (Object) null)
        return;
      switch (this.Mode)
      {
        case SLAM.SLAMMode.Disabled:
          if (isFlip)
          {
            this.SetMode(SLAM.SLAMMode.LaserPlace);
            SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Flip, this.transform.position);
            break;
          }
          this.SetMode(SLAM.SLAMMode.ThrownArmed);
          SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_ArmButtonClick, this.transform.position);
          break;
        case SLAM.SLAMMode.LaserPlace:
          if (!isFlip)
            break;
          this.SetMode(SLAM.SLAMMode.Disabled);
          SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Flip, this.transform.position);
          break;
        case SLAM.SLAMMode.LaserArmed:
          if (isFlip)
            break;
          this.DetachAndDisarm();
          SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_ArmButtonClick, this.transform.position);
          break;
        case SLAM.SLAMMode.ThrownArmed:
          if (isFlip)
            break;
          this.SetMode(SLAM.SLAMMode.Disabled);
          SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_ArmButtonClick, this.transform.position);
          SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Armed, this.transform.position);
          break;
      }
    }

    private void DetachAndDisarm()
    {
      this.SetMode(SLAM.SLAMMode.Disabled);
      this.RootRigidbody.isKinematic = false;
    }

    private void PrimeLaser()
    {
      this.m_isPriming = true;
      this.m_primeLerp = 0.0f;
    }

    private void EngageLaser()
    {
      this.Laser_Root.SetActive(true);
      float num = 200f;
      if (Physics.Raycast(this.Laser_Root.transform.position, this.Laser_Root.transform.forward, out this.m_hit, 200f, (int) this.LM_Beam, QueryTriggerInteraction.Ignore))
        num = this.m_hit.distance;
      this.m_storedLaserLength = num;
      this.Laser_Beam.localScale = new Vector3(0.02f, 0.02f, this.m_storedLaserLength * 100f);
    }

    protected override void FVRUpdate()
    {
      if (this.m_isPriming)
      {
        this.m_primeLerp += Time.deltaTime;
        if ((double) this.m_primeLerp > 3.0)
        {
          this.m_isPriming = false;
          this.m_isPrimed = true;
          SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Armed, this.transform.position);
          this.EngageLaser();
        }
      }
      if (this.m_isPrimed && this.Mode == SLAM.SLAMMode.LaserArmed)
        this.LaserTest();
      if (this.Mode != SLAM.SLAMMode.LaserPlace)
        return;
      this.AttachTest();
    }

    private void AttachTest()
    {
      if (!Physics.Raycast(this.transform.position, this.transform.forward, out this.m_hit, 0.1f, (int) this.LM_Attach, QueryTriggerInteraction.Ignore))
        return;
      this.transform.position = this.m_hit.point;
      this.transform.rotation = Quaternion.LookRotation(-this.m_hit.normal, this.transform.up);
      SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Priming, this.transform.position);
      this.SetMode(SLAM.SLAMMode.LaserArmed);
      if (this.IsHeld)
        this.ForceBreakInteraction();
      this.RootRigidbody.isKinematic = true;
    }

    private void LaserTest()
    {
      float num = 200f;
      if (Physics.Raycast(this.Laser_Root.transform.position, this.Laser_Root.transform.forward, out this.m_hit, 200f, (int) this.LM_Beam, QueryTriggerInteraction.Ignore))
        num = this.m_hit.distance;
      if ((double) this.m_storedLaserLength > 0.0 && (double) Mathf.Abs(this.m_storedLaserLength - num) > 0.100000001490116)
        this.Detonate();
      this.m_storedLaserLength = num;
      this.Laser_Beam.localScale = new Vector3(0.02f, 0.02f, this.m_storedLaserLength * 100f);
    }

    public void Detonate()
    {
      if (this.m_hasDetonated)
        return;
      FXM.DeRegisterSLAM(this);
      this.m_hasDetonated = true;
      if (this.Mode == SLAM.SLAMMode.LaserArmed || this.Mode == SLAM.SLAMMode.LaserPlace)
      {
        for (int index = 0; index < this.SpawnOnDestroy.Count; ++index)
          Object.Instantiate<GameObject>(this.SpawnOnDestroy[index], this.SpawnPoint.position, this.SpawnPoint.rotation);
      }
      else
      {
        for (int index = 0; index < this.SpawnOnDestroy.Count; ++index)
          Object.Instantiate<GameObject>(this.SpawnOnDestroy_Broad[index], this.SpawnPoint_Broad.position, this.SpawnPoint_Broad.rotation);
      }
      Object.Destroy((Object) this.gameObject);
    }

    public void Damage(FistVR.Damage d)
    {
      if (d.Class != FistVR.Damage.DamageClass.Projectile || this.Mode != SLAM.SLAMMode.LaserArmed && this.Mode != SLAM.SLAMMode.ThrownArmed)
        return;
      this.Detonate();
    }

    public enum SLAMMode
    {
      Disabled,
      LaserPlace,
      LaserArmed,
      ThrownArmed,
    }
  }
}
