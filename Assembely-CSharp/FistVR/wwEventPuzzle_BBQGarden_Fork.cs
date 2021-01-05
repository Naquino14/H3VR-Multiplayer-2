using UnityEngine;

namespace FistVR
{
	public class wwEventPuzzle_BBQGarden_Fork : MonoBehaviour, IFVRDamageable
	{
		public wwEventPuzzle_BBQGarden Garden;

		public bool isFork;

		public int ForkIndex;

		public AudioSource Aud;

		private float timeSincePlay;

		public void Damage(Damage dam)
		{
			if (dam.Class == FistVR.Damage.DamageClass.Projectile && timeSincePlay > 0.2f)
			{
				Aud.PlayOneShot(Aud.clip, 0.4f);
				if (isFork)
				{
					Garden.ForkHit(ForkIndex);
				}
				else
				{
					Aud.pitch = Random.Range(0.97f, 1.03f);
				}
			}
		}

		private void Update()
		{
			if (timeSincePlay < 2f)
			{
				timeSincePlay += Time.deltaTime;
			}
		}
	}
}
