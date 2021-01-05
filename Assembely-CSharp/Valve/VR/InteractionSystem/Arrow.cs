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

		private void Start()
		{
			Physics.IgnoreCollision(shaftRB.GetComponent<Collider>(), Player.instance.headCollider);
		}

		private void FixedUpdate()
		{
			if (released && inFlight)
			{
				prevPosition = base.transform.position;
				prevRotation = base.transform.rotation;
				prevVelocity = GetComponent<Rigidbody>().velocity;
				prevHeadPosition = arrowHeadRB.transform.position;
				travelledFrames++;
			}
		}

		public void ArrowReleased(float inputVelocity)
		{
			inFlight = true;
			released = true;
			airReleaseSound.Play();
			if (glintParticle != null)
			{
				glintParticle.Play();
			}
			if (base.gameObject.GetComponentInChildren<FireSource>().isBurning)
			{
				fireReleaseSound.Play();
			}
			RaycastHit[] array = Physics.SphereCastAll(base.transform.position, 0.01f, base.transform.forward, 0.8f, -5, QueryTriggerInteraction.Ignore);
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				if (raycastHit.collider.gameObject != base.gameObject && raycastHit.collider.gameObject != arrowHeadRB.gameObject && raycastHit.collider != Player.instance.headCollider)
				{
					Object.Destroy(base.gameObject);
					return;
				}
			}
			travelledFrames = 0;
			prevPosition = base.transform.position;
			prevRotation = base.transform.rotation;
			prevHeadPosition = arrowHeadRB.transform.position;
			prevVelocity = GetComponent<Rigidbody>().velocity;
			SetCollisionMode(CollisionDetectionMode.ContinuousDynamic);
			Object.Destroy(base.gameObject, 30f);
		}

		protected void SetCollisionMode(CollisionDetectionMode newMode, bool force = false)
		{
			Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (!componentsInChildren[i].isKinematic || force)
				{
					componentsInChildren[i].collisionDetectionMode = newMode;
				}
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!inFlight)
			{
				return;
			}
			Rigidbody component = GetComponent<Rigidbody>();
			float sqrMagnitude = component.velocity.sqrMagnitude;
			bool flag = targetPhysMaterial != null && collision.collider.sharedMaterial == targetPhysMaterial && sqrMagnitude > 0.2f;
			bool flag2 = collision.collider.gameObject.GetComponent<Balloon>() != null;
			if (travelledFrames < 2 && !flag)
			{
				base.transform.position = prevPosition - prevVelocity * Time.deltaTime;
				base.transform.rotation = prevRotation;
				Vector3 vector = Vector3.Reflect(arrowHeadRB.velocity, collision.contacts[0].normal);
				arrowHeadRB.velocity = vector * 0.25f;
				shaftRB.velocity = vector * 0.25f;
				travelledFrames = 0;
				return;
			}
			if (glintParticle != null)
			{
				glintParticle.Stop(withChildren: true);
			}
			if (sqrMagnitude > 0.1f)
			{
				hitGroundSound.Play();
			}
			FireSource componentInChildren = base.gameObject.GetComponentInChildren<FireSource>();
			FireSource componentInParent = collision.collider.GetComponentInParent<FireSource>();
			if (componentInChildren != null && componentInChildren.isBurning && componentInParent != null)
			{
				if (!hasSpreadFire)
				{
					collision.collider.gameObject.SendMessageUpwards("FireExposure", base.gameObject, SendMessageOptions.DontRequireReceiver);
					hasSpreadFire = true;
				}
			}
			else if (sqrMagnitude > 0.1f || flag2)
			{
				collision.collider.gameObject.SendMessageUpwards("ApplyDamage", SendMessageOptions.DontRequireReceiver);
				base.gameObject.SendMessage("HasAppliedDamage", SendMessageOptions.DontRequireReceiver);
			}
			if (flag2)
			{
				base.transform.position = prevPosition;
				base.transform.rotation = prevRotation;
				arrowHeadRB.velocity = prevVelocity;
				Physics.IgnoreCollision(arrowHeadRB.GetComponent<Collider>(), collision.collider);
				Physics.IgnoreCollision(shaftRB.GetComponent<Collider>(), collision.collider);
			}
			if (flag)
			{
				StickInTarget(collision, travelledFrames < 2);
			}
			if ((bool)Player.instance && collision.collider == Player.instance.headCollider)
			{
				Player.instance.PlayerShotSelf();
			}
		}

		private void StickInTarget(Collision collision, bool bSkipRayCast)
		{
			Vector3 direction = prevRotation * Vector3.forward;
			if (!bSkipRayCast)
			{
				RaycastHit[] array = Physics.RaycastAll(prevHeadPosition - prevVelocity * Time.deltaTime, direction, prevVelocity.magnitude * Time.deltaTime * 2f);
				bool flag = false;
				foreach (RaycastHit raycastHit in array)
				{
					if (raycastHit.collider == collision.collider)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return;
				}
			}
			Object.Destroy(glintParticle);
			inFlight = false;
			SetCollisionMode(CollisionDetectionMode.Discrete, force: true);
			shaftRB.velocity = Vector3.zero;
			shaftRB.angularVelocity = Vector3.zero;
			shaftRB.isKinematic = true;
			shaftRB.useGravity = false;
			shaftRB.transform.GetComponent<BoxCollider>().enabled = false;
			arrowHeadRB.velocity = Vector3.zero;
			arrowHeadRB.angularVelocity = Vector3.zero;
			arrowHeadRB.isKinematic = true;
			arrowHeadRB.useGravity = false;
			arrowHeadRB.transform.GetComponent<BoxCollider>().enabled = false;
			hitTargetSound.Play();
			scaleParentObject = new GameObject("Arrow Scale Parent");
			Transform transform = collision.collider.transform;
			ExplosionWobble component = collision.collider.gameObject.GetComponent<ExplosionWobble>();
			if (!component && (bool)transform.parent)
			{
				transform = transform.parent;
			}
			scaleParentObject.transform.parent = transform;
			base.transform.parent = scaleParentObject.transform;
			base.transform.rotation = prevRotation;
			base.transform.position = prevPosition;
			base.transform.position = collision.contacts[0].point - base.transform.forward * (0.75f - (Util.RemapNumberClamped(prevVelocity.magnitude, 0f, 10f, 0f, 0.1f) + Random.Range(0f, 0.05f)));
		}

		private void OnDestroy()
		{
			if (scaleParentObject != null)
			{
				Object.Destroy(scaleParentObject);
			}
		}
	}
}
