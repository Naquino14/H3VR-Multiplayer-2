using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu]
	public class PMaterialDefinition : ScriptableObject
	{
		public PMaterial material;

		public float yieldStrength;

		public float roughness;

		public float stiffness;

		public float density;

		public float bounciness;

		public float toughness;

		public PMatSoundCategory soundCategory;

		public PMatImpactEffectCategory impactCategory;
	}
}
