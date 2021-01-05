using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Muzzle Effect Config", menuName = "Firearms/MuzzleEffect", order = 0)]
	public class MuzzleEffectConfig : ScriptableObject
	{
		public MuzzleEffectEntry Entry;

		public List<GameObject> Prefabs_Highlight;

		public List<GameObject> Prefabs_Lowlight;

		public List<int> NumParticles_Highlight;

		public List<int> NumParticles_Lowlight;
	}
}
