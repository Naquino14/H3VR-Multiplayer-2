using System.Collections.Generic;

namespace FistVR
{
	public class ZosigQuestDestroyableGroup : ZosigQuestManager
	{
		public List<ZosigDestroyable> Destroyables;

		private ZosigGameManager m;

		public override void Init(ZosigGameManager M)
		{
			m = M;
			for (int i = 0; i < Destroyables.Count; i++)
			{
				Destroyables[i].Init(M);
			}
		}
	}
}
