// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_SphericalProjection
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR
{
  [ExecuteInEditMode]
  public class SteamVR_SphericalProjection : MonoBehaviour
  {
    private static Material material;

    public void Set(
      Vector3 N,
      float phi0,
      float phi1,
      float theta0,
      float theta1,
      Vector3 uAxis,
      Vector3 uOrigin,
      float uScale,
      Vector3 vAxis,
      Vector3 vOrigin,
      float vScale)
    {
      if ((UnityEngine.Object) SteamVR_SphericalProjection.material == (UnityEngine.Object) null)
        SteamVR_SphericalProjection.material = new Material(Shader.Find("Custom/SteamVR_SphericalProjection"));
      SteamVR_SphericalProjection.material.SetVector("_N", new Vector4(N.x, N.y, N.z));
      SteamVR_SphericalProjection.material.SetFloat("_Phi0", phi0 * ((float) Math.PI / 180f));
      SteamVR_SphericalProjection.material.SetFloat("_Phi1", phi1 * ((float) Math.PI / 180f));
      SteamVR_SphericalProjection.material.SetFloat("_Theta0", (float) ((double) theta0 * (Math.PI / 180.0) + 1.57079637050629));
      SteamVR_SphericalProjection.material.SetFloat("_Theta1", (float) ((double) theta1 * (Math.PI / 180.0) + 1.57079637050629));
      SteamVR_SphericalProjection.material.SetVector("_UAxis", (Vector4) uAxis);
      SteamVR_SphericalProjection.material.SetVector("_VAxis", (Vector4) vAxis);
      SteamVR_SphericalProjection.material.SetVector("_UOrigin", (Vector4) uOrigin);
      SteamVR_SphericalProjection.material.SetVector("_VOrigin", (Vector4) vOrigin);
      SteamVR_SphericalProjection.material.SetFloat("_UScale", uScale);
      SteamVR_SphericalProjection.material.SetFloat("_VScale", vScale);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest) => Graphics.Blit((Texture) src, dest, SteamVR_SphericalProjection.material);
  }
}
