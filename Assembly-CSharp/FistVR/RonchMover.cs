// Decompiled with JetBrains decompiler
// Type: FistVR.RonchMover
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RonchMover : MonoBehaviour
  {
    [Header("Body Connections")]
    public Transform Torso;
    [Header("Ground Sensing")]
    public Transform CastLeft_Forward;
    public Transform CastLeft_Rearward;
    public Transform CastRight_Forward;
    public Transform CastRight_Rearward;
    public Transform IK_LeftFoot;
    public Transform IK_RightFoot;
    private Vector3 m_curLeftFootPoint;
    private Vector3 m_tarLeftFootPoint;
    private Vector3 m_curRightFootPoint;
    private Vector3 m_tarRightFootPoint;
    public LayerMask LM_GroundCast;
    private RaycastHit m_hit;
    private float m_walkingLerp;
    private float m_torsoLerp_LeftRight = 0.5f;
    private float m_footLerp_Left = 0.5f;
    private float m_footLerp_Right = 0.5f;
    public AnimationCurve CastDistanceCurve_Walking;
    public AudioEvent FootSteps;
    private bool wasLeftFootLerpGoingUp;
    private float m_lastLeftFootLerp;
    private float m_curWorldY;
    private float m_tarWorldY;

    private void Start()
    {
      this.m_curWorldY = this.Torso.position.y;
      this.m_curLeftFootPoint = this.IK_LeftFoot.position;
      this.m_tarLeftFootPoint = this.IK_LeftFoot.position;
      this.m_curRightFootPoint = this.IK_RightFoot.position;
      this.m_curRightFootPoint = this.IK_RightFoot.position;
    }

    public void SetFacing(Vector3 facing) => this.transform.rotation = Quaternion.LookRotation(facing, Vector3.up);

    public bool MoveTowardsPosition(RonchWaypoint tarPoint, float speed)
    {
      Vector3 position1 = this.transform.position;
      position1.y = 0.0f;
      Vector3 position2 = tarPoint.transform.position;
      position2.y = 0.0f;
      Vector3 b = Vector3.MoveTowards(position1, position2, speed * Time.deltaTime);
      bool flag = false;
      if ((double) Vector3.Distance(position2, b) < 1.0 / 1000.0)
        flag = true;
      b.y = this.transform.position.y;
      if (Physics.Raycast(new Vector3(b.x, b.y + 5f, b.z), -Vector3.up, out this.m_hit, 25f, (int) this.LM_GroundCast, QueryTriggerInteraction.Ignore))
        b.y = this.m_hit.point.y;
      this.transform.position = b;
      return flag;
    }

    private void Update()
    {
    }

    public void Shudder() => this.Torso.Rotate(new Vector3(Random.Range(-5f, 5f), 0.0f, Random.Range(-5f, 5f)));

    public void SetToDeathPose()
    {
      this.Torso.localPosition = new Vector3(0.0f, 2f, -4f);
      this.Torso.localEulerAngles = new Vector3(-33f, 0.0f, -4f);
    }

    public float UpdateWalking(float t)
    {
      this.m_walkingLerp += t;
      this.m_walkingLerp = Mathf.Repeat(this.m_walkingLerp, 6.283185f);
      this.m_footLerp_Left = (float) (((double) Mathf.Sin(this.m_walkingLerp) + 1.0) * 0.5);
      this.m_footLerp_Right = (float) (((double) Mathf.Sin(this.m_walkingLerp + 3.141593f) + 1.0) * 0.5);
      if ((double) this.m_footLerp_Left > (double) this.m_lastLeftFootLerp)
      {
        if (!this.wasLeftFootLerpGoingUp)
          SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, this.FootSteps, this.IK_LeftFoot.position + Vector3.up * 2f, Vector3.Distance(this.IK_LeftFoot.position, GM.CurrentPlayerBody.Head.position) / 343f);
        this.wasLeftFootLerpGoingUp = true;
      }
      else
      {
        if (this.wasLeftFootLerpGoingUp)
          SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, this.FootSteps, this.IK_RightFoot.position + Vector3.up * 2f, Vector3.Distance(this.IK_RightFoot.position, GM.CurrentPlayerBody.Head.position) / 343f);
        this.wasLeftFootLerpGoingUp = false;
      }
      this.m_lastLeftFootLerp = this.m_footLerp_Left;
      this.m_torsoLerp_LeftRight = Mathf.Sin(this.m_walkingLerp);
      float time1 = this.m_walkingLerp / 3.141593f;
      float time2 = (float) ((double) this.m_walkingLerp / 3.14159274101257 + 1.0);
      return this.MoveIK(this.m_footLerp_Left, this.m_footLerp_Right, -this.m_torsoLerp_LeftRight, this.CastDistanceCurve_Walking.Evaluate(time1), this.CastDistanceCurve_Walking.Evaluate(time2));
    }

    private float MoveIK(
      float lerpLeft,
      float lerpRight,
      float lerpLeftRight,
      float castDistanceLeft,
      float castDistanceRight)
    {
      Vector3 direction1 = Vector3.Lerp(this.CastLeft_Forward.forward, this.CastLeft_Rearward.forward, lerpLeft);
      Vector3 direction2 = Vector3.Lerp(this.CastRight_Forward.forward, this.CastRight_Rearward.forward, lerpRight);
      this.m_tarLeftFootPoint = !Physics.Raycast(this.CastLeft_Forward.position, direction1, out this.m_hit, castDistanceLeft, (int) this.LM_GroundCast, QueryTriggerInteraction.Ignore) ? this.CastLeft_Forward.position + direction1.normalized * castDistanceLeft : this.m_hit.point;
      this.m_tarRightFootPoint = !Physics.Raycast(this.CastRight_Forward.position, direction2, out this.m_hit, castDistanceRight, (int) this.LM_GroundCast, QueryTriggerInteraction.Ignore) ? this.CastRight_Forward.position + direction2.normalized * castDistanceRight : this.m_hit.point;
      this.m_tarWorldY = Mathf.Lerp(this.IK_LeftFoot.position.y, this.IK_RightFoot.position.y, 0.5f) + 6f;
      this.m_curWorldY = Mathf.MoveTowards(this.m_curWorldY, this.m_tarWorldY, Time.deltaTime * 2f);
      Vector3 vector3 = this.transform.InverseTransformPoint(new Vector3(0.0f, this.m_curWorldY, 0.0f));
      this.Torso.localPosition = new Vector3(lerpLeftRight, vector3.y, Mathf.Lerp(this.IK_LeftFoot.localPosition.z, this.IK_RightFoot.localPosition.z, 0.5f));
      float num = Mathf.Max(Vector3.Distance(new Vector3(this.m_curLeftFootPoint.x, 0.0f, this.m_curLeftFootPoint.z), new Vector3(this.m_tarLeftFootPoint.x, 0.0f, this.m_tarLeftFootPoint.z)), Vector3.Distance(new Vector3(this.m_curRightFootPoint.x, 0.0f, this.m_curRightFootPoint.z), new Vector3(this.m_tarRightFootPoint.x, 0.0f, this.m_tarRightFootPoint.z)));
      this.m_curLeftFootPoint = Vector3.MoveTowards(this.m_curLeftFootPoint, this.m_tarLeftFootPoint, Time.deltaTime * 50f);
      this.m_curRightFootPoint = Vector3.MoveTowards(this.m_curRightFootPoint, this.m_tarRightFootPoint, Time.deltaTime * 50f);
      this.IK_LeftFoot.position = this.m_curLeftFootPoint;
      this.IK_RightFoot.position = this.m_curRightFootPoint;
      return num;
    }
  }
}
