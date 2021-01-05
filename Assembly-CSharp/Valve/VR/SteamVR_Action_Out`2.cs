// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Out`2
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace Valve.VR
{
  [Serializable]
  public abstract class SteamVR_Action_Out<SourceMap, SourceElement> : SteamVR_Action<SourceMap, SourceElement>, ISteamVR_Action_Out, ISteamVR_Action, ISteamVR_Action_Out_Source, ISteamVR_Action_Source
    where SourceMap : SteamVR_Action_Source_Map<SourceElement>, new()
    where SourceElement : SteamVR_Action_Out_Source, new()
  {
  }
}
