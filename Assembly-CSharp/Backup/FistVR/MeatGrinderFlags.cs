// Decompiled with JetBrains decompiler
// Type: FistVR.MeatGrinderFlags
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FistVR
{
  [Serializable]
  public class MeatGrinderFlags
  {
    public bool HasNarratorDoneLongIntro;
    public bool HasPlayerEverWon;
    public MeatGrinderMaster.EventAI.EventAIMood AIMood = MeatGrinderMaster.EventAI.EventAIMood.Nasty;
    public MeatGrinderFlags.MeatGrinderMode MGMode;
    public MeatGrinderFlags.LightSourceOption PrimaryLight;
    public MeatGrinderFlags.LightSourceOption SecondaryLight = MeatGrinderFlags.LightSourceOption.Random;
    public MeatGrinderFlags.MeatGrinderNarratorMode NarratorMode;
    public int SuccessEventVoiceIndex;
    public int MaxSuccessEventVoiceIndex = 4;
    public int ShortIntroIndex;
    public int MaxShortIntroIndex = 15;

    public void InitializeFromSaveFile(ES2Reader reader)
    {
      if (reader.TagExists("HasNarratorDoneLongIntro"))
        this.HasNarratorDoneLongIntro = reader.Read<bool>("HasNarratorDoneLongIntro");
      if (reader.TagExists("HasPlayerEverWon"))
        this.HasPlayerEverWon = reader.Read<bool>("HasPlayerEverWon");
      if (reader.TagExists("AIMood"))
        this.AIMood = reader.Read<MeatGrinderMaster.EventAI.EventAIMood>("AIMood");
      if (reader.TagExists("MGMode"))
        this.MGMode = reader.Read<MeatGrinderFlags.MeatGrinderMode>("MGMode");
      if (reader.TagExists("PrimaryLight"))
        this.PrimaryLight = reader.Read<MeatGrinderFlags.LightSourceOption>("PrimaryLight");
      if (reader.TagExists("SecondaryLight"))
        this.SecondaryLight = reader.Read<MeatGrinderFlags.LightSourceOption>("SecondaryLight");
      if (!reader.TagExists("NarratorMode"))
        return;
      this.NarratorMode = reader.Read<MeatGrinderFlags.MeatGrinderNarratorMode>("NarratorMode");
    }

    public void SaveToFile(ES2Writer writer)
    {
      writer.Write<bool>(this.HasNarratorDoneLongIntro, "HasNarratorDoneLongIntro");
      writer.Write<bool>(this.HasPlayerEverWon, "HasPlayerEverWon");
      writer.Write<MeatGrinderMaster.EventAI.EventAIMood>(this.AIMood, "AIMood");
      writer.Write<MeatGrinderFlags.MeatGrinderMode>(this.MGMode, "MGMode");
      writer.Write<MeatGrinderFlags.LightSourceOption>(this.PrimaryLight, "PrimaryLight");
      writer.Write<MeatGrinderFlags.LightSourceOption>(this.SecondaryLight, "SecondaryLight");
      writer.Write<MeatGrinderFlags.MeatGrinderNarratorMode>(this.NarratorMode, "NarratorMode");
    }

    public enum MeatGrinderMode
    {
      Classic,
      AllYouCanMeat,
      BuildYourOwnMeat,
      KidsMeatyMeal,
    }

    public enum LightSourceOption
    {
      FlashLight,
      Lighter,
      GlowStick,
      BoxOfMatches,
      Random,
    }

    public enum MeatGrinderNarratorMode
    {
      Classic,
      Terse,
      Silent,
    }
  }
}
