// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.UIElement
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Valve.VR.InteractionSystem
{
  [RequireComponent(typeof (Interactable))]
  public class UIElement : MonoBehaviour
  {
    public CustomEvents.UnityEventHand onHandClick;
    protected Hand currentHand;

    protected virtual void Awake()
    {
      Button component = this.GetComponent<Button>();
      if (!(bool) (Object) component)
        return;
      component.onClick.AddListener(new UnityAction(this.OnButtonClick));
    }

    protected virtual void OnHandHoverBegin(Hand hand)
    {
      this.currentHand = hand;
      InputModule.instance.HoverBegin(this.gameObject);
      ControllerButtonHints.ShowButtonHint(hand, (ISteamVR_Action_In_Source) hand.uiInteractAction);
    }

    protected virtual void OnHandHoverEnd(Hand hand)
    {
      InputModule.instance.HoverEnd(this.gameObject);
      ControllerButtonHints.HideButtonHint(hand, (ISteamVR_Action_In_Source) hand.uiInteractAction);
      this.currentHand = (Hand) null;
    }

    protected virtual void HandHoverUpdate(Hand hand)
    {
      if (!((SteamVR_Action) hand.uiInteractAction != (SteamVR_Action) null) || !hand.uiInteractAction.GetStateDown(hand.handType))
        return;
      InputModule.instance.Submit(this.gameObject);
      ControllerButtonHints.HideButtonHint(hand, (ISteamVR_Action_In_Source) hand.uiInteractAction);
    }

    protected virtual void OnButtonClick() => this.onHandClick.Invoke(this.currentHand);
  }
}
