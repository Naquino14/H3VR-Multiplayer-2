// Decompiled with JetBrains decompiler
// Type: FistVR.PokeBuzzer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class PokeBuzzer : FVRInteractiveObject
  {
    public Material Mat_Unpushed;
    public Material Mat_Pushed;
    private Renderer m_rend;
    private AudioSource m_aud;
    private bool m_hasBeenPressed;
    public GameObject Target;
    public string Method;

    protected override void Awake()
    {
      base.Awake();
      this.m_rend = this.GetComponent<Renderer>();
      this.m_aud = this.GetComponent<AudioSource>();
    }

    public override void Poke(FVRViveHand hand)
    {
      base.Poke(hand);
      if (this.m_hasBeenPressed)
        return;
      this.m_hasBeenPressed = true;
      this.Press();
    }

    private void Press()
    {
      this.m_rend.material = this.Mat_Pushed;
      if ((Object) this.Target != (Object) null)
        this.Target.SendMessage(this.Method);
      this.m_aud.PlayOneShot(this.m_aud.clip, 0.5f);
    }

    private void Reset()
    {
      this.m_rend.material = this.Mat_Unpushed;
      this.m_hasBeenPressed = false;
    }
  }
}
