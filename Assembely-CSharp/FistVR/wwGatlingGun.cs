using System;
using UnityEngine;

namespace FistVR
{
	public class wwGatlingGun : MonoBehaviour
	{
		[Serializable]
		public class MuzzleFireType
		{
			public Vector2 ShotVolume = new Vector2(0.9f, 1f);

			public ParticleSystem[] MuzzleFires;

			public int[] MuzzleFireAmounts;

			public GameObject ProjectilePrefab;
		}

		public Transform GunBarrels;

		public Transform MuzzlePos;

		private float m_curRot;

		private float m_rotTilShot = 36f;

		public int AmmoType;

		public FVRFirearmAudioSet AudioClipSet;

		public FVRTailSoundClass TailClass = FVRTailSoundClass.FullPower;

		protected SM.AudioSourcePool m_pool_shot;

		protected SM.AudioSourcePool m_pool_tail;

		protected SM.AudioSourcePool m_pool_mechanics;

		public MuzzleFireType[] MuzzleFX;

		public void Awake()
		{
			m_pool_shot = SM.CreatePool(3, 3, FVRPooledAudioType.GunShot);
			m_pool_tail = SM.CreatePool(2, 2, FVRPooledAudioType.GunTail);
			m_pool_mechanics = SM.CreatePool(3, 3, FVRPooledAudioType.GunMech);
		}

		public void CrankGun(float crank)
		{
			float value = crank * 0.4f;
			value = Mathf.Clamp(value, 0f, 2f);
			m_curRot += value;
			if (m_curRot > 180f)
			{
				m_curRot -= 360f;
			}
			GunBarrels.transform.localEulerAngles = new Vector3(0f, 0f, m_curRot);
			m_rotTilShot -= value;
			if (m_rotTilShot <= 0f)
			{
				FireShot();
				m_rotTilShot = 36f;
			}
		}

		private void FireShot()
		{
			MuzzleFireType muzzleFireType = MuzzleFX[AmmoType];
			for (int i = 0; i < muzzleFireType.MuzzleFires.Length; i++)
			{
				muzzleFireType.MuzzleFires[i].Emit(muzzleFireType.MuzzleFireAmounts[i]);
			}
			m_pool_shot.PlayClip(AudioClipSet.Shots_Main, MuzzlePos.position);
			m_pool_mechanics.PlayClip(AudioClipSet.HammerHit, MuzzlePos.position);
			m_pool_tail.PlayClipPitchOverride(SM.GetTailSet(TailClass, GM.CurrentPlayerBody.GetCurrentSoundEnvironment()), MuzzlePos.position, AudioClipSet.TailPitchMod_Main);
			GameObject gameObject = UnityEngine.Object.Instantiate(muzzleFireType.ProjectilePrefab, MuzzlePos.position, MuzzlePos.rotation);
			gameObject.GetComponent<BallisticProjectile>().Fire(MuzzlePos.forward, null);
		}

		public void Update()
		{
		}
	}
}
