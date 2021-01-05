// Decompiled with JetBrains decompiler
// Type: FistVR.FVRMatchbox
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRMatchbox : FVRPhysicalObject
  {
    [Header("Matchbox Config")]
    public Transform InnerBox;
    public GameObject NewMatchTrigger;
    private float m_openZ = 0.04f;
    private bool m_isOpen;

    protected override void FVRUpdate()
    {
      if (this.m_isOpen)
      {
        this.NewMatchTrigger.SetActive(true);
        this.InnerBox.transform.localPosition = Vector3.Lerp(this.InnerBox.transform.localPosition, new Vector3(0.0f, 0.0f, this.m_openZ), Time.deltaTime * 4f);
      }
      else
      {
        this.NewMatchTrigger.SetActive(false);
        this.InnerBox.transform.localPosition = Vector3.Lerp(this.InnerBox.transform.localPosition, new Vector3(0.0f, 0.0f, 0.0f), Time.deltaTime * 4f);
      }
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      bool flag = false;
      if (hand.Input.TriggerDown)
        flag = true;
      if (hand.IsInStreamlinedMode && hand.Input.BYButtonDown)
        flag = true;
      else if (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown)
        flag = true;
      if (!flag)
        return;
      this.m_isOpen = !this.m_isOpen;
    }
  }
}
