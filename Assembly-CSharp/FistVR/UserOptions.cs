// Decompiled with JetBrains decompiler
// Type: FistVR.UserOptions
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FistVR
{
  [Serializable]
  public class UserOptions
  {
    public string DefaultHighScoreName = string.Empty;

    public void InitializeFromSaveFile(ES2Reader reader)
    {
      if (!reader.TagExists("DefaultHighScoreName"))
        return;
      this.DefaultHighScoreName = reader.Read<string>("DefaultHighScoreName");
    }

    public void SaveToFile(ES2Writer writer) => writer.Write<string>(this.DefaultHighScoreName, "DefaultHighScoreName");
  }
}
