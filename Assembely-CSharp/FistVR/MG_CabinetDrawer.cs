using UnityEngine;

namespace FistVR
{
	public class MG_CabinetDrawer : FVRInteractiveObject
	{
		[Header("Cabinet Params")]
		public Rigidbody RB;

		private Vector3 m_appliedForce = Vector3.zero;

		public Transform ItemPoint;

		public float XZRangeFromPoint = 0.1f;

		public bool CanBeSpawnedInto = true;

		protected override void Awake()
		{
			base.Awake();
			RB = GetComponent<Rigidbody>();
		}

		public void Init()
		{
			base.transform.localPosition = new Vector3(0f, base.transform.localPosition.y, Mathf.Clamp(Random.Range(-0.2f, 0.3f), 0.2f, 0.5f));
			ItemPoint.localEulerAngles = new Vector3(0f, Random.Range(0f, 360f), -90f);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			m_appliedForce = hand.transform.position - base.transform.position;
			m_appliedForce = Vector3.Project(m_appliedForce, base.transform.forward);
			float num = Mathf.Clamp(m_appliedForce.magnitude * 2f, 0f, 1f);
			num *= num;
			m_appliedForce = Vector3.ClampMagnitude(m_appliedForce, num);
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			m_appliedForce = Vector3.zero;
			base.EndInteraction(hand);
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (base.IsHeld)
			{
				RB.AddForce(m_appliedForce, ForceMode.Impulse);
			}
		}

		public void SpawnIntoCabinet(GameObject go)
		{
			go.transform.position = new Vector3(ItemPoint.position.x + Random.Range(0f - XZRangeFromPoint, XZRangeFromPoint), ItemPoint.position.y, ItemPoint.position.z + Random.Range(0f - XZRangeFromPoint, XZRangeFromPoint));
			go.transform.rotation = ItemPoint.rotation;
			CanBeSpawnedInto = false;
		}

		public void SpawnIntoCabinet(GameObject[] gos)
		{
			for (int i = 0; i < gos.Length; i++)
			{
				gos[i].transform.position = new Vector3(ItemPoint.position.x + Random.Range(0f - XZRangeFromPoint, XZRangeFromPoint), ItemPoint.position.y, ItemPoint.position.z + Random.Range(0f - XZRangeFromPoint, XZRangeFromPoint));
				gos[i].transform.rotation = ItemPoint.rotation;
			}
			CanBeSpawnedInto = false;
		}
	}
}
