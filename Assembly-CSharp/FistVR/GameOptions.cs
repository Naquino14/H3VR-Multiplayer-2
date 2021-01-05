// Decompiled with JetBrains decompiler
// Type: FistVR.GameOptions
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace FistVR
{
  [Serializable]
  public class GameOptions
  {
    public MovementOptions MovementOptions = new MovementOptions();
    public QuickbeltOptions QuickbeltOptions = new QuickbeltOptions();
    public ControlOptions ControlOptions = new ControlOptions();
    public UnitOptions UnitOptions = new UnitOptions();
    public PerformanceOptions PerformanceOptions = new PerformanceOptions();
    public UserOptions UserOptions = new UserOptions();
    public AudioOptions AudioOptions = new AudioOptions();
    public SimulationOptions SimulationOptions = new SimulationOptions();
    public MeatGrinderFlags MeatGrinderFlags = new MeatGrinderFlags();
    public XmasFlags XmasFlags = new XmasFlags();

    public void InitializeFromSaveFile()
    {
      if (ES2.Exists("Options.txt"))
      {
        using (ES2Reader reader = ES2Reader.Create("Options.txt"))
        {
          this.MovementOptions.InitializeFromSaveFile(reader);
          this.QuickbeltOptions.InitializeFromSaveFile(reader);
          this.ControlOptions.InitializeFromSaveFile(reader);
          this.UnitOptions.InitializeFromSaveFile(reader);
          this.PerformanceOptions.InitializeFromSaveFile(reader);
          this.UserOptions.InitializeFromSaveFile(reader);
          this.AudioOptions.InitializeFromSaveFile(reader);
          this.SimulationOptions.InitializeFromSaveFile(reader);
          this.MeatGrinderFlags.InitializeFromSaveFile(reader);
          this.XmasFlags.InitializeFromSaveFile(reader);
        }
        if (!((UnityEngine.Object) GM.CurrentOptionsPanel != (UnityEngine.Object) null))
          return;
        GM.CurrentOptionsPanel.GetComponent<OptionsPanel_Screenmanager>().RefreshScreens();
      }
      else
      {
        this.SaveToFile();
        this.InitializeFromSaveFile();
      }
    }

    public void SaveToFile()
    {
      using (ES2Writer writer = ES2Writer.Create("Options.txt"))
      {
        this.MovementOptions.SaveToFile(writer);
        this.QuickbeltOptions.SaveToFile(writer);
        this.ControlOptions.SaveToFile(writer);
        this.UnitOptions.SaveToFile(writer);
        this.PerformanceOptions.SaveToFile(writer);
        this.UserOptions.SaveToFile(writer);
        this.AudioOptions.SaveToFile(writer);
        this.SimulationOptions.SaveToFile(writer);
        this.MeatGrinderFlags.SaveToFile(writer);
        this.XmasFlags.SaveToFile(writer);
        writer.Save();
      }
    }
  }
}
