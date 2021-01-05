using System;
using UnityEngine;

namespace AmplifyColor
{
	[Serializable]
	public class VersionInfo
	{
		public const byte Major = 1;

		public const byte Minor = 6;

		public const byte Release = 6;

		private static string StageSuffix = "_dev002";

		private static string TrialSuffix = string.Empty;

		[SerializeField]
		private int m_major;

		[SerializeField]
		private int m_minor;

		[SerializeField]
		private int m_release;

		public int Number => m_major * 100 + m_minor * 10 + m_release;

		private VersionInfo()
		{
			m_major = 1;
			m_minor = 6;
			m_release = 6;
		}

		private VersionInfo(byte major, byte minor, byte release)
		{
			m_major = major;
			m_minor = minor;
			m_release = release;
		}

		public static string StaticToString()
		{
			return $"{(byte)1}.{(byte)6}.{(byte)6}" + StageSuffix + TrialSuffix;
		}

		public override string ToString()
		{
			return $"{m_major}.{m_minor}.{m_release}" + StageSuffix + TrialSuffix;
		}

		public static VersionInfo Current()
		{
			return new VersionInfo(1, 6, 6);
		}

		public static bool Matches(VersionInfo version)
		{
			return version.m_major == 1 && version.m_minor == 6 && 6 == version.m_release;
		}
	}
}
