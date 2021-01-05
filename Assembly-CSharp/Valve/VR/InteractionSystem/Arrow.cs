// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Arrow
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class Arrow : MonoBehaviour
  {
    public ParticleSystem glintParticle;
    public Rigidbody arrowHeadRB;
    public Rigidbody shaftRB;
    public PhysicMaterial targetPhysMaterial;
    private Vector3 prevPosition;
    private Quaternion prevRotation;
    private Vector3 prevVelocity;
    private Vector3 prevHeadPosition;
    public SoundPlayOneshot fireReleaseSound;
    public SoundPlayOneshot airReleaseSound;
    public SoundPlayOneshot hitTargetSound;
    public PlaySound hitGroundSound;
    private bool inFlight;
    private bool released;
    private bool hasSpreadFire;
    private int travelledFrames;
    private GameObject scaleParentObject;

    private void Start() => Physics.IgnoreCollision(this.shaftRB.GetComponent<Collider>(), Player.instance.headCollider);

    private void FixedUpdate()
    {
      if (!this.released || !this.inFlight)
        return;
      this.prevPosition = this.transform.position;
      this.prevRotation = this.transform.rotation;
      this.prevVelocity = this.GetComponent<Rigidbody>().velocity;
      this.prevHeadPosition = this.arrowHeadRB.transform.position;
      ++this.travelledFrames;
    }

    public void ArrowReleased(float inputVelocity)
    {
      this.inFlight = true;
      this.released = true;
      this.airReleaseSound.Play();
      if ((Object) this.glintParticle != (Object) null)
        this.glintParticle.Play();
      if (this.gameObject.GetComponentInChildren<FireSource>().isBurning)
        this.fireReleaseSound.Play();
      foreach (RaycastHit raycastHit in Physics.SphereCastAll(this.transform.position, 0.01f, this.transform.forward, 0.8f, -5, QueryTriggerInteraction.Ignore))
      {
        if ((Object) raycastHit.collider.gameObject != (Object) this.gameObject && (Object) raycastHit.collider.gameObject != (Object) this.arrowHeadRB.gameObject && (Object) raycastHit.collider != (Object) Player.instance.headCollider)
        {
          Object.Destroy((Object) this.gameObject);
          return;
        }
      }
      this.travelledFrames = 0;
      this.prevPosition = this.transform.position;
      this.prevRotation = this.transform.rotation;
      this.prevHeadPosition = this.arrowHeadRB.transform.position;
      this.prevVelocity = this.GetComponent<Rigidbody>().velocity;
      this.SetCollisionMode(CollisionDetectionMode.ContinuousDynamic);
      Object.Destroy((Object) this.gameObject, 30f);
    }

    protected void SetCollisionMode(CollisionDetectionMode newMode, bool force = false)
    {
      Rigidbody[] componentsInChildren = this.GetComponentsInChildren<Rigidbody>();
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        if (!componentsInChildren[index].isKinematic || force)
          componentsInChildren[index].collisionDetectionMode = newMode;
      }
    }

    private void OnCollisionEnter(Collision collision)
    {
      if (!this.inFlight)
        return;
      float sqrMagnitude = this.GetComponent<Rigidbody>().velocity.sqrMagnitude;
      bool flag1 = (Object) this.targetPhysMaterial != (Object) null && (Object) collision.collider.sharedMaterial == (Object) this.targetPhysMaterial && (double) sqrMagnitude > 0.200000002980232;
      bool flag2 = (Object) collision.collider.gameObject.GetComponent<Balloon>() != (Object) null;
      if (this.travelledFrames < 2 && !flag1)
      {
        this.transform.position = this.prevPosition - this.prevVelocity * Time.deltaTime;
        this.transform.rotation = this.prevRotation;
        Vector3 vector3 = Vector3.Reflect(this.arrowHeadRB.velocity, collision.contacts[0].normal);
        this.arrowHeadRB.velocity = vector3 * 0.25f;
        this.shaftRB.velocity = vector3 * 0.25f;
        this.travelledFrames = 0;
      }
      else
      {
        if ((Object) this.glintParticle != (Object) null)
          this.glintParticle.Stop(true);
        if ((double) sqrMagnitude > 0.100000001490116)
          this.hitGroundSound.Play();
        FireSource componentInChildren = this.gameObject.GetComponentInChildren<FireSource>();
        FireSource componentInParent = collision.collider.GetComponentInParent<FireSource>();
        if ((Object) componentInChildren != (Object) null && componentInChildren.isBurning && (Object) componentInParent != (Object) null)
        {
          if (!this.hasSpreadFire)
          {
            collision.collider.gameObject.SendMessageUpwards("FireExposure", (object) this.gameObject, SendMessageOptions.DontRequireReceiver);
            this.hasSpreadFire = true;
          }
        }
        else if ((double) sqrMagnitude > 0.100000001490116 || flag2)
        {
          collision.collider.gameObject.SendMessageUpwards("ApplyDamage", SendMessageOptions.DontRequireReceiver);
          this.gameObject.SendMessage("HasAppliedDamage", SendMessageOptions.DontRequireReceiver);
        }
        if (flag2)
        {
          this.transform.position = this.prevPosition;
          this.transform.rotation = this.prevRotation;
          this.arrowHeadRB.velocity = this.prevVelocity;
          Physics.IgnoreCollision(this.arrowHeadRB.GetComponent<Collider>(), collision.collider);
          Physics.IgnoreCollision(this.shaftRB.GetComponent<Collider>(), collision.collider);
        }
        if (flag1)
          this.StickInTarget(collision, this.travelledFrames < 2);
        if (!(bool) (Object) Player.instance || !((Object) collision.collider == (Object) Player.instance.headCollider))
          return;
        Player.instance.PlayerShotSelf();
      }
    }

    private void StickInTarget(Collision collision, bool bSkipRayCast)
    {
      Vector3 direction = this.prevRotation * Vector3.forward;
      if (!bSkipRayCast)
      {
        RaycastHit[] raycastHitArray = Physics.RaycastAll(this.prevHeadPosition - this.prevVelocity * Time.deltaTime, direction, (float) ((double) this.prevVelocity.magnitude * (double) Time.deltaTime * 2.0));
        bool flag = false;
        for (int index = 0; index < raycastHitArray.Length; ++index)
        {
          if ((Object) raycastHitArray[index].collider == (Object) collision.collider)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          return;
      }
      Object.Destroy((Object) this.glintParticle);
      this.inFlight = false;
      this.SetCollisionMode(CollisionDetectionMode.Discrete, true);
      this.shaftRB.velocity = Vector3.zero;
      this.shaftRB.angularVelocity = Vector3.zero;
      this.shaftRB.isKinematic = true;
      this.shaftRB.useGravity = false;
      this.shaftRB.transform.GetComponent<BoxCollider>().enabled = false;
      this.arrowHeadRB.velocity = Vector3.zero;
      this.arrowHeadRB.angularVelocity = Vector3.zero;
      this.arrowHeadRB.isKinematic = true;
      this.arrowHeadRB.useGravity = false;
      this.arrowHeadRB.transform.GetComponent<BoxCollider>().enabled = false;
      this.hitTargetSound.Play();
      this.scaleParentObject = new GameObject("Arrow Scale Parent");
      Transform transform = collision.collider.transform;
      if (!(bool) (Object) collision.collider.gameObject.GetComponent<ExplosionWobble>() && (bool) (Object) transform.parent)
        transform = transform.parent;
      this.scaleParentObject.transform.parent = transform;
      this.transform.parent = this.scaleParentObject.transform;
      this.transform.rotation = this.prevRotation;
      this.transform.position = this.prevPosition;
      this.transform.position = collision.contacts[0].point - this.transform.forward * (float) (0.75 - ((double) Util.RemapNumberClamped(this.prevVelocity.magnitude, 0.0f, 10f, 0.0f, 0.1f) + (double) Random.Range(0.0f, 0.05f)));
    }

    private void OnDestroy()
    {
      if (!((Object) this.scaleParentObject != (Object) null))
        return;
      Object.Destroy((Object) this.scaleParentObject);
    }
  }
}
