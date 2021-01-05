// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_VoiceDatabase
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New TNH_VoiceDatabase", menuName = "TNH/TNH_VoiceDatabase", order = 0)]
  public class TNH_VoiceDatabase : ScriptableObject
  {
    public List<TNH_VoiceDatabase.TNH_VoiceLine> Lines;

    [Serializable]
    public class TNH_VoiceLine
    {
      public TNH_VoiceLineID ID;
      public AudioClip Clip_Standard;
      public AudioClip Clip_Corrupted;
    }
  }
}
