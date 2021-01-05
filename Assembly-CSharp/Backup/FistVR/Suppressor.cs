// Decompiled with JetBrains decompiler
// Type: FistVR.Suppressor
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class Suppressor : MuzzleDevice
  {
    [Header("Suppressor Settings")]
    public float CatchRot;
    private float m_smoothedCatchRotDelta;
    public AudioSource AudSourceScrewOnOff;
    public AudioClip[] AudClipsScrewOnOff;
    public bool IsIntegrate;
    private Vector3 lastHandForward = Vector3.zero;
    private Vector3 lastMountForward = Vector3.zero;

    public override void BeginInteraction(FVRViveHand hand)
    {
      if (this.IsIntegrate)
        return;
      base.BeginInteraction(hand);
    }

    public void CatchRotDeltaAdd(float f) => this.m_smoothedCatchRotDelta += Mathf.Abs(f);

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) this.m_smoothedCatchRotDelta > 0.0)
      {
        if (!this.AudSourceScrewOnOff.isPlaying)
        {
          this.AudSourceScrewOnOff.clip = this.AudClipsScrewOnOff[Random.Range(0, this.AudClipsScrewOnOff.Length)];
          this.AudSourceScrewOnOff.Play();
          this.AudSourceScrewOnOff.volume = Mathf.Clamp(this.m_smoothedCatchRotDelta * 0.03f, 0.0f, 1.2f);
        }
        else
          this.AudSourceScrewOnOff.volume = Mathf.Clamp(this.m_smoothedCatchRotDelta * 0.03f, 0.0f, 1.2f);
        this.m_smoothedCatchRotDelta -= 400f * Time.deltaTime;
      }
      else
      {
        if (!((Object) this.AudSourceScrewOnOff != (Object) null))
          return;
        this.AudSourceScrewOnOff.volume = 0.0f;
        if (!this.AudSourceScrewOnOff.isPlaying)
          return;
        this.AudSourceScrewOnOff.Stop();
      }
    }

    protected override Quaternion GetRotTarget()
    {
      if (!((Object) this.Sensor.CurHoveredMount != (Object) null))
        return base.GetRotTarget();
      Vector3 up = this.Sensor.CurHoveredMount.transform.up;
      return Quaternion.LookRotation(this.Sensor.CurHoveredMount.transform.forward, Quaternion.AngleAxis(this.CatchRot, this.Sensor.CurHoveredMount.transform.forward) * up);
    }

    protected override Vector3 GetPosTarget()
    {
      if (!((Object) this.Sensor.CurHoveredMount != (Object) null))
        return base.GetPosTarget();
      Vector3 closestValidPoint = this.GetClosestValidPoint(this.Sensor.CurHoveredMount.Point_Front.position, this.Sensor.CurHoveredMount.Point_Rear.position, this.m_handPos);
      return (double) Vector3.Distance(closestValidPoint, this.m_handPos) < 0.150000005960464 || (double) this.CatchRot > 1.0 ? closestValidPoint : base.GetPosTarget();
    }

    protected override void UpdateSnappingBasedOnDistance()
    {
      if ((Object) this.Sensor.CurHoveredMount != (Object) null)
      {
        if ((double) Vector3.Distance(this.GetClosestValidPoint(this.Sensor.CurHoveredMount.Point_Front.position, (this.Sensor.CurHoveredMount.GetRootMount().MyObject as FVRFireArm).MuzzlePos.position, this.transform.position), this.transform.position) < 0.0799999982118607 || (double) this.CatchRot > 1.0)
          this.SetSnapping(true);
        else
          this.SetSnapping(false);
      }
      else
        this.SetSnapping(false);
    }

    protected override void FVRFixedUpdate()
    {
      if (this.IsHeld && this.m_isInSnappingMode && (Object) this.Sensor.CurHoveredMount != (Object) null)
      {
        float catchRot = this.CatchRot;
        Vector3 lhs1 = Vector3.ProjectOnPlane(this.m_hand.transform.up, this.transform.forward);
        Vector3 rhs1 = Vector3.ProjectOnPlane(this.lastHandForward, this.transform.forward);
        this.CatchRot -= Mathf.Atan2(Vector3.Dot(this.transform.forward, Vector3.Cross(lhs1, rhs1)), Vector3.Dot(lhs1, rhs1)) * 57.29578f;
        Vector3 lhs2 = Vector3.ProjectOnPlane(this.Sensor.CurHoveredMount.transform.up, this.transform.forward);
        Vector3 rhs2 = Vector3.ProjectOnPlane(this.lastMountForward, this.transform.forward);
        this.CatchRot += Mathf.Atan2(Vector3.Dot(this.transform.forward, Vector3.Cross(lhs2, rhs2)), Vector3.Dot(lhs2, rhs2)) * 57.29578f;
        this.CatchRot = Mathf.Clamp(this.CatchRot, 0.0f, 360f);
        this.lastHandForward = this.m_hand.transform.up;
        this.lastMountForward = this.Sensor.CurHoveredMount.transform.up;
        this.CatchRotDeltaAdd(Mathf.Abs(this.CatchRot - catchRot));
      }
      base.FVRFixedUpdate();
    }

    public void AutoMountWell()
    {
      this.CatchRot = 360f;
      this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, this.CatchRot);
    }

    protected override void SetSnapping(bool b)
    {
      if (this.m_isInSnappingMode == b)
        return;
      this.m_isInSnappingMode = b;
      if (this.m_isInSnappingMode)
      {
        this.SetAllCollidersToLayer(false, "NoCol");
        if ((Object) this.m_hand != (Object) null)
          this.lastHandForward = this.m_hand.transform.up;
        this.lastMountForward = this.Sensor.CurHoveredMount.transform.up;
      }
      else
        this.SetAllCollidersToLayer(false, "Default");
    }

    public override void AttachToMount(FVRFireArmAttachmentMount m, bool playSound)
    {
      base.AttachToMount(m, playSound);
      this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, this.CatchRot);
    }

    public virtual void ShotEffect()
    {
    }
  }
}
