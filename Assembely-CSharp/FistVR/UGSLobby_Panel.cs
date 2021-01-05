using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FistVR
{
	public class UGSLobby_Panel : MonoBehaviour
	{
		public enum LobbyPanelScreen
		{
			CreateNewLevel,
			LoadExistingLevel,
			CurrentLevelParams
		}

		private LobbyPanelScreen m_curScreen;

		public Keyboard Keyboard;

		[Header("Root Transforms")]
		public GameObject TopMenu;

		public GameObject Page_CreateNewLevel;

		public GameObject Page_LoadExistingLevel;

		public GameObject Page_CurrentLevelParams;

		[Header("Top Buttons")]
		public GameObject Button_NewLevel;

		public GameObject Button_LoadLevel;

		public GameObject Button_CurrentLevel;

		public GameObject Button_BackToLobby;

		[Header("CreateNewLevelPage")]
		public GameObject Button_CreateNewLevel;

		public Text TXT_NewLevelName;

		public OptionsPanel_ButtonSet OBS_TemplateSelection;

		[Header("LoadExistingLevelPage")]
		public UGSLobby_LoadLevelLabel[] Labels_ExistingLevels;

		public OptionsPanel_ButtonSet OBS_ExistingLevelList;

		[Header("CurrentLevelPage")]
		public GameObject Button_SaveLevel;

		private bool m_hasEnteredNewLevelName;

		private string m_curNewLevelName = string.Empty;

		private string m_curSceneToCreateFrom = string.Empty;

		private int m_curExistingLevelIndex;

		private void Awake()
		{
			OBS_ExistingLevelList.SetSelectedButton(0);
			OnLevelWasLoaded(SceneManager.GetActiveScene().buildIndex);
			OBS_TemplateSelection.SetSelectedButton(0);
		}

		public void OnLevelWasLoaded(int level)
		{
			Debug.Log(SceneManager.GetActiveScene().name + " is the currently loaded scene");
			if (SceneManager.GetActiveScene().name == "UGS_Lobby")
			{
				Button_NewLevel.SetActive(value: true);
				Button_LoadLevel.SetActive(value: true);
				Button_CurrentLevel.SetActive(value: false);
				Button_BackToLobby.SetActive(value: false);
				SetPanelScreen(0);
			}
			else
			{
				Button_NewLevel.SetActive(value: false);
				Button_LoadLevel.SetActive(value: true);
				Button_CurrentLevel.SetActive(value: true);
				Button_BackToLobby.SetActive(value: true);
				SetPanelScreen(2);
			}
		}

		public void SetPanelScreen(int i)
		{
			m_curScreen = (LobbyPanelScreen)i;
			switch (m_curScreen)
			{
			case LobbyPanelScreen.CreateNewLevel:
				Page_CreateNewLevel.SetActive(value: true);
				Page_LoadExistingLevel.SetActive(value: false);
				Page_CurrentLevelParams.SetActive(value: false);
				UpdatePage_CreateNewLevel();
				Keyboard.gameObject.SetActive(value: true);
				break;
			case LobbyPanelScreen.LoadExistingLevel:
				Page_CreateNewLevel.SetActive(value: false);
				Page_LoadExistingLevel.SetActive(value: true);
				Page_CurrentLevelParams.SetActive(value: false);
				UpdatePage_LoadExistingLevel();
				Keyboard.gameObject.SetActive(value: false);
				break;
			case LobbyPanelScreen.CurrentLevelParams:
				Page_CreateNewLevel.SetActive(value: false);
				Page_LoadExistingLevel.SetActive(value: false);
				Page_CurrentLevelParams.SetActive(value: true);
				Keyboard.gameObject.SetActive(value: false);
				UpdatePage_CurrentLevel();
				break;
			}
		}

		public void Button_SetSceneToCreateFrom(string s)
		{
			m_curSceneToCreateFrom = s;
			Button_CreateNewLevel.SetActive(value: true);
		}

		public void Button_CreateLevelAndEnterEditMode()
		{
			if (m_curSceneToCreateFrom == string.Empty)
			{
				Debug.Log("Template is blank asshole");
				return;
			}
			if (!m_hasEnteredNewLevelName || TXT_NewLevelName.text == string.Empty)
			{
				Debug.Log("Player hasn't entered a level name yet");
				return;
			}
			m_curNewLevelName = TXT_NewLevelName.text;
			LM.CreateNewLevelAndSetToCurrent(m_curNewLevelName, m_curSceneToCreateFrom);
			LM.SaveCurrentLevel();
			LM.LoadCurrentLevelIntoEditMode();
		}

		public void Button_LoadExistingLevelEditMode()
		{
			LM.SetCurrentLevelToFile(Labels_ExistingLevels[m_curExistingLevelIndex].FileName);
			LM.LoadCurrentLevelIntoEditMode();
		}

		public void Button_LoadExistingLevelPlayMode()
		{
			LM.SetCurrentLevelToFile(Labels_ExistingLevels[m_curExistingLevelIndex].FileName);
			LM.LoadCurrentLevelIntoPlayMode();
		}

		public void Button_SetKeyboardInputToNewLevelName()
		{
			TXT_NewLevelName.text = string.Empty;
			Keyboard.ActiveText = TXT_NewLevelName;
			m_hasEnteredNewLevelName = true;
		}

		public void Button_SetExistingLevelToLoad(int i)
		{
			m_curExistingLevelIndex = i;
		}

		public void Button_DeleteExistingLevel()
		{
			string fileName = Labels_ExistingLevels[m_curExistingLevelIndex].FileName;
			if (ES2.Exists(fileName))
			{
				ES2.Delete(fileName);
			}
			OBS_ExistingLevelList.SetSelectedButton(0);
			UpdatePage_LoadExistingLevel();
		}

		public void Button_SaveCurrentLevel()
		{
			LM.SaveCurrentLevel();
		}

		public void Button_LoadLobby()
		{
			LM.CurrentLevel = null;
			SteamVR_LoadLevel.Begin("UGS_Lobby");
		}

		private void UpdatePage_CreateNewLevel()
		{
		}

		private void UpdatePage_LoadExistingLevel()
		{
			string[] files = ES2.GetFiles("./");
			List<string> list = new List<string>();
			for (int i = 0; i < files.Length; i++)
			{
				string text = files[i].Substring(0, 6);
				if (text == "Level_")
				{
					list.Add(files[i]);
				}
			}
			for (int j = 0; j < Labels_ExistingLevels.Length; j++)
			{
				if (j < list.Count)
				{
					Labels_ExistingLevels[j].gameObject.SetActive(value: true);
					Labels_ExistingLevels[j].FileName = list[j];
					using (ES2Reader eS2Reader = ES2Reader.Create(list[j]))
					{
						Labels_ExistingLevels[j].DisplayName = eS2Reader.Read<string>("displayName");
					}
					Labels_ExistingLevels[j].Label.text = Labels_ExistingLevels[j].DisplayName;
				}
				else
				{
					Labels_ExistingLevels[j].Label.text = string.Empty;
					Labels_ExistingLevels[j].FileName = string.Empty;
					Labels_ExistingLevels[j].DisplayName = string.Empty;
					Labels_ExistingLevels[j].gameObject.SetActive(value: false);
				}
			}
		}

		private void UpdatePage_CurrentLevel()
		{
			if (LM.State == LevelManagerState.EditMode)
			{
				Button_SaveLevel.SetActive(value: true);
				Keyboard.gameObject.SetActive(value: true);
			}
			else
			{
				Button_SaveLevel.SetActive(value: false);
				Keyboard.gameObject.SetActive(value: false);
			}
		}
	}
}
