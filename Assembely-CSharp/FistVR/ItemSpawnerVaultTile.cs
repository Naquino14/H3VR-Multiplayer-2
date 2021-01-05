using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class ItemSpawnerVaultTile : MonoBehaviour
	{
		public Image Image;

		public Text Text_Name;

		public Text Text_Date;

		public Image LockedCorner;

		public Text LockedText;

		public GameObject DeleteButton;

		public GameObject ConfirmButton;

		public Image[] AttachedComponents;

		public SavedGun SavedGun;
	}
}
