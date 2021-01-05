// Decompiled with JetBrains decompiler
// Type: Valve.VR.Extras.SteamVR_TestTrackedCamera
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.Extras
{
  public class SteamVR_TestTrackedCamera : MonoBehaviour
  {
    public Material material;
    public Transform target;
    public bool undistorted = true;
    public bool cropped = true;

    private void OnEnable()
    {
      SteamVR_TrackedCamera.VideoStreamTexture videoStreamTexture = SteamVR_TrackedCamera.Source(this.undistorted);
      long num = (long) videoStreamTexture.Acquire();
      if (videoStreamTexture.hasCamera)
        return;
      this.enabled = false;
    }

    private void OnDisable()
    {
      this.material.mainTexture = (Texture) null;
      long num = (long) SteamVR_TrackedCamera.Source(this.undistorted).Release();
    }

    private void Update()
    {
      SteamVR_TrackedCamera.VideoStreamTexture videoStreamTexture = SteamVR_TrackedCamera.Source(this.undistorted);
      Texture2D texture = videoStreamTexture.texture;
      if ((Object) texture == (Object) null)
        return;
      this.material.mainTexture = (Texture) texture;
      float num = (float) texture.width / (float) texture.height;
      if (this.cropped)
      {
        VRTextureBounds_t frameBounds = videoStreamTexture.frameBounds;
        this.material.mainTextureOffset = new Vector2(frameBounds.uMin, frameBounds.vMin);
        float x = frameBounds.uMax - frameBounds.uMin;
        float y = frameBounds.vMax - frameBounds.vMin;
        this.material.mainTextureScale = new Vector2(x, y);
        num *= Mathf.Abs(x / y);
      }
      else
      {
        this.material.mainTextureOffset = Vector2.zero;
        this.material.mainTextureScale = new Vector2(1f, -1f);
      }
      this.target.localScale = new Vector3(1f, 1f / num, 1f);
      if (!videoStreamTexture.hasTracking)
        return;
      SteamVR_Utils.RigidTransform transform = videoStreamTexture.transform;
      this.target.localPosition = transform.pos;
      this.target.localRotation = transform.rot;
    }
  }
}
