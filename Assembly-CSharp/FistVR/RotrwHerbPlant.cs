// Decompiled with JetBrains decompiler
// Type: FistVR.RotrwHerbPlant
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RotrwHerbPlant : FVRInteractiveObject
  {
    public RotrwHerb.HerbType Type;
    public GameObject VisibleFruit;
    private bool m_hasFruit = true;
    public FVRObject FruitObject;
    public AudioEvent AudEvent_PickFruit;
    private float m_timeTilRegrow;

    public override bool IsInteractable() => this.m_hasFruit;

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.m_hasFruit = false;
      this.PickFruit();
      FVRPhysicalObject component = Object.Instantiate<GameObject>(this.FruitObject.GetGameObject(), this.VisibleFruit.transform.position, this.VisibleFruit.transform.rotation).GetComponent<FVRPhysicalObject>();
      hand.ForceSetInteractable((FVRInteractiveObject) component);
      component.BeginInteraction(hand);
    }

    private void PickFruit()
    {
      this.m_hasFruit = false;
      this.VisibleFruit.SetActive(false);
      SM.PlayGenericSound(this.AudEvent_PickFruit, this.VisibleFruit.transform.position);
      this.m_timeTilRegrow = Random.Range(180f, 360f);
    }

    public void Update()
    {
      if (this.m_hasFruit)
        return;
      if ((double) this.m_timeTilRegrow > 0.0)
        this.m_timeTilRegrow -= Time.deltaTime;
      else
        this.RegrowFruit();
    }

    public void RegrowFruit()
    {
      this.m_hasFruit = true;
      this.VisibleFruit.SetActive(true);
    }
  }
}
