// Decompiled with JetBrains decompiler
// Type: FistVR.XmasFlags
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FistVR
{
  [Serializable]
  public class XmasFlags
  {
    public int OrnamentShatterIndex;
    public bool[] BunkersOpened = new bool[24];
    public bool[] FieldsOpened = new bool[24];
    public bool[] TowersActive = new bool[9];
    public bool[] MessagesAcquired = new bool[50];
    public bool[] MessagesRead = new bool[50];

    public void InitializeFromSaveFile(ES2Reader reader)
    {
      if (reader.TagExists("BunkersOpened"))
        this.BunkersOpened = reader.ReadArray<bool>("BunkersOpened");
      if (reader.TagExists("FieldsOpened"))
        this.FieldsOpened = reader.ReadArray<bool>("FieldsOpened");
      if (reader.TagExists("TowersActive"))
        this.TowersActive = reader.ReadArray<bool>("TowersActive");
      if (reader.TagExists("MessagesAcquired"))
        this.MessagesAcquired = reader.ReadArray<bool>("MessagesAcquired");
      if (!reader.TagExists("MessagesRead"))
        return;
      this.MessagesRead = reader.ReadArray<bool>("MessagesRead");
    }

    public void SaveToFile(ES2Writer writer)
    {
      writer.Write<bool>(this.BunkersOpened, "BunkersOpened");
      writer.Write<bool>(this.FieldsOpened, "FieldsOpened");
      writer.Write<bool>(this.TowersActive, "TowersActive");
      writer.Write<bool>(this.MessagesAcquired, "MessagesAcquired");
      writer.Write<bool>(this.MessagesRead, "MessagesRead");
    }
  }
}
