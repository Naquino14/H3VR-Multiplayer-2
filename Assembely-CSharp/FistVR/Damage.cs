using System;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class Damage
	{
		public enum DamageClass
		{
			Abstract,
			Projectile,
			Explosive,
			Melee,
			Environment
		}

		public DamageClass Class;

		public float Dam_Blunt;

		public float Dam_Piercing;

		public float Dam_Cutting;

		public float Dam_TotalKinetic;

		public float Dam_Thermal;

		public float Dam_Chilling;

		public float Dam_EMP;

		public float Dam_TotalEnergetic;

		public float Dam_Stunning;

		public float Dam_Blinding;

		public Vector3 point;

		public Vector3 hitNormal;

		public Vector3 strikeDir;

		public Vector3 edgeNormal = Vector3.zero;

		public float damageSize;

		public int Source_IFF = -1;

		public Transform Source_Transform;

		public Vector3 Source_Point;
	}
}
