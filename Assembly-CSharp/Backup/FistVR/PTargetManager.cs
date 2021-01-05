// Decompiled with JetBrains decompiler
// Type: FistVR.PTargetManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class PTargetManager : MonoBehaviour
  {
    public PTargetRailJoint RJ;
    public Text TXT_RangeDisplay_Current;
    public Text TXT_RangeDisplay_GoTo;
    public float[] m_rangeMultipliers;
    public int[] RangeClamps = new int[3];
    public Vector2 HeightClamps;
    public float DefaultHeight;
    public float HeightIncrement;
    public Text TXT_Height;
    private float m_height;
    public PTargetManager.RangeType m_curRangeUnits;
    private int m_curRangeValue;
    public PTargetManager.RangeType m_enteredRangeUnits;
    private int m_curEnteredValue;
    public AudioEvent AudEvent_Beep;
    public AudioEvent AudEvent_Boop;
    private float m_targetHealth = 1f;
    private bool m_invuln;
    public PTarget Target;
    public GameObject CNT_Selection;
    public GameObject CNT_Details;
    public List<Image> IMG_Buttons;
    public Image IMG_Details;
    public Text TXT_Details_Name;
    public Text TXT_Details_Descrip;
    public GameObject BTN_Back;
    public Text TXT_Category;
    private PTargetManager.TargetSelectionMode TSelectionMode;
    public PTargetCategoryDic CategoryDic;
    private PTargetProfile m_selectedProfile;
    private PTargetCategoryDic.PTCat m_selectedCategory;
    public Renderer TargetDisplayRend;

    private void Start()
    {
      this.SetSelectionMode(PTargetManager.TargetSelectionMode.Category);
      this.SetCategory(0);
      this.SetProfile(this.m_selectedCategory, 0);
      this.UpdateTextDisplays();
      this.ResetTarget();
      this.m_height = this.DefaultHeight;
      this.TXT_Height.text = this.m_height.ToString();
    }

    private void Beep() => SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Beep, this.transform.position);

    private void Boop() => SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Boop, this.transform.position);

    private void SetCategory(int i) => this.m_selectedCategory = this.CategoryDic.Cats[i];

    private void SetProfile(PTargetCategoryDic.PTCat c, int index)
    {
      this.m_selectedProfile = c.Targets[index].GetGameObject().GetComponent<PTargetReferenceHolder>().Profile;
      this.TargetDisplayRend.material.SetTexture("_MainTex", this.m_selectedProfile.background.material.GetTexture("_MainTex"));
    }

    public void ButtonPressed_RaiseTarget()
    {
      this.m_height += this.HeightIncrement;
      this.m_height = Mathf.Clamp(this.m_height, this.HeightClamps.x, this.HeightClamps.y);
      this.TXT_Height.text = this.m_height.ToString();
      this.RJ.SetHeight(this.m_height);
      this.Beep();
    }

    public void ButtonPressed_LowerTarget()
    {
      this.m_height -= this.HeightIncrement;
      this.m_height = Mathf.Clamp(this.m_height, this.HeightClamps.x, this.HeightClamps.y);
      this.TXT_Height.text = this.m_height.ToString();
      this.RJ.SetHeight(this.m_height);
      this.Boop();
    }

    public void ButtonPressed_TargetOption(int i)
    {
      switch (this.TSelectionMode)
      {
        case PTargetManager.TargetSelectionMode.Category:
          this.SetCategory(i);
          this.SetSelectionMode(PTargetManager.TargetSelectionMode.Target);
          break;
        case PTargetManager.TargetSelectionMode.Target:
          this.SetProfile(this.m_selectedCategory, i);
          this.SetSelectionMode(PTargetManager.TargetSelectionMode.Details);
          break;
      }
      this.Beep();
    }

    public void ButtonPressed_Back()
    {
      switch (this.TSelectionMode)
      {
        case PTargetManager.TargetSelectionMode.Target:
          this.SetSelectionMode(PTargetManager.TargetSelectionMode.Category);
          break;
        case PTargetManager.TargetSelectionMode.Details:
          this.SetSelectionMode(PTargetManager.TargetSelectionMode.Target);
          break;
      }
      this.Boop();
    }

    private void SetSelectionMode(PTargetManager.TargetSelectionMode m)
    {
      this.TSelectionMode = m;
      switch (m)
      {
        case PTargetManager.TargetSelectionMode.Category:
          this.TXT_Category.text = "Select Category";
          this.BTN_Back.SetActive(false);
          this.CNT_Selection.SetActive(true);
          this.CNT_Details.SetActive(false);
          for (int index = 0; index < this.IMG_Buttons.Count; ++index)
          {
            if (index < this.CategoryDic.Cats.Count)
            {
              this.IMG_Buttons[index].gameObject.SetActive(true);
              this.IMG_Buttons[index].sprite = this.CategoryDic.Cats[index].CatImage;
            }
            else
              this.IMG_Buttons[index].gameObject.SetActive(false);
          }
          break;
        case PTargetManager.TargetSelectionMode.Target:
          this.TXT_Category.text = this.m_selectedCategory.Name;
          this.BTN_Back.SetActive(true);
          this.CNT_Selection.SetActive(true);
          this.CNT_Details.SetActive(false);
          for (int index = 0; index < this.IMG_Buttons.Count; ++index)
          {
            if (index < this.m_selectedCategory.Targets.Count)
            {
              this.IMG_Buttons[index].gameObject.SetActive(true);
              this.IMG_Buttons[index].sprite = this.m_selectedCategory.TargetIcons[index];
            }
            else
              this.IMG_Buttons[index].gameObject.SetActive(false);
          }
          break;
        case PTargetManager.TargetSelectionMode.Details:
          this.TXT_Category.text = this.m_selectedProfile.displayName;
          this.BTN_Back.SetActive(true);
          this.CNT_Selection.SetActive(false);
          this.CNT_Details.SetActive(true);
          this.IMG_Details.sprite = this.m_selectedProfile.displayIcon;
          this.TXT_Details_Name.text = this.m_selectedProfile.displayName;
          this.TXT_Details_Descrip.text = this.m_selectedProfile.displayDetails;
          break;
      }
    }

    public void ButtonPressed_Units(int which)
    {
      this.m_enteredRangeUnits = (PTargetManager.RangeType) which;
      this.m_curEnteredValue = Mathf.Clamp(this.m_curEnteredValue, 0, this.RangeClamps[which]);
      this.UpdateTextDisplays();
      this.Beep();
    }

    public void ButtonPressed_Number(int num)
    {
      this.m_curEnteredValue = Convert.ToInt32(this.m_curEnteredValue.ToString() + num.ToString());
      this.m_curEnteredValue = Mathf.Clamp(this.m_curEnteredValue, 0, this.RangeClamps[(int) this.m_enteredRangeUnits]);
      this.UpdateTextDisplays();
      this.Beep();
    }

    public void ButtonPressed_Clear()
    {
      this.m_curEnteredValue = 0;
      this.UpdateTextDisplays();
      this.Boop();
    }

    public void ButtonPressed_Reset()
    {
      this.m_curEnteredValue = 0;
      this.m_curRangeValue = 0;
      this.RJ.GoToDistance(0.0f);
      this.UpdateTextDisplays();
      this.Boop();
    }

    public void ButtonPressed_GoToDistance()
    {
      this.m_curRangeUnits = this.m_enteredRangeUnits;
      this.m_curRangeValue = this.m_curEnteredValue;
      this.RJ.GoToDistance((float) this.m_curRangeValue * this.m_rangeMultipliers[(int) this.m_curRangeUnits]);
      this.UpdateTextDisplays();
      this.Boop();
    }

    public void ButtonPressed_TargetHealth(int i)
    {
      switch (i)
      {
        case 0:
          this.m_targetHealth = 1f;
          this.m_invuln = false;
          break;
        case 1:
          this.m_targetHealth = 0.02f;
          this.m_invuln = false;
          break;
        default:
          this.m_targetHealth = 1f;
          this.m_invuln = true;
          break;
      }
      this.Beep();
    }

    public void ButtonPressed_TargetReset()
    {
      this.ResetTarget();
      this.Boop();
    }

    private void ResetTarget()
    {
      this.Target.ResetTarget(this.m_selectedProfile, this.m_targetHealth, this.m_invuln);
      this.TargetDisplayRend.material.SetTexture("_MainTex", this.m_selectedProfile.background.material.GetTexture("_MainTex"));
    }

    private void UpdateTextDisplays()
    {
      this.TXT_RangeDisplay_Current.text = this.m_curRangeValue.ToString() + " " + this.m_curRangeUnits.ToString();
      this.TXT_RangeDisplay_GoTo.text = "Go To: " + this.m_curEnteredValue.ToString() + " " + this.m_enteredRangeUnits.ToString();
    }

    private void Update()
    {
    }

    public enum RangeType
    {
      Meters,
      Yards,
      Feet,
    }

    public enum TargetSelectionMode
    {
      Category,
      Target,
      Details,
    }
  }
}
