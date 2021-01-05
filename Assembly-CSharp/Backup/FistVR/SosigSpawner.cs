// Decompiled with JetBrains decompiler
// Type: FistVR.SosigSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
  public class SosigSpawner : FVRPhysicalObject
  {
    [Header("Sosig Spawner Stuff")]
    public List<SosigSpawner.SosigSpawnerGroup> SpawnerGroups;
    public List<GameObject> Pages;
    public List<UnityEngine.UI.Text> TemplateFields;
    public List<UnityEngine.UI.Text> PointedAtFields;
    public SosigSpawner.SpawnerPageMode PageMode;
    public SosigSpawner.CommandToolMode ToolMode;
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

    private string Encode(
      int group,
      int template,
      int iff,
      int equipment,
      Vector3 pos,
      Vector3 forward)
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
      this.PlacementReticle.SetParent((Transform) null);
      this.PlacementReticle.rotation = Quaternion.identity;
      this.SetPage(0);
      this.OBS_SpawnSosig_Group.SetSelectedButton(0);
      this.OBS_SpawnSosig_Template.SetSelectedButton(0);
      this.OBS_SpawnSosig_IFF.SetSelectedButton(0);
      this.OBS_SpawnSosig_Equipment.SetSelectedButton(0);
      this.OBS_SpawnSosig_Order.SetSelectedButton(0);
      this.OBS_Command_ToolMode.SetSelectedButton(0);
      this.OBS_Command_Selection_IFF.SetSelectedButton(0);
      this.OBS_Command_Selection_Behavior.SetSelectedButton(0);
    }

    private void SetPage(int page)
    {
      for (int index = 0; index < this.Pages.Count; ++index)
      {
        if (index == page)
          this.Pages[index].SetActive(true);
        else
          this.Pages[index].SetActive(false);
      }
      switch (page)
      {
        case 0:
          this.SetPage_Player();
          break;
        case 1:
          this.SetPage_SpawnSosig();
          break;
        case 2:
          this.SetPage_SosigItems();
          break;
        case 3:
          this.SetPage_Commands();
          break;
        case 4:
          this.SetPage_Stats();
          break;
      }
      this.PageMode = (SosigSpawner.SpawnerPageMode) page;
      this.PlacementBeam1.gameObject.SetActive(false);
      this.PlacementBeam2.gameObject.SetActive(false);
      this.PlacementReticle.gameObject.SetActive(false);
    }

    private void SetPage_Player()
    {
    }

    private void SetPage_SpawnSosig()
    {
      this.SelectGroup(this.m_spawn_group);
      this.SelectTemplate(0);
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
      this.SetPage(i);
      this.OBS_TopBar.SetSelectedButton(i);
      SM.PlayGenericSound(this.AudEvent_Category, this.transform.position);
    }

    public void BTN_Player_SetIFF(int i)
    {
    }

    public void BTN_Player_SetHealth(int i)
    {
    }

    public void BTN_Spawn_Select_Group(int g)
    {
      SM.PlayGenericSound(this.AudEvent_Select, this.transform.position);
      this.SelectGroup(g);
    }

    private void SelectGroup(int g)
    {
      this.OBS_SpawnSosig_Group.SetSelectedButton(g);
      this.m_spawn_group = g;
      for (int index = 0; index < this.TemplateFields.Count; ++index)
      {
        if (index < this.SpawnerGroups[this.m_spawn_group].Templates.Count)
        {
          this.TemplateFields[index].transform.parent.gameObject.SetActive(true);
          this.TemplateFields[index].text = this.SpawnerGroups[this.m_spawn_group].DisplayNames[index];
        }
        else
          this.TemplateFields[index].transform.parent.gameObject.SetActive(false);
      }
      this.SetSpawnIFF(this.SpawnerGroups[this.m_spawn_group].DefaultIFF);
    }

    public void BTN_Spawn_Select_Template(int i)
    {
      SM.PlayGenericSound(this.AudEvent_Select, this.transform.position);
      this.SelectTemplate(i);
    }

    private void SelectTemplate(int i)
    {
      this.OBS_SpawnSosig_Template.SetSelectedButton(i);
      this.m_spawn_template = i;
    }

    public void BTN_Spawn_Set_IFF(int i)
    {
      SM.PlayGenericSound(this.AudEvent_Select, this.transform.position);
      this.SetSpawnIFF(i);
    }

    public void BTN_Spawn_Set_Ammo(int i)
    {
      SM.PlayGenericSound(this.AudEvent_Select, this.transform.position);
      this.SetAmmo(i);
    }

    private void SetAmmo(int i) => this.m_spawnWithFullAmmo = i;

    private void SetSpawnIFF(int i)
    {
      this.OBS_SpawnSosig_IFF.SetSelectedButton(i);
      this.m_spawn_iff = i;
    }

    public void BTN_Spawn_Set_Order(int i)
    {
      this.OBS_SpawnSosig_Order.SetSelectedButton(i);
      SM.PlayGenericSound(this.AudEvent_Select, this.transform.position);
      this.m_spawn_initialState = i;
    }

    public void BTN_Spawn_Set_Equipment(int i)
    {
      this.OBS_SpawnSosig_Equipment.SetSelectedButton(i);
      SM.PlayGenericSound(this.AudEvent_Select, this.transform.position);
      this.m_spawn_equipmentMode = i;
    }

    public void BTN_Spawn_Set_SpawnActivated(int i)
    {
      this.OBS_SpawnSosig_SpawnActivated.SetSelectedButton(i);
      SM.PlayGenericSound(this.AudEvent_Select, this.transform.position);
      if (i == 0)
        this.m_spawn_activated = false;
      else
        this.m_spawn_activated = true;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      switch (this.PageMode)
      {
        case SosigSpawner.SpawnerPageMode.Player:
          this.PageUpdate_Player();
          break;
        case SosigSpawner.SpawnerPageMode.SpawnSosig:
          this.PageUpdate_SpawnSosig();
          break;
        case SosigSpawner.SpawnerPageMode.SpawnEquipment:
          this.PageUpdate_SpawnEquipment();
          break;
        case SosigSpawner.SpawnerPageMode.Commands:
          this.PageUpdate_Commands();
          break;
        case SosigSpawner.SpawnerPageMode.Stats:
          this.PageUpdate_Stats();
          break;
      }
      this.BeamUpdate();
    }

    private void PageUpdate_Player()
    {
    }

    private void PageUpdate_SpawnSosig()
    {
      float num = 0.0f;
      Vector3 zero = Vector3.zero;
      bool flag;
      Vector3 sourcePosition;
      if (Physics.Raycast(this.Muzzle.position, this.Muzzle.forward, out this.m_hit, this.Range_PlacementBeam, (int) this.LM_PlacementBeam, QueryTriggerInteraction.Ignore))
      {
        if ((double) Vector3.Angle(this.m_hit.normal, Vector3.up) < 45.0)
        {
          flag = true;
          float distance = this.m_hit.distance;
          sourcePosition = this.m_hit.point;
          this.PlacementBeam1.gameObject.SetActive(true);
          this.PlacementBeam2.gameObject.SetActive(false);
          this.PlacementBeam1.localScale = new Vector3(0.005f, 0.005f, distance);
        }
        else
        {
          Vector3 origin = this.Muzzle.position + this.Muzzle.forward * Mathf.Clamp(this.m_hit.distance - 1f, 0.01f, this.m_hit.distance - 0.6f);
          if (Physics.Raycast(origin, -Vector3.up, out this.m_hit2, 3f, (int) this.LM_PlacementBeam, QueryTriggerInteraction.Ignore))
          {
            flag = true;
            float distance = this.m_hit.distance;
            sourcePosition = origin + -Vector3.up * this.m_hit2.distance;
            this.PlacementBeam1.gameObject.SetActive(true);
            this.PlacementBeam2.gameObject.SetActive(true);
            this.PlacementBeam1.localScale = new Vector3(0.005f, 0.005f, distance);
            this.PlacementBeam2.position = origin;
            this.PlacementBeam2.rotation = Quaternion.LookRotation(-Vector3.up);
            this.PlacementBeam2.localScale = new Vector3(0.005f, 0.005f, this.m_hit2.distance);
          }
          else
          {
            flag = false;
            num = this.m_hit.distance;
            sourcePosition = this.Muzzle.position + this.Muzzle.forward * this.m_hit.distance;
            this.PlacementBeam1.gameObject.SetActive(false);
            this.PlacementBeam2.gameObject.SetActive(false);
          }
        }
      }
      else
      {
        flag = false;
        num = this.Range_PlacementBeam;
        sourcePosition = this.Muzzle.position + this.Muzzle.forward * this.Range_PlacementBeam;
        this.PlacementBeam1.gameObject.SetActive(false);
        this.PlacementBeam2.gameObject.SetActive(false);
      }
      if (flag)
      {
        if (NavMesh.SamplePosition(sourcePosition, out this.m_navHit, 0.4f, -1))
        {
          this.PlacementReticle.gameObject.SetActive(true);
          this.PlacementReticle_Valid.SetActive(true);
          this.PlacementReticle_Invalid.SetActive(false);
          this.m_canSpawn_Sosig = true;
          this.m_sosigSpawn_Point = this.m_navHit.position;
          this.PlacementReticle.position = this.m_navHit.position + Vector3.up * 0.01f;
        }
        else
        {
          this.PlacementReticle.gameObject.SetActive(true);
          this.PlacementReticle_Valid.SetActive(false);
          this.PlacementReticle_Invalid.SetActive(true);
          this.m_canSpawn_Sosig = false;
        }
      }
      else
      {
        this.PlacementReticle.gameObject.SetActive(false);
        this.m_canSpawn_Sosig = false;
      }
    }

    private void PageUpdate_SpawnEquipment()
    {
    }

    private void BeamUpdate()
    {
      for (int index = 0; index < this.m_sosigSelection.Count; ++index)
      {
        if (this.m_selectionRays.Count <= index)
          this.m_selectionRays.Add(UnityEngine.Object.Instantiate<GameObject>(this.PlacementBeam1.gameObject, this.Muzzle.position, this.Muzzle.rotation).transform);
      }
      if (this.m_selectionRays.Count > this.m_sosigSelection.Count)
      {
        int num = this.m_selectionRays.Count - this.m_sosigSelection.Count;
        for (int index = this.m_selectionRays.Count - 1; index >= 0; --index)
        {
          if (num > 0)
          {
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_selectionRays[index].gameObject);
            this.m_selectionRays.RemoveAt(index);
          }
        }
      }
      this.m_selectionRays.TrimExcess();
      if (this.ToolMode == SosigSpawner.CommandToolMode.Select || this.ToolMode == SosigSpawner.CommandToolMode.AddToSelection || this.ToolMode == SosigSpawner.CommandToolMode.DeSelectSosig)
      {
        for (int index = 0; index < this.m_selectionRays.Count; ++index)
        {
          if ((UnityEngine.Object) this.m_sosigSelection[index] != (UnityEngine.Object) null)
          {
            Vector3 forward = this.m_sosigSelection[index].CoreTarget.transform.position - this.Muzzle.position;
            this.m_selectionRays[index].gameObject.SetActive(true);
            this.m_selectionRays[index].position = this.Muzzle.position;
            this.m_selectionRays[index].rotation = Quaternion.LookRotation(forward);
            this.m_selectionRays[index].localScale = new Vector3(0.005f, 0.005f, forward.magnitude);
          }
          else
            this.m_selectionRays[index].gameObject.SetActive(false);
        }
      }
      else if (this.ToolMode == SosigSpawner.CommandToolMode.SetGuardPoint)
      {
        for (int index = 0; index < this.m_selectionRays.Count; ++index)
        {
          Vector3 position = this.m_sosigSelection[index].CoreTarget.transform.position;
          Vector3 forward = this.m_sosigSelection[index].GetGuardPoint() - position;
          this.m_selectionRays[index].position = position;
          this.m_selectionRays[index].rotation = Quaternion.LookRotation(forward);
          this.m_selectionRays[index].localScale = new Vector3(0.005f, 0.005f, forward.magnitude);
        }
      }
      else if (this.ToolMode == SosigSpawner.CommandToolMode.SetGuardLookAt)
      {
        for (int index = 0; index < this.m_selectionRays.Count; ++index)
        {
          Vector3 position = this.m_sosigSelection[index].CoreTarget.transform.position;
          Vector3 forward = position + this.m_sosigSelection[index].GetGuardDir() - position;
          this.m_selectionRays[index].position = position;
          this.m_selectionRays[index].rotation = Quaternion.LookRotation(forward);
          this.m_selectionRays[index].localScale = new Vector3(0.005f, 0.005f, forward.magnitude);
        }
      }
      else if (this.ToolMode == SosigSpawner.CommandToolMode.SetAssaultPoint)
      {
        for (int index = 0; index < this.m_selectionRays.Count; ++index)
        {
          Vector3 position = this.m_sosigSelection[index].CoreTarget.transform.position;
          Vector3 forward = this.m_sosigSelection[index].GetAssaultPoint() - position;
          this.m_selectionRays[index].position = position;
          this.m_selectionRays[index].rotation = Quaternion.LookRotation(forward);
          this.m_selectionRays[index].localScale = new Vector3(0.005f, 0.005f, forward.magnitude);
        }
      }
      else
      {
        for (int index = 0; index < this.m_selectionRays.Count; ++index)
        {
          this.m_selectionRays[index].position = this.Muzzle.position;
          this.m_selectionRays[index].rotation = this.Muzzle.rotation;
          this.m_selectionRays[index].localScale = new Vector3(0.005f, 0.005f, 0.005f);
        }
      }
    }

    private void PageUpdate_Commands()
    {
      this.m_hasPointPoint = false;
      if (this.ToolMode != SosigSpawner.CommandToolMode.BeamOff && this.ToolMode != SosigSpawner.CommandToolMode.SetGuardPoint && (this.ToolMode != SosigSpawner.CommandToolMode.SetGuardLookAt && this.ToolMode != SosigSpawner.CommandToolMode.SetAssaultPoint))
      {
        bool flag = false;
        if (Physics.Raycast(this.Muzzle.position, this.Muzzle.forward, out this.m_hit, this.Range_SelectionBeam, (int) this.LM_SelectionBeam, QueryTriggerInteraction.Ignore) && (UnityEngine.Object) this.m_hit.collider.attachedRigidbody != (UnityEngine.Object) null)
        {
          SosigLink component = this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<SosigLink>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          {
            this.m_pointedAtSosig = component.S;
            flag = true;
          }
        }
        if (!flag)
        {
          this.m_pointedAtSosig = (Sosig) null;
          this.PlacementBeam1.gameObject.SetActive(false);
        }
        else
        {
          this.PlacementBeam1.gameObject.SetActive(true);
          this.PlacementBeam1.localScale = new Vector3(0.005f, 0.005f, this.m_hit.distance);
        }
      }
      else if (this.ToolMode == SosigSpawner.CommandToolMode.SetGuardPoint || this.ToolMode == SosigSpawner.CommandToolMode.SetGuardLookAt || this.ToolMode == SosigSpawner.CommandToolMode.SetAssaultPoint)
      {
        this.m_pointedAtSosig = (Sosig) null;
        if (Physics.Raycast(this.Muzzle.position, this.Muzzle.forward, out this.m_hit, this.Range_SelectionBeam, (int) this.LM_PlacementBeam, QueryTriggerInteraction.Ignore))
        {
          this.PlacementBeam1.gameObject.SetActive(true);
          this.PlacementBeam1.localScale = new Vector3(0.005f, 0.005f, this.m_hit.distance);
          this.m_pointPoint = this.m_hit.point;
          this.m_hasPointPoint = true;
        }
        else
          this.PlacementBeam1.gameObject.SetActive(false);
      }
      else
      {
        this.m_pointedAtSosig = (Sosig) null;
        this.PlacementBeam1.gameObject.SetActive(false);
      }
      if ((UnityEngine.Object) this.m_pointedAtSosig != (UnityEngine.Object) null)
      {
        if (this.m_pointedAtSosig.BodyState == Sosig.SosigBodyState.Dead)
        {
          this.PointedAtFields[0].text = "Dead";
          this.PointedAtFields[1].text = string.Empty;
          this.PointedAtFields[2].text = string.Empty;
        }
        else
        {
          this.PointedAtFields[0].text = "Current Behavior: " + this.m_pointedAtSosig.CurrentOrder.ToString();
          this.PointedAtFields[1].text = "Active Behavior: " + this.m_pointedAtSosig.FallbackOrder.ToString();
          this.PointedAtFields[2].text = "Active Behavior: " + this.m_pointedAtSosig.E.IFFCode.ToString();
        }
      }
      else
      {
        this.PointedAtFields[0].text = "None Dectected";
        this.PointedAtFields[1].text = string.Empty;
        this.PointedAtFields[2].text = string.Empty;
      }
    }

    public void BTN_Command_SetToolMode(int i)
    {
      SM.PlayGenericSound(this.AudEvent_Select, this.transform.position);
      this.OBS_Command_ToolMode.SetSelectedButton(i);
      this.ToolMode = (SosigSpawner.CommandToolMode) i;
    }

    public void BTN_Command_Global(int i)
    {
      this.CleanSelection();
      SM.PlayGenericSound(this.AudEvent_Spawn, this.transform.position);
      switch (i)
      {
        case 0:
          this.Command_ActivateAll();
          break;
        case 1:
          this.Command_DisableAll();
          break;
        case 2:
          this.Command_SetAllToDead();
          break;
        case 3:
          this.Command_SplodeAll();
          break;
        case 4:
          this.Command_DeleteAll();
          break;
      }
      this.CleanSelection();
    }

    public void BTN_Command_Selection(int i)
    {
      this.CleanSelection();
      SM.PlayGenericSound(this.AudEvent_Spawn, this.transform.position);
      switch (i)
      {
        case 0:
          this.Command_SelectAll();
          break;
        case 1:
          this.Command_DeSelectAll();
          break;
        case 2:
          this.Command_ActivateSelection();
          break;
        case 3:
          this.Command_DisableSelection();
          break;
        case 4:
          this.Command_SetSelectionToDead();
          break;
        case 5:
          this.Command_SplodeSelection();
          break;
        case 6:
          this.Command_DeleteSelection();
          break;
        case 7:
          this.Command_SetSelectionIFF();
          break;
        case 8:
          this.Command_SetSelectionBehavior();
          break;
      }
      this.CleanSelection();
    }

    public void BTN_Command_SetIFF(int i)
    {
      SM.PlayGenericSound(this.AudEvent_Select, this.transform.position);
      this.OBS_Command_Selection_IFF.SetSelectedButton(i);
      this.m_command_iff = i;
    }

    public void BTN_Command_SetBehavior(int i)
    {
      SM.PlayGenericSound(this.AudEvent_Select, this.transform.position);
      this.OBS_Command_Selection_Behavior.SetSelectedButton(i);
      this.m_command_behavior = i;
    }

    private void CleanSelection()
    {
      for (int index = this.m_sosigSelection.Count - 1; index >= 0; --index)
      {
        if ((UnityEngine.Object) this.m_sosigSelection[index] == (UnityEngine.Object) null)
        {
          this.ClearSosigFromSystem(this.m_sosigSelection[index]);
          this.m_sosigSelection.RemoveAt(index);
        }
      }
    }

    private void TriggerCurrentTool()
    {
      this.CleanSelection();
      switch (this.ToolMode)
      {
        case SosigSpawner.CommandToolMode.Select:
          if ((UnityEngine.Object) this.m_pointedAtSosig != (UnityEngine.Object) null)
          {
            this.Command_Select(this.m_pointedAtSosig);
            break;
          }
          break;
        case SosigSpawner.CommandToolMode.AddToSelection:
          if ((UnityEngine.Object) this.m_pointedAtSosig != (UnityEngine.Object) null)
          {
            this.Command_AddToSelection(this.m_pointedAtSosig);
            break;
          }
          break;
        case SosigSpawner.CommandToolMode.DeSelectSosig:
          if ((UnityEngine.Object) this.m_pointedAtSosig != (UnityEngine.Object) null)
          {
            this.Command_DeSelectSosig(this.m_pointedAtSosig);
            break;
          }
          break;
        case SosigSpawner.CommandToolMode.SetGuardPoint:
          for (int index = 0; index < this.m_sosigSelection.Count; ++index)
            this.Command_SetGuardPoint(this.m_sosigSelection[index]);
          break;
        case SosigSpawner.CommandToolMode.SetGuardLookAt:
          for (int index = 0; index < this.m_sosigSelection.Count; ++index)
            this.Command_SetGuardLookat(this.m_sosigSelection[index]);
          break;
        case SosigSpawner.CommandToolMode.SetAssaultPoint:
          for (int index = 0; index < this.m_sosigSelection.Count; ++index)
            this.Command_SetAssaultPoint(this.m_sosigSelection[index]);
          break;
        case SosigSpawner.CommandToolMode.Disable:
          if ((UnityEngine.Object) this.m_pointedAtSosig != (UnityEngine.Object) null)
          {
            this.Command_Disable(this.m_pointedAtSosig);
            break;
          }
          break;
        case SosigSpawner.CommandToolMode.SetToDead:
          if ((UnityEngine.Object) this.m_pointedAtSosig != (UnityEngine.Object) null)
          {
            this.Command_SetToDead(this.m_pointedAtSosig);
            break;
          }
          break;
        case SosigSpawner.CommandToolMode.Splode:
          if ((UnityEngine.Object) this.m_pointedAtSosig != (UnityEngine.Object) null)
          {
            this.Command_Splode(this.m_pointedAtSosig);
            break;
          }
          break;
        case SosigSpawner.CommandToolMode.Delete:
          if ((UnityEngine.Object) this.m_pointedAtSosig != (UnityEngine.Object) null)
          {
            this.Command_Delete(this.m_pointedAtSosig);
            break;
          }
          break;
        case SosigSpawner.CommandToolMode.BeamOff:
          this.Command_BeamOff();
          break;
      }
      this.CleanSelection();
    }

    private void ClearSosigFromSystem(Sosig s)
    {
      if (this.m_activeSosigs.Contains(s))
        this.m_activeSosigs.Remove(s);
      if (!this.m_originalOrderDictionary.ContainsKey(s))
        return;
      this.m_activeSosigs.Remove(s);
    }

    private void Command_SetWhichIFF(int i) => this.m_command_iff = i;

    private void Command_Select(Sosig s)
    {
      this.Command_DeSelectAll();
      this.m_sosigSelection.Add(s);
    }

    private void Command_AddToSelection(Sosig s)
    {
      if (this.m_sosigSelection.Contains(s))
        return;
      this.m_sosigSelection.Add(s);
    }

    private void Command_DeSelectSosig(Sosig s)
    {
      if (!this.m_sosigSelection.Contains(s))
        return;
      this.m_sosigSelection.Remove(s);
      this.m_sosigSelection.TrimExcess();
    }

    private void Command_SetGuardPoint(Sosig s)
    {
      if (!this.m_hasPointPoint)
        return;
      s.UpdateGuardPoint(this.m_pointPoint);
    }

    private void Command_SetGuardLookat(Sosig s)
    {
      if (!this.m_hasPointPoint)
        return;
      Vector3 v = this.m_pointPoint - s.transform.position;
      v.y = 0.0f;
      v.Normalize();
      s.SetDominantGuardDirection(v);
    }

    private void Command_SetAssaultPoint(Sosig s)
    {
      if (!this.m_hasPointPoint)
        return;
      s.UpdateAssaultPoint(this.m_pointPoint);
    }

    private void Command_Activate(Sosig s) => s.SetCurrentOrder(s.FallbackOrder);

    private void Command_Disable(Sosig s)
    {
      s.FallbackOrder = this.m_originalOrderDictionary[s];
      s.CurrentOrder = Sosig.SosigOrder.Disabled;
    }

    private void Command_SetToDead(Sosig s) => s.KillSosig();

    private void Command_Splode(Sosig s)
    {
      this.ClearSosigFromSystem(s);
      s.ClearSosig();
    }

    private void Command_Delete(Sosig s)
    {
      this.ClearSosigFromSystem(s);
      s.DeSpawnSosig();
    }

    private void Command_SetIff(Sosig s)
    {
      if (this.m_command_iff < 5)
        s.E.IFFCode = this.m_command_iff;
      else
        s.E.IFFCode = UnityEngine.Random.Range(5, 10000);
    }

    private void Command_SetBehavior(Sosig s)
    {
      Sosig.SosigOrder o = this.m_command_behavior != 0 ? (this.m_command_behavior != 1 ? Sosig.SosigOrder.Assault : Sosig.SosigOrder.Wander) : Sosig.SosigOrder.GuardPoint;
      if (s.CurrentOrder != Sosig.SosigOrder.Disabled)
        s.SetCurrentOrder(o);
      s.FallbackOrder = o;
      this.m_originalOrderDictionary[s] = o;
    }

    private void Command_BeamOff()
    {
    }

    private void Command_ActivateAll()
    {
      for (int index = this.m_activeSosigs.Count - 1; index >= 0; --index)
        this.Command_Activate(this.m_activeSosigs[index]);
    }

    private void Command_DisableAll()
    {
      for (int index = this.m_activeSosigs.Count - 1; index >= 0; --index)
        this.Command_Disable(this.m_activeSosigs[index]);
    }

    private void Command_SetAllToDead()
    {
      for (int index = this.m_activeSosigs.Count - 1; index >= 0; --index)
        this.Command_SetToDead(this.m_activeSosigs[index]);
    }

    private void Command_SplodeAll()
    {
      for (int index = this.m_activeSosigs.Count - 1; index >= 0; --index)
        this.Command_Splode(this.m_activeSosigs[index]);
    }

    private void Command_DeleteAll()
    {
      for (int index = this.m_activeSosigs.Count - 1; index >= 0; --index)
        this.Command_Delete(this.m_activeSosigs[index]);
    }

    private void Command_SelectAll()
    {
      for (int index = this.m_activeSosigs.Count - 1; index >= 0; --index)
        this.Command_AddToSelection(this.m_activeSosigs[index]);
    }

    private void Command_DeSelectAll() => this.m_sosigSelection.Clear();

    private void Command_ActivateSelection()
    {
      for (int index = this.m_sosigSelection.Count - 1; index >= 0; --index)
        this.Command_Activate(this.m_sosigSelection[index]);
    }

    private void Command_DisableSelection()
    {
      for (int index = this.m_sosigSelection.Count - 1; index >= 0; --index)
        this.Command_Disable(this.m_sosigSelection[index]);
    }

    private void Command_SetSelectionToDead()
    {
      for (int index = this.m_sosigSelection.Count - 1; index >= 0; --index)
        this.Command_SetToDead(this.m_sosigSelection[index]);
    }

    private void Command_SplodeSelection()
    {
      for (int index = this.m_sosigSelection.Count - 1; index >= 0; --index)
        this.Command_Splode(this.m_sosigSelection[index]);
    }

    private void Command_DeleteSelection()
    {
      for (int index = this.m_sosigSelection.Count - 1; index >= 0; --index)
        this.Command_Delete(this.m_sosigSelection[index]);
    }

    private void Command_SetSelectionIFF()
    {
      for (int index = this.m_sosigSelection.Count - 1; index >= 0; --index)
        this.Command_SetIff(this.m_sosigSelection[index]);
    }

    private void Command_SetSelectionBehavior()
    {
      for (int index = this.m_sosigSelection.Count - 1; index >= 0; --index)
        this.Command_SetBehavior(this.m_sosigSelection[index]);
    }

    private void PageUpdate_Stats()
    {
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      switch (this.PageMode)
      {
        case SosigSpawner.SpawnerPageMode.SpawnSosig:
          this.UpdateInteraction_SpawnSosig(hand);
          break;
        case SosigSpawner.SpawnerPageMode.Commands:
          this.UpdateInteraction_Commands(hand);
          break;
      }
    }

    private void UpdateInteraction_SpawnSosig(FVRViveHand hand)
    {
      if (!this.m_hasTriggeredUpSinceBegin || !hand.Input.TriggerDown)
        return;
      if (this.m_canSpawn_Sosig)
      {
        SM.PlayGenericSound(this.AudEvent_Spawn, this.transform.position);
        SosigEnemyTemplate template = this.SpawnerGroups[this.m_spawn_group].Templates[this.m_spawn_template];
        Vector3 vector3 = this.m_sosigSpawn_Point - this.transform.position;
        vector3.y = 0.0f;
        this.SpawnSosigWithTemplate(template, this.m_sosigSpawn_Point, -vector3);
      }
      else
        SM.PlayGenericSound(this.AudEvent_Fail, this.transform.position);
    }

    private void UpdateInteraction_Commands(FVRViveHand hand)
    {
      if (!this.m_hasTriggeredUpSinceBegin || !hand.Input.TriggerDown)
        return;
      this.TriggerCurrentTool();
    }

    private void SpawnSosigWithTemplate(
      SosigEnemyTemplate template,
      Vector3 position,
      Vector3 forward)
    {
      FVRObject sosigPrefab = template.SosigPrefabs[UnityEngine.Random.Range(0, template.SosigPrefabs.Count)];
      SosigConfigTemplate configTemplate = template.ConfigTemplates[UnityEngine.Random.Range(0, template.ConfigTemplates.Count)];
      SosigOutfitConfig w1 = template.OutfitConfig[UnityEngine.Random.Range(0, template.OutfitConfig.Count)];
      Sosig key = this.SpawnSosigAndConfigureSosig(sosigPrefab.GetGameObject(), position, Quaternion.LookRotation(forward, Vector3.up), configTemplate, w1);
      key.InitHands();
      key.Inventory.Init();
      if (template.WeaponOptions.Count > 0)
      {
        SosigWeapon w2 = this.SpawnWeapon(template.WeaponOptions);
        w2.SetAutoDestroy(true);
        key.ForceEquip(w2);
        if (w2.Type == SosigWeapon.SosigWeaponType.Gun && this.m_spawnWithFullAmmo > 0)
          key.Inventory.FillAmmoWithType(w2.AmmoType);
      }
      bool flag1 = false;
      if (this.m_spawn_equipmentMode == 0 || this.m_spawn_equipmentMode == 2 || this.m_spawn_equipmentMode == 3 && (double) UnityEngine.Random.Range(0.0f, 1f) >= (double) template.SecondaryChance)
        flag1 = true;
      if (template.WeaponOptions_Secondary.Count > 0 && flag1)
      {
        SosigWeapon w2 = this.SpawnWeapon(template.WeaponOptions_Secondary);
        w2.SetAutoDestroy(true);
        key.ForceEquip(w2);
        if (w2.Type == SosigWeapon.SosigWeaponType.Gun && this.m_spawnWithFullAmmo > 0)
          key.Inventory.FillAmmoWithType(w2.AmmoType);
      }
      bool flag2 = false;
      if (this.m_spawn_equipmentMode == 0 || this.m_spawn_equipmentMode == 3 && (double) UnityEngine.Random.Range(0.0f, 1f) >= (double) template.TertiaryChance)
        flag2 = true;
      if (template.WeaponOptions_Tertiary.Count > 0 && flag2)
      {
        SosigWeapon w2 = this.SpawnWeapon(template.WeaponOptions_Tertiary);
        w2.SetAutoDestroy(true);
        key.ForceEquip(w2);
        if (w2.Type == SosigWeapon.SosigWeaponType.Gun && this.m_spawnWithFullAmmo > 0)
          key.Inventory.FillAmmoWithType(w2.AmmoType);
      }
      int num = this.m_spawn_iff;
      if (this.m_spawn_iff >= 5)
        num = UnityEngine.Random.Range(6, 10000);
      key.E.IFFCode = num;
      key.CurrentOrder = Sosig.SosigOrder.Disabled;
      switch (this.m_spawn_initialState)
      {
        case 0:
          key.FallbackOrder = Sosig.SosigOrder.Disabled;
          break;
        case 1:
          key.FallbackOrder = Sosig.SosigOrder.GuardPoint;
          break;
        case 2:
          key.FallbackOrder = Sosig.SosigOrder.Wander;
          break;
        case 3:
          key.FallbackOrder = Sosig.SosigOrder.Assault;
          break;
      }
      key.UpdateGuardPoint(position);
      key.SetDominantGuardDirection(forward);
      key.UpdateAssaultPoint(position);
      this.m_activeSosigs.Add(key);
      this.m_originalOrderDictionary.Add(key, key.FallbackOrder);
      if (!this.m_spawn_activated)
        return;
      key.SetCurrentOrder(key.FallbackOrder);
    }

    private SosigWeapon SpawnWeapon(List<FVRObject> o) => UnityEngine.Object.Instantiate<GameObject>(o[UnityEngine.Random.Range(0, o.Count)].GetGameObject(), new Vector3(0.0f, 30f, 0.0f), Quaternion.identity).GetComponent<SosigWeapon>();

    private Sosig SpawnSosigAndConfigureSosig(
      GameObject prefab,
      Vector3 pos,
      Quaternion rot,
      SosigConfigTemplate t,
      SosigOutfitConfig w)
    {
      Sosig componentInChildren = UnityEngine.Object.Instantiate<GameObject>(prefab, pos, rot).GetComponentInChildren<Sosig>();
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) w.Chance_Headwear)
        this.SpawnAccesoryToLink(w.Headwear, componentInChildren.Links[0]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) w.Chance_Facewear)
        this.SpawnAccesoryToLink(w.Facewear, componentInChildren.Links[0]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) w.Chance_Eyewear)
        this.SpawnAccesoryToLink(w.Eyewear, componentInChildren.Links[0]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) w.Chance_Torsowear)
        this.SpawnAccesoryToLink(w.Torsowear, componentInChildren.Links[1]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) w.Chance_Pantswear)
        this.SpawnAccesoryToLink(w.Pantswear, componentInChildren.Links[2]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) w.Chance_Pantswear_Lower)
        this.SpawnAccesoryToLink(w.Pantswear_Lower, componentInChildren.Links[3]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) w.Chance_Backpacks)
        this.SpawnAccesoryToLink(w.Backpacks, componentInChildren.Links[1]);
      if (t.UsesLinkSpawns)
      {
        for (int index = 0; index < componentInChildren.Links.Count; ++index)
        {
          if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) t.LinkSpawnChance[index])
            componentInChildren.Links[index].RegisterSpawnOnDestroy(t.LinkSpawns[index]);
        }
      }
      componentInChildren.Configure(t);
      return componentInChildren;
    }

    private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l)
    {
      if (gs.Count < 1)
        return;
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(gs[UnityEngine.Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
      gameObject.transform.SetParent(l.transform);
      gameObject.GetComponent<SosigWearable>().RegisterWearable(l);
    }

    public enum SpawnerPageMode
    {
      Player,
      SpawnSosig,
      SpawnEquipment,
      Commands,
      Stats,
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
      BeamOff,
    }

    public enum CommandGlobal
    {
      ActivateAll,
      DisableAll,
      SetAllToDead,
      SplodeAll,
      DeleteAll,
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
      SetBehaviorTo,
    }

    [Serializable]
    public class SosigSpawnerGroup
    {
      public string GroupName;
      public List<SosigEnemyTemplate> Templates;
      public List<string> DisplayNames;
      public int DefaultIFF = 1;
    }
  }
}
