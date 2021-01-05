// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_CharacterDatabase
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New TNH_CharacterDatabase", menuName = "TNH/CharacterDatabase", order = 0)]
  public class TNH_CharacterDatabase : ScriptableObject
  {
    public List<TNH_CharacterDef> Characters;

    public TNH_CharacterDef GetDef(TNH_Char c)
    {
      for (int index = 0; index < this.Characters.Count; ++index)
      {
        if (this.Characters[index].CharacterID == c)
          return this.Characters[index];
      }
      return (TNH_CharacterDef) null;
    }
  }
}
