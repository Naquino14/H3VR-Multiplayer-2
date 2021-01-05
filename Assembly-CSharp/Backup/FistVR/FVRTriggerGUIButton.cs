// Decompiled with JetBrains decompiler
// Type: FistVR.FVRTriggerGUIButton
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class FVRTriggerGUIButton : FVRInteractiveObject
  {
    private Button button;
    private float coolDown;
    public float pressCooldown = 0.5f;
    public AudioClip PressedSound;
    public AudioSource AudioSource;
    public float volume;

    private new void Awake() => this.button = this.GetComponent<Button>();

    public void Update()
    {
      if ((double) this.coolDown <= 0.0)
        return;
      this.coolDown -= Time.deltaTime;
    }

    public override void Poke(FVRViveHand hand)
    {
      if ((double) this.coolDown > 0.0)
        return;
      base.Poke(hand);
      this.button.onClick.Invoke();
      if ((Object) this.AudioSource != (Object) null)
        this.AudioSource.PlayOneShot(this.PressedSound, this.volume);
      this.coolDown = Mathf.Max(this.pressCooldown, 0.5f);
    }
  }
}
