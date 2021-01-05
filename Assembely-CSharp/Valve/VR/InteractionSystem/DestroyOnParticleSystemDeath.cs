using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(ParticleSystem))]
	public class DestroyOnParticleSystemDeath : MonoBehaviour
	{
		private ParticleSystem particles;

		private void Awake()
		{
			particles = GetComponent<ParticleSystem>();
			InvokeRepeating("CheckParticleSystem", 0.1f, 0.1f);
		}

		private void CheckParticleSystem()
		{
			if (!particles.IsAlive())
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
