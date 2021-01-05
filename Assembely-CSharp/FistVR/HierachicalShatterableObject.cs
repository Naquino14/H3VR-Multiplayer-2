using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class HierachicalShatterableObject : FVRShatterableObject
	{
		[Header("Hierachichal elements")]
		[HideInInspector]
		private List<HierachicalShatterableObject> Parents = new List<HierachicalShatterableObject>();

		public HierachicalShatterableObject[] Children;

		public HierachicalShatterableObject Root;

		public HierachicalShatterableObject[] RootsMasterList;

		public override void Awake()
		{
			base.Awake();
			for (int i = 0; i < Children.Length; i++)
			{
				if (!Children[i].Parents.Contains(this))
				{
					Children[i].Parents.Add(this);
				}
			}
		}

		private void DetachAllChildrenIfDisconnected(Vector3 point, Vector3 force, int points)
		{
			for (int i = 0; i < Children.Length; i++)
			{
				if (Children[i] != null)
				{
				}
				if (Children[i] != null)
				{
					Children[i].DetachAllChildrenIfDisconnected(base.transform.position, force * 0.8f, 2);
				}
			}
			GoNonKinematic(point, force);
		}

		public override void Destroy(Vector3 damagePoint, Vector3 damageDir)
		{
			DetachAllChildrenIfDisconnected(damagePoint, damageDir, 5);
			base.Destroy(damagePoint, damageDir);
		}
	}
}
