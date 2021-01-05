using UnityEngine;

namespace FistVR
{
	public class MG_BurningMeatBall : MonoBehaviour, IFVRDamageable
	{
		private Rigidbody RB;

		public Transform TargOverride;

		private bool m_isExploded;

		public GameObject[] Spawns;

		private float Life = 2500f;

		public AudioSource Source;

		public AudioLowPassFilter LowPassFilter;

		public AnimationCurve OcclusionFactorCurve;

		public AnimationCurve OcclusionVolumeCurve;

		public LayerMask OcclusionLM;

		private float fuse = 1f;

		private void Awake()
		{
			RB = GetComponent<Rigidbody>();
		}

		private void FixedUpdate()
		{
			Tick();
		}

		private float GetLowPassOcclusionValue(Vector3 start, Vector3 end)
		{
			if (Physics.Linecast(start, end, OcclusionLM, QueryTriggerInteraction.Ignore))
			{
				float time = Vector3.Distance(start, end);
				Source.volume = 0.3f * OcclusionVolumeCurve.Evaluate(time);
				return OcclusionFactorCurve.Evaluate(time);
			}
			return 22000f;
		}

		private void Tick()
		{
			if (GM.CurrentPlayerBody == null)
			{
				return;
			}
			float lowPassOcclusionValue = GetLowPassOcclusionValue(base.transform.position, GM.CurrentPlayerBody.Head.position);
			LowPassFilter.cutoffFrequency = Mathf.MoveTowards(LowPassFilter.cutoffFrequency, lowPassOcclusionValue, Time.deltaTime * 20000f);
			Vector3 position = GM.CurrentPlayerBody.transform.position;
			float num = Vector3.Distance(position, base.transform.position);
			if (num > 20f)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			Vector3 vector = position - base.transform.position;
			vector.Normalize();
			Vector3 vector2 = -Vector3.Cross(vector, Vector3.up);
			RB.angularVelocity = Vector3.Lerp(RB.angularVelocity, vector2 * 8f, Time.deltaTime * 20f);
			RB.AddForce(vector * 3f, ForceMode.Acceleration);
			if (num < 1.2f)
			{
				fuse -= Time.deltaTime;
				if (fuse < 0f)
				{
					Explode();
				}
			}
		}

		public void Damage(Damage d)
		{
			Life -= d.Dam_TotalKinetic;
			if (Life <= 0f)
			{
				Explode();
			}
		}

		private void Explode()
		{
			if (!m_isExploded)
			{
				m_isExploded = true;
				for (int i = 0; i < Spawns.Length; i++)
				{
					Object.Instantiate(Spawns[i], base.transform.position, base.transform.rotation);
				}
				Object.Destroy(base.gameObject);
			}
		}
	}
}
