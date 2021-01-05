using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class GPSosigSpawner : GPPlaceable
	{
		[Header("SosigSpawner")]
		public Transform SpawnPoint;

		public OptionsPanel_ButtonSet OBS_SpawnWhen;

		public OptionsPanel_ButtonSet OBS_InitialState;

		public OptionsPanel_ButtonSet OBS_Equipment;

		public OptionsPanel_ButtonSet OBS_Ammo;

		private GPSosigManager.SpawnWhen m_spawnWhen;

		private GPSosigManager.InitialState m_initialState;

		private GPSosigManager.Equipment m_equipment;

		private GPSosigManager.Ammo m_ammo;

		private int m_teamIFF;

		private int m_eventChannel;

		private int m_assaultGroup;

		private int m_patrolPath;

		public Text LBL_TeamIFF;

		public Text LBL_SpawnChannel;

		public Text LBL_AssaultGroup;

		public Text LBL_PatrolPath;

		private List<SosigEnemyCategory> m_categories = new List<SosigEnemyCategory>();

		private List<SosigEnemyID> m_curDisplayedIDsToAdd = new List<SosigEnemyID>();

		private List<SosigEnemyTemplate> m_curDisplayedTemplatesToAdd = new List<SosigEnemyTemplate>();

		private List<SosigEnemyTemplate> m_myTemplates = new List<SosigEnemyTemplate>();

		public List<Text> List_Categories;

		public List<Text> List_TemplatesToAdd;

		public List<Text> List_TemplatesToRemove;

		private float m_spawnStartTick;

		private bool m_isSpawnStartTickingDown;

		public override void Init(GPSceneMode mode)
		{
			m_categories = ManagerSingleton<IM>.Instance.olistSosigCats;
			base.Init(mode);
			switch (mode)
			{
			case GPSceneMode.Design:
				BTN_Template_SetGroup(0);
				UpdateParamsDisplay();
				break;
			case GPSceneMode.Play:
				SetParamsPanel(b: false);
				if (m_spawnWhen == GPSosigManager.SpawnWhen.OnEvent)
				{
					GM.CurrentGamePlannerManager.AddReceiver(m_eventChannel, SpawnMySosig);
					break;
				}
				m_isSpawnStartTickingDown = true;
				m_spawnStartTick = UnityEngine.Random.Range(0.01f, 0.05f);
				break;
			}
		}

		private void Update()
		{
			if (m_isSpawnStartTickingDown)
			{
				m_spawnStartTick -= Time.deltaTime;
				if (m_spawnStartTick <= 0f)
				{
					m_isSpawnStartTickingDown = false;
					SpawnMySosig();
				}
			}
		}

		public void SpawnMySosig()
		{
			if (GM.CurrentGamePlannerManager.CanSpawnSosigs() && m_myTemplates.Count > 0)
			{
				GM.CurrentGamePlannerManager.SosigManager.SpawnSosig(m_myTemplates[UnityEngine.Random.Range(0, m_myTemplates.Count)], SpawnPoint, m_initialState, m_equipment, m_ammo, m_teamIFF, m_assaultGroup, m_patrolPath);
			}
		}

		public override void UpdateParamsDisplay()
		{
			for (int i = 0; i < List_Categories.Count; i++)
			{
				if (i < m_categories.Count)
				{
					List_Categories[i].gameObject.SetActive(value: true);
					List_Categories[i].text = m_categories[i].ToString();
				}
				else
				{
					List_Categories[i].gameObject.SetActive(value: false);
				}
			}
			for (int j = 0; j < List_TemplatesToAdd.Count; j++)
			{
				if (j < m_curDisplayedTemplatesToAdd.Count)
				{
					List_TemplatesToAdd[j].gameObject.SetActive(value: true);
					List_TemplatesToAdd[j].text = m_curDisplayedTemplatesToAdd[j].DisplayName.ToString();
				}
				else
				{
					List_TemplatesToAdd[j].gameObject.SetActive(value: false);
				}
			}
			for (int k = 0; k < List_TemplatesToRemove.Count; k++)
			{
				if (k < m_myTemplates.Count)
				{
					List_TemplatesToRemove[k].gameObject.SetActive(value: true);
					List_TemplatesToRemove[k].text = m_myTemplates[k].SosigEnemyCategory.ToString() + " > " + m_myTemplates[k].DisplayName;
				}
				else
				{
					List_TemplatesToRemove[k].gameObject.SetActive(value: false);
				}
			}
			LBL_TeamIFF.text = m_teamIFF.ToString();
			LBL_SpawnChannel.text = m_eventChannel.ToString();
			LBL_AssaultGroup.text = m_assaultGroup.ToString();
			LBL_PatrolPath.text = m_patrolPath.ToString();
			OBS_SpawnWhen.SetSelectedButton((int)m_spawnWhen);
			OBS_InitialState.SetSelectedButton((int)m_initialState);
			OBS_Equipment.SetSelectedButton((int)m_equipment);
			OBS_Ammo.SetSelectedButton((int)m_ammo);
		}

		public void BTN_Template_SetGroup(int i)
		{
			m_curDisplayedIDsToAdd.Clear();
			m_curDisplayedTemplatesToAdd.Clear();
			SosigEnemyCategory key = m_categories[i];
			m_curDisplayedIDsToAdd = ManagerSingleton<IM>.Instance.odicSosigIDsByCategory[key];
			m_curDisplayedTemplatesToAdd = ManagerSingleton<IM>.Instance.odicSosigObjsByCategory[key];
			UpdateParamsDisplay();
		}

		public void BTN_Template_Add(int i)
		{
			if (m_myTemplates.Count <= 19)
			{
				m_myTemplates.Add(m_curDisplayedTemplatesToAdd[i]);
				AddToInternalList(m_curDisplayedIDsToAdd[i].ToString(), exclusive: true);
				UpdateParamsDisplay();
			}
		}

		public void BTN_Template_Remove(int i)
		{
			if (m_myTemplates.Count < i)
			{
				m_myTemplates.RemoveAt(i);
				RemoveFromInternalList(m_curDisplayedIDsToAdd[i].ToString());
				UpdateParamsDisplay();
			}
		}

		public void BTN_SpawnWhen(int i)
		{
			m_spawnWhen = (GPSosigManager.SpawnWhen)i;
			GPSosigManager.SpawnWhen spawnWhen = (GPSosigManager.SpawnWhen)i;
			SetFlag("SpawnWhen", spawnWhen.ToString());
			UpdateParamsDisplay();
		}

		public void BTN_InitialState(int i)
		{
			m_initialState = (GPSosigManager.InitialState)i;
			GPSosigManager.InitialState initialState = (GPSosigManager.InitialState)i;
			SetFlag("InitialState", initialState.ToString());
			UpdateParamsDisplay();
		}

		public void BTN_Equipment(int i)
		{
			m_equipment = (GPSosigManager.Equipment)i;
			GPSosigManager.Equipment equipment = (GPSosigManager.Equipment)i;
			SetFlag("Equipment", equipment.ToString());
			UpdateParamsDisplay();
		}

		public void BTN_Ammo(int i)
		{
			m_ammo = (GPSosigManager.Ammo)i;
			GPSosigManager.Ammo ammo = (GPSosigManager.Ammo)i;
			SetFlag("Ammo", ammo.ToString());
			UpdateParamsDisplay();
		}

		public void BTN_ModTeamIFF(int i)
		{
			m_teamIFF += i;
			m_teamIFF = Mathf.Clamp(m_teamIFF, 0, 99);
			SetFlag("TeamIFF", m_teamIFF.ToString());
			UpdateParamsDisplay();
		}

		public void BTN_ModSpawnChannel(int i)
		{
			m_eventChannel += i;
			m_eventChannel = Mathf.Clamp(m_eventChannel, 0, 99);
			SetFlag("EventChannel", m_eventChannel.ToString());
			UpdateParamsDisplay();
		}

		public void BTN_ModAssaultGroup(int i)
		{
			m_assaultGroup += i;
			m_assaultGroup = Mathf.Clamp(m_assaultGroup, 0, 99);
			SetFlag("AssaultGroup", m_assaultGroup.ToString());
			UpdateParamsDisplay();
		}

		public void BTN_ModPatrolPath(int i)
		{
			m_patrolPath += i;
			m_patrolPath = Mathf.Clamp(m_patrolPath, 0, 99);
			SetFlag("PatrolPath", m_patrolPath.ToString());
			UpdateParamsDisplay();
		}

		public override void ConfigureFromSavedPlaceable(GPSavedPlaceable p)
		{
			base.ConfigureFromSavedPlaceable(p);
			for (int i = 0; i < InternalList.Count; i++)
			{
				m_myTemplates.Add(ManagerSingleton<IM>.Instance.odicSosigObjsByID[(SosigEnemyID)Enum.Parse(typeof(SosigEnemyID), InternalList[i])]);
			}
			if (FlagExists("SpawnWhen"))
			{
				m_spawnWhen = (GPSosigManager.SpawnWhen)Enum.Parse(typeof(GPSosigManager.SpawnWhen), Flags["SpawnWhen"]);
			}
			if (FlagExists("InitialState"))
			{
				m_initialState = (GPSosigManager.InitialState)Enum.Parse(typeof(GPSosigManager.InitialState), Flags["InitialState"]);
			}
			if (FlagExists("Equipment"))
			{
				m_equipment = (GPSosigManager.Equipment)Enum.Parse(typeof(GPSosigManager.Equipment), Flags["Equipment"]);
			}
			if (FlagExists("Ammo"))
			{
				m_ammo = (GPSosigManager.Ammo)Enum.Parse(typeof(GPSosigManager.Ammo), Flags["Ammo"]);
			}
			if (FlagExists("TeamIFF"))
			{
				m_teamIFF = int.Parse(Flags["TeamIFF"]);
			}
			if (FlagExists("EventChannel"))
			{
				m_eventChannel = int.Parse(Flags["EventChannel"]);
			}
			if (FlagExists("AssaultGroup"))
			{
				m_assaultGroup = int.Parse(Flags["AssaultGroup"]);
			}
			if (FlagExists("PatrolPath"))
			{
				m_patrolPath = int.Parse(Flags["PatrolPath"]);
			}
		}
	}
}
