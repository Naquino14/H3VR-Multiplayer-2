using UnityEngine;

namespace FistVR
{
	public class IPSCTarget : MonoBehaviour, IFVRDamageable
	{
		public Texture2D MaskTexture;

		private bool HasBeenShot;

		public GameObject[] HitZones;

		public BreachingTargetManager Manager;

		public Transform XYGridOrigin;

		public bool IsAutoResetting;

		private float m_resetTick;

		public float RefireRate = 0.25f;

		public AudioEvent HitSound;

		private void Update()
		{
			if (IsAutoResetting)
			{
				if (m_resetTick > 0f)
				{
					m_resetTick -= Time.deltaTime;
				}
				else if (HasBeenShot)
				{
					ResetState();
				}
			}
		}

		public void Damage(Damage dam)
		{
			if (dam.Class == FistVR.Damage.DamageClass.Projectile && !HasBeenShot)
			{
				Vector3 vector = XYGridOrigin.InverseTransformPoint(dam.point);
				vector.z = 0f;
				vector.x = Mathf.Clamp(vector.x, 0f, 1f);
				vector.y = Mathf.Clamp(vector.y, 0f, 1f);
				int x = Mathf.RoundToInt((float)MaskTexture.width * vector.x);
				int y = Mathf.RoundToInt((float)MaskTexture.width * vector.y);
				Color pixel = MaskTexture.GetPixel(x, y);
				if (pixel.r > 0.5f && pixel.g < 0.5f)
				{
					HasBeenShot = true;
					RegisterHit(0);
					m_resetTick = RefireRate;
				}
				else if (pixel.r > 0.5f && pixel.g > 0.5f)
				{
					HasBeenShot = true;
					RegisterHit(1);
					m_resetTick = RefireRate;
				}
				else if (pixel.g > 0.5f && pixel.r < 0.5f)
				{
					HasBeenShot = true;
					RegisterHit(2);
					m_resetTick = RefireRate;
				}
				else if (pixel.b > 0.5f)
				{
					HasBeenShot = true;
					RegisterHit(3);
					m_resetTick = RefireRate;
				}
			}
		}

		private void ResetState()
		{
			HasBeenShot = false;
			Debug.Log("resetting");
			for (int i = 0; i < HitZones.Length; i++)
			{
				if (HitZones[i] != null && HitZones[i].activeSelf)
				{
					HitZones[i].SetActive(value: false);
				}
			}
		}

		private void RegisterHit(int i)
		{
			HitZones[i].SetActive(value: true);
			Invoke("PlaySound", 0.15f);
			if (Manager != null)
			{
				switch (i)
				{
				case 0:
					Manager.RegisterScore(5);
					break;
				case 1:
					Manager.RegisterScore(4);
					break;
				case 2:
					Manager.RegisterScore(3);
					break;
				case 3:
					Manager.RegisterScore(1);
					break;
				}
			}
		}

		private void PlaySound()
		{
			float num = Vector3.Distance(GM.CurrentPlayerRoot.position, base.transform.position);
			float num2 = Mathf.Lerp(0.4f, 0.2f, num / 100f);
			HitSound.VolumeRange = new Vector2(num2, num2);
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, HitSound, base.transform.position);
		}
	}
}
