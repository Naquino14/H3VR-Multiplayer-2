using UnityEngine;

namespace FistVR
{
	public class MF_GameManager : MonoBehaviour
	{
		public FVRFMODController FMODController;

		public MF_TeamManager TeamManager;

		private AnvilPrewarmCallback LoadingCallback;

		[Header("UI - Panels")]
		public GameObject Panel_Mode_TDM;

		public GameObject Panel_RespawnReset;

		[Header("UI - PlayerSettings")]
		public OptionsPanel_ButtonSet OP_PlayerSettings_Health;

		public OptionsPanel_ButtonSet OP_PlayerSettings_BlastJumping;

		public OptionsPanel_ButtonSet OP_PlayerSettings_BlastJumpSelfDamage;

		[Header("UI - TeamDM")]
		public OptionsPanel_ButtonSet OP_TDM_PlayerTeam;

		public OptionsPanel_ButtonSet OP_TDM_RedTeamSize;

		public OptionsPanel_ButtonSet OP_TDM_BlueTeamSize;

		public OptionsPanel_ButtonSet OP_TDM_RedSpawnSpeed;

		public OptionsPanel_ButtonSet OP_TDM_BlueSpawnSpeed;

		public OptionsPanel_ButtonSet OP_TDM_PlayArea;

		private float curVol;

		private float tarVol = 0.25f;

		private void Awake()
		{
			FMODController.SetMasterVolume(0f);
		}

		private void Start()
		{
			LoadingCallback = AnvilManager.PreloadBundleAsync("assets_resources_objectids_weaponry_mf2");
			LoadingCallback.AddCallback(delegate
			{
				Debug.Log("Prewarm Completed MF");
			});
			SetPlayerIFF(1);
			InitSound();
			InitPage_PlayerSettings();
			InitPage_TDM();
			InitPlayerSettings();
		}

		private void InitSound()
		{
			FMODController.SwitchTo(0, 2f, shouldStop: false, shouldDeadStop: false);
		}

		private void Update()
		{
			if (curVol < tarVol)
			{
				curVol = Mathf.MoveTowards(curVol, tarVol, Time.deltaTime * 0.25f);
				FMODController.SetMasterVolume(curVol);
			}
		}

		public void OnPlayerDeath()
		{
			FMODController.SetIntParameterByIndex(0, "Intensity", 1f);
		}

		private void SetPlayerHealthToIndex(int index)
		{
			GM.CurrentPlayerBody.SetHealthThreshold(GM.MFFlags.PlayerSetting_HealthSettings[index]);
		}

		private void SetPlayerIFF(int i)
		{
			GM.CurrentSceneSettings.DefaultPlayerIFF = i;
			GM.CurrentPlayerBody.SetPlayerIFF(i);
		}

		public void Respawn()
		{
			Transform targetPointByClass = TeamManager.PlayerRespawnZone.GetTargetPointByClass(MF_Class.Soldier);
			GM.CurrentMovementManager.TeleportToPoint(targetPointByClass.position, isAbsolute: true, targetPointByClass.forward);
			FMODController.SetIntParameterByIndex(0, "Intensity", 2f);
		}

		public void ResetGame()
		{
		}

		private void InitPage_PlayerSettings()
		{
			OP_PlayerSettings_Health.SetSelectedButton(GM.MFFlags.PlayerSetting_HealthIndex);
			OP_PlayerSettings_BlastJumping.SetSelectedButton((int)GM.MFFlags.PlayerSetting_BlastJumping);
			OP_PlayerSettings_BlastJumpSelfDamage.SetSelectedButton((int)GM.MFFlags.PlayerSetting_BlastJumpingSelfDamage);
			SetPlayerHealthToIndex(GM.MFFlags.PlayerSetting_HealthIndex);
		}

		private void InitPage_TDM()
		{
			OP_TDM_PlayerTeam.SetSelectedButton((int)GM.MFFlags.TDMOption_PlayerTeam);
			OP_TDM_RedTeamSize.SetSelectedButton(GM.MFFlags.TDMOption_RedTeamSizeIndex);
			OP_TDM_BlueTeamSize.SetSelectedButton(GM.MFFlags.TDMOption_BlueTeamSizeIndex);
			OP_TDM_RedSpawnSpeed.SetSelectedButton((int)GM.MFFlags.TDMOption_RedTeamSpeed);
			OP_TDM_BlueSpawnSpeed.SetSelectedButton((int)GM.MFFlags.TDMOption_BlueTeamSpeed);
			OP_TDM_PlayArea.SetSelectedButton((int)GM.MFFlags.TDMOption_PlayArea);
		}

		private void InitPlayerSettings()
		{
			GM.MFFlags.PlayerTeam = GM.MFFlags.TDMOption_PlayerTeam;
			if (GM.MFFlags.TDMOption_PlayerTeam == MF_TeamColor.Red)
			{
				SetPlayerIFF(TeamManager.IFF_Red);
			}
			else
			{
				SetPlayerIFF(TeamManager.IFF_Blue);
			}
		}

		public void SetMusicOnOff(int i)
		{
			if (i == 0)
			{
				FMODController.SwitchTo(0, 0f, shouldStop: true, shouldDeadStop: false);
				FMODController.SetIntParameterByIndex(0, "Intensity", 1f);
				FMODController.SetMasterVolume(0.25f);
			}
			else
			{
				FMODController.SetIntParameterByIndex(0, "Intensity", 0f);
			}
		}

