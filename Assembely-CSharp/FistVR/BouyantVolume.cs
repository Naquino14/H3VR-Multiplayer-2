using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class BouyantVolume : MonoBehaviour
	{
		public HashSet<Rigidbody> ContainedObjectsHash = new HashSet<Rigidbody>();

		public List<Rigidbody> ContainedObjectsList = new List<Rigidbody>();

		public Dictionary<Rigidbody, float> OgDrag = new Dictionary<Rigidbody, float>();

		public Dictionary<Rigidbody, float> OgAngDrag = new Dictionary<Rigidbody, float>();

		private int testIndex;

		public float SurfaceOfWaterY = 2f;

		private Color FogColor;

		private float FogDensity;

		public Color WaterFogColor;

		public float WaterFogDensity;

		private bool m_isPlayerInWater;

		private Bounds waterBounds;

		public bool UsesImpactSplash;

		private void Awake()
		{
			FogColor = RenderSettings.fogColor;
			FogDensity = RenderSettings.fogDensity;
			waterBounds = GetComponent<BoxCollider>().bounds;
		}

		private void FixedUpdate()
		{
			TestForPlayerHead();
			TestingLoop();
		}

		private void TestForPlayerHead()
		{
			bool flag = waterBounds.Contains(GM.CurrentPlayerBody.Head.position);
			if (m_isPlayerInWater && !flag)
			{
				PlayerExitedWater();
			}
			else if (!m_isPlayerInWater && flag)
			{
				PlayerEnteredWater();
			}
		}

		private void PlayerEnteredWater()
		{
			m_isPlayerInWater = true;
			RenderSettings.fogColor = WaterFogColor;
			RenderSettings.fogDensity = WaterFogDensity;
		}

		private void PlayerExitedWater()
		{
			m_isPlayerInWater = false;
			RenderSettings.fogColor = FogColor;
			RenderSettings.fogDensity = FogDensity;
		}

		private void TestingLoop()
		{
			int count = ContainedObjectsList.Count;
			if (count <= 0)
			{
				return;
			}
			for (int num = ContainedObjectsList.Count - 1; num >= 0; num--)
			{
				if (ContainedObjectsList[num] == null)
				{
					OgDrag.Remove(ContainedObjectsList[num]);
					OgAngDrag.Remove(ContainedObjectsList[num]);
					ContainedObjectsHash.Remove(ContainedObjectsList[num]);
					ContainedObjectsList.RemoveAt(num);
				}
				else
				{
					TestRigidbody(ContainedObjectsList[num]);
				}
			}
		}

		private void OnTriggerEnter(Collider col)
		{
			TestColliderEnter(col);
		}

		private void OnTriggerExit(Collider col)
		{
			TestColliderExit(col);
		}

		private void TestColliderEnter(Collider col)
		{
			if (col.attachedRigidbody == null)
			{
				return;
			}
			Rigidbody attachedRigidbody = col.attachedRigidbody;
			if (!ContainedObjectsHash.Add(attachedRigidbody))
			{
				return;
			}
			ContainedObjectsList.Add(attachedRigidbody);
			if (!OgDrag.ContainsKey(attachedRigidbody))
			{
				OgDrag.Add(attachedRigidbody, attachedRigidbody.drag);
			}
			if (!OgAngDrag.ContainsKey(attachedRigidbody))
			{
				OgAngDrag.Add(attachedRigidbody, attachedRigidbody.angularDrag);
			}
			FVRPhysicalObject component = attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
			if (component != null)
			{
				component.IsInWater = true;
				Vector3 pos = new Vector3(component.transform.position.x, SurfaceOfWaterY, component.transform.position.z);
				if (component.HasImpactController)
				{
					SM.PlayImpactSound(component.AudioImpactController.ImpactType, MatSoundType.Water, AudioImpactIntensity.Hard, pos, FVRPooledAudioType.Impacts, 20f);
				}
				else
				{
					SM.PlayImpactSound(ImpactType.Generic, MatSoundType.Water, AudioImpactIntensity.Hard, pos, FVRPooledAudioType.Impacts, 20f);
				}
				if (component is FVRFireArmRound)
				{
					FXM.SpawnImpactEffect(pos, Vector3.up, 3, ImpactEffectMagnitude.Medium, forwardBack: true);
				}
				else
				{
					FXM.SpawnImpactEffect(pos, Vector3.up, 3, ImpactEffectMagnitude.Large, forwardBack: true);
				}
			}
			attachedRigidbody.drag = 5f;
			attachedRigidbody.angularDrag = 5f;
		}

		private void TestColliderExit(Collider col)
		{
			if (col.attachedRigidbody == null)
			{
				return;
			}
			Rigidbody attachedRigidbody = col.attachedRigidbody;
			if (ContainedObjectsHash.Remove(attachedRigidbody))
			{
				ContainedObjectsList.Remove(attachedRigidbody);
				FVRPhysicalObject component = attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
				if (component != null)
				{
					component.IsInWater = false;
				}
				if (OgDrag.ContainsKey(attachedRigidbody))
				{
					col.attachedRigidbody.drag = OgDrag[attachedRigidbody];
				}
				if (OgAngDrag.ContainsKey(attachedRigidbody))
				{
					col.attachedRigidbody.angularDrag = OgAngDrag[attachedRigidbody];
				}
			}
		}

		private void TestRigidbody(Rigidbody rb)
		{
			Vector3 worldCenterOfMass = rb.worldCenterOfMass;
			float num = worldCenterOfMass.y - SurfaceOfWaterY;
			if (num < 0f)
			{
				Vector3 force = -13f * Mathf.Abs(Physics.gravity.y) * Mathf.Clamp(num, -2f, 2f) * Vector3.up;
				force.x = 0f;
				force.z = 0f;
				rb.AddForceAtPosition(force, worldCenterOfMass, ForceMode.Acceleration);
				Vector3 position = rb.transform.position;
				float num2 = Time.time * (Mathf.PerlinNoise(Time.time * 0.2f, position.y) * 0.2f + 0.1f);
				float num3 = Mathf.PerlinNoise(position.x + num2, position.z + num2);
				float num4 = Mathf.PerlinNoise(position.x + 0.1f - num2, position.z + num2);
				float num5 = Mathf.PerlinNoise(position.x + num2, position.z + 0.1f - num2);
				Vector2 vector = new Vector2(num4 - num3, num5 - num3);
				Vector3 force2 = new Vector3(vector.x, 0f, vector.y) * 3f;
				rb.AddForce(force2, ForceMode.Acceleration);
				Vector3 vector2 = new Vector3(vector.x, 0f, vector.y) * 1f;
				rb.AddTorque(vector2 * num2 * 0.002f);
			}
		}
	}
}
