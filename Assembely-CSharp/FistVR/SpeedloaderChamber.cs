using UnityEngine;

namespace FistVR
{
	public class SpeedloaderChamber : MonoBehaviour
	{
		public Speedloader SpeedLoader;

		public FireArmRoundType Type;

		public FireArmRoundClass LoadedClass;

		public MeshFilter Filter;

		public Renderer LoadedRenderer;

		public bool IsLoaded = true;

		public bool IsSpent;

		private void Awake()
		{
		}

		public void Load(FireArmRoundClass rclass, bool playSound = false)
		{
			IsLoaded = true;
			IsSpent = false;
			LoadedClass = rclass;
			Filter.mesh = AM.GetRoundMesh(Type, LoadedClass);
			LoadedRenderer.material = AM.GetRoundMaterial(Type, LoadedClass);
			LoadedRenderer.enabled = true;
			if (playSound && SpeedLoader.ProfileOverride != null)
			{
				SM.PlayGenericSound(SpeedLoader.ProfileOverride.MagazineInsertRound, base.transform.position);
			}
		}

		public void LoadEmpty(FireArmRoundClass rclass, bool playSound = false)
		{
			IsLoaded = true;
			IsSpent = true;
			LoadedClass = rclass;
			Filter.mesh = AM.GetRoundSelfPrefab(Type, LoadedClass).GetGameObject().GetComponent<FVRFireArmRound>()
				.FiredRenderer.gameObject.GetComponent<MeshFilter>().sharedMesh;
			LoadedRenderer.material = AM.GetRoundMaterial(Type, LoadedClass);
			LoadedRenderer.enabled = true;
			if (playSound && SpeedLoader.ProfileOverride != null)
			{
				SM.PlayGenericSound(SpeedLoader.ProfileOverride.MagazineInsertRound, base.transform.position);
			}
		}

		public FireArmRoundClass Unload()
		{
			IsLoaded = false;
			LoadedRenderer.enabled = false;
			return LoadedClass;
		}
	}
}
