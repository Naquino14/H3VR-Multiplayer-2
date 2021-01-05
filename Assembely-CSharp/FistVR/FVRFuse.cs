using UnityEngine;

namespace FistVR
{
	public class FVRFuse : MonoBehaviour, IFVRDamageable
	{
		public FVRPhysicalObject Dynamite;

		public Transform DynamiteCenter;

		public Transform[] JointPos;

		public GameObject FuseFire;

		public AudioClip FuseIgnite;

		public GameObject ExplosionVFX;

		public GameObject ExplosionSFX;

		private bool m_isIgnited;

		private float m_igniteTick;

		private float m_igniteSpeed = 0.1f;

		public Renderer FuseRend;

		protected bool hasBoomed;

		public void Damage(Damage d)
		{
			if (d.Dam_Thermal > 0f)
			{
				Ignite(0.5f);
			}
		}

		public bool IsIgnited()
		{
			return m_isIgnited;
		}

		public void OnParticleCollision(GameObject other)
		{
			if (other.CompareTag("IgnitorSystem"))
			{
				Ignite(Random.Range(0.1f, 0.9f));
			}
		}

		private void Update()
		{
			if (m_isIgnited)
			{
				FuseFire.SetActive(value: true);
				m_igniteTick += Time.deltaTime * m_igniteSpeed;
				if (m_igniteTick >= 1f)
				{
					Boom();
				}
				int value = Mathf.FloorToInt(m_igniteTick * (float)JointPos.Length);
				int value2 = Mathf.RoundToInt(m_igniteTick * (float)JointPos.Length);
				value = Mathf.Clamp(value, 0, JointPos.Length - 1);
				value2 = Mathf.Clamp(value2, 0, JointPos.Length - 1);
				FuseFire.transform.position = Vector3.Lerp(JointPos[value].transform.position, JointPos[value2].transform.position, m_igniteTick);
				if (value > 0 && JointPos[value - 1].gameObject.GetComponent<Joint>() != null)
				{
					Object.Destroy(JointPos[value - 1].gameObject.GetComponent<Collider>());
				}
				FuseRend.material.SetFloat("_DissolveCutoff", m_igniteTick);
			}
		}

		public void Ignite(float f)
		{
			if (Dynamite == null || Dynamite.QuickbeltSlot == null)
			{
				m_igniteTick = Mathf.Max(m_igniteTick, f);
				if (!m_isIgnited)
				{
					m_isIgnited = true;
					FuseFire.SetActive(value: true);
					FuseFire.GetComponent<AudioSource>().Play();
					FuseFire.GetComponent<AudioSource>().PlayOneShot(FuseIgnite, 0.2f);
				}
			}
		}

		public virtual void Boom()
		{
			if (!hasBoomed)
			{
				hasBoomed = true;
				Object.Instantiate(ExplosionVFX, DynamiteCenter.position, Quaternion.identity);
				Object.Instantiate(ExplosionSFX, DynamiteCenter.position, Quaternion.identity);
				Object.Destroy(DynamiteCenter.gameObject);
			}
		}
	}
}
