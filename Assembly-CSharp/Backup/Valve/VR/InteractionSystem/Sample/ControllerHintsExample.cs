// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.ControllerHintsExample
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class ControllerHintsExample : MonoBehaviour
  {
    private Coroutine buttonHintCoroutine;
    private Coroutine textHintCoroutine;

    public void ShowButtonHints(Hand hand)
    {
      if (this.buttonHintCoroutine != null)
        this.StopCoroutine(this.buttonHintCoroutine);
      this.buttonHintCoroutine = this.StartCoroutine(this.TestButtonHints(hand));
    }

    public void ShowTextHints(Hand hand)
    {
      if (this.textHintCoroutine != null)
        this.StopCoroutine(this.textHintCoroutine);
      this.textHintCoroutine = this.StartCoroutine(this.TestTextHints(hand));
    }

    public void DisableHints()
    {
      if (this.buttonHintCoroutine != null)
      {
        this.StopCoroutine(this.buttonHintCoroutine);
        this.buttonHintCoroutine = (Coroutine) null;
      }
      if (this.textHintCoroutine != null)
      {
        this.StopCoroutine(this.textHintCoroutine);
        this.textHintCoroutine = (Coroutine) null;
      }
      foreach (Hand hand in Player.instance.hands)
      {
        ControllerButtonHints.HideAllButtonHints(hand);
        ControllerButtonHints.HideAllTextHints(hand);
      }
    }

    [DebuggerHidden]
    private IEnumerator TestButtonHints(Hand hand) => (IEnumerator) new ControllerHintsExample.\u003CTestButtonHints\u003Ec__Iterator0()
    {
      hand = hand
    };

    [DebuggerHidden]
    private IEnumerator TestTextHints(Hand hand) => (IEnumerator) new ControllerHintsExample.\u003CTestTextHints\u003Ec__Iterator1()
    {
      hand = hand
    };
  }
}
