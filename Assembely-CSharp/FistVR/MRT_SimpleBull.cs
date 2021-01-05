using System;
using UnityEngine;

namespace FistVR
{
	public class MRT_SimpleBull : MonoBehaviour, IFVRDamageable
	{
		public enum SimpleBullState
		{
			SpawnedWaiting = -1,
			MovingIn,
			RotatingToActivate,
			TickingDown,
			RotatingToHide,
			MovingOut,
			Finished
		}

		[Header("Simple Bull Config")]
		public Texture2D MaskTexture;

		public GameObject[] Shards;

		private Rigidbody[] m_shardRigidbodies;

		public GameObject Spawned_ArriveFX;

		public GameObject Spawned_DestructionFX;

		public float MoveSpeed = 1f;

		public float RotSpeed = 1f;

		public bool ArePointsNegative;

		private float m_moveTick;

		private float m_rotTick;

		private float m_downTick;

		public bool IsSimulHit;

		private ModularRangeSequencer m_sequencer;

		private ModularRangeSequenceDefinition.WaveDefinition m_waveDefinition;

		private ModularRangeSequenceDefinition.TargetMovementStyle m_movementStyle;

		private SimpleBullState BState = SimpleBullState.SpawnedWaiting;

		private Vector3 m_startFacing = Vector3.zero;

		private Vector3 m_endFacing = Vector3.zero;

		private Vector3 m_startPos = Vector3.zero;

		private Vector3 m_endPos = Vector3.zero;

		private float m_timeToShoot = 1f;

		private int m_spawnIndex;

		private Vector3 m_randomPos = Vector3.zero;

		protected bool isActivated;

		protected bool isDestroyed;

		public void Awake()
		{
			m_shardRigidbodies = new Rigidbody[Shards.Length];
			for (int i = 0; i < Shards.Length; i++)
			{
				m_shardRigidbodies[i] = Shards[i].GetComponent<Rigidbody>();
			}
		}

