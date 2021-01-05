using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class SteamVR_LoadLevel : MonoBehaviour
{
	private static SteamVR_LoadLevel _active;

	public string levelName;

	public string internalProcessPath;

	public string internalProcessArgs;

	public bool loadAdditive;

	public bool loadAsync = true;

	public Texture loadingScreen;

	public Texture progressBarEmpty;

	public Texture progressBarFull;

	public float loadingScreenWidthInMeters = 6f;

	public float progressBarWidthInMeters = 3f;

	public float loadingScreenDistance;

	public Transform loadingScreenTransform;

	public Transform progressBarTransform;

	public Texture front;

	public Texture back;

	public Texture left;

	public Texture right;

	public Texture top;

	public Texture bottom;

	public Color backgroundColor = Color.black;

	public bool showGrid;

	public float fadeOutTime = 0.5f;

	public float fadeInTime = 0.5f;

	public float postLoadSettleTime;

	public float loadingScreenFadeInTime = 1f;

	public float loadingScreenFadeOutTime = 0.25f;

	private float fadeRate = 1f;

	private float alpha;

	private AsyncOperation async;

	private RenderTexture renderTexture;

	private ulong loadingScreenOverlayHandle;

	private ulong progressBarOverlayHandle;

	public bool autoTriggerOnEnable;

	public static bool loading => _active != null;

	public static float progress => (!(_active != null) || _active.async == null) ? 0f : _active.async.progress;

	public static Texture progressTexture => (!(_active != null)) ? null : _active.renderTexture;

	private void OnEnable()
	{
		if (autoTriggerOnEnable)
		{
			Trigger();
		}
	}

	public void Trigger()
	{
		if (!loading && !string.IsNullOrEmpty(levelName))
		{
			StartCoroutine(LoadLevel());
		}
	}

	public static void Begin(string levelName, bool showGrid = false, float fadeOutTime = 0.5f, float r = 0f, float g = 0f, float b = 0f, float a = 1f)
	{
		SteamVR_LoadLevel steamVR_LoadLevel = new GameObject("loader").AddComponent<SteamVR_LoadLevel>();
		steamVR_LoadLevel.levelName = levelName;
		steamVR_LoadLevel.showGrid = showGrid;
		steamVR_LoadLevel.fadeOutTime = fadeOutTime;
		steamVR_LoadLevel.backgroundColor = new Color(r, g, b, a);
		steamVR_LoadLevel.Trigger();
	}

	private void OnGUI()
	{
		if (_active != this || !(progressBarEmpty != null) || !(progressBarFull != null))
		{
			return;
		}
		if (progressBarOverlayHandle == 0)
		{
			progressBarOverlayHandle = GetOverlayHandle("progressBar", (!(progressBarTransform != null)) ? base.transform : progressBarTransform, progressBarWidthInMeters);
		}
		if (progressBarOverlayHandle != 0)
		{
			float num = ((async == null) ? 0f : async.progress);
			int width = progressBarFull.width;
			int height = progressBarFull.height;
			if (renderTexture == null)
			{
				renderTexture = new RenderTexture(width, height, 0);
				renderTexture.Create();
			}
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = renderTexture;
			if (Event.current.type == EventType.Repaint)
			{
				GL.Clear(clearDepth: false, clearColor: true, Color.clear);
			}
			GUILayout.BeginArea(new Rect(0f, 0f, width, height));
			GUI.DrawTexture(new Rect(0f, 0f, width, height), progressBarEmpty);
			GUI.DrawTextureWithTexCoords(new Rect(0f, 0f, num * (float)width, height), progressBarFull, new Rect(0f, 0f, num, 1f));
			GUILayout.EndArea();
			RenderTexture.active = active;
			CVROverlay overlay = OpenVR.Overlay;
			if (overlay != null)
			{
				Texture_t pTexture = default(Texture_t);
				pTexture.handle = renderTexture.GetNativeTexturePtr();
				pTexture.eType = SteamVR.instance.textureType;
				pTexture.eColorSpace = EColorSpace.Auto;
				overlay.SetOverlayTexture(progressBarOverlayHandle, ref pTexture);
			}
		}
	}

	private void Update()
	{
		if (_active != this)
		{
			return;
		}
		alpha = Mathf.Clamp01(alpha + fadeRate * Time.deltaTime);
		CVROverlay overlay = OpenVR.Overlay;
		if (overlay != null)
		{
			if (loadingScreenOverlayHandle != 0)
			{
				overlay.SetOverlayAlpha(loadingScreenOverlayHandle, alpha);
			}
			if (progressBarOverlayHandle != 0)
			{
				overlay.SetOverlayAlpha(progressBarOverlayHandle, alpha);
			}
		}
	}

	private IEnumerator LoadLevel()
	{
		if (loadingScreen != null && loadingScreenDistance > 0f)
		{
			Transform transform = base.transform;
			if (Camera.main != null)
			{
				transform = Camera.main.transform;
			}
			Quaternion rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
			Vector3 position = transform.position + transform.rotation * new Vector3(0f, 0f, loadingScreenDistance);
			Transform transform2 = ((!(loadingScreenTransform != null)) ? base.transform : loadingScreenTransform);
			transform2.position = position;
			transform2.rotation = rotation;
		}
		_active = this;
		SteamVR_Events.Loading.Send(arg0: true);
		if (loadingScreenFadeInTime > 0f)
		{
			fadeRate = 1f / loadingScreenFadeInTime;
		}
		else
		{
			alpha = 1f;
		}
		CVROverlay overlay = OpenVR.Overlay;
		if (loadingScreen != null && overlay != null)
		{
			loadingScreenOverlayHandle = GetOverlayHandle("loadingScreen", (!(loadingScreenTransform != null)) ? base.transform : loadingScreenTransform, loadingScreenWidthInMeters);
			if (loadingScreenOverlayHandle != 0)
			{
				Texture_t pTexture = default(Texture_t);
				pTexture.handle = loadingScreen.GetNativeTexturePtr();
				pTexture.eType = SteamVR.instance.textureType;
				pTexture.eColorSpace = EColorSpace.Auto;
				overlay.SetOverlayTexture(loadingScreenOverlayHandle, ref pTexture);
			}
		}
		bool fadedForeground = false;
		SteamVR_Events.LoadingFadeOut.Send(fadeOutTime);
		CVRCompositor compositor2 = OpenVR.Compositor;
		if (compositor2 != null)
		{
			if (front != null)
			{
				SteamVR_Skybox.SetOverride(front, back, left, right, top, bottom);
				compositor2.FadeGrid(fadeOutTime, bFadeIn: true);
				yield return new WaitForSeconds(fadeOutTime);
			}
			else if (backgroundColor != Color.clear)
			{
				if (showGrid)
				{
					compositor2.FadeToColor(0f, backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a, bBackground: true);
					compositor2.FadeGrid(fadeOutTime, bFadeIn: true);
					yield return new WaitForSeconds(fadeOutTime);
				}
				else
				{
					compositor2.FadeToColor(fadeOutTime, backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a, bBackground: false);
					yield return new WaitForSeconds(fadeOutTime + 0.1f);
					compositor2.FadeGrid(0f, bFadeIn: true);
					fadedForeground = true;
				}
			}
		}
		SteamVR_Render.pauseRendering = true;
		while (alpha < 1f)
		{
			yield return null;
		}
		base.transform.parent = null;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (!string.IsNullOrEmpty(internalProcessPath))
		{
			UnityEngine.Debug.Log("Launching external application...");
			CVRApplications applications = OpenVR.Applications;
			if (applications == null)
			{
				UnityEngine.Debug.Log("Failed to get OpenVR.Applications interface!");
			}
			else
			{
				string currentDirectory = Directory.GetCurrentDirectory();
				string text = Path.Combine(currentDirectory, internalProcessPath);
				UnityEngine.Debug.Log("LaunchingInternalProcess");
				UnityEngine.Debug.Log("ExternalAppPath = " + internalProcessPath);
				UnityEngine.Debug.Log("FullPath = " + text);
				UnityEngine.Debug.Log("ExternalAppArgs = " + internalProcessArgs);
				UnityEngine.Debug.Log("WorkingDirectory = " + currentDirectory);
				EVRApplicationError eVRApplicationError = applications.LaunchInternalProcess(text, internalProcessArgs, currentDirectory);
				UnityEngine.Debug.Log("LaunchInternalProcessError: " + eVRApplicationError);
				Process.GetCurrentProcess().Kill();
			}
		}
		else
		{
			LoadSceneMode mode = (loadAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
			if (loadAsync)
			{
				Application.backgroundLoadingPriority = ThreadPriority.Low;
				async = SceneManager.LoadSceneAsync(levelName, mode);
				UnityEngine.Debug.Log("AsyncLoadLevelBegin at:" + Time.realtimeSinceStartup);
				while (!async.isDone)
				{
					yield return null;
				}
				UnityEngine.Debug.Log("AsyncLoadLevelComplete at:" + Time.realtimeSinceStartup);
			}
			else
			{
				SceneManager.LoadScene(levelName, mode);
			}
		}
		yield return null;
		GC.Collect();
		yield return null;
		yield return new WaitForSeconds(postLoadSettleTime);
		SteamVR_Render.pauseRendering = false;
		if (loadingScreenFadeOutTime > 0f)
		{
			fadeRate = -1f / loadingScreenFadeOutTime;
		}
		else
		{
			alpha = 0f;
		}
		SteamVR_Events.LoadingFadeIn.Send(fadeInTime);
		compositor2 = OpenVR.Compositor;
		if (compositor2 != null)
		{
			if (fadedForeground)
			{
				compositor2.FadeGrid(0f, bFadeIn: false);
				compositor2.FadeToColor(fadeInTime, 0f, 0f, 0f, 0f, bBackground: false);
				yield return new WaitForSeconds(fadeInTime);
			}
			else
			{
				compositor2.FadeGrid(fadeInTime, bFadeIn: false);
				yield return new WaitForSeconds(fadeInTime);
				if (front != null)
				{
					SteamVR_Skybox.ClearOverride();
				}
			}
		}
		while (alpha > 0f)
		{
			yield return null;
		}
		if (overlay != null)
		{
			if (progressBarOverlayHandle != 0)
			{
				overlay.HideOverlay(progressBarOverlayHandle);
			}
			if (loadingScreenOverlayHandle != 0)
			{
				overlay.HideOverlay(loadingScreenOverlayHandle);
			}
		}
		UnityEngine.Object.Destroy(base.gameObject);
		_active = null;
		SteamVR_Events.Loading.Send(arg0: false);
	}

	private ulong GetOverlayHandle(string overlayName, Transform transform, float widthInMeters = 1f)
	{
		ulong pOverlayHandle = 0uL;
		CVROverlay overlay = OpenVR.Overlay;
		if (overlay == null)
		{
			return pOverlayHandle;
		}
		string pchOverlayKey = SteamVR_Overlay.key + "." + overlayName;
		EVROverlayError eVROverlayError = overlay.FindOverlay(pchOverlayKey, ref pOverlayHandle);
		if (eVROverlayError != 0)
		{
			eVROverlayError = overlay.CreateOverlay(pchOverlayKey, overlayName, ref pOverlayHandle);
		}
		if (eVROverlayError == EVROverlayError.None)
		{
			overlay.ShowOverlay(pOverlayHandle);
			overlay.SetOverlayAlpha(pOverlayHandle, alpha);
			overlay.SetOverlayWidthInMeters(pOverlayHandle, widthInMeters);
			if (SteamVR.instance.textureType == ETextureType.DirectX)
			{
				VRTextureBounds_t pOverlayTextureBounds = default(VRTextureBounds_t);
				pOverlayTextureBounds.uMin = 0f;
				pOverlayTextureBounds.vMin = 1f;
				pOverlayTextureBounds.uMax = 1f;
				pOverlayTextureBounds.vMax = 0f;
				overlay.SetOverlayTextureBounds(pOverlayHandle, ref pOverlayTextureBounds);
			}
			SteamVR_Camera steamVR_Camera = ((loadingScreenDistance != 0f) ? null : SteamVR_Render.Top());
			if (steamVR_Camera != null && steamVR_Camera.origin != null)
			{
				SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(steamVR_Camera.origin, transform);
				rigidTransform.pos.x /= steamVR_Camera.origin.localScale.x;
				rigidTransform.pos.y /= steamVR_Camera.origin.localScale.y;
				rigidTransform.pos.z /= steamVR_Camera.origin.localScale.z;
				HmdMatrix34_t pmatTrackingOriginToOverlayTransform = rigidTransform.ToHmdMatrix34();
				overlay.SetOverlayTransformAbsolute(pOverlayHandle, SteamVR_Settings.instance.trackingSpace, ref pmatTrackingOriginToOverlayTransform);
			}
			else
			{
				HmdMatrix34_t pmatTrackingOriginToOverlayTransform2 = new SteamVR_Utils.RigidTransform(transform).ToHmdMatrix34();
				overlay.SetOverlayTransformAbsolute(pOverlayHandle, SteamVR_Settings.instance.trackingSpace, ref pmatTrackingOriginToOverlayTransform2);
			}
		}
		return pOverlayHandle;
	}
}
