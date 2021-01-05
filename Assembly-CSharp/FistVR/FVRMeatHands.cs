// Decompiled with JetBrains decompiler
// Type: FistVR.FVRMeatHands
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FVRMeatHands : MonoBehaviour
  {
    public FVRViveHand hand;
    public List<Transform> FingerJoints_0;
    public List<Transform> WaggleJoints_0;
    public List<WaggleJoint> WaggleJointsCore_0;
    public List<Transform> FingerJoints_1;
    public List<Transform> WaggleJoints_1;
    public List<WaggleJoint> WaggleJointsCore_1;
    public List<Transform> FingerJoints_2;
    public List<Transform> WaggleJoints_2;
    public List<WaggleJoint> WaggleJointsCore_2;
    public List<Transform> FingerJoints_3;
    public List<Transform> WaggleJoints_3;
    public List<WaggleJoint> WaggleJointsCore_3;
    public List<Transform> FingerJoints_Thumb;
    public List<Transform> WaggleJoints_Thumb;
    public List<WaggleJoint> WaggleJointsCore_Thumb;

    private void Update()
    {
      if (this.hand.CMode != ControlMode.Index)
        return;
      for (int index = 0; index < this.FingerJoints_0.Count; ++index)
      {
        float fingerCurlIndex = this.hand.Input.FingerCurl_Index;
        float b = Mathf.Lerp(10f, 75f, this.hand.Input.FingerCurl_Index);
        float a = this.WaggleJoints_0[index].localEulerAngles.x + b;
        if ((double) a > 360.0)
          a -= 360f;
        if ((double) a < -360.0)
          a += 360f;
        float x = Mathf.Lerp(a, b, fingerCurlIndex);
        this.FingerJoints_0[index].localEulerAngles = new Vector3(x, 0.0f, 0.0f);
        this.WaggleJointsCore_0[index].Execute();
      }
      for (int index = 0; index < this.FingerJoints_1.Count; ++index)
      {
        float fingerCurlMiddle = this.hand.Input.FingerCurl_Middle;
        float b = Mathf.Lerp(10f, 75f, this.hand.Input.FingerCurl_Middle);
        float a = this.WaggleJoints_1[index].localEulerAngles.x + b;
        if ((double) a > 360.0)
          a -= 360f;
        if ((double) a < -360.0)
          a += 360f;
        float x = Mathf.Lerp(a, b, fingerCurlMiddle);
        this.FingerJoints_1[index].localEulerAngles = new Vector3(x, 0.0f, 0.0f);
        this.WaggleJointsCore_1[index].Execute();
      }
      for (int index = 0; index < this.FingerJoints_2.Count; ++index)
      {
        float fingerCurlRing = this.hand.Input.FingerCurl_Ring;
        float b = Mathf.Lerp(10f, 75f, this.hand.Input.FingerCurl_Ring);
        float a = this.WaggleJoints_2[index].localEulerAngles.x + b;
        if ((double) a > 360.0)
          a -= 360f;
        if ((double) a < -360.0)
          a += 360f;
        float x = Mathf.Lerp(a, b, fingerCurlRing);
        this.FingerJoints_2[index].localEulerAngles = new Vector3(x, 0.0f, 0.0f);
        this.WaggleJointsCore_2[index].Execute();
      }
      for (int index = 0; index < this.FingerJoints_3.Count; ++index)
      {
        float fingerCurlPinky = this.hand.Input.FingerCurl_Pinky;
        float b = Mathf.Lerp(10f, 75f, this.hand.Input.FingerCurl_Pinky);
        float a = this.WaggleJoints_3[index].localEulerAngles.x + b;
        if ((double) a > 360.0)
          a -= 360f;
        if ((double) a < -360.0)
          a += 360f;
        float x = Mathf.Lerp(a, b, fingerCurlPinky);
        this.FingerJoints_3[index].localEulerAngles = new Vector3(x, 0.0f, 0.0f);
        this.WaggleJointsCore_3[index].Execute();
      }
      for (int index = 0; index < this.FingerJoints_Thumb.Count; ++index)
      {
        float fingerCurlThumb = this.hand.Input.FingerCurl_Thumb;
        float b = Mathf.Lerp(10f, 75f, this.hand.Input.FingerCurl_Thumb);
        float a = this.WaggleJoints_Thumb[index].localEulerAngles.x + b;
        if ((double) a > 360.0)
          a -= 360f;
        if ((double) a < -360.0)
          a += 360f;
        float x = Mathf.Lerp(a, b, fingerCurlThumb);
        this.FingerJoints_Thumb[index].localEulerAngles = new Vector3(x, 0.0f, 0.0f);
        this.WaggleJointsCore_Thumb[index].Execute();
      }
    }
  }
}
