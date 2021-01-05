using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class BalloonSpawner : MonoBehaviour
	{
		public float minSpawnTime = 5f;

		public float maxSpawnTime = 15f;

		private float nextSpawnTime;

		public GameObject balloonPrefab;

		public bool autoSpawn = true;

		public bool spawnAtStartup = true;

		public bool playSounds = true;

		public SoundPlayOneshot inflateSound;

		public SoundPlayOneshot stretchSound;

		public bool sendSpawnMessageToParent;

		public float scale = 1f;

		public Transform spawnDirectionTransform;

		public float spawnForce;

		public bool attachBalloon;

		public Balloon.BalloonColor color = Balloon.BalloonColor.Random;

		private void Start()
		{
			if (!(balloonPrefab == null) && autoSpawn && spawnAtStartup)
			{
				SpawnBalloon(color);
				nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime) + Time.time;
			}
		}

		private void Update()
		{
			if (!(balloonPrefab == null) && Time.time > nextSpawnTime && autoSpawn)
			{
				SpawnBalloon(color);
				nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime) + Time.time;
			}
		}

		public GameObject SpawnBalloon(Balloon.BalloonColor color = Balloon.BalloonColor.Red)
		{
			if (balloonPrefab == null)
			{
				return null;
			}
			GameObject gameObject = Object.Instantiate(balloonPrefab, base.transform.position, base.transform.rotation);
			gameObject.transform.localScale = new Vector3(scale, scale, scale);
			if (attachBalloon)
			{
				gameObject.transform.parent = base.transform;
			}
			if (sendSpawnMessageToParent && base.transform.parent != null)
			{
				base.transform.parent.SendMessage("OnBalloonSpawned", gameObject, SendMessageOptions.DontRequireReceiver);
			}
			if (playSounds)
			{
				if (inflateSound != null)
				{
					inflateSound.Play();
				}
				if (stretchSound != null)
				{
					stretchSound.Play();
				}
			}
			gameObject.GetComponentInChildren<Balloon>().SetColor(color);
			if (spawnDirectionTransform != null)
			{
				gameObject.GetComponentInChildren<Rigidbody>().AddForce(spawnDirectionTransform.forward * spawnForce);
			}
			return gameObject;
		}

		public void SpawnBalloonFromEvent(int color)
		{
			SpawnBalloon((Balloon.BalloonColor)color);
		}
	}
}
