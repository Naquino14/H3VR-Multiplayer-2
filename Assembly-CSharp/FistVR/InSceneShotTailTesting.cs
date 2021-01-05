// Decompiled with JetBrains decompiler
// Type: FistVR.InSceneShotTailTesting
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class InSceneShotTailTesting : MonoBehaviour
  {
    public FVRFireArm FireArmToTest;
    public AudioEvent[] ShotSets;
    public FVRSoundEnvironment[] Environments;
    public Text DebugDisplay;
    public AudioSource Aud;

    public void SetShotSet(int i)
    {
    }

    private void Update()
    {
    }

    public void SetTailSet(int i)
    {
      GM.CurrentSceneSettings.DefaultSoundEnvironment = this.Environments[i];
      this.Aud.PlayOneShot(this.Aud.clip, 0.25f);
    }
  }
}
