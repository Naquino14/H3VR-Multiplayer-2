// Decompiled with JetBrains decompiler
// Type: ES2UserType_FistVROmniScoreList
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using FistVR;

public class ES2UserType_FistVROmniScoreList : ES2Type
{
  public ES2UserType_FistVROmniScoreList()
    : base(typeof (OmniScoreList))
  {
  }

  public override void Write(object obj, ES2Writer writer)
  {
    OmniScoreList omniScoreList = (OmniScoreList) obj;
    writer.Write<string>(omniScoreList.SequenceID);
    writer.Write<OmniScore>(omniScoreList.Scores);
    writer.Write<int>(omniScoreList.Trophy);
  }

  public override object Read(ES2Reader reader)
  {
    OmniScoreList omniScoreList = new OmniScoreList();
    this.Read(reader, (object) omniScoreList);
    return (object) omniScoreList;
  }

  public override void Read(ES2Reader reader, object c)
  {
    OmniScoreList omniScoreList = (OmniScoreList) c;
    omniScoreList.SequenceID = reader.Read<string>();
    omniScoreList.Scores = reader.ReadList<OmniScore>();
    omniScoreList.Trophy = reader.Read<int>();
  }
}
