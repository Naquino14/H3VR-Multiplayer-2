// Decompiled with JetBrains decompiler
// Type: FistVR.WurstWorldSaveGame
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public class WurstWorldSaveGame
  {
    public WWFlags Flags = new WWFlags();

    public void InitializeFromSaveFile()
    {
      if (ES2.Exists("WW.txt"))
      {
        Debug.Log((object) "WW.txt exists, initializing from it");
        using (ES2Reader reader = ES2Reader.Create("WW.txt"))
          this.Flags.InitializeFromSaveFile(reader);
      }
      else
      {
        Debug.Log((object) "WW.txt does not exist, creating it");
        this.SaveToFile();
        this.InitializeFromSaveFile();
      }
    }

    public void SaveToFile()
    {
      using (ES2Writer writer = ES2Writer.Create("WW.txt"))
      {
        this.Flags.SaveToFile(writer);
        writer.Save();
      }
    }
  }
}
