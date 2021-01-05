using UnityEngine;

namespace FistVR
{
	public class MaskedTarget : MonoBehaviour, IFVRDamageable
	{
		public Texture2D MaskTexture;

		public string DisplayText;

		public Rigidbody[] Shards;

		private bool isMoving;

		private bool isRotating;

		private bool isDestroyed;

		private float m_speedMult;

		private Vector3 m_startPos;

		private Vector3 m_endPos;

		private Vector3 m_startFacing;

		private Vector3 m_endFacing;

		private Vector3 m_endUpVector;

		private float m_moveTick;

		private float m_rotTick;

		public GameObject ExplosionSound;

		private SequencerV1 Sequencer;

		public void Init(Vector3 startPos, Vector3 endPos, float speedMult, Vector3 startFacing, Vector3 endFacing, SequencerV1 seq)
		{
			Sequencer = seq;
			m_startPos = startPos;
			m_endPos = endPos;
			m_speedMult = speedMult;
			m_startFacing = startFacing;
			m_endFacing = endFacing;
			base.transform.position = startPos;
			base.transform.rotation = Quaternion.LookRotation(startFacing);
			isMoving = true;
		}

		public void Damage(Damage dam)
		{
			if (dam.Class == FistVR.Damage.DamageClass.Projectile)
			{
				Vector3 vector = base.transform.InverseTransformPoint(dam.point);
				vector *= 0.5f;
				vector += new Vector3(0.5f, 0.5f, 0f);
				int x = Mathf.RoundToInt((float)MaskTexture.width * vector.x);
				int y = Mathf.RoundToInt((float)MaskTexture.width * vector.y);
				Color pixel = MaskTexture.GetPixel(x, y);
				Debug.Log("local point" + vector);
				Debug.Log("alpha is:" + pixel.a);
				if (Mathf.RoundToInt(pixel.a * 10f) > 0 && !isDestroyed)
				{
					isDestroyed = true;
					Sequencer.AddTargetPoints(Mathf.RoundToInt(pixel.a * 10f));
					Destroy(dam.point, dam.strikeDir * 5f);
				}
			}
		}

		public void Update()
		{
			if (isMoving)
			{
				if (m_moveTick < 1f)
				{
					m_moveTick += Time.deltaTime * m_speedMult;
				}
				else
				{
					m_moveTick = 1f;
					isMoving = false;
					isRotating = true;
					m_endUpVector = Random.onUnitSphere;
				}
				base.transform.position = Vector3.Lerp(m_startPos, m_endPos, m_moveTick * m_moveTick);
			}
			if (isRotating)
			{
				if (m_rotTick < 1f)
				{
					m_rotTick += Time.deltaTime * m_speedMult;
				}
				else
				{
					m_rotTick = 1f;
					isRotating = false;
				}
				base.transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(m_startFacing, Vector3.up), Quaternion.LookRotation(m_endFacing, m_endUpVector), m_rotTick * m_rotTick);
			}
		}

		public void Destroy(Vector3 point, Vector3 force)
		{
			Object.Instantiate(ExplosionSound, base.transform.position, base.transform.rotation);
			for (int i = 0; i < Shards.Length; i++)
			{
				Shards[i].gameObject.SetActive(value: true);
				Shards[i].transform.SetParent(null);
				Shards[i].AddExplosionForce(4f, point, 15f, 0.25f, ForceMode.Impulse);
			}
			Object.Destroy(base.gameObject);
		}
	}
}
