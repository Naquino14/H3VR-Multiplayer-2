// Decompiled with JetBrains decompiler
// Type: FistVR.LAPD2019Cylinder
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class LAPD2019Cylinder : FVRInteractiveObject
  {
    [Header("Revolver Cylinder Config")]
    public LAPD2019 Gun;
    private float m_fakeAngularVel;
    public int numChambers = 5;

    protected override void Awake() => base.Awake();

    public override bool IsInteractable() => !this.Gun.isCylinderArmLocked;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.Gun.AddCylinderCloseVel((float) (-(double) this.Gun.transform.InverseTransformDirection(hand.Input.VelLinearWorld).x * 2800.0));
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      this.m_fakeAngularVel = (float) (-(double) this.Gun.transform.InverseTransformDirection(hand.Input.VelLinearWorld).y * 120.0);
      this.m_fakeAngularVel = Mathf.Clamp(this.m_fakeAngularVel, -360f, 360f);
      base.EndInteraction(hand);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.Gun.isCylinderArmLocked)
      {
        this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, this.transform.localEulerAngles.z + this.m_fakeAngularVel * 30f * Time.deltaTime);
        if ((double) Mathf.Abs(this.m_fakeAngularVel) > 0.0)
          this.m_fakeAngularVel = Mathf.Lerp(this.m_fakeAngularVel, 0.0f, Time.deltaTime * 0.8f);
        if ((double) Mathf.Abs(this.m_fakeAngularVel) <= 5.0)
          ;
      }
      else
        this.m_fakeAngularVel = 0.0f;
    }

    public int GetClosestChamberIndex() => Mathf.CeilToInt(Mathf.Repeat(-this.transform.localEulerAngles.z + (float) (360.0 / (double) this.numChambers * 0.5), 360f) / (360f / (float) this.numChambers)) - 1;

    public Quaternion GetLocalRotationFromCylinder(int cylinder) => Quaternion.Euler(new Vector3(0.0f, 0.0f, Mathf.Repeat((float) ((double) cylinder * (360.0 / (double) this.numChambers) * -1.0), 360f)));
  }
}
