// Decompiled with JetBrains decompiler
// Type: FistVR.VolcanicLoadingGate
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class VolcanicLoadingGate : FVRInteractiveObject
  {
    public Transform LoadingGate;
    private float m_curRot;
    private float m_tarRot;
    private bool m_isOpen;
    public FVRFireArmMagazine Magazine;
    public GameObject Gate;

    public new void Awake()
    {
      this.Magazine.IsExtractable = !this.m_isOpen;
      this.Gate.SetActive(this.m_isOpen);
    }

    public void Update()
    {
      this.m_curRot = Mathf.MoveTowards(this.m_curRot, this.m_tarRot, Time.deltaTime * 700f);
      this.LoadingGate.localEulerAngles = new Vector3(0.0f, 0.0f, this.m_curRot);
    }

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      this.ToggleGate();
    }

    private void ToggleGate()
    {
      this.m_isOpen = !this.m_isOpen;
      this.m_tarRot = !this.m_isOpen ? 0.0f : 180f;
      this.Magazine.IsExtractable = !this.m_isOpen;
      this.Gate.SetActive(this.m_isOpen);
    }
  }
}
