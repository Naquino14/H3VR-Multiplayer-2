// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.HapticRack
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
  [RequireComponent(typeof (Interactable))]
  public class HapticRack : MonoBehaviour
  {
    [Tooltip("The linear mapping driving the haptic rack")]
    public LinearMapping linearMapping;
    [Tooltip("The number of haptic pulses evenly distributed along the mapping")]
    public int teethCount = 128;
    [Tooltip("Minimum duration of the haptic pulse")]
    public int minimumPulseDuration = 500;
    [Tooltip("Maximum duration of the haptic pulse")]
    public int maximumPulseDuration = 900;
    [Tooltip("This event is triggered every time a haptic pulse is made")]
    public UnityEvent onPulse;
    private Hand hand;
    private int previousToothIndex = -1;

    private void Awake()
    {
      if (!((Object) this.linearMapping == (Object) null))
        return;
      this.linearMapping = this.GetComponent<LinearMapping>();
    }

    private void OnHandHoverBegin(Hand hand) => this.hand = hand;

    private void OnHandHoverEnd(Hand hand) => this.hand = (Hand) null;

    private void Update()
    {
      int num = Mathf.RoundToInt((float) ((double) this.linearMapping.value * (double) this.teethCount - 0.5));
      if (num == this.previousToothIndex)
        return;
      this.Pulse();
      this.previousToothIndex = num;
    }

    private void Pulse()
    {
      if (!(bool) (Object) this.hand || !this.hand.isActive || this.hand.GetBestGrabbingType() == GrabTypes.None)
        return;
      this.hand.TriggerHapticPulse((ushort) Random.Range(this.minimumPulseDuration, this.maximumPulseDuration + 1));
      this.onPulse.Invoke();
    }
  }
}
