using UnityEngine;

namespace FistVR
{
	public class OmniCleric : MonoBehaviour, IFVRDamageable
	{
		private float m_life = 600f;

		private bool m_isDestroyed;

		private float m_headRadius = 0.14f;

		private OmniSpawner_Cleric m_spawner;

		public OmniSpawnDef_Cleric.TargetLocation m_pos;

		private Vector3 m_startPos;

		private Vector3 m_endPos;

		private bool m_isMovingIntoPosition;

		private float m_moveLerp;

		private float m_moveSpeed = 10f;

		public Rigidbody[] Shards_Top;

		public Rigidbody[] Shards_Middle;

		public GameObject[] PFXPrefabs;

		public Transform TR_HeadSpot;

		private bool m_doesFire;

		public Transform ShotRoot;

		public Transform ShotBeam;

		public Renderer BeamRenderer;

		public Color StartColor;

		public Color EndColor;

		private float m_beamTick;

		private float m_beamMax;

		private Vector3 m_beamStart = new Vector3(0.025f, 0.025f, 6f);

		private Vector3 m_beamEnd = new Vector3(0.005f, 0.005f, 6f);

		private bool m_hasFired;

		private bool m_isTickingDown;

		public ParticleSystem MuzzleFire;

		public LayerMask ShotMask;

		private RaycastHit m_shotHit;

		public AudioSource GunShotSound;

		public AudioClip[] GunShotClips;

		public void Init(OmniSpawner_Cleric spawner, Transform spawnPoint, bool doesFire, float FiringTime, OmniSpawnDef_Cleric.TargetLocation PositionIndex)
		{
			m_spawner = spawner;
			m_startPos = spawnPoint.position;
			m_endPos = m_startPos;
			m_endPos.y += 2f;
			m_isMovingIntoPosition = true;
			m_moveLerp = 0f;
			m_doesFire = doesFire;
			m_pos = PositionIndex;
			m_isTickingDown = true;
			m_beamTick = Random.Range(FiringTime, FiringTime * 1.1f);
			m_beamMax = m_beamTick;
		}

		private void Update()
		{
			if (m_isMovingIntoPosition)
			{
				if (m_moveLerp < 1f)
				{
					m_moveLerp += Time.deltaTime * m_moveSpeed;
				}
				else
				{
					m_moveLerp = 1f;
				}
				base.transform.position = Vector3.Lerp(m_startPos, m_endPos, m_moveLerp);
				if (m_moveLerp >= 1f)
				{
					m_isMovingIntoPosition = false;
					ShotBeam.LookAt(GM.CurrentPlayerBody.Torso, Vector3.up);
					ShotBeam.Rotate(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f));
				}
			}
			else
			{
				if (!m_doesFire || !m_isTickingDown)
				{
					return;
				}
				if (m_beamTick > 0f)
				{
					if (!ShotBeam.gameObject.activeSelf)
					{
						ShotBeam.gameObject.SetActive(value: true);
					}
					float t = 1f - m_beamTick / m_beamMax;
					ShotBeam.localScale = Vector3.Lerp(m_beamStart, m_beamEnd, t);
					BeamRenderer.material.SetColor("_Color", Color.Lerp(StartColor, EndColor, t));
					m_beamTick -= Time.deltaTime;
					m_hasFired = false;
				}
				else if (!m_hasFired)
				{
					if (ShotBeam.gameObject.activeSelf)
					{
						ShotBeam.gameObject.SetActive(value: false);
					}
					m_hasFired = true;
					GunShotSound.PlayOneShot(GunShotClips[Random.Range(0, GunShotClips.Length)], 1.2f);
					MuzzleFire.Emit(3);
					FXM.InitiateMuzzleFlash(ShotBeam.position, ShotBeam.forward, 3f, Color.white, 3f);
					if (Physics.Raycast(ShotBeam.position, ShotBeam.forward, out m_shotHit, 6f, ShotMask, QueryTriggerInteraction.Collide) && m_shotHit.collider.gameObject.GetComponent<FVRPlayerHitbox>() != null)
					{
						GM.CurrentPlayerBody.HitEffect();
						m_spawner.AddPoints(-500);
					}
					m_beamTick = m_beamMax;
					ShotBeam.LookAt(GM.CurrentPlayerBody.Torso, Vector3.up);
					ShotBeam.Rotate(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f));
				}
			}
		}

		public void Damage(Damage dam)
		{
			if (!m_isMovingIntoPosition && dam.Class == FistVR.Damage.DamageClass.Projectile)
			{
				bool isHeadShot = false;
				if (Vector3.Distance(dam.point, TR_HeadSpot.transform.position) <= m_headRadius)
				{
					isHeadShot = true;
					m_life = 0f;
				}
				else
				{
					m_life -= dam.Dam_TotalKinetic;
				}
				if (m_life <= 0f)
				{
					Destroy(isHeadShot, dam);
				}
			}
		}

		private void Destroy(bool isHeadShot, Damage dam)
		{
			if (m_isDestroyed)
			{
				return;
			}
			m_isDestroyed = true;
			for (int i = 0; i < PFXPrefabs.Length; i++)
			{
				Object.Instantiate(PFXPrefabs[i], dam.point, Quaternion.identity);
			}
			if (isHeadShot)
			{
				for (int j = 0; j < Shards_Top.Length; j++)
				{
					Shards_Top[j].transform.SetParent(null);
					Shards_Top[j].gameObject.SetActive(value: true);
					Shards_Top[j].AddForceAtPosition(dam.strikeDir * dam.Dam_TotalKinetic * 0.01f, dam.point, ForceMode.Impulse);
					Shards_Top[j].AddExplosionForce(2f, dam.point, 2f, 0.1f, ForceMode.Impulse);
				}
			}
			else
			{
				for (int k = 0; k < Shards_Middle.Length; k++)
				{
					Shards_Middle[k].transform.SetParent(null);
					Shards_Middle[k].gameObject.SetActive(value: true);
					Shards_Middle[k].AddForceAtPosition(dam.strikeDir * dam.Dam_TotalKinetic * 0.01f, dam.point, ForceMode.Impulse);
					Shards_Middle[k].AddExplosionForce(2f, dam.point, 1f, 0.1f, ForceMode.Impulse);
				}
			}
			m_spawner.ClearCleric(this, isHeadShot);
		}
	}
}
