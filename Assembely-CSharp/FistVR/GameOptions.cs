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
					MovementOptions.InitializeFromSaveFile(reader);
					QuickbeltOptions.InitializeFromSaveFile(reader);
					ControlOptions.InitializeFromSaveFile(reader);
					UnitOptions.InitializeFromSaveFile(reader);
					PerformanceOptions.InitializeFromSaveFile(reader);
					UserOptions.InitializeFromSaveFile(reader);
					AudioOptions.InitializeFromSaveFile(reader);
					SimulationOptions.InitializeFromSaveFile(reader);
					MeatGrinderFlags.InitializeFromSaveFile(reader);
					XmasFlags.InitializeFromSaveFile(reader);
				}
				if (GM.CurrentOptionsPanel != null)
				{
					GM.CurrentOptionsPanel.GetComponent<OptionsPanel_Screenmanager>().RefreshScreens();
				}
			}
			else
			{
				SaveToFile();
				InitializeFromSaveFile();
			}
		}

		public void SaveToFile()
		{
			using ES2Writer eS2Writer = ES2Writer.Create("Options.txt");
			MovementOptions.SaveToFile(eS2Writer);
			QuickbeltOptions.SaveToFile(eS2Writer);
			ControlOptions.SaveToFile(eS2Writer);
			UnitOptions.SaveToFile(eS2Writer);
			PerformanceOptions.SaveToFile(eS2Writer);
			UserOptions.SaveToFile(eS2Writer);
			AudioOptions.SaveToFile(eS2Writer);
			SimulationOptions.SaveToFile(eS2Writer);
			MeatGrinderFlags.SaveToFile(eS2Writer);
			XmasFlags.SaveToFile(eS2Writer);
			eS2Writer.Save();
		}
	}
}
