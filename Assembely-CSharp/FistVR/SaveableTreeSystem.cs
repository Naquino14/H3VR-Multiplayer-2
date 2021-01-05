using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class SaveableTreeSystem : MonoBehaviour
	{
		private Dictionary<string, GameObject> HangableDefs = new Dictionary<string, GameObject>();

		public GameObject SkyRing;

		private void Awake()
		{
			PrimeDics();
			LoadTreeFromDisk();
		}

		private void PrimeDics()
		{
			HangableDef[] array = Resources.LoadAll<HangableDef>("HangableDefs");
			Debug.Log(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				HangableDefs.Add(array[i].ID, array[i].Prefab);
			}
		}

		public void ToggleSkyRing()
		{
			SkyRing.SetActive(!SkyRing.activeSelf);
		}

		private void LoadTreeFromDisk()
		{
			if (!ES2.Exists("MeatmasTree.txt"))
			{
				return;
			}
			using ES2Reader eS2Reader = ES2Reader.Create("MeatmasTree.txt");
			List<string> list = new List<string>();
			List<Vector3> list2 = new List<Vector3>();
			List<Vector3> list3 = new List<Vector3>();
			if (eS2Reader.TagExists("OrnamentList_IDs"))
			{
				list = eS2Reader.ReadList<string>("OrnamentList_IDs");
			}
			if (eS2Reader.TagExists("OrnamentList_Positions"))
			{
				list2 = eS2Reader.ReadList<Vector3>("OrnamentList_Positions");
			}
			if (eS2Reader.TagExists("OrnamentList_Eulers"))
			{
				list3 = eS2Reader.ReadList<Vector3>("OrnamentList_Eulers");
			}
			if (list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					GameObject gameObject = Object.Instantiate(HangableDefs[list[i]], list2[i], Quaternion.identity);
					gameObject.transform.eulerAngles = list3[i];
					MeatmasHangable component = gameObject.GetComponent<MeatmasHangable>();
					component.SetIsKinematicLocked(b: true);
				}
			}
		}

		public void SaveTreeToDisk()
		{
			using ES2Writer eS2Writer = ES2Writer.Create("MeatmasTree.txt");
			MeatmasHangable[] array = Object.FindObjectsOfType<MeatmasHangable>();
			List<string> list = new List<string>();
			List<Vector3> list2 = new List<Vector3>();
			List<Vector3> list3 = new List<Vector3>();
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 a = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
				Vector3 b = new Vector3(array[i].transform.position.x, 0f, array[i].transform.position.z);
				if (Vector3.Distance(a, b) <= 2.5f && array[i].QuickbeltSlot == null && !array[i].IsHeld)
				{
					list.Add(array[i].Def.ID);
					list2.Add(array[i].transform.position);
					list3.Add(array[i].transform.eulerAngles);
				}
			}
			eS2Writer.Write(list, "OrnamentList_IDs");
			eS2Writer.Write(list2, "OrnamentList_Positions");
			eS2Writer.Write(list3, "OrnamentList_Eulers");
			eS2Writer.Save();
		}
	}
}
