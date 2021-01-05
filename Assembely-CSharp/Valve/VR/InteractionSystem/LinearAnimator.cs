using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class LinearAnimator : MonoBehaviour
	{
		public LinearMapping linearMapping;

		public Animator animator;

		private float currentLinearMapping = float.NaN;

		private int framesUnchanged;

		private void Awake()
		{
			if (animator == null)
			{
				animator = GetComponent<Animator>();
			}
			animator.speed = 0f;
			if (linearMapping == null)
			{
				linearMapping = GetComponent<LinearMapping>();
			}
		}

		private void Update()
		{
			if (currentLinearMapping != linearMapping.value)
			{
				currentLinearMapping = linearMapping.value;
				animator.enabled = true;
				animator.Play(0, 0, currentLinearMapping);
				framesUnchanged = 0;
			}
			else
			{
				framesUnchanged++;
				if (framesUnchanged > 2)
				{
					animator.enabled = false;
				}
			}
		}
	}
}
