using System;

namespace FistVR
{
	[Serializable]
	public class MeatGrinderFlags
	{
		public enum MeatGrinderMode
		{
			Classic,
			AllYouCanMeat,
			BuildYourOwnMeat,
			KidsMeatyMeal
		}

		public enum LightSourceOption
		{
			FlashLight,
			Lighter,
			GlowStick,
			BoxOfMatches,
			Random
		}

		public enum MeatGrinderNarratorMode
		{
			Classic,
			Terse,
			Silent
		}

		public bool HasNarratorDoneLongIntro;

		public bool HasPlayerEverWon;

		public MeatGrinderMaster.EventAI.EventAIMood AIMood = MeatGrinderMaster.EventAI.EventAIMood.Nasty;

		public MeatGrinderMode MGMode;

		public LightSourceOption PrimaryLight;

		public LightSourceOption SecondaryLight = LightSourceOption.Random;

		public MeatGrinderNarratorMode NarratorMode;

		public int SuccessEventVoiceIndex;

		public int MaxSuccessEventVoiceIndex = 4;

		public int ShortIntroIndex;

		public int MaxShortIntroIndex = 15;

		public void InitializeFromSaveFile(ES2Reader reader)
		{
			if (reader.TagExists("HasNarratorDoneLongIntro"))
			{
				HasNarratorDoneLongIntro = reader.Read<bool>("HasNarratorDoneLongIntro");
			}
			if (reader.TagExists("HasPlayerEverWon"))
			{
				HasPlayerEverWon = reader.Read<bool>("HasPlayerEverWon");
			}
			if (reader.TagExists("AIMood"))
			{
				AIMood = reader.Read<MeatGrinderMaster.EventAI.EventAIMood>("AIMood");
			}
			if (reader.TagExists("MGMode"))
			{
				MGMode = reader.Read<MeatGrinderMode>("MGMode");
			}
			if (reader.TagExists("PrimaryLight"))
			{
				PrimaryLight = reader.Read<LightSourceOption>("PrimaryLight");
			}
			if (reader.TagExists("SecondaryLight"))
			{
				SecondaryLight = reader.Read<LightSourceOption>("SecondaryLight");
			}
			if (reader.TagExists("NarratorMode"))
			{
				NarratorMode = reader.Read<MeatGrinderNarratorMode>("NarratorMode");
			}
		}

		public void SaveToFile(ES2Writer writer)
		{
			writer.Write(HasNarratorDoneLongIntro, "HasNarratorDoneLongIntro");
			writer.Write(HasPlayerEverWon, "HasPlayerEverWon");
			writer.Write(AIMood, "AIMood");
			writer.Write(MGMode, "MGMode");
			writer.Write(PrimaryLight, "PrimaryLight");
			writer.Write(SecondaryLight, "SecondaryLight");
			writer.Write(NarratorMode, "NarratorMode");
		}
	}
}
