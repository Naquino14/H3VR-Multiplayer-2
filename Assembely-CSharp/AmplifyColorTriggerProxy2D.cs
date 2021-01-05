using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[AddComponentMenu("")]
public class AmplifyColorTriggerProxy2D : AmplifyColorTriggerProxyBase
{
	private CircleCollider2D circleCollider;

	private Rigidbody2D rigidBody;

	private void Start()
	{
		circleCollider = GetComponent<CircleCollider2D>();
		circleCollider.radius = 0.01f;
		circleCollider.isTrigger = true;
		rigidBody = GetComponent<Rigidbody2D>();
		rigidBody.gravityScale = 0f;
		rigidBody.isKinematic = true;
	}

	private void LateUpdate()
	{
		base.transform.position = Reference.position;
		base.transform.rotation = Reference.rotation;
	}
}
