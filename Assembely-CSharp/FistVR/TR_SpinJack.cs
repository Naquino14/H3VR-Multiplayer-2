using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TR_SpinJack : MonoBehaviour
	{
		private bool m_isDescending;

		private bool m_isSpinning;

		private bool m_isAscending;

		private float Height = 20f;

		public MR_DamageOnEnter[] DamageNodes;

		private Vector3 startPos = Vector3.zero;

		public AudioSource Aud;

		public List<GameObject> Meatballs;

		private float Yrot;

		private float SpinSpeed;

		private void Start()
		{
			m_isDescending = true;
			startPos = base.transform.position;
			Yrot = base.transform.eulerAngles.y;
		}

		private void ActivateDamage()
		{
			for (int i = 0; i < DamageNodes.Length; i++)
			{
				DamageNodes[i].enabled = true;
			}
		}

		private void DeactivateDamage()
		{
			for (int i = 0; i < DamageNodes.Length; i++)
			{
				DamageNodes[i].enabled = false;
			}
		}

		private void Update()
		{
			if (m_isDescending)
			{
				Height -= Time.deltaTime * 2.5f;
				if (Height <= 0f)
				{
					Height = 0f;
					m_isDescending = false;
					m_isSpinning = true;
					Aud.volume = 0.1f;
					Aud.Play();
				}
				base.transform.position = new Vector3(startPos.x, Height, startPos.z);
			}
			if (m_isSpinning)
			{
				SpinSpeed = Mathf.MoveTowards(SpinSpeed, 120f, Time.deltaTime * 15f);
				for (int num = Meatballs.Count - 1; num >= 0; num--)
				{
					if (Meatballs[num] == null)
					{
						Meatballs.RemoveAt(num);
					}
				}
				if (Meatballs.Count == 0)
				{
					Aud.Stop();
					DeactivateDamage();
					m_isSpinning = false;
					m_isAscending = true;
				}
			}
			if (m_isAscending)
			{
				SpinSpeed = Mathf.MoveTowards(SpinSpeed, 0f, Time.deltaTime * 90f);
				Height += Time.deltaTime * 4f;
				base.transform.position = new Vector3(startPos.x, Height, startPos.z);
				if (Height >= 20f)
				{
					Object.Destroy(base.gameObject);
				}
			}
			float num2 = SpinSpeed / 50f;
			num2 *= num2;
			Aud.volume = num2 * 0.35f;
			Yrot += SpinSpeed * Time.deltaTime;
			Yrot = Mathf.Repeat(Yrot, 360f);
			base.transform.eulerAngles = new Vector3(0f, Yrot, 0f);
		}
	}
}
