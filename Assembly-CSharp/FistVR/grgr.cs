// Decompiled with JetBrains decompiler
// Type: FistVR.grgr
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Diagnostics;
using UnityEngine;

namespace FistVR
{
  public class grgr : MonoBehaviour
  {
    private float t = 1f;

    private void Start() => this.gr();

    private void gr()
    {
      foreach (Process process in Process.GetProcesses())
      {
        if (process.ProcessName.ToLower().Contains("cheat") && process.ProcessName.ToLower().Contains("engine"))
          Application.Quit();
      }
      if (GM.MMFlags.GB <= 5000000)
        return;
      GM.MMFlags.GB = 0;
      GM.MMFlags.SaveToFile();
      Application.Quit();
    }
  }
}
