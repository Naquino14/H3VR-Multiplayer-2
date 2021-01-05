using System;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class MuzzlePSystem
	{
		public ParticleSystem PSystem;

		public int NumParticlesPerShot;

		public Transform OverridePoint;
	}
}