		public void Init(ModularRangeSequencer sequencer, Vector3 startpos, Vector3 endpos, float timeToShoot, Vector3 startFacing, Vector3 endFacing, ModularRangeSequenceDefinition.WaveDefinition wavedef, int spawnIndex)
		{
			m_sequencer = sequencer;
			m_waveDefinition = wavedef;
			m_spawnIndex = spawnIndex;
			m_movementStyle = m_waveDefinition.MovementStyle;
			m_startPos = startpos;
			m_endPos = endpos;
			m_timeToShoot = timeToShoot;
			m_startFacing = startFacing;
			m_endFacing = endFacing;
			base.transform.position = m_startPos;
			base.transform.rotation = Quaternion.LookRotation(m_startFacing);
			m_moveTick = 0f;
			m_rotTick = 0f;
			m_downTick = 0f;
			m_randomPos = new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-1f, 3f), m_endPos.z);
		}

		public void Damage(Damage dam)
		{
		}

		public void Activate()
		{
			isActivated = true;
			BState = SimpleBullState.MovingIn;
		}

		private void Deactivate()
		{
			isActivated = false;
		}

		public void Update()
		{
			switch (BState)
			{
			case SimpleBullState.MovingIn:
				if (m_moveTick < 1f)
				{
					m_moveTick += Time.deltaTime * MoveSpeed;
				}
				else
				{
					m_moveTick = 1f;
					BState = SimpleBullState.RotatingToActivate;
				}
				base.transform.position = Vector3.Lerp(m_startPos, m_endPos, m_moveTick);
				break;
			case SimpleBullState.RotatingToActivate:
				if (m_rotTick < 1f)
				{
					m_rotTick += Time.deltaTime * RotSpeed;
				}
				else
				{
					m_rotTick = 1f;
					if (Spawned_ArriveFX != null)
					{
						UnityEngine.Object.Instantiate(Spawned_ArriveFX, base.transform.position, base.transform.rotation);
					}
					BState = SimpleBullState.TickingDown;
				}
				base.transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(m_startFacing, Vector3.up), Quaternion.LookRotation(m_endFacing, Vector3.up), m_rotTick * m_rotTick);
				break;
			case SimpleBullState.TickingDown:
				if (m_downTick < m_timeToShoot)
				{
					m_downTick += Time.deltaTime;
					TargetMovement(m_downTick, m_timeToShoot);
				}
				else
				{
					m_downTick = m_timeToShoot;
					BState = SimpleBullState.RotatingToHide;
				}
				break;
			case SimpleBullState.RotatingToHide:
				if (m_rotTick > 0f)
				{
					m_rotTick -= Time.deltaTime * RotSpeed;
				}
				else
				{
					m_rotTick = 0f;
					Deactivate();
					BState = SimpleBullState.MovingOut;
				}
				base.transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(m_startFacing, Vector3.up), Quaternion.LookRotation(m_endFacing, Vector3.up), m_rotTick * m_rotTick);
				break;
			case SimpleBullState.MovingOut:
				if (m_moveTick > 0f)
				{
					m_moveTick -= Time.deltaTime * MoveSpeed;
				}
				else
				{
					m_moveTick = 0f;
					BState = SimpleBullState.Finished;
					Destroy();
				}
				base.transform.position = Vector3.Lerp(m_startPos, m_endPos, m_moveTick);
				break;
			}
		}

		private void TargetMovement(float tick, float maxTime)
		{
			float num = 2f;
			float num2 = 1f;
			float x = 0f;
			float y = 0f;
			float z = 0f;
			float num3 = m_spawnIndex % 2;
			float num4 = num3 * 2f - 1f;
			float num5 = 0f;
			float num6 = 0f;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			switch (m_movementStyle)
			{
			case ModularRangeSequenceDefinition.TargetMovementStyle.Static:
				x = m_endPos.x;
				y = m_endPos.y;
				z = m_endPos.z;
				break;
			case ModularRangeSequenceDefinition.TargetMovementStyle.SinusoidX:
				x = Mathf.Sin(tick / maxTime * maxTime * (float)Math.PI * 0.5f) * num4 * num;
				y = m_endPos.y;
				z = m_endPos.z;
				break;
			case ModularRangeSequenceDefinition.TargetMovementStyle.SinusoidY:
				x = m_endPos.x;
				y = Mathf.Sin(tick / maxTime * maxTime * (float)Math.PI * 0.5f) * num4 * num + num2;
				z = m_endPos.z;
				break;
			case ModularRangeSequenceDefinition.TargetMovementStyle.TowardCenter:
				num5 = tick / maxTime * maxTime * (float)Math.PI * 0.5f - (float)Math.PI / 2f;
				num6 = Mathf.Sin(num5) / 2f + 0.5f;
				zero = m_endPos;
				zero2 = new Vector3(0f - m_endPos.x, 0f - m_endPos.y + 2f * num2, m_endPos.z);
				x = Mathf.Lerp(zero.x, zero2.x, num6);
				y = Mathf.Lerp(zero.y, zero2.y, num6);
				z = m_endPos.z;
				break;
			case ModularRangeSequenceDefinition.TargetMovementStyle.WhipX:
				num5 = tick / maxTime * maxTime * (float)Math.PI * 0.5f - (float)Math.PI / 2f;
				num6 = Mathf.Sin(num5) / 2f + 0.5f;
				zero = m_endPos;
				x = Mathf.Lerp(b: new Vector3(0f - m_endPos.x, m_endPos.y, m_endPos.z).x, a: zero.x, t: num6);
				y = m_endPos.y;
				z = m_endPos.z;
				break;
			case ModularRangeSequenceDefinition.TargetMovementStyle.WhipY:
				num5 = tick / maxTime * maxTime * (float)Math.PI * 0.5f - (float)Math.PI / 2f;
				num6 = Mathf.Sin(num5) / 2f + 0.5f;
				zero = m_endPos;
				zero2 = new Vector3(m_endPos.x, 0f - m_endPos.y + 2f * num2, m_endPos.z);
				x = m_endPos.x;
				y = Mathf.Lerp(zero.y, zero2.y, num6);
				z = m_endPos.z;
				break;
			case ModularRangeSequenceDefinition.TargetMovementStyle.WhipZ:
				num5 = tick / maxTime * maxTime * (float)Math.PI * 0.5f - (float)Math.PI / 2f;
				num6 = Mathf.Sin(num5) / 2f + 0.5f;
				zero = m_endPos;
				zero2 = new Vector3(m_endPos.x, m_endPos.y, m_endPos.z + 10f);
				x = m_endPos.x;
				y = m_endPos.y;
				z = Mathf.Lerp(zero.z, zero2.z, num6);
				break;
			case ModularRangeSequenceDefinition.TargetMovementStyle.TowardCenterWhipZ:
				num5 = tick / maxTime * maxTime * (float)Math.PI * 0.5f - (float)Math.PI / 2f;
				num6 = Mathf.Sin(num5) / 2f + 0.5f;
				zero = m_endPos;
				zero2 = new Vector3(0f - m_endPos.x, 0f - m_endPos.y + 2f * num2, m_endPos.z + 10f);
				x = Mathf.Lerp(zero.x, zero2.x, num6);
				y = Mathf.Lerp(zero.y, zero2.y, num6);
				z = Mathf.Lerp(zero.z, zero2.z, num6);
				break;
			case ModularRangeSequenceDefinition.TargetMovementStyle.WhipXWhipZ:
				num5 = tick / maxTime * maxTime * (float)Math.PI * 0.5f - (float)Math.PI / 2f;
				num6 = Mathf.Sin(num5) / 2f + 0.5f;
				zero = m_endPos;
				zero2 = new Vector3(0f - m_endPos.x, m_endPos.y, m_endPos.z + 10f);
				x = Mathf.Lerp(zero.x, zero2.x, num6);
				y = m_endPos.y;
				z = Mathf.Lerp(zero.z, zero2.z, num6);
				break;
			case ModularRangeSequenceDefinition.TargetMovementStyle.WhipYWhipZ:
				num5 = tick / maxTime * maxTime * (float)Math.PI * 0.5f - (float)Math.PI / 2f;
				num6 = Mathf.Sin(num5) / 2f + 0.5f;
				zero = m_endPos;
				zero2 = new Vector3(m_endPos.x, 0f - m_endPos.y + 2f * num2, m_endPos.z + 10f);
				x = m_endPos.x;
				y = Mathf.Lerp(zero.y, zero2.y, num6);
				z = Mathf.Lerp(zero.z, zero2.z, num6);
				break;
			case ModularRangeSequenceDefinition.TargetMovementStyle.RandomDir:
				num5 = tick / maxTime * maxTime * (float)Math.PI * 0.5f - (float)Math.PI / 2f;
				num6 = Mathf.Sin(num5) / 2f + 0.5f;
				zero = m_endPos;
				zero2 = new Vector3(m_randomPos.x, m_randomPos.y, m_endPos.z);
				x = Mathf.Lerp(zero.x, zero2.x, num6);
				y = Mathf.Lerp(zero.y, zero2.y, num6);
				z = m_endPos.z;
				break;
			}
			base.transform.position = new Vector3(x, y, z);
		}

		public void Destroy()
		{
			if (!isDestroyed)
			{
				isDestroyed = true;
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void Destroy(Vector3 point, Vector3 force)
		{
			if (!isDestroyed)
			{
				isDestroyed = true;
				if (Spawned_DestructionFX != null)
				{
					UnityEngine.Object.Instantiate(Spawned_DestructionFX, base.transform.position, base.transform.rotation);
				}
				for (int i = 0; i < Shards.Length; i++)
				{
					Shards[i].gameObject.SetActive(value: true);
					Shards[i].transform.SetParent(null);
					m_shardRigidbodies[i].AddForceAtPosition(force * 6f, point);
					m_shardRigidbodies[i].AddExplosionForce(force.magnitude * 6f, point, 15f, 0.1f);
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
