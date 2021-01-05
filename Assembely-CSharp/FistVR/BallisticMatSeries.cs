using System;

namespace FistVR
{
	[Serializable]
	public class BallisticMatSeries
	{
		public MatBallisticType MaterialType;

		public float PenThreshold;

		public float ShatterThreshold;

		public float Absorption;

		public float RicochetLimit;

		public float MinAngularAbsord;

		public float Roughness;

		public bool StopsOnPen;

		public bool DownGradesOnPen;

		public BallisticProjectileType DownGradesTo;
	}
}
