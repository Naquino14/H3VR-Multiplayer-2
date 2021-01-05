// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Settings
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Serialization;

namespace Valve.VR
{
  public class SteamVR_Settings : ScriptableObject
  {
    private static SteamVR_Settings _instance;
    public bool pauseGameWhenDashboardVisible = true;
    public bool lockPhysicsUpdateRateToRenderFrequency = true;
    [SerializeField]
    [FormerlySerializedAs("trackingSpace")]
    private ETrackingUniverseOrigin trackingSpaceOrigin = ETrackingUniverseOrigin.TrackingUniverseStanding;
    [Tooltip("Filename local to the project root (or executable, in a build)")]
    public string actionsFilePath = "actions.json";
    [Tooltip("Path local to the Assets folder")]
    public string steamVRInputPath = "SteamVR_Input";
    public SteamVR_UpdateModes inputUpdateMode = SteamVR_UpdateModes.OnUpdate;
    public SteamVR_UpdateModes poseUpdateMode = SteamVR_UpdateModes.OnPreCull;
    public bool activateFirstActionSetOnStart = true;
    [Tooltip("This is the app key the unity editor will use to identify your application. (can be \"steam.app.[appid]\" to persist bindings between editor steam)")]
    public string editorAppKey;
    [Tooltip("The SteamVR Plugin can automatically make sure VR is enabled in your player settings and if not, enable it.")]
    public bool autoEnableVR = true;

    public static SteamVR_Settings instance
    {
      get
      {
        SteamVR_Settings.LoadInstance();
        return SteamVR_Settings._instance;
      }
    }

    public ETrackingUniverseOrigin trackingSpace
    {
      get => this.trackingSpaceOrigin;
      set
      {
        this.trackingSpaceOrigin = value;
        if (!SteamVR_Behaviour.isPlaying)
          return;
        SteamVR_Action_Pose.SetTrackingUniverseOrigin(this.trackingSpaceOrigin);
      }
    }

    public bool IsInputUpdateMode(SteamVR_UpdateModes tocheck) => (this.inputUpdateMode & tocheck) == tocheck;

    public bool IsPoseUpdateMode(SteamVR_UpdateModes tocheck) => (this.poseUpdateMode & tocheck) == tocheck;

    public static void VerifyScriptableObject() => SteamVR_Settings.LoadInstance();

    private static void LoadInstance()
    {
      if (!((Object) SteamVR_Settings._instance == (Object) null))
        return;
      SteamVR_Settings._instance = Resources.Load<SteamVR_Settings>(nameof (SteamVR_Settings));
      if ((Object) SteamVR_Settings._instance == (Object) null)
        SteamVR_Settings._instance = ScriptableObject.CreateInstance<SteamVR_Settings>();
      if (!string.IsNullOrEmpty(SteamVR_Settings._instance.editorAppKey))
        return;
      SteamVR_Settings._instance.editorAppKey = SteamVR.GenerateAppKey();
      Debug.Log((object) ("<b>[SteamVR Setup]</b> Generated you an editor app key of: " + SteamVR_Settings._instance.editorAppKey + ". This lets the editor tell SteamVR what project this is. Has no effect on builds. This can be changed in Assets/SteamVR/Resources/SteamVR_Settings"));
    }
  }
}
