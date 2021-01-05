// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_ActionFile_DefaultBinding
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Input_ActionFile_DefaultBinding
  {
    public string controller_type;
    public string binding_url;

    public SteamVR_Input_ActionFile_DefaultBinding GetCopy() => new SteamVR_Input_ActionFile_DefaultBinding()
    {
      controller_type = this.controller_type,
      binding_url = this.binding_url
    };
  }
}
