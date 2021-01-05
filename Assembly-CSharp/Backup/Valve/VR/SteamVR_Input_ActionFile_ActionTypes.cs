// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_ActionFile_ActionTypes
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace Valve.VR
{
  public static class SteamVR_Input_ActionFile_ActionTypes
  {
    public static string boolean = nameof (boolean);
    public static string vector1 = nameof (vector1);
    public static string vector2 = nameof (vector2);
    public static string vector3 = nameof (vector3);
    public static string vibration = nameof (vibration);
    public static string pose = nameof (pose);
    public static string skeleton = nameof (skeleton);
    public static string skeletonLeftPath = "\\skeleton\\hand\\left";
    public static string skeletonRightPath = "\\skeleton\\hand\\right";
    public static string[] listAll = new string[7]
    {
      SteamVR_Input_ActionFile_ActionTypes.boolean,
      SteamVR_Input_ActionFile_ActionTypes.vector1,
      SteamVR_Input_ActionFile_ActionTypes.vector2,
      SteamVR_Input_ActionFile_ActionTypes.vector3,
      SteamVR_Input_ActionFile_ActionTypes.vibration,
      SteamVR_Input_ActionFile_ActionTypes.pose,
      SteamVR_Input_ActionFile_ActionTypes.skeleton
    };
    public static string[] listIn = new string[6]
    {
      SteamVR_Input_ActionFile_ActionTypes.boolean,
      SteamVR_Input_ActionFile_ActionTypes.vector1,
      SteamVR_Input_ActionFile_ActionTypes.vector2,
      SteamVR_Input_ActionFile_ActionTypes.vector3,
      SteamVR_Input_ActionFile_ActionTypes.pose,
      SteamVR_Input_ActionFile_ActionTypes.skeleton
    };
    public static string[] listOut = new string[1]
    {
      SteamVR_Input_ActionFile_ActionTypes.vibration
    };
    public static string[] listSkeletons = new string[2]
    {
      SteamVR_Input_ActionFile_ActionTypes.skeletonLeftPath,
      SteamVR_Input_ActionFile_ActionTypes.skeletonRightPath
    };
  }
}
