// Decompiled with JetBrains decompiler
// Type: LIV.SDK.Unity.SteamVRCompatibility
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace LIV.SDK.Unity
{
  internal static class SteamVRCompatibility
  {
    public static bool IsAvailable = SteamVRCompatibility.FindSteamVRAsset();
    public static Type SteamVRCamera;
    public static Type SteamVRExternalCamera;
    public static Type SteamVRFade;

    private static bool FindSteamVRAsset()
    {
      if (SteamVRCompatibility.SteamVRCamera == null)
        SteamVRCompatibility.SteamVRCamera = Type.GetType("SteamVR_Camera", false);
      if (SteamVRCompatibility.SteamVRCamera == null)
        SteamVRCompatibility.SteamVRCamera = Type.GetType("Valve.VR.SteamVR_Camera", false);
      if (SteamVRCompatibility.SteamVRExternalCamera == null)
        SteamVRCompatibility.SteamVRExternalCamera = Type.GetType("SteamVR_ExternalCamera", false);
      if (SteamVRCompatibility.SteamVRExternalCamera == null)
        SteamVRCompatibility.SteamVRExternalCamera = Type.GetType("Valve.VR.SteamVR_ExternalCamera", false);
      if (SteamVRCompatibility.SteamVRFade == null)
        SteamVRCompatibility.SteamVRFade = Type.GetType("SteamVR_Fade", false);
      if (SteamVRCompatibility.SteamVRFade == null)
        SteamVRCompatibility.SteamVRFade = Type.GetType("Valve.VR.SteamVR_Fade", false);
      return SteamVRCompatibility.SteamVRCamera != null && SteamVRCompatibility.SteamVRExternalCamera != null && SteamVRCompatibility.SteamVRFade != null;
    }
  }
}
