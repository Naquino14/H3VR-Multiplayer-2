// Decompiled with JetBrains decompiler
// Type: FistVR.OmniMaster
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public class OmniMaster
  {
    public OmniFlags OmniFlags = new OmniFlags();
    public OmniUnlocks OmniUnlocks = new OmniUnlocks();

    public void InitializeFromSaveFile()
    {
      if (ES2.Exists("OmniScores.txt"))
      {
        Debug.Log((object) "OmniScores.txt exists, initializing from it");
        using (ES2Reader reader = ES2Reader.Create("OmniScores.txt"))
          this.OmniFlags.InitializeFromSaveFile(reader);
      }
      else
      {
        Debug.Log((object) "OmniScores.txt does not exist, creating it");
        this.SaveToFile();
        this.InitializeFromSaveFile();
      }
    }

    public void SaveToFile()
    {
      using (ES2Writer writer = ES2Writer.Create("OmniScores.txt"))
      {
        this.OmniFlags.SaveToFile(writer);
        writer.Save();
      }
    }

    public void InitializeUnlocks()
    {
      if (ES2.Exists("OmniUnlocks.txt"))
      {
        Debug.Log((object) "OmniUnlocks.txt exists, initializing from it");
        using (ES2Reader reader = ES2Reader.Create("OmniUnlocks.txt"))
          this.OmniUnlocks.InitializeFromSaveFile(reader);
      }
      else
      {
        Debug.Log((object) "OmniUnlocks.txt does not exist, creating it");
        this.SaveToFile();
        this.InitializeFromSaveFile();
      }
    }

    public void SaveUnlocksToFile()
    {
      using (ES2Writer writer = ES2Writer.Create("OmniUnlocks.txt"))
      {
        this.OmniUnlocks.SaveToFile(writer);
        writer.Save();
      }
    }
  }
}
