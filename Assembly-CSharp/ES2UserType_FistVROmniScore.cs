// Decompiled with JetBrains decompiler
// Type: ES2UserType_FistVROmniScore
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using FistVR;

public class ES2UserType_FistVROmniScore : ES2Type
{
  public ES2UserType_FistVROmniScore()
    : base(typeof (OmniScore))
  {
  }

  public override void Write(object obj, ES2Writer writer)
  {
    OmniScore omniScore = (OmniScore) obj;
    writer.Write<int>(omniScore.Score);
    writer.Write<string>(omniScore.Name);
  }

  public override object Read(ES2Reader reader)
  {
    OmniScore omniScore = new OmniScore();
    this.Read(reader, (object) omniScore);
    return (object) omniScore;
  }

  public override void Read(ES2Reader reader, object c)
  {
    OmniScore omniScore = (OmniScore) c;
    omniScore.Score = reader.Read<int>();
    omniScore.Name = reader.Read<string>();
  }
}
