// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.ButtonEffect
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class ButtonEffect : MonoBehaviour
  {
    public void OnButtonDown(Hand fromHand)
    {
      this.ColorSelf(Color.cyan);
      fromHand.TriggerHapticPulse((ushort) 1000);
    }

    public void OnButtonUp(Hand fromHand) => this.ColorSelf(Color.white);

    private void ColorSelf(Color newColor)
    {
      foreach (Renderer componentsInChild in this.GetComponentsInChildren<Renderer>())
        componentsInChild.material.color = newColor;
    }
  }
}