		public void SetOption_Player_Health(int i)
		{
			GM.MFFlags.PlayerSetting_HealthIndex = i;
			SetPlayerHealthToIndex(GM.MFFlags.PlayerSetting_HealthIndex);
			GM.MFFlags.SaveToFile();
		}

		public void SetOption_Player_BlastJumping(int i)
		{
			GM.MFFlags.PlayerSetting_BlastJumping = (MF_BlastJumping)i;
			GM.MFFlags.SaveToFile();
		}

		public void SetOption_Player_BlastJumpingSelfDamage(int i)
		{
			GM.MFFlags.PlayerSetting_BlastJumpingSelfDamage = (MF_BlastJumpingSelfDamage)i;
			GM.MFFlags.SaveToFile();
		}

		public void SetOption_TDM_PlayerTeam(int i)
		{
			GM.MFFlags.TDMOption_PlayerTeam = (MF_TeamColor)i;
			GM.MFFlags.SaveToFile();
			if (GM.MFFlags.TDMOption_PlayerTeam == MF_TeamColor.Red)
			{
				SetPlayerIFF(TeamManager.IFF_Red);
			}
			else
			{
				SetPlayerIFF(TeamManager.IFF_Blue);
			}
		}

		public void SetOption_TDM_RedTeamSize(int i)
		{
			GM.MFFlags.TDMOption_RedTeamSizeIndex = i;
			GM.MFFlags.SaveToFile();
		}

		public void SetOption_TDM_BlueTeamSize(int i)
		{
			GM.MFFlags.TDMOption_BlueTeamSizeIndex = i;
			GM.MFFlags.SaveToFile();
		}

		public void SetOption_TDM_RedSpawnSpeed(int i)
		{
			GM.MFFlags.TDMOption_RedTeamSpeed = (MF_SpawnSpeed)i;
			GM.MFFlags.SaveToFile();
		}

		public void SetOption_TDM_BlueSpawnSpeed(int i)
		{
			GM.MFFlags.TDMOption_BlueTeamSpeed = (MF_SpawnSpeed)i;
			GM.MFFlags.SaveToFile();
		}

		public void SetOption_TDM_PlayArea(int i)
		{
			GM.MFFlags.TDMOption_PlayArea = (MF_PlayArea)i;
			GM.MFFlags.SaveToFile();
		}

		public void StartGame()
		{
			if (LoadingCallback == null || LoadingCallback.IsCompleted)
			{
				float redTeamSpawnCadence = 6f;
				float blueTeamSpawnCadence = 6f;
				MF_SpawnSpeed tDMOption_RedTeamSpeed = GM.MFFlags.TDMOption_RedTeamSpeed;
				MF_SpawnSpeed tDMOption_BlueTeamSpeed = GM.MFFlags.TDMOption_BlueTeamSpeed;
				switch (tDMOption_RedTeamSpeed)
				{
				case MF_SpawnSpeed.Slow:
					redTeamSpawnCadence = 9f;
					break;
				case MF_SpawnSpeed.Fast:
					redTeamSpawnCadence = 3f;
					break;
				}
				switch (tDMOption_BlueTeamSpeed)
				{
				case MF_SpawnSpeed.Slow:
					blueTeamSpawnCadence = 9f;
					break;
				case MF_SpawnSpeed.Fast:
					blueTeamSpawnCadence = 3f;
					break;
				}
				Panel_Mode_TDM.SetActive(value: false);
				Panel_RespawnReset.SetActive(value: true);
				TeamManager.InitGameMode(MF_GameMode.TeamDM, GM.MFFlags.TDM_TeamSizes[GM.MFFlags.TDMOption_RedTeamSizeIndex], GM.MFFlags.TDM_TeamSizes[GM.MFFlags.TDMOption_BlueTeamSizeIndex], GM.MFFlags.TDMOption_PlayerTeam, redTeamSpawnCadence, blueTeamSpawnCadence, GM.MFFlags.TDMOption_PlayArea);
				Respawn();
			}
		}

		private void TestStart()
		{
			float redTeamSpawnCadence = 6f;
			float blueTeamSpawnCadence = 6f;
			MF_SpawnSpeed tDMOption_RedTeamSpeed = GM.MFFlags.TDMOption_RedTeamSpeed;
			MF_SpawnSpeed tDMOption_BlueTeamSpeed = GM.MFFlags.TDMOption_BlueTeamSpeed;
			switch (tDMOption_RedTeamSpeed)
			{
			case MF_SpawnSpeed.Slow:
				redTeamSpawnCadence = 9f;
				break;
			case MF_SpawnSpeed.Fast:
				redTeamSpawnCadence = 3f;
				break;
			}
			switch (tDMOption_BlueTeamSpeed)
			{
			case MF_SpawnSpeed.Slow:
				blueTeamSpawnCadence = 9f;
				break;
			case MF_SpawnSpeed.Fast:
				blueTeamSpawnCadence = 3f;
				break;
			}
			Panel_Mode_TDM.SetActive(value: false);
			Panel_RespawnReset.SetActive(value: true);
			TeamManager.InitGameMode(MF_GameMode.TeamDM, 16, 16, MF_TeamColor.Red, redTeamSpawnCadence, blueTeamSpawnCadence, MF_PlayArea.FullMap);
			Respawn();
		}
	}
}
