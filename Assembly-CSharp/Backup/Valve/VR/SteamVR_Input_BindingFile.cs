// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_BindingFile
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Input_BindingFile
  {
    public string app_key;
    public Dictionary<string, SteamVR_Input_BindingFile_ActionList> bindings = new Dictionary<string, SteamVR_Input_BindingFile_ActionList>();
    public string controller_type;
    public string description;
    public string name;
  }
}
