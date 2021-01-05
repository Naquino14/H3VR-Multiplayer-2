// Decompiled with JetBrains decompiler
// Type: FistVR.PTargetTesting
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class PTargetTesting : MonoBehaviour
  {
    public PTargetRailJoint RJ;
    public PTarget Targ;
    public AudioSource Aud;
    public PTargetProfile[] Profile_Strong;
    public PTargetProfile[] Profile_Weak;

    public void GoTo(int i)
    {
      this.Aud.PlayOneShot(this.Aud.clip, 0.25f);
      this.RJ.GoToDistance((float) i);
    }

    public void ResetStrong(int i)
    {
      this.Aud.PlayOneShot(this.Aud.clip, 0.25f);
      this.Targ.ResetTarget(this.Profile_Strong[i]);
    }

    public void ResetWeak(int i)
    {
      this.Aud.PlayOneShot(this.Aud.clip, 0.25f);
      this.Targ.ResetTarget(this.Profile_Weak[i]);
    }

    public void ResetNoShatter(int i)
    {
      this.Aud.PlayOneShot(this.Aud.clip, 0.25f);
      this.Targ.ResetTarget(this.Profile_Strong[i], decalsOnly: true);
    }
  }
}
