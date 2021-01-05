using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace LIV.SDK.Unity
{
	[AddComponentMenu("LIV/SharedTextureProtocol")]
	public class SharedTextureProtocol : MonoBehaviour
	{
		public static bool IsActive => GetIsCaptureActive();

		public static int TextureWidth => GetTextureWidth();

		public static int TextureHeight => GetTextureHeight();

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

		public static void SetOutputTexture(Texture texture)
		{
			if (IsActive)
			{
				SetTexture(texture.GetNativeTexturePtr());
			}
		}

		private void OnEnable()
		{
			StartCoroutine(CallPluginAtEndOfFrames());
		}

		private IEnumerator CallPluginAtEndOfFrames()
		{
			while (base.enabled)
			{
				yield return new WaitForEndOfFrame();
				GL.IssuePluginEvent(GetRenderEventFunc(), 1);
			}
		}

		private void OnDisable()
		{
			SetTexture(IntPtr.Zero);
		}
	}
}
