// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.ProceduralHats
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class ProceduralHats : MonoBehaviour
  {
    public GameObject[] hats;
    public float hatSwitchTime;

    private void Start() => this.SwitchToHat(0);

    private void OnEnable() => this.StartCoroutine(this.HatSwitcher());

    [DebuggerHidden]
    private IEnumerator HatSwitcher() => (IEnumerator) new ProceduralHats.\u003CHatSwitcher\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    private void ChooseHat() => this.SwitchToHat(Random.Range(0, this.hats.Length));

    private void SwitchToHat(int hat)
    {
      for (int index = 0; index < this.hats.Length; ++index)
        this.hats[index].SetActive(hat == index);
    }
  }
}
