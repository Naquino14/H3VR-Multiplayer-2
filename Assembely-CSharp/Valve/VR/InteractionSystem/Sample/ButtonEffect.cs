using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class ButtonEffect : MonoBehaviour
	{
		public void OnButtonDown(Hand fromHand)
		{
			ColorSelf(Color.cyan);
			fromHand.TriggerHapticPulse(1000);
		}

		public void OnButtonUp(Hand fromHand)
		{
			ColorSelf(Color.white);
		}

		private void ColorSelf(Color newColor)
		{
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].material.color = newColor;
			}
		}
	}
}
