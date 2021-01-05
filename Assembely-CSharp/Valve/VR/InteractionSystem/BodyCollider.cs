using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(CapsuleCollider))]
	public class BodyCollider : MonoBehaviour
	{
		public Transform head;

		private CapsuleCollider capsuleCollider;

		private void Awake()
		{
			capsuleCollider = GetComponent<CapsuleCollider>();
		}

		private void FixedUpdate()
		{
			float num = Vector3.Dot(head.localPosition, Vector3.up);
			capsuleCollider.height = Mathf.Max(capsuleCollider.radius, num);
			base.transform.localPosition = head.localPosition - 0.5f * num * Vector3.up;
		}
	}
}
