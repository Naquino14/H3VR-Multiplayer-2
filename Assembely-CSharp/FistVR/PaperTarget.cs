using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class PaperTarget : MonoBehaviour, IFVRDamageable
	{
		public Texture2D MaskTexture;

		public GameObject[] BulletHolePrefabs;

		public Transform XYGridOrigin;

		public List<GameObject> CurrentShots = new List<GameObject>();

		public List<GameObject> CurrentDisplayShots = new List<GameObject>();

		private Vector3 tarPos = Vector3.zero;

		private bool isMoving;

		private float StartPos;

		private float EndPost;

		private float LerpTick;

		private float MoveSpeed = 0.25f;

		private float StoredDistance = 30f;

		public Transform DisplayTarget;

		public Transform DisplayUpperLeft;

		public Transform DisplayUpperRight;

		public Transform DisplayLowerLeft;

		public GameObject DisplayHitObject;

		public GameObject DisplayLastHitObject;

		private GameObject m_lastHitDot;

		public TargetRangePanel MyPanel;

		public List<float> times = new List<float>();

		public void Awake()
		{
			tarPos = base.transform.position;
			StoredDistance = MyPanel.MaxMeters;
		}

		public Vector3 GetCurrentScoring()
		{
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < CurrentShots.Count; i++)
			{
				if (CurrentShots[i] != null)
				{
					zero.x += 1f;
					zero.y += CurrentShots[i].GetComponent<PaperTargetBulletHole>().Score;
				}
			}
			zero.z = StoredDistance;
			return zero;
		}

		public void ClearHoles()
		{
			for (int num = CurrentShots.Count - 1; num >= 0; num--)
			{
				Object.Destroy(CurrentShots[num]);
			}
			CurrentShots.Clear();
			for (int num2 = CurrentDisplayShots.Count - 1; num2 >= 0; num2--)
			{
				Object.Destroy(CurrentDisplayShots[num2]);
			}
			CurrentDisplayShots.Clear();
			Object.Destroy(m_lastHitDot);
			m_lastHitDot = null;
			StoredDistance = MyPanel.MaxMeters;
		}

		public Vector3 ClearHolesAndReportScore()
		{
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < CurrentShots.Count; i++)
			{
				if (CurrentShots[i] != null)
				{
					zero.x += 1f;
					zero.y += CurrentShots[i].GetComponent<PaperTargetBulletHole>().Score;
				}
			}
			zero.z = StoredDistance;
			for (int num = CurrentShots.Count - 1; num >= 0; num--)
			{
				Object.Destroy(CurrentShots[num]);
			}
			CurrentShots.Clear();
			for (int num2 = CurrentDisplayShots.Count - 1; num2 >= 0; num2--)
			{
				Object.Destroy(CurrentDisplayShots[num2]);
			}
			CurrentDisplayShots.Clear();
			Object.Destroy(m_lastHitDot);
			m_lastHitDot = null;
			StoredDistance = 30f;
			return zero;
		}

		public void Update()
		{
			if (isMoving)
			{
				float num = 0f;
				if (LerpTick < 1f)
				{
					LerpTick += Time.deltaTime * MoveSpeed;
				}
				else
				{
					LerpTick = 1f;
					isMoving = false;
				}
				num = Mathf.Lerp(StartPos, EndPost, LerpTick);
				base.transform.position = new Vector3(0f, tarPos.y, num);
			}
		}

		public void GoToDest(int i)
		{
			float desiredDistance = MyPanel.GetDesiredDistance();
			isMoving = true;
			StartPos = base.transform.position.z;
			EndPost = desiredDistance;
			LerpTick = 0f;
		}

		public void ResetDist()
		{
			float endPost = 0f;
			isMoving = true;
			StartPos = base.transform.position.z;
			EndPost = endPost;
			LerpTick = 0f;
		}

		public void Damage(Damage dam)
		{
			if (dam.Class == FistVR.Damage.DamageClass.Projectile)
			{
				times.Add(Time.time);
				Vector3 vector = XYGridOrigin.InverseTransformPoint(dam.point);
				vector.z = 0f;
				vector.x = Mathf.Clamp(vector.x, 0f, 1f);
				vector.y = Mathf.Clamp(vector.y, 0f, 1f);
				int x = Mathf.RoundToInt((float)MaskTexture.width * vector.x);
				int y = Mathf.RoundToInt((float)MaskTexture.width * vector.y);
				int score = Mathf.RoundToInt(MaskTexture.GetPixel(x, y).a * 10f);
				SpawnBulletHole(dam.point, score);
				SpawnDisplayBulletHole(new Vector2(vector.x, vector.y));
				StoredDistance = Mathf.Min(StoredDistance, base.transform.position.z);
				MyPanel.UpdatePaperSheet();
			}
		}

		public void SpawnBulletHole(Vector3 point, int score)
		{
			Vector3 position = point + -base.transform.forward * Random.Range(0.001f, 0.008f);
			GameObject gameObject = Object.Instantiate(BulletHolePrefabs[Random.Range(0, BulletHolePrefabs.Length)], position, Quaternion.LookRotation(-base.transform.forward, Random.onUnitSphere));
			gameObject.transform.SetParent(base.transform);
			gameObject.GetComponent<PaperTargetBulletHole>().Score = score;
			CurrentShots.Add(gameObject);
		}

		public void SpawnDisplayBulletHole(Vector2 coord)
		{
			Vector3 position = Vector3.Lerp(DisplayUpperLeft.position, DisplayUpperRight.position, coord.x);
			position.y = Mathf.Lerp(DisplayUpperLeft.position.y, DisplayLowerLeft.position.y, coord.y);
			position += -DisplayTarget.forward * Random.Range(0.0001f, 0.0003f);
			if (m_lastHitDot == null)
			{
				m_lastHitDot = Object.Instantiate(DisplayLastHitObject, position, Quaternion.LookRotation(-DisplayTarget.forward, Random.onUnitSphere));
				m_lastHitDot.transform.SetParent(DisplayTarget);
				return;
			}
			GameObject gameObject = Object.Instantiate(DisplayHitObject, m_lastHitDot.transform.position, m_lastHitDot.transform.rotation);
			gameObject.transform.SetParent(DisplayTarget);
			CurrentDisplayShots.Add(gameObject);
			m_lastHitDot.transform.position = position;
		}
	}
}
