using System;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace LIV.SDK.Unity
{
	public static class Calibration
	{
		private const string ConfigFileName = "externalcamera.cfg";

		public static float X;

		public static float Y;

		public static float Z;

		public static float Yaw;

		public static float Pitch;

		public static float Roll;

		public static float FieldOfVision;

		public static float NearClip;

		public static float FarClip;

		public static float HMDOffset;

		public static float NearOffset;

		private static readonly FileSystemWatcher ConfigWatcher;

		public static Vector3 PositionOffset => new Vector3(X, Y, Z);

		public static Quaternion RotationOffset => Quaternion.Euler(Pitch, Yaw, Roll);

		public static event EventHandler Changed;

		static Calibration()
		{
			try
			{
				FileInfo fileInfo = new FileInfo("externalcamera.cfg");
				ConfigWatcher = new FileSystemWatcher(fileInfo.DirectoryName ?? string.Empty, fileInfo.Name)
				{
					NotifyFilter = NotifyFilters.LastWrite
				};
				ConfigWatcher.Changed += delegate
				{
					Read();
				};
				ConfigWatcher.EnableRaisingEvents = true;
			}
			catch
			{
			}
			Read();
		}

		private static void Reset()
		{
			X = (Y = (Z = 0f));
			Pitch = (Yaw = (Roll = 0f));
			FieldOfVision = 60f;
			NearClip = 0.01f;
			FarClip = 1000f;
			HMDOffset = 0f;
			NearOffset = 0f;
		}

		public static void Read()
		{
			Reset();
			string[] array = new string[0];
			try
			{
				array = File.ReadAllLines("externalcamera.cfg");
			}
			catch (Exception ex)
			{
				Debug.LogWarningFormat("LIV: Failed to read camera calibration from disk. Error: {0}", ex);
			}
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split(new char[1]
				{
					'='
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array3.Length == 2)
				{
					switch (array3[0].ToLowerInvariant())
					{
					case "x":
						TryParseInvariantFloat(array3[1], out X);
						break;
					case "y":
						TryParseInvariantFloat(array3[1], out Y);
						break;
					case "z":
						TryParseInvariantFloat(array3[1], out Z);
						break;
					case "rx":
						TryParseInvariantFloat(array3[1], out Pitch);
						break;
					case "ry":
						TryParseInvariantFloat(array3[1], out Yaw);
						break;
					case "rz":
						TryParseInvariantFloat(array3[1], out Roll);
						break;
					case "fov":
						TryParseInvariantFloat(array3[1], out FieldOfVision);
						break;
					case "near":
						TryParseInvariantFloat(array3[1], out NearClip);
						break;
					case "far":
						TryParseInvariantFloat(array3[1], out FarClip);
						break;
					case "hmdoffset":
						TryParseInvariantFloat(array3[1], out HMDOffset);
						break;
					case "nearoffset":
						TryParseInvariantFloat(array3[1], out NearOffset);
						break;
					}
				}
			}
			if (Calibration.Changed != null)
			{
				Calibration.Changed(null, EventArgs.Empty);
			}
		}

		private static bool TryParseInvariantFloat(string number, out float result)
		{
			return float.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
		}
	}
}
