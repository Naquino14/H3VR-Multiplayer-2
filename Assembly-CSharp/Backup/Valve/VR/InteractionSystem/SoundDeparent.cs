// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.SoundDeparent
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class SoundDeparent : MonoBehaviour
  {
    public bool destroyAfterPlayOnce = true;
    private AudioSource thisAudioSource;

    private void Awake() => this.thisAudioSource = this.GetComponent<AudioSource>();

    private void Start()
    {
      this.gameObject.transform.parent = (Transform) null;
      if (!this.destroyAfterPlayOnce)
        return;
      Object.Destroy((Object) this.gameObject, this.thisAudioSource.clip.length);
    }
  }
}
