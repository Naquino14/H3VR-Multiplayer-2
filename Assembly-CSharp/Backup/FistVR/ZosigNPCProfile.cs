// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigNPCProfile
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New NPC Profile", menuName = "Zosig/NPCProfile", order = 0)]
  public class ZosigNPCProfile : ScriptableObject
  {
    public int NPCIndex;
    public Vector2 SpeakJitterRange = new Vector2(0.05f, 0.25f);
    public Vector2 SpeakPowerRange = new Vector2(0.5f, 1f);
    public List<ZosigNPCProfile.NPCLine> Lines;

    public enum NPCLineType
    {
      Howdy,
      WhoRUInitial,
      WhoRUAgain,
      QuestInitial,
      QuestReminder,
      QuestRadiant,
      QuestComplete,
      Info,
      Map,
      Etc,
      Info_Backpack,
      Info_Baiting,
      Info_Bullets,
      Info_Cans,
      Info_Grenade,
      Info_Herbs,
      Info_Junk,
      Info_Meatcores,
      Info_PacificationSquad,
      Info_Pie,
      Info_Powerups,
      Info_Revolver,
      Info_Rotwieners,
      Info_Teleport,
      Info_Trade,
    }

    [Serializable]
    public class NPCLine
    {
      public ZosigNPCProfile.NPCLineType Type;
      public List<AudioClip> Clips;
      [Space(5f)]
      public string FlagRequiredToPlay;
      public int FlagValueRequiredToPlay = 1;
      [Space(5f)]
      public string FlagOnLineSpoken;
      public int FlagValueOnLineSpoken = 1;
    }
  }
}
