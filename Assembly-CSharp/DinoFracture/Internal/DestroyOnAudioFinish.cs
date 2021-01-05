// Decompiled with JetBrains decompiler
// Type: DinoFracture.Internal.DestroyOnAudioFinish
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace DinoFracture.Internal
{
  [RequireComponent(typeof (AudioSource))]
  public class DestroyOnAudioFinish : MonoBehaviour
  {
    private AudioSource _source;

    private void Start()
    {
      this._source = this.GetComponent<AudioSource>();
      this._source.Play();
    }

    private void Update()
    {
      if (this._source.isPlaying)
        return;
      Object.Destroy((Object) this.gameObject);
    }
  }
}
