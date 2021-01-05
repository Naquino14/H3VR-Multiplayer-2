using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu]
	public class MatDef : ScriptableObject
	{
		public MatBallisticType BallisticType;

		public MatSoundType SoundType;

		public BallisticImpactEffectType ImpactEffectType;

		public BulletHoleDecalType BulletHoleType;

		public BulletImpactSoundType BulletImpactSound;
	}
}
