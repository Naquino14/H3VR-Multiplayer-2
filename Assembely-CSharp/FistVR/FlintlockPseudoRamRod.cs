using UnityEngine;

namespace FistVR
{
	public class FlintlockPseudoRamRod : FVRInteractiveObject
	{
		public enum RamRodState
		{
			Lower,
			Barrel
		}

		public Transform Root;

		public RamRodState RState;

		public Transform Point_Lower_Rear;

		public Transform Point_Lower_Forward;

		private float lastHandZ;

		private float m_ramZ;

		private float m_minZ_lower;

		private float m_maxZ_lower;

		public GameObject RamRodPrefab;

		private float m_minZ_barrel;

		private float m_maxZ_barrel;

		private FlintlockBarrel m_curBarrel;

		[Header("Audio")]
		public AudioEvent AudEvent_Grab;

		public AudioEvent AudEvent_ExtractHolder;

		public AudioEvent AudEvent_ExtractBarrel;

		public AudioEvent AudEvent_InsertHolder;

		public AudioEvent AudEvent_InsertBarrel;

		private float m_curHandRodOffsetZ;

		public FlintlockBarrel GetCurBarrel()
		{
			return m_curBarrel;
		}

		protected override void Awake()
		{
			base.Awake();
			m_ramZ = base.transform.localPosition.z;
			m_minZ_lower = Point_Lower_Rear.localPosition.z;
			m_maxZ_lower = Point_Lower_Forward.localPosition.z;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			Vector3 zero = Vector3.zero;
			zero = ((RState != 0) ? m_curBarrel.Muzzle.InverseTransformPoint(hand.Input.Pos) : Root.InverseTransformPoint(hand.Input.Pos));
			SM.PlayGenericSound(AudEvent_Grab, base.transform.position);
			lastHandZ = zero.z;
			m_curHandRodOffsetZ = base.transform.InverseTransformPoint(hand.Input.Pos).z;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 zero = Vector3.zero;
			float num = 0f;
			float z = ((RState != 0) ? m_curBarrel.Muzzle.InverseTransformPoint(hand.Input.Pos) : Root.InverseTransformPoint(hand.Input.Pos)).z;
			float num2 = z - lastHandZ;
			num2 = base.transform.InverseTransformPoint(hand.Input.Pos).z - m_curHandRodOffsetZ;
			MoveRamRod(num2, hand);
			lastHandZ = z;
		}

		public void MountToUnder(FVRViveHand h)
		{
			base.transform.SetParent(Root);
			RState = RamRodState.Lower;
			m_ramZ = m_maxZ_lower - 0.002f;
			SM.PlayGenericSound(AudEvent_InsertHolder, base.transform.position);
			base.transform.localPosition = new Vector3(Point_Lower_Rear.localPosition.x, Point_Lower_Rear.localPosition.y, m_ramZ);
			m_curHandRodOffsetZ = base.transform.InverseTransformPoint(h.Input.Pos).z;
		}

		public void MountToBarrel(FlintlockBarrel b, FVRViveHand h)
		{
			if (b == null)
			{
				m_curBarrel = null;
				base.gameObject.SetActive(value: false);
				return;
			}
			base.transform.SetParent(b.Muzzle);
			m_maxZ_barrel = 0.01f;
			m_minZ_barrel = 0f - b.BarrelLength;
			m_curBarrel = b;
			m_ramZ = -0.02f;
			SM.PlayGenericSound(AudEvent_InsertBarrel, base.transform.position);
			base.transform.localPosition = new Vector3(Point_Lower_Rear.localPosition.x, Point_Lower_Rear.localPosition.y, m_ramZ);
			if (h != null)
			{
				m_curHandRodOffsetZ = base.transform.InverseTransformPoint(h.Input.Pos).z;
			}
		}

		private void MoveRamRod(float delta, FVRViveHand hand)
		{
			if (RState == RamRodState.Lower)
			{
				float ramZ = m_ramZ;
				m_ramZ += delta;
				m_ramZ = Mathf.Clamp(m_ramZ, m_minZ_lower, m_ramZ);
				float num = m_ramZ - ramZ;
				base.transform.localPosition = new Vector3(Point_Lower_Rear.localPosition.x, Point_Lower_Rear.localPosition.y, m_ramZ);
				if (m_ramZ >= m_maxZ_lower)
				{
					GameObject gameObject = Object.Instantiate(RamRodPrefab, base.transform.position, base.transform.rotation);
					ForceBreakInteraction();
					FlintlockRamRod component = gameObject.GetComponent<FlintlockRamRod>();
					hand.ForceSetInteractable(component);
					component.BeginInteraction(hand);
					SM.PlayGenericSound(AudEvent_ExtractHolder, base.transform.position);
					Hide();
				}
				return;
			}
			m_ramZ += delta;
			float num2 = Mathf.Clamp(m_ramZ, 0f - m_curBarrel.GetMaxDepth(), m_ramZ);
			base.transform.localPosition = new Vector3(0f, 0f, num2);
			if (m_ramZ < num2)
			{
				m_curBarrel.Tamp((m_ramZ - num2) * 1f, m_ramZ);
			}
			if (m_ramZ >= 0f)
			{
				GameObject gameObject2 = Object.Instantiate(RamRodPrefab, base.transform.position, base.transform.rotation);
				ForceBreakInteraction();
				FlintlockRamRod component2 = gameObject2.GetComponent<FlintlockRamRod>();
				hand.ForceSetInteractable(component2);
				component2.BeginInteraction(hand);
				SM.PlayGenericSound(AudEvent_ExtractBarrel, base.transform.position);
				m_curBarrel = null;
				Hide();
			}
			m_ramZ = num2;
		}

		private void Hide()
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
