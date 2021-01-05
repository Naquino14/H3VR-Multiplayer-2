// Decompiled with JetBrains decompiler
// Type: FistVR.AmmoSpawnerV2
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class AmmoSpawnerV2 : MonoBehaviour
  {
    public OptionsPanel_ButtonSet OBS_Category;
    public OptionsPanel_ButtonSet OBS_Type;
    public OptionsPanel_ButtonSet OBS_Class;
    public List<Text> BTNS_Category;
    public List<Text> BTNS_Type;
    public List<Text> BTNS_Class;
    public Text LBL_Page_Type;
    public Text LBL_Page_Class;
    private int m_curCategory;
    private int m_typePage;
    private int m_classPage;
    private int m_maxTypePage;
    private int m_maxClassPage;
    private FireArmRoundType m_curAmmoType;
    private FireArmRoundClass m_curAmmoClass = FireArmRoundClass.FMJ;
    public GameObject BTNGO_Select;
    public GameObject BTNGO_Fill;
    public Transform SpawnPosition;
    public AudioEvent AudEvent_Beep;
    public AudioEvent AudEvent_Boop;
    private List<AmmoSpawnerV2.CartridgeCategory> Categories = new List<AmmoSpawnerV2.CartridgeCategory>();
    private Dictionary<FireArmRoundType, int> reverseCatDic = new Dictionary<FireArmRoundType, int>();
    private bool m_hasHeldType;
    private FireArmRoundType heldType = FireArmRoundType.a10mmAuto;

    public void Beep() => SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Beep, this.transform.position);

    private void Boop() => SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Boop, this.transform.position);

    private void Start()
    {
      this.PrimeCats();
      this.SetCategory(0);
      this.UpdateDisplay();
    }

    public void SpawnCartridge() => UnityEngine.Object.Instantiate<GameObject>(AM.GetRoundSelfPrefab(this.m_curAmmoType, this.m_curAmmoClass).GetGameObject(), this.SpawnPosition.position, this.SpawnPosition.rotation);

    private void Update() => this.CheckFillButton();

    private void UpdateDisplay()
    {
      this.LBL_Page_Type.text = (this.m_typePage + 1).ToString() + "/" + this.m_maxTypePage.ToString();
      int index1 = 0 + this.m_typePage * 10;
      for (int index2 = 0; index2 < this.BTNS_Type.Count; ++index2)
      {
        if (index1 < this.Categories[this.m_curCategory].Entries.Count)
        {
          this.BTNS_Type[index2].gameObject.SetActive(true);
          this.BTNS_Type[index2].text = this.Categories[this.m_curCategory].Entries[index1].DisplayName;
        }
        else
          this.BTNS_Type[index2].gameObject.SetActive(false);
        ++index1;
      }
      this.LBL_Page_Class.text = (this.m_classPage + 1).ToString() + "/" + (this.m_maxClassPage + 1).ToString();
      int index3 = 0 + this.m_classPage * 10;
      for (int index2 = 0; index2 < this.BTNS_Class.Count; ++index2)
      {
        if (index3 < AM.SRoundDisplayDataDic[this.m_curAmmoType].Classes.Length)
        {
          this.BTNS_Class[index2].gameObject.SetActive(true);
          this.BTNS_Class[index2].text = AM.SRoundDisplayDataDic[this.m_curAmmoType].Classes[index3].Name;
        }
        else
          this.BTNS_Class[index2].gameObject.SetActive(false);
        ++index3;
      }
    }

    public void PButton_SetCategory(int i)
    {
      this.Beep();
      this.SetCategory(i);
      this.UpdateDisplay();
    }

    public void PButton_SetType(int i)
    {
      this.Beep();
      this.SetType(this.Categories[this.m_curCategory].Entries[i + this.m_typePage * 10].Type);
      this.UpdateDisplay();
    }

    public void PButton_SetClass(int i)
    {
      this.Beep();
      this.SetClass(AM.SRoundDisplayDataDic[this.m_curAmmoType].Classes[i + this.m_classPage * 10].Class);
      this.UpdateDisplay();
    }

    private void SetCategory(int i)
    {
      this.m_curCategory = i;
      this.ResetType();
      this.m_typePage = 0;
      this.m_maxTypePage = Mathf.CeilToInt((float) this.Categories[this.m_curCategory].Entries.Count / 10f);
      this.ResetClass();
    }

    private void SetType(FireArmRoundType t)
    {
      this.m_curAmmoType = t;
      this.ResetClass();
      this.m_classPage = 0;
      this.m_maxClassPage = Mathf.CeilToInt((float) AM.STypeClassLists[this.m_curAmmoType].Count / 10f);
    }

    private void SetClass(FireArmRoundClass c) => this.m_curAmmoClass = c;

    private void ResetType()
    {
      this.m_curAmmoType = this.Categories[this.m_curCategory].Entries[0].Type;
      this.OBS_Type.SetSelectedButton(0);
    }

    private void ResetClass()
    {
      this.m_curAmmoClass = AM.STypeClassLists[this.m_curAmmoType][0];
      this.OBS_Class.SetSelectedButton(0);
    }

    public void PButton_Type_Prev()
    {
      --this.m_typePage;
      if (this.m_typePage < 0)
        this.m_typePage = 0;
      this.Boop();
      this.UpdateDisplay();
    }

    public void PButton_Type_Next()
    {
      ++this.m_typePage;
      if (this.m_typePage > this.m_maxTypePage)
        this.m_typePage = Mathf.Clamp(this.m_typePage, 0, this.m_maxTypePage - 1);
      this.Beep();
      this.UpdateDisplay();
    }

    public void PButton_Class_Prev()
    {
      --this.m_classPage;
      if (this.m_classPage < 0)
        this.m_classPage = 0;
      this.Boop();
      this.UpdateDisplay();
    }

    public void PButton_Class_Next()
    {
      ++this.m_classPage;
      if (this.m_classPage >= this.m_maxClassPage)
        this.m_classPage = Mathf.Clamp(this.m_classPage, 0, this.m_maxClassPage - 1);
      this.Beep();
      this.UpdateDisplay();
    }

    public void PButton_SelectHeldType()
    {
      if (this.m_hasHeldType)
      {
        this.Beep();
        this.SetCategory(this.reverseCatDic[this.heldType]);
        this.OBS_Category.SetSelectedButton(this.reverseCatDic[this.heldType]);
        this.SetType(this.heldType);
        this.SetCorrectTypePage(this.heldType);
        this.UpdateDisplay();
      }
      else
        this.Boop();
    }

    public void PButton_SpawnCartridge()
    {
      this.Beep();
      this.SpawnCartridge();
    }

    public void PButton_FillHeldObject()
    {
      this.Beep();
      this.LoadIntoHeldObjects();
    }

    private void SetCorrectTypePage(FireArmRoundType t)
    {
      for (int index1 = 0; index1 < this.Categories[this.reverseCatDic[t]].Entries.Count; ++index1)
      {
        if (this.Categories[this.reverseCatDic[t]].Entries[index1].Type == t)
        {
          int num = Mathf.FloorToInt((float) (index1 / 10));
          int index2 = index1 % 10;
          this.m_typePage = num;
          this.OBS_Type.SetSelectedButton(index2);
          break;
        }
      }
    }

    private bool IsRoundOfPower(
      FVRFireArmRoundDisplayData data,
      FVRObject.OTagFirearmRoundPower p,
      bool mf)
    {
      return mf != data.IsMeatFortress && (mf && data.IsMeatFortress || data.RoundPower == p);
    }

    private void PrimeCats()
    {
      for (int index = 0; index < 10; ++index)
        this.Categories.Add(new AmmoSpawnerV2.CartridgeCategory());
      for (int index1 = 0; index1 < AM.STypeList.Count; ++index1)
      {
        FVRFireArmRoundDisplayData roundDisplayData = AM.SRoundDisplayDataDic[AM.STypeList[index1]];
        int index2 = !roundDisplayData.IsMeatFortress ? (int) (roundDisplayData.RoundPower - 1) : 8;
        this.Categories[index2].Entries.Add(roundDisplayData);
        this.reverseCatDic.Add(roundDisplayData.Type, index2);
      }
    }

    public void LoadIntoHeldObjects()
    {
      FireArmRoundType curAmmoType = this.m_curAmmoType;
      FireArmRoundClass curAmmoClass = this.m_curAmmoClass;
      for (int index = 0; index < GM.CurrentMovementManager.Hands.Length; ++index)
      {
        if ((UnityEngine.Object) GM.CurrentMovementManager.Hands[index].CurrentInteractable != (UnityEngine.Object) null && GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRPhysicalObject)
        {
          if (GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArmMagazine)
          {
            FVRFireArmMagazine currentInteractable = GM.CurrentMovementManager.Hands[index].CurrentInteractable as FVRFireArmMagazine;
            if (currentInteractable.RoundType == curAmmoType)
              currentInteractable.ReloadMagWithType(curAmmoClass);
          }
          if (GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArm)
          {
            FVRFireArm currentInteractable = GM.CurrentMovementManager.Hands[index].CurrentInteractable as FVRFireArm;
            if (currentInteractable.RoundType == curAmmoType && (UnityEngine.Object) currentInteractable.Magazine != (UnityEngine.Object) null)
              currentInteractable.Magazine.ReloadMagWithType(curAmmoClass);
          }
          if (GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArmClip)
          {
            FVRFireArmClip currentInteractable = GM.CurrentMovementManager.Hands[index].CurrentInteractable as FVRFireArmClip;
            if (currentInteractable.RoundType == curAmmoType)
              currentInteractable.ReloadClipWithType(curAmmoClass);
          }
          if (GM.CurrentMovementManager.Hands[index].CurrentInteractable is Speedloader)
          {
            Speedloader currentInteractable = GM.CurrentMovementManager.Hands[index].CurrentInteractable as Speedloader;
            if (currentInteractable.Chambers[0].Type == curAmmoType)
              currentInteractable.ReloadClipWithType(curAmmoClass);
          }
        }
      }
    }

    private void CheckFillButton()
    {
      bool flag = false;
      for (int index = 0; index < GM.CurrentMovementManager.Hands.Length; ++index)
      {
        if ((UnityEngine.Object) GM.CurrentMovementManager.Hands[index].CurrentInteractable != (UnityEngine.Object) null && GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRPhysicalObject)
        {
          FireArmRoundType curAmmoType = this.m_curAmmoType;
          FireArmRoundClass curAmmoClass = this.m_curAmmoClass;
          if (GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArmMagazine)
          {
            FVRFireArmMagazine currentInteractable = GM.CurrentMovementManager.Hands[index].CurrentInteractable as FVRFireArmMagazine;
            this.m_hasHeldType = true;
            this.heldType = currentInteractable.RoundType;
            if (currentInteractable.RoundType == curAmmoType)
              flag = true;
          }
          if (GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArmClip)
          {
            FVRFireArmClip currentInteractable = GM.CurrentMovementManager.Hands[index].CurrentInteractable as FVRFireArmClip;
            this.m_hasHeldType = true;
            this.heldType = currentInteractable.RoundType;
            if (currentInteractable.RoundType == curAmmoType)
              flag = true;
          }
          if (GM.CurrentMovementManager.Hands[index].CurrentInteractable is Speedloader)
          {
            Speedloader currentInteractable = GM.CurrentMovementManager.Hands[index].CurrentInteractable as Speedloader;
            this.m_hasHeldType = true;
            this.heldType = currentInteractable.Chambers[0].Type;
            if (currentInteractable.Chambers[0].Type == curAmmoType)
              flag = true;
          }
          if (GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArm)
          {
            FVRFireArm currentInteractable = GM.CurrentMovementManager.Hands[index].CurrentInteractable as FVRFireArm;
            this.m_hasHeldType = true;
            this.heldType = currentInteractable.RoundType;
            if (currentInteractable.RoundType == curAmmoType && (UnityEngine.Object) currentInteractable.Magazine != (UnityEngine.Object) null)
              flag = true;
          }
        }
      }
      if (flag && !this.BTNGO_Fill.activeSelf)
        this.BTNGO_Fill.SetActive(true);
      else if (!flag && this.BTNGO_Fill.activeSelf)
        this.BTNGO_Fill.SetActive(false);
      if (this.m_hasHeldType && !this.BTNGO_Select.activeSelf)
      {
        this.BTNGO_Select.SetActive(true);
      }
      else
      {
        if (this.m_hasHeldType || !this.BTNGO_Select.activeSelf)
          return;
        this.BTNGO_Select.SetActive(false);
      }
    }

    [Serializable]
    private class CartridgeCategory
    {
      public List<FVRFireArmRoundDisplayData> Entries = new List<FVRFireArmRoundDisplayData>();
    }
  }
}
