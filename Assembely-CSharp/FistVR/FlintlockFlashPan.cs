using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FlintlockFlashPan : MonoBehaviour, IFVRDamageable
	{
		public enum FState
		{
			Down,
			Up
		}

		private FlintlockWeapon m_weapon;

		[Header("Frizen")]
		public Transform Frizen;

		public Vector2 FrizenRots = new Vector2(0f, 45f);

		public FState FrizenState = FState.Up;

		[Header("VFX")]
		public GameObject GrainPrefab;

		public Transform GrainSpawnPoint;

		public List<Renderer> GrainPileGeo;

		public ParticleSystem Flash_Fire;

		public ParticleSystem Flash_Smoke;

		public ParticleSystem Shot_Fire;

		public ParticleSystem Shot_Smoke;

		[Header("Audio")]
		public AudioEvent AudEvent_FrizenUp;

		public AudioEvent AudEvent_FrizenDown;

		public AudioEvent AudEvent_FlashpanIgnite;

		public AudioEvent AudEvent_PowderLandOnFlashpan;

		[Header("Barrels")]
		public List<FlintlockBarrel> Barrels;

		private float numGrainsPowderOn;

		private bool m_isIgnited;

		private float timeSinceSpawn;

		public FlintlockWeapon GetWeapon()
		{
			return m_weapon;
		}

		public void SetWeapon(FlintlockWeapon w)
		{
			m_weapon = w;
			for (int i = 0; i < Barrels.Count; i++)
			{
				Barrels[i].SetWeapon(w);
				Barrels[i].SetPan(this);
			}
		}

		public void FlashBlast(int x, int y)
		{
			Shot_Fire.Emit(x);
			Shot_Smoke.Emit(y);
		}

		private void Update()
		{
			if (m_isIgnited)
			{
				numGrainsPowderOn -= Time.deltaTime * 20f;
				numGrainsPowderOn = Mathf.Clamp(numGrainsPowderOn, 0f, numGrainsPowderOn);
				SetGrainPileGeo(Mathf.CeilToInt(numGrainsPowderOn));
				Flash_Fire.Emit(1);
				Flash_Smoke.Emit(2);
				if (numGrainsPowderOn <= 0f)
				{
					SetGrainPileGeo(-1);
					m_isIgnited = false;
					FireBarrels();
				}
			}
			if (timeSinceSpawn < 1f)
			{
				timeSinceSpawn += Time.deltaTime;
			}
			if (FrizenState == FState.Up)
			{
				float num = Vector3.Angle(m_weapon.transform.up, Vector3.up);
				if (num > 85f && timeSinceSpawn > 0.04f && numGrainsPowderOn > 0f)
				{
					numGrainsPowderOn -= 1f;
					numGrainsPowderOn = Mathf.Clamp(numGrainsPowderOn, 0f, numGrainsPowderOn);
					if (numGrainsPowderOn <= 0f)
					{
						SetGrainPileGeo(-1);
					}
					else
					{
						SetGrainPileGeo(Mathf.CeilToInt(numGrainsPowderOn));
					}
					timeSinceSpawn = 0f;
					Object.Instantiate(GrainPrefab, GrainSpawnPoint.position, Random.rotation);
				}
			}
			if (!m_weapon.IsHeld)
			{
			}
		}

		private void FireBarrels()
		{
			for (int i = 0; i < Barrels.Count; i++)
			{
				Barrels[i].Ignite();
			}
		}

		public void HammerHit(FlintlockWeapon.FlintState f, bool Flint)
		{
			if (FrizenState == FState.Down && Flint)
			{
				Ignite();
			}
			SetFrizenUp();
		}

		public void Ignite()
		{
			if (numGrainsPowderOn > 0f)
			{
				m_isIgnited = true;
				m_weapon.PlayAudioAsHandling(AudEvent_FlashpanIgnite, Frizen.position);
			}
		}

		private void SetGrainPileGeo(int g)
		{
			for (int i = 0; i < GrainPileGeo.Count; i++)
			{
				if (i == g)
				{
					GrainPileGeo[i].enabled = true;
				}
				else
				{
					GrainPileGeo[i].enabled = false;
				}
			}
		}

		public void ToggleFrizenState()
		{
			if (FrizenState == FState.Down)
			{
				SetFrizenUp();
			}
			else
			{
				SetFrizenDown();
			}
		}

		private void SetFrizenUp()
		{
			if (FrizenState != FState.Up)
			{
				FrizenState = FState.Up;
				m_weapon.SetAnimatedComponent(Frizen, FrizenRots.y, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
				m_weapon.PlayAudioAsHandling(AudEvent_FrizenUp, Frizen.position);
			}
		}

		private void SetFrizenDown()
		{
			if (FrizenState != 0 && m_weapon.HammerState != 0)
			{
				FrizenState = FState.Down;
				m_weapon.SetAnimatedComponent(Frizen, FrizenRots.x, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
				m_weapon.PlayAudioAsHandling(AudEvent_FrizenDown, Frizen.position);
			}
		}

		private bool IsPanFull()
		{
			if (numGrainsPowderOn > 4f)
			{
				return true;
			}
			return false;
		}

		public float GetPanContents()
		{
			return numGrainsPowderOn;
		}

		public void ClearPan()
		{
			numGrainsPowderOn = 0f;
			SetGrainPileGeo(-1);
		}

		private void AddGrain()
		{
			numGrainsPowderOn += 1f;
			SetGrainPileGeo(Mathf.CeilToInt(numGrainsPowderOn));
			m_weapon.PlayAudioAsHandling(AudEvent_PowderLandOnFlashpan, Frizen.position);
		}

		public void OnTriggerEnter(Collider other)
		{
			if (FrizenState == FState.Down || IsPanFull() || other.attachedRigidbody == null)
			{
				return;
			}
			float num = Vector3.Angle(base.transform.up, Vector3.up);
			if (!(num > 70f))
			{
				GameObject gameObject = other.attachedRigidbody.gameObject;
				if (gameObject.CompareTag("flintlock_powdergrain"))
				{
					AddGrain();
					Object.Destroy(other.gameObject);
				}
			}
		}

		public void Damage(Damage d)
		{
			if (FrizenState == FState.Up && (d.Dam_Thermal > 0f || d.Class == FistVR.Damage.DamageClass.Projectile))
			{
				Ignite();
			}
		}
	}
}
