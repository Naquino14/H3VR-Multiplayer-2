using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class LockToPoint : MonoBehaviour
	{
		public Transform snapTo;

		private Rigidbody body;

		public float snapTime = 2f;

		private float dropTimer;

		private Interactable interactable;

		private void Start()
		{
			interactable = GetComponent<Interactable>();
			body = GetComponent<Rigidbody>();
		}

		private void FixedUpdate()
		{
			bool flag = false;
			if (interactable != null)
			{
				flag = interactable.attachedToHand;
			}
			if (flag)
			{
				body.isKinematic = false;
				dropTimer = -1f;
				return;
			}
			dropTimer += Time.deltaTime / (snapTime / 2f);
			body.isKinematic = dropTimer > 1f;
			if (dropTimer > 1f)
			{
				base.transform.position = snapTo.position;
				base.transform.rotation = snapTo.rotation;
				return;
			}
			float num = Mathf.Pow(35f, dropTimer);
			body.velocity = Vector3.Lerp(body.velocity, Vector3.zero, Time.fixedDeltaTime * 4f);
			if (body.useGravity)
			{
				body.AddForce(-Physics.gravity);
			}
			base.transform.position = Vector3.Lerp(base.transform.position, snapTo.position, Time.fixedDeltaTime * num * 3f);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, snapTo.rotation, Time.fixedDeltaTime * num * 2f);
		}
	}
}
