using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class PTargetManager : MonoBehaviour
	{
		public enum RangeType
		{
			Meters,
			Yards,
			Feet
		}

		public enum TargetSelectionMode
		{
			Category,
			Target,
			Details
		}

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

		public RangeType m_curRangeUnits;

		private int m_curRangeValue;

		public RangeType m_enteredRangeUnits;

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

		private TargetSelectionMode TSelectionMode;

		public PTargetCategoryDic CategoryDic;

		private PTargetProfile m_selectedProfile;

		private PTargetCategoryDic.PTCat m_selectedCategory;

		public Renderer TargetDisplayRend;

		private void Start()
		{
			SetSelectionMode(TargetSelectionMode.Category);
			SetCategory(0);
			SetProfile(m_selectedCategory, 0);
			UpdateTextDisplays();
			ResetTarget();
			m_height = DefaultHeight;
			TXT_Height.text = m_height.ToString();
		}

		private void Beep()
		{
			SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Beep, base.transform.position);
		}

		private void Boop()
		{
			SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Boop, base.transform.position);
		}

		private void SetCategory(int i)
		{
			m_selectedCategory = CategoryDic.Cats[i];
		}

		private void SetProfile(PTargetCategoryDic.PTCat c, int index)
		{
			m_selectedProfile = c.Targets[index].GetGameObject().GetComponent<PTargetReferenceHolder>().Profile;
			TargetDisplayRend.material.SetTexture("_MainTex", m_selectedProfile.background.material.GetTexture("_MainTex"));
		}

		public void ButtonPressed_RaiseTarget()
		{
			m_height += HeightIncrement;
			m_height = Mathf.Clamp(m_height, HeightClamps.x, HeightClamps.y);
			TXT_Height.text = m_height.ToString();
			RJ.SetHeight(m_height);
			Beep();
		}

		public void ButtonPressed_LowerTarget()
		{
			m_height -= HeightIncrement;
			m_height = Mathf.Clamp(m_height, HeightClamps.x, HeightClamps.y);
			TXT_Height.text = m_height.ToString();
			RJ.SetHeight(m_height);
			Boop();
		}

		public void ButtonPressed_TargetOption(int i)
		{
			switch (TSelectionMode)
			{
			case TargetSelectionMode.Category:
				SetCategory(i);
				SetSelectionMode(TargetSelectionMode.Target);
				break;
			case TargetSelectionMode.Target:
				SetProfile(m_selectedCategory, i);
				SetSelectionMode(TargetSelectionMode.Details);
				break;
			}
			Beep();
		}

		public void ButtonPressed_Back()
		{
			switch (TSelectionMode)
			{
			case TargetSelectionMode.Target:
				SetSelectionMode(TargetSelectionMode.Category);
				break;
			case TargetSelectionMode.Details:
				SetSelectionMode(TargetSelectionMode.Target);
				break;
			}
			Boop();
		}

		private void SetSelectionMode(TargetSelectionMode m)
		{
			TSelectionMode = m;
			switch (m)
			{
			case TargetSelectionMode.Category:
			{
				TXT_Category.text = "Select Category";
				BTN_Back.SetActive(value: false);
				CNT_Selection.SetActive(value: true);
				CNT_Details.SetActive(value: false);
				for (int j = 0; j < IMG_Buttons.Count; j++)
				{
					if (j < CategoryDic.Cats.Count)
					{
						IMG_Buttons[j].gameObject.SetActive(value: true);
						IMG_Buttons[j].sprite = CategoryDic.Cats[j].CatImage;
					}
					else
					{
						IMG_Buttons[j].gameObject.SetActive(value: false);
					}
				}
				break;
			}
			case TargetSelectionMode.Target:
			{
				TXT_Category.text = m_selectedCategory.Name;
				BTN_Back.SetActive(value: true);
				CNT_Selection.SetActive(value: true);
				CNT_Details.SetActive(value: false);
				for (int i = 0; i < IMG_Buttons.Count; i++)
				{
					if (i < m_selectedCategory.Targets.Count)
					{
						IMG_Buttons[i].gameObject.SetActive(value: true);
						IMG_Buttons[i].sprite = m_selectedCategory.TargetIcons[i];
					}
					else
					{
						IMG_Buttons[i].gameObject.SetActive(value: false);
					}
				}
				break;
			}
			case TargetSelectionMode.Details:
				TXT_Category.text = m_selectedProfile.displayName;
				BTN_Back.SetActive(value: true);
				CNT_Selection.SetActive(value: false);
				CNT_Details.SetActive(value: true);
				IMG_Details.sprite = m_selectedProfile.displayIcon;
				TXT_Details_Name.text = m_selectedProfile.displayName;
				TXT_Details_Descrip.text = m_selectedProfile.displayDetails;
				break;
			}
		}

		public void ButtonPressed_Units(int which)
		{
			m_enteredRangeUnits = (RangeType)which;
			m_curEnteredValue = Mathf.Clamp(m_curEnteredValue, 0, RangeClamps[which]);
			UpdateTextDisplays();
			Beep();
		}

		public void ButtonPressed_Number(int num)
		{
			string text = m_curEnteredValue.ToString();
			text += num;
			m_curEnteredValue = Convert.ToInt32(text);
			m_curEnteredValue = Mathf.Clamp(m_curEnteredValue, 0, RangeClamps[(int)m_enteredRangeUnits]);
			UpdateTextDisplays();
			Beep();
		}

		public void ButtonPressed_Clear()
		{
			m_curEnteredValue = 0;
			UpdateTextDisplays();
			Boop();
		}

		public void ButtonPressed_Reset()
		{
			m_curEnteredValue = 0;
			m_curRangeValue = 0;
			RJ.GoToDistance(0f);
			UpdateTextDisplays();
			Boop();
		}

		public void ButtonPressed_GoToDistance()
		{
			m_curRangeUnits = m_enteredRangeUnits;
			m_curRangeValue = m_curEnteredValue;
			float distance = (float)m_curRangeValue * m_rangeMultipliers[(int)m_curRangeUnits];
			RJ.GoToDistance(distance);
			UpdateTextDisplays();
			Boop();
		}

		public void ButtonPressed_TargetHealth(int i)
		{
			switch (i)
			{
			case 0:
				m_targetHealth = 1f;
				m_invuln = false;
				break;
			case 1:
				m_targetHealth = 0.02f;
				m_invuln = false;
				break;
			default:
				m_targetHealth = 1f;
				m_invuln = true;
				break;
			}
			Beep();
		}

		public void ButtonPressed_TargetReset()
		{
			ResetTarget();
			Boop();
		}

		private void ResetTarget()
		{
			Target.ResetTarget(m_selectedProfile, m_targetHealth, m_invuln);
			TargetDisplayRend.material.SetTexture("_MainTex", m_selectedProfile.background.material.GetTexture("_MainTex"));
		}

		private void UpdateTextDisplays()
		{
			TXT_RangeDisplay_Current.text = m_curRangeValue + " " + m_curRangeUnits;
			TXT_RangeDisplay_GoTo.text = "Go To: " + m_curEnteredValue + " " + m_enteredRangeUnits;
		}

		private void Update()
		{
		}
	}
}
