using UnityEngine;

namespace FistVR
{
	public class FlintlockPowderHornCap : FVRInteractiveObject
	{
		public FlintlockPowderHorn Horn;

		public bool UsesPourTrigger;

		public Transform PourTriggerUp;

		public Transform OverflowPoint;

		public GameObject PowderPrefab;

		public Transform PowderSpawnPoint;

		public AudioEvent AudEvent_PowderIn;

		private int m_numGrains;

		public Renderer Powder;

		public Vector3 Pos_Empty;

		public Vector3 Pos_Full;

		public Vector3 Scale_Empty;

		public Vector3 Scale_Full;

		private float m_insertSoundRefire = 0.2f;

		private float timeSinceSpawn;

		public bool IsFull()
		{
			if (m_numGrains >= 20)
			{
				return true;
			}
			return false;
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			Horn.ToggleCap();
		}

		protected override void Awake()
		{
			base.Awake();
			UpdateViz();
		}

		public void OnTriggerEnter(Collider other)
		{
			if (!UsesPourTrigger || other.attachedRigidbody == null || Horn.IsCapped)
			{
				return;
			}
			float num = Vector3.Angle(PourTriggerUp.forward, Vector3.up);
			if (num > 90f)
			{
				return;
			}
			GameObject gameObject = other.attachedRigidbody.gameObject;
			if (gameObject.CompareTag("flintlock_powdergrain"))
			{
				if (m_insertSoundRefire > 0.15f)
				{
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_PowderIn, base.transform.position);
				}
				AddGrain();
				Object.Destroy(other.gameObject);
			}
		}

		private void AddGrain()
		{
			m_numGrains++;
			if (m_numGrains > 20)
			{
				SpawnOverflow();
			}
			UpdateViz();
		}

		private void UpdateViz()
		{
			float t = (float)m_numGrains / 20f;
			if (Powder != null)
			{
				Powder.transform.localPosition = Vector3.Lerp(Pos_Empty, Pos_Full, t);
				Powder.transform.localScale = Vector3.Lerp(Scale_Empty, Scale_Full, t);
			}
		}

		private void SpawnOverflow()
		{
			m_numGrains--;
			Object.Instantiate(PowderPrefab, OverflowPoint.position, Random.rotation);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_insertSoundRefire < 0.2f)
			{
				m_insertSoundRefire += Time.deltaTime;
			}
			if (timeSinceSpawn < 1f)
			{
				timeSinceSpawn += Time.deltaTime;
			}
			if (m_numGrains > 0 && !Horn.IsCapped)
			{
				float num = Vector3.Angle(PourTriggerUp.forward, Vector3.up);
				if (num > 105f && timeSinceSpawn > 0.04f)
				{
					m_numGrains--;
					timeSinceSpawn = 0f;
					Object.Instantiate(PowderPrefab, PowderSpawnPoint.position, Random.rotation);
					UpdateViz();
				}
			}
		}
	}
}
