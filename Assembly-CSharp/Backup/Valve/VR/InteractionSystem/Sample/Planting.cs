// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.Planting
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class Planting : MonoBehaviour
  {
    public SteamVR_Action_Boolean plantAction;
    public Hand hand;
    public GameObject prefabToPlant;

    private void OnEnable()
    {
      if ((Object) this.hand == (Object) null)
        this.hand = this.GetComponent<Hand>();
      if ((SteamVR_Action) this.plantAction == (SteamVR_Action) null)
        UnityEngine.Debug.LogError((object) "<b>[SteamVR Interaction]</b> No plant action assigned");
      else
        this.plantAction.AddOnChangeListener(new SteamVR_Action_Boolean.ChangeHandler(this.OnPlantActionChange), this.hand.handType);
    }

    private void OnDisable()
    {
      if (!((SteamVR_Action) this.plantAction != (SteamVR_Action) null))
        return;
      this.plantAction.RemoveOnChangeListener(new SteamVR_Action_Boolean.ChangeHandler(this.OnPlantActionChange), this.hand.handType);
    }

    private void OnPlantActionChange(
      SteamVR_Action_Boolean actionIn,
      SteamVR_Input_Sources inputSource,
      bool newValue)
    {
      if (!newValue)
        return;
      this.Plant();
    }

    public void Plant() => this.StartCoroutine(this.DoPlant());

    [DebuggerHidden]
    private IEnumerator DoPlant() => (IEnumerator) new Planting.\u003CDoPlant\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
