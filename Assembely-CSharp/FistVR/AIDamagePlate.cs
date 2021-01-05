using UnityEngine;

namespace FistVR
{
	public class AIDamagePlate : MonoBehaviour, IFVRDamageable
	{
		public float XRotUp;

		public float XRotDown;

		private AudioSource m_aud;

		public bool IsDown;

		private void Start()
		{
			m_aud = GetComponent<AudioSource>();
		}

		public virtual void Damage(Vector3 damagePoint, Vector3 damageDir, Vector2 damageUVCoord)
		{
			if (!IsDown)
			{
				IsDown = true;
				m_aud.PlayOneShot(m_aud.clip, 1f);
				base.transform.localEulerAngles = new Vector3(XRotDown, 0f, 0f);
			}
		}

		public virtual void Damage(Damage dam)
		{
			if (!IsDown)
			{
				IsDown = true;
				m_aud.PlayOneShot(m_aud.clip, 1f);
				base.transform.localEulerAngles = new Vector3(XRotDown, 0f, 0f);
			}
		}

		public virtual void Reset()
		{
			IsDown = false;
			base.transform.localEulerAngles = new Vector3(XRotUp, 0f, 0f);
		}
	}
}
