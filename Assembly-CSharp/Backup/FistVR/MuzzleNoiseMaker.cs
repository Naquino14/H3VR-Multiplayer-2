// Decompiled with JetBrains decompiler
// Type: FistVR.MuzzleNoiseMaker
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FistVR
{
  public class MuzzleNoiseMaker : MuzzleDevice
  {
    public bool PlaysTail = true;
    public AudioEvent AudEvent_SoundToPlay;

    public override void OnShot(FVRFireArm f, FVRTailSoundClass tailClass)
    {
      FVRFirearmAudioSet audioClipSet = f.AudioClipSet;
      if (this.PlaysTail)
      {
        f.PlayShotTail(FVRTailSoundClass.ExplosionBigDistant, FVRSoundEnvironment.InsideWarehouse);
        GM.CurrentSceneSettings.OnPerceiveableSound(200f, 80f, this.transform.position, GM.CurrentPlayerBody.GetPlayerIFF());
      }
      else
        f.PlayAudioAsHandling(this.AudEvent_SoundToPlay, this.transform.position);
      base.OnShot(f, tailClass);
    }
  }
}
