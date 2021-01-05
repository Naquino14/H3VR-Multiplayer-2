// Decompiled with JetBrains decompiler
// Type: FistVR.wwBotDependantSingleCallout
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwBotDependantSingleCallout : MonoBehaviour
  {
    public GameObject Bot;
    public AudioSource Aud;
    private bool m_hasPlayed;

    private void OnTriggerEnter()
    {
      if (this.m_hasPlayed)
        return;
      this.m_hasPlayed = true;
      this.Aud.Play();
    }
  }
}
