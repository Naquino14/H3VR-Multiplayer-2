using UnityEngine;

namespace FistVR
{
	public class SmokeSolidEmitter : MonoBehaviour
	{
		public bool Engaged = true;

		public GameObject[] SmokePrefabs;

		public Vector2 DecayRange;

		public Vector2 TickToNextSmokeRange;

		public Vector2 VelRange;

		public int NumSmokeLeftToSpawn = 20;

		private float tickToNextSmoke;

		private void Start()
		{
		}

		private void Update()
		{
			if (!Engaged)
			{
				return;
			}
			if (tickToNextSmoke > 0f)
			{
				tickToNextSmoke -= Time.deltaTime;
				return;
			}
			tickToNextSmoke = Random.Range(TickToNextSmokeRange.x, TickToNextSmokeRange.y);
			NumSmokeLeftToSpawn--;
			if (NumSmokeLeftToSpawn > 0)
			{
				SpawnSmoke();
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void SpawnSmoke()
		{
			GameObject gameObject = Object.Instantiate(SmokePrefabs[Random.Range(0, SmokePrefabs.Length)], base.transform.position + Random.onUnitSphere * 0.05f, Random.rotation);
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			component.velocity = (Random.onUnitSphere + Vector3.up) * Random.Range(VelRange.x, VelRange.y);
			SmokeSolid component2 = gameObject.GetComponent<SmokeSolid>();
			component2.DecaySpeed = Random.Range(DecayRange.x, DecayRange.y);
		}
	}
}
