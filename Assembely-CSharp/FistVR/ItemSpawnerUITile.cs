using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class ItemSpawnerUITile : MonoBehaviour
	{
		public Image Image;

		public Text Text;

		public Image LockedCorner;

		public Text LockedText;

		public bool IsSpawnable;

		public ItemSpawnerID.EItemCategory Category;

		public ItemSpawnerID.ESubCategory SubCategory;

		public ItemSpawnerID Item;
	}
}
