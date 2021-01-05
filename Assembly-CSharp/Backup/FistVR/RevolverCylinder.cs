// Decompiled with JetBrains decompiler
// Type: FistVR.RevolverCylinder
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RevolverCylinder : FVRInteractiveObject
  {
    [Header("Revolver Cylinder Config")]
    public Revolver Revolver;
    private float m_fakeAngularVel;
    public int numChambers = 6;
    public float CartridgeLength = 0.04f;

    protected override void Awake() => base.Awake();

    public override bool IsInteractable() => !this.Revolver.isCylinderArmLocked;

    public void LoadFromSpeedLoader(Speedloader loader)
    {
      bool flag = false;
      for (int index = 0; index < loader.Chambers.Count; ++index)
      {
        if (index < this.Revolver.Chambers.Length && loader.Chambers[index].IsLoaded && !this.Revolver.Chambers[index].IsFull)
        {
          this.Revolver.Chambers[index].Autochamber(loader.Chambers[index].Unload());
          flag = true;
        }
      }
      if (!flag)
        return;
      this.Revolver.PlayAudioEvent(FirearmAudioEventType.MagazineIn);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.Revolver.AddCylinderCloseVel((float) (-(double) this.Revolver.transform.InverseTransformDirection(hand.Input.VelLinearWorld).x * 2800.0));
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      this.m_fakeAngularVel = (float) (-(double) this.Revolver.transform.InverseTransformDirection(hand.Input.VelLinearWorld).y * 120.0);
      this.m_fakeAngularVel = Mathf.Clamp(this.m_fakeAngularVel, -360f, 360f);
      base.EndInteraction(hand);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.Revolver.isCylinderArmLocked)
      {
        this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, this.transform.localEulerAngles.z + this.m_fakeAngularVel);
        if ((double) Mathf.Abs(this.m_fakeAngularVel) <= 0.0)
          return;
        this.m_fakeAngularVel = Mathf.Lerp(this.m_fakeAngularVel, 0.0f, Time.deltaTime * 0.8f);
      }
      else
        this.m_fakeAngularVel = 0.0f;
    }

    public int GetClosestChamberIndex()
    {
      float num = -this.transform.localEulerAngles.z;
      if (this.Revolver.isChiappa)
        num += 180f;
      return Mathf.CeilToInt(Mathf.Repeat(num + (float) (360.0 / (double) this.numChambers * 0.5), 360f) / (360f / (float) this.numChambers)) - 1;
    }

    public Quaternion GetLocalRotationFromCylinder(int cylinder)
    {
      float t = (float) ((double) cylinder * (360.0 / (double) this.numChambers) * -1.0);
      if (this.Revolver.isChiappa)
        t += 180f;
      return Quaternion.Euler(new Vector3(0.0f, 0.0f, Mathf.Repeat(t, 360f)));
    }
  }
}
