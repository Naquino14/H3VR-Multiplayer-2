using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New RecticleColorProg", menuName = "Attachments/ReticleColorProgression", order = 0)]
	public class ReticleColorProgression : ScriptableObject
	{
		[ColorUsage(true, true, 0f, 30f, 0.125f, 3f)]
		public List<Color> Colors;
	}
}
