using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class Unparent : MonoBehaviour
	{
		private Transform oldParent;

		private void Start()
		{
			oldParent = base.transform.parent;
			base.transform.parent = null;
			base.gameObject.name = oldParent.gameObject.name + "." + base.gameObject.name;
		}

		private void Update()
		{
			if (oldParent == null)
			{
				Object.Destroy(base.gameObject);
			}
		}

		public Transform GetOldParent()
		{
			return oldParent;
		}
	}
}
