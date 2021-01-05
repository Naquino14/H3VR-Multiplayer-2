using System;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Definition", menuName = "ModularRange/SequenceDefinition", order = 0)]
	public class ModularRangeSequenceDefinition : ScriptableObject
	{
		public enum TargetLayout
		{
			HorizontalLeft,
			HorizontalRight,
			VerticalUp,
			VerticalDown,
			DiagonalLeftUp,
			DiagonalRightUp,
			DiagonalLeftDown,
			DiagonalRightDown,
			SquareUp,
			SquareDown,
			CircleClockWise,
			CircleCounterClockWise
		}

		public enum TargetTiming
		{
			SequentialOnHit,
			SequentialTimed,
			RandomOnHit,
			RandomTimed,
			Flood
		}

		public enum TargetMovementStyle
		{
			Static,
			SinusoidX,
			SinusoidY,
			WhipX,
			WhipY,
			WhipZ,
			TowardCenterWhipZ,
			RandomDir,
			TowardCenter,
			WhipXWhipZ,
			WhipYWhipZ
		}

		[Serializable]
		public class WaveDefinition
		{
			public GameObject[] TargetPrefabs;

			public GameObject[] NoShootTargetPrefabs;

			public TargetLayout Layout;

			public TargetTiming Timing;

			public TargetMovementStyle MovementStyle;

			public int TargetNum;

			public int NumNoShootTarget;

			public float Distance;

			public float EndDistance;

			public float TimeForWave;

			public float TimePerTarget;

			public float DelayPerTarget;

			public float TimeForReload;
		}

		public enum SequenceCategory
		{
			Reflex,
			Recognition,
			Precision,
			Cognition
		}

		public enum SequenceDifficulty
		{
			Easy,
			Intermediate,
			Advanced,
			Nightmarish
		}

		public enum SequenceRange
		{
			Short,
			Medium,
			Long,
			Mixed
		}

		[Serializable]
		public class SequenceMetaData
		{
			public string DisplayName;

			public string EndPointName;

			public SequenceCategory Category;

			public SequenceDifficulty Difficulty;

			public string Capacity;

			public int WaveCount;

			public SequenceRange Range;
		}

		public SequenceMetaData MetaData;

		public WaveDefinition[] Waves;
	}
}
