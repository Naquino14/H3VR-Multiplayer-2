// Decompiled with JetBrains decompiler
// Type: FistVR.M203_Fore
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class M203_Fore : FVRInteractiveObject
  {
    public M203 launcher;
    public M203_Fore.ForePos CurPos = M203_Fore.ForePos.Rearward;
    public M203_Fore.ForePos LastPos = M203_Fore.ForePos.Rearward;
    public Transform Point_Forward;
    public Transform Point_Rearward;
    private float m_handZOffset;
    private float m_curSlideSpeed;
    private float m_slideZ_current;
    private float m_slideZ_heldTarget;
    private float m_slideZ_forward;
    private float m_slideZ_rear;
    public Transform EjectPos;

    protected override void Awake()
    {
      base.Awake();
      this.m_slideZ_current = this.transform.localPosition.z;
      this.m_slideZ_forward = this.Point_Forward.localPosition.z;
      this.m_slideZ_rear = this.Point_Rearward.localPosition.z;
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.m_handZOffset = this.transform.InverseTransformPoint(hand.Input.Pos).z;
    }

    public void UpdateSlide()
    {
      bool flag = false;
      if (this.IsHeld)
        flag = true;
      if (this.IsHeld)
        this.m_slideZ_heldTarget = this.launcher.transform.InverseTransformPoint(this.GetClosestValidPoint(this.Point_Forward.position, this.Point_Rearward.position, this.m_hand.Input.Pos + -this.transform.forward * this.m_handZOffset * this.launcher.transform.localScale.x)).z;
      Vector2 vector2 = new Vector2(this.m_slideZ_rear, this.m_slideZ_forward);
      float num1 = this.m_slideZ_current;
      float slideZCurrent = this.m_slideZ_current;
      if (flag)
        num1 = Mathf.MoveTowards(this.m_slideZ_current, this.m_slideZ_heldTarget, 5f * Time.deltaTime);
      float num2 = Mathf.Clamp(num1, vector2.x, vector2.y);
      if ((double) Mathf.Abs(num2 - this.m_slideZ_current) > (double) Mathf.Epsilon)
      {
        this.m_slideZ_current = num2;
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.m_slideZ_current);
      }
      else
        this.m_curSlideSpeed = 0.0f;
      M203_Fore.ForePos curPos1 = this.CurPos;
      M203_Fore.ForePos forePos = (double) Mathf.Abs(this.m_slideZ_current - this.m_slideZ_forward) >= 3.0 / 1000.0 ? ((double) Mathf.Abs(this.m_slideZ_current - this.m_slideZ_rear) >= 1.0 / 1000.0 ? M203_Fore.ForePos.Mid : M203_Fore.ForePos.Rearward) : M203_Fore.ForePos.Forward;
      int curPos2 = (int) this.CurPos;
      this.CurPos = (M203_Fore.ForePos) Mathf.Clamp((int) forePos, curPos2 - 1, curPos2 + 1);
      if (this.CurPos == M203_Fore.ForePos.Rearward && this.LastPos != M203_Fore.ForePos.Rearward)
      {
        this.launcher.Chamber.IsAccessible = false;
        this.CloseEvent();
      }
      else if (this.CurPos != M203_Fore.ForePos.Rearward && this.LastPos == M203_Fore.ForePos.Rearward)
      {
        this.launcher.Chamber.IsAccessible = true;
        this.OpenEvent();
      }
      else if (this.CurPos == M203_Fore.ForePos.Forward && this.LastPos != M203_Fore.ForePos.Forward)
        this.EjectEvent();
      this.LastPos = this.CurPos;
    }

    private void CloseEvent() => this.launcher.PlayAudioEvent(FirearmAudioEventType.BreachClose);

    private void OpenEvent() => this.launcher.PlayAudioEvent(FirearmAudioEventType.BreachOpen);

    private void EjectEvent()
    {
      if (!this.launcher.Chamber.IsFull)
        return;
      this.launcher.Chamber.EjectRound(this.EjectPos.position, -this.transform.forward, Vector3.zero);
      this.launcher.PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
    }

    public enum ForePos
    {
      Forward,
      Mid,
      Rearward,
    }
  }
}
