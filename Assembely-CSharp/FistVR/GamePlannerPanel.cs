using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class GamePlannerPanel : FVRPhysicalObject
	{
		public enum PanelPageMode
		{
			Mode,
			Scene,
			Spawn,
			Tools,
			Sosig,
			Load,
			Save
		}

		public enum SpawnPageMode
		{
			CategoryList,
			SubCategoryList
		}

		public enum Tool
		{
			None,
			ManipBeam,
			ParamBeam,
			ResetBeam,
			DeleteBeam
		}

		public enum ManipulatorMode
		{
			Nudge,
			Shunt,
			Rotate
		}

		public enum ManipulatorAxis
		{
			World,
			Local
		}

		public enum ManipulatorRelativeTo
		{
			Self,
			Global
		}

		public enum LoadSaveTextEntry
		{
			None,
			FileName,
			Description
		}

		[Header("GamePlannerPanel")]
		public Transform AimAperture;

		private float m_beamWidth = 0.005f;

		public AudioEvent AudEvent_Beep;

		public AudioEvent AudEvent_Boop;

		public AudioEvent AudEvent_Err;

		private PanelPageMode m_pageMode;

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

		public SpawnPageMode SpawnMode;

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

		private Tool m_toolState;

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

		public ManipulatorMode ManipMode;

		public ManipulatorAxis ManipAxis_Nudge;

		public ManipulatorAxis ManipAxis_Shunt;

		public ManipulatorAxis ManipAxis_Rotate;

		public ManipulatorRelativeTo ManipRelativeTo_Nudge;

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

		private LoadSaveTextEntry m_loadSaveTextEntry;

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

		public GPManipulatorButton CurrentManipButton
		{
			get
			{
				return m_currentManipButton;
			}
			set
			{
				if (m_currentManipButton == value)
				{
					if (m_currentManipButton != null)
					{
						m_currentManipButton.OnPoint();
					}
					return;
				}
				if (m_currentManipButton != null)
				{
					m_currentManipButton.EndPoint();
				}
				m_currentManipButton = value;
				if (m_currentManipButton != null)
				{
					m_currentManipButton.OnPoint();
				}
			}
		}

		protected override void Start()
		{
			base.Start();
			Button_TopBar(0);
			UpdateSceneModeState();
			PageSwitchCleanup();
		}

		private void UpdateSceneModeState()
		{
			m_scenemode = GM.CurrentGamePlannerManager.SMode;
			UpdateTopBar(m_scenemode == GPSceneMode.Play);
		}

		private void Beep()
		{
			SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Beep, base.transform.position);
		}

		private void Boop()
		{
			SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Boop, base.transform.position);
		}

		private void Err()
		{
			SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Err, base.transform.position);
		}

		private void SetPageMode(PanelPageMode mode)
		{
			PageSwitchCleanup();
			m_pageMode = mode;
			switch (m_pageMode)
			{
			case PanelPageMode.Mode:
				SetPageMode_Mode();
				break;
			case PanelPageMode.Scene:
				break;
			case PanelPageMode.Spawn:
				SetPageMode_Spawn();
				break;
			case PanelPageMode.Tools:
				SetPageMode_Tools();
				break;
			case PanelPageMode.Sosig:
				break;
			case PanelPageMode.Load:
				SetPageMode_Load();
				break;
			case PanelPageMode.Save:
				SetPageMode_Save();
				break;
			}
		}

		private void PageSwitchCleanup()
		{
			PageSwitchCleanup_Mode();
			PageSwitchCleanup_Spawn();
			PageSwitchCleanup_Tools();
			PageSwitchCleanup_LoadSave();
		}

		private void UpdateTopBar(bool isPlayMode)
		{
			if (isPlayMode)
			{
				LBL_PanelState.text = "GAMEPLANNER PANEL - PLAY MODE - " + m_pageMode;
				for (int i = 0; i < BTN_TopBarList.Count; i++)
				{
					if (i == 3 || i == 6)
					{
						BTN_TopBarList[i].SetActive(value: false);
					}
					else
					{
						BTN_TopBarList[i].SetActive(value: true);
					}
				}
				return;
			}
			LBL_PanelState.text = "GAMEPLANNER PANEL - DESIGN MODE - " + m_pageMode;
			for (int j = 0; j < BTN_TopBarList.Count; j++)
			{
				if (j == 4)
				{
					BTN_TopBarList[j].SetActive(value: false);
				}
				else
				{
					BTN_TopBarList[j].SetActive(value: true);
				}
			}
		}

		public void Button_TopBar(int i)
		{
			Beep();
			if (Page_Mode != null)
			{
				Page_Mode.SetActive(value: false);
			}
			if (Page_Scene != null)
			{
				Page_Scene.SetActive(value: false);
			}
			if (Page_Spawn != null)
			{
				Page_Spawn.SetActive(value: false);
			}
			if (Page_Tools != null)
			{
				Page_Tools.SetActive(value: false);
			}
			if (Page_Sosig != null)
			{
				Page_Sosig.SetActive(value: false);
			}
			if (Page_LoadSave != null)
			{
				Page_LoadSave.SetActive(value: false);
			}
			SetPageMode((PanelPageMode)i);
		}

		public void Button_Mode_Switch()
		{
			Beep();
			SetConfirmCancel(b: true);
		}

		public void Button_Mode_ConfirmCancel(bool isConfirm)
		{
			if (!m_mode_isConfirmCancel)
			{
				Err();
				return;
			}
			Beep();
			SetConfirmCancel(b: false);
			if (isConfirm)
			{
				GM.CurrentGamePlannerManager.ToggleSceneMode();
				UpdateSceneModeState();
			}
		}

		public void Button_Mode_Help(int p)
		{
			Beep();
			for (int i = 0; i < Mode_HelpPages.Count; i++)
			{
				if (i == p)
				{
					Mode_HelpPages[i].SetActive(value: true);
				}
				else
				{
					Mode_HelpPages[i].SetActive(value: false);
				}
			}
		}

		private void SetPageMode_Mode()
		{
			Page_Mode.SetActive(value: true);
		}

		private void PageSwitchCleanup_Mode()
		{
			SetConfirmCancel(b: false);
			Mode_UpdateDisplay();
		}

		private void Mode_UpdateDisplay()
		{
			if (GM.CurrentGamePlannerManager.SMode == GPSceneMode.Play)
			{
				LBL_Mode_CurrentMode.text = "Current Mode: Play";
				LBL_Mode_Switch.text = "Switch to Design Mode";
			}
			else
			{
				LBL_Mode_CurrentMode.text = "Current Mode: Design";
				LBL_Mode_Switch.text = "Switch to Play Mode";
			}
		}

		private void SetConfirmCancel(bool b)
		{
			m_mode_isConfirmCancel = b;
			BTN_Mode_Confirm.SetActive(b);
			BTN_Mode_Cancel.SetActive(b);
		}

		private void Spawn_Update()
		{
			if (!base.IsHeld)
			{
				return;
			}
			if (m_curSelectedID != null)
			{
				SpawnRayBeam.gameObject.SetActive(value: true);
				float z = m_spawnMaxDistance;
				if (Physics.Raycast(AimAperture.position, AimAperture.forward, out m_spawnHit, m_spawnMaxDistance, LM_SpawnPlace_Env, QueryTriggerInteraction.Ignore))
				{
					z = m_spawnHit.distance;
					m_spawnPos = m_spawnHit.point;
					m_hasSpawnPoint = true;
				}
				else
				{
					m_hasSpawnPoint = false;
				}
				SpawnRayBeam.localScale = new Vector3(m_beamWidth, m_beamWidth, z);
			}
			else
			{
				m_hasSpawnPoint = false;
				SpawnRayBeam.gameObject.SetActive(value: false);
			}
			if (m_hasSpawnPoint && m_hand.Input.TriggerDown)
			{
				SpawnCurrentItemSpawnerID(m_curSelectedID, m_spawnPos);
			}
		}

		private void SpawnCurrentItemSpawnerID(ItemSpawnerID id, Vector3 p)
		{
			GameObject gameObject = Object.Instantiate(id.MainObject.GetGameObject(), p, Quaternion.identity);
			if (m_scenemode == GPSceneMode.Design)
			{
				GPPlaceable component = gameObject.GetComponent<GPPlaceable>();
				GM.CurrentGamePlannerManager.RegisterAndInitSpawnedPlaceable(component);
			}
		}

		private void SetPageMode_Spawn()
		{
			Page_Spawn.SetActive(value: true);
		}

		public void Button_Spawn_SetLinearGrid(int i)
		{
		}

		public void Button_Spawn_SetAngularGrid(int i)
		{
		}

		public void Button_Spawn_ToggleGuides()
		{
			m_SpawnSnapGuides = !m_SpawnSnapGuides;
			Beep();
			if (m_SpawnSnapGuides)
			{
				LBL_Spawn_Guides.text = "Enabled";
			}
			else
			{
				LBL_Spawn_Guides.text = "Disabled";
			}
		}

		public void Button_Spawn_CatSubCat(int i)
		{
			Beep();
			if (SpawnMode == SpawnPageMode.CategoryList)
			{
				m_curCat = m_curDefFile.Categories[i].Cat;
				m_curCatIndex = i;
				m_curSubCat = m_curDefFile.Categories[i].Subcats[0].Subcat;
				m_curSubCatIndex = 0;
				m_curSelectedID = null;
				OBS_Spawn_Item.SetSelectedButton(0);
				SpawnMode = SpawnPageMode.SubCategoryList;
			}
			else if (SpawnMode == SpawnPageMode.SubCategoryList)
			{
				if (i == 9)
				{
					SpawnMode = SpawnPageMode.CategoryList;
					m_curSelectedID = null;
					m_curSubCat = ItemSpawnerID.ESubCategory.None;
					m_curSubCatIndex = 0;
					Spawn_UpdateDisplay();
					return;
				}
				m_curSubCat = m_curDefFile.Categories[m_curCatIndex].Subcats[i].Subcat;
				m_curSubCatIndex = i;
				OBS_Spawn_Item.SetSelectedButton(0);
				Debug.Log("fired");
				if (IM.SCD[m_curSubCat].Count > 0)
				{
					m_curSelectedID = IM.SCD[m_curSubCat][0];
				}
				else
				{
					m_curSelectedID = null;
				}
			}
			Spawn_UpdateDisplay();
		}

		public void Button_Spawn_Item(int i)
		{
			Boop();
			Spawn_UpdateDisplay();
		}

		private void PageSwitchCleanup_Spawn()
		{
			if (m_scenemode == GPSceneMode.Play)
			{
				m_curDefFile = Spawn_DefFile_Play;
			}
			else
			{
				m_curDefFile = Spawn_DefFile_Design;
			}
			SpawnMode = SpawnPageMode.CategoryList;
			m_curCat = m_curDefFile.Categories[0].Cat;
			m_curSubCat = m_curDefFile.Categories[0].Subcats[0].Subcat;
			m_curCatIndex = 0;
			m_curSubCatIndex = 0;
			m_curSelectedID = null;
			OBS_Spawn_Item.SetSelectedButton(0);
			SpawnRayBeam.gameObject.SetActive(value: false);
			Spawn_UpdateDisplay();
		}

		private void Spawn_UpdateDisplay()
		{
			if (SpawnMode == SpawnPageMode.CategoryList)
			{
				for (int i = 0; i < GO_Spawn_CatSubCats.Count; i++)
				{
					if (i < m_curDefFile.Categories.Length)
					{
						GO_Spawn_CatSubCats[i].SetActive(value: true);
						LBL_Spawn_CatSubCats[i].text = m_curDefFile.Categories[i].DisplayName;
					}
					else
					{
						GO_Spawn_CatSubCats[i].SetActive(value: false);
					}
				}
				LBL_Spawn_PickCategory.text = "Pick Category:";
				LBL_Spawn_CatSubCatName.text = string.Empty;
				for (int j = 0; j < GO_Spawn_Items.Count; j++)
				{
					GO_Spawn_Items[j].SetActive(value: false);
				}
				return;
			}
			for (int k = 0; k < GO_Spawn_CatSubCats.Count - 1; k++)
			{
				if (k < m_curDefFile.Categories[m_curCatIndex].Subcats.Length)
				{
					GO_Spawn_CatSubCats[k].SetActive(value: true);
					LBL_Spawn_CatSubCats[k].text = m_curDefFile.Categories[m_curCatIndex].Subcats[k].DisplayName;
				}
				else
				{
					GO_Spawn_CatSubCats[k].SetActive(value: false);
				}
			}
			GO_Spawn_CatSubCats[9].SetActive(value: true);
			LBL_Spawn_CatSubCats[9].text = "<< Back <<";
			LBL_Spawn_PickCategory.text = m_curDefFile.Categories[m_curCatIndex].DisplayName + ";";
			LBL_Spawn_CatSubCatName.text = m_curDefFile.Categories[m_curCatIndex].Subcats[m_curSubCatIndex].DisplayName;
			for (int l = 0; l < GO_Spawn_Items.Count; l++)
			{
				if (l < IM.SCD[m_curSubCat].Count)
				{
					GO_Spawn_Items[l].SetActive(value: true);
					LBL_Spawn_Items[l].text = IM.SCD[m_curSubCat][l].DisplayName;
				}
				else
				{
					GO_Spawn_Items[l].SetActive(value: false);
				}
			}
		}

		public void Button_SetManipMode(int i)
		{
			ManipMode = (ManipulatorMode)i;
			m_tools_Manipulator.UpdateManipulatorFrame(ManipMode);
		}

		public void Button_Manip_ResetRotation()
		{
			m_tools_Manipulator.ResetRotation();
		}

		public float GetManipNudgeInterval()
		{
			return m_manipNudgeGridIntervals[ManipNudgeGrid_Index];
		}

		public float GetManipShuntInterval()
		{
			return m_manipShuntMaxIntervals[ManipShuntMax_Index];
		}

		public float GetManipRotateInterval()
		{
			return m_manipRotateSnapIntervals[ManipRotateSnap_Index];
		}

		public void Button_Manip_SetNudgeGridInterval(int i)
		{
			ManipNudgeGrid_Index = i;
		}

		public void Button_Manip_SetShuntMaxInterval(int i)
		{
			ManipShuntMax_Index = i;
		}

		public void Button_Manip_SetRotateSnapInterval(int i)
		{
			ManipRotateSnap_Index = i;
		}

		public void Button_Manip_SetAxis_Nudge(int i)
		{
			ManipAxis_Nudge = (ManipulatorAxis)i;
			m_tools_Manipulator.UpdateManipulatorFrame(ManipMode);
		}

		public void Button_Manip_SetAxis_Shunt(int i)
		{
			ManipAxis_Shunt = (ManipulatorAxis)i;
			m_tools_Manipulator.UpdateManipulatorFrame(ManipMode);
		}

		public void Button_Manip_SetAxis_Rotate(int i)
		{
			ManipAxis_Rotate = (ManipulatorAxis)i;
			m_tools_Manipulator.UpdateManipulatorFrame(ManipMode);
		}

		public void Button_Manip_SetRelativeTo_Nudge(int i)
		{
			ManipRelativeTo_Nudge = (ManipulatorRelativeTo)i;
			m_tools_Manipulator.UpdateManipulatorFrame(ManipMode);
		}

		private void SetPageMode_Tools()
		{
			Page_Tools.SetActive(value: true);
		}

		private void PageSwitchCleanup_Tools()
		{
			m_toolState = Tool.None;
			if (m_tools_Manipulator == null)
			{
				GameObject gameObject = Object.Instantiate(ManipulatorPrefab, Vector3.zero, Quaternion.identity);
				m_tools_Manipulator = gameObject.GetComponent<GamePlannerManipulator>();
				m_tools_Manipulator.Panel = this;
				gameObject.SetActive(value: false);
			}
		}

		private void Tools_Cleanup()
		{
			if (m_tools_Manipulator != null)
			{
				m_tools_Manipulator.SetControlledObject(null);
				m_tools_Manipulator.gameObject.SetActive(value: false);
			}
		}

		public void Button_Tool_SelectTool(int i)
		{
			Beep();
			Tools_Cleanup();
			m_toolState = (Tool)i;
			switch (m_toolState)
			{
			case Tool.None:
				SpawnRayBeam.gameObject.SetActive(value: false);
				ToolsPage_ManipBeam.SetActive(value: false);
				break;
			case Tool.ManipBeam:
				SpawnRayBeam.gameObject.SetActive(value: true);
				ToolsPage_ManipBeam.SetActive(value: true);
				break;
			case Tool.ParamBeam:
				SpawnRayBeam.gameObject.SetActive(value: true);
				ToolsPage_ManipBeam.SetActive(value: false);
				break;
			case Tool.ResetBeam:
				SpawnRayBeam.gameObject.SetActive(value: true);
				ToolsPage_ManipBeam.SetActive(value: false);
				break;
			case Tool.DeleteBeam:
				SpawnRayBeam.gameObject.SetActive(value: true);
				ToolsPage_ManipBeam.SetActive(value: false);
				break;
			}
		}

		private void Tools_Update()
		{
			switch (m_toolState)
			{
			case Tool.None:
				break;
			case Tool.ManipBeam:
				Tool_Update_ManipBeam();
				break;
			case Tool.ParamBeam:
				Tool_Update_ParamBeam();
				break;
			case Tool.ResetBeam:
				Tool_Update_ResetBeam();
				break;
			case Tool.DeleteBeam:
				Tool_Update_DeleteBeam();
				break;
			}
		}

		private void Tool_Update_ManipBeam()
		{
			if (!base.IsHeld)
			{
				return;
			}
			float z = m_maxManipBeamRange;
			bool flag = false;
			bool flag2 = false;
			if (Physics.Raycast(AimAperture.position, AimAperture.forward, out m_manipHit, m_maxManipBeamRange, LM_Tools_ManipulatorUI, QueryTriggerInteraction.Collide) && m_manipHit.collider.transform.GetComponent<GPManipulatorButton>() != null)
			{
				z = m_manipHit.distance;
				flag = true;
				flag2 = true;
				CurrentManipButton = m_manipHit.collider.transform.GetComponent<GPManipulatorButton>();
			}
			else if (Physics.Raycast(AimAperture.position, AimAperture.forward, out m_manipHit, m_maxManipBeamRange, LM_Tools_ManupulatorObjects, QueryTriggerInteraction.Ignore))
			{
				CurrentManipButton = null;
				z = m_manipHit.distance;
				flag = true;
			}
			else
			{
				CurrentManipButton = null;
			}
			if (flag && m_hand.Input.TriggerDown)
			{
				if (flag2)
				{
					CurrentManipButton.Button.onClick.Invoke();
					Beep();
				}
				else
				{
					GPPlaceable component = m_manipHit.transform.root.gameObject.GetComponent<GPPlaceable>();
					if (component != null)
					{
						m_tools_Manipulator.gameObject.SetActive(value: true);
						m_tools_Manipulator.SetControlledObject(m_manipHit.transform.root);
						m_tools_Manipulator.transform.position = m_manipHit.transform.position;
						Boop();
					}
				}
			}
			SpawnRayBeam.localScale = new Vector3(m_beamWidth, m_beamWidth, z);
		}

		private void Tool_Update_ParamBeam()
		{
			if (base.IsHeld)
			{
				float z = m_maxManipBeamRange;
				if (Physics.Raycast(AimAperture.position, AimAperture.forward, out m_manipHit, m_maxManipBeamRange, LM_Tools_ManupulatorObjects, QueryTriggerInteraction.Ignore))
				{
					z = m_manipHit.distance;
					GPPlaceable component = m_manipHit.collider.transform.root.gameObject.GetComponent<GPPlaceable>();
				}
				SpawnRayBeam.localScale = new Vector3(m_beamWidth, m_beamWidth, z);
			}
		}

		private void Tool_Update_ResetBeam()
		{
			if (base.IsHeld)
			{
				float z = m_maxManipBeamRange;
				if (Physics.Raycast(AimAperture.position, AimAperture.forward, out m_manipHit, m_maxManipBeamRange, LM_Tools_ManupulatorObjects, QueryTriggerInteraction.Ignore))
				{
					z = m_manipHit.distance;
					GPPlaceable component = m_manipHit.collider.transform.root.gameObject.GetComponent<GPPlaceable>();
					GM.CurrentGamePlannerManager.ResetPlaceable(component);
				}
				SpawnRayBeam.localScale = new Vector3(m_beamWidth, m_beamWidth, z);
			}
		}

		private void Tool_Update_DeleteBeam()
		{
			if (base.IsHeld)
			{
				float z = m_maxManipBeamRange;
				if (Physics.Raycast(AimAperture.position, AimAperture.forward, out m_manipHit, m_maxManipBeamRange, LM_Tools_ManupulatorObjects, QueryTriggerInteraction.Ignore))
				{
					z = m_manipHit.distance;
					GPPlaceable component = m_manipHit.collider.transform.root.gameObject.GetComponent<GPPlaceable>();
					GM.CurrentGamePlannerManager.DeregisterAndDeletePlaceable(component);
				}
				SpawnRayBeam.localScale = new Vector3(m_beamWidth, m_beamWidth, z);
			}
		}

		private void PageSwitchCleanup_LoadSave()
		{
			m_saveload_curfilepage = 0;
			m_saveload_maxfilepage = 0;
			m_filenameEntry = string.Empty;
			m_curDescriptionText = string.Empty;
			m_isLoadSaveConfirm = false;
			m_isLoadSaveDeleteConfirm = false;
			m_LoadSave_Filelist.Clear();
			m_selectedFileToLoadIndex = -1;
			m_selectedFileToSaveIndex = -1;
		}

		public void Button_LoadSave_FileSelect(int f)
		{
			if (m_pageMode == PanelPageMode.Load)
			{
				m_selectedFileToLoadIndex = f + 12 * m_saveload_curfilepage;
				string text = m_LoadSave_Filelist[m_selectedFileToLoadIndex];
				text += "\n";
				text += GM.CurrentGamePlannerManager.GetDescription(m_LoadSave_Filelist[m_selectedFileToLoadIndex]);
				LBL_LoadSave_Description.text = text;
				BTN_LoadSave_EditDescription.SetActive(value: false);
				BTN_LoadSave_EditFilename.SetActive(value: false);
				BTN_LoadSave_Save.SetActive(value: false);
				BTN_LoadSave_SaveAs.SetActive(value: false);
				BTN_LoadSave_ConfirmSave.SetActive(value: false);
				BTN_LoadSave_ConfirmDelete.SetActive(value: false);
				BTN_LoadSave_Load.SetActive(value: true);
			}
			else if (m_pageMode == PanelPageMode.Save)
			{
				m_selectedFileToSaveIndex = f + 12 * m_saveload_curfilepage;
				m_filenameEntry = m_LoadSave_Filelist[m_selectedFileToSaveIndex];
				LBL_LoadSave_FileName.text = m_filenameEntry;
				string text2 = m_LoadSave_Filelist[m_selectedFileToLoadIndex];
				text2 += "\n";
				text2 += GM.CurrentGamePlannerManager.GetDescription(m_LoadSave_Filelist[m_selectedFileToLoadIndex]);
				LBL_LoadSave_Description.text = text2;
				m_loadSaveTextEntry = LoadSaveTextEntry.None;
				BTN_LoadSave_EditDescription.SetActive(value: false);
				BTN_LoadSave_EditFilename.SetActive(value: false);
				BTN_LoadSave_ConfirmSave.SetActive(value: false);
				BTN_LoadSave_ConfirmDelete.SetActive(value: false);
				BTN_LoadSave_Load.SetActive(value: false);
				BTN_LoadSave_Save.SetActive(value: true);
				BTN_LoadSave_SaveAs.SetActive(value: false);
			}
			Beep();
			m_isLoadSaveConfirm = false;
			m_isLoadSaveDeleteConfirm = false;
			BTN_LoadSave_ConfirmSave.SetActive(value: false);
			BTN_LoadSave_ConfirmDelete.SetActive(value: false);
		}

		public void Button_LoadSave_EditDescription()
		{
			m_isLoadSaveConfirm = false;
			m_isLoadSaveDeleteConfirm = false;
			BTN_LoadSave_ConfirmSave.SetActive(value: false);
			BTN_LoadSave_ConfirmDelete.SetActive(value: false);
			m_loadSaveTextEntry = LoadSaveTextEntry.Description;
			BTN_LoadSave_EditDescription.SetActive(value: false);
			BTN_LoadSave_EditFilename.SetActive(value: true);
			Beep();
		}

		public void Button_LoadSave_EditFilename()
		{
			m_isLoadSaveConfirm = false;
			m_isLoadSaveDeleteConfirm = false;
			BTN_LoadSave_ConfirmSave.SetActive(value: false);
			BTN_LoadSave_ConfirmDelete.SetActive(value: false);
			m_loadSaveTextEntry = LoadSaveTextEntry.FileName;
			BTN_LoadSave_EditFilename.SetActive(value: false);
			BTN_LoadSave_EditDescription.SetActive(value: true);
			Beep();
		}

		public void Button_LoadSave_LoadScenario()
		{
			m_isLoadSaveConfirm = false;
			m_isLoadSaveDeleteConfirm = false;
			BTN_LoadSave_ConfirmSave.SetActive(value: false);
			BTN_LoadSave_ConfirmDelete.SetActive(value: false);
			if (m_selectedFileToLoadIndex < 0)
			{
				Err();
			}
			else if (GM.CurrentGamePlannerManager.LoadScenario(m_LoadSave_Filelist[m_selectedFileToLoadIndex]))
			{
				Boop();
			}
			else
			{
				Err();
			}
		}

		public void Button_LoadSave_Save()
		{
			m_isLoadSaveConfirm = true;
			m_isLoadSaveDeleteConfirm = false;
			BTN_LoadSave_ConfirmSave.SetActive(value: true);
			BTN_LoadSave_ConfirmDelete.SetActive(value: false);
			Beep();
			LoadSave_RefreshFileList();
		}

		public void Button_LoadSave_SaveAs()
		{
			m_isLoadSaveConfirm = true;
			m_isLoadSaveDeleteConfirm = false;
			BTN_LoadSave_ConfirmSave.SetActive(value: true);
			BTN_LoadSave_ConfirmDelete.SetActive(value: false);
			Beep();
			LoadSave_RefreshFileList();
		}

		public void Button_LoadSave_ConfirmSave()
		{
			m_isLoadSaveConfirm = false;
			m_isLoadSaveDeleteConfirm = false;
			BTN_LoadSave_ConfirmSave.SetActive(value: false);
			BTN_LoadSave_ConfirmDelete.SetActive(value: false);
			GM.CurrentGamePlannerManager.SaveActiveScenario(m_filenameEntry, m_curDescriptionText);
			m_saveload_curfilepage = 0;
			LoadSave_RefreshFileList();
			LoadSave_CalcMaxPage();
			Boop();
		}

		public void Button_LoadSave_Delete()
		{
			m_isLoadSaveConfirm = false;
			m_isLoadSaveDeleteConfirm = true;
			BTN_LoadSave_ConfirmSave.SetActive(value: false);
			BTN_LoadSave_ConfirmDelete.SetActive(value: true);
			Beep();
			LoadSave_RefreshFileList();
		}

		public void Button_LoadSave_ConfirmDelete()
		{
			if (GM.CurrentGamePlannerManager.DeleteScenario(m_filenameEntry))
			{
				m_isLoadSaveConfirm = false;
				m_isLoadSaveDeleteConfirm = false;
				BTN_LoadSave_ConfirmSave.SetActive(value: false);
				BTN_LoadSave_ConfirmDelete.SetActive(value: false);
				m_saveload_curfilepage = 0;
				LoadSave_RefreshFileList();
				LoadSave_CalcMaxPage();
				PageSwitchCleanup_LoadSave();
				Boop();
			}
			else
			{
				Err();
			}
		}

		public void Button_LoadSave_PageNext()
		{
			m_saveload_curfilepage++;
			if (m_saveload_curfilepage > m_saveload_maxfilepage)
			{
				m_saveload_curfilepage = m_saveload_maxfilepage;
			}
			LoadSave_RefreshFileList();
			Beep();
			m_isLoadSaveConfirm = false;
			m_isLoadSaveDeleteConfirm = false;
			BTN_LoadSave_ConfirmSave.SetActive(value: false);
			BTN_LoadSave_ConfirmDelete.SetActive(value: false);
		}

		public void Button_LoadSave_PagePrev()
		{
			m_saveload_curfilepage--;
			if (m_saveload_curfilepage < 0)
			{
				m_saveload_curfilepage = 0;
			}
			LoadSave_RefreshFileList();
			Beep();
			m_isLoadSaveConfirm = false;
			m_isLoadSaveDeleteConfirm = false;
			BTN_LoadSave_ConfirmSave.SetActive(value: false);
			BTN_LoadSave_ConfirmDelete.SetActive(value: false);
		}

		public void Button_LoadSave_KeyEntry(string s)
		{
			m_isLoadSaveConfirm = false;
			m_isLoadSaveDeleteConfirm = false;
			BTN_LoadSave_ConfirmSave.SetActive(value: false);
			BTN_LoadSave_ConfirmDelete.SetActive(value: false);
			if (m_loadSaveTextEntry == LoadSaveTextEntry.FileName)
			{
				Beep();
				if (s == "Backspace")
				{
					if (m_filenameEntry.Length > 0)
					{
						m_filenameEntry = m_filenameEntry.Substring(0, m_filenameEntry.Length - 1);
					}
				}
				else if (!(s == "Enter"))
				{
					m_filenameEntry += s;
				}
				if (m_filenameEntry == string.Empty)
				{
					BTN_LoadSave_Save.SetActive(value: false);
					BTN_LoadSave_SaveAs.SetActive(value: false);
				}
				else if (m_LoadSave_Filelist.Contains(m_filenameEntry))
				{
					BTN_LoadSave_Save.SetActive(value: true);
					BTN_LoadSave_SaveAs.SetActive(value: false);
				}
				else
				{
					BTN_LoadSave_Save.SetActive(value: false);
					BTN_LoadSave_SaveAs.SetActive(value: true);
				}
			}
			else if (m_loadSaveTextEntry == LoadSaveTextEntry.Description)
			{
				Beep();
				if (s == "Backspace")
				{
					if (m_curDescriptionText.Length > 1)
					{
						if (m_curDescriptionText.Substring(m_curDescriptionText.Length - 2, m_curDescriptionText.Length) == "\n")
						{
							m_curDescriptionText.Substring(0, m_curDescriptionText.Length - 2);
						}
						else
						{
							m_curDescriptionText = m_curDescriptionText.Substring(0, m_curDescriptionText.Length - 1);
						}
					}
					else if (m_curDescriptionText.Length > 0)
					{
						m_curDescriptionText = m_curDescriptionText.Substring(0, m_curDescriptionText.Length - 1);
					}
				}
				else if (s == "Enter")
				{
					m_curDescriptionText += "\n";
				}
				else
				{
					m_curDescriptionText += s;
				}
			}
			else
			{
				Err();
			}
			if (m_filenameEntry == string.Empty)
			{
				LBL_LoadSave_FileName.text = "ENTER FILENAME";
			}
			else
			{
				LBL_LoadSave_FileName.text = m_filenameEntry;
			}
		}

		private void SetPageMode_Load()
		{
			Page_LoadSave.SetActive(value: true);
			LoadSave_Keyboard.SetActive(value: false);
			m_filenameEntry = string.Empty;
			LBL_LoadSave_Top.text = "Scenarios for " + GM.CurrentGamePlannerManager.levelName;
			m_saveload_curfilepage = 0;
			LoadSave_CalcMaxPage();
			LBL_LoadSave_ListPage.text = m_saveload_curfilepage + 1 + "/" + (m_saveload_maxfilepage + 1);
			LoadSave_RefreshFileList();
			LBL_LoadSave_Description.text = string.Empty;
			BTN_LoadSave_Load.SetActive(value: false);
			BTN_LoadSave_Save.SetActive(value: false);
			BTN_LoadSave_SaveAs.SetActive(value: false);
			BTN_LoadSave_ConfirmSave.SetActive(value: false);
			BTN_LoadSave_ConfirmDelete.SetActive(value: false);
			BTN_LoadSave_EditDescription.SetActive(value: false);
			BTN_LoadSave_EditFilename.SetActive(value: false);
			BTN_LoadSave_Load.SetActive(value: false);
		}

		private void SetPageMode_Save()
		{
			Page_LoadSave.SetActive(value: true);
			LoadSave_Keyboard.SetActive(value: true);
			m_filenameEntry = string.Empty;
			LBL_LoadSave_FileName.text = "ENTER FILENAME";
			LBL_LoadSave_Top.text = "Scenarios for " + GM.CurrentGamePlannerManager.levelName;
			m_saveload_curfilepage = 0;
			LoadSave_CalcMaxPage();
			LoadSave_RefreshFileList();
			LBL_LoadSave_Description.text = string.Empty;
			BTN_LoadSave_Load.SetActive(value: false);
			BTN_LoadSave_Save.SetActive(value: false);
			BTN_LoadSave_SaveAs.SetActive(value: false);
			BTN_LoadSave_ConfirmSave.SetActive(value: false);
			BTN_LoadSave_ConfirmDelete.SetActive(value: false);
			BTN_LoadSave_EditDescription.SetActive(value: true);
			BTN_LoadSave_EditFilename.SetActive(value: true);
		}

		private void LoadSave_CalcMaxPage()
		{
			m_saveload_maxfilepage = Mathf.Clamp(Mathf.FloorToInt(m_LoadSave_Filelist.Count), 0, 99);
			LBL_LoadSave_ListPage.text = m_saveload_curfilepage + 1 + "/" + (m_saveload_maxfilepage + 1);
		}

		private void LoadSave_RefreshFileList()
		{
			m_LoadSave_Filelist.Clear();
			m_LoadSave_Filelist = new List<string>(GM.CurrentGamePlannerManager.GetFileListForCurrentScene());
			int num = m_saveload_curfilepage * 12;
			for (int i = 0; i < LBL_LoadSave_FileNameList.Count; i++)
			{
				if (num < m_LoadSave_Filelist.Count)
				{
					LBL_LoadSave_FileNameList[i].gameObject.SetActive(value: true);
					LBL_LoadSave_FileNameList[i].text = m_LoadSave_Filelist[num];
				}
				else
				{
					LBL_LoadSave_FileNameList[i].gameObject.SetActive(value: false);
				}
				num++;
			}
			LBL_LoadSave_ListPage.text = m_saveload_curfilepage + 1 + "/" + (m_saveload_maxfilepage + 1);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			switch (m_pageMode)
			{
			case PanelPageMode.Mode:
				break;
			case PanelPageMode.Scene:
				break;
			case PanelPageMode.Spawn:
				Spawn_Update();
				break;
			case PanelPageMode.Tools:
				Tools_Update();
				break;
			case PanelPageMode.Sosig:
				break;
			case PanelPageMode.Load:
				break;
			case PanelPageMode.Save:
				break;
			}
		}
	}
}
