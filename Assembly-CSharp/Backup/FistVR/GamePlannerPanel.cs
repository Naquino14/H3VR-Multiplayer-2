// Decompiled with JetBrains decompiler
// Type: FistVR.GamePlannerPanel
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class GamePlannerPanel : FVRPhysicalObject
  {
    [Header("GamePlannerPanel")]
    public Transform AimAperture;
    private float m_beamWidth = 0.005f;
    public AudioEvent AudEvent_Beep;
    public AudioEvent AudEvent_Boop;
    public AudioEvent AudEvent_Err;
    private GamePlannerPanel.PanelPageMode m_pageMode;
    private GPSceneMode m_scenemode;
    [Header("Main Panel Stuff")]
    public Text LBL_PanelState;
    public List<GameObject> BTN_TopBarList;
    [Header("Mode Panel")]
    public GameObject Page_Mode;
    public GameObject BTN_Mode_Switch;
    public GameObject BTN_Mode_Confirm;
    public GameObject BTN_Mode_Cancel;
    public Text LBL_Mode_CurrentMode;
    public Text LBL_Mode_Switch;
    public List<GameObject> Mode_HelpPages;
    private bool m_mode_isConfirmCancel;
    [Header("Scene Panel")]
    public GameObject Page_Scene;
    [Header("Spawn Panel")]
    public GameObject Page_Spawn;
    public Text LBL_Spawn_PickCategory;
    public Text LBL_Spawn_CatSubCatName;
    public Text LBL_Spawn_PageNumber;
    public List<Text> LBL_Spawn_CatSubCats;
    public List<Text> LBL_Spawn_Items;
    public List<GameObject> GO_Spawn_CatSubCats;
    public List<GameObject> GO_Spawn_Items;
    public GameObject GO_Spawn_Page_Next;
    public GameObject GO_Spawn_Page_Prev;
    public GamePlannerPanel.SpawnPageMode SpawnMode;
    public ItemSpawnerCategoryDefinitions Spawn_DefFile_Play;
    public ItemSpawnerCategoryDefinitions Spawn_DefFile_Design;
    private ItemSpawnerCategoryDefinitions m_curDefFile;
    private ItemSpawnerID.EItemCategory m_curCat;
    private int m_curCatIndex;
    private ItemSpawnerID.ESubCategory m_curSubCat;
    private int m_curSubCatIndex;
    private ItemSpawnerID m_curSelectedID;
    public OptionsPanel_ButtonSet OBS_Spawn_Item;
    public Text LBL_Spawn_Guides;
    public LayerMask LM_SpawnPlace_Env;
    public LayerMask LM_SpawnPlace_All;
    private float m_spawnMaxDistance = 60f;
    public Transform SpawnRayBeam;
    private RaycastHit m_spawnHit;
    private bool m_hasSpawnPoint;
    private Vector3 m_spawnPos;
    private float m_currentSpawnRot;
    private float[] spawnGridsLinear = new float[4]
    {
      0.1f,
      0.5f,
      1f,
      5f
    };
    private float[] spawnGridsAngular = new float[4]
    {
      5f,
      15f,
      45f,
      90f
    };
    private int m_spawnGridLinearIndex = 2;
    private int m_spawnGridAngularIndex = 2;
    private bool m_SpawnSnapGuides;
    [Header("Tools Panel")]
    public GameObject Page_Tools;
    private GamePlannerPanel.Tool m_toolState;
    public GameObject ToolsPage_ManipBeam;
    public GameObject ToolsPage_ParamBeam;
    public GameObject ToolsPage_ResetBeam;
    public GameObject ToolsPage_DeleteBeam;
    [Header("Manipulator")]
    public GameObject ManipulatorPrefab;
    private GamePlannerManipulator m_tools_Manipulator;
    public LayerMask LM_Tools_ManipulatorUI;
    public LayerMask LM_Tools_ManupulatorObjects;
    private float m_maxManipBeamRange = 20f;
    private RaycastHit m_manipHit;
    private GPManipulatorButton m_currentManipButton;
    public GamePlannerPanel.ManipulatorMode ManipMode;
    public GamePlannerPanel.ManipulatorAxis ManipAxis_Nudge;
    public GamePlannerPanel.ManipulatorAxis ManipAxis_Shunt;
    public GamePlannerPanel.ManipulatorAxis ManipAxis_Rotate;
    public GamePlannerPanel.ManipulatorRelativeTo ManipRelativeTo_Nudge;
    private int ManipNudgeGrid_Index;
    private int ManipShuntMax_Index;
    private int ManipRotateSnap_Index = 4;
    private float[] m_manipNudgeGridIntervals = new float[5]
    {
      0.1f,
      0.25f,
      0.5f,
      1f,
      2f
    };
    private float[] m_manipShuntMaxIntervals = new float[5]
    {
      0.5f,
      1f,
      2f,
      5f,
      10f
    };
    private float[] m_manipRotateSnapIntervals = new float[5]
    {
      1f,
      5f,
      15f,
      45f,
      90f
    };
    [Header("Sosig Panel")]
    public GameObject Page_Sosig;
    [Header("Load/Save Panel")]
    public GameObject Page_LoadSave;
    private GamePlannerPanel.LoadSaveTextEntry m_loadSaveTextEntry;
    public Text LBL_LoadSave_Top;
    public Text LBL_LoadSave_Description;
    public Text LBL_LoadSave_ListPage;
    public Text LBL_LoadSave_FileName;
    public List<Text> LBL_LoadSave_FileNameList;
    public GameObject BTN_LoadSave_PageNext;
    public GameObject BTN_LoadSave_PagePrev;
    public GameObject BTN_LoadSave_Load;
    public GameObject BTN_LoadSave_Save;
    public GameObject BTN_LoadSave_SaveAs;
    public GameObject BTN_LoadSave_Delete;
    public GameObject BTN_LoadSave_ConfirmSave;
    public GameObject BTN_LoadSave_ConfirmDelete;
    public GameObject BTN_LoadSave_EditDescription;
    public GameObject BTN_LoadSave_EditFilename;
    public GameObject LoadSave_Keyboard;
    private int m_saveload_curfilepage;
    private int m_saveload_maxfilepage;
    private string m_filenameEntry = string.Empty;
    private string m_curDescriptionText = string.Empty;
    private bool m_isLoadSaveConfirm;
    private bool m_isLoadSaveDeleteConfirm;
    private List<string> m_LoadSave_Filelist = new List<string>();
    private int m_selectedFileToLoadIndex = -1;
    private int m_selectedFileToSaveIndex = -1;

    protected override void Start()
    {
      base.Start();
      this.Button_TopBar(0);
      this.UpdateSceneModeState();
      this.PageSwitchCleanup();
    }

    private void UpdateSceneModeState()
    {
      this.m_scenemode = GM.CurrentGamePlannerManager.SMode;
      this.UpdateTopBar(this.m_scenemode == GPSceneMode.Play);
    }

    private void Beep() => SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Beep, this.transform.position);

    private void Boop() => SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Boop, this.transform.position);

    private void Err() => SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Err, this.transform.position);

    private void SetPageMode(GamePlannerPanel.PanelPageMode mode)
    {
      this.PageSwitchCleanup();
      this.m_pageMode = mode;
      switch (this.m_pageMode)
      {
        case GamePlannerPanel.PanelPageMode.Mode:
          this.SetPageMode_Mode();
          break;
        case GamePlannerPanel.PanelPageMode.Spawn:
          this.SetPageMode_Spawn();
          break;
        case GamePlannerPanel.PanelPageMode.Tools:
          this.SetPageMode_Tools();
          break;
        case GamePlannerPanel.PanelPageMode.Load:
          this.SetPageMode_Load();
          break;
        case GamePlannerPanel.PanelPageMode.Save:
          this.SetPageMode_Save();
          break;
      }
    }

    private void PageSwitchCleanup()
    {
      this.PageSwitchCleanup_Mode();
      this.PageSwitchCleanup_Spawn();
      this.PageSwitchCleanup_Tools();
      this.PageSwitchCleanup_LoadSave();
    }

    private void UpdateTopBar(bool isPlayMode)
    {
      if (isPlayMode)
      {
        this.LBL_PanelState.text = "GAMEPLANNER PANEL - PLAY MODE - " + this.m_pageMode.ToString();
        for (int index = 0; index < this.BTN_TopBarList.Count; ++index)
        {
          if (index == 3 || index == 6)
            this.BTN_TopBarList[index].SetActive(false);
          else
            this.BTN_TopBarList[index].SetActive(true);
        }
      }
      else
      {
        this.LBL_PanelState.text = "GAMEPLANNER PANEL - DESIGN MODE - " + this.m_pageMode.ToString();
        for (int index = 0; index < this.BTN_TopBarList.Count; ++index)
        {
          if (index == 4)
            this.BTN_TopBarList[index].SetActive(false);
          else
            this.BTN_TopBarList[index].SetActive(true);
        }
      }
    }

    public void Button_TopBar(int i)
    {
      this.Beep();
      if ((Object) this.Page_Mode != (Object) null)
        this.Page_Mode.SetActive(false);
      if ((Object) this.Page_Scene != (Object) null)
        this.Page_Scene.SetActive(false);
      if ((Object) this.Page_Spawn != (Object) null)
        this.Page_Spawn.SetActive(false);
      if ((Object) this.Page_Tools != (Object) null)
        this.Page_Tools.SetActive(false);
      if ((Object) this.Page_Sosig != (Object) null)
        this.Page_Sosig.SetActive(false);
      if ((Object) this.Page_LoadSave != (Object) null)
        this.Page_LoadSave.SetActive(false);
      this.SetPageMode((GamePlannerPanel.PanelPageMode) i);
    }

    public void Button_Mode_Switch()
    {
      this.Beep();
      this.SetConfirmCancel(true);
    }

    public void Button_Mode_ConfirmCancel(bool isConfirm)
    {
      if (!this.m_mode_isConfirmCancel)
      {
        this.Err();
      }
      else
      {
        this.Beep();
        this.SetConfirmCancel(false);
        if (!isConfirm)
          return;
        GM.CurrentGamePlannerManager.ToggleSceneMode();
        this.UpdateSceneModeState();
      }
    }

    public void Button_Mode_Help(int p)
    {
      this.Beep();
      for (int index = 0; index < this.Mode_HelpPages.Count; ++index)
      {
        if (index == p)
          this.Mode_HelpPages[index].SetActive(true);
        else
          this.Mode_HelpPages[index].SetActive(false);
      }
    }

    private void SetPageMode_Mode() => this.Page_Mode.SetActive(true);

    private void PageSwitchCleanup_Mode()
    {
      this.SetConfirmCancel(false);
      this.Mode_UpdateDisplay();
    }

    private void Mode_UpdateDisplay()
    {
      if (GM.CurrentGamePlannerManager.SMode == GPSceneMode.Play)
      {
        this.LBL_Mode_CurrentMode.text = "Current Mode: Play";
        this.LBL_Mode_Switch.text = "Switch to Design Mode";
      }
      else
      {
        this.LBL_Mode_CurrentMode.text = "Current Mode: Design";
        this.LBL_Mode_Switch.text = "Switch to Play Mode";
      }
    }

    private void SetConfirmCancel(bool b)
    {
      this.m_mode_isConfirmCancel = b;
      this.BTN_Mode_Confirm.SetActive(b);
      this.BTN_Mode_Cancel.SetActive(b);
    }

    private void Spawn_Update()
    {
      if (!this.IsHeld)
        return;
      if ((Object) this.m_curSelectedID != (Object) null)
      {
        this.SpawnRayBeam.gameObject.SetActive(true);
        float z = this.m_spawnMaxDistance;
        if (Physics.Raycast(this.AimAperture.position, this.AimAperture.forward, out this.m_spawnHit, this.m_spawnMaxDistance, (int) this.LM_SpawnPlace_Env, QueryTriggerInteraction.Ignore))
        {
          z = this.m_spawnHit.distance;
          this.m_spawnPos = this.m_spawnHit.point;
          this.m_hasSpawnPoint = true;
        }
        else
          this.m_hasSpawnPoint = false;
        this.SpawnRayBeam.localScale = new Vector3(this.m_beamWidth, this.m_beamWidth, z);
      }
      else
      {
        this.m_hasSpawnPoint = false;
        this.SpawnRayBeam.gameObject.SetActive(false);
      }
      if (!this.m_hasSpawnPoint || !this.m_hand.Input.TriggerDown)
        return;
      this.SpawnCurrentItemSpawnerID(this.m_curSelectedID, this.m_spawnPos);
    }

    private void SpawnCurrentItemSpawnerID(ItemSpawnerID id, Vector3 p)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(id.MainObject.GetGameObject(), p, Quaternion.identity);
      if (this.m_scenemode != GPSceneMode.Design)
        return;
      GM.CurrentGamePlannerManager.RegisterAndInitSpawnedPlaceable(gameObject.GetComponent<GPPlaceable>());
    }

    private void SetPageMode_Spawn() => this.Page_Spawn.SetActive(true);

    public void Button_Spawn_SetLinearGrid(int i)
    {
    }

    public void Button_Spawn_SetAngularGrid(int i)
    {
    }

    public void Button_Spawn_ToggleGuides()
    {
      this.m_SpawnSnapGuides = !this.m_SpawnSnapGuides;
      this.Beep();
      if (this.m_SpawnSnapGuides)
        this.LBL_Spawn_Guides.text = "Enabled";
      else
        this.LBL_Spawn_Guides.text = "Disabled";
    }

    public void Button_Spawn_CatSubCat(int i)
    {
      this.Beep();
      if (this.SpawnMode == GamePlannerPanel.SpawnPageMode.CategoryList)
      {
        this.m_curCat = this.m_curDefFile.Categories[i].Cat;
        this.m_curCatIndex = i;
        this.m_curSubCat = this.m_curDefFile.Categories[i].Subcats[0].Subcat;
        this.m_curSubCatIndex = 0;
        this.m_curSelectedID = (ItemSpawnerID) null;
        this.OBS_Spawn_Item.SetSelectedButton(0);
        this.SpawnMode = GamePlannerPanel.SpawnPageMode.SubCategoryList;
      }
      else if (this.SpawnMode == GamePlannerPanel.SpawnPageMode.SubCategoryList)
      {
        if (i == 9)
        {
          this.SpawnMode = GamePlannerPanel.SpawnPageMode.CategoryList;
          this.m_curSelectedID = (ItemSpawnerID) null;
          this.m_curSubCat = ItemSpawnerID.ESubCategory.None;
          this.m_curSubCatIndex = 0;
          this.Spawn_UpdateDisplay();
          return;
        }
        this.m_curSubCat = this.m_curDefFile.Categories[this.m_curCatIndex].Subcats[i].Subcat;
        this.m_curSubCatIndex = i;
        this.OBS_Spawn_Item.SetSelectedButton(0);
        Debug.Log((object) "fired");
        this.m_curSelectedID = IM.SCD[this.m_curSubCat].Count <= 0 ? (ItemSpawnerID) null : IM.SCD[this.m_curSubCat][0];
      }
      this.Spawn_UpdateDisplay();
    }

    public void Button_Spawn_Item(int i)
    {
      this.Boop();
      this.Spawn_UpdateDisplay();
    }

    private void PageSwitchCleanup_Spawn()
    {
      this.m_curDefFile = this.m_scenemode != GPSceneMode.Play ? this.Spawn_DefFile_Design : this.Spawn_DefFile_Play;
      this.SpawnMode = GamePlannerPanel.SpawnPageMode.CategoryList;
      this.m_curCat = this.m_curDefFile.Categories[0].Cat;
      this.m_curSubCat = this.m_curDefFile.Categories[0].Subcats[0].Subcat;
      this.m_curCatIndex = 0;
      this.m_curSubCatIndex = 0;
      this.m_curSelectedID = (ItemSpawnerID) null;
      this.OBS_Spawn_Item.SetSelectedButton(0);
      this.SpawnRayBeam.gameObject.SetActive(false);
      this.Spawn_UpdateDisplay();
    }

    private void Spawn_UpdateDisplay()
    {
      if (this.SpawnMode == GamePlannerPanel.SpawnPageMode.CategoryList)
      {
        for (int index = 0; index < this.GO_Spawn_CatSubCats.Count; ++index)
        {
          if (index < this.m_curDefFile.Categories.Length)
          {
            this.GO_Spawn_CatSubCats[index].SetActive(true);
            this.LBL_Spawn_CatSubCats[index].text = this.m_curDefFile.Categories[index].DisplayName;
          }
          else
            this.GO_Spawn_CatSubCats[index].SetActive(false);
        }
        this.LBL_Spawn_PickCategory.text = "Pick Category:";
        this.LBL_Spawn_CatSubCatName.text = string.Empty;
        for (int index = 0; index < this.GO_Spawn_Items.Count; ++index)
          this.GO_Spawn_Items[index].SetActive(false);
      }
      else
      {
        for (int index = 0; index < this.GO_Spawn_CatSubCats.Count - 1; ++index)
        {
          if (index < this.m_curDefFile.Categories[this.m_curCatIndex].Subcats.Length)
          {
            this.GO_Spawn_CatSubCats[index].SetActive(true);
            this.LBL_Spawn_CatSubCats[index].text = this.m_curDefFile.Categories[this.m_curCatIndex].Subcats[index].DisplayName;
          }
          else
            this.GO_Spawn_CatSubCats[index].SetActive(false);
        }
        this.GO_Spawn_CatSubCats[9].SetActive(true);
        this.LBL_Spawn_CatSubCats[9].text = "<< Back <<";
        this.LBL_Spawn_PickCategory.text = this.m_curDefFile.Categories[this.m_curCatIndex].DisplayName + ";";
        this.LBL_Spawn_CatSubCatName.text = this.m_curDefFile.Categories[this.m_curCatIndex].Subcats[this.m_curSubCatIndex].DisplayName;
        for (int index = 0; index < this.GO_Spawn_Items.Count; ++index)
        {
          if (index < IM.SCD[this.m_curSubCat].Count)
          {
            this.GO_Spawn_Items[index].SetActive(true);
            this.LBL_Spawn_Items[index].text = IM.SCD[this.m_curSubCat][index].DisplayName;
          }
          else
            this.GO_Spawn_Items[index].SetActive(false);
        }
      }
    }

    public GPManipulatorButton CurrentManipButton
    {
      get => this.m_currentManipButton;
      set
      {
        if ((Object) this.m_currentManipButton == (Object) value)
        {
          if (!((Object) this.m_currentManipButton != (Object) null))
            return;
          this.m_currentManipButton.OnPoint();
        }
        else
        {
          if ((Object) this.m_currentManipButton != (Object) null)
            this.m_currentManipButton.EndPoint();
          this.m_currentManipButton = value;
          if (!((Object) this.m_currentManipButton != (Object) null))
            return;
          this.m_currentManipButton.OnPoint();
        }
      }
    }

    public void Button_SetManipMode(int i)
    {
      this.ManipMode = (GamePlannerPanel.ManipulatorMode) i;
      this.m_tools_Manipulator.UpdateManipulatorFrame(this.ManipMode);
    }

    public void Button_Manip_ResetRotation() => this.m_tools_Manipulator.ResetRotation();

    public float GetManipNudgeInterval() => this.m_manipNudgeGridIntervals[this.ManipNudgeGrid_Index];

    public float GetManipShuntInterval() => this.m_manipShuntMaxIntervals[this.ManipShuntMax_Index];

    public float GetManipRotateInterval() => this.m_manipRotateSnapIntervals[this.ManipRotateSnap_Index];

    public void Button_Manip_SetNudgeGridInterval(int i) => this.ManipNudgeGrid_Index = i;

    public void Button_Manip_SetShuntMaxInterval(int i) => this.ManipShuntMax_Index = i;

    public void Button_Manip_SetRotateSnapInterval(int i) => this.ManipRotateSnap_Index = i;

    public void Button_Manip_SetAxis_Nudge(int i)
    {
      this.ManipAxis_Nudge = (GamePlannerPanel.ManipulatorAxis) i;
      this.m_tools_Manipulator.UpdateManipulatorFrame(this.ManipMode);
    }

    public void Button_Manip_SetAxis_Shunt(int i)
    {
      this.ManipAxis_Shunt = (GamePlannerPanel.ManipulatorAxis) i;
      this.m_tools_Manipulator.UpdateManipulatorFrame(this.ManipMode);
    }

    public void Button_Manip_SetAxis_Rotate(int i)
    {
      this.ManipAxis_Rotate = (GamePlannerPanel.ManipulatorAxis) i;
      this.m_tools_Manipulator.UpdateManipulatorFrame(this.ManipMode);
    }

    public void Button_Manip_SetRelativeTo_Nudge(int i)
    {
      this.ManipRelativeTo_Nudge = (GamePlannerPanel.ManipulatorRelativeTo) i;
      this.m_tools_Manipulator.UpdateManipulatorFrame(this.ManipMode);
    }

    private void SetPageMode_Tools() => this.Page_Tools.SetActive(true);

    private void PageSwitchCleanup_Tools()
    {
      this.m_toolState = GamePlannerPanel.Tool.None;
      if (!((Object) this.m_tools_Manipulator == (Object) null))
        return;
      GameObject gameObject = Object.Instantiate<GameObject>(this.ManipulatorPrefab, Vector3.zero, Quaternion.identity);
      this.m_tools_Manipulator = gameObject.GetComponent<GamePlannerManipulator>();
      this.m_tools_Manipulator.Panel = this;
      gameObject.SetActive(false);
    }

    private void Tools_Cleanup()
    {
      if (!((Object) this.m_tools_Manipulator != (Object) null))
        return;
      this.m_tools_Manipulator.SetControlledObject((Transform) null);
      this.m_tools_Manipulator.gameObject.SetActive(false);
    }

    public void Button_Tool_SelectTool(int i)
    {
      this.Beep();
      this.Tools_Cleanup();
      this.m_toolState = (GamePlannerPanel.Tool) i;
      switch (this.m_toolState)
      {
        case GamePlannerPanel.Tool.None:
          this.SpawnRayBeam.gameObject.SetActive(false);
          this.ToolsPage_ManipBeam.SetActive(false);
          break;
        case GamePlannerPanel.Tool.ManipBeam:
          this.SpawnRayBeam.gameObject.SetActive(true);
          this.ToolsPage_ManipBeam.SetActive(true);
          break;
        case GamePlannerPanel.Tool.ParamBeam:
          this.SpawnRayBeam.gameObject.SetActive(true);
          this.ToolsPage_ManipBeam.SetActive(false);
          break;
        case GamePlannerPanel.Tool.ResetBeam:
          this.SpawnRayBeam.gameObject.SetActive(true);
          this.ToolsPage_ManipBeam.SetActive(false);
          break;
        case GamePlannerPanel.Tool.DeleteBeam:
          this.SpawnRayBeam.gameObject.SetActive(true);
          this.ToolsPage_ManipBeam.SetActive(false);
          break;
      }
    }

    private void Tools_Update()
    {
      switch (this.m_toolState)
      {
        case GamePlannerPanel.Tool.ManipBeam:
          this.Tool_Update_ManipBeam();
          break;
        case GamePlannerPanel.Tool.ParamBeam:
          this.Tool_Update_ParamBeam();
          break;
        case GamePlannerPanel.Tool.ResetBeam:
          this.Tool_Update_ResetBeam();
          break;
        case GamePlannerPanel.Tool.DeleteBeam:
          this.Tool_Update_DeleteBeam();
          break;
      }
    }

    private void Tool_Update_ManipBeam()
    {
      if (!this.IsHeld)
        return;
      float z = this.m_maxManipBeamRange;
      bool flag1 = false;
      bool flag2 = false;
      if (Physics.Raycast(this.AimAperture.position, this.AimAperture.forward, out this.m_manipHit, this.m_maxManipBeamRange, (int) this.LM_Tools_ManipulatorUI, QueryTriggerInteraction.Collide) && (Object) this.m_manipHit.collider.transform.GetComponent<GPManipulatorButton>() != (Object) null)
      {
        z = this.m_manipHit.distance;
        flag1 = true;
        flag2 = true;
        this.CurrentManipButton = this.m_manipHit.collider.transform.GetComponent<GPManipulatorButton>();
      }
      else if (Physics.Raycast(this.AimAperture.position, this.AimAperture.forward, out this.m_manipHit, this.m_maxManipBeamRange, (int) this.LM_Tools_ManupulatorObjects, QueryTriggerInteraction.Ignore))
      {
        this.CurrentManipButton = (GPManipulatorButton) null;
        z = this.m_manipHit.distance;
        flag1 = true;
      }
      else
        this.CurrentManipButton = (GPManipulatorButton) null;
      if (flag1 && this.m_hand.Input.TriggerDown)
      {
        if (flag2)
        {
          this.CurrentManipButton.Button.onClick.Invoke();
          this.Beep();
        }
        else if ((Object) this.m_manipHit.transform.root.gameObject.GetComponent<GPPlaceable>() != (Object) null)
        {
          this.m_tools_Manipulator.gameObject.SetActive(true);
          this.m_tools_Manipulator.SetControlledObject(this.m_manipHit.transform.root);
          this.m_tools_Manipulator.transform.position = this.m_manipHit.transform.position;
          this.Boop();
        }
      }
      this.SpawnRayBeam.localScale = new Vector3(this.m_beamWidth, this.m_beamWidth, z);
    }

    private void Tool_Update_ParamBeam()
    {
      if (!this.IsHeld)
        return;
      float z = this.m_maxManipBeamRange;
      if (Physics.Raycast(this.AimAperture.position, this.AimAperture.forward, out this.m_manipHit, this.m_maxManipBeamRange, (int) this.LM_Tools_ManupulatorObjects, QueryTriggerInteraction.Ignore))
      {
        z = this.m_manipHit.distance;
        this.m_manipHit.collider.transform.root.gameObject.GetComponent<GPPlaceable>();
      }
      this.SpawnRayBeam.localScale = new Vector3(this.m_beamWidth, this.m_beamWidth, z);
    }

    private void Tool_Update_ResetBeam()
    {
      if (!this.IsHeld)
        return;
      float z = this.m_maxManipBeamRange;
      if (Physics.Raycast(this.AimAperture.position, this.AimAperture.forward, out this.m_manipHit, this.m_maxManipBeamRange, (int) this.LM_Tools_ManupulatorObjects, QueryTriggerInteraction.Ignore))
      {
        z = this.m_manipHit.distance;
        GM.CurrentGamePlannerManager.ResetPlaceable(this.m_manipHit.collider.transform.root.gameObject.GetComponent<GPPlaceable>());
      }
      this.SpawnRayBeam.localScale = new Vector3(this.m_beamWidth, this.m_beamWidth, z);
    }

    private void Tool_Update_DeleteBeam()
    {
      if (!this.IsHeld)
        return;
      float z = this.m_maxManipBeamRange;
      if (Physics.Raycast(this.AimAperture.position, this.AimAperture.forward, out this.m_manipHit, this.m_maxManipBeamRange, (int) this.LM_Tools_ManupulatorObjects, QueryTriggerInteraction.Ignore))
      {
        z = this.m_manipHit.distance;
        GM.CurrentGamePlannerManager.DeregisterAndDeletePlaceable(this.m_manipHit.collider.transform.root.gameObject.GetComponent<GPPlaceable>());
      }
      this.SpawnRayBeam.localScale = new Vector3(this.m_beamWidth, this.m_beamWidth, z);
    }

    private void PageSwitchCleanup_LoadSave()
    {
      this.m_saveload_curfilepage = 0;
      this.m_saveload_maxfilepage = 0;
      this.m_filenameEntry = string.Empty;
      this.m_curDescriptionText = string.Empty;
      this.m_isLoadSaveConfirm = false;
      this.m_isLoadSaveDeleteConfirm = false;
      this.m_LoadSave_Filelist.Clear();
      this.m_selectedFileToLoadIndex = -1;
      this.m_selectedFileToSaveIndex = -1;
    }

    public void Button_LoadSave_FileSelect(int f)
    {
      if (this.m_pageMode == GamePlannerPanel.PanelPageMode.Load)
      {
        this.m_selectedFileToLoadIndex = f + 12 * this.m_saveload_curfilepage;
        this.LBL_LoadSave_Description.text = this.m_LoadSave_Filelist[this.m_selectedFileToLoadIndex] + "\n" + GM.CurrentGamePlannerManager.GetDescription(this.m_LoadSave_Filelist[this.m_selectedFileToLoadIndex]);
        this.BTN_LoadSave_EditDescription.SetActive(false);
        this.BTN_LoadSave_EditFilename.SetActive(false);
        this.BTN_LoadSave_Save.SetActive(false);
        this.BTN_LoadSave_SaveAs.SetActive(false);
        this.BTN_LoadSave_ConfirmSave.SetActive(false);
        this.BTN_LoadSave_ConfirmDelete.SetActive(false);
        this.BTN_LoadSave_Load.SetActive(true);
      }
      else if (this.m_pageMode == GamePlannerPanel.PanelPageMode.Save)
      {
        this.m_selectedFileToSaveIndex = f + 12 * this.m_saveload_curfilepage;
        this.m_filenameEntry = this.m_LoadSave_Filelist[this.m_selectedFileToSaveIndex];
        this.LBL_LoadSave_FileName.text = this.m_filenameEntry;
        this.LBL_LoadSave_Description.text = this.m_LoadSave_Filelist[this.m_selectedFileToLoadIndex] + "\n" + GM.CurrentGamePlannerManager.GetDescription(this.m_LoadSave_Filelist[this.m_selectedFileToLoadIndex]);
        this.m_loadSaveTextEntry = GamePlannerPanel.LoadSaveTextEntry.None;
        this.BTN_LoadSave_EditDescription.SetActive(false);
        this.BTN_LoadSave_EditFilename.SetActive(false);
        this.BTN_LoadSave_ConfirmSave.SetActive(false);
        this.BTN_LoadSave_ConfirmDelete.SetActive(false);
        this.BTN_LoadSave_Load.SetActive(false);
        this.BTN_LoadSave_Save.SetActive(true);
        this.BTN_LoadSave_SaveAs.SetActive(false);
      }
      this.Beep();
      this.m_isLoadSaveConfirm = false;
      this.m_isLoadSaveDeleteConfirm = false;
      this.BTN_LoadSave_ConfirmSave.SetActive(false);
      this.BTN_LoadSave_ConfirmDelete.SetActive(false);
    }

    public void Button_LoadSave_EditDescription()
    {
      this.m_isLoadSaveConfirm = false;
      this.m_isLoadSaveDeleteConfirm = false;
      this.BTN_LoadSave_ConfirmSave.SetActive(false);
      this.BTN_LoadSave_ConfirmDelete.SetActive(false);
      this.m_loadSaveTextEntry = GamePlannerPanel.LoadSaveTextEntry.Description;
      this.BTN_LoadSave_EditDescription.SetActive(false);
      this.BTN_LoadSave_EditFilename.SetActive(true);
      this.Beep();
    }

    public void Button_LoadSave_EditFilename()
    {
      this.m_isLoadSaveConfirm = false;
      this.m_isLoadSaveDeleteConfirm = false;
      this.BTN_LoadSave_ConfirmSave.SetActive(false);
      this.BTN_LoadSave_ConfirmDelete.SetActive(false);
      this.m_loadSaveTextEntry = GamePlannerPanel.LoadSaveTextEntry.FileName;
      this.BTN_LoadSave_EditFilename.SetActive(false);
      this.BTN_LoadSave_EditDescription.SetActive(true);
      this.Beep();
    }

    public void Button_LoadSave_LoadScenario()
    {
      this.m_isLoadSaveConfirm = false;
      this.m_isLoadSaveDeleteConfirm = false;
      this.BTN_LoadSave_ConfirmSave.SetActive(false);
      this.BTN_LoadSave_ConfirmDelete.SetActive(false);
      if (this.m_selectedFileToLoadIndex < 0)
        this.Err();
      else if (GM.CurrentGamePlannerManager.LoadScenario(this.m_LoadSave_Filelist[this.m_selectedFileToLoadIndex]))
        this.Boop();
      else
        this.Err();
    }

    public void Button_LoadSave_Save()
    {
      this.m_isLoadSaveConfirm = true;
      this.m_isLoadSaveDeleteConfirm = false;
      this.BTN_LoadSave_ConfirmSave.SetActive(true);
      this.BTN_LoadSave_ConfirmDelete.SetActive(false);
      this.Beep();
      this.LoadSave_RefreshFileList();
    }

    public void Button_LoadSave_SaveAs()
    {
      this.m_isLoadSaveConfirm = true;
      this.m_isLoadSaveDeleteConfirm = false;
      this.BTN_LoadSave_ConfirmSave.SetActive(true);
      this.BTN_LoadSave_ConfirmDelete.SetActive(false);
      this.Beep();
      this.LoadSave_RefreshFileList();
    }

    public void Button_LoadSave_ConfirmSave()
    {
      this.m_isLoadSaveConfirm = false;
      this.m_isLoadSaveDeleteConfirm = false;
      this.BTN_LoadSave_ConfirmSave.SetActive(false);
      this.BTN_LoadSave_ConfirmDelete.SetActive(false);
      GM.CurrentGamePlannerManager.SaveActiveScenario(this.m_filenameEntry, this.m_curDescriptionText);
      this.m_saveload_curfilepage = 0;
      this.LoadSave_RefreshFileList();
      this.LoadSave_CalcMaxPage();
      this.Boop();
    }

    public void Button_LoadSave_Delete()
    {
      this.m_isLoadSaveConfirm = false;
      this.m_isLoadSaveDeleteConfirm = true;
      this.BTN_LoadSave_ConfirmSave.SetActive(false);
      this.BTN_LoadSave_ConfirmDelete.SetActive(true);
      this.Beep();
      this.LoadSave_RefreshFileList();
    }

    public void Button_LoadSave_ConfirmDelete()
    {
      if (GM.CurrentGamePlannerManager.DeleteScenario(this.m_filenameEntry))
      {
        this.m_isLoadSaveConfirm = false;
        this.m_isLoadSaveDeleteConfirm = false;
        this.BTN_LoadSave_ConfirmSave.SetActive(false);
        this.BTN_LoadSave_ConfirmDelete.SetActive(false);
        this.m_saveload_curfilepage = 0;
        this.LoadSave_RefreshFileList();
        this.LoadSave_CalcMaxPage();
        this.PageSwitchCleanup_LoadSave();
        this.Boop();
      }
      else
        this.Err();
    }

    public void Button_LoadSave_PageNext()
    {
      ++this.m_saveload_curfilepage;
      if (this.m_saveload_curfilepage > this.m_saveload_maxfilepage)
        this.m_saveload_curfilepage = this.m_saveload_maxfilepage;
      this.LoadSave_RefreshFileList();
      this.Beep();
      this.m_isLoadSaveConfirm = false;
      this.m_isLoadSaveDeleteConfirm = false;
      this.BTN_LoadSave_ConfirmSave.SetActive(false);
      this.BTN_LoadSave_ConfirmDelete.SetActive(false);
    }

    public void Button_LoadSave_PagePrev()
    {
      --this.m_saveload_curfilepage;
      if (this.m_saveload_curfilepage < 0)
        this.m_saveload_curfilepage = 0;
      this.LoadSave_RefreshFileList();
      this.Beep();
      this.m_isLoadSaveConfirm = false;
      this.m_isLoadSaveDeleteConfirm = false;
      this.BTN_LoadSave_ConfirmSave.SetActive(false);
      this.BTN_LoadSave_ConfirmDelete.SetActive(false);
    }

    public void Button_LoadSave_KeyEntry(string s)
    {
      this.m_isLoadSaveConfirm = false;
      this.m_isLoadSaveDeleteConfirm = false;
      this.BTN_LoadSave_ConfirmSave.SetActive(false);
      this.BTN_LoadSave_ConfirmDelete.SetActive(false);
      if (this.m_loadSaveTextEntry == GamePlannerPanel.LoadSaveTextEntry.FileName)
      {
        this.Beep();
        if (s == "Backspace")
        {
          if (this.m_filenameEntry.Length > 0)
            this.m_filenameEntry = this.m_filenameEntry.Substring(0, this.m_filenameEntry.Length - 1);
        }
        else if (!(s == "Enter"))
          this.m_filenameEntry += s;
        if (this.m_filenameEntry == string.Empty)
        {
          this.BTN_LoadSave_Save.SetActive(false);
          this.BTN_LoadSave_SaveAs.SetActive(false);
        }
        else if (this.m_LoadSave_Filelist.Contains(this.m_filenameEntry))
        {
          this.BTN_LoadSave_Save.SetActive(true);
          this.BTN_LoadSave_SaveAs.SetActive(false);
        }
        else
        {
          this.BTN_LoadSave_Save.SetActive(false);
          this.BTN_LoadSave_SaveAs.SetActive(true);
        }
      }
      else if (this.m_loadSaveTextEntry == GamePlannerPanel.LoadSaveTextEntry.Description)
      {
        this.Beep();
        if (s == "Backspace")
        {
          if (this.m_curDescriptionText.Length > 1)
          {
            if (this.m_curDescriptionText.Substring(this.m_curDescriptionText.Length - 2, this.m_curDescriptionText.Length) == "\n")
              this.m_curDescriptionText.Substring(0, this.m_curDescriptionText.Length - 2);
            else
              this.m_curDescriptionText = this.m_curDescriptionText.Substring(0, this.m_curDescriptionText.Length - 1);
          }
          else if (this.m_curDescriptionText.Length > 0)
            this.m_curDescriptionText = this.m_curDescriptionText.Substring(0, this.m_curDescriptionText.Length - 1);
        }
        else if (s == "Enter")
          this.m_curDescriptionText += "\n";
        else
          this.m_curDescriptionText += s;
      }
      else
        this.Err();
      if (this.m_filenameEntry == string.Empty)
        this.LBL_LoadSave_FileName.text = "ENTER FILENAME";
      else
        this.LBL_LoadSave_FileName.text = this.m_filenameEntry;
    }

    private void SetPageMode_Load()
    {
      this.Page_LoadSave.SetActive(true);
      this.LoadSave_Keyboard.SetActive(false);
      this.m_filenameEntry = string.Empty;
      this.LBL_LoadSave_Top.text = "Scenarios for " + GM.CurrentGamePlannerManager.levelName;
      this.m_saveload_curfilepage = 0;
      this.LoadSave_CalcMaxPage();
      this.LBL_LoadSave_ListPage.text = (this.m_saveload_curfilepage + 1).ToString() + "/" + (this.m_saveload_maxfilepage + 1).ToString();
      this.LoadSave_RefreshFileList();
      this.LBL_LoadSave_Description.text = string.Empty;
      this.BTN_LoadSave_Load.SetActive(false);
      this.BTN_LoadSave_Save.SetActive(false);
      this.BTN_LoadSave_SaveAs.SetActive(false);
      this.BTN_LoadSave_ConfirmSave.SetActive(false);
      this.BTN_LoadSave_ConfirmDelete.SetActive(false);
      this.BTN_LoadSave_EditDescription.SetActive(false);
      this.BTN_LoadSave_EditFilename.SetActive(false);
      this.BTN_LoadSave_Load.SetActive(false);
    }

    private void SetPageMode_Save()
    {
      this.Page_LoadSave.SetActive(true);
      this.LoadSave_Keyboard.SetActive(true);
      this.m_filenameEntry = string.Empty;
      this.LBL_LoadSave_FileName.text = "ENTER FILENAME";
      this.LBL_LoadSave_Top.text = "Scenarios for " + GM.CurrentGamePlannerManager.levelName;
      this.m_saveload_curfilepage = 0;
      this.LoadSave_CalcMaxPage();
      this.LoadSave_RefreshFileList();
      this.LBL_LoadSave_Description.text = string.Empty;
      this.BTN_LoadSave_Load.SetActive(false);
      this.BTN_LoadSave_Save.SetActive(false);
      this.BTN_LoadSave_SaveAs.SetActive(false);
      this.BTN_LoadSave_ConfirmSave.SetActive(false);
      this.BTN_LoadSave_ConfirmDelete.SetActive(false);
      this.BTN_LoadSave_EditDescription.SetActive(true);
      this.BTN_LoadSave_EditFilename.SetActive(true);
    }

    private void LoadSave_CalcMaxPage()
    {
      this.m_saveload_maxfilepage = Mathf.Clamp(Mathf.FloorToInt((float) this.m_LoadSave_Filelist.Count), 0, 99);
      this.LBL_LoadSave_ListPage.text = (this.m_saveload_curfilepage + 1).ToString() + "/" + (this.m_saveload_maxfilepage + 1).ToString();
    }

    private void LoadSave_RefreshFileList()
    {
      this.m_LoadSave_Filelist.Clear();
      this.m_LoadSave_Filelist = new List<string>((IEnumerable<string>) GM.CurrentGamePlannerManager.GetFileListForCurrentScene());
      int index1 = this.m_saveload_curfilepage * 12;
      for (int index2 = 0; index2 < this.LBL_LoadSave_FileNameList.Count; ++index2)
      {
        if (index1 < this.m_LoadSave_Filelist.Count)
        {
          this.LBL_LoadSave_FileNameList[index2].gameObject.SetActive(true);
          this.LBL_LoadSave_FileNameList[index2].text = this.m_LoadSave_Filelist[index1];
        }
        else
          this.LBL_LoadSave_FileNameList[index2].gameObject.SetActive(false);
        ++index1;
      }
      this.LBL_LoadSave_ListPage.text = (this.m_saveload_curfilepage + 1).ToString() + "/" + (this.m_saveload_maxfilepage + 1).ToString();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      switch (this.m_pageMode)
      {
        case GamePlannerPanel.PanelPageMode.Spawn:
          this.Spawn_Update();
          break;
        case GamePlannerPanel.PanelPageMode.Tools:
          this.Tools_Update();
          break;
      }
    }

    public enum PanelPageMode
    {
      Mode,
      Scene,
      Spawn,
      Tools,
      Sosig,
      Load,
      Save,
    }

    public enum SpawnPageMode
    {
      CategoryList,
      SubCategoryList,
    }

    public enum Tool
    {
      None,
      ManipBeam,
      ParamBeam,
      ResetBeam,
      DeleteBeam,
    }

    public enum ManipulatorMode
    {
      Nudge,
      Shunt,
      Rotate,
    }

    public enum ManipulatorAxis
    {
      World,
      Local,
    }

    public enum ManipulatorRelativeTo
    {
      Self,
      Global,
    }

    public enum LoadSaveTextEntry
    {
      None,
      FileName,
      Description,
    }
  }
}
