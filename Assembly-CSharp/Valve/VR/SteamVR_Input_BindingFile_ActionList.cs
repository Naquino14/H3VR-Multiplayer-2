// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_BindingFile_ActionList
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Input_BindingFile_ActionList
  {
    public List<SteamVR_Input_BindingFile_Chord> chords = new List<SteamVR_Input_BindingFile_Chord>();
    public List<SteamVR_Input_BindingFile_Pose> poses = new List<SteamVR_Input_BindingFile_Pose>();
    public List<SteamVR_Input_BindingFile_Haptic> haptics = new List<SteamVR_Input_BindingFile_Haptic>();
    public List<SteamVR_Input_BindingFile_Source> sources = new List<SteamVR_Input_BindingFile_Source>();
    public List<SteamVR_Input_BindingFile_Skeleton> skeleton = new List<SteamVR_Input_BindingFile_Skeleton>();
  }
}
