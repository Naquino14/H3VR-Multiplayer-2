// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFirearmBeltDisplayData
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRFirearmBeltDisplayData : MonoBehaviour
  {
    public FVRFireArm Firearm;
    public FVRObject BeltSegmentPrefab;
    public Transform HiddenInBoxSpot;
    public Transform GrabPoint_Box;
    public Transform GrabPoint_Gun;
    public float DistanceBetweenRounds = 0.1f;
    public float GrabRotLimitToTheRight = 20f;
    public float GrabRotLimitToTheLeft = 20f;
    public FVRFirearmBeltDisplayData.BeltPointChain ChainInterpolated01;
    public FVRFirearmBeltDisplayData.BeltPointChain ChainInterpolatedInOut;
    public FVRFirearmBeltDisplayData.BeltPointChain Chain_In;
    public FVRFirearmBeltDisplayData.BeltPointChain Chain_Out;
    public List<Transform> TempBeltChainIn;
    public List<Transform> TempBeltChainOut;
    public List<Transform> ProxyRounds;
    private List<Renderer> m_proxyRends = new List<Renderer>();
    private List<MeshFilter> m_proxyMeshes = new List<MeshFilter>();
    public int BeltCapacity = 15;
    public List<FVRLoadedRound> BeltRounds = new List<FVRLoadedRound>();
    private int m_roundsOnBelt;
    private float m_grabLerp;
    private bool m_isBeltGrabbed;
    private bool m_isStraightAngle;
    public bool InvertBeltSide;
    public AnimationCurve AngleFromUpToInOutLerp;
    private float m_lerpCycle;
    private float m_jitterImpulse;
    private Vector3 m_handPos;

    public bool isBeltGrabbed() => this.m_isBeltGrabbed;

    private void Awake()
    {
      for (int index = 0; index < this.ProxyRounds.Count; ++index)
      {
        this.m_proxyRends.Add(this.ProxyRounds[index].gameObject.GetComponent<Renderer>());
        this.m_proxyMeshes.Add(this.ProxyRounds[index].gameObject.GetComponent<MeshFilter>());
      }
    }

    public void AddJitter() => this.m_jitterImpulse += UnityEngine.Random.Range(-0.2f, 0.4f);

    public void UpdateBelt()
    {
      this.UpdateBeltData();
      if (!this.m_isBeltGrabbed)
        this.UpdateProxyRounds(0);
      if ((double) this.m_jitterImpulse > 0.0)
        this.m_jitterImpulse -= Time.deltaTime * 1.5f;
      this.m_jitterImpulse = Mathf.Clamp(this.m_jitterImpulse, 0.0f, 1f);
    }

    public FVRFireArmBeltSegment StripBeltSegment(Vector3 handPos)
    {
      FVRFireArmBeltSegment component = UnityEngine.Object.Instantiate<GameObject>(this.BeltSegmentPrefab.GetGameObject(), handPos, Quaternion.LookRotation(this.Firearm.transform.forward, this.Firearm.transform.up)).GetComponent<FVRFireArmBeltSegment>();
      for (int index = 0; index < this.BeltRounds.Count; ++index)
        component.RoundList.Add(this.BeltRounds[index]);
      this.BeltRounds.Clear();
      this.m_roundsOnBelt = 0;
      component.UpdateBulletDisplay();
      this.Firearm.HasBelt = false;
      this.Firearm.ConnectedToBox = false;
      this.m_isBeltGrabbed = false;
      this.m_isStraightAngle = false;
      this.Firearm.PlayAudioEvent(FirearmAudioEventType.BeltGrab);
      return component;
    }

    public void MountBeltSegment(FVRFireArmBeltSegment segment)
    {
      for (int index = 0; index < segment.RoundList.Count; ++index)
        this.BeltRounds.Add(segment.RoundList[index]);
      this.m_roundsOnBelt = this.BeltRounds.Count;
      this.Firearm.PlayAudioEvent(FirearmAudioEventType.BeltRelease);
      this.Firearm.HasBelt = true;
      this.Firearm.ConnectedToBox = false;
      this.m_isBeltGrabbed = false;
      this.m_isStraightAngle = false;
    }

    public void UpdateProxyRounds(int offset)
    {
      this.UpdateBeltData();
      for (int index = 0; index < offset; ++index)
      {
        if (index < this.ProxyRounds.Count && this.ProxyRounds[index].gameObject.activeSelf)
          this.ProxyRounds[index].gameObject.SetActive(false);
      }
      int index1 = 0;
      for (int index2 = offset; index2 < this.m_roundsOnBelt + offset; ++index2)
      {
        if (index2 < this.ProxyRounds.Count)
        {
          if (!this.ProxyRounds[index2].gameObject.activeSelf)
            this.ProxyRounds[index2].gameObject.SetActive(true);
          this.ProxyRounds[index2].localPosition = this.ChainInterpolated01.localPosList[index2];
          if (this.m_isStraightAngle && this.m_isBeltGrabbed)
            this.ProxyRounds[index2].rotation = this.ChainInterpolated01.localRotList[index2];
          else
            this.ProxyRounds[index2].localRotation = this.ChainInterpolated01.localRotList[index2];
          this.m_proxyRends[index2].material = this.BeltRounds[index1].LR_Material;
          this.m_proxyMeshes[index2].mesh = this.BeltRounds[index1].LR_Mesh;
          ++index1;
        }
      }
      for (int index2 = this.m_roundsOnBelt + offset; index2 < this.ProxyRounds.Count; ++index2)
        this.ProxyRounds[index2].gameObject.SetActive(false);
    }

    public bool HasARound() => (!this.Firearm.UsesTopCover || !this.Firearm.IsTopCoverUp) && this.m_roundsOnBelt > 0;

    public GameObject RemoveRound(bool b)
    {
      GameObject gameObject = this.BeltRounds[0].LR_ObjectWrapper.GetGameObject();
      if (!GM.CurrentPlayerBody.IsInfiniteAmmo && this.m_roundsOnBelt > 0)
      {
        this.BeltRounds.RemoveAt(0);
        --this.m_roundsOnBelt;
      }
      this.PullPushBelt(this.Firearm.Magazine, this.BeltCapacity);
      if (this.m_roundsOnBelt <= 0)
        this.Firearm.HasBelt = false;
      return gameObject;
    }

    public void BeltGrabbed(FVRFireArmMagazine mag, FVRViveHand hand)
    {
      this.m_isBeltGrabbed = true;
      this.Firearm.PlayAudioEvent(FirearmAudioEventType.BeltGrab);
    }

    public void BeltGrabUpdate(FVRFireArmMagazine mag, FVRViveHand hand)
    {
      if (!this.m_isBeltGrabbed)
        return;
      this.m_handPos = hand.Input.Pos;
      int desiredNumber;
      if (this.m_isStraightAngle)
      {
        desiredNumber = Mathf.Clamp((int) ((double) Vector3.ProjectOnPlane(this.m_handPos - this.GrabPoint_Box.position, this.Firearm.transform.forward).magnitude / (double) this.DistanceBetweenRounds) + 2, 0, this.BeltCapacity);
      }
      else
      {
        this.m_handPos = hand.Input.Pos;
        this.m_grabLerp = this.GetGrabPointLerp(this.m_handPos);
        desiredNumber = Mathf.Clamp((int) Mathf.Lerp(0.0f, (float) this.BeltCapacity, this.m_grabLerp), 0, this.BeltCapacity);
      }
      this.PullPushBelt(mag, desiredNumber);
    }

    public void BeltReleased(FVRFireArmMagazine mag, FVRViveHand hand)
    {
      this.m_isBeltGrabbed = false;
      this.m_isStraightAngle = false;
      bool flag1 = false;
      Vector3 closestValidPoint = this.Firearm.GetClosestValidPoint(this.GrabPoint_Box.position, this.GrabPoint_Gun.position, hand.Input.Pos);
      bool flag2 = true;
      if (this.Firearm is OpenBoltReceiver && (double) (this.Firearm as OpenBoltReceiver).Bolt.GetBoltLerpBetweenLockAndFore() > 0.00499999988824129 && this.Firearm.RequiresBoltBackToSeatBelt)
        flag2 = false;
      if (!this.Firearm.HasBelt && (this.Firearm.IsTopCoverUp || !this.Firearm.UsesTopCover) && ((double) Vector3.Distance(closestValidPoint, this.GrabPoint_Gun.position) < 0.0199999995529652 && flag2))
      {
        flag1 = true;
        this.PullPushBelt(mag, this.BeltCapacity);
      }
      if (flag1)
      {
        this.Firearm.PlayAudioEvent(FirearmAudioEventType.BeltSeat);
        this.Firearm.ConnectedToBox = true;
        this.Firearm.HasBelt = true;
      }
      else
      {
        this.Firearm.PlayAudioEvent(FirearmAudioEventType.BeltRelease);
        this.Firearm.ConnectedToBox = false;
        this.Firearm.HasBelt = false;
        if ((UnityEngine.Object) mag != (UnityEngine.Object) null)
          this.PullPushBelt(mag, 0);
      }
      this.UpdateProxyRounds(0);
    }

    public void ForceRelease()
    {
      this.Firearm.PlayAudioEvent(FirearmAudioEventType.BeltRelease);
      this.Firearm.ConnectedToBox = false;
      this.Firearm.HasBelt = false;
      if (!((UnityEngine.Object) this.Firearm.Magazine != (UnityEngine.Object) null))
        return;
      this.PullPushBelt(this.Firearm.Magazine, 0);
    }

    public void PullPushBelt(FVRFireArmMagazine mag, int desiredNumber)
    {
      if ((UnityEngine.Object) mag == (UnityEngine.Object) null)
      {
        this.UpdateProxyRounds(this.BeltCapacity - desiredNumber);
      }
      else
      {
        if (desiredNumber > this.m_roundsOnBelt)
        {
          int num = desiredNumber - this.m_roundsOnBelt;
          for (int index = 0; index < num; ++index)
          {
            if ((this.Firearm.ConnectedToBox || this.isBeltGrabbed()) && mag.HasARound())
              this.BeltRounds.Add(mag.RemoveRound(0));
            else
              this.Firearm.ConnectedToBox = false;
          }
        }
        else if (desiredNumber < this.m_roundsOnBelt)
        {
          int num = this.m_roundsOnBelt - desiredNumber;
          for (int index = 0; index < num; ++index)
          {
            if (this.BeltRounds.Count > 0)
            {
              FVRLoadedRound beltRound = this.BeltRounds[this.BeltRounds.Count - 1];
              mag.AddRound(beltRound.LR_Class, false, false);
              this.BeltRounds.RemoveAt(this.BeltRounds.Count - 1);
            }
          }
        }
        this.m_roundsOnBelt = this.BeltRounds.Count;
        if (this.m_isStraightAngle)
          this.UpdateProxyRounds(0);
        else
          this.UpdateProxyRounds(this.BeltCapacity - desiredNumber);
      }
    }

    public void UpdateBeltData()
    {
      this.m_isStraightAngle = false;
      Vector3 vector3_1 = Vector3.ProjectOnPlane(this.m_handPos - this.GrabPoint_Box.position, this.Firearm.transform.forward);
      Vector3 to = this.transform.right;
      if (this.InvertBeltSide)
        to = -this.transform.right;
      if ((double) Vector3.Angle(vector3_1, to) < 90.0)
      {
        if ((double) Vector3.Angle(vector3_1, this.transform.up) < (double) this.GrabRotLimitToTheRight)
          this.m_isStraightAngle = true;
      }
      else if ((double) Vector3.Angle(vector3_1, this.transform.up) < (double) this.GrabRotLimitToTheLeft)
      {
        this.m_isStraightAngle = true;
      }
      else
      {
        vector3_1 = Vector3.RotateTowards(this.transform.up, vector3_1, this.GrabRotLimitToTheLeft * 0.0174533f, 1f);
        this.m_isStraightAngle = true;
      }
      if (this.m_isBeltGrabbed && this.m_isStraightAngle)
      {
        Vector3 direction = Vector3.ClampMagnitude(vector3_1, 1f / 1000f + Mathf.Clamp(this.DistanceBetweenRounds * ((float) this.BeltCapacity - 1f), 0.0f, 1f));
        Vector3 vector3_2 = direction;
        Vector3 vector3_3 = this.transform.InverseTransformDirection(direction);
        Vector3 vector3_4 = -vector3_3.normalized * this.DistanceBetweenRounds;
        Vector3 vector3_5 = this.GrabPoint_Box.localPosition + vector3_3;
        Quaternion quaternion = Quaternion.LookRotation(this.transform.forward, -vector3_2);
        for (int index = 0; index < this.ChainInterpolated01.localPosList.Count; ++index)
        {
          this.ChainInterpolated01.localPosList[index] = vector3_5 + vector3_4 * (float) index;
          this.ChainInterpolated01.localRotList[index] = quaternion;
        }
      }
      else
      {
        Vector3 normalized = (this.transform.up - this.transform.right).normalized;
        float num1 = Vector3.Angle(normalized, Vector3.up);
        float num2 = 0.0f;
        if (this.Firearm.IsHeld)
          num2 = (float) ((double) Vector3.Dot(this.Firearm.m_hand.Input.VelLinearWorld.normalized, normalized) * (double) this.Firearm.m_hand.Input.VelLinearWorld.magnitude * 4.0);
        this.ChainInterpolatedInOut.SetInterp(this.Chain_In, this.Chain_Out, this.AngleFromUpToInOutLerp.Evaluate(num1 - num2) + this.m_jitterImpulse);
        if (this.Firearm is OpenBoltReceiver)
        {
          OpenBoltReceiver firearm = this.Firearm as OpenBoltReceiver;
          bool flag = true;
          if (this.Firearm.UsesTopCover && this.Firearm.IsTopCoverUp)
            flag = false;
          this.m_lerpCycle = firearm.HasExtractedRound() || !flag ? 1f : firearm.Bolt.GetBoltLerpBetweenLockAndFore();
        }
        else if (this.Firearm is ClosedBoltWeapon)
        {
          ClosedBoltWeapon firearm = this.Firearm as ClosedBoltWeapon;
          bool flag = true;
          if (this.Firearm.UsesTopCover && this.Firearm.IsTopCoverUp)
            flag = false;
          this.m_lerpCycle = firearm.HasExtractedRound() || !flag ? 1f : firearm.Bolt.GetBoltLerpBetweenLockAndFore();
        }
        this.ChainInterpolated01.SetInterp(this.ChainInterpolatedInOut, this.m_lerpCycle);
      }
    }

    public float GetGrabPointLerp(Vector3 point)
    {
      Vector3 closestValidPoint = this.Firearm.GetClosestValidPoint(this.GrabPoint_Box.position, this.GrabPoint_Gun.position, point);
      float num = Vector3.Distance(this.GrabPoint_Box.position, this.GrabPoint_Gun.position);
      return Vector3.Distance(closestValidPoint, this.GrabPoint_Box.position) / num;
    }

    [ContextMenu("SaveChainsToDisplayData")]
    public void SaveChainsToDisplayData()
    {
      this.Chain_In.Clear();
      this.Chain_Out.Clear();
      this.ChainInterpolatedInOut.Clear();
      this.ChainInterpolated01.Clear();
      for (int index = 0; index < this.TempBeltChainIn.Count; ++index)
      {
        this.Chain_In.localPosList.Add(this.TempBeltChainIn[index].localPosition);
        this.Chain_In.localRotList.Add(this.TempBeltChainIn[index].rotation);
        this.ChainInterpolatedInOut.localPosList.Add(this.TempBeltChainIn[index].localPosition);
        this.ChainInterpolatedInOut.localRotList.Add(this.TempBeltChainIn[index].rotation);
        this.ChainInterpolated01.localPosList.Add(this.TempBeltChainIn[index].localPosition);
        this.ChainInterpolated01.localRotList.Add(this.TempBeltChainIn[index].rotation);
      }
      for (int index = 0; index < this.TempBeltChainOut.Count; ++index)
      {
        this.Chain_Out.localPosList.Add(this.TempBeltChainOut[index].localPosition);
        this.Chain_Out.localRotList.Add(this.TempBeltChainOut[index].rotation);
      }
    }

    [Serializable]
    public class BeltPointChain
    {
      public List<Vector3> localPosList = new List<Vector3>();
      public List<Quaternion> localRotList = new List<Quaternion>();

      public void Clear()
      {
        this.localPosList.Clear();
        this.localRotList.Clear();
      }

      public void SetInterp(
        FVRFirearmBeltDisplayData.BeltPointChain bpc1,
        FVRFirearmBeltDisplayData.BeltPointChain bpc2,
        float l)
      {
        for (int index = 0; index < this.localPosList.Count; ++index)
        {
          this.localPosList[index] = Vector3.Lerp(bpc1.localPosList[index], bpc2.localPosList[index], l);
          this.localRotList[index] = Quaternion.Slerp(bpc1.localRotList[index], bpc2.localRotList[index], l);
        }
      }

      public void SetInterp(FVRFirearmBeltDisplayData.BeltPointChain bpc1, float l)
      {
        for (int index = 0; index < this.localPosList.Count - 1; ++index)
        {
          this.localPosList[index] = Vector3.Lerp(bpc1.localPosList[index], bpc1.localPosList[index + 1], l);
          this.localRotList[index] = Quaternion.Slerp(bpc1.localRotList[index], bpc1.localRotList[index + 1], l);
        }
      }
    }
  }
}
