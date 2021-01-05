using System;

namespace FistVR
{
	[Serializable]
	public class AudioOptions
	{
		public float MasterVolume = 1f;

		public float FXVolume = 1f;

		public float AmbientVolume = 1f;

		public float MusicVolume = 1f;

		public void InitializeFromSaveFile(ES2Reader reader)
		{
			if (reader.TagExists("MasterVolume"))
			{
				MasterVolume = reader.Read<float>("MasterVolume");
			}
			if (reader.TagExists("FXVolume"))
			{
				FXVolume = reader.Read<float>("FXVolume");
			}
			if (reader.TagExists("AmbientVolume"))
			{
				AmbientVolume = reader.Read<float>("AmbientVolume");
			}
			if (reader.TagExists("MusicVolume"))
			{
				MusicVolume = reader.Read<float>("MusicVolume");
			}
		}

		public void SaveToFile(ES2Writer writer)
		{
			writer.Write(MasterVolume, "MasterVolume");
			writer.Write(FXVolume, "FXVolume");
			writer.Write(AmbientVolume, "AmbientVolume");
			writer.Write(MusicVolume, "MusicVolume");
		}
	}
}
