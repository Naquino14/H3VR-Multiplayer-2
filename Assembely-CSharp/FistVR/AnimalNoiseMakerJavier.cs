using UnityEngine;

namespace FistVR
{
	public class AnimalNoiseMakerJavier : FVRPhysicalObject
	{
		private bool m_isPrimed;

		private bool m_hasActivated;

		private bool m_hasSploded;

		public Transform[] CandleSpot;

		public GameObject[] CandlePrefabs;

		private float timeSinceSpawn = 10.5f;

		private float timeSinceFlash = 15f;

		private float timeTilSplode = 20.9f;

		public GameObject Flame;

		public GameObject[] SPlodes;

		private float curPos = 0.35f;

		private Vector3 m_curPos = Vector3.zero;

		private float m_startScale = 0.35f;

		private Vector3 m_startPos = Vector3.zero;

		private float m_endScale = 7f;

		private Vector3 m_endPos = Vector3.zero;

		private float m_transitionTick;

		private Vector3 Wiggledir = Vector3.zero;

		public override bool IsInteractable()
		{
			if (m_hasActivated)
			{
				return false;
			}
			return base.IsInteractable();
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (Vector3.Dot(base.transform.up, Vector3.up) < -0.1f && !m_isPrimed)
			{
				m_isPrimed = true;
			}
			if (Vector3.Dot(base.transform.up, Vector3.up) > 0.3f && m_isPrimed && !m_hasActivated && base.IsHeld)
			{
				m_hasActivated = true;
				BeginSequence();
			}
			if (!m_hasActivated)
			{
				return;
			}
			m_transitionTick += 0.1f * Time.deltaTime;
			base.transform.position = Vector3.Lerp(m_startPos, m_endPos, m_transitionTick);
			float num = Mathf.Lerp(m_startScale, m_endScale, m_transitionTick);
			base.transform.localScale = new Vector3(num, num, num);
			Vector3 vector = Vector3.Cross(Wiggledir, Vector3.up);
			Vector3 vector2 = Vector3.Lerp(vector, -vector, 1f - Mathf.Abs(Mathf.Sin(m_transitionTick * 20f)));
			Vector3 upwards = Vector3.up + vector2;
			base.transform.rotation = Quaternion.LookRotation(-Wiggledir, upwards);
			timeSinceSpawn -= Time.deltaTime;
			if (timeSinceSpawn <= 0f)
			{
				timeSinceSpawn = Random.Range(0.75f, 1f);
				Object.Instantiate(CandlePrefabs[Random.Range(0, CandlePrefabs.Length)], base.transform.position + Vector3.up * Random.Range(5f, 10f) + Random.onUnitSphere * 4f, Quaternion.identity);
			}
			timeSinceFlash -= Time.deltaTime;
			if (timeSinceFlash <= 0f)
			{
				timeSinceFlash = Random.Range(0.025f, 0.075f);
				FXM.InitiateMuzzleFlash(base.transform.position + Random.onUnitSphere, Vector3.up, Random.Range(0.8f, 5f), Color.white, Random.Range(2f, 6f));
			}
			if (m_transitionTick > 0.2f && !Flame.activeSelf)
			{
				Flame.SetActive(value: true);
			}
			timeTilSplode -= Time.deltaTime;
			if (timeTilSplode <= 0f && !m_hasSploded)
			{
				m_hasSploded = true;
				for (int i = 0; i < SPlodes.Length; i++)
				{
					Object.Instantiate(SPlodes[i], base.transform.position, Quaternion.identity);
				}
				Object.Destroy(base.gameObject);
			}
		}

		private void BeginSequence()
		{
			FVRViveHand hand = m_hand;
			EndInteraction(hand);
			hand.ForceSetInteractable(null);
			m_startPos = base.transform.position;
			base.RootRigidbody.isKinematic = true;
			Vector3 vector = base.transform.position - Camera.main.transform.position;
			vector.y = 0f;
			Wiggledir = vector;
			vector = vector.normalized * 2f;
			m_endPos = m_startPos + vector;
			GetComponent<AudioSource>().Play();
		}
	}
}
