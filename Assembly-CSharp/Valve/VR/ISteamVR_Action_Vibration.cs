// Decompiled with JetBrains decompiler
// Type: Valve.VR.ISteamVR_Action_Vibration
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace Valve.VR
{
  public interface ISteamVR_Action_Vibration : ISteamVR_Action_Out, ISteamVR_Action, ISteamVR_Action_Out_Source, ISteamVR_Action_Source
  {
    void Execute(
      float secondsFromNow,
      float durationSeconds,
      float frequency,
      float amplitude,
      SteamVR_Input_Sources inputSource);
  }
}
