using UnityEngine;

namespace FistVR
{
	public class GenericCleanupController : MonoBehaviour
	{
		public GameObject DestructibleTargets;

		private Vector3 m_storedPos = Vector3.zero;

		private GameObject SpawnedTargets;

		public void Awake()
		{
			DestructibleTargets.SetActive(value: false);
			m_storedPos = DestructibleTargets.transform.position;
			SpawnedTargets = Object.Instantiate(DestructibleTargets, m_storedPos, Quaternion.identity);
			SpawnedTargets.SetActive(value: true);
		}

		public void DeleteMagazines()
		{
			FVRFireArmMagazine[] array = Object.FindObjectsOfType<FVRFireArmMagazine>();
			for (int num = array.Length - 1; num >= 0; num--)
			{
				if (!array[num].IsHeld && array[num].QuickbeltSlot == null && array[num].FireArm == null)
				{
					Object.Destroy(array[num].gameObject);
				}
			}
			FVRFireArmRound[] array2 = Object.FindObjectsOfType<FVRFireArmRound>();
			for (int num2 = array2.Length - 1; num2 >= 0; num2--)
			{
				if (!array2[num2].IsHeld && array2[num2].QuickbeltSlot == null)
				{
					Object.Destroy(array2[num2].gameObject);
				}
			}
		}

		public void ResetTargetJunk()
		{
			Object.Destroy(SpawnedTargets);
			ShatterablePhysicalObject[] array = Object.FindObjectsOfType<ShatterablePhysicalObject>();
			for (int num = array.Length - 1; num >= 0; num--)
			{
				Object.Destroy(array[num].gameObject);
			}
			ReactiveSteelTarget[] array2 = Object.FindObjectsOfType<ReactiveSteelTarget>();
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].ClearHoles();
			}
			SpawnedTargets = Object.Instantiate(DestructibleTargets, m_storedPos, Quaternion.identity);
			SpawnedTargets.SetActive(value: true);
		}
	}
}
