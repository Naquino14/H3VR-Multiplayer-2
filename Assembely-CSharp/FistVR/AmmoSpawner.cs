using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class AmmoSpawner : MonoBehaviour
	{
		public OptionsPanel_ButtonSet ButtonSet;

		public GameObject[] ButtonList;

		public Text[] ButtonText;

		public Text TopBarText;

		public GameObject BackToTypesButton;

		private bool m_isInTypeMode = true;

		private int m_maxTypePages = 1;

		private int m_currentTypePage;

		private int m_currentClassPage;

		private int m_currentTypeSelected;

		private int m_currentClassSelected;

		public GameObject NextPageButton;

		public GameObject PrevPageButton;

		public GameObject SpawnCartridgeButton;

		public GameObject SpawnAmmoBoxButton;

		public GameObject LoadIntoMagButton;

		public Transform SpawnPosition;

		public void NextPage()
		{
			m_currentTypePage++;
			m_currentTypePage = Mathf.Clamp(m_currentTypePage, 0, m_maxTypePages);
			UpdateButtonText();
		}

		public void PrevPage()
		{
			m_currentTypePage--;
			m_currentTypePage = Mathf.Clamp(m_currentTypePage, 0, m_maxTypePages);
			UpdateButtonText();
		}

		private void Start()
		{
			m_maxTypePages = Mathf.CeilToInt((float)AM.STypeList.Count / 12f);
			m_currentTypePage = 0;
			ButtonSet.EnableAllButtons();
			UpdateButtonText();
		}

		private void Update()
		{
			if (m_isInTypeMode)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < GM.CurrentMovementManager.Hands.Length; i++)
			{
				if (!(GM.CurrentMovementManager.Hands[i].CurrentInteractable != null) || !(GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRPhysicalObject))
				{
					continue;
				}
				FireArmRoundType fireArmRoundType = AM.STypeList[m_currentTypeSelected];
				FireArmRoundClass fireArmRoundClass = AM.STypeClassLists[fireArmRoundType][m_currentClassSelected];
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArmMagazine)
				{
					FVRFireArmMagazine fVRFireArmMagazine = GM.CurrentMovementManager.Hands[i].CurrentInteractable as FVRFireArmMagazine;
					if (fVRFireArmMagazine.RoundType == fireArmRoundType)
					{
						flag = true;
					}
				}
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArmClip)
				{
					FVRFireArmClip fVRFireArmClip = GM.CurrentMovementManager.Hands[i].CurrentInteractable as FVRFireArmClip;
					if (fVRFireArmClip.RoundType == fireArmRoundType)
					{
						flag = true;
					}
				}
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable is Speedloader)
				{
					Speedloader speedloader = GM.CurrentMovementManager.Hands[i].CurrentInteractable as Speedloader;
					if (speedloader.Chambers[0].Type == fireArmRoundType)
					{
						flag = true;
					}
				}
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArm)
				{
					FVRFireArm fVRFireArm = GM.CurrentMovementManager.Hands[i].CurrentInteractable as FVRFireArm;
					if (fVRFireArm.RoundType == fireArmRoundType && fVRFireArm.Magazine != null)
					{
						flag = true;
					}
				}
			}
			if (flag && !LoadIntoMagButton.activeSelf)
			{
				LoadIntoMagButton.SetActive(value: true);
			}
			else if (!flag && LoadIntoMagButton.activeSelf)
			{
				LoadIntoMagButton.SetActive(value: false);
			}
		}

		public void LoadIntoHeldObjects()
		{
			FireArmRoundType fireArmRoundType = AM.STypeList[m_currentTypeSelected];
			FireArmRoundClass rClass = AM.STypeClassLists[fireArmRoundType][m_currentClassSelected];
			for (int i = 0; i < GM.CurrentMovementManager.Hands.Length; i++)
			{
				if (!(GM.CurrentMovementManager.Hands[i].CurrentInteractable != null) || !(GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRPhysicalObject))
				{
					continue;
				}
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArmMagazine)
				{
					FVRFireArmMagazine fVRFireArmMagazine = GM.CurrentMovementManager.Hands[i].CurrentInteractable as FVRFireArmMagazine;
					if (fVRFireArmMagazine.RoundType == fireArmRoundType)
					{
						fVRFireArmMagazine.ReloadMagWithType(rClass);
					}
				}
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArm)
				{
					FVRFireArm fVRFireArm = GM.CurrentMovementManager.Hands[i].CurrentInteractable as FVRFireArm;
					if (fVRFireArm.RoundType == fireArmRoundType && fVRFireArm.Magazine != null)
					{
						fVRFireArm.Magazine.ReloadMagWithType(rClass);
					}
				}
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArmClip)
				{
					FVRFireArmClip fVRFireArmClip = GM.CurrentMovementManager.Hands[i].CurrentInteractable as FVRFireArmClip;
					if (fVRFireArmClip.RoundType == fireArmRoundType)
					{
						fVRFireArmClip.ReloadClipWithType(rClass);
					}
				}
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable is Speedloader)
				{
					Speedloader speedloader = GM.CurrentMovementManager.Hands[i].CurrentInteractable as Speedloader;
					if (speedloader.Chambers[0].Type == fireArmRoundType)
					{
						speedloader.ReloadClipWithType(rClass);
					}
				}
			}
		}

		private void UpdateButtonText()
		{
			if (m_isInTypeMode)
			{
				BackToTypesButton.SetActive(value: false);
				SpawnCartridgeButton.SetActive(value: false);
				SpawnAmmoBoxButton.SetActive(value: false);
				LoadIntoMagButton.SetActive(value: false);
				NextPageButton.SetActive(value: true);
				PrevPageButton.SetActive(value: true);
				TopBarText.text = "Select Caliber " + (m_currentTypePage + 1) + "/" + m_maxTypePages;
				for (int i = 0; i < ButtonText.Length; i++)
				{
					int num = i + 12 * m_currentTypePage;
					if (num < AM.STypeList.Count)
					{
						ButtonList[i].SetActive(value: true);
						ButtonText[i].text = AM.SRoundDisplayDataDic[AM.STypeList[num]].DisplayName;
					}
					else
					{
						ButtonList[i].SetActive(value: false);
						ButtonText[i].text = string.Empty;
					}
				}
				return;
			}
			BackToTypesButton.SetActive(value: true);
			NextPageButton.SetActive(value: false);
			PrevPageButton.SetActive(value: false);
			TopBarText.text = AM.SRoundDisplayDataDic[AM.STypeList[m_currentTypeSelected]].DisplayName;
			FireArmRoundType fireArmRoundType = AM.STypeList[m_currentTypeSelected];
			for (int j = 0; j < ButtonText.Length; j++)
			{
				if (j < AM.STypeClassLists[fireArmRoundType].Count)
				{
					FireArmRoundClass key = AM.STypeClassLists[fireArmRoundType][j];
					ButtonList[j].SetActive(value: true);
					ButtonText[j].text = AM.STypeDic[fireArmRoundType][key].Name;
				}
				else
				{
					ButtonList[j].SetActive(value: false);
					ButtonText[j].text = string.Empty;
				}
			}
			FireArmRoundClass rClass = AM.STypeClassLists[fireArmRoundType][m_currentClassSelected];
			if (AM.GetRoundSelfPrefab(fireArmRoundType, rClass) != null && AM.GetRoundSelfPrefab(fireArmRoundType, rClass).GetGameObject() != null)
			{
				SpawnCartridgeButton.SetActive(value: true);
			}
			else
			{
				SpawnCartridgeButton.SetActive(value: false);
			}
			SpawnAmmoBoxButton.SetActive(value: false);
		}

		public void AmmoButtonPressed(int i)
		{
			if (m_isInTypeMode)
			{
				m_currentTypeSelected = i + 12 * m_currentTypePage;
				ButtonSet.EnableAllButtons();
				m_isInTypeMode = false;
				AmmoButtonPressed(0);
			}
			else
			{
				m_currentClassSelected = i;
				ButtonSet.SetSelectedButton(i);
			}
			UpdateButtonText();
		}

		public void ReturnToCaliberSelection()
		{
			m_isInTypeMode = true;
			ButtonSet.EnableAllButtons();
			UpdateButtonText();
		}

		public void SpawnCartridge()
		{
			FireArmRoundType fireArmRoundType = AM.STypeList[m_currentTypeSelected];
			FireArmRoundClass rClass = AM.STypeClassLists[fireArmRoundType][m_currentClassSelected];
			GameObject gameObject = AM.GetRoundSelfPrefab(fireArmRoundType, rClass).GetGameObject();
			Object.Instantiate(gameObject, SpawnPosition.position, SpawnPosition.rotation);
		}
	}
}
