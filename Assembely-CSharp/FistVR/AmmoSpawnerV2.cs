using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class AmmoSpawnerV2 : MonoBehaviour
	{
		[Serializable]
		private class CartridgeCategory
		{
			public List<FVRFireArmRoundDisplayData> Entries = new List<FVRFireArmRoundDisplayData>();
		}

		public OptionsPanel_ButtonSet OBS_Category;

		public OptionsPanel_ButtonSet OBS_Type;

		public OptionsPanel_ButtonSet OBS_Class;

		public List<Text> BTNS_Category;

		public List<Text> BTNS_Type;

		public List<Text> BTNS_Class;

		public Text LBL_Page_Type;

		public Text LBL_Page_Class;

		private int m_curCategory;

		private int m_typePage;

		private int m_classPage;

		private int m_maxTypePage;

		private int m_maxClassPage;

		private FireArmRoundType m_curAmmoType;

		private FireArmRoundClass m_curAmmoClass = FireArmRoundClass.FMJ;

		public GameObject BTNGO_Select;

		public GameObject BTNGO_Fill;

		public Transform SpawnPosition;

		public AudioEvent AudEvent_Beep;

		public AudioEvent AudEvent_Boop;

		private List<CartridgeCategory> Categories = new List<CartridgeCategory>();

		private Dictionary<FireArmRoundType, int> reverseCatDic = new Dictionary<FireArmRoundType, int>();

		private bool m_hasHeldType;

		private FireArmRoundType heldType = FireArmRoundType.a10mmAuto;

		public void Beep()
		{
			SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Beep, base.transform.position);
		}

		private void Boop()
		{
			SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Boop, base.transform.position);
		}

		private void Start()
		{
			PrimeCats();
			SetCategory(0);
			UpdateDisplay();
		}

		public void SpawnCartridge()
		{
			FireArmRoundType curAmmoType = m_curAmmoType;
			FireArmRoundClass curAmmoClass = m_curAmmoClass;
			GameObject gameObject = AM.GetRoundSelfPrefab(curAmmoType, curAmmoClass).GetGameObject();
			UnityEngine.Object.Instantiate(gameObject, SpawnPosition.position, SpawnPosition.rotation);
		}

		private void Update()
		{
			CheckFillButton();
		}

		private void UpdateDisplay()
		{
			LBL_Page_Type.text = m_typePage + 1 + "/" + m_maxTypePage;
			int num = 0;
			num += m_typePage * 10;
			for (int i = 0; i < BTNS_Type.Count; i++)
			{
				if (num < Categories[m_curCategory].Entries.Count)
				{
					BTNS_Type[i].gameObject.SetActive(value: true);
					BTNS_Type[i].text = Categories[m_curCategory].Entries[num].DisplayName;
				}
				else
				{
					BTNS_Type[i].gameObject.SetActive(value: false);
				}
				num++;
			}
			LBL_Page_Class.text = m_classPage + 1 + "/" + (m_maxClassPage + 1);
			int num2 = 0;
			num2 += m_classPage * 10;
			for (int j = 0; j < BTNS_Class.Count; j++)
			{
				if (num2 < AM.SRoundDisplayDataDic[m_curAmmoType].Classes.Length)
				{
					BTNS_Class[j].gameObject.SetActive(value: true);
					BTNS_Class[j].text = AM.SRoundDisplayDataDic[m_curAmmoType].Classes[num2].Name;
				}
				else
				{
					BTNS_Class[j].gameObject.SetActive(value: false);
				}
				num2++;
			}
		}

		public void PButton_SetCategory(int i)
		{
			Beep();
			SetCategory(i);
			UpdateDisplay();
		}

		public void PButton_SetType(int i)
		{
			Beep();
			int num = i;
			num += m_typePage * 10;
			SetType(Categories[m_curCategory].Entries[num].Type);
			UpdateDisplay();
		}

		public void PButton_SetClass(int i)
		{
			Beep();
			int num = i;
			num += m_classPage * 10;
			SetClass(AM.SRoundDisplayDataDic[m_curAmmoType].Classes[num].Class);
			UpdateDisplay();
		}

		private void SetCategory(int i)
		{
			m_curCategory = i;
			ResetType();
			m_typePage = 0;
			m_maxTypePage = Mathf.CeilToInt((float)Categories[m_curCategory].Entries.Count / 10f);
			ResetClass();
		}

		private void SetType(FireArmRoundType t)
		{
			m_curAmmoType = t;
			ResetClass();
			m_classPage = 0;
			m_maxClassPage = Mathf.CeilToInt((float)AM.STypeClassLists[m_curAmmoType].Count / 10f);
		}

		private void SetClass(FireArmRoundClass c)
		{
			m_curAmmoClass = c;
		}

		private void ResetType()
		{
			m_curAmmoType = Categories[m_curCategory].Entries[0].Type;
			OBS_Type.SetSelectedButton(0);
		}

		private void ResetClass()
		{
			m_curAmmoClass = AM.STypeClassLists[m_curAmmoType][0];
			OBS_Class.SetSelectedButton(0);
		}

		public void PButton_Type_Prev()
		{
			m_typePage--;
			if (m_typePage < 0)
			{
				m_typePage = 0;
			}
			Boop();
			UpdateDisplay();
		}

		public void PButton_Type_Next()
		{
			m_typePage++;
			if (m_typePage > m_maxTypePage)
			{
				m_typePage = Mathf.Clamp(m_typePage, 0, m_maxTypePage - 1);
			}
			Beep();
			UpdateDisplay();
		}

		public void PButton_Class_Prev()
		{
			m_classPage--;
			if (m_classPage < 0)
			{
				m_classPage = 0;
			}
			Boop();
			UpdateDisplay();
		}

		public void PButton_Class_Next()
		{
			m_classPage++;
			if (m_classPage >= m_maxClassPage)
			{
				m_classPage = Mathf.Clamp(m_classPage, 0, m_maxClassPage - 1);
			}
			Beep();
			UpdateDisplay();
		}

		public void PButton_SelectHeldType()
		{
			if (m_hasHeldType)
			{
				Beep();
				SetCategory(reverseCatDic[heldType]);
				OBS_Category.SetSelectedButton(reverseCatDic[heldType]);
				SetType(heldType);
				SetCorrectTypePage(heldType);
				UpdateDisplay();
			}
			else
			{
				Boop();
			}
		}

		public void PButton_SpawnCartridge()
		{
			Beep();
			SpawnCartridge();
		}

		public void PButton_FillHeldObject()
		{
			Beep();
			LoadIntoHeldObjects();
		}

		private void SetCorrectTypePage(FireArmRoundType t)
		{
			for (int i = 0; i < Categories[reverseCatDic[t]].Entries.Count; i++)
			{
				if (Categories[reverseCatDic[t]].Entries[i].Type == t)
				{
					int typePage = Mathf.FloorToInt(i / 10);
					int selectedButton = i % 10;
					m_typePage = typePage;
					OBS_Type.SetSelectedButton(selectedButton);
					break;
				}
			}
		}

		private bool IsRoundOfPower(FVRFireArmRoundDisplayData data, FVRObject.OTagFirearmRoundPower p, bool mf)
		{
			if (mf == data.IsMeatFortress)
			{
				return false;
			}
			if (mf && data.IsMeatFortress)
			{
				return true;
			}
			if (data.RoundPower == p)
			{
				return true;
			}
			return false;
		}

		private void PrimeCats()
		{
			for (int i = 0; i < 10; i++)
			{
				CartridgeCategory item = new CartridgeCategory();
				Categories.Add(item);
			}
			for (int j = 0; j < AM.STypeList.Count; j++)
			{
				FVRFireArmRoundDisplayData fVRFireArmRoundDisplayData = AM.SRoundDisplayDataDic[AM.STypeList[j]];
				int num = 0;
				num = (int)((!fVRFireArmRoundDisplayData.IsMeatFortress) ? (fVRFireArmRoundDisplayData.RoundPower - 1) : FVRObject.OTagFirearmRoundPower.Exotic);
				Categories[num].Entries.Add(fVRFireArmRoundDisplayData);
				reverseCatDic.Add(fVRFireArmRoundDisplayData.Type, num);
			}
		}

		public void LoadIntoHeldObjects()
		{
			FireArmRoundType curAmmoType = m_curAmmoType;
			FireArmRoundClass curAmmoClass = m_curAmmoClass;
			for (int i = 0; i < GM.CurrentMovementManager.Hands.Length; i++)
			{
				if (!(GM.CurrentMovementManager.Hands[i].CurrentInteractable != null) || !(GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRPhysicalObject))
				{
					continue;
				}
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArmMagazine)
				{
					FVRFireArmMagazine fVRFireArmMagazine = GM.CurrentMovementManager.Hands[i].CurrentInteractable as FVRFireArmMagazine;
					if (fVRFireArmMagazine.RoundType == curAmmoType)
					{
						fVRFireArmMagazine.ReloadMagWithType(curAmmoClass);
					}
				}
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArm)
				{
					FVRFireArm fVRFireArm = GM.CurrentMovementManager.Hands[i].CurrentInteractable as FVRFireArm;
					if (fVRFireArm.RoundType == curAmmoType && fVRFireArm.Magazine != null)
					{
						fVRFireArm.Magazine.ReloadMagWithType(curAmmoClass);
					}
				}
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArmClip)
				{
					FVRFireArmClip fVRFireArmClip = GM.CurrentMovementManager.Hands[i].CurrentInteractable as FVRFireArmClip;
					if (fVRFireArmClip.RoundType == curAmmoType)
					{
						fVRFireArmClip.ReloadClipWithType(curAmmoClass);
					}
				}
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable is Speedloader)
				{
					Speedloader speedloader = GM.CurrentMovementManager.Hands[i].CurrentInteractable as Speedloader;
					if (speedloader.Chambers[0].Type == curAmmoType)
					{
						speedloader.ReloadClipWithType(curAmmoClass);
					}
				}
			}
		}

		private void CheckFillButton()
		{
			bool flag = false;
			for (int i = 0; i < GM.CurrentMovementManager.Hands.Length; i++)
			{
				if (!(GM.CurrentMovementManager.Hands[i].CurrentInteractable != null) || !(GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRPhysicalObject))
				{
					continue;
				}
				FireArmRoundType curAmmoType = m_curAmmoType;
				FireArmRoundClass curAmmoClass = m_curAmmoClass;
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArmMagazine)
				{
					FVRFireArmMagazine fVRFireArmMagazine = GM.CurrentMovementManager.Hands[i].CurrentInteractable as FVRFireArmMagazine;
					m_hasHeldType = true;
					heldType = fVRFireArmMagazine.RoundType;
					if (fVRFireArmMagazine.RoundType == curAmmoType)
					{
						flag = true;
					}
				}
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArmClip)
				{
					FVRFireArmClip fVRFireArmClip = GM.CurrentMovementManager.Hands[i].CurrentInteractable as FVRFireArmClip;
					m_hasHeldType = true;
					heldType = fVRFireArmClip.RoundType;
					if (fVRFireArmClip.RoundType == curAmmoType)
					{
						flag = true;
					}
				}
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable is Speedloader)
				{
					Speedloader speedloader = GM.CurrentMovementManager.Hands[i].CurrentInteractable as Speedloader;
					m_hasHeldType = true;
					heldType = speedloader.Chambers[0].Type;
					if (speedloader.Chambers[0].Type == curAmmoType)
					{
						flag = true;
					}
				}
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArm)
				{
					FVRFireArm fVRFireArm = GM.CurrentMovementManager.Hands[i].CurrentInteractable as FVRFireArm;
					m_hasHeldType = true;
					heldType = fVRFireArm.RoundType;
					if (fVRFireArm.RoundType == curAmmoType && fVRFireArm.Magazine != null)
					{
						flag = true;
					}
				}
			}
			if (flag && !BTNGO_Fill.activeSelf)
			{
				BTNGO_Fill.SetActive(value: true);
			}
			else if (!flag && BTNGO_Fill.activeSelf)
			{
				BTNGO_Fill.SetActive(value: false);
			}
			if (m_hasHeldType && !BTNGO_Select.activeSelf)
			{
				BTNGO_Select.SetActive(value: true);
			}
			else if (!m_hasHeldType && BTNGO_Select.activeSelf)
			{
				BTNGO_Select.SetActive(value: false);
			}
		}
	}
}
