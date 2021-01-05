using System;

namespace FistVR
{
	[Serializable]
	public class SimulationOptions
	{
		public enum HitDecals
		{
			Disabled,
			Enabled
		}

		public enum HitSounds
		{
			Disabled,
			Enabled
		}

		public enum GravityMode
		{
			Realistic,
			Playful,
			OnTheMoon,
			None
		}

		public enum SpentShellDespawnTime
		{
			Seconds_5,
			Seconds_10,
			Seconds_30,
			Infinite
		}

		public GravityMode ObjectGravityMode = GravityMode.Playful;

		public GravityMode PlayerGravityMode = GravityMode.Playful;

		public GravityMode BallisticGravityMode;

		public SpentShellDespawnTime ShellTime = SpentShellDespawnTime.Seconds_10;

		public bool SosigClownMode;

		public HitDecals HitDecalMode;

		public HitSounds HitSoundMode;

		public int MaxHitDecalIndex = 2;

		public int[] MaxHitDecals = new int[5]
		{
			5,
			25,
			100,
			250,
			1000
		};

		public void InitializeFromSaveFile(ES2Reader reader)
		{
			if (reader.TagExists("ObjectGravityMode"))
			{
				ObjectGravityMode = reader.Read<GravityMode>("ObjectGravityMode");
			}
			if (reader.TagExists("PlayerGravityMode"))
			{
				PlayerGravityMode = reader.Read<GravityMode>("PlayerGravityMode");
			}
			if (reader.TagExists("BallisticGravityMode"))
			{
				BallisticGravityMode = reader.Read<GravityMode>("BallisticGravityMode");
			}
			if (reader.TagExists("ShellTime"))
			{
				ShellTime = reader.Read<SpentShellDespawnTime>("ShellTime");
			}
			if (reader.TagExists("SosigClownMode"))
			{
				SosigClownMode = reader.Read<bool>("SosigClownMode");
			}
			if (reader.TagExists("HitDecalMode"))
			{
				HitDecalMode = reader.Read<HitDecals>("HitDecalMode");
			}
			if (reader.TagExists("HitSoundMode"))
			{
				HitSoundMode = reader.Read<HitSounds>("HitSoundMode");
			}
			if (reader.TagExists("MaxHitDecalIndex"))
			{
				MaxHitDecalIndex = reader.Read<int>("MaxHitDecalIndex");
			}
			ManagerSingleton<GM>.Instance.RefreshGravity();
		}

		public void SaveToFile(ES2Writer writer)
		{
			writer.Write(ObjectGravityMode, "ObjectGravityMode");
			writer.Write(PlayerGravityMode, "PlayerGravityMode");
			writer.Write(BallisticGravityMode, "BallisticGravityMode");
			writer.Write(ShellTime, "ShellTime");
			writer.Write(SosigClownMode, "SosigClownMode");
			writer.Write(HitDecalMode, "HitDecalMode");
			writer.Write(HitSoundMode, "HitSoundMode");
			writer.Write(MaxHitDecalIndex, "MaxHitDecalIndex");
			ManagerSingleton<GM>.Instance.RefreshGravity();
		}
	}
}
