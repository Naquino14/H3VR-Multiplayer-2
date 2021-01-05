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
			{
				BunkersOpened = reader.ReadArray<bool>("BunkersOpened");
			}
			if (reader.TagExists("FieldsOpened"))
			{
				FieldsOpened = reader.ReadArray<bool>("FieldsOpened");
			}
			if (reader.TagExists("TowersActive"))
			{
				TowersActive = reader.ReadArray<bool>("TowersActive");
			}
			if (reader.TagExists("MessagesAcquired"))
			{
				MessagesAcquired = reader.ReadArray<bool>("MessagesAcquired");
			}
			if (reader.TagExists("MessagesRead"))
			{
				MessagesRead = reader.ReadArray<bool>("MessagesRead");
			}
		}

		public void SaveToFile(ES2Writer writer)
		{
			writer.Write(BunkersOpened, "BunkersOpened");
			writer.Write(FieldsOpened, "FieldsOpened");
			writer.Write(TowersActive, "TowersActive");
			writer.Write(MessagesAcquired, "MessagesAcquired");
			writer.Write(MessagesRead, "MessagesRead");
		}
	}
}
