using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class TNH_ObjectConstructorIcon : MonoBehaviour
	{
		public enum IconState
		{
			Item,
			Accept,
			Cancel
		}

		public Image Image;

		public List<GameObject> Costs;

		private Sprite m_sprite;

		public int Cost;

		public Sprite Sprite_Accept;

		public Sprite Sprite_Cancel;

		public IconState State;

		public void Init()
		{
		}

		public void SetOption(IconState state, Sprite s, int cost)
		{
			State = state;
			m_sprite = s;
			Cost = cost;
			Cost = Mathf.Clamp(Cost, Cost, 12);
			UpdateCostDisplay();
			UpdateIconDisplay();
		}

		private void UpdateCostDisplay()
		{
			for (int i = 0; i < Costs.Count; i++)
			{
				if (i + 1 == Cost && State == IconState.Item)
				{
					Costs[i].SetActive(value: true);
				}
				else
				{
					Costs[i].SetActive(value: false);
				}
			}
		}

		private void UpdateIconDisplay()
		{
			switch (State)
			{
			case IconState.Item:
				Image.sprite = m_sprite;
				break;
			case IconState.Accept:
				Image.sprite = Sprite_Accept;
				break;
			case IconState.Cancel:
				Image.sprite = Sprite_Cancel;
				break;
			}
		}
	}
}
