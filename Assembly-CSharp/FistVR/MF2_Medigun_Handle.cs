// Decompiled with JetBrains decompiler
// Type: FistVR.MF2_Medigun_Handle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MF2_Medigun_Handle : FVRInteractiveObject
  {
    public MF2_Medigun Gun;
    public MF2_Medigun_Handle.HandleState State;
    private Quaternion m_curLocalRot;
    private Quaternion m_tarLocalRot;

    protected override void Awake()
    {
      base.Awake();
      this.m_curLocalRot = this.Gun.HandleRot_Back.localRotation;
      this.transform.localRotation = this.m_curLocalRot;
      this.m_tarLocalRot = this.m_curLocalRot;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 from = Vector3.ProjectOnPlane(hand.transform.position - this.transform.position, this.Gun.HandleRot_Up.forward);
      if ((double) Vector3.Angle(from, this.Gun.HandleRot_Front.forward) < (double) Vector3.Angle(from, this.Gun.HandleRot_Back.forward))
      {
        this.m_tarLocalRot = this.Gun.HandleRot_Front.localRotation;
        if (this.State == MF2_Medigun_Handle.HandleState.Back)
          this.Gun.HandleEngage();
        this.State = MF2_Medigun_Handle.HandleState.Forward;
      }
      else
      {
        this.m_tarLocalRot = this.Gun.HandleRot_Back.localRotation;
        if (this.State == MF2_Medigun_Handle.HandleState.Forward)
          this.Gun.HandleDisEngage();
        this.State = MF2_Medigun_Handle.HandleState.Back;
      }
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.m_curLocalRot = Quaternion.Slerp(this.m_curLocalRot, this.m_tarLocalRot, Time.deltaTime * 10f);
      this.transform.localRotation = this.m_curLocalRot;
    }

    public enum HandleState
    {
      Back,
      Forward,
    }
  }
}
