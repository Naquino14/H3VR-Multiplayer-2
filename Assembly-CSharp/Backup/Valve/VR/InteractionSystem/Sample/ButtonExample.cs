// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.ButtonExample
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem.Sample
{
  public class ButtonExample : MonoBehaviour
  {
    public HoverButton hoverButton;
    public GameObject prefab;

    private void Start() => this.hoverButton.onButtonDown.AddListener(new UnityAction<Hand>(this.OnButtonDown));

    private void OnButtonDown(Hand hand) => this.StartCoroutine(this.DoPlant());

    [DebuggerHidden]
    private IEnumerator DoPlant() => (IEnumerator) new ButtonExample.\u003CDoPlant\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
