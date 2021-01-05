// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigSaveGameManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class ZosigSaveGameManager : MonoBehaviour
  {
    public string LevelName;
    private int m_selectedSlot = -1;
    private bool isLoading;
    private int m_slotToDelete = -1;
    public List<Text> SaveGameSlotTextFields;
    public Color Color_Selected;
    public Color Color_UnSelected;
    public List<GameObject> DeleteButtons = new List<GameObject>();
    public List<GameObject> ConfirmButtons = new List<GameObject>();
    public List<GameObject> DifficultyButtons_0;
    public List<GameObject> DifficultyButtons_1;
    public List<GameObject> DifficultyButtons_2;
    public List<GameObject> IntroButtons_0;
    public List<GameObject> IntroButtons_1;
    public List<GameObject> IntroButtons_2;
    public Text StartButtonText;
    public GameObject StartButton;
    private int m_selectedDifficulty_Slot0;
    private int m_selectedDifficulty_Slot1;
    private int m_selectedDifficulty_Slot2;
    private int m_selectedIntro_Slot0;
    private int m_selectedIntro_Slot1;
    private int m_selectedIntro_Slot2;

    public void Start() => this.Invoke("UpdateUI", 0.1f);

    public void Button_SelectSlot(int i)
    {
      if (this.isLoading)
        return;
      this.m_selectedSlot = i;
      this.m_slotToDelete = -1;
      this.UpdateUI();
    }

    public void Button_Delete()
    {
      if (this.isLoading || this.m_selectedSlot < 0)
        return;
      this.m_slotToDelete = this.m_selectedSlot;
      this.UpdateUI();
    }

    public void Button_SelectDifficulty_Slot0(int i)
    {
      if (this.isLoading)
        return;
      this.m_selectedSlot = 0;
      this.m_selectedDifficulty_Slot0 = i;
      this.UpdateUI();
    }

    public void Button_SelectDifficulty_Slot1(int i)
    {
      if (this.isLoading)
        return;
      this.m_selectedSlot = 1;
      this.m_selectedDifficulty_Slot1 = i;
      this.UpdateUI();
    }

    public void Button_SelectDifficulty_Slot2(int i)
    {
      if (this.isLoading)
        return;
      this.m_selectedSlot = 2;
      this.m_selectedDifficulty_Slot2 = i;
      this.UpdateUI();
    }

    public void Button_SelectIntro_Slot0(int i)
    {
      if (this.isLoading)
        return;
      this.m_selectedSlot = 0;
      this.m_selectedIntro_Slot0 = i;
      this.UpdateUI();
    }

    public void Button_SelectIntro_Slot1(int i)
    {
      if (this.isLoading)
        return;
      this.m_selectedSlot = 1;
      this.m_selectedIntro_Slot1 = i;
      this.UpdateUI();
    }

    public void Button_SelectIntro_Slot2(int i)
    {
      if (this.isLoading)
        return;
      this.m_selectedSlot = 2;
      this.m_selectedIntro_Slot2 = i;
      this.UpdateUI();
    }

    public void Button_Confirm()
    {
      if (this.isLoading || this.m_selectedSlot < 0 || this.m_slotToDelete < 0)
        return;
      this.DeleteSaveGame(this.m_selectedSlot);
      this.m_selectedSlot = -1;
      this.UpdateUI();
    }

    public void Button_StartGame()
    {
      if (this.isLoading || this.m_selectedSlot < 0)
        return;
      switch (this.m_selectedSlot)
      {
        case 0:
          if (GM.ROTRWSaves.SaveGame1.Count < 1)
          {
            GM.ROTRWSaves.SaveGame1.Add("flag_Difficulty", this.m_selectedDifficulty_Slot0);
            GM.ROTRWSaves.SaveGame1.Add("skip_intro", this.m_selectedIntro_Slot0);
            break;
          }
          break;
        case 1:
          if (GM.ROTRWSaves.SaveGame2.Count < 1)
          {
            GM.ROTRWSaves.SaveGame2.Add("flag_Difficulty", this.m_selectedDifficulty_Slot1);
            GM.ROTRWSaves.SaveGame2.Add("skip_intro", this.m_selectedIntro_Slot1);
            break;
          }
          break;
        case 2:
          if (GM.ROTRWSaves.SaveGame3.Count < 1)
          {
            GM.ROTRWSaves.SaveGame3.Add("flag_Difficulty", this.m_selectedDifficulty_Slot2);
            GM.ROTRWSaves.SaveGame3.Add("skip_intro", this.m_selectedIntro_Slot2);
            break;
          }
          break;
      }
      GM.ROTRWSaves.SaveToFile();
      this.m_slotToDelete = -1;
      this.UpdateUI();
      GM.ROTRWSaves.SetCurrentSaveGame(this.m_selectedSlot + 1);
      this.isLoading = true;
      SteamVR_LoadLevel.Begin(this.LevelName);
    }

    private void UpdateUI()
    {
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      if (GM.ROTRWSaves.SaveGame1.Count > 0)
        flag1 = true;
      if (GM.ROTRWSaves.SaveGame2.Count > 0)
        flag2 = true;
      if (GM.ROTRWSaves.SaveGame3.Count > 0)
        flag3 = true;
      if (flag1)
        this.SaveGameSlotTextFields[0].text = !GM.ROTRWSaves.SaveGame1.ContainsKey("flag_Difficulty") || GM.ROTRWSaves.SaveGame1["flag_Difficulty"] <= 1 ? (!GM.ROTRWSaves.SaveGame1.ContainsKey("flag_Difficulty") || GM.ROTRWSaves.SaveGame1["flag_Difficulty"] <= 0 ? "Classic Save 1" : "Arcade Save 1") : "Hardcore Save 1";
      else if (this.m_selectedDifficulty_Slot0 == 0)
        this.SaveGameSlotTextFields[0].text = "New Classic Save";
      else if (this.m_selectedDifficulty_Slot0 == 1)
        this.SaveGameSlotTextFields[0].text = "New Arcade Save";
      else if (this.m_selectedDifficulty_Slot0 == 2)
        this.SaveGameSlotTextFields[0].text = "New Hardcore Save";
      if (flag2)
        this.SaveGameSlotTextFields[1].text = !GM.ROTRWSaves.SaveGame2.ContainsKey("flag_Difficulty") || GM.ROTRWSaves.SaveGame2["flag_Difficulty"] <= 1 ? (!GM.ROTRWSaves.SaveGame2.ContainsKey("flag_Difficulty") || GM.ROTRWSaves.SaveGame2["flag_Difficulty"] <= 0 ? "Classic Save 2" : "Arcade Save 2") : "Hardcore Save 2";
      else if (this.m_selectedDifficulty_Slot1 == 0)
        this.SaveGameSlotTextFields[1].text = "New Classic Save";
      else if (this.m_selectedDifficulty_Slot1 == 1)
        this.SaveGameSlotTextFields[1].text = "New Arcade Save";
      else if (this.m_selectedDifficulty_Slot1 == 2)
        this.SaveGameSlotTextFields[1].text = "New Hardcore Save";
      if (flag3)
        this.SaveGameSlotTextFields[2].text = !GM.ROTRWSaves.SaveGame3.ContainsKey("flag_Difficulty") || GM.ROTRWSaves.SaveGame3["flag_Difficulty"] <= 1 ? (!GM.ROTRWSaves.SaveGame3.ContainsKey("flag_Difficulty") || GM.ROTRWSaves.SaveGame3["flag_Difficulty"] <= 0 ? "Classic Save 3" : "Arcade Save 3") : "Hardcore Save 3";
      else if (this.m_selectedDifficulty_Slot2 == 0)
        this.SaveGameSlotTextFields[2].text = "New Classic Save";
      else if (this.m_selectedDifficulty_Slot2 == 1)
        this.SaveGameSlotTextFields[2].text = "New Arcade Save";
      else if (this.m_selectedDifficulty_Slot2 == 2)
        this.SaveGameSlotTextFields[2].text = "New Hardcore Save";
      if (this.m_selectedSlot > -1)
      {
        switch (this.m_selectedSlot)
        {
          case 0:
            this.SaveGameSlotTextFields[0].color = this.Color_Selected;
            this.SaveGameSlotTextFields[1].color = this.Color_UnSelected;
            this.SaveGameSlotTextFields[2].color = this.Color_UnSelected;
            break;
          case 1:
            this.SaveGameSlotTextFields[0].color = this.Color_UnSelected;
            this.SaveGameSlotTextFields[1].color = this.Color_Selected;
            this.SaveGameSlotTextFields[2].color = this.Color_UnSelected;
            break;
          case 2:
            this.SaveGameSlotTextFields[0].color = this.Color_UnSelected;
            this.SaveGameSlotTextFields[1].color = this.Color_UnSelected;
            this.SaveGameSlotTextFields[2].color = this.Color_Selected;
            break;
        }
      }
      else
      {
        this.SaveGameSlotTextFields[0].color = this.Color_UnSelected;
        this.SaveGameSlotTextFields[1].color = this.Color_UnSelected;
        this.SaveGameSlotTextFields[2].color = this.Color_UnSelected;
      }
      if (flag1)
      {
        for (int index = 0; index < this.DifficultyButtons_0.Count; ++index)
          this.DifficultyButtons_0[index].SetActive(false);
        for (int index = 0; index < this.IntroButtons_0.Count; ++index)
          this.IntroButtons_0[index].SetActive(false);
      }
      else
      {
        for (int index = 0; index < this.DifficultyButtons_0.Count; ++index)
          this.DifficultyButtons_0[index].SetActive(true);
        for (int index = 0; index < this.IntroButtons_0.Count; ++index)
          this.IntroButtons_0[index].SetActive(true);
      }
      if (flag2)
      {
        for (int index = 0; index < this.DifficultyButtons_1.Count; ++index)
          this.DifficultyButtons_1[index].SetActive(false);
        for (int index = 0; index < this.IntroButtons_1.Count; ++index)
          this.IntroButtons_1[index].SetActive(false);
      }
      else
      {
        for (int index = 0; index < this.DifficultyButtons_1.Count; ++index)
          this.DifficultyButtons_1[index].SetActive(true);
        for (int index = 0; index < this.IntroButtons_1.Count; ++index)
          this.IntroButtons_1[index].SetActive(true);
      }
      if (flag3)
      {
        for (int index = 0; index < this.DifficultyButtons_2.Count; ++index)
          this.DifficultyButtons_2[index].SetActive(false);
        for (int index = 0; index < this.IntroButtons_2.Count; ++index)
          this.IntroButtons_2[index].SetActive(false);
      }
      else
      {
        for (int index = 0; index < this.DifficultyButtons_2.Count; ++index)
          this.DifficultyButtons_2[index].SetActive(true);
        for (int index = 0; index < this.IntroButtons_2.Count; ++index)
          this.IntroButtons_2[index].SetActive(true);
      }
      if (this.m_selectedSlot > -1)
      {
        switch (this.m_selectedSlot)
        {
          case 0:
            if (flag1)
              this.DeleteButtons[0].SetActive(true);
            else
              this.DeleteButtons[0].SetActive(false);
            this.DeleteButtons[1].SetActive(false);
            this.DeleteButtons[2].SetActive(false);
            break;
          case 1:
            if (flag2)
              this.DeleteButtons[1].SetActive(true);
            else
              this.DeleteButtons[1].SetActive(false);
            this.DeleteButtons[0].SetActive(false);
            this.DeleteButtons[2].SetActive(false);
            break;
          case 2:
            if (flag3)
              this.DeleteButtons[2].SetActive(true);
            else
              this.DeleteButtons[2].SetActive(false);
            this.DeleteButtons[0].SetActive(false);
            this.DeleteButtons[1].SetActive(false);
            break;
        }
      }
      else
      {
        for (int index = 0; index < this.DeleteButtons.Count; ++index)
          this.DeleteButtons[index].SetActive(false);
      }
      if (this.m_slotToDelete > -1)
      {
        switch (this.m_selectedSlot)
        {
          case 0:
            if (flag1)
              this.ConfirmButtons[0].SetActive(true);
            else
              this.ConfirmButtons[0].SetActive(false);
            this.ConfirmButtons[1].SetActive(false);
            this.ConfirmButtons[2].SetActive(false);
            break;
          case 1:
            if (flag2)
              this.ConfirmButtons[1].SetActive(true);
            else
              this.ConfirmButtons[1].SetActive(false);
            this.ConfirmButtons[0].SetActive(false);
            this.ConfirmButtons[2].SetActive(false);
            break;
          case 2:
            if (flag3)
              this.ConfirmButtons[2].SetActive(true);
            else
              this.ConfirmButtons[2].SetActive(false);
            this.ConfirmButtons[0].SetActive(false);
            this.ConfirmButtons[1].SetActive(false);
            break;
        }
      }
      else
      {
        for (int index = 0; index < this.ConfirmButtons.Count; ++index)
          this.ConfirmButtons[index].SetActive(false);
      }
      if (this.m_selectedSlot < 0)
      {
        this.StartButton.SetActive(false);
      }
      else
      {
        this.StartButton.SetActive(true);
        switch (this.m_selectedSlot)
        {
          case 0:
            if (flag1)
            {
              this.StartButtonText.text = "Load Save Game";
              break;
            }
            this.StartButtonText.text = "Start New Game";
            break;
          case 1:
            if (flag2)
            {
              this.StartButtonText.text = "Load Save Game";
              break;
            }
            this.StartButtonText.text = "Start New Game";
            break;
          case 2:
            if (flag3)
            {
              this.StartButtonText.text = "Load Save Game";
              break;
            }
            this.StartButtonText.text = "Start New Game";
            break;
        }
      }
    }

    private void DeleteSaveGame(int index)
    {
      GM.ROTRWSaves.DeleteSaveGame(index);
      GM.ROTRWSaves.SaveToFile();
      this.m_selectedSlot = -1;
      this.m_slotToDelete = -1;
      this.UpdateUI();
    }

    private void Update()
    {
    }
  }
}
