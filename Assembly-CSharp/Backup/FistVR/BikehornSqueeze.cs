// Decompiled with JetBrains decompiler
// Type: FistVR.BikehornSqueeze
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class BikehornSqueeze : FVRInteractiveObject
  {
    public Renderer Horn_Unsqueezed;
    public Renderer Horn_Squeezed;
    public Transform Spot;
    public AudioEvent AudEvent_Squeak;

    protected override void Start()
    {
      base.Start();
      this.Horn_Unsqueezed.enabled = true;
      this.Horn_Squeezed.enabled = false;
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Squeak, this.Spot.position);
      GM.CurrentSceneSettings.OnPerceiveableSound(60f, 24f, this.transform.position, GM.CurrentPlayerBody.GetPlayerIFF());
      this.Horn_Unsqueezed.enabled = false;
      this.Horn_Squeezed.enabled = true;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      this.Horn_Unsqueezed.enabled = true;
      this.Horn_Squeezed.enabled = false;
    }
  }
}
