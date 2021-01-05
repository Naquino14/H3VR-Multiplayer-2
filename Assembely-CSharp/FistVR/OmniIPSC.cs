using UnityEngine;

namespace FistVR
{
	public class OmniIPSC : MonoBehaviour, IFVRDamageable
	{
		public enum TargetState
		{
			Activating,
			Deactivating,
			Activated
		}

		public bool IsNoShoot;

		public Transform SpawnPoint;

		public Texture2D MaskTexture;

		private bool HasBeenShot;

		public GameObject[] HitZones;

		public Transform XYGridOrigin;

		public OmniSpawner_IPSC Spawner;

		private float m_stateTick;

		private float m_timeLeft = 1f;

		private Vector3 m_startPos;

		private Vector3 m_endPos;

		private TargetState m_state;

		public void Configure(OmniSpawner_IPSC spawner, Transform point, Vector2 TimeActivated)
		{
			Spawner = spawner;
			SpawnPoint = point;
			m_startPos = point.position;
			m_startPos.y = -3f;
			m_endPos = point.position;
			m_stateTick = 0f;
			m_timeLeft = Random.Range(TimeActivated.x, TimeActivated.y);
		}

		private void Update()
		{
			switch (m_state)
			{
			case TargetState.Activating:
				if (m_stateTick < 1f)
				{
					m_stateTick += Time.deltaTime * 4f;
				}
				else
				{
					m_stateTick = 1f;
					m_state = TargetState.Activated;
				}
				base.transform.position = Vector3.Lerp(m_startPos, m_endPos, m_stateTick);
				break;
			case TargetState.Activated:
				if (m_timeLeft > 0f)
				{
					m_timeLeft -= Time.deltaTime;
					break;
				}
				m_timeLeft = 0f;
				m_state = TargetState.Deactivating;
				break;
			case TargetState.Deactivating:
				if (m_stateTick > 0f)
				{
					m_stateTick -= Time.deltaTime * 4f;
				}
				else
				{
					m_stateTick = 0f;
					Spawner.TargetDeactivating(this);
				}
				base.transform.position = Vector3.Lerp(m_startPos, m_endPos, m_stateTick);
				break;
			}
		}

		private void Deactivate()
		{
			m_state = TargetState.Deactivating;
		}

		public void Damage(Damage dam)
		{
			if (!HasBeenShot)
			{
				HasBeenShot = true;
				if (IsNoShoot)
				{
					Spawner.Invoke("PlayFailureSound", 0.15f);
				}
				else
				{
					Spawner.Invoke("PlaySuccessSound", 0.15f);
				}
				Invoke("Deactivate", 0.5f);
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
				}
				else if (pixel.r > 0.5f && pixel.g > 0.5f)
				{
					HasBeenShot = true;
					RegisterHit(1);
				}
				else if (pixel.g > 0.5f && pixel.r < 0.5f)
				{
					HasBeenShot = true;
					RegisterHit(2);
				}
				else if (pixel.b > 0.5f)
				{
					HasBeenShot = true;
					RegisterHit(3);
				}
			}
		}

		private void RegisterHit(int i)
		{
			HitZones[i].SetActive(value: true);
			int num = 1;
			if (IsNoShoot)
			{
				num = -1;
			}
			switch (i)
			{
			case 0:
				Spawner.AddPoints(100 * num);
				break;
			case 1:
				Spawner.AddPoints(80 * num);
				break;
			case 2:
				Spawner.AddPoints(60 * num);
				break;
			case 3:
				Spawner.AddPoints(20 * num);
				break;
			}
		}
	}
}
