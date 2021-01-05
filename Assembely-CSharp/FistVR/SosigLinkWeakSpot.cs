using UnityEngine;

namespace FistVR
{
	public class SosigLinkWeakSpot : MonoBehaviour, IFVRDamageable
	{
		public SosigLink L;

		public bool RandomPlace = true;

		public float Radius;

		public float HeightRange;

		public void Awake()
		{
			if (RandomPlace)
			{
				Vector3 onUnitSphere = Random.onUnitSphere;
				onUnitSphere.y = 0f;
				onUnitSphere.Normalize();
				onUnitSphere *= Radius;
				base.transform.localPosition = new Vector3(onUnitSphere.x, Random.Range(0f - HeightRange, HeightRange), onUnitSphere.z);
			}
		}

		public void Damage(Damage D)
		{
			if (D.Class != FistVR.Damage.DamageClass.Explosive)
			{
				L.LinkExplodes(D.Class);
			}
		}
	}
}
