// Decompiled with JetBrains decompiler
// Type: FistVR.AmmoSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class AmmoSpawner : MonoBehaviour
  {
    public OptionsPanel_ButtonSet ButtonSet;
    public GameObject[] ButtonList;
    public Text[] ButtonText;
    public Text TopBarText;
    public GameObject BackToTypesButton;
    private bool m_isInTypeMode = true;
    private int m_maxTypePages = 1;
    private int m_currentTypePage;
    private int m_currentClassPage;
    private int m_currentTypeSelected;
    private int m_currentClassSelected;
    public GameObject NextPageButton;
    public GameObject PrevPageButton;
    public GameObject SpawnCartridgeButton;
    public GameObject SpawnAmmoBoxButton;
    public GameObject LoadIntoMagButton;
    public Transform SpawnPosition;

    public void NextPage()
    {
      ++this.m_currentTypePage;
      this.m_currentTypePage = Mathf.Clamp(this.m_currentTypePage, 0, this.m_maxTypePages);
      this.UpdateButtonText();
    }

    public void PrevPage()
    {
      --this.m_currentTypePage;
      this.m_currentTypePage = Mathf.Clamp(this.m_currentTypePage, 0, this.m_maxTypePages);
      this.UpdateButtonText();
    }

    private void Start()
    {
      this.m_maxTypePages = Mathf.CeilToInt((float) AM.STypeList.Count / 12f);
      this.m_currentTypePage = 0;
      this.ButtonSet.EnableAllButtons();
      this.UpdateButtonText();
    }

    private void Update()
    {
      if (this.m_isInTypeMode)
        return;
      bool flag = false;
      for (int index = 0; index < GM.CurrentMovementManager.Hands.Length; ++index)
      {
        if ((Object) GM.CurrentMovementManager.Hands[index].CurrentInteractable != (Object) null && GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRPhysicalObject)
        {
          FireArmRoundType stype = AM.STypeList[this.m_currentTypeSelected];
          FireArmRoundClass fireArmRoundClass = AM.STypeClassLists[stype][this.m_currentClassSelected];
          if (GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArmMagazine && (GM.CurrentMovementManager.Hands[index].CurrentInteractable as FVRFireArmMagazine).RoundType == stype)
            flag = true;
          if (GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArmClip && (GM.CurrentMovementManager.Hands[index].CurrentInteractable as FVRFireArmClip).RoundType == stype)
            flag = true;
          if (GM.CurrentMovementManager.Hands[index].CurrentInteractable is Speedloader && (GM.CurrentMovementManager.Hands[index].CurrentInteractable as Speedloader).Chambers[0].Type == stype)
            flag = true;
          if (GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArm)
          {
            FVRFireArm currentInteractable = GM.CurrentMovementManager.Hands[index].CurrentInteractable as FVRFireArm;
            if (currentInteractable.RoundType == stype && (Object) currentInteractable.Magazine != (Object) null)
              flag = true;
          }
        }
      }
      if (flag && !this.LoadIntoMagButton.activeSelf)
      {
        this.LoadIntoMagButton.SetActive(true);
      }
      else
      {
        if (flag || !this.LoadIntoMagButton.activeSelf)
          return;
        this.LoadIntoMagButton.SetActive(false);
      }
    }

    public void LoadIntoHeldObjects()
    {
      FireArmRoundType stype = AM.STypeList[this.m_currentTypeSelected];
      FireArmRoundClass rClass = AM.STypeClassLists[stype][this.m_currentClassSelected];
      for (int index = 0; index < GM.CurrentMovementManager.Hands.Length; ++index)
      {
        if ((Object) GM.CurrentMovementManager.Hands[index].CurrentInteractable != (Object) null && GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRPhysicalObject)
        {
          if (GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArmMagazine)
          {
            FVRFireArmMagazine currentInteractable = GM.CurrentMovementManager.Hands[index].CurrentInteractable as FVRFireArmMagazine;
            if (currentInteractable.RoundType == stype)
              currentInteractable.ReloadMagWithType(rClass);
          }
          if (GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArm)
          {
            FVRFireArm currentInteractable = GM.CurrentMovementManager.Hands[index].CurrentInteractable as FVRFireArm;
            if (currentInteractable.RoundType == stype && (Object) currentInteractable.Magazine != (Object) null)
              currentInteractable.Magazine.ReloadMagWithType(rClass);
          }
          if (GM.CurrentMovementManager.Hands[index].CurrentInteractable is FVRFireArmClip)
          {
            FVRFireArmClip currentInteractable = GM.CurrentMovementManager.Hands[index].CurrentInteractable as FVRFireArmClip;
            if (currentInteractable.RoundType == stype)
              currentInteractable.ReloadClipWithType(rClass);
          }
          if (GM.CurrentMovementManager.Hands[index].CurrentInteractable is Speedloader)
          {
            Speedloader currentInteractable = GM.CurrentMovementManager.Hands[index].CurrentInteractable as Speedloader;
            if (currentInteractable.Chambers[0].Type == stype)
              currentInteractable.ReloadClipWithType(rClass);
          }
        }
      }
    }

    private void UpdateButtonText()
    {
      if (this.m_isInTypeMode)
      {
        this.BackToTypesButton.SetActive(false);
        this.SpawnCartridgeButton.SetActive(false);
        this.SpawnAmmoBoxButton.SetActive(false);
        this.LoadIntoMagButton.SetActive(false);
        this.NextPageButton.SetActive(true);
        this.PrevPageButton.SetActive(true);
        this.TopBarText.text = "Select Caliber " + (object) (this.m_currentTypePage + 1) + "/" + (object) this.m_maxTypePages;
        for (int index1 = 0; index1 < this.ButtonText.Length; ++index1)
        {
          int index2 = index1 + 12 * this.m_currentTypePage;
          if (index2 < AM.STypeList.Count)
          {
            this.ButtonList[index1].SetActive(true);
            this.ButtonText[index1].text = AM.SRoundDisplayDataDic[AM.STypeList[index2]].DisplayName;
          }
          else
          {
            this.ButtonList[index1].SetActive(false);
            this.ButtonText[index1].text = string.Empty;
          }
        }
      }
      else
      {
        this.BackToTypesButton.SetActive(true);
        this.NextPageButton.SetActive(false);
        this.PrevPageButton.SetActive(false);
        this.TopBarText.text = AM.SRoundDisplayDataDic[AM.STypeList[this.m_currentTypeSelected]].DisplayName;
        FireArmRoundType stype = AM.STypeList[this.m_currentTypeSelected];
        for (int index = 0; index < this.ButtonText.Length; ++index)
        {
          if (index < AM.STypeClassLists[stype].Count)
          {
            FireArmRoundClass key = AM.STypeClassLists[stype][index];
            this.ButtonList[index].SetActive(true);
            this.ButtonText[index].text = AM.STypeDic[stype][key].Name;
          }
          else
          {
            this.ButtonList[index].SetActive(false);
            this.ButtonText[index].text = string.Empty;
          }
        }
        FireArmRoundClass rClass = AM.STypeClassLists[stype][this.m_currentClassSelected];
        if ((Object) AM.GetRoundSelfPrefab(stype, rClass) != (Object) null && (Object) AM.GetRoundSelfPrefab(stype, rClass).GetGameObject() != (Object) null)
          this.SpawnCartridgeButton.SetActive(true);
        else
          this.SpawnCartridgeButton.SetActive(false);
        this.SpawnAmmoBoxButton.SetActive(false);
      }
    }

    public void AmmoButtonPressed(int i)
    {
      if (this.m_isInTypeMode)
      {
        this.m_currentTypeSelected = i + 12 * this.m_currentTypePage;
        this.ButtonSet.EnableAllButtons();
        this.m_isInTypeMode = false;
        this.AmmoButtonPressed(0);
      }
      else
      {
        this.m_currentClassSelected = i;
        this.ButtonSet.SetSelectedButton(i);
      }
      this.UpdateButtonText();
    }

    public void ReturnToCaliberSelection()
    {
      this.m_isInTypeMode = true;
      this.ButtonSet.EnableAllButtons();
      this.UpdateButtonText();
    }

    public void SpawnCartridge()
    {
      FireArmRoundType stype = AM.STypeList[this.m_currentTypeSelected];
      FireArmRoundClass rClass = AM.STypeClassLists[stype][this.m_currentClassSelected];
      Object.Instantiate<GameObject>(AM.GetRoundSelfPrefab(stype, rClass).GetGameObject(), this.SpawnPosition.position, this.SpawnPosition.rotation);
    }
  }
}
