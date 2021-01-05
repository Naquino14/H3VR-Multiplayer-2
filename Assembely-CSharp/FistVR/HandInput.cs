using System;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public struct HandInput
	{
		public FVRViveHand Hand;

		public bool TriggerUp;

		public bool TriggerDown;

		public bool TriggerPressed;

		public float TriggerFloat;

		public bool TriggerTouchUp;

		public bool TriggerTouchDown;

		public bool TriggerTouched;

		public bool GripUp;

		public bool GripDown;

		public bool GripPressed;

		public bool GripTouchUp;

		public bool GripTouchDown;

		public bool GripTouched;

		public bool TouchpadUp;

		public bool TouchpadDown;

		public bool TouchpadPressed;

		public bool TouchpadTouchUp;

		public bool TouchpadTouchDown;

		public bool TouchpadTouched;

		public Vector2 TouchpadAxes;

		public bool TouchpadNorthUp;

		public bool TouchpadNorthDown;

		public bool TouchpadNorthPressed;

		public bool TouchpadSouthUp;

		public bool TouchpadSouthDown;

		public bool TouchpadSouthPressed;

		public bool TouchpadWestUp;

		public bool TouchpadWestDown;

		public bool TouchpadWestPressed;

		public bool TouchpadEastUp;

		public bool TouchpadEastDown;

		public bool TouchpadEastPressed;

		public bool TouchpadCenterUp;

		public bool TouchpadCenterDown;

		public bool TouchpadCenterPressed;

		public bool BYButtonUp;

		public bool BYButtonDown;

		public bool BYButtonPressed;

		public bool AXButtonUp;

		public bool AXButtonDown;

		public bool AXButtonPressed;

		public bool Secondary2AxisInputUp;

		public bool Secondary2AxisInputDown;

		public bool Secondary2AxisInputPressed;

		public bool Secondary2AxisInputTouchUp;

		public bool Secondary2AxisInputTouchDown;

		public bool Secondary2AxisInputTouched;

		public Vector2 Secondary2AxisInputAxes;

		public bool Secondary2AxisNorthUp;

		public bool Secondary2AxisNorthDown;

		public bool Secondary2AxisNorthPressed;

		public bool Secondary2AxisSouthUp;

		public bool Secondary2AxisSouthDown;

		public bool Secondary2AxisSouthPressed;

		public bool Secondary2AxisWestUp;

		public bool Secondary2AxisWestDown;

		public bool Secondary2AxisWestPressed;

		public bool Secondary2AxisEastUp;

		public bool Secondary2AxisEastDown;

		public bool Secondary2AxisEastPressed;

		public bool Secondary2AxisCenterUp;

		public bool Secondary2AxisCenterDown;

		public bool Secondary2AxisCenterPressed;

		public float FingerCurl_Thumb;

		public float FingerCurl_Index;

		public float FingerCurl_Middle;

		public float FingerCurl_Ring;

		public float FingerCurl_Pinky;

		public float LastCurlAverage;

		private Vector3 m_pos;

		private Quaternion m_rot;

		private Vector3 m_palmpos;

		private Quaternion m_palmrot;

		public Vector3 FilteredPos;

		public Quaternion FilteredRot;

		public Vector3 FilteredPalmPos;

		public Quaternion FilteredPalmRot;

		public Vector3 Forward;

		public Vector3 FilteredForward;

		public Vector3 Up;

		public Vector3 FilteredUp;

		public Vector3 Right;

		public Vector3 FilteredRight;

		public Vector3 VelLinearLocal;

		public Vector3 VelAngularLocal;

		public Vector3 VelLinearWorld;

		public Vector3 VelAngularWorld;

		public bool IsGrabUp;

		public bool IsGrabDown;

		public bool IsGrabbing;

		private OneEuroFilter<Vector3> positionFilter;

		private OneEuroFilter<Quaternion> rotationFilter;

		private OneEuroFilter<Vector3> positionFilterPalm;

		private OneEuroFilter<Quaternion> rotationFilterPalm;

		public Vector3 OneEuroPosition;

		public Quaternion OneEuroRotation;

		public Vector3 OneEuroPalmPosition;

		public Quaternion OneEuroPalmRotation;

		public Vector3 LastPalmPos1;

		public Vector3 LastPalmPos2;

		public Vector3 Pos
		{
			get
			{
				m_pos = Hand.PoseOverride.position;
				return m_pos;
			}
			set
			{
				m_pos = value;
			}
		}

		public Quaternion Rot
		{
			get
			{
				m_rot = Hand.PoseOverride.rotation;
				return m_rot;
			}
			set
			{
				m_rot = value;
			}
		}

		public Vector3 PalmPos
		{
			get
			{
				m_palmpos = Hand.PalmTransform.position;
				return m_palmpos;
			}
			set
			{
				m_palmpos = value;
			}
		}

		public Quaternion PalmRot
		{
			get
			{
				m_palmrot = Hand.PalmTransform.rotation;
				return m_palmrot;
			}
			set
			{
				m_palmrot = value;
			}
		}

		public void Init()
		{
			positionFilter = new OneEuroFilter<Vector3>(20f);
			rotationFilter = new OneEuroFilter<Quaternion>(20f);
			positionFilterPalm = new OneEuroFilter<Vector3>(20f);
			rotationFilterPalm = new OneEuroFilter<Quaternion>(20f);
		}

		public void UpdateEuroFilter()
		{
			positionFilter.UpdateParams(20f, 1f, 0.007f);
			rotationFilter.UpdateParams(20f, 1f, 0.007f);
			positionFilterPalm.UpdateParams(20f, 1f, 0.007f);
			rotationFilterPalm.UpdateParams(20f, 1f, 0.007f);
			Vector3 value = GM.CurrentPlayerBody.transform.InverseTransformPoint(Hand.Input.Pos);
			OneEuroPosition = positionFilter.Filter(value);
			OneEuroPosition = GM.CurrentPlayerBody.transform.TransformPoint(OneEuroPosition);
			Vector3 value2 = GM.CurrentPlayerBody.transform.InverseTransformPoint(Hand.PalmTransform.position);
			OneEuroPalmPosition = positionFilterPalm.Filter(value2);
			OneEuroPalmPosition = GM.CurrentPlayerBody.transform.TransformPoint(OneEuroPalmPosition);
			OneEuroRotation = rotationFilter.Filter(Rot);
			OneEuroPalmRotation = rotationFilterPalm.Filter(Hand.PalmTransform.rotation);
		}
	}
}
