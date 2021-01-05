// Decompiled with JetBrains decompiler
// Type: FistVR.UGSLobby_Panel
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FistVR
{
  public class UGSLobby_Panel : MonoBehaviour
  {
    private UGSLobby_Panel.LobbyPanelScreen m_curScreen;
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
      this.OBS_ExistingLevelList.SetSelectedButton(0);
      this.OnLevelWasLoaded(SceneManager.GetActiveScene().buildIndex);
      this.OBS_TemplateSelection.SetSelectedButton(0);
    }

    public void OnLevelWasLoaded(int level)
    {
      Debug.Log((object) (SceneManager.GetActiveScene().name + " is the currently loaded scene"));
      if (SceneManager.GetActiveScene().name == "UGS_Lobby")
      {
        this.Button_NewLevel.SetActive(true);
        this.Button_LoadLevel.SetActive(true);
        this.Button_CurrentLevel.SetActive(false);
        this.Button_BackToLobby.SetActive(false);
        this.SetPanelScreen(0);
      }
      else
      {
        this.Button_NewLevel.SetActive(false);
        this.Button_LoadLevel.SetActive(true);
        this.Button_CurrentLevel.SetActive(true);
        this.Button_BackToLobby.SetActive(true);
        this.SetPanelScreen(2);
      }
    }

    public void SetPanelScreen(int i)
    {
      this.m_curScreen = (UGSLobby_Panel.LobbyPanelScreen) i;
      switch (this.m_curScreen)
      {
        case UGSLobby_Panel.LobbyPanelScreen.CreateNewLevel:
          this.Page_CreateNewLevel.SetActive(true);
          this.Page_LoadExistingLevel.SetActive(false);
          this.Page_CurrentLevelParams.SetActive(false);
          this.UpdatePage_CreateNewLevel();
          this.Keyboard.gameObject.SetActive(true);
          break;
        case UGSLobby_Panel.LobbyPanelScreen.LoadExistingLevel:
          this.Page_CreateNewLevel.SetActive(false);
          this.Page_LoadExistingLevel.SetActive(true);
          this.Page_CurrentLevelParams.SetActive(false);
          this.UpdatePage_LoadExistingLevel();
          this.Keyboard.gameObject.SetActive(false);
          break;
        case UGSLobby_Panel.LobbyPanelScreen.CurrentLevelParams:
          this.Page_CreateNewLevel.SetActive(false);
          this.Page_LoadExistingLevel.SetActive(false);
          this.Page_CurrentLevelParams.SetActive(true);
          this.Keyboard.gameObject.SetActive(false);
          this.UpdatePage_CurrentLevel();
          break;
      }
    }

    public void Button_SetSceneToCreateFrom(string s)
    {
      this.m_curSceneToCreateFrom = s;
      this.Button_CreateNewLevel.SetActive(true);
    }

    public void Button_CreateLevelAndEnterEditMode()
    {
      if (this.m_curSceneToCreateFrom == string.Empty)
        Debug.Log((object) "Template is blank asshole");
      else if (!this.m_hasEnteredNewLevelName || this.TXT_NewLevelName.text == string.Empty)
      {
        Debug.Log((object) "Player hasn't entered a level name yet");
      }
      else
      {
        this.m_curNewLevelName = this.TXT_NewLevelName.text;
        LM.CreateNewLevelAndSetToCurrent(this.m_curNewLevelName, this.m_curSceneToCreateFrom);
        LM.SaveCurrentLevel();
        LM.LoadCurrentLevelIntoEditMode();
      }
    }

    public void Button_LoadExistingLevelEditMode()
    {
      LM.SetCurrentLevelToFile(this.Labels_ExistingLevels[this.m_curExistingLevelIndex].FileName);
      LM.LoadCurrentLevelIntoEditMode();
    }

    public void Button_LoadExistingLevelPlayMode()
    {
      LM.SetCurrentLevelToFile(this.Labels_ExistingLevels[this.m_curExistingLevelIndex].FileName);
      LM.LoadCurrentLevelIntoPlayMode();
    }

    public void Button_SetKeyboardInputToNewLevelName()
    {
      this.TXT_NewLevelName.text = string.Empty;
      this.Keyboard.ActiveText = this.TXT_NewLevelName;
      this.m_hasEnteredNewLevelName = true;
    }

    public void Button_SetExistingLevelToLoad(int i) => this.m_curExistingLevelIndex = i;

    public void Button_DeleteExistingLevel()
    {
      string fileName = this.Labels_ExistingLevels[this.m_curExistingLevelIndex].FileName;
      if (ES2.Exists(fileName))
        ES2.Delete(fileName);
      this.OBS_ExistingLevelList.SetSelectedButton(0);
      this.UpdatePage_LoadExistingLevel();
    }

    public void Button_SaveCurrentLevel() => LM.SaveCurrentLevel();

    public void Button_LoadLobby()
    {
      LM.CurrentLevel = (FVRLevelData) null;
      SteamVR_LoadLevel.Begin("UGS_Lobby");
    }

    private void UpdatePage_CreateNewLevel()
    {
    }

    private void UpdatePage_LoadExistingLevel()
    {
      string[] files = ES2.GetFiles("./");
      List<string> stringList = new List<string>();
      for (int index = 0; index < files.Length; ++index)
      {
        if (files[index].Substring(0, 6) == "Level_")
          stringList.Add(files[index]);
      }
      for (int index = 0; index < this.Labels_ExistingLevels.Length; ++index)
      {
        if (index < stringList.Count)
        {
          this.Labels_ExistingLevels[index].gameObject.SetActive(true);
          this.Labels_ExistingLevels[index].FileName = stringList[index];
          using (ES2Reader es2Reader = ES2Reader.Create(stringList[index]))
            this.Labels_ExistingLevels[index].DisplayName = es2Reader.Read<string>("displayName");
          this.Labels_ExistingLevels[index].Label.text = this.Labels_ExistingLevels[index].DisplayName;
        }
        else
        {
          this.Labels_ExistingLevels[index].Label.text = string.Empty;
          this.Labels_ExistingLevels[index].FileName = string.Empty;
          this.Labels_ExistingLevels[index].DisplayName = string.Empty;
          this.Labels_ExistingLevels[index].gameObject.SetActive(false);
        }
      }
    }

    private void UpdatePage_CurrentLevel()
    {
      if (LM.State == LevelManagerState.EditMode)
      {
        this.Button_SaveLevel.SetActive(true);
        this.Keyboard.gameObject.SetActive(true);
      }
      else
      {
        this.Button_SaveLevel.SetActive(false);
        this.Keyboard.gameObject.SetActive(false);
      }
    }

    public enum LobbyPanelScreen
    {
      CreateNewLevel,
      LoadExistingLevel,
      CurrentLevelParams,
    }
  }
}
