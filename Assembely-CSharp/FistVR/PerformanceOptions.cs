using System;

namespace FistVR
{
	[Serializable]
	public class PerformanceOptions
	{
		public enum QualitySetting
		{
			Ultra,
			High,
			Medium,
			Low,
			Potato,
			TurboPotato
		}

		public QualitySetting CurrentQualitySetting = QualitySetting.High;

		public bool IsPostEnabled_AO;

		public bool IsPostEnabled_CC = true;

		public bool IsPostEnabled_Bloom;

		public void InitializeFromSaveFile(ES2Reader reader)
		{
			if (reader.TagExists("CurrentQualitySetting"))
			{
				CurrentQualitySetting = reader.Read<QualitySetting>("CurrentQualitySetting");
			}
			if (reader.TagExists("IsPostEnabled_AO"))
			{
				IsPostEnabled_AO = reader.Read<bool>("IsPostEnabled_AO");
			}
			if (reader.TagExists("IsPostEnabled_CC"))
			{
				IsPostEnabled_CC = reader.Read<bool>("IsPostEnabled_CC");
			}
			if (reader.TagExists("IsPostEnabled_Bloom"))
			{
				IsPostEnabled_Bloom = reader.Read<bool>("IsPostEnabled_Bloom");
			}
		}

		public void SaveToFile(ES2Writer writer)
		{
			writer.Write(CurrentQualitySetting, "CurrentQualitySetting");
			writer.Write(IsPostEnabled_AO, "IsPostEnabled_AO");
			writer.Write(IsPostEnabled_CC, "IsPostEnabled_CC");
			writer.Write(IsPostEnabled_Bloom, "IsPostEnabled_Bloom");
		}
	}
}
