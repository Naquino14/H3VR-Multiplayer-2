using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TeslaTrap : ZosigQuestManager
	{
		public List<GameObject> LightningFX;

		public Transform LightningOrigin;

		public LayerMask LM_SosigDetect;

		public float range;

		private bool m_isEnabled;

		public AudioSource Buzzing;

		public GameObject AmbientVFX;

		private ZosigGameManager M;

		public string Flag;

		public int ValueWhenOn;

		public AudioEvent AudEvent_Lightning;

		private bool m_isGassed;

		private float m_checkTick = 0.2f;

		public override void Init(ZosigGameManager m)
		{
			M = m;
		}

		private void Start()
		{
		}

		public void TurnOn()
		{
			m_isGassed = true;
		}

		public void TurnOff()
		{
			m_isGassed = false;
		}

		private void Update()
		{
			m_checkTick -= Time.deltaTime;
			if (m_checkTick < 0f)
			{
				m_checkTick = Random.Range(0.3f, 0.8f);
				Check();
			}
		}

		public void ON()
		{
			if (!m_isEnabled && GM.ZMaster != null)
			{
				GM.ZMaster.FlagM.AddToFlag("s_t", 1);
			}
			M.FlagM.SetFlag(Flag, ValueWhenOn);
			m_isEnabled = true;
		}

		public void OFF()
		{
			m_isEnabled = false;
		}

		private void Check()
		{
			if (m_isEnabled && m_isGassed)
			{
				float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position);
				if (num < 40f)
				{
					if (!Buzzing.isPlaying)
					{
						Buzzing.Play();
					}
				}
				else if (Buzzing.isPlaying)
				{
					Buzzing.Stop();
				}
				if (num < 100f)
				{
					AmbientVFX.SetActive(value: true);
				}
				else
				{
					AmbientVFX.SetActive(value: false);
				}
				Collider[] array = Physics.OverlapSphere(base.transform.position, range, LM_SosigDetect, QueryTriggerInteraction.Collide);
				bool flag = false;
				for (int i = 0; i < array.Length; i++)
				{
					if (!(array[i].attachedRigidbody == null))
					{
						SosigLink component = array[i].attachedRigidbody.gameObject.GetComponent<SosigLink>();
						if (component != null && component.S != null)
						{
							component.S.Shudder(2f);
							Vector3 forward = component.transform.position - LightningOrigin.position;
							GameObject gameObject = Object.Instantiate(LightningFX[Random.Range(0, LightningFX.Count)], LightningOrigin.position, Quaternion.LookRotation(forward, Random.onUnitSphere));
							float magnitude = forward.magnitude;
							gameObject.transform.localScale = new Vector3(magnitude * 1.2f, magnitude * 1.2f, magnitude * 1.2f);
							flag = true;
						}
						break;
					}
				}
				if (flag && num < 60f)
				{
					SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, AudEvent_Lightning, base.transform.position, num / 343f);
				}
			}
			else
			{
				if (Buzzing.isPlaying)
				{
					Buzzing.Stop();
				}
				AmbientVFX.SetActive(value: false);
			}
		}
	}
}
