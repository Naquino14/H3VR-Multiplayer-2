using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class PTargetHitRegistrar : MonoBehaviour, IOnPTargetHit
	{
		public PTargetScoringManager Manager;

		void IOnPTargetHit.OnTargetHit(List<OnHitInfo> bulletHits)
		{
			for (int i = 0; i < bulletHits.Count; i++)
			{
				Manager.ProcessHit(bulletHits[i].score, bulletHits[i].uv);
			}
		}
	}
}
