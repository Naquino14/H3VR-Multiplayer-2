using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class SMEMErow : MonoBehaviour
	{
		public Image Image;

		public Text LBL_Name;

		public Text LBL_Rarity;

		public Text LBL_Amount;

		public Text LBL_BuySell;

		public Text LBL_Price;

		public Text BuySellButton;

		private ItemSpawnerID m_id;

		private int price;

		private int marketIndex;

		public ItemSpawnerID GetID()
		{
			return m_id;
		}

		public int GetPrice()
		{
			return price;
		}

		public void SetMarketPlaceListing(ItemSpawnerID id, SMEME.HatRarity r, int q, int p)
		{
			m_id = id;
			Image.sprite = id.Sprite;
			LBL_Name.text = id.DisplayName;
			LBL_Rarity.text = r.ToString();
			LBL_Amount.text = q.ToString();
			float num = p;
			num *= 0.01f;
			price = p;
			LBL_Price.text = "G" + num.ToString("C", new CultureInfo("en-US"));
		}
	}
}
