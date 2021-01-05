// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Behaviour
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VR;

namespace Valve.VR
{
  public class SteamVR_Behaviour : MonoBehaviour
  {
    private const string openVRDeviceName = "OpenVR";
    public static bool forcingInitialization;
    private static SteamVR_Behaviour _instance;
    public bool initializeSteamVROnAwake = true;
    public bool doNotDestroy = true;
    [HideInInspector]
    public SteamVR_Render steamvr_render;
    internal static bool isPlaying;
    private static bool initializing;
    private Coroutine initializeCoroutine;
    protected static int lastFrameCount = -1;

    public static SteamVR_Behaviour instance
    {
      get
      {
        if ((Object) SteamVR_Behaviour._instance == (Object) null)
          SteamVR_Behaviour.Initialize();
        return SteamVR_Behaviour._instance;
      }
    }

    public static void Initialize(bool forceUnityVRToOpenVR = false)
    {
      if (!((Object) SteamVR_Behaviour._instance == (Object) null) || SteamVR_Behaviour.initializing)
        return;
      SteamVR_Behaviour.initializing = true;
      GameObject gameObject1 = (GameObject) null;
      if (forceUnityVRToOpenVR)
        SteamVR_Behaviour.forcingInitialization = true;
      SteamVR_Render objectOfType = Object.FindObjectOfType<SteamVR_Render>();
      if ((Object) objectOfType != (Object) null)
        gameObject1 = objectOfType.gameObject;
      SteamVR_Behaviour steamVrBehaviour = Object.FindObjectOfType<SteamVR_Behaviour>();
      if ((Object) steamVrBehaviour != (Object) null)
        gameObject1 = steamVrBehaviour.gameObject;
      if ((Object) gameObject1 == (Object) null)
      {
        GameObject gameObject2 = new GameObject("[SteamVR]");
        SteamVR_Behaviour._instance = gameObject2.AddComponent<SteamVR_Behaviour>();
        SteamVR_Behaviour._instance.steamvr_render = gameObject2.AddComponent<SteamVR_Render>();
      }
      else
      {
        steamVrBehaviour = gameObject1.GetComponent<SteamVR_Behaviour>();
        if ((Object) steamVrBehaviour == (Object) null)
          steamVrBehaviour = gameObject1.AddComponent<SteamVR_Behaviour>();
        if ((Object) objectOfType != (Object) null)
        {
          steamVrBehaviour.steamvr_render = objectOfType;
        }
        else
        {
          steamVrBehaviour.steamvr_render = gameObject1.GetComponent<SteamVR_Render>();
          if ((Object) steamVrBehaviour.steamvr_render == (Object) null)
            steamVrBehaviour.steamvr_render = gameObject1.AddComponent<SteamVR_Render>();
        }
        SteamVR_Behaviour._instance = steamVrBehaviour;
      }
      if ((Object) steamVrBehaviour != (Object) null && steamVrBehaviour.doNotDestroy)
        Object.DontDestroyOnLoad((Object) steamVrBehaviour.transform.root.gameObject);
      SteamVR_Behaviour.initializing = false;
    }

    protected void Awake()
    {
      SteamVR_Behaviour.isPlaying = true;
      if (!this.initializeSteamVROnAwake || SteamVR_Behaviour.forcingInitialization)
        return;
      this.InitializeSteamVR();
    }

    public void InitializeSteamVR(bool forceUnityVRToOpenVR = false)
    {
      if (forceUnityVRToOpenVR)
      {
        SteamVR_Behaviour.forcingInitialization = true;
        if (this.initializeCoroutine != null)
          this.StopCoroutine(this.initializeCoroutine);
        if (VRSettings.loadedDeviceName == "OpenVR")
          this.EnableOpenVR();
        else
          this.initializeCoroutine = this.StartCoroutine(this.DoInitializeSteamVR(forceUnityVRToOpenVR));
      }
      else
        SteamVR.Initialize();
    }

    [DebuggerHidden]
    private IEnumerator DoInitializeSteamVR(bool forceUnityVRToOpenVR = false) => (IEnumerator) new SteamVR_Behaviour.\u003CDoInitializeSteamVR\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    private void EnableOpenVR()
    {
      VRSettings.enabled = true;
      SteamVR.Initialize();
      this.initializeCoroutine = (Coroutine) null;
      SteamVR_Behaviour.forcingInitialization = false;
    }

    protected void OnEnable()
    {
      Camera.onPreCull += new Camera.CameraCallback(this.OnCameraPreCull);
      SteamVR_Events.System(EVREventType.VREvent_Quit).Listen(new UnityAction<VREvent_t>(this.OnQuit));
    }

    protected void OnDisable()
    {
      Camera.onPreCull -= new Camera.CameraCallback(this.OnCameraPreCull);
      SteamVR_Events.System(EVREventType.VREvent_Quit).Remove(new UnityAction<VREvent_t>(this.OnQuit));
    }

    protected void OnCameraPreCull(Camera cam)
    {
      if (!cam.stereoEnabled)
        return;
      this.PreCull();
    }

    protected void PreCull()
    {
      if (Time.frameCount == SteamVR_Behaviour.lastFrameCount)
        return;
      SteamVR_Behaviour.lastFrameCount = Time.frameCount;
      SteamVR_Input.OnPreCull();
    }

    protected void FixedUpdate() => SteamVR_Input.FixedUpdate();

    protected void LateUpdate() => SteamVR_Input.LateUpdate();

    protected void Update() => SteamVR_Input.Update();

    protected void OnQuit(VREvent_t vrEvent) => Application.Quit();
  }
}
