using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MM_MeatMachine : MonoBehaviour
	{
		public ParticleSystem PSystem_Sparks;

		public ParticleSystem PSystem_Sauce;

		public Transform[] Rollers;

		private float m_roll;

		public AudioSource Aud;

		public AudioClip Aud_Grind;

		public FVRObject[] Exclusions;

		private HashSet<FVRObject> hashy = new HashSet<FVRObject>();

		private bool m_shouldSave;

		private List<FVRPhysicalObject> objs_list = new List<FVRPhysicalObject>();

		private HashSet<FVRPhysicalObject> objs_hash = new HashSet<FVRPhysicalObject>();

		private float saveTick = 1f;

		private float GrindTick = 0.1f;

		private void Start()
		{
			for (int i = 0; i < Exclusions.Length; i++)
			{
				hashy.Add(Exclusions[i]);
			}
		}

		private void Update()
		{
			m_roll += Time.deltaTime * 1720f;
			m_roll = Mathf.Repeat(m_roll, 360f);
			Rollers[0].localEulerAngles = new Vector3(0f, 0f, 0f - m_roll);
			Rollers[1].localEulerAngles = new Vector3(0f, 0f, m_roll);
			if (GrindTick > 0f)
			{
				GrindTick -= Time.deltaTime;
			}
			if (saveTick > 0f)
			{
				saveTick -= Time.deltaTime;
				if (m_shouldSave)
				{
					m_shouldSave = false;
				}
			}
			if (objs_list.Count <= 0)
			{
				return;
			}
			for (int num = objs_list.Count - 1; num >= 0; num--)
			{
				if (objs_list[num] == null)
				{
					objs_hash.Remove(objs_list[num]);
					objs_list.RemoveAt(num);
				}
			}
		}

		private void OnTriggerEnter(Collider col)
		{
			CheckCol(col);
		}

		private void CheckCol(Collider col)
		{
			if (col.attachedRigidbody == null)
			{
				return;
			}
			FVRPhysicalObject component = col.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
			if (component == null || component.QuickbeltSlot != null || component.m_isHardnessed || !objs_hash.Add(component))
			{
				return;
			}
			objs_list.Add(component);
			if (GrindTick <= 0f && Aud_Grind != null)
			{
				GrindTick = Random.Range(0.2f, 0.5f);
				PSystem_Sparks.Emit(50);
				Aud.pitch = Random.Range(0.85f, 1.05f);
				Aud.PlayOneShot(Aud_Grind, 0.5f);
			}
			int num = 0;
			if (component.ObjectWrapper != null && hashy.Contains(component.ObjectWrapper))
			{
				num = 0;
			}
			else if (!(component is FVRFireArm))
			{
				if (component is FVRFireArmMagazine)
				{
					num = 0;
				}
				else if (component is FVRFireArmAttachment)
				{
					num = 20;
				}
				else if (component is FVRMeleeWeapon)
				{
					num = 50;
				}
				else if (component is FVRFireArmRound)
				{
					num = 0;
				}
				else if (component is MM_Currency)
				{
					MM_Currency mM_Currency = component as MM_Currency;
					int type = (int)mM_Currency.Type;
					num = ((type >= 15) ? 500 : ((type >= 9) ? 200 : ((type >= 5) ? 50 : ((type < 3) ? 2 : 20))));
				}
				else
				{
					num = 0;
				}
			}
			if (num > 0)
			{
				PSystem_Sauce.Emit(num);
				m_shouldSave = true;
				GM.Omni.OmniUnlocks.GainCurrency(num);
			}
			Object.Destroy(component.gameObject);
		}
	}
}
