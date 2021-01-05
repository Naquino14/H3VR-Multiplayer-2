using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace FistVR
{
	public class SosigSpawner : FVRPhysicalObject
	{
		public enum SpawnerPageMode
		{
			Player,
			SpawnSosig,
			SpawnEquipment,
			Commands,
			Stats
		}

		public enum CommandToolMode
		{
			Select,
			AddToSelection,
			DeSelectSosig,
			SetGuardPoint,
			SetGuardLookAt,
			SetAssaultPoint,
			Disable,
			SetToDead,
			Splode,
			Delete,
			BeamOff
		}

		public enum CommandGlobal
		{
			ActivateAll,
			DisableAll,
			SetAllToDead,
			SplodeAll,
			DeleteAll
		}

		public enum CommandSelection
		{
			SelectAll,
			DeSelectAll,
			ActivateSelection,
			DisableSelection,
			SetSelectionToDead,
			SplodeSelection,
			DeleteSelection,
			SetIffTo,
			SetBehaviorTo
		}

		[Serializable]
		public class SosigSpawnerGroup
		{
			public string GroupName;

			public List<SosigEnemyTemplate> Templates;

			public List<string> DisplayNames;

			public int DefaultIFF = 1;
		}

		[Header("Sosig Spawner Stuff")]
		public List<SosigSpawnerGroup> SpawnerGroups;

		public List<GameObject> Pages;

		public List<Text> TemplateFields;

		public List<Text> PointedAtFields;

		public SpawnerPageMode PageMode;

		public CommandToolMode ToolMode;

		[Header("Options Panel Button Sets")]
		public OptionsPanel_ButtonSet OBS_TopBar;

		public OptionsPanel_ButtonSet OBS_SpawnSosig_Group;

		public OptionsPanel_ButtonSet OBS_SpawnSosig_Template;

		public OptionsPanel_ButtonSet OBS_SpawnSosig_IFF;

		public OptionsPanel_ButtonSet OBS_SpawnSosig_Equipment;

		public OptionsPanel_ButtonSet OBS_SpawnSosig_Order;

		public OptionsPanel_ButtonSet OBS_SpawnSosig_SpawnActivated;

		public OptionsPanel_ButtonSet OBS_Command_ToolMode;

		public OptionsPanel_ButtonSet OBS_Command_Selection_IFF;

		public OptionsPanel_ButtonSet OBS_Command_Selection_Behavior;

		[Header("Raycast stuff")]
		public Transform Muzzle;

		public Transform PlacementBeam1;

		public Transform PlacementBeam2;

		public Transform PlacementReticle;

		public GameObject PlacementReticle_Valid;

		public GameObject PlacementReticle_Invalid;

		public LayerMask LM_PlacementBeam;

		public float Range_PlacementBeam = 20f;

		public LayerMask LM_SelectionBeam;

		public float Range_SelectionBeam = 20f;

		private RaycastHit m_hit;

		private RaycastHit m_hit2;

		private NavMeshHit m_navHit;

		[Header("Sound stuff")]
		public AudioEvent AudEvent_Category;

		public AudioEvent AudEvent_Select;

		public AudioEvent AudEvent_Spawn;

		public AudioEvent AudEvent_Fail;

		public AudioEvent AudEvent_CommandSelect;

		public AudioEvent AudEvent_CommandDeSelect;

		public AudioEvent AudEvent_CommandChange;

		private List<Sosig> m_activeSosigs = new List<Sosig>();

		private Dictionary<Sosig, Sosig.SosigOrder> m_originalOrderDictionary = new Dictionary<Sosig, Sosig.SosigOrder>();

		private int m_spawn_group;

		private int m_spawn_template;

		private int m_spawn_iff;

		private int m_spawn_initialState;

		private int m_spawn_equipmentMode;

		private bool m_spawn_activated;

		private int m_spawnWithFullAmmo;

		private bool m_canSpawn_Sosig;

		private Vector3 m_sosigSpawn_Point = Vector3.zero;

		private Sosig m_pointedAtSosig;

		private List<Sosig> m_sosigSelection = new List<Sosig>();

		private int m_command_iff;

		private int m_command_behavior;

		private List<Transform> m_selectionRays = new List<Transform>();

		private Vector3 m_pointPoint = Vector3.zero;

		private bool m_hasPointPoint;

		private string Encode(int group, int template, int iff, int equipment, Vector3 pos, Vector3 forward)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(group.ToString());
			stringBuilder.Append("_");
			stringBuilder.Append(template.ToString());
			stringBuilder.Append("_");
			stringBuilder.Append(iff.ToString());
			stringBuilder.Append("_");
			stringBuilder.Append(equipment.ToString());
			stringBuilder.Append("_");
			stringBuilder.Append(pos.x.ToString());
			stringBuilder.Append("_");
			stringBuilder.Append(pos.y.ToString());
			stringBuilder.Append("_");
			stringBuilder.Append(pos.z.ToString());
			stringBuilder.Append("_");
			stringBuilder.Append(forward.x.ToString());
			stringBuilder.Append("_");
			stringBuilder.Append(forward.z.ToString());
			stringBuilder.Append("_");
			stringBuilder.Append("done");
			return stringBuilder.ToString();
		}

		protected override void Awake()
		{
			base.Awake();
			PlacementReticle.SetParent(null);
			PlacementReticle.rotation = Quaternion.identity;
			SetPage(0);
			OBS_SpawnSosig_Group.SetSelectedButton(0);
			OBS_SpawnSosig_Template.SetSelectedButton(0);
			OBS_SpawnSosig_IFF.SetSelectedButton(0);
			OBS_SpawnSosig_Equipment.SetSelectedButton(0);
			OBS_SpawnSosig_Order.SetSelectedButton(0);
			OBS_Command_ToolMode.SetSelectedButton(0);
			OBS_Command_Selection_IFF.SetSelectedButton(0);
			OBS_Command_Selection_Behavior.SetSelectedButton(0);
		}

		private void SetPage(int page)
		{
			for (int i = 0; i < Pages.Count; i++)
			{
				if (i == page)
				{
					Pages[i].SetActive(value: true);
				}
				else
				{
					Pages[i].SetActive(value: false);
				}
			}
			switch (page)
			{
			case 0:
				SetPage_Player();
				break;
			case 1:
				SetPage_SpawnSosig();
				break;
			case 2:
				SetPage_SosigItems();
				break;
			case 3:
				SetPage_Commands();
				break;
			case 4:
				SetPage_Stats();
				break;
			}
			PageMode = (SpawnerPageMode)page;
			PlacementBeam1.gameObject.SetActive(value: false);
			PlacementBeam2.gameObject.SetActive(value: false);
			PlacementReticle.gameObject.SetActive(value: false);
		}

		private void SetPage_Player()
		{
		}

		private void SetPage_SpawnSosig()
		{
			SelectGroup(m_spawn_group);
			SelectTemplate(0);
		}

		private void SetPage_SosigItems()
		{
		}

		private void SetPage_Commands()
		{
		}

		private void SetPage_Stats()
		{
		}

		public void BTN_TopBar_SetPage(int i)
		{
			SetPage(i);
			OBS_TopBar.SetSelectedButton(i);
			SM.PlayGenericSound(AudEvent_Category, base.transform.position);
		}

		public void BTN_Player_SetIFF(int i)
		{
		}

		public void BTN_Player_SetHealth(int i)
		{
		}

		public void BTN_Spawn_Select_Group(int g)
		{
			SM.PlayGenericSound(AudEvent_Select, base.transform.position);
			SelectGroup(g);
		}

		private void SelectGroup(int g)
		{
			OBS_SpawnSosig_Group.SetSelectedButton(g);
			m_spawn_group = g;
			for (int i = 0; i < TemplateFields.Count; i++)
			{
				if (i < SpawnerGroups[m_spawn_group].Templates.Count)
				{
					TemplateFields[i].transform.parent.gameObject.SetActive(value: true);
					TemplateFields[i].text = SpawnerGroups[m_spawn_group].DisplayNames[i];
				}
				else
				{
					TemplateFields[i].transform.parent.gameObject.SetActive(value: false);
				}
			}
			SetSpawnIFF(SpawnerGroups[m_spawn_group].DefaultIFF);
		}

		public void BTN_Spawn_Select_Template(int i)
		{
			SM.PlayGenericSound(AudEvent_Select, base.transform.position);
			SelectTemplate(i);
		}

		private void SelectTemplate(int i)
		{
			OBS_SpawnSosig_Template.SetSelectedButton(i);
			m_spawn_template = i;
		}

		public void BTN_Spawn_Set_IFF(int i)
		{
			SM.PlayGenericSound(AudEvent_Select, base.transform.position);
			SetSpawnIFF(i);
		}

		public void BTN_Spawn_Set_Ammo(int i)
		{
			SM.PlayGenericSound(AudEvent_Select, base.transform.position);
			SetAmmo(i);
		}

		private void SetAmmo(int i)
		{
			m_spawnWithFullAmmo = i;
		}

		private void SetSpawnIFF(int i)
		{
			OBS_SpawnSosig_IFF.SetSelectedButton(i);
			m_spawn_iff = i;
		}

		public void BTN_Spawn_Set_Order(int i)
		{
			OBS_SpawnSosig_Order.SetSelectedButton(i);
			SM.PlayGenericSound(AudEvent_Select, base.transform.position);
			m_spawn_initialState = i;
		}

		public void BTN_Spawn_Set_Equipment(int i)
		{
			OBS_SpawnSosig_Equipment.SetSelectedButton(i);
			SM.PlayGenericSound(AudEvent_Select, base.transform.position);
			m_spawn_equipmentMode = i;
		}

		public void BTN_Spawn_Set_SpawnActivated(int i)
		{
			OBS_SpawnSosig_SpawnActivated.SetSelectedButton(i);
			SM.PlayGenericSound(AudEvent_Select, base.transform.position);
			if (i == 0)
			{
				m_spawn_activated = false;
			}
			else
			{
				m_spawn_activated = true;
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			switch (PageMode)
			{
			case SpawnerPageMode.Player:
				PageUpdate_Player();
				break;
			case SpawnerPageMode.SpawnSosig:
				PageUpdate_SpawnSosig();
				break;
			case SpawnerPageMode.SpawnEquipment:
				PageUpdate_SpawnEquipment();
				break;
			case SpawnerPageMode.Commands:
				PageUpdate_Commands();
				break;
			case SpawnerPageMode.Stats:
				PageUpdate_Stats();
				break;
			}
			BeamUpdate();
		}

		private void PageUpdate_Player()
		{
		}

		private void PageUpdate_SpawnSosig()
		{
			float num = 0f;
			Vector3 zero = Vector3.zero;
			bool flag = false;
			if (Physics.Raycast(Muzzle.position, Muzzle.forward, out m_hit, Range_PlacementBeam, LM_PlacementBeam, QueryTriggerInteraction.Ignore))
			{
				if (Vector3.Angle(m_hit.normal, Vector3.up) < 45f)
				{
					flag = true;
					num = m_hit.distance;
					zero = m_hit.point;
					PlacementBeam1.gameObject.SetActive(value: true);
					PlacementBeam2.gameObject.SetActive(value: false);
					PlacementBeam1.localScale = new Vector3(0.005f, 0.005f, num);
				}
				else
				{
					Vector3 vector = Muzzle.position + Muzzle.forward * Mathf.Clamp(m_hit.distance - 1f, 0.01f, m_hit.distance - 0.6f);
					if (Physics.Raycast(vector, -Vector3.up, out m_hit2, 3f, LM_PlacementBeam, QueryTriggerInteraction.Ignore))
					{
						flag = true;
						num = m_hit.distance;
						zero = vector + -Vector3.up * m_hit2.distance;
						PlacementBeam1.gameObject.SetActive(value: true);
						PlacementBeam2.gameObject.SetActive(value: true);
						PlacementBeam1.localScale = new Vector3(0.005f, 0.005f, num);
						PlacementBeam2.position = vector;
						PlacementBeam2.rotation = Quaternion.LookRotation(-Vector3.up);
						PlacementBeam2.localScale = new Vector3(0.005f, 0.005f, m_hit2.distance);
					}
					else
					{
						flag = false;
						num = m_hit.distance;
						zero = Muzzle.position + Muzzle.forward * m_hit.distance;
						PlacementBeam1.gameObject.SetActive(value: false);
						PlacementBeam2.gameObject.SetActive(value: false);
					}
				}
			}
			else
			{
				flag = false;
				num = Range_PlacementBeam;
				zero = Muzzle.position + Muzzle.forward * Range_PlacementBeam;
				PlacementBeam1.gameObject.SetActive(value: false);
				PlacementBeam2.gameObject.SetActive(value: false);
			}
			if (flag)
			{
				if (NavMesh.SamplePosition(zero, out m_navHit, 0.4f, -1))
				{
					PlacementReticle.gameObject.SetActive(value: true);
					PlacementReticle_Valid.SetActive(value: true);
					PlacementReticle_Invalid.SetActive(value: false);
					m_canSpawn_Sosig = true;
					m_sosigSpawn_Point = m_navHit.position;
					PlacementReticle.position = m_navHit.position + Vector3.up * 0.01f;
				}
				else
				{
					PlacementReticle.gameObject.SetActive(value: true);
					PlacementReticle_Valid.SetActive(value: false);
					PlacementReticle_Invalid.SetActive(value: true);
					m_canSpawn_Sosig = false;
				}
			}
			else
			{
				PlacementReticle.gameObject.SetActive(value: false);
				m_canSpawn_Sosig = false;
			}
		}

		private void PageUpdate_SpawnEquipment()
		{
		}

		private void BeamUpdate()
		{
			int num = 0;
			for (num = 0; num < m_sosigSelection.Count; num++)
			{
				if (m_selectionRays.Count <= num)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(PlacementBeam1.gameObject, Muzzle.position, Muzzle.rotation);
					m_selectionRays.Add(gameObject.transform);
				}
			}
			if (m_selectionRays.Count > m_sosigSelection.Count)
			{
				int num2 = m_selectionRays.Count - m_sosigSelection.Count;
				for (int num3 = m_selectionRays.Count - 1; num3 >= 0; num3--)
				{
					if (num2 > 0)
					{
						UnityEngine.Object.Destroy(m_selectionRays[num3].gameObject);
						m_selectionRays.RemoveAt(num3);
					}
				}
			}
			m_selectionRays.TrimExcess();
			if (ToolMode == CommandToolMode.Select || ToolMode == CommandToolMode.AddToSelection || ToolMode == CommandToolMode.DeSelectSosig)
			{
				for (int i = 0; i < m_selectionRays.Count; i++)
				{
					if (m_sosigSelection[i] != null)
					{
						Vector3 position = m_sosigSelection[i].CoreTarget.transform.position;
						Vector3 forward = position - Muzzle.position;
						m_selectionRays[i].gameObject.SetActive(value: true);
						m_selectionRays[i].position = Muzzle.position;
						m_selectionRays[i].rotation = Quaternion.LookRotation(forward);
						m_selectionRays[i].localScale = new Vector3(0.005f, 0.005f, forward.magnitude);
					}
					else
					{
						m_selectionRays[i].gameObject.SetActive(value: false);
					}
				}
			}
			else if (ToolMode == CommandToolMode.SetGuardPoint)
			{
				for (int j = 0; j < m_selectionRays.Count; j++)
				{
					Vector3 position2 = m_sosigSelection[j].CoreTarget.transform.position;
					Vector3 guardPoint = m_sosigSelection[j].GetGuardPoint();
					Vector3 forward2 = guardPoint - position2;
					m_selectionRays[j].position = position2;
					m_selectionRays[j].rotation = Quaternion.LookRotation(forward2);
					m_selectionRays[j].localScale = new Vector3(0.005f, 0.005f, forward2.magnitude);
				}
			}
			else if (ToolMode == CommandToolMode.SetGuardLookAt)
			{
				for (int k = 0; k < m_selectionRays.Count; k++)
				{
					Vector3 position3 = m_sosigSelection[k].CoreTarget.transform.position;
					Vector3 vector = position3 + m_sosigSelection[k].GetGuardDir();
					Vector3 forward3 = vector - position3;
					m_selectionRays[k].position = position3;
					m_selectionRays[k].rotation = Quaternion.LookRotation(forward3);
					m_selectionRays[k].localScale = new Vector3(0.005f, 0.005f, forward3.magnitude);
				}
			}
			else if (ToolMode == CommandToolMode.SetAssaultPoint)
			{
				for (int l = 0; l < m_selectionRays.Count; l++)
				{
					Vector3 position4 = m_sosigSelection[l].CoreTarget.transform.position;
					Vector3 assaultPoint = m_sosigSelection[l].GetAssaultPoint();
					Vector3 forward4 = assaultPoint - position4;
					m_selectionRays[l].position = position4;
					m_selectionRays[l].rotation = Quaternion.LookRotation(forward4);
					m_selectionRays[l].localScale = new Vector3(0.005f, 0.005f, forward4.magnitude);
				}
			}
			else
			{
				for (int m = 0; m < m_selectionRays.Count; m++)
				{
					m_selectionRays[m].position = Muzzle.position;
					m_selectionRays[m].rotation = Muzzle.rotation;
					m_selectionRays[m].localScale = new Vector3(0.005f, 0.005f, 0.005f);
				}
			}
		}

		private void PageUpdate_Commands()
		{
			m_hasPointPoint = false;
			if (ToolMode != CommandToolMode.BeamOff && ToolMode != CommandToolMode.SetGuardPoint && ToolMode != CommandToolMode.SetGuardLookAt && ToolMode != CommandToolMode.SetAssaultPoint)
			{
				bool flag = false;
				if (Physics.Raycast(Muzzle.position, Muzzle.forward, out m_hit, Range_SelectionBeam, LM_SelectionBeam, QueryTriggerInteraction.Ignore) && m_hit.collider.attachedRigidbody != null)
				{
					SosigLink component = m_hit.collider.attachedRigidbody.gameObject.GetComponent<SosigLink>();
					if (component != null)
					{
						m_pointedAtSosig = component.S;
						flag = true;
					}
				}
				if (!flag)
				{
					m_pointedAtSosig = null;
					PlacementBeam1.gameObject.SetActive(value: false);
				}
				else
				{
					PlacementBeam1.gameObject.SetActive(value: true);
					PlacementBeam1.localScale = new Vector3(0.005f, 0.005f, m_hit.distance);
				}
			}
			else if (ToolMode == CommandToolMode.SetGuardPoint || ToolMode == CommandToolMode.SetGuardLookAt || ToolMode == CommandToolMode.SetAssaultPoint)
			{
				m_pointedAtSosig = null;
				if (Physics.Raycast(Muzzle.position, Muzzle.forward, out m_hit, Range_SelectionBeam, LM_PlacementBeam, QueryTriggerInteraction.Ignore))
				{
					PlacementBeam1.gameObject.SetActive(value: true);
					PlacementBeam1.localScale = new Vector3(0.005f, 0.005f, m_hit.distance);
					m_pointPoint = m_hit.point;
					m_hasPointPoint = true;
				}
				else
				{
					PlacementBeam1.gameObject.SetActive(value: false);
				}
			}
			else
			{
				m_pointedAtSosig = null;
				PlacementBeam1.gameObject.SetActive(value: false);
			}
			if (m_pointedAtSosig != null)
			{
				if (m_pointedAtSosig.BodyState == Sosig.SosigBodyState.Dead)
				{
					PointedAtFields[0].text = "Dead";
					PointedAtFields[1].text = string.Empty;
					PointedAtFields[2].text = string.Empty;
				}
				else
				{
					PointedAtFields[0].text = "Current Behavior: " + m_pointedAtSosig.CurrentOrder;
					PointedAtFields[1].text = "Active Behavior: " + m_pointedAtSosig.FallbackOrder;
					PointedAtFields[2].text = "Active Behavior: " + m_pointedAtSosig.E.IFFCode;
				}
			}
			else
			{
				PointedAtFields[0].text = "None Dectected";
				PointedAtFields[1].text = string.Empty;
				PointedAtFields[2].text = string.Empty;
			}
		}

		public void BTN_Command_SetToolMode(int i)
		{
			SM.PlayGenericSound(AudEvent_Select, base.transform.position);
			OBS_Command_ToolMode.SetSelectedButton(i);
			ToolMode = (CommandToolMode)i;
		}

		public void BTN_Command_Global(int i)
		{
			CleanSelection();
			SM.PlayGenericSound(AudEvent_Spawn, base.transform.position);
			switch (i)
			{
			case 0:
				Command_ActivateAll();
				break;
			case 1:
				Command_DisableAll();
				break;
			case 2:
				Command_SetAllToDead();
				break;
			case 3:
				Command_SplodeAll();
				break;
			case 4:
				Command_DeleteAll();
				break;
			}
			CleanSelection();
		}

		public void BTN_Command_Selection(int i)
		{
			CleanSelection();
			SM.PlayGenericSound(AudEvent_Spawn, base.transform.position);
			switch (i)
			{
			case 0:
				Command_SelectAll();
				break;
			case 1:
				Command_DeSelectAll();
				break;
			case 2:
				Command_ActivateSelection();
				break;
			case 3:
				Command_DisableSelection();
				break;
			case 4:
				Command_SetSelectionToDead();
				break;
			case 5:
				Command_SplodeSelection();
				break;
			case 6:
				Command_DeleteSelection();
				break;
			case 7:
				Command_SetSelectionIFF();
				break;
			case 8:
				Command_SetSelectionBehavior();
				break;
			}
			CleanSelection();
		}

		public void BTN_Command_SetIFF(int i)
		{
			SM.PlayGenericSound(AudEvent_Select, base.transform.position);
			OBS_Command_Selection_IFF.SetSelectedButton(i);
			m_command_iff = i;
		}

		public void BTN_Command_SetBehavior(int i)
		{
			SM.PlayGenericSound(AudEvent_Select, base.transform.position);
			OBS_Command_Selection_Behavior.SetSelectedButton(i);
			m_command_behavior = i;
		}

		private void CleanSelection()
		{
			for (int num = m_sosigSelection.Count - 1; num >= 0; num--)
			{
				if (m_sosigSelection[num] == null)
				{
					ClearSosigFromSystem(m_sosigSelection[num]);
					m_sosigSelection.RemoveAt(num);
				}
			}
		}

		private void TriggerCurrentTool()
		{
			CleanSelection();
			switch (ToolMode)
			{
			case CommandToolMode.Select:
				if (m_pointedAtSosig != null)
				{
					Command_Select(m_pointedAtSosig);
				}
				break;
			case CommandToolMode.AddToSelection:
				if (m_pointedAtSosig != null)
				{
					Command_AddToSelection(m_pointedAtSosig);
				}
				break;
			case CommandToolMode.DeSelectSosig:
				if (m_pointedAtSosig != null)
				{
					Command_DeSelectSosig(m_pointedAtSosig);
				}
				break;
			case CommandToolMode.SetGuardPoint:
			{
				for (int j = 0; j < m_sosigSelection.Count; j++)
				{
					Command_SetGuardPoint(m_sosigSelection[j]);
				}
				break;
			}
			case CommandToolMode.SetGuardLookAt:
			{
				for (int i = 0; i < m_sosigSelection.Count; i++)
				{
					Command_SetGuardLookat(m_sosigSelection[i]);
				}
				break;
			}
			case CommandToolMode.SetAssaultPoint:
			{
				for (int k = 0; k < m_sosigSelection.Count; k++)
				{
					Command_SetAssaultPoint(m_sosigSelection[k]);
				}
				break;
			}
			case CommandToolMode.Disable:
				if (m_pointedAtSosig != null)
				{
					Command_Disable(m_pointedAtSosig);
				}
				break;
			case CommandToolMode.SetToDead:
				if (m_pointedAtSosig != null)
				{
					Command_SetToDead(m_pointedAtSosig);
				}
				break;
			case CommandToolMode.Splode:
				if (m_pointedAtSosig != null)
				{
					Command_Splode(m_pointedAtSosig);
				}
				break;
			case CommandToolMode.Delete:
				if (m_pointedAtSosig != null)
				{
					Command_Delete(m_pointedAtSosig);
				}
				break;
			case CommandToolMode.BeamOff:
				Command_BeamOff();
				break;
			}
			CleanSelection();
		}

		private void ClearSosigFromSystem(Sosig s)
		{
			if (m_activeSosigs.Contains(s))
			{
				m_activeSosigs.Remove(s);
			}
			if (m_originalOrderDictionary.ContainsKey(s))
			{
				m_activeSosigs.Remove(s);
			}
		}

		private void Command_SetWhichIFF(int i)
		{
			m_command_iff = i;
		}

		private void Command_Select(Sosig s)
		{
			Command_DeSelectAll();
			m_sosigSelection.Add(s);
		}

		private void Command_AddToSelection(Sosig s)
		{
			if (!m_sosigSelection.Contains(s))
			{
				m_sosigSelection.Add(s);
			}
		}

		private void Command_DeSelectSosig(Sosig s)
		{
			if (m_sosigSelection.Contains(s))
			{
				m_sosigSelection.Remove(s);
				m_sosigSelection.TrimExcess();
			}
		}

		private void Command_SetGuardPoint(Sosig s)
		{
			if (m_hasPointPoint)
			{
				s.UpdateGuardPoint(m_pointPoint);
			}
		}

		private void Command_SetGuardLookat(Sosig s)
		{
			if (m_hasPointPoint)
			{
				Vector3 dominantGuardDirection = m_pointPoint - s.transform.position;
				dominantGuardDirection.y = 0f;
				dominantGuardDirection.Normalize();
				s.SetDominantGuardDirection(dominantGuardDirection);
			}
		}

		private void Command_SetAssaultPoint(Sosig s)
		{
			if (m_hasPointPoint)
			{
				s.UpdateAssaultPoint(m_pointPoint);
			}
		}

		private void Command_Activate(Sosig s)
		{
			s.SetCurrentOrder(s.FallbackOrder);
		}

		private void Command_Disable(Sosig s)
		{
			s.FallbackOrder = m_originalOrderDictionary[s];
			s.CurrentOrder = Sosig.SosigOrder.Disabled;
		}

		private void Command_SetToDead(Sosig s)
		{
			s.KillSosig();
		}

		private void Command_Splode(Sosig s)
		{
			ClearSosigFromSystem(s);
			s.ClearSosig();
		}

		private void Command_Delete(Sosig s)
		{
			ClearSosigFromSystem(s);
			s.DeSpawnSosig();
		}

		private void Command_SetIff(Sosig s)
		{
			if (m_command_iff < 5)
			{
				s.E.IFFCode = m_command_iff;
			}
			else
			{
				s.E.IFFCode = UnityEngine.Random.Range(5, 10000);
			}
		}

		private void Command_SetBehavior(Sosig s)
		{
			Sosig.SosigOrder sosigOrder = ((m_command_behavior == 0) ? Sosig.SosigOrder.GuardPoint : ((m_command_behavior != 1) ? Sosig.SosigOrder.Assault : Sosig.SosigOrder.Wander));
			if (s.CurrentOrder != 0)
			{
				s.SetCurrentOrder(sosigOrder);
			}
			s.FallbackOrder = sosigOrder;
			m_originalOrderDictionary[s] = sosigOrder;
		}

		private void Command_BeamOff()
		{
		}

		private void Command_ActivateAll()
		{
			for (int num = m_activeSosigs.Count - 1; num >= 0; num--)
			{
				Command_Activate(m_activeSosigs[num]);
			}
		}

		private void Command_DisableAll()
		{
			for (int num = m_activeSosigs.Count - 1; num >= 0; num--)
			{
				Command_Disable(m_activeSosigs[num]);
			}
		}

		private void Command_SetAllToDead()
		{
			for (int num = m_activeSosigs.Count - 1; num >= 0; num--)
			{
				Command_SetToDead(m_activeSosigs[num]);
			}
		}

		private void Command_SplodeAll()
		{
			for (int num = m_activeSosigs.Count - 1; num >= 0; num--)
			{
				Command_Splode(m_activeSosigs[num]);
			}
		}

		private void Command_DeleteAll()
		{
			for (int num = m_activeSosigs.Count - 1; num >= 0; num--)
			{
				Command_Delete(m_activeSosigs[num]);
			}
		}

		private void Command_SelectAll()
		{
			for (int num = m_activeSosigs.Count - 1; num >= 0; num--)
			{
				Command_AddToSelection(m_activeSosigs[num]);
			}
		}

		private void Command_DeSelectAll()
		{
			m_sosigSelection.Clear();
		}

		private void Command_ActivateSelection()
		{
			for (int num = m_sosigSelection.Count - 1; num >= 0; num--)
			{
				Command_Activate(m_sosigSelection[num]);
			}
		}

		private void Command_DisableSelection()
		{
			for (int num = m_sosigSelection.Count - 1; num >= 0; num--)
			{
				Command_Disable(m_sosigSelection[num]);
			}
		}

		private void Command_SetSelectionToDead()
		{
			for (int num = m_sosigSelection.Count - 1; num >= 0; num--)
			{
				Command_SetToDead(m_sosigSelection[num]);
			}
		}

		private void Command_SplodeSelection()
		{
			for (int num = m_sosigSelection.Count - 1; num >= 0; num--)
			{
				Command_Splode(m_sosigSelection[num]);
			}
		}

		private void Command_DeleteSelection()
		{
			for (int num = m_sosigSelection.Count - 1; num >= 0; num--)
			{
				Command_Delete(m_sosigSelection[num]);
			}
		}

		private void Command_SetSelectionIFF()
		{
			for (int num = m_sosigSelection.Count - 1; num >= 0; num--)
			{
				Command_SetIff(m_sosigSelection[num]);
			}
		}

		private void Command_SetSelectionBehavior()
		{
			for (int num = m_sosigSelection.Count - 1; num >= 0; num--)
			{
				Command_SetBehavior(m_sosigSelection[num]);
			}
		}

		private void PageUpdate_Stats()
		{
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			switch (PageMode)
			{
			case SpawnerPageMode.Player:
				break;
			case SpawnerPageMode.SpawnSosig:
				UpdateInteraction_SpawnSosig(hand);
				break;
			case SpawnerPageMode.SpawnEquipment:
				break;
			case SpawnerPageMode.Commands:
				UpdateInteraction_Commands(hand);
				break;
			case SpawnerPageMode.Stats:
				break;
			}
		}

		private void UpdateInteraction_SpawnSosig(FVRViveHand hand)
		{
			if (m_hasTriggeredUpSinceBegin && hand.Input.TriggerDown)
			{
				if (m_canSpawn_Sosig)
				{
					SM.PlayGenericSound(AudEvent_Spawn, base.transform.position);
					SosigEnemyTemplate template = SpawnerGroups[m_spawn_group].Templates[m_spawn_template];
					Vector3 vector = m_sosigSpawn_Point - base.transform.position;
					vector.y = 0f;
					SpawnSosigWithTemplate(template, m_sosigSpawn_Point, -vector);
				}
				else
				{
					SM.PlayGenericSound(AudEvent_Fail, base.transform.position);
				}
			}
		}

		private void UpdateInteraction_Commands(FVRViveHand hand)
		{
			if (m_hasTriggeredUpSinceBegin && hand.Input.TriggerDown)
			{
				TriggerCurrentTool();
			}
		}

		private void SpawnSosigWithTemplate(SosigEnemyTemplate template, Vector3 position, Vector3 forward)
		{
			FVRObject fVRObject = template.SosigPrefabs[UnityEngine.Random.Range(0, template.SosigPrefabs.Count)];
			SosigConfigTemplate t = template.ConfigTemplates[UnityEngine.Random.Range(0, template.ConfigTemplates.Count)];
			SosigOutfitConfig w = template.OutfitConfig[UnityEngine.Random.Range(0, template.OutfitConfig.Count)];
			Sosig sosig = SpawnSosigAndConfigureSosig(fVRObject.GetGameObject(), position, Quaternion.LookRotation(forward, Vector3.up), t, w);
			sosig.InitHands();
			sosig.Inventory.Init();
			if (template.WeaponOptions.Count > 0)
			{
				SosigWeapon sosigWeapon = SpawnWeapon(template.WeaponOptions);
				sosigWeapon.SetAutoDestroy(b: true);
				sosig.ForceEquip(sosigWeapon);
				if (sosigWeapon.Type == SosigWeapon.SosigWeaponType.Gun && m_spawnWithFullAmmo > 0)
				{
					sosig.Inventory.FillAmmoWithType(sosigWeapon.AmmoType);
				}
			}
			bool flag = false;
			if (m_spawn_equipmentMode == 0 || m_spawn_equipmentMode == 2 || (m_spawn_equipmentMode == 3 && UnityEngine.Random.Range(0f, 1f) >= template.SecondaryChance))
			{
				flag = true;
			}
			if (template.WeaponOptions_Secondary.Count > 0 && flag)
			{
				SosigWeapon sosigWeapon2 = SpawnWeapon(template.WeaponOptions_Secondary);
				sosigWeapon2.SetAutoDestroy(b: true);
				sosig.ForceEquip(sosigWeapon2);
				if (sosigWeapon2.Type == SosigWeapon.SosigWeaponType.Gun && m_spawnWithFullAmmo > 0)
				{
					sosig.Inventory.FillAmmoWithType(sosigWeapon2.AmmoType);
				}
			}
			bool flag2 = false;
			if (m_spawn_equipmentMode == 0 || (m_spawn_equipmentMode == 3 && UnityEngine.Random.Range(0f, 1f) >= template.TertiaryChance))
			{
				flag2 = true;
			}
			if (template.WeaponOptions_Tertiary.Count > 0 && flag2)
			{
				SosigWeapon sosigWeapon3 = SpawnWeapon(template.WeaponOptions_Tertiary);
				sosigWeapon3.SetAutoDestroy(b: true);
				sosig.ForceEquip(sosigWeapon3);
				if (sosigWeapon3.Type == SosigWeapon.SosigWeaponType.Gun && m_spawnWithFullAmmo > 0)
				{
					sosig.Inventory.FillAmmoWithType(sosigWeapon3.AmmoType);
				}
			}
			int iFFCode = m_spawn_iff;
			if (m_spawn_iff >= 5)
			{
				iFFCode = UnityEngine.Random.Range(6, 10000);
			}
			sosig.E.IFFCode = iFFCode;
			sosig.CurrentOrder = Sosig.SosigOrder.Disabled;
			switch (m_spawn_initialState)
			{
			case 0:
				sosig.FallbackOrder = Sosig.SosigOrder.Disabled;
				break;
			case 1:
				sosig.FallbackOrder = Sosig.SosigOrder.GuardPoint;
				break;
			case 2:
				sosig.FallbackOrder = Sosig.SosigOrder.Wander;
				break;
			case 3:
				sosig.FallbackOrder = Sosig.SosigOrder.Assault;
				break;
			}
			sosig.UpdateGuardPoint(position);
			sosig.SetDominantGuardDirection(forward);
			sosig.UpdateAssaultPoint(position);
			m_activeSosigs.Add(sosig);
			m_originalOrderDictionary.Add(sosig, sosig.FallbackOrder);
			if (m_spawn_activated)
			{
				sosig.SetCurrentOrder(sosig.FallbackOrder);
			}
		}

		private SosigWeapon SpawnWeapon(List<FVRObject> o)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(o[UnityEngine.Random.Range(0, o.Count)].GetGameObject(), new Vector3(0f, 30f, 0f), Quaternion.identity);
			return gameObject.GetComponent<SosigWeapon>();
		}

		private Sosig SpawnSosigAndConfigureSosig(GameObject prefab, Vector3 pos, Quaternion rot, SosigConfigTemplate t, SosigOutfitConfig w)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, pos, rot);
			Sosig componentInChildren = gameObject.GetComponentInChildren<Sosig>();
			float num = 0f;
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < w.Chance_Headwear)
			{
				SpawnAccesoryToLink(w.Headwear, componentInChildren.Links[0]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < w.Chance_Facewear)
			{
				SpawnAccesoryToLink(w.Facewear, componentInChildren.Links[0]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < w.Chance_Eyewear)
			{
				SpawnAccesoryToLink(w.Eyewear, componentInChildren.Links[0]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < w.Chance_Torsowear)
			{
				SpawnAccesoryToLink(w.Torsowear, componentInChildren.Links[1]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < w.Chance_Pantswear)
			{
				SpawnAccesoryToLink(w.Pantswear, componentInChildren.Links[2]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < w.Chance_Pantswear_Lower)
			{
				SpawnAccesoryToLink(w.Pantswear_Lower, componentInChildren.Links[3]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < w.Chance_Backpacks)
			{
				SpawnAccesoryToLink(w.Backpacks, componentInChildren.Links[1]);
			}
			if (t.UsesLinkSpawns)
			{
				for (int i = 0; i < componentInChildren.Links.Count; i++)
				{
					float num2 = UnityEngine.Random.Range(0f, 1f);
					if (num2 < t.LinkSpawnChance[i])
					{
						componentInChildren.Links[i].RegisterSpawnOnDestroy(t.LinkSpawns[i]);
					}
				}
			}
			componentInChildren.Configure(t);
			return componentInChildren;
		}

		private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l)
		{
			if (gs.Count >= 1)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(gs[UnityEngine.Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
				gameObject.transform.SetParent(l.transform);
				SosigWearable component = gameObject.GetComponent<SosigWearable>();
				component.RegisterWearable(l);
			}
		}
	}
}
