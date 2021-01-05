using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Prefab Dir", menuName = "AudioPooling/PrefabDirectory", order = 0)]
	public class FVRPooledAudioPrefabDirectory : ScriptableObject
	{
		public List<PoolTypePrefabBinding> PrefabBindings;
	}
}
