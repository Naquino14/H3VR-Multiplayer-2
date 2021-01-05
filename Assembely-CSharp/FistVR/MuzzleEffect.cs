using System;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class MuzzleEffect
	{
		[Header("NewParams")]
		public MuzzleEffectEntry Entry;

		public MuzzleEffectSize Size;

		public Transform OverridePoint;
	}
}
