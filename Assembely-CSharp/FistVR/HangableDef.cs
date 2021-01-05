using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Definition", menuName = "Meatmas/HangableDef", order = 0)]
	public class HangableDef : ScriptableObject
	{
		public string ID;

		public GameObject Prefab;
	}
}
