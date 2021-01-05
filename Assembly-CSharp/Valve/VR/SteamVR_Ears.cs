// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Ears
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR
{
  [RequireComponent(typeof (AudioListener))]
  public class SteamVR_Ears : MonoBehaviour
  {
    public SteamVR_Camera vrcam;
    private bool usingSpeakers;
    private Quaternion offset;

    private void OnNewPosesApplied()
    {
      Transform origin = this.vrcam.origin;
      this.transform.rotation = (!((Object) origin != (Object) null) ? Quaternion.identity : origin.rotation) * this.offset;
    }

    private void OnEnable()
    {
      this.usingSpeakers = false;
      CVRSettings settings = OpenVR.Settings;
      if (settings != null)
      {
        EVRSettingsError peError = EVRSettingsError.None;
        if (settings.GetBool("steamvr", "usingSpeakers", ref peError))
        {
          this.usingSpeakers = true;
          this.offset = Quaternion.Euler(0.0f, settings.GetFloat("steamvr", "speakersForwardYawOffsetDegrees", ref peError), 0.0f);
        }
      }
      if (!this.usingSpeakers)
        return;
      SteamVR_Events.NewPosesApplied.Listen(new UnityAction(this.OnNewPosesApplied));
    }

    private void OnDisable()
    {
      if (!this.usingSpeakers)
        return;
      SteamVR_Events.NewPosesApplied.Remove(new UnityAction(this.OnNewPosesApplied));
    }
  }
}
