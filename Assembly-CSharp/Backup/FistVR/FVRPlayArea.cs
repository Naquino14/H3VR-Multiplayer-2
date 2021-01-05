// Decompiled with JetBrains decompiler
// Type: FistVR.FVRPlayArea
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Valve.VR;

namespace FistVR
{
  public static class FVRPlayArea
  {
    public static bool TryGetPlayArea(
      out Vector3 c1,
      out Vector3 c2,
      out Vector3 c3,
      out Vector3 c4)
    {
      CVRChaperone chaperone = OpenVR.Chaperone;
      c1 = c2 = c3 = c4 = Vector3.zero;
      if (chaperone == null || chaperone.GetCalibrationState() != ChaperoneCalibrationState.OK)
        return false;
      HmdQuad_t rect = new HmdQuad_t();
      if (!chaperone.GetPlayAreaRect(ref rect))
        return false;
      c1 = new Vector3(rect.vCorners0.v0, rect.vCorners0.v1, rect.vCorners0.v2);
      c2 = new Vector3(rect.vCorners1.v0, rect.vCorners1.v1, rect.vCorners1.v2);
      c3 = new Vector3(rect.vCorners2.v0, rect.vCorners2.v1, rect.vCorners2.v2);
      c4 = new Vector3(rect.vCorners3.v0, rect.vCorners3.v1, rect.vCorners3.v2);
      return true;
    }
  }
}
