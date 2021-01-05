// Decompiled with JetBrains decompiler
// Type: FistVR.TeleportationRig
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TeleportationRig : MonoBehaviour
  {
    public Transform CircularIndicatorTransform;
    public Renderer CircularIndicatorRenderer;
    public Transform CircularIndicatorBillboard;
    public Transform HeadModel;
    public Transform LeftHandModel;
    public Transform RightHandModel;
    private float m_circularOffsetStart = 0.5f;
    private float m_circularOffsetEnd;
    private float m_indicatorTick;

    private void Start()
    {
      this.CircularIndicatorRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.0f, this.m_circularOffsetStart));
      this.m_indicatorTick = 0.0f;
    }

    public void SetCircularIndicatorTick(float t) => this.CircularIndicatorRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.0f, Mathf.Lerp(this.m_circularOffsetStart, this.m_circularOffsetEnd, t)));

    public void SetRigPositions(
      Vector3 RootPosition,
      Transform Head,
      Transform LeftHand,
      Transform RightHand,
      bool indicatorAtHeadXZ,
      bool displayCircle)
    {
      this.CircularIndicatorTransform.localPosition = !indicatorAtHeadXZ ? new Vector3(0.0f, 1.8f, 0.0f) : new Vector3(Head.localPosition.x, 1.8f, Head.localPosition.z);
      if (displayCircle)
      {
        this.CircularIndicatorBillboard.gameObject.SetActive(true);
        this.CircularIndicatorBillboard.LookAt(Head.position, Vector3.up);
      }
      else
        this.CircularIndicatorBillboard.gameObject.SetActive(false);
      this.transform.position = RootPosition;
      this.HeadModel.localPosition = Head.localPosition;
      this.LeftHandModel.localPosition = LeftHand.localPosition;
      this.RightHandModel.localPosition = RightHand.localPosition;
      this.HeadModel.localRotation = Head.localRotation;
      this.LeftHandModel.localRotation = LeftHand.localRotation;
      this.RightHandModel.localRotation = RightHand.localRotation;
    }
  }
}
