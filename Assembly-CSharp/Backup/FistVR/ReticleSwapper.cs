// Decompiled with JetBrains decompiler
// Type: FistVR.ReticleSwapper
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class ReticleSwapper : FVRInteractiveObject
  {
    public Text ReadoutText;
    public Renderer ReticleRenderer;
    public Texture2D[] Reticles;
    private int m_curReticle;
    public Transform Switch;
    public Vector3[] Eulers;
    public AudioSource Aud;
    public AudioClip Clip;

    private void ReticleForward()
    {
      ++this.m_curReticle;
      if (this.m_curReticle >= this.Reticles.Length)
        this.m_curReticle = 0;
      this.ReticleRenderer.material.SetTexture("_MainTex", (Texture) this.Reticles[this.m_curReticle]);
      this.ReadoutText.text = "Reticle " + (object) (this.m_curReticle + 1);
      if ((Object) this.Switch != (Object) null)
        this.Switch.localEulerAngles = this.Eulers[this.m_curReticle];
      if (!((Object) this.Aud != (Object) null))
        return;
      this.Aud.PlayOneShot(this.Clip, 0.5f);
    }

    private void ReticleBack()
    {
      --this.m_curReticle;
      if (this.m_curReticle < 0)
        this.m_curReticle = this.Reticles.Length - 1;
      this.ReticleRenderer.material.SetTexture("_MainTex", (Texture) this.Reticles[this.m_curReticle]);
      this.ReadoutText.text = "Reticle " + (object) (this.m_curReticle + 1);
      if ((Object) this.Switch != (Object) null)
        this.Switch.localEulerAngles = this.Eulers[this.m_curReticle];
      if (!((Object) this.Aud != (Object) null))
        return;
      this.Aud.PlayOneShot(this.Clip, 0.5f);
    }

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      this.ReticleForward();
    }
  }
}
