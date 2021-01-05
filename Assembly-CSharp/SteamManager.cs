// Decompiled with JetBrains decompiler
// Type: SteamManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using Steamworks;
using System;
using System.Text;
using UnityEngine;

[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
  private static SteamManager s_instance;
  private static bool s_EverInialized;
  private bool m_bInitialized;
  private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

  private static SteamManager Instance => (UnityEngine.Object) SteamManager.s_instance == (UnityEngine.Object) null ? new GameObject(nameof (SteamManager)).AddComponent<SteamManager>() : SteamManager.s_instance;

  public static bool Initialized => SteamManager.Instance.m_bInitialized;

  private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText) => Debug.LogWarning((object) pchDebugText);

  private void Awake()
  {
    if ((UnityEngine.Object) SteamManager.s_instance != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }
    else
    {
      SteamManager.s_instance = this;
      if (SteamManager.s_EverInialized)
        throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
      UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.gameObject);
      if (!Packsize.Test())
        Debug.LogError((object) "[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", (UnityEngine.Object) this);
      if (!DllCheck.Test())
        Debug.LogError((object) "[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", (UnityEngine.Object) this);
      try
      {
        if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
        {
          Application.Quit();
          return;
        }
      }
      catch (DllNotFoundException ex)
      {
        Debug.LogError((object) ("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + (object) ex), (UnityEngine.Object) this);
        Application.Quit();
        return;
      }
      this.m_bInitialized = SteamAPI.Init();
      if (!this.m_bInitialized)
        Debug.LogError((object) "[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", (UnityEngine.Object) this);
      else
        SteamManager.s_EverInialized = true;
    }
  }

  private void OnEnable()
  {
    if ((UnityEngine.Object) SteamManager.s_instance == (UnityEngine.Object) null)
      SteamManager.s_instance = this;
    if (!this.m_bInitialized || this.m_SteamAPIWarningMessageHook != null)
      return;
    this.m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamManager.SteamAPIDebugTextHook);
    SteamClient.SetWarningMessageHook(this.m_SteamAPIWarningMessageHook);
  }

  private void OnDestroy()
  {
    if ((UnityEngine.Object) SteamManager.s_instance != (UnityEngine.Object) this)
      return;
    SteamManager.s_instance = (SteamManager) null;
    if (!this.m_bInitialized)
      return;
    SteamAPI.Shutdown();
  }

  private void Update()
  {
    if (!this.m_bInitialized)
      return;
    SteamAPI.RunCallbacks();
  }
}
