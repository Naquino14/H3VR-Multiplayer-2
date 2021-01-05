// Decompiled with JetBrains decompiler
// Type: FistVR.PD
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class PD : MonoBehaviour
  {
    [SerializeField]
    protected List<int> m_validSys = new List<int>();
    [SerializeField]
    protected PDSys[] m_sys = new PDSys[256];
    public float TempCoolantRate = 50f;
    [Header("System Connections")]
    public PDElectricalSystem ESystem;
    public PDFCS FCS;
    [Header("Configs")]
    public PDSystemListConfig SLC;
    public PDHierarchyConfig HC_Power;
    public PDHierarchyConfig HC_Coolant;
    [Header("DebugTexture")]
    public Texture2D debugTex_Power;
    public Texture2D debugTex_Coolant;
    public Texture2D debugTex_Heat;
    public Color32 cPower_On;
    public Color32 cPower_Off;
    public Color32 cPower_Emergency;
    public Color32 cPower_None;
    public Color32 cCoolant_On;
    public Color32 cCoolant_Off;
    public Color32 cCoolant_None;
    public Color32 cHeat_Max;
    public Color32 cHeat_Min;
    public Color32 cHeat_None;
    public Color32[] m_colorsDebugPower;
    public Color32[] m_colorsDebugCoolant;
    public Color32[] m_colorsDebugHeat;

    private void Start() => this.ESystem.Init();

    private void Update()
    {
      float deltaTime = Time.deltaTime;
      this.ESystem.Tick(deltaTime);
      if (!Input.GetKeyDown(KeyCode.H))
        ;
      if (!Input.GetKeyDown(KeyCode.J))
        ;
      if (!Input.GetKeyDown(KeyCode.K))
        ;
      if (!Input.GetKeyDown(KeyCode.L))
        ;
      int length1 = this.debugTex_Power.width * this.debugTex_Power.height;
      if (length1 != this.m_colorsDebugPower.Length)
        this.m_colorsDebugPower = new Color32[length1];
      int length2 = this.debugTex_Coolant.width * this.debugTex_Coolant.height;
      if (length2 != this.m_colorsDebugCoolant.Length)
        this.m_colorsDebugCoolant = new Color32[length2];
      int length3 = this.debugTex_Heat.width * this.debugTex_Heat.height;
      if (length3 != this.m_colorsDebugHeat.Length)
        this.m_colorsDebugHeat = new Color32[length3];
      for (int index = 0; index < this.m_validSys.Count; ++index)
      {
        PDSys sy = this.m_sys[this.m_validSys[index]];
        if (sy.NeedsPower)
          sy.UpdatePowerState();
        if (sy.NeedsCoolant || sy.TakesDamFromThermal)
          sy.UpdateCoolantState(deltaTime);
        this.m_colorsDebugPower[this.m_validSys[index]] = sy.NeedsPower || sy.ID == PDComponentID.SYS_PDU || (sy.ID == PDComponentID.SYS_EPO0 || sy.ID == PDComponentID.SYS_EPO1) ? (sy.PState != PDSys.PowerState.Powered ? (sy.PState != PDSys.PowerState.EmergencyPower ? this.cPower_Off : this.cPower_Emergency) : this.cPower_On) : this.cPower_None;
        if (sy.NeedsCoolant || sy.ID == PDComponentID.SYS_CCS)
        {
          this.m_colorsDebugCoolant[this.m_validSys[index]] = sy.CState != PDSys.CoolantState.Pressurized ? this.cCoolant_Off : this.cCoolant_On;
          Color32 color32 = Color32.Lerp(this.cHeat_Min, this.cHeat_Max, sy.GetHeat() / 200f);
          this.m_colorsDebugHeat[this.m_validSys[index]] = color32;
        }
        else
        {
          this.m_colorsDebugCoolant[this.m_validSys[index]] = this.cCoolant_None;
          this.m_colorsDebugHeat[this.m_validSys[index]] = this.cHeat_None;
        }
      }
      this.debugTex_Heat.SetPixels32(this.m_colorsDebugHeat);
      this.debugTex_Heat.Apply();
      this.debugTex_Coolant.SetPixels32(this.m_colorsDebugCoolant);
      this.debugTex_Coolant.Apply();
      this.debugTex_Power.SetPixels32(this.m_colorsDebugPower);
      this.debugTex_Power.Apply();
    }

    private int GetZone(Vector3 worldPoint) => 0;

    public PDSys GetSys(int i) => this.m_sys[i];

    [ContextMenu("ConstructDroneFromConfigFiles")]
    private void ConstructDroneFromConfigFiles()
    {
      this.m_validSys.Clear();
      for (int index = 0; index < this.SLC.Components.Count; ++index)
      {
        PDSys pdSys = new PDSys();
        PDSystemListConfig.ComponentConfig component = this.SLC.Components[index];
        pdSys.PD = this;
        pdSys.ID = component.ID;
        pdSys.SysType = component.SysType;
        pdSys.ArmType = component.ArmType;
        pdSys.OmnType = component.OmnType;
        pdSys.NeedsPower = component.Pow;
        pdSys.NeedsCoolant = component.Col;
        pdSys.TakesDamFromThermal = component.TTD;
        pdSys.CanIgnite = component.CIG;
        this.m_validSys.Add((int) pdSys.ID);
        this.m_sys[(int) pdSys.ID] = pdSys;
      }
      for (int index1 = 0; index1 < this.HC_Power.IDs.Count; ++index1)
      {
        PDHierarchyConfig.PDChildren id = this.HC_Power.IDs[index1];
        for (int index2 = 0; index2 < id.Children.Count; ++index2)
        {
          this.m_sys[(int) id.ID].AddPowerChild(id.Children[index2]);
          this.m_sys[(int) id.Children[index2]].AddPowerParent(id.ID);
        }
      }
      for (int index1 = 0; index1 < this.HC_Coolant.IDs.Count; ++index1)
      {
        PDHierarchyConfig.PDChildren id = this.HC_Coolant.IDs[index1];
        for (int index2 = 0; index2 < id.Children.Count; ++index2)
        {
          this.m_sys[(int) id.ID].AddCoolantChild(id.Children[index2]);
          this.m_sys[(int) id.Children[index2]].AddCoolantParent(id.ID);
        }
      }
    }
  }
}
