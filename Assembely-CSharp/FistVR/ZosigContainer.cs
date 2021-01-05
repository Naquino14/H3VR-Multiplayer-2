using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigContainer : MonoBehaviour
	{
		protected bool m_isOpen;

		protected bool m_containsItems;

		protected FVRObject m_storedObject1;

		protected List<GameObject> m_spawnedObjects = new List<GameObject>();

		public bool SetFlagOnOpenDestroy;

		public string FlagToSet;

		public int ValueToSet;

		public void FlagOpen()
		{
			if (SetFlagOnOpenDestroy && GM.ZMaster != null)
			{
				GM.ZMaster.FlagM.SetFlagMaxBlend(FlagToSet, ValueToSet);
			}
		}

		public virtual void PlaceObjectsInContainer(FVRObject obj1, int minAmmo = -1, int maxAmmo = 30)
		{
			m_containsItems = true;
		}

		public void TestForReset()
		{
		}
	}
}
