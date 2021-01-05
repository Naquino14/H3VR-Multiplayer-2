using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class ZosigSaveGameManager : MonoBehaviour
	{
		public string LevelName;

		private int m_selectedSlot = -1;

		private bool isLoading;

		private int m_slotToDelete = -1;

		public List<Text> SaveGameSlotTextFields;

		public Color Color_Selected;

		public Color Color_UnSelected;

		public List<GameObject> DeleteButtons = new List<GameObject>();

		public List<GameObject> ConfirmButtons = new List<GameObject>();

		public List<GameObject> DifficultyButtons_0;

		public List<GameObject> DifficultyButtons_1;

		public List<GameObject> DifficultyButtons_2;

		public List<GameObject> IntroButtons_0;

		public List<GameObject> IntroButtons_1;

		public List<GameObject> IntroButtons_2;

		public Text StartButtonText;

		public GameObject StartButton;

		private int m_selectedDifficulty_Slot0;

		private int m_selectedDifficulty_Slot1;

		private int m_selectedDifficulty_Slot2;

		private int m_selectedIntro_Slot0;

		private int m_selectedIntro_Slot1;

		private int m_selectedIntro_Slot2;

		public void Start()
		{
			Invoke("UpdateUI", 0.1f);
		}

		public void Button_SelectSlot(int i)
		{
			if (!isLoading)
			{
				m_selectedSlot = i;
				m_slotToDelete = -1;
				UpdateUI();
			}
		}

		public void Button_Delete()
		{
			if (!isLoading && m_selectedSlot >= 0)
			{
				m_slotToDelete = m_selectedSlot;
				UpdateUI();
			}
		}

		public void Button_SelectDifficulty_Slot0(int i)
		{
			if (!isLoading)
			{
				m_selectedSlot = 0;
				m_selectedDifficulty_Slot0 = i;
				UpdateUI();
			}
		}

		public void Button_SelectDifficulty_Slot1(int i)
		{
			if (!isLoading)
			{
				m_selectedSlot = 1;
				m_selectedDifficulty_Slot1 = i;
				UpdateUI();
			}
		}

		public void Button_SelectDifficulty_Slot2(int i)
		{
			if (!isLoading)
			{
				m_selectedSlot = 2;
				m_selectedDifficulty_Slot2 = i;
				UpdateUI();
			}
		}

		public void Button_SelectIntro_Slot0(int i)
		{
			if (!isLoading)
			{
				m_selectedSlot = 0;
				m_selectedIntro_Slot0 = i;
				UpdateUI();
			}
		}

		public void Button_SelectIntro_Slot1(int i)
		{
			if (!isLoading)
			{
				m_selectedSlot = 1;
				m_selectedIntro_Slot1 = i;
				UpdateUI();
			}
		}

		public void Button_SelectIntro_Slot2(int i)
		{
			if (!isLoading)
			{
				m_selectedSlot = 2;
				m_selectedIntro_Slot2 = i;
				UpdateUI();
			}
		}

		public void Button_Confirm()
		{
			if (!isLoading && m_selectedSlot >= 0 && m_slotToDelete >= 0)
			{
				DeleteSaveGame(m_selectedSlot);
				m_selectedSlot = -1;
				UpdateUI();
			}
		}

		public void Button_StartGame()
		{
			if (isLoading || m_selectedSlot < 0)
			{
				return;
			}
			switch (m_selectedSlot)
			{
			case 0:
				if (GM.ROTRWSaves.SaveGame1.Count < 1)
				{
					GM.ROTRWSaves.SaveGame1.Add("flag_Difficulty", m_selectedDifficulty_Slot0);
					GM.ROTRWSaves.SaveGame1.Add("skip_intro", m_selectedIntro_Slot0);
				}
				break;
			case 1:
				if (GM.ROTRWSaves.SaveGame2.Count < 1)
				{
					GM.ROTRWSaves.SaveGame2.Add("flag_Difficulty", m_selectedDifficulty_Slot1);
					GM.ROTRWSaves.SaveGame2.Add("skip_intro", m_selectedIntro_Slot1);
				}
				break;
			case 2:
				if (GM.ROTRWSaves.SaveGame3.Count < 1)
				{
					GM.ROTRWSaves.SaveGame3.Add("flag_Difficulty", m_selectedDifficulty_Slot2);
					GM.ROTRWSaves.SaveGame3.Add("skip_intro", m_selectedIntro_Slot2);
				}
				break;
			}
			GM.ROTRWSaves.SaveToFile();
			m_slotToDelete = -1;
			UpdateUI();
			GM.ROTRWSaves.SetCurrentSaveGame(m_selectedSlot + 1);
			isLoading = true;
			SteamVR_LoadLevel.Begin(LevelName);
		}

		private void UpdateUI()
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (GM.ROTRWSaves.SaveGame1.Count > 0)
			{
				flag = true;
			}
			if (GM.ROTRWSaves.SaveGame2.Count > 0)
			{
				flag2 = true;
			}
			if (GM.ROTRWSaves.SaveGame3.Count > 0)
			{
				flag3 = true;
			}
			if (flag)
			{
				if (GM.ROTRWSaves.SaveGame1.ContainsKey("flag_Difficulty") && GM.ROTRWSaves.SaveGame1["flag_Difficulty"] > 1)
				{
					SaveGameSlotTextFields[0].text = "Hardcore Save 1";
				}
				else if (GM.ROTRWSaves.SaveGame1.ContainsKey("flag_Difficulty") && GM.ROTRWSaves.SaveGame1["flag_Difficulty"] > 0)
				{
					SaveGameSlotTextFields[0].text = "Arcade Save 1";
				}
				else
				{
					SaveGameSlotTextFields[0].text = "Classic Save 1";
				}
			}
			else if (m_selectedDifficulty_Slot0 == 0)
			{
				SaveGameSlotTextFields[0].text = "New Classic Save";
			}
			else if (m_selectedDifficulty_Slot0 == 1)
			{
				SaveGameSlotTextFields[0].text = "New Arcade Save";
			}
			else if (m_selectedDifficulty_Slot0 == 2)
			{
				SaveGameSlotTextFields[0].text = "New Hardcore Save";
			}
			if (flag2)
			{
				if (GM.ROTRWSaves.SaveGame2.ContainsKey("flag_Difficulty") && GM.ROTRWSaves.SaveGame2["flag_Difficulty"] > 1)
				{
					SaveGameSlotTextFields[1].text = "Hardcore Save 2";
				}
				else if (GM.ROTRWSaves.SaveGame2.ContainsKey("flag_Difficulty") && GM.ROTRWSaves.SaveGame2["flag_Difficulty"] > 0)
				{
					SaveGameSlotTextFields[1].text = "Arcade Save 2";
				}
				else
				{
					SaveGameSlotTextFields[1].text = "Classic Save 2";
				}
			}
			else if (m_selectedDifficulty_Slot1 == 0)
			{
				SaveGameSlotTextFields[1].text = "New Classic Save";
			}
			else if (m_selectedDifficulty_Slot1 == 1)
			{
				SaveGameSlotTextFields[1].text = "New Arcade Save";
			}
			else if (m_selectedDifficulty_Slot1 == 2)
			{
				SaveGameSlotTextFields[1].text = "New Hardcore Save";
			}
			if (flag3)
			{
				if (GM.ROTRWSaves.SaveGame3.ContainsKey("flag_Difficulty") && GM.ROTRWSaves.SaveGame3["flag_Difficulty"] > 1)
				{
					SaveGameSlotTextFields[2].text = "Hardcore Save 3";
				}
				else if (GM.ROTRWSaves.SaveGame3.ContainsKey("flag_Difficulty") && GM.ROTRWSaves.SaveGame3["flag_Difficulty"] > 0)
				{
					SaveGameSlotTextFields[2].text = "Arcade Save 3";
				}
				else
				{
					SaveGameSlotTextFields[2].text = "Classic Save 3";
				}
			}
			else if (m_selectedDifficulty_Slot2 == 0)
			{
				SaveGameSlotTextFields[2].text = "New Classic Save";
			}
			else if (m_selectedDifficulty_Slot2 == 1)
			{
				SaveGameSlotTextFields[2].text = "New Arcade Save";
			}
			else if (m_selectedDifficulty_Slot2 == 2)
			{
				SaveGameSlotTextFields[2].text = "New Hardcore Save";
			}
			if (m_selectedSlot > -1)
			{
				switch (m_selectedSlot)
				{
				case 0:
					SaveGameSlotTextFields[0].color = Color_Selected;
					SaveGameSlotTextFields[1].color = Color_UnSelected;
					SaveGameSlotTextFields[2].color = Color_UnSelected;
					break;
				case 1:
					SaveGameSlotTextFields[0].color = Color_UnSelected;
					SaveGameSlotTextFields[1].color = Color_Selected;
					SaveGameSlotTextFields[2].color = Color_UnSelected;
					break;
				case 2:
					SaveGameSlotTextFields[0].color = Color_UnSelected;
					SaveGameSlotTextFields[1].color = Color_UnSelected;
					SaveGameSlotTextFields[2].color = Color_Selected;
					break;
				}
			}
			else
			{
				SaveGameSlotTextFields[0].color = Color_UnSelected;
				SaveGameSlotTextFields[1].color = Color_UnSelected;
				SaveGameSlotTextFields[2].color = Color_UnSelected;
			}
			if (flag)
			{
				for (int i = 0; i < DifficultyButtons_0.Count; i++)
				{
					DifficultyButtons_0[i].SetActive(value: false);
				}
				for (int j = 0; j < IntroButtons_0.Count; j++)
				{
					IntroButtons_0[j].SetActive(value: false);
				}
			}
			else
			{
				for (int k = 0; k < DifficultyButtons_0.Count; k++)
				{
					DifficultyButtons_0[k].SetActive(value: true);
				}
				for (int l = 0; l < IntroButtons_0.Count; l++)
				{
					IntroButtons_0[l].SetActive(value: true);
				}
			}
			if (flag2)
			{
				for (int m = 0; m < DifficultyButtons_1.Count; m++)
				{
					DifficultyButtons_1[m].SetActive(value: false);
				}
				for (int n = 0; n < IntroButtons_1.Count; n++)
				{
					IntroButtons_1[n].SetActive(value: false);
				}
			}
			else
			{
				for (int num = 0; num < DifficultyButtons_1.Count; num++)
				{
					DifficultyButtons_1[num].SetActive(value: true);
				}
				for (int num2 = 0; num2 < IntroButtons_1.Count; num2++)
				{
					IntroButtons_1[num2].SetActive(value: true);
				}
			}
			if (flag3)
			{
				for (int num3 = 0; num3 < DifficultyButtons_2.Count; num3++)
				{
					DifficultyButtons_2[num3].SetActive(value: false);
				}
				for (int num4 = 0; num4 < IntroButtons_2.Count; num4++)
				{
					IntroButtons_2[num4].SetActive(value: false);
				}
			}
			else
			{
				for (int num5 = 0; num5 < DifficultyButtons_2.Count; num5++)
				{
					DifficultyButtons_2[num5].SetActive(value: true);
				}
				for (int num6 = 0; num6 < IntroButtons_2.Count; num6++)
				{
					IntroButtons_2[num6].SetActive(value: true);
				}
			}
			if (m_selectedSlot > -1)
			{
				switch (m_selectedSlot)
				{
				case 0:
					if (flag)
					{
						DeleteButtons[0].SetActive(value: true);
					}
					else
					{
						DeleteButtons[0].SetActive(value: false);
					}
					DeleteButtons[1].SetActive(value: false);
					DeleteButtons[2].SetActive(value: false);
					break;
				case 1:
					if (flag2)
					{
						DeleteButtons[1].SetActive(value: true);
					}
					else
					{
						DeleteButtons[1].SetActive(value: false);
					}
					DeleteButtons[0].SetActive(value: false);
					DeleteButtons[2].SetActive(value: false);
					break;
				case 2:
					if (flag3)
					{
						DeleteButtons[2].SetActive(value: true);
					}
					else
					{
						DeleteButtons[2].SetActive(value: false);
					}
					DeleteButtons[0].SetActive(value: false);
					DeleteButtons[1].SetActive(value: false);
					break;
				}
			}
			else
			{
				for (int num7 = 0; num7 < DeleteButtons.Count; num7++)
				{
					DeleteButtons[num7].SetActive(value: false);
				}
			}
			if (m_slotToDelete > -1)
			{
				switch (m_selectedSlot)
				{
				case 0:
					if (flag)
					{
						ConfirmButtons[0].SetActive(value: true);
					}
					else
					{
						ConfirmButtons[0].SetActive(value: false);
					}
					ConfirmButtons[1].SetActive(value: false);
					ConfirmButtons[2].SetActive(value: false);
					break;
				case 1:
					if (flag2)
					{
						ConfirmButtons[1].SetActive(value: true);
					}
					else
					{
						ConfirmButtons[1].SetActive(value: false);
					}
					ConfirmButtons[0].SetActive(value: false);
					ConfirmButtons[2].SetActive(value: false);
					break;
				case 2:
					if (flag3)
					{
						ConfirmButtons[2].SetActive(value: true);
					}
					else
					{
						ConfirmButtons[2].SetActive(value: false);
					}
					ConfirmButtons[0].SetActive(value: false);
					ConfirmButtons[1].SetActive(value: false);
					break;
				}
			}
			else
			{
				for (int num8 = 0; num8 < ConfirmButtons.Count; num8++)
				{
					ConfirmButtons[num8].SetActive(value: false);
				}
			}
			if (m_selectedSlot < 0)
			{
				StartButton.SetActive(value: false);
				return;
			}
			StartButton.SetActive(value: true);
			switch (m_selectedSlot)
			{
			case 0:
				if (flag)
				{
					StartButtonText.text = "Load Save Game";
				}
				else
				{
					StartButtonText.text = "Start New Game";
				}
				break;
			case 1:
				if (flag2)
				{
					StartButtonText.text = "Load Save Game";
				}
				else
				{
					StartButtonText.text = "Start New Game";
				}
				break;
			case 2:
				if (flag3)
				{
					StartButtonText.text = "Load Save Game";
				}
				else
				{
					StartButtonText.text = "Start New Game";
				}
				break;
			}
		}

		private void DeleteSaveGame(int index)
		{
			GM.ROTRWSaves.DeleteSaveGame(index);
			GM.ROTRWSaves.SaveToFile();
			m_selectedSlot = -1;
			m_slotToDelete = -1;
			UpdateUI();
		}

		private void Update()
		{
		}
	}
}
