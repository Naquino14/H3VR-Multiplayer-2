using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class Tome : MonoBehaviour
	{
		private Vector3 basePos;

		public List<FVRIgnitable> Igniteables;

		public Transform JerrySpawnPoint;

		public GameObject JerrySpawnIn;

		public GameObject JerrySpawnOut;

		public GameObject StaffSpawn;

		public AudioSource Chant;

		private bool m_hasSummoned;

		private float m_tick;

		private bool m_isCircleReady;

		private void Start()
		{
			basePos = base.transform.position;
			GM.CurrentSceneSettings.SosigKillEvent += CheckIfDeadSosigWasMine;
		}

		private void OnDestroy()
		{
			GM.CurrentSceneSettings.SosigKillEvent -= CheckIfDeadSosigWasMine;
		}

		public void CheckIfDeadSosigWasMine(Sosig s)
		{
			if (m_isCircleReady && !m_hasSummoned && !(s == null) && !(s.Links[1] == null) && s.E.IFFCode != 1 && s.E.IFFCode != 2 && (s.GetDiedFromType() == Sosig.SosigDeathType.JointPullApart || s.GetDiedFromType() == Sosig.SosigDeathType.JointSever))
			{
				float num = Vector3.Distance(s.Links[1].transform.position, base.transform.position);
				if (num < 5f)
				{
					Summon();
				}
			}
		}

		private void Summon()
		{
			m_hasSummoned = true;
			UnityEngine.Object.Instantiate(JerrySpawnIn, JerrySpawnPoint.position, JerrySpawnPoint.rotation);
			Invoke("SummonStaff", 5f);
		}

		private void SummonStaff()
		{
			UnityEngine.Object.Instantiate(JerrySpawnOut, JerrySpawnPoint.position, JerrySpawnPoint.rotation);
			UnityEngine.Object.Instantiate(StaffSpawn, JerrySpawnPoint.position, UnityEngine.Random.rotation);
			GM.ZMaster.FlagM.SetFlag("m_ttt_sm", 1);
		}

		private void Update()
		{
			float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position);
			if (num > 45f)
			{
				if (Chant.isPlaying)
				{
					Chant.Stop();
				}
			}
			else if (num < 40f)
			{
				if (!Chant.isPlaying)
				{
					Chant.Play();
				}
				float t = Mathf.InverseLerp(20f, 40f, num);
				float volume = Mathf.Lerp(0.3f, 0f, t);
				Chant.volume = volume;
			}
			m_tick += Time.deltaTime * 0.2f;
			m_tick = Mathf.Repeat(m_tick, (float)Math.PI * 2f);
			base.transform.position = basePos + Mathf.Sin(m_tick) * Vector3.up * 0.1f;
			if (m_isCircleReady)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < Igniteables.Count; i++)
			{
				if (!Igniteables[i].IsOnFire())
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				m_isCircleReady = true;
			}
		}
	}
}
