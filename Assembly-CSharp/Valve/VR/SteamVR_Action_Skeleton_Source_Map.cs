// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Skeleton_Source_Map
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace Valve.VR
{
  public class SteamVR_Action_Skeleton_Source_Map : SteamVR_Action_Pose_Source_Map<SteamVR_Action_Skeleton_Source>
  {
    protected override SteamVR_Action_Skeleton_Source GetSourceElementForIndexer(
      SteamVR_Input_Sources inputSource)
    {
      return this.sources[SteamVR_Input_Sources.Any];
    }
  }
}
