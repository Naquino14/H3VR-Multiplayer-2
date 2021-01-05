using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TranslocatorManager : MonoBehaviour
	{
		public List<Translocator> Pads;

		public void Init(ZosigFlagManager m)
		{
			for (int i = 0; i < Pads.Count; i++)
			{
				Pads[i].Init(m);
			}
		}
	}
}
