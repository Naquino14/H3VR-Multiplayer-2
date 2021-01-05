using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu]
	public class BallisticChart : ScriptableObject
	{
		public BallisticProjectileType ProjectileType;

		public List<BallisticMatSeries> Mats;
	}
}
