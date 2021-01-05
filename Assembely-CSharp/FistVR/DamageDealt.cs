using System;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public struct DamageDealt
	{
		public enum DamageType
		{
			None,
			Pistol,
			Shotgun,
			SMGRifle,
			Support,
			Explosive,
			Melee,
			Trap
		}

		public Vector3 force;

		public float MPa;

		public float MPaRootMeter;

		public float PointsDamage;

		public float StunDamage;

		public Vector3 point;

		public Vector3 hitNormal;

		public Vector3 strikeDir;

		public Vector2 uvCoords;

		public bool IsInitialContact;

		public bool IsInside;

		public bool IsMelee;

		public bool DoesIgnite;

		public bool DoesFreeze;

		public bool DoesDisrupt;

		public bool IsPlayer;

		public DamageType Type;

		public Transform ShotOrigin;

		public FVRFireArm SourceFirearm;
	}
}
