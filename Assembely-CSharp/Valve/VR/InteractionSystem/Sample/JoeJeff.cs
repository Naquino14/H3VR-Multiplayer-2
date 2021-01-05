using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class JoeJeff : MonoBehaviour
	{
		public float animationSpeed;

		public float jumpVelocity;

		[SerializeField]
		private float m_MovingTurnSpeed = 360f;

		[SerializeField]
		private float m_StationaryTurnSpeed = 180f;

		public float airControl;

		[Tooltip("The time it takes after landing a jump to slow down")]
		public float frictionTime = 0.2f;

		[SerializeField]
		private float footHeight = 0.1f;

		[SerializeField]
		private float footRadius = 0.03f;

		private RaycastHit footHit;

		private bool isGrounded;

		private float turnAmount;

		private float forwardAmount;

		private float groundedTime;

		private Animator animator;

		private Vector3 input;

		private bool held;

		private Rigidbody rigidbody;

		private Interactable interactable;

		public FireSource fire;

		private float jumpTimer;

		private void Start()
		{
			animator = GetComponent<Animator>();
			rigidbody = GetComponent<Rigidbody>();
			interactable = GetComponent<Interactable>();
			animator.speed = animationSpeed;
		}

		private void Update()
		{
			held = interactable.attachedToHand != null;
			jumpTimer -= Time.deltaTime;
			CheckGrounded();
			rigidbody.freezeRotation = !held;
			if (!held)
			{
				FixRotation();
			}
		}

		private void FixRotation()
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.z = 0f;
			Quaternion b = Quaternion.Euler(eulerAngles);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * (float)((!isGrounded) ? 3 : 20));
		}

		public void OnAnimatorMove()
		{
			if (!(Time.deltaTime > 0f))
			{
				return;
			}
			Vector3 vector = animator.deltaPosition / Time.deltaTime;
			vector = Vector3.ProjectOnPlane(vector, footHit.normal);
			if (isGrounded && jumpTimer < 0f)
			{
				if (groundedTime < frictionTime)
				{
					float num = Mathf.InverseLerp(0f, frictionTime, groundedTime);
					Vector3 vector2 = Vector3.Lerp(rigidbody.velocity, vector, num * Time.deltaTime * 30f);
					vector.x = vector2.x;
					vector.z = vector2.z;
				}
				vector.y += -0.2f;
				rigidbody.velocity = vector;
			}
			else
			{
				rigidbody.velocity += input * Time.deltaTime * airControl;
			}
		}

		public void Move(Vector3 move, bool jump)
		{
			input = move;
			if (move.magnitude > 1f)
			{
				move.Normalize();
			}
			move = base.transform.InverseTransformDirection(move);
			turnAmount = Mathf.Atan2(move.x, move.z);
			forwardAmount = move.z;
			ApplyExtraTurnRotation();
			if (isGrounded)
			{
				HandleGroundedMovement(jump);
			}
			UpdateAnimator(move);
		}

		private void UpdateAnimator(Vector3 move)
		{
			animator.speed = ((!fire.isBurning) ? animationSpeed : (animationSpeed * 2f));
			animator.SetFloat("Forward", (!fire.isBurning) ? forwardAmount : 2f, 0.1f, Time.deltaTime);
			animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
			animator.SetBool("OnGround", isGrounded);
			animator.SetBool("Holding", held);
			if (!isGrounded)
			{
				animator.SetFloat("FallSpeed", Mathf.Abs(rigidbody.velocity.y));
				animator.SetFloat("Jump", rigidbody.velocity.y);
			}
		}

		private void ApplyExtraTurnRotation()
		{
			float num = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, forwardAmount);
			base.transform.Rotate(0f, turnAmount * num * Time.deltaTime, 0f);
		}

		private void CheckGrounded()
		{
			isGrounded = false;
			if ((jumpTimer < 0f) & !held)
			{
				isGrounded = Physics.SphereCast(new Ray(base.transform.position + Vector3.up * footHeight, Vector3.down), footRadius, out footHit, footHeight - footRadius);
				if (Vector2.Distance(new Vector2(base.transform.position.x, base.transform.position.z), new Vector2(footHit.point.x, footHit.point.z)) > footRadius / 2f)
				{
					isGrounded = false;
				}
			}
		}

		private void FixedUpdate()
		{
			groundedTime += Time.fixedDeltaTime;
			if (!isGrounded)
			{
				groundedTime = 0f;
			}
			if (isGrounded & !held)
			{
				Debug.DrawLine(base.transform.position, footHit.point);
				rigidbody.position = new Vector3(rigidbody.position.x, footHit.point.y, rigidbody.position.z);
			}
		}

		private void HandleGroundedMovement(bool jump)
		{
			if (jump && isGrounded)
			{
				Jump();
			}
		}

		public void Jump()
		{
			isGrounded = false;
			jumpTimer = 0.1f;
			animator.applyRootMotion = false;
			rigidbody.position += Vector3.up * 0.03f;
			Vector3 velocity = rigidbody.velocity;
			velocity.y = jumpVelocity;
			rigidbody.velocity = velocity;
		}
	}
}
