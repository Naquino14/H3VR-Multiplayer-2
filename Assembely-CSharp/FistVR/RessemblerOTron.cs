using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class RessemblerOTron : MonoBehaviour
	{
		private int m_numMeatCoresLoaded;

		public List<Renderer> MeatCoreIndiciators;

		public ParticleSystem PFX_GrindInsert;

		public AudioEvent AudEvent_Insert;

		public AudioEvent AudEvent_Fail;

		public AudioEvent AudEvent_Success;

		public FVRObject RessemblerCore;

		public Transform SpawnPoint;

		[Header("Accordian")]
		public Transform Accordian;

		private float m_accordianLerp;

		private bool m_isAnimating;

		public AnimationCurve AccordianCurve;

		public float AccordianSpeed = 1f;

		public AudioEvent AudEvent_Accordian;

		private bool m_insertedCoreThisFrame;

		private int[] ValueByType = new int[8]
		{
			2,
			2,
			2,
			2,
			3,
			3,
			4,
			5
		};

		private void OnTriggerEnter(Collider col)
		{
			TestCollider(col);
		}

		private void TestCollider(Collider col)
		{
			if (col.attachedRigidbody == null)
			{
				return;
			}
			bool flag = false;
			RotrwMeatCore component = col.attachedRigidbody.gameObject.GetComponent<RotrwMeatCore>();
			RotrwMeatCore.CoreType type = component.Type;
			if (component != null && !m_insertedCoreThisFrame)
			{
				if (m_numMeatCoresLoaded >= 10)
				{
					EjectIngredient(component);
				}
				else
				{
					GrindEffect();
					Object.Destroy(component.gameObject);
					MeatCoreInserted(type);
				}
				flag = true;
			}
			if (flag)
			{
				return;
			}
			if (col.attachedRigidbody.GetComponent<FVRPhysicalObject>() != null)
			{
				FVRPhysicalObject component2 = col.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
				if (component2.IsHeld)
				{
					FVRViveHand hand = component2.m_hand;
					component2.EndInteraction(hand);
					hand.ForceSetInteractable(null);
				}
			}
			col.attachedRigidbody.velocity = Vector3.up * 4f + Random.onUnitSphere * 1.5f;
		}

		private void GrindEffect()
		{
			PFX_GrindInsert.Emit(20);
			SM.PlayGenericSound(AudEvent_Insert, base.transform.position);
		}

		private void MeatCoreInserted(RotrwMeatCore.CoreType ctype)
		{
			m_numMeatCoresLoaded += ValueByType[(int)ctype];
			m_insertedCoreThisFrame = true;
			UpdateIndicators();
		}

		private void EjectIngredient(FVRPhysicalObject obj)
		{
			if (obj.IsHeld)
			{
				FVRViveHand hand = obj.m_hand;
				obj.EndInteraction(hand);
				hand.ForceSetInteractable(null);
			}
			obj.RootRigidbody.velocity = Vector3.up * 4f + Random.onUnitSphere * 1.5f;
		}

		private void UpdateIndicators()
		{
			for (int i = 0; i < MeatCoreIndiciators.Count; i++)
			{
				if (i < m_numMeatCoresLoaded)
				{
					MeatCoreIndiciators[i].enabled = true;
				}
				else
				{
					MeatCoreIndiciators[i].enabled = false;
				}
			}
		}

		public void Grind(int derp)
		{
			if (m_numMeatCoresLoaded < 10)
			{
				SM.PlayGenericSound(AudEvent_Fail, base.transform.position);
				return;
			}
			SM.PlayGenericSound(AudEvent_Success, base.transform.position);
			SM.PlayGenericSound(AudEvent_Accordian, base.transform.position);
			m_accordianLerp = 0f;
			m_isAnimating = true;
			GameObject gameObject = Object.Instantiate(RessemblerCore.GetGameObject(), SpawnPoint.position, SpawnPoint.rotation);
			RW_Powerup component = gameObject.GetComponent<RW_Powerup>();
			if (GM.ZMaster != null)
			{
				GM.ZMaster.FlagM.AddToFlag("s_c", 1);
			}
			m_numMeatCoresLoaded = 0;
			UpdateIndicators();
		}

		private void Update()
		{
			m_insertedCoreThisFrame = false;
			Accordianing();
		}

		private void Accordianing()
		{
			if (m_isAnimating)
			{
				m_accordianLerp += Time.deltaTime;
				float y = AccordianCurve.Evaluate(m_accordianLerp);
				Accordian.localScale = new Vector3(1f, y, 1f);
				if (m_accordianLerp > 1f)
				{
					m_isAnimating = false;
				}
			}
		}
	}
}
