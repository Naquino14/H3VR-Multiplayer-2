// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_ManifestFile_Application
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Valve.VR
{
  public class SteamVR_Input_ManifestFile_Application
  {
    public string app_key;
    public string launch_type;
    public string url;
    public string binary_path_windows;
    public string binary_path_linux;
    public string binary_path_osx;
    public string action_manifest_path;
    public string image_path;
    public Dictionary<string, SteamVR_Input_ManifestFile_ApplicationString> strings = new Dictionary<string, SteamVR_Input_ManifestFile_ApplicationString>();
  }
}
