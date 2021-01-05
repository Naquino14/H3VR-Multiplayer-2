using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class FVRPanelButton : FVRInteractiveObject
	{
		private Button button;

		public float RefireLimit = 0.12f;

		private float tick;

		protected override void Awake()
		{
			base.Awake();
			button = GetComponent<Button>();
		}

		public override void Poke(FVRViveHand hand)
		{
			if (tick >= RefireLimit)
			{
				tick = 0f;
				base.Poke(hand);
				button.onClick.Invoke();
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (tick < 1f)
			{
				tick += Time.deltaTime;
			}
		}
	}
}
