﻿// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Windows_Editor_Helper
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace Valve.VR
{
  public class SteamVR_Windows_Editor_Helper
  {
    public static SteamVR_Windows_Editor_Helper.BrowserApplication GetDefaultBrowser() => SteamVR_Windows_Editor_Helper.BrowserApplication.Firefox;

    public enum BrowserApplication
    {
      Unknown,
      InternetExplorer,
      Firefox,
      Chrome,
      Opera,
      Safari,
      Edge,
    }
  }
}
