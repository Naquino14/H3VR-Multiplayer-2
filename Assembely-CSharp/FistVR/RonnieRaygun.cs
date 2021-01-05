using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
	public class RonnieRaygun : MonoBehaviour
	{
		public Transform Gun;

		public Transform GunMuzzle;

		public GameObject Projectile;

		private Vector3 currentTarget;

		public AudioSource GunAudio;

		public AudioClip[] AudClip_Raygun;

		public float AngleThreshold = 40f;

		private float m_RefireTick = 0.2f;

		public Vector2 RefireRange = new Vector2(0.2f, 0.5f);

		public NavMeshAgent Agent;

		public Transform Facing;

		public Vector2 SpeedRange;

		public float AccurateFireDistance = 20f;

		public Vector2 PitchRange = new Vector2(0.95f, 1.05f);

		private float NavTick = 1f;

		private void Start()
		{
			NavTick = Random.Range(0.1f, 5f);
			Agent.enabled = true;
			Agent.speed = Random.Range(SpeedRange.x, SpeedRange.y);
		}

		private void Update()
		{
			currentTarget = GM.CurrentPlayerBody.gameObject.transform.position + Vector3.up * 1.2f;
			if (m_RefireTick <= 0f)
			{
				Vector3 from = currentTarget - Gun.position;
				if (Vector3.Angle(from, Facing.forward) <= AngleThreshold)
				{
					if (Vector3.Distance(currentTarget, base.transform.position) < AccurateFireDistance)
					{
						Gun.transform.LookAt(currentTarget + Random.onUnitSphere * 0.3f, Vector3.up);
						m_RefireTick = Random.Range(RefireRange.x, RefireRange.y);
					}
					else
					{
						Gun.transform.LookAt(Facing.position + Facing.forward * 10f + Facing.up * 10f + Random.onUnitSphere * 4f);
						m_RefireTick = Random.Range(RefireRange.x * 2f, RefireRange.y * 2.5f);
					}
					Fire();
				}
			}
			else
			{
				m_RefireTick -= Time.deltaTime;
			}
			NavUpdate();
		}

		private void NavUpdate()
		{
			if (Vector3.Distance(currentTarget, base.transform.position) < 20f)
			{
				Vector3 forward = currentTarget - Facing.position;
				forward.y = 0f;
				Facing.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
			}
			else
			{
				Facing.transform.localEulerAngles = Vector3.zero;
			}
			if (NavTick <= 0f)
			{
				NavTick = Random.Range(2f, 6f);
				GrabPath();
			}
			else
			{
				NavTick -= Time.deltaTime;
			}
		}

		private void GrabPath()
		{
			Vector3 onUnitSphere = Random.onUnitSphere;
			onUnitSphere.y = 0f;
			onUnitSphere.Normalize();
			Agent.SetDestination(currentTarget + onUnitSphere * 2f);
		}

		private void Fire()
		{
			GameObject gameObject = Object.Instantiate(Projectile, GunMuzzle.position, GunMuzzle.rotation);
			FVRProjectile component = gameObject.GetComponent<FVRProjectile>();
			component.Fire(component.transform.forward, null);
			GunAudio.pitch = Random.Range(PitchRange.x, PitchRange.y);
			GunAudio.PlayOneShot(AudClip_Raygun[Random.Range(0, AudClip_Raygun.Length)], 1f);
			FXM.InitiateMuzzleFlash(GunMuzzle.position, GunMuzzle.forward, 4.5f, Color.red, 2f);
		}
	}
}
