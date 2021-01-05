// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Overlay
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Overlay : MonoBehaviour
  {
    public Texture texture;
    public bool curved = true;
    public bool antialias = true;
    public bool highquality = true;
    [Tooltip("Size of overlay view.")]
    public float scale = 3f;
    [Tooltip("Distance from surface.")]
    public float distance = 1.25f;
    [Tooltip("Opacity")]
    [Range(0.0f, 1f)]
    public float alpha = 1f;
    public Vector4 uvOffset = new Vector4(0.0f, 0.0f, 1f, 1f);
    public Vector2 mouseScale = new Vector2(1f, 1f);
    public Vector2 curvedRange = new Vector2(1f, 2f);
    public VROverlayInputMethod inputMethod;
    private ulong handle;

    public static SteamVR_Overlay instance { get; private set; }

    public static string key => "unity:" + Application.companyName + "." + Application.productName;

    private void OnEnable()
    {
      CVROverlay overlay1 = OpenVR.Overlay;
      if (overlay1 != null)
      {
        EVROverlayError overlay2 = overlay1.CreateOverlay(SteamVR_Overlay.key, this.gameObject.name, ref this.handle);
        if (overlay2 != EVROverlayError.None)
        {
          Debug.Log((object) ("<b>[SteamVR]</b> " + overlay1.GetOverlayErrorNameFromEnum(overlay2)));
          this.enabled = false;
          return;
        }
      }
      SteamVR_Overlay.instance = this;
    }

    private void OnDisable()
    {
      if (this.handle != 0UL)
      {
        CVROverlay overlay = OpenVR.Overlay;
        if (overlay != null)
        {
          int num = (int) overlay.DestroyOverlay(this.handle);
        }
        this.handle = 0UL;
      }
      SteamVR_Overlay.instance = (SteamVR_Overlay) null;
    }

    public void UpdateOverlay()
    {
      CVROverlay overlay = OpenVR.Overlay;
      if (overlay == null)
        return;
      if ((Object) this.texture != (Object) null)
      {
        switch (overlay.ShowOverlay(this.handle))
        {
          case EVROverlayError.UnknownOverlay:
          case EVROverlayError.InvalidHandle:
            if (overlay.FindOverlay(SteamVR_Overlay.key, ref this.handle) != EVROverlayError.None)
              return;
            break;
        }
        int num1 = (int) overlay.SetOverlayTexture(this.handle, ref new Texture_t()
        {
          handle = this.texture.GetNativeTexturePtr(),
          eType = SteamVR.instance.textureType,
          eColorSpace = EColorSpace.Auto
        });
        int num2 = (int) overlay.SetOverlayAlpha(this.handle, this.alpha);
        int num3 = (int) overlay.SetOverlayWidthInMeters(this.handle, this.scale);
        int num4 = (int) overlay.SetOverlayAutoCurveDistanceRangeInMeters(this.handle, this.curvedRange.x, this.curvedRange.y);
        int num5 = (int) overlay.SetOverlayTextureBounds(this.handle, ref new VRTextureBounds_t()
        {
          uMin = this.uvOffset.x * this.uvOffset.z,
          vMin = (1f + this.uvOffset.y) * this.uvOffset.w,
          uMax = (1f + this.uvOffset.x) * this.uvOffset.z,
          vMax = this.uvOffset.y * this.uvOffset.w
        });
        int num6 = (int) overlay.SetOverlayMouseScale(this.handle, ref new HmdVector2_t()
        {
          v0 = this.mouseScale.x,
          v1 = this.mouseScale.y
        });
        SteamVR_Camera steamVrCamera = SteamVR_Render.Top();
        if ((Object) steamVrCamera != (Object) null && (Object) steamVrCamera.origin != (Object) null)
        {
          SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(steamVrCamera.origin, this.transform);
          rigidTransform.pos.x /= steamVrCamera.origin.localScale.x;
          rigidTransform.pos.y /= steamVrCamera.origin.localScale.y;
          rigidTransform.pos.z /= steamVrCamera.origin.localScale.z;
          rigidTransform.pos.z += this.distance;
          HmdMatrix34_t hmdMatrix34 = rigidTransform.ToHmdMatrix34();
          int num7 = (int) overlay.SetOverlayTransformAbsolute(this.handle, SteamVR.settings.trackingSpace, ref hmdMatrix34);
        }
        int num8 = (int) overlay.SetOverlayInputMethod(this.handle, this.inputMethod);
        if (this.curved || this.antialias)
          this.highquality = true;
        if (this.highquality)
        {
          int num7 = (int) overlay.SetHighQualityOverlay(this.handle);
          int num9 = (int) overlay.SetOverlayFlag(this.handle, VROverlayFlags.Curved, this.curved);
          int num10 = (int) overlay.SetOverlayFlag(this.handle, VROverlayFlags.RGSS4X, this.antialias);
        }
        else
        {
          if ((long) overlay.GetHighQualityOverlay() != (long) this.handle)
            return;
          int num7 = (int) overlay.SetHighQualityOverlay(0UL);
        }
      }
      else
      {
        int num = (int) overlay.HideOverlay(this.handle);
      }
    }

    public bool PollNextEvent(ref VREvent_t pEvent)
    {
      CVROverlay overlay = OpenVR.Overlay;
      if (overlay == null)
        return false;
      uint uncbVREvent = (uint) Marshal.SizeOf(typeof (VREvent_t));
      return overlay.PollNextOverlayEvent(this.handle, ref pEvent, uncbVREvent);
    }

    public bool ComputeIntersection(
      Vector3 source,
      Vector3 direction,
      ref SteamVR_Overlay.IntersectionResults results)
    {
      CVROverlay overlay = OpenVR.Overlay;
      if (overlay == null)
        return false;
      VROverlayIntersectionParams_t pParams = new VROverlayIntersectionParams_t();
      pParams.eOrigin = SteamVR.settings.trackingSpace;
      pParams.vSource.v0 = source.x;
      pParams.vSource.v1 = source.y;
      pParams.vSource.v2 = -source.z;
      pParams.vDirection.v0 = direction.x;
      pParams.vDirection.v1 = direction.y;
      pParams.vDirection.v2 = -direction.z;
      VROverlayIntersectionResults_t pResults = new VROverlayIntersectionResults_t();
      if (!overlay.ComputeOverlayIntersection(this.handle, ref pParams, ref pResults))
        return false;
      results.point = new Vector3(pResults.vPoint.v0, pResults.vPoint.v1, -pResults.vPoint.v2);
      results.normal = new Vector3(pResults.vNormal.v0, pResults.vNormal.v1, -pResults.vNormal.v2);
      results.UVs = new Vector2(pResults.vUVs.v0, pResults.vUVs.v1);
      results.distance = pResults.fDistance;
      return true;
    }

    public struct IntersectionResults
    {
      public Vector3 point;
      public Vector3 normal;
      public Vector2 UVs;
      public float distance;
    }
  }
}
