// Decompiled with JetBrains decompiler
// Type: LIV.SDK.Unity.SharedTextureProtocol
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace LIV.SDK.Unity
{
  [AddComponentMenu("LIV/SharedTextureProtocol")]
  public class SharedTextureProtocol : MonoBehaviour
  {
    [DllImport("LIV_MR")]
    private static extern IntPtr GetRenderEventFunc();

    [DllImport("LIV_MR", EntryPoint = "LivCaptureIsActive")]
    [return: MarshalAs(UnmanagedType.U1)]
    private static extern bool GetIsCaptureActive();

    [DllImport("LIV_MR", EntryPoint = "LivCaptureWidth")]
    private static extern int GetTextureWidth();

    [DllImport("LIV_MR", EntryPoint = "LivCaptureHeight")]
    private static extern int GetTextureHeight();

    [DllImport("LIV_MR", EntryPoint = "LivCaptureSetTextureFromUnity")]
    private static extern void SetTexture(IntPtr texture);

    public static bool IsActive => SharedTextureProtocol.GetIsCaptureActive();

    public static int TextureWidth => SharedTextureProtocol.GetTextureWidth();

    public static int TextureHeight => SharedTextureProtocol.GetTextureHeight();

    public static void SetOutputTexture(Texture texture)
    {
      if (!SharedTextureProtocol.IsActive)
        return;
      SharedTextureProtocol.SetTexture(texture.GetNativeTexturePtr());
    }

    private void OnEnable() => this.StartCoroutine(this.CallPluginAtEndOfFrames());

    [DebuggerHidden]
    private IEnumerator CallPluginAtEndOfFrames() => (IEnumerator) new SharedTextureProtocol.\u003CCallPluginAtEndOfFrames\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    private void OnDisable() => SharedTextureProtocol.SetTexture(IntPtr.Zero);
  }
}
