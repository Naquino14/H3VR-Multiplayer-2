// Decompiled with JetBrains decompiler
// Type: FistVR.PlayAudioEventOnAwake
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class PlayAudioEventOnAwake : MonoBehaviour
  {
    public AudioEvent AudioEvent;
    public bool IsDelayed;
    public FVRPooledAudioType Type = FVRPooledAudioType.GenericLongRange;

    private void Start()
    {
      if (this.IsDelayed)
        SM.PlayCoreSoundDelayed(this.Type, this.AudioEvent, this.transform.position, Vector3.Distance(GM.CurrentPlayerRoot.position, this.transform.position) / 343f);
      else
        SM.PlayCoreSound(this.Type, this.AudioEvent, this.transform.position);
    }
  }
}
