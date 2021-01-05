using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New AudioImpactSet", menuName = "AudioPooling/AudioImpactSet", order = 0)]
	public class AudioImpactSet : ScriptableObject
	{
		public ImpactType ImpactType;

		public Vector2 PitchRange = new Vector2(1f, 1.03f);

		public AudioImpactMaterialGroup Carpet;

		public AudioImpactMaterialGroup HardSurface;

		public AudioImpactMaterialGroup LooseSurface;

		public AudioImpactMaterialGroup Meat;

		public AudioImpactMaterialGroup Metal;

		public AudioImpactMaterialGroup Plastic;

		public AudioImpactMaterialGroup SoftSurface;

		public AudioImpactMaterialGroup Tile;

		public AudioImpactMaterialGroup Water;

		public AudioImpactMaterialGroup Wood;
	}
}
