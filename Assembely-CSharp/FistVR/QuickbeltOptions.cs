using System;

namespace FistVR
{
	[Serializable]
	public class QuickbeltOptions
	{
		public enum ObjectToHandConnectionMode
		{
			Direct,
			Floating
		}

		public enum BoltActionMode
		{
			Quickbolting,
			Slidebolting
		}

		public int QuickbeltPreset;

		public int QuickbeltHandedness;

		public bool AreBulletTrailsEnabled;

		public bool HideControllerGeoWhenObjectHeld = true;

		public ObjectToHandConnectionMode ObjectToHandMode;

		public int TrailDecaySetting = 3;

		public float[] TrailDecayTimes = new float[5]
		{
			0.25f,
			0.5f,
			1f,
			5f,
			60f
		};

		public BoltActionMode BoltActionModeSetting;

		public void InitializeFromSaveFile(ES2Reader reader)
		{
			if (reader.TagExists("QuickbeltPreset"))
			{
				QuickbeltPreset = reader.Read<int>("QuickbeltPreset");
			}
			if (reader.TagExists("TrailDecaySetting"))
			{
				TrailDecaySetting = reader.Read<int>("TrailDecaySetting");
			}
			if (reader.TagExists("QuickbeltHandedness"))
			{
				QuickbeltHandedness = reader.Read<int>("QuickbeltHandedness");
			}
			if (reader.TagExists("AreBulletTrailsEnabled"))
			{
				AreBulletTrailsEnabled = reader.Read<bool>("AreBulletTrailsEnabled");
			}
			if (reader.TagExists("HideControllerGeoWhenObjectHeld"))
			{
				HideControllerGeoWhenObjectHeld = reader.Read<bool>("HideControllerGeoWhenObjectHeld");
			}
			if (reader.TagExists("ObjectToHandMode"))
			{
				ObjectToHandMode = reader.Read<ObjectToHandConnectionMode>("ObjectToHandMode");
			}
			if (reader.TagExists("BoltActionModeSetting"))
			{
				BoltActionModeSetting = reader.Read<BoltActionMode>("BoltActionModeSetting");
			}
		}

		public void SaveToFile(ES2Writer writer)
		{
			writer.Write(QuickbeltPreset, "QuickbeltPreset");
			writer.Write(QuickbeltHandedness, "QuickbeltHandedness");
			writer.Write(TrailDecaySetting, "TrailDecaySetting");
			writer.Write(AreBulletTrailsEnabled, "AreBulletTrailsEnabled");
			writer.Write(HideControllerGeoWhenObjectHeld, "HideControllerGeoWhenObjectHeld");
			writer.Write(ObjectToHandMode, "ObjectToHandMode");
			writer.Write(BoltActionModeSetting, "BoltActionModeSetting");
		}
	}
}
