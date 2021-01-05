using FistVR;
using UnityEngine;

namespace Anvil
{
	[ExecuteInEditMode]
	public class AnvilPrefabSpawn : MonoBehaviour
	{
		public AnvilAsset Prefab;

		public wwTargetManager tm;

		private void Awake()
		{
			if (Application.isPlaying)
			{
				AnvilCallback<GameObject> gameObjectAsync = Prefab.GetGameObjectAsync();
				if (gameObjectAsync.IsCompleted)
				{
					SpawnChild(gameObjectAsync.Result);
				}
				else
				{
					gameObjectAsync.AddCallback(SpawnChild);
				}
			}
		}

		private void SpawnChild(GameObject result)
		{
			InstantiateAndZero(result);
		}

		public GameObject InstantiateAndZero(GameObject result)
		{
			GameObject gameObject = Object.Instantiate(result, base.transform.position, base.transform.rotation);
			gameObject.SetActive(value: true);
			if (tm != null)
			{
				gameObject.GetComponent<wwTargetShatterable>().SetManager(tm);
			}
			return gameObject;
		}
	}
}
