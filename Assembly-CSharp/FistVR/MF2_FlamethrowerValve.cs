// Decompiled with JetBrains decompiler
// Type: FistVR.MF2_FlamethrowerValve
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MF2_FlamethrowerValve : FVRInteractiveObject
  {
    public Transform RefFrame;
    public Vector2 ValveRotRange = new Vector2(-50f, 50f);
    private float m_valveRot;
    public Transform Valve;
    public float Lerp = 0.5f;
    private Vector3 refDir = Vector3.one;

    public override void BeginInteraction(FVRViveHand hand)
    {
      this.refDir = Vector3.ProjectOnPlane(hand.Input.Up, this.RefFrame.forward).normalized;
      base.BeginInteraction(hand);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      Vector3 normalized = Vector3.ProjectOnPlane(hand.Input.Up, this.RefFrame.forward).normalized;
      Vector3 refDir = this.refDir;
      this.m_valveRot += Mathf.Clamp(Mathf.Atan2(Vector3.Dot(this.RefFrame.forward, Vector3.Cross(refDir, normalized)), Vector3.Dot(refDir, normalized)) * 57.29578f, -5f, 5f);
      this.m_valveRot = Mathf.Clamp(this.m_valveRot, -50f, 50f);
      this.Valve.localEulerAngles = new Vector3(0.0f, 0.0f, this.m_valveRot);
      this.Lerp = Mathf.InverseLerp(this.ValveRotRange.x, this.ValveRotRange.y, this.m_valveRot);
      this.refDir = normalized;
      base.UpdateInteraction(hand);
    }
  }
}
