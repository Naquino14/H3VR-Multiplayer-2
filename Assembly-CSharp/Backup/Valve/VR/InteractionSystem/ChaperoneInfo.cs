// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.ChaperoneInfo
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
  public class ChaperoneInfo : MonoBehaviour
  {
    public static SteamVR_Events.Event Initialized = new SteamVR_Events.Event();
    private static ChaperoneInfo _instance;

    public bool initialized { get; private set; }

    public float playAreaSizeX { get; private set; }

    public float playAreaSizeZ { get; private set; }

    public bool roomscale { get; private set; }

    public static SteamVR_Events.Action InitializedAction(UnityAction action) => (SteamVR_Events.Action) new SteamVR_Events.ActionNoArgs(ChaperoneInfo.Initialized, action);

    public static ChaperoneInfo instance
    {
      get
      {
        if ((Object) ChaperoneInfo._instance == (Object) null)
        {
          ChaperoneInfo._instance = new GameObject("[ChaperoneInfo]").AddComponent<ChaperoneInfo>();
          ChaperoneInfo._instance.initialized = false;
          ChaperoneInfo._instance.playAreaSizeX = 1f;
          ChaperoneInfo._instance.playAreaSizeZ = 1f;
          ChaperoneInfo._instance.roomscale = false;
          Object.DontDestroyOnLoad((Object) ChaperoneInfo._instance.gameObject);
        }
        return ChaperoneInfo._instance;
      }
    }

    [DebuggerHidden]
    private IEnumerator Start() => (IEnumerator) new ChaperoneInfo.\u003CStart\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
