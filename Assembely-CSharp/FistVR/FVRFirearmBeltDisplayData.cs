using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRFirearmBeltDisplayData : MonoBehaviour
	{
		[Serializable]
		public class BeltPointChain
		{
			public List<Vector3> localPosList = new List<Vector3>();

			public List<Quaternion> localRotList = new List<Quaternion>();

			public void Clear()
			{
				localPosList.Clear();
				localRotList.Clear();
			}

			public void SetInterp(BeltPointChain bpc1, BeltPointChain bpc2, float l)
			{
				for (int i = 0; i < localPosList.Count; i++)
				{
					localPosList[i] = Vector3.Lerp(bpc1.localPosList[i], bpc2.localPosList[i], l);
					localRotList[i] = Quaternion.Slerp(bpc1.localRotList[i], bpc2.localRotList[i], l);
				}
			}

			public void SetInterp(BeltPointChain bpc1, float l)
			{
				for (int i = 0; i < localPosList.Count - 1; i++)
				{
					localPosList[i] = Vector3.Lerp(bpc1.localPosList[i], bpc1.localPosList[i + 1], l);
					localRotList[i] = Quaternion.Slerp(bpc1.localRotList[i], bpc1.localRotList[i + 1], l);
				}
			}
		}

		public FVRFireArm Firearm;

		public FVRObject BeltSegmentPrefab;

		public Transform HiddenInBoxSpot;

		public Transform GrabPoint_Box;

		public Transform GrabPoint_Gun;

		public float DistanceBetweenRounds = 0.1f;

		public float GrabRotLimitToTheRight = 20f;

		public float GrabRotLimitToTheLeft = 20f;

		public BeltPointChain ChainInterpolated01;

		public BeltPointChain ChainInterpolatedInOut;

		public BeltPointChain Chain_In;

		public BeltPointChain Chain_Out;

		public List<Transform> TempBeltChainIn;

		public List<Transform> TempBeltChainOut;

		public List<Transform> ProxyRounds;

		private List<Renderer> m_proxyRends = new List<Renderer>();

		private List<MeshFilter> m_proxyMeshes = new List<MeshFilter>();

		public int BeltCapacity = 15;

		public List<FVRLoadedRound> BeltRounds = new List<FVRLoadedRound>();

		private int m_roundsOnBelt;

		private float m_grabLerp;

		private bool m_isBeltGrabbed;

		private bool m_isStraightAngle;

		public bool InvertBeltSide;

		public AnimationCurve AngleFromUpToInOutLerp;

		private float m_lerpCycle;

		private float m_jitterImpulse;

		private Vector3 m_handPos;

		public bool isBeltGrabbed()
		{
			return m_isBeltGrabbed;
		}

		private void Awake()
		{
			for (int i = 0; i < ProxyRounds.Count; i++)
			{
				m_proxyRends.Add(ProxyRounds[i].gameObject.GetComponent<Renderer>());
				m_proxyMeshes.Add(ProxyRounds[i].gameObject.GetComponent<MeshFilter>());
			}
		}

		public void AddJitter()
		{
			m_jitterImpulse += UnityEngine.Random.Range(-0.2f, 0.4f);
		}

		public void UpdateBelt()
		{
			UpdateBeltData();
			if (!m_isBeltGrabbed)
			{
				UpdateProxyRounds(0);
			}
			if (m_jitterImpulse > 0f)
			{
				m_jitterImpulse -= Time.deltaTime * 1.5f;
			}
			m_jitterImpulse = Mathf.Clamp(m_jitterImpulse, 0f, 1f);
		}

		public FVRFireArmBeltSegment StripBeltSegment(Vector3 handPos)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(BeltSegmentPrefab.GetGameObject(), handPos, Quaternion.LookRotation(Firearm.transform.forward, Firearm.transform.up));
			FVRFireArmBeltSegment component = gameObject.GetComponent<FVRFireArmBeltSegment>();
			for (int i = 0; i < BeltRounds.Count; i++)
			{
				component.RoundList.Add(BeltRounds[i]);
			}
			BeltRounds.Clear();
			m_roundsOnBelt = 0;
			component.UpdateBulletDisplay();
			Firearm.HasBelt = false;
			Firearm.ConnectedToBox = false;
			m_isBeltGrabbed = false;
			m_isStraightAngle = false;
			Firearm.PlayAudioEvent(FirearmAudioEventType.BeltGrab);
			return component;
		}

		public void MountBeltSegment(FVRFireArmBeltSegment segment)
		{
			for (int i = 0; i < segment.RoundList.Count; i++)
			{
				BeltRounds.Add(segment.RoundList[i]);
			}
			m_roundsOnBelt = BeltRounds.Count;
			Firearm.PlayAudioEvent(FirearmAudioEventType.BeltRelease);
			Firearm.HasBelt = true;
			Firearm.ConnectedToBox = false;
			m_isBeltGrabbed = false;
			m_isStraightAngle = false;
		}

		public void UpdateProxyRounds(int offset)
		{
			UpdateBeltData();
			for (int i = 0; i < offset; i++)
			{
				if (i < ProxyRounds.Count && ProxyRounds[i].gameObject.activeSelf)
				{
					ProxyRounds[i].gameObject.SetActive(value: false);
				}
			}
			int num = 0;
			for (int j = offset; j < m_roundsOnBelt + offset; j++)
			{
				if (j < ProxyRounds.Count)
				{
					if (!ProxyRounds[j].gameObject.activeSelf)
					{
						ProxyRounds[j].gameObject.SetActive(value: true);
					}
					ProxyRounds[j].localPosition = ChainInterpolated01.localPosList[j];
					if (m_isStraightAngle && m_isBeltGrabbed)
					{
						ProxyRounds[j].rotation = ChainInterpolated01.localRotList[j];
					}
					else
					{
						ProxyRounds[j].localRotation = ChainInterpolated01.localRotList[j];
					}
					m_proxyRends[j].material = BeltRounds[num].LR_Material;
					m_proxyMeshes[j].mesh = BeltRounds[num].LR_Mesh;
					num++;
				}
			}
			for (int k = m_roundsOnBelt + offset; k < ProxyRounds.Count; k++)
			{
				ProxyRounds[k].gameObject.SetActive(value: false);
			}
		}

		public bool HasARound()
		{
			if (Firearm.UsesTopCover && Firearm.IsTopCoverUp)
			{
				return false;
			}
			if (m_roundsOnBelt > 0)
			{
				return true;
			}
			return false;
		}

		public GameObject RemoveRound(bool b)
		{
			GameObject gameObject = BeltRounds[0].LR_ObjectWrapper.GetGameObject();
			if (!GM.CurrentPlayerBody.IsInfiniteAmmo && m_roundsOnBelt > 0)
			{
				BeltRounds.RemoveAt(0);
				m_roundsOnBelt--;
			}
			PullPushBelt(Firearm.Magazine, BeltCapacity);
			if (m_roundsOnBelt <= 0)
			{
				Firearm.HasBelt = false;
			}
			return gameObject;
		}

		public void BeltGrabbed(FVRFireArmMagazine mag, FVRViveHand hand)
		{
			m_isBeltGrabbed = true;
			Firearm.PlayAudioEvent(FirearmAudioEventType.BeltGrab);
		}

		public void BeltGrabUpdate(FVRFireArmMagazine mag, FVRViveHand hand)
		{
			if (m_isBeltGrabbed)
			{
				m_handPos = hand.Input.Pos;
				int num = 0;
				if (m_isStraightAngle)
				{
					Vector3 vector = m_handPos - GrabPoint_Box.position;
					num = (int)(Vector3.ProjectOnPlane(vector, Firearm.transform.forward).magnitude / DistanceBetweenRounds) + 2;
					num = Mathf.Clamp(num, 0, BeltCapacity);
				}
				else
				{
					m_handPos = hand.Input.Pos;
					m_grabLerp = GetGrabPointLerp(m_handPos);
					num = (int)Mathf.Lerp(0f, BeltCapacity, m_grabLerp);
					num = Mathf.Clamp(num, 0, BeltCapacity);
				}
				PullPushBelt(mag, num);
			}
		}

		public void BeltReleased(FVRFireArmMagazine mag, FVRViveHand hand)
		{
			m_isBeltGrabbed = false;
			m_isStraightAngle = false;
			bool flag = false;
			Vector3 closestValidPoint = Firearm.GetClosestValidPoint(GrabPoint_Box.position, GrabPoint_Gun.position, hand.Input.Pos);
			bool flag2 = true;
			if (Firearm is OpenBoltReceiver)
			{
				float boltLerpBetweenLockAndFore = (Firearm as OpenBoltReceiver).Bolt.GetBoltLerpBetweenLockAndFore();
				if (boltLerpBetweenLockAndFore > 0.005f && Firearm.RequiresBoltBackToSeatBelt)
				{
					flag2 = false;
				}
			}
			if (!Firearm.HasBelt && (Firearm.IsTopCoverUp || !Firearm.UsesTopCover) && Vector3.Distance(closestValidPoint, GrabPoint_Gun.position) < 0.02f && flag2)
			{
				flag = true;
				PullPushBelt(mag, BeltCapacity);
			}
			if (flag)
			{
				Firearm.PlayAudioEvent(FirearmAudioEventType.BeltSeat);
				Firearm.ConnectedToBox = true;
				Firearm.HasBelt = true;
			}
			else
			{
				Firearm.PlayAudioEvent(FirearmAudioEventType.BeltRelease);
				Firearm.ConnectedToBox = false;
				Firearm.HasBelt = false;
				if (mag != null)
				{
					PullPushBelt(mag, 0);
				}
			}
			UpdateProxyRounds(0);
		}

		public void ForceRelease()
		{
			Firearm.PlayAudioEvent(FirearmAudioEventType.BeltRelease);
			Firearm.ConnectedToBox = false;
			Firearm.HasBelt = false;
			if (Firearm.Magazine != null)
			{
				PullPushBelt(Firearm.Magazine, 0);
			}
		}

		public void PullPushBelt(FVRFireArmMagazine mag, int desiredNumber)
		{
			if (mag == null)
			{
				UpdateProxyRounds(BeltCapacity - desiredNumber);
				return;
			}
			if (desiredNumber > m_roundsOnBelt)
			{
				int num = desiredNumber - m_roundsOnBelt;
				for (int i = 0; i < num; i++)
				{
					if ((Firearm.ConnectedToBox || isBeltGrabbed()) && mag.HasARound())
					{
						FVRLoadedRound item = mag.RemoveRound(0);
						BeltRounds.Add(item);
					}
					else
					{
						Firearm.ConnectedToBox = false;
					}
				}
			}
			else if (desiredNumber < m_roundsOnBelt)
			{
				int num2 = m_roundsOnBelt - desiredNumber;
				for (int j = 0; j < num2; j++)
				{
					if (BeltRounds.Count > 0)
					{
						FVRLoadedRound fVRLoadedRound = BeltRounds[BeltRounds.Count - 1];
						mag.AddRound(fVRLoadedRound.LR_Class, makeSound: false, updateDisplay: false);
						BeltRounds.RemoveAt(BeltRounds.Count - 1);
					}
				}
			}
			m_roundsOnBelt = BeltRounds.Count;
			if (m_isStraightAngle)
			{
				UpdateProxyRounds(0);
			}
			else
			{
				UpdateProxyRounds(BeltCapacity - desiredNumber);
			}
		}

		public void UpdateBeltData()
		{
			m_isStraightAngle = false;
			Vector3 vector = m_handPos - GrabPoint_Box.position;
			Vector3 vector2 = Vector3.ProjectOnPlane(vector, Firearm.transform.forward);
			Vector3 to = base.transform.right;
			if (InvertBeltSide)
			{
				to = -base.transform.right;
			}
			if (Vector3.Angle(vector2, to) < 90f)
			{
				if (Vector3.Angle(vector2, base.transform.up) < GrabRotLimitToTheRight)
				{
					m_isStraightAngle = true;
				}
			}
			else if (Vector3.Angle(vector2, base.transform.up) < GrabRotLimitToTheLeft)
			{
				m_isStraightAngle = true;
			}
			else
			{
				vector2 = Vector3.RotateTowards(base.transform.up, vector2, GrabRotLimitToTheLeft * 0.0174533f, 1f);
				m_isStraightAngle = true;
			}
			if (m_isBeltGrabbed && m_isStraightAngle)
			{
				vector2 = Vector3.ClampMagnitude(vector2, 0.001f + Mathf.Clamp(DistanceBetweenRounds * ((float)BeltCapacity - 1f), 0f, 1f));
				Vector3 vector3 = vector2;
				vector2 = base.transform.InverseTransformDirection(vector2);
				Vector3 vector4 = -vector2.normalized * DistanceBetweenRounds;
				Vector3 vector5 = GrabPoint_Box.localPosition + vector2;
				Quaternion value = Quaternion.LookRotation(base.transform.forward, -vector3);
				for (int i = 0; i < ChainInterpolated01.localPosList.Count; i++)
				{
					ChainInterpolated01.localPosList[i] = vector5 + vector4 * i;
					ChainInterpolated01.localRotList[i] = value;
				}
				return;
			}
			Vector3 normalized = (base.transform.up - base.transform.right).normalized;
			float num = Vector3.Angle(normalized, Vector3.up);
			float num2 = 0f;
			if (Firearm.IsHeld)
			{
				num2 = Vector3.Dot(Firearm.m_hand.Input.VelLinearWorld.normalized, normalized);
				num2 = num2 * Firearm.m_hand.Input.VelLinearWorld.magnitude * 4f;
			}
			num -= num2;
			float num3 = AngleFromUpToInOutLerp.Evaluate(num);
			ChainInterpolatedInOut.SetInterp(Chain_In, Chain_Out, num3 + m_jitterImpulse);
			if (Firearm is OpenBoltReceiver)
			{
				OpenBoltReceiver openBoltReceiver = Firearm as OpenBoltReceiver;
				bool flag = true;
				if (Firearm.UsesTopCover && Firearm.IsTopCoverUp)
				{
					flag = false;
				}
				if (openBoltReceiver.HasExtractedRound() || !flag)
				{
					m_lerpCycle = 1f;
				}
				else
				{
					m_lerpCycle = openBoltReceiver.Bolt.GetBoltLerpBetweenLockAndFore();
				}
			}
			else if (Firearm is ClosedBoltWeapon)
			{
				ClosedBoltWeapon closedBoltWeapon = Firearm as ClosedBoltWeapon;
				bool flag2 = true;
				if (Firearm.UsesTopCover && Firearm.IsTopCoverUp)
				{
					flag2 = false;
				}
				if (closedBoltWeapon.HasExtractedRound() || !flag2)
				{
					m_lerpCycle = 1f;
				}
				else
				{
					m_lerpCycle = closedBoltWeapon.Bolt.GetBoltLerpBetweenLockAndFore();
				}
			}
			ChainInterpolated01.SetInterp(ChainInterpolatedInOut, m_lerpCycle);
		}

		public float GetGrabPointLerp(Vector3 point)
		{
			Vector3 closestValidPoint = Firearm.GetClosestValidPoint(GrabPoint_Box.position, GrabPoint_Gun.position, point);
			float num = Vector3.Distance(GrabPoint_Box.position, GrabPoint_Gun.position);
			float num2 = Vector3.Distance(closestValidPoint, GrabPoint_Box.position);
			return num2 / num;
		}

		[ContextMenu("SaveChainsToDisplayData")]
		public void SaveChainsToDisplayData()
		{
			Chain_In.Clear();
			Chain_Out.Clear();
			ChainInterpolatedInOut.Clear();
			ChainInterpolated01.Clear();
			for (int i = 0; i < TempBeltChainIn.Count; i++)
			{
				Chain_In.localPosList.Add(TempBeltChainIn[i].localPosition);
				Chain_In.localRotList.Add(TempBeltChainIn[i].rotation);
				ChainInterpolatedInOut.localPosList.Add(TempBeltChainIn[i].localPosition);
				ChainInterpolatedInOut.localRotList.Add(TempBeltChainIn[i].rotation);
				ChainInterpolated01.localPosList.Add(TempBeltChainIn[i].localPosition);
				ChainInterpolated01.localRotList.Add(TempBeltChainIn[i].rotation);
			}
			for (int j = 0; j < TempBeltChainOut.Count; j++)
			{
				Chain_Out.localPosList.Add(TempBeltChainOut[j].localPosition);
				Chain_Out.localRotList.Add(TempBeltChainOut[j].rotation);
			}
		}
	}
}
