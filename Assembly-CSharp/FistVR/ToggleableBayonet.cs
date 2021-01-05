// Decompiled with JetBrains decompiler
// Type: FistVR.ToggleableBayonet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ToggleableBayonet : FVRInteractiveObject
  {
    public FVRFireArm FA;
    public Transform Bayonet;
    public float BayonetRot_Closed;
    public float BayonetRot_Open;
    private bool m_bayonetEnabled;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      this.ToggleBayonet();
    }

    private void ToggleBayonet()
    {
      if (this.FA.MP.IsJointedToObject)
        return;
      this.m_bayonetEnabled = !this.m_bayonetEnabled;
      if (this.m_bayonetEnabled)
      {
        this.Bayonet.localEulerAngles = new Vector3(this.BayonetRot_Open, 0.0f, 0.0f);
        this.FA.MP.CanNewStab = true;
        this.FA.PlayAudioEvent(FirearmAudioEventType.BipodOpen);
      }
      else
      {
        this.Bayonet.localEulerAngles = new Vector3(this.BayonetRot_Closed, 0.0f, 0.0f);
        this.FA.MP.CanNewStab = false;
        this.FA.PlayAudioEvent(FirearmAudioEventType.BipodClosed);
      }
    }
  }
}
