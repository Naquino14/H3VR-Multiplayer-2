// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.DebugUI
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class DebugUI : MonoBehaviour
  {
    private Player player;
    private static DebugUI _instance;

    public static DebugUI instance
    {
      get
      {
        if ((Object) DebugUI._instance == (Object) null)
          DebugUI._instance = Object.FindObjectOfType<DebugUI>();
        return DebugUI._instance;
      }
    }

    private void Start() => this.player = Player.instance;

    private void OnGUI()
    {
      if (!Debug.isDebugBuild)
        return;
      this.player.Draw2DDebug();
    }
  }
}
