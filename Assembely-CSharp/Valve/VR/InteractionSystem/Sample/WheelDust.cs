using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class WheelDust : MonoBehaviour
	{
		private WheelCollider col;

		public ParticleSystem p;

		public float EmissionMul;

		public float velocityMul = 2f;

		public float maxEmission;

		public float minSlip;

		[HideInInspector]
		public float amt;

		[HideInInspector]
		public Vector3 slip;

		private float emitTimer;

		private void Start()
		{
			col = GetComponent<WheelCollider>();
			StartCoroutine(emitter());
		}

		private void Update()
		{
			slip = Vector3.zero;
			if (col.isGrounded)
			{
				col.GetGroundHit(out var hit);
				slip += Vector3.right * hit.sidewaysSlip;
				slip += Vector3.forward * (0f - hit.forwardSlip);
			}
			amt = slip.magnitude;
		}

		private IEnumerator emitter()
		{
			while (true)
			{
				if (emitTimer < 1f)
				{
					yield return null;
					if (amt > minSlip)
					{
						emitTimer += Mathf.Clamp(EmissionMul * amt, 0.01f, maxEmission);
					}
				}
				else
				{
					emitTimer = 0f;
					DoEmit();
				}
			}
		}

		private void DoEmit()
		{
			p.transform.rotation = Quaternion.LookRotation(base.transform.TransformDirection(slip));
			p.startSpeed = velocityMul * amt;
			p.Emit(1);
		}
	}
}
