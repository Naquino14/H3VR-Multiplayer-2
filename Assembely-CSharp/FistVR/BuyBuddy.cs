using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class BuyBuddy : MonoBehaviour
	{
		public Text ItemReadout;

		public List<Image> MeatCoresRequiredArray;

		public List<Sprite> MeatCoreSprites;

		private FVRObject m_storedObject1;

		private FVRObject m_storedObject2;

		public Transform CasePosition;

		public Transform CasePosition_Small;

		public FVRObject LargeCase;

		public FVRObject SmallCase;

		private List<GameObject> m_spawnedObjects = new List<GameObject>();

		private ZosigContainer_WeaponCase guncase;

		[Header("Audio")]
		public AudioEvent AudEvent_Buy;

		public ParticleSystem PFX_GrindInsert;

		public AudioEvent AudEvent_Insert;

		private bool m_insertedCoreThisFrame;

		public ZosigFlagManager F;

		private List<int> m_numCoresLeftToUnlock = new List<int>
		{
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};

		private string m_flagWhenPurchased = string.Empty;

		private float reFireLimit = 0.1f;

		public void ConfigureBuddy(ObjectTableDef tableDef, bool isLargeCase, string flag, List<int> pricetag)
		{
			F = GM.ZMaster.FlagM;
			m_flagWhenPurchased = flag;
			GameObject gameObject = null;
			Transform transform = CasePosition;
			if (isLargeCase)
			{
				gameObject = LargeCase.GetGameObject();
			}
			else
			{
				gameObject = SmallCase.GetGameObject();
				transform = CasePosition_Small;
			}
			GameObject gameObject2 = Object.Instantiate(gameObject, transform.position, transform.rotation);
			if (gameObject2.GetComponent<ZosigContainer>() != null)
			{
				ZosigContainer component = gameObject2.GetComponent<ZosigContainer>();
				component.PlaceObjectsInContainer(null);
				guncase = component as ZosigContainer_WeaponCase;
				guncase.Cover.LockCase();
			}
			ObjectTable objectTable = new ObjectTable();
			objectTable.Initialize(tableDef);
			FVRObject fVRObject = null;
			fVRObject = objectTable.GetRandomObject();
			int minAmmoCapacity = tableDef.MinAmmoCapacity;
			int maxAmmoCapacity = tableDef.MaxAmmoCapacity;
			guncase.PlaceObjectsInContainer(null);
			guncase.LoadIntoCrate(fVRObject, minAmmoCapacity, maxAmmoCapacity);
			if (F.GetFlagValue(flag) > 0)
			{
				CasePurchased();
			}
			else
			{
				for (int i = 0; i < pricetag.Count; i++)
				{
					m_numCoresLeftToUnlock[i] = pricetag[i];
				}
			}
			ItemReadout.text = tableDef.name;
			UpdatePriceDisplay();
		}

		private void GrindEffect()
		{
			PFX_GrindInsert.Emit(20);
			SM.PlayGenericSound(AudEvent_Insert, base.transform.position);
		}

		private void MeatCoreInserted(RotrwMeatCore.CoreType t)
		{
			m_insertedCoreThisFrame = true;
			m_numCoresLeftToUnlock[(int)t] = m_numCoresLeftToUnlock[(int)t] - 1;
			UpdatePriceDisplay();
			CheckIfPurchased();
		}

		private void CheckIfPurchased()
		{
			bool flag = true;
			for (int i = 0; i < m_numCoresLeftToUnlock.Count; i++)
			{
				if (m_numCoresLeftToUnlock[i] > 0)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				SM.PlayGenericSound(AudEvent_Buy, base.transform.position);
				CasePurchased();
				GM.ZMaster.FlagM.SetFlag(m_flagWhenPurchased, 1);
			}
		}

		private bool NeedsType(RotrwMeatCore.CoreType t)
		{
			if (m_numCoresLeftToUnlock[(int)t] > 0)
			{
				return true;
			}
			return false;
		}

		private void OnTriggerEnter(Collider col)
		{
			if (!(reFireLimit > 0f))
			{
				TestCollider(col);
			}
		}

		private void Update()
		{
			m_insertedCoreThisFrame = false;
			if (reFireLimit >= 0f)
			{
				reFireLimit -= Time.deltaTime;
			}
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
				if (NeedsType(component.Type))
				{
					GrindEffect();
					Object.Destroy(component.gameObject);
					MeatCoreInserted(type);
				}
				else
				{
					EjectIngredient(component);
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

		private void CasePurchased()
		{
			guncase.Cover.UnlockCase();
		}

		private void UpdatePriceDisplay()
		{
			int num = 0;
			for (int num2 = m_numCoresLeftToUnlock.Count - 1; num2 >= 0; num2--)
			{
				if (m_numCoresLeftToUnlock[num2] >= 1)
				{
					for (int i = 0; i < m_numCoresLeftToUnlock[num2]; i++)
					{
						if (num >= MeatCoresRequiredArray.Count)
						{
							break;
						}
						MeatCoresRequiredArray[num].gameObject.SetActive(value: true);
						MeatCoresRequiredArray[num].sprite = MeatCoreSprites[num2];
						num++;
					}
				}
			}
			for (int j = num; j < MeatCoresRequiredArray.Count; j++)
			{
				MeatCoresRequiredArray[j].gameObject.SetActive(value: false);
			}
		}
	}
}
