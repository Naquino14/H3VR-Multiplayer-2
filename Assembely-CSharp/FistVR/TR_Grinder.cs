using UnityEngine;

namespace FistVR
{
	public class TR_Grinder : MonoBehaviour
	{
		public TR_GrinderContoller GController;

		public Transform TopGrinder;

		public Transform BottomGrinder;

		private float m_curRot1;

		private float m_curRot2 = 45f;

		private float m_curSpeed;

		private float m_tarSpeed;

		private float m_transitionMult = 1f;

		private float m_MaxSpinSpeed = 1000f;

		public ParticleSystem Sparks;

		private ParticleSystem.EmitParams emitParams;

		public Transform ParticlePoint1;

		public Transform ParticlePoint2;

		public GameObject[] Triggers;

		public TR_GrinderDamageTrigger[] DamageTriggers;

		public GameObject[] SmokePEffects;

		public GameObject[] FirePEffects;

		private void Start()
		{
			StartSpinning();
		}

		public void SetGController(TR_GrinderContoller c)
		{
			GController = c;
			for (int i = 0; i < DamageTriggers.Length; i++)
			{
				DamageTriggers[i].GController = c;
			}
		}

		public void StartSpinning()
		{
			m_tarSpeed = m_MaxSpinSpeed;
			for (int i = 0; i < Triggers.Length; i++)
			{
				Triggers[i].SetActive(value: true);
			}
		}

		public void StopSpinning()
		{
			m_tarSpeed = 0f;
			m_transitionMult = 2f;
			for (int i = 0; i < Triggers.Length; i++)
			{
				Triggers[i].SetActive(value: false);
			}
		}

		public void EmitEvent(Vector3 point)
		{
			FXM.InitiateMuzzleFlash(point, Sparks.transform.forward, Random.Range(0.25f, 1f), Color.white, Random.Range(0.25f, 1.5f));
			for (int i = 0; i < Random.Range(15, 20); i++)
			{
				Vector3 velocity = Sparks.transform.forward * Random.Range(2f, 8f) * 1f;
				velocity += Random.onUnitSphere * 12f;
				emitParams.position = point;
				emitParams.velocity = velocity;
				Sparks.Emit(emitParams, 1);
			}
		}

		private void Update()
		{
			m_curSpeed = Mathf.Lerp(m_curSpeed, m_tarSpeed, Time.deltaTime * m_transitionMult);
			m_curRot1 += m_curSpeed * Time.deltaTime;
			m_curRot1 = Mathf.Repeat(m_curRot1, 360f);
			m_curRot2 += m_curSpeed * Time.deltaTime;
			m_curRot2 = Mathf.Repeat(m_curRot2, 360f);
			TopGrinder.localEulerAngles = new Vector3(0f, 0f, m_curRot1);
			BottomGrinder.localEulerAngles = new Vector3(0f, 0f, 0f - m_curRot2);
			float num = m_curSpeed / m_MaxSpinSpeed;
			if (num > 0.1f && Random.Range(0, 10) == 0)
			{
				Vector3 vector = Vector3.Lerp(ParticlePoint1.position, ParticlePoint2.position, Random.Range(0f, 1f));
				FXM.InitiateMuzzleFlash(vector, Sparks.transform.forward, Random.Range(0.25f, 1f), Color.white, Random.Range(0.25f, 1.5f));
				for (int i = 0; i < Random.Range(3, 9); i++)
				{
					Vector3 velocity = Sparks.transform.forward * Random.Range(2f, 15f) * num;
					velocity += Random.onUnitSphere * 6f;
					emitParams.position = vector;
					emitParams.velocity = velocity;
					Sparks.Emit(emitParams, 1);
				}
			}
		}
	}
}
