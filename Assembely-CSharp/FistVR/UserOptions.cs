using System;

namespace FistVR
{
	[Serializable]
	public class UserOptions
	{
		public string DefaultHighScoreName = string.Empty;

		public void InitializeFromSaveFile(ES2Reader reader)
		{
			if (reader.TagExists("DefaultHighScoreName"))
			{
				DefaultHighScoreName = reader.Read<string>("DefaultHighScoreName");
			}
		}

		public void SaveToFile(ES2Writer writer)
		{
			writer.Write(DefaultHighScoreName, "DefaultHighScoreName");
		}
	}
}
