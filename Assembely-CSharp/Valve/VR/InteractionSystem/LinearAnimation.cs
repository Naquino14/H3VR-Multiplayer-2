using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class LinearAnimation : MonoBehaviour
	{
		public LinearMapping linearMapping;

		public Animation animation;

		private AnimationState animState;

		private float animLength;

		private float lastValue;

		private void Awake()
		{
			if (animation == null)
			{
				animation = GetComponent<Animation>();
			}
			if (linearMapping == null)
			{
				linearMapping = GetComponent<LinearMapping>();
			}
			animation.playAutomatically = true;
			animState = animation[animation.clip.name];
			animState.wrapMode = WrapMode.PingPong;
			animState.speed = 0f;
			animLength = animState.length;
		}

		private void Update()
		{
			float value = linearMapping.value;
			if (value != lastValue)
			{
				animState.time = value * animLength;
			}
			lastValue = value;
		}
	}
}
