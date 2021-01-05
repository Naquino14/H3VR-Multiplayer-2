using UnityEngine;

namespace FistVR
{
	public class TankFirework : MonoBehaviour
	{
		public GameObject FuseFire;

		public GameObject BurnFireTrigger;

		public Transform Fuse_Point1;

		public Transform Fuse_Point2;

		public float burnSpeed = 0.15f;

		public Renderer TankRend;

		private float m_burnLerp;

		private bool m_isIgnited;

		private bool m_isFireworkTriggered;

		private bool m_doneBurning;

		public ParticleSystem PSystem_BarrelSparkles;

		public ParticleSystem PSystem_BarrelSparkles2;

		public ParticleSystem PSystem_Splodes;

		public ParticleSystem PSystem_Smoke;

		public ParticleSystem BurnFire;

		public AudioSource Aud_Crackling;

		public AudioSource Aud_Sparkler;

		public Transform Muzzle;

		public GameObject DragonProj;

		public Rigidbody RB;

		private int popTick = 4;

		private float projTick = 1f;

		private float dieCounter;

		public void Ignite()
		{
			if (!m_isIgnited)
			{
				m_isIgnited = true;
				FuseFire.SetActive(value: true);
				FuseFire.transform.localPosition = Fuse_Point1.localPosition;
			}
		}

		public void Update()
		{
			Burn();
		}

		public void Awake()
		{
			popTick = Random.Range(5, 15);
			projTick = Random.Range(0.25f, 0.75f);
		}

		private void Burn()
		{
			if (!m_isIgnited || m_doneBurning)
			{
				if (m_doneBurning)
				{
					dieCounter += Time.deltaTime;
				}
				if (dieCounter > 10f)
				{
					Object.Destroy(base.gameObject);
				}
				return;
			}
			m_burnLerp += Time.deltaTime * burnSpeed;
			if (m_burnLerp < 0.3f)
			{
				FuseFire.transform.localPosition = Vector3.Lerp(Fuse_Point1.localPosition, Fuse_Point2.localPosition, 4f * m_burnLerp);
			}
			else if (m_burnLerp < 0.99f)
			{
				PSystem_BarrelSparkles.Emit(3);
				PSystem_BarrelSparkles2.Emit(1);
				PSystem_Splodes.Emit(1);
				BurnFire.Emit(1);
				if (!m_isFireworkTriggered)
				{
					m_isFireworkTriggered = true;
					Aud_Crackling.Play();
					Aud_Sparkler.Play();
				}
				if (FuseFire.activeSelf)
				{
					FuseFire.SetActive(value: false);
				}
				if (!BurnFireTrigger.activeSelf)
				{
					BurnFireTrigger.SetActive(value: true);
				}
				if (!PSystem_Smoke.gameObject.activeSelf)
				{
					PSystem_Smoke.gameObject.SetActive(value: true);
				}
				if (popTick > 0)
				{
					popTick--;
				}
				else
				{
					popTick = Random.Range(5, 15);
					FXM.InitiateMuzzleFlash(PSystem_BarrelSparkles.transform.position, PSystem_BarrelSparkles.transform.forward, Random.Range(0.3f, 2f), new Color(1f, Random.Range(0.2f, 1f), 0.2f), Random.Range(0.5f, 2f));
				}
				if (projTick > 0f)
				{
					projTick -= Time.deltaTime;
				}
				else
				{
					projTick = Random.Range(0.25f, 0.75f);
					GameObject gameObject = Object.Instantiate(DragonProj, Muzzle.position, Muzzle.rotation);
					gameObject.transform.Rotate(new Vector3(Random.Range(-15f, 15f), Random.Range(-15f, 15f), Random.Range(-15f, 15f)));
					RB.AddForceAtPosition(-gameObject.transform.forward * Random.Range(0.1f, 0.4f), Muzzle.position, ForceMode.Impulse);
					gameObject.GetComponent<BallisticProjectile>().Fire(gameObject.transform.forward, null);
				}
			}
			else if (!m_doneBurning)
			{
				m_isFireworkTriggered = false;
				m_doneBurning = true;
				if (PSystem_Smoke.gameObject.activeSelf)
				{
					PSystem_Smoke.gameObject.SetActive(value: false);
				}
				Aud_Crackling.Stop();
				Aud_Sparkler.Stop();
				BurnFireTrigger.SetActive(value: false);
			}
			float value = Mathf.Clamp(m_burnLerp, 0f, 1f);
			float value2 = Mathf.Clamp(m_burnLerp - 0.2f, 0f, 0.775f);
			TankRend.material.SetFloat("_TransitionCutoff", value);
			TankRend.material.SetFloat("_DissolveCutoff", value2);
		}
	}
}
