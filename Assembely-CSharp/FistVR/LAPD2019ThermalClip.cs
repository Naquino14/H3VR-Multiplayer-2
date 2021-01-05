using UnityEngine;

namespace FistVR
{
	public class LAPD2019ThermalClip : FVRPhysicalObject
	{
		public float StoredHeat;

		public ParticleSystem PSystem;

		private ParticleSystem.EmissionModule emission;

		public LAPD2019ThermalClipTrigger Trig;

		public GameObject splode;

		public Renderer rend;

		protected override void Awake()
		{
			base.Awake();
			emission = PSystem.emission;
			emission.rateOverTimeMultiplier = 0f;
		}

		public void SetHeat(float h)
		{
			h = Mathf.Clamp(h, 0f, 1f);
			StoredHeat = h;
			emission.rateOverTimeMultiplier = StoredHeat;
		}

		private void OnTriggerEnter(Collider col)
		{
			if (base.QuickbeltSlot == null && col.gameObject.CompareTag("FVRFireArmMagazineReloadTrigger"))
			{
				LAPD2019ThermalClipTrigger component = col.gameObject.GetComponent<LAPD2019ThermalClipTrigger>();
				if (component != null)
				{
					Trig = component;
				}
			}
		}

		private void OnTriggerExit(Collider col)
		{
			if (Trig != null && col.gameObject.CompareTag("FVRFireArmMagazineReloadTrigger"))
			{
				LAPD2019ThermalClipTrigger component = col.gameObject.GetComponent<LAPD2019ThermalClipTrigger>();
				if (component == Trig)
				{
					Trig = null;
				}
			}
		}

		public override void OnCollisionEnter(Collision col)
		{
			base.OnCollisionEnter(col);
			if (!base.IsHeld && base.QuickbeltSlot == null && StoredHeat > 0.2f && col.collider.attachedRigidbody == null && col.relativeVelocity.magnitude > 1f)
			{
				if (splode != null)
				{
					Object.Instantiate(splode, base.transform.position, base.transform.rotation);
				}
				Object.Destroy(base.gameObject);
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			rend.material.SetFloat("_EmissionWeight", Mathf.Clamp(Mathf.Pow(StoredHeat, 1.5f), 0f, 1f));
			if (Trig != null && Trig.Gun.LoadThermalClip(StoredHeat))
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
