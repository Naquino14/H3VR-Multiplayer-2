using UnityEngine;

namespace FistVR
{
	public class TNH_Token : MonoBehaviour
	{
		public TNH_Manager M;

		public Transform Bounds;

		public Transform Display;

		private float m_scale = 0.01f;

		private bool canCollect;

		public GameObject PickupEffect;

		private float yRot;

		private bool m_isCollected;

		private void Start()
		{
		}

		private void Update()
		{
			Display.localPosition = new Vector3(0f, Mathf.Sin(Time.time * 2f) * 0.05f, 0f);
			if (canCollect)
			{
				Vector3 position = GM.CurrentPlayerBody.LeftHand.position;
				Vector3 position2 = GM.CurrentPlayerBody.RightHand.position;
				if (IsPointInBounds(position) || IsPointInBounds(position2))
				{
					Collect();
				}
			}
			if (m_scale < 1f)
			{
				m_scale = Mathf.MoveTowards(m_scale, 1f, Time.deltaTime * 3f);
				Display.localScale = new Vector3(m_scale, m_scale, m_scale);
				Display.localEulerAngles = new Vector3(0f, m_scale * 720f, 0f);
			}
			else
			{
				canCollect = true;
				yRot += Time.deltaTime * 90f;
				yRot = Mathf.Repeat(yRot, 360f);
				Display.localEulerAngles = new Vector3(0f, yRot, 0f);
			}
		}

		private void Collect()
		{
			if (!m_isCollected)
			{
				m_isCollected = true;
				M.AddTokens(1, Scorethis: true);
				M.EnqueueTokenLine(1);
				Object.Instantiate(PickupEffect, base.transform.position, base.transform.rotation);
				Object.Destroy(base.gameObject);
			}
		}

		public bool IsPointInBounds(Vector3 p)
		{
			if (TestVolumeBool(Bounds, p))
			{
				return true;
			}
			return false;
		}

		public bool TestVolumeBool(Transform t, Vector3 pos)
		{
			bool result = true;
			Vector3 vector = t.InverseTransformPoint(pos);
			if (Mathf.Abs(vector.x) > 0.5f || Mathf.Abs(vector.y) > 0.5f || Mathf.Abs(vector.z) > 0.5f)
			{
				result = false;
			}
			return result;
		}
	}
}
