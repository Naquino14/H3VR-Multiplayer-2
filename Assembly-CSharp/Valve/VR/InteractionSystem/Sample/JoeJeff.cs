// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.JoeJeff
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
      this.animator = this.GetComponent<Animator>();
      this.rigidbody = this.GetComponent<Rigidbody>();
      this.interactable = this.GetComponent<Interactable>();
      this.animator.speed = this.animationSpeed;
    }

    private void Update()
    {
      this.held = (Object) this.interactable.attachedToHand != (Object) null;
      this.jumpTimer -= Time.deltaTime;
      this.CheckGrounded();
      this.rigidbody.freezeRotation = !this.held;
      if (this.held)
        return;
      this.FixRotation();
    }

    private void FixRotation()
    {
      Vector3 eulerAngles = this.transform.eulerAngles;
      eulerAngles.x = 0.0f;
      eulerAngles.z = 0.0f;
      this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(eulerAngles), Time.deltaTime * (!this.isGrounded ? 3f : 20f));
    }

    public void OnAnimatorMove()
    {
      if ((double) Time.deltaTime <= 0.0)
        return;
      Vector3 b = Vector3.ProjectOnPlane(this.animator.deltaPosition / Time.deltaTime, this.footHit.normal);
      if (this.isGrounded && (double) this.jumpTimer < 0.0)
      {
        if ((double) this.groundedTime < (double) this.frictionTime)
        {
          float num = Mathf.InverseLerp(0.0f, this.frictionTime, this.groundedTime);
          Vector3 vector3 = Vector3.Lerp(this.rigidbody.velocity, b, (float) ((double) num * (double) Time.deltaTime * 30.0));
          b.x = vector3.x;
          b.z = vector3.z;
        }
        b.y += -0.2f;
        this.rigidbody.velocity = b;
      }
      else
        this.rigidbody.velocity += this.input * Time.deltaTime * this.airControl;
    }

    public void Move(Vector3 move, bool jump)
    {
      this.input = move;
      if ((double) move.magnitude > 1.0)
        move.Normalize();
      move = this.transform.InverseTransformDirection(move);
      this.turnAmount = Mathf.Atan2(move.x, move.z);
      this.forwardAmount = move.z;
      this.ApplyExtraTurnRotation();
      if (this.isGrounded)
        this.HandleGroundedMovement(jump);
      this.UpdateAnimator(move);
    }

    private void UpdateAnimator(Vector3 move)
    {
      this.animator.speed = !this.fire.isBurning ? this.animationSpeed : this.animationSpeed * 2f;
      this.animator.SetFloat("Forward", !this.fire.isBurning ? this.forwardAmount : 2f, 0.1f, Time.deltaTime);
      this.animator.SetFloat("Turn", this.turnAmount, 0.1f, Time.deltaTime);
      this.animator.SetBool("OnGround", this.isGrounded);
      this.animator.SetBool("Holding", this.held);
      if (this.isGrounded)
        return;
      this.animator.SetFloat("FallSpeed", Mathf.Abs(this.rigidbody.velocity.y));
      this.animator.SetFloat("Jump", this.rigidbody.velocity.y);
    }

    private void ApplyExtraTurnRotation() => this.transform.Rotate(0.0f, this.turnAmount * Mathf.Lerp(this.m_StationaryTurnSpeed, this.m_MovingTurnSpeed, this.forwardAmount) * Time.deltaTime, 0.0f);

    private void CheckGrounded()
    {
      this.isGrounded = false;
      if (!((double) this.jumpTimer < 0.0 & !this.held))
        return;
      this.isGrounded = Physics.SphereCast(new Ray(this.transform.position + Vector3.up * this.footHeight, Vector3.down), this.footRadius, out this.footHit, this.footHeight - this.footRadius);
      if ((double) Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z), new Vector2(this.footHit.point.x, this.footHit.point.z)) <= (double) this.footRadius / 2.0)
        return;
      this.isGrounded = false;
    }

    private void FixedUpdate()
    {
      this.groundedTime += Time.fixedDeltaTime;
      if (!this.isGrounded)
        this.groundedTime = 0.0f;
      if (!(this.isGrounded & !this.held))
        return;
      Debug.DrawLine(this.transform.position, this.footHit.point);
      this.rigidbody.position = new Vector3(this.rigidbody.position.x, this.footHit.point.y, this.rigidbody.position.z);
    }

    private void HandleGroundedMovement(bool jump)
    {
      if (!jump || !this.isGrounded)
        return;
      this.Jump();
    }

    public void Jump()
    {
      this.isGrounded = false;
      this.jumpTimer = 0.1f;
      this.animator.applyRootMotion = false;
      this.rigidbody.position += Vector3.up * 0.03f;
      Vector3 velocity = this.rigidbody.velocity;
      velocity.y = this.jumpVelocity;
      this.rigidbody.velocity = velocity;
    }
  }
}
