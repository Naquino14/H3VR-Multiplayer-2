// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Skybox
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Skybox : MonoBehaviour
  {
    public Texture front;
    public Texture back;
    public Texture left;
    public Texture right;
    public Texture top;
    public Texture bottom;
    public SteamVR_Skybox.CellSize StereoCellSize = SteamVR_Skybox.CellSize.x32;
    public float StereoIpdMm = 64f;

    public void SetTextureByIndex(int i, Texture t)
    {
      switch (i)
      {
        case 0:
          this.front = t;
          break;
        case 1:
          this.back = t;
          break;
        case 2:
          this.left = t;
          break;
        case 3:
          this.right = t;
          break;
        case 4:
          this.top = t;
          break;
        case 5:
          this.bottom = t;
          break;
      }
    }

    public Texture GetTextureByIndex(int i)
    {
      switch (i)
      {
        case 0:
          return this.front;
        case 1:
          return this.back;
        case 2:
          return this.left;
        case 3:
          return this.right;
        case 4:
          return this.top;
        case 5:
          return this.bottom;
        default:
          return (Texture) null;
      }
    }

    public static void SetOverride(
      Texture front = null,
      Texture back = null,
      Texture left = null,
      Texture right = null,
      Texture top = null,
      Texture bottom = null)
    {
      CVRCompositor compositor = OpenVR.Compositor;
      if (compositor == null)
        return;
      Texture[] textureArray = new Texture[6]
      {
        front,
        back,
        left,
        right,
        top,
        bottom
      };
      Texture_t[] pTextures = new Texture_t[6];
      for (int index = 0; index < 6; ++index)
      {
        pTextures[index].handle = !((UnityEngine.Object) textureArray[index] != (UnityEngine.Object) null) ? IntPtr.Zero : textureArray[index].GetNativeTexturePtr();
        pTextures[index].eType = SteamVR.instance.textureType;
        pTextures[index].eColorSpace = EColorSpace.Auto;
      }
      EVRCompositorError evrCompositorError = compositor.SetSkyboxOverride(pTextures);
      if (evrCompositorError == EVRCompositorError.None)
        return;
      Debug.LogError((object) ("<b>[SteamVR]</b> Failed to set skybox override with error: " + (object) evrCompositorError));
      if (evrCompositorError == EVRCompositorError.TextureIsOnWrongDevice)
      {
        Debug.Log((object) "<b>[SteamVR]</b> Set your graphics driver to use the same video card as the headset is plugged into for Unity.");
      }
      else
      {
        if (evrCompositorError != EVRCompositorError.TextureUsesUnsupportedFormat)
          return;
        Debug.Log((object) "<b>[SteamVR]</b> Ensure skybox textures are not compressed and have no mipmaps.");
      }
    }

    public static void ClearOverride() => OpenVR.Compositor?.ClearSkyboxOverride();

    private void OnEnable() => SteamVR_Skybox.SetOverride(this.front, this.back, this.left, this.right, this.top, this.bottom);

    private void OnDisable() => SteamVR_Skybox.ClearOverride();

    public enum CellSize
    {
      x1024,
      x64,
      x32,
      x16,
      x8,
    }
  }
}
