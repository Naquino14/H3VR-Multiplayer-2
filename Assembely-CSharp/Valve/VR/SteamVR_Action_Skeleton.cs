using System;
using UnityEngine;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Action_Skeleton : SteamVR_Action_Pose_Base<SteamVR_Action_Skeleton_Source_Map, SteamVR_Action_Skeleton_Source>, ISteamVR_Action_Skeleton_Source, ISerializationCallbackReceiver
	{
		public delegate void ActiveChangeHandler(SteamVR_Action_Skeleton fromAction, bool active);

		public delegate void ChangeHandler(SteamVR_Action_Skeleton fromAction);

		public delegate void UpdateHandler(SteamVR_Action_Skeleton fromAction);

		public delegate void TrackingChangeHandler(SteamVR_Action_Skeleton fromAction, ETrackingResult trackingState);

		public delegate void ValidPoseChangeHandler(SteamVR_Action_Skeleton fromAction, bool validPose);

		public delegate void DeviceConnectedChangeHandler(SteamVR_Action_Skeleton fromAction, bool deviceConnected);

		public const int numBones = 31;

		public static Quaternion steamVRFixUpRotation = Quaternion.AngleAxis(180f, Vector3.up);

		public Vector3[] bonePositions => sourceMap[SteamVR_Input_Sources.Any].bonePositions;

		public Quaternion[] boneRotations => sourceMap[SteamVR_Input_Sources.Any].boneRotations;

		public Vector3[] lastBonePositions => sourceMap[SteamVR_Input_Sources.Any].lastBonePositions;

		public Quaternion[] lastBoneRotations => sourceMap[SteamVR_Input_Sources.Any].lastBoneRotations;

		public EVRSkeletalMotionRange rangeOfMotion
		{
			get
			{
				return sourceMap[SteamVR_Input_Sources.Any].rangeOfMotion;
			}
			set
			{
				sourceMap[SteamVR_Input_Sources.Any].rangeOfMotion = value;
			}
		}

		public EVRSkeletalTransformSpace skeletalTransformSpace
		{
			get
			{
				return sourceMap[SteamVR_Input_Sources.Any].skeletalTransformSpace;
			}
			set
			{
				sourceMap[SteamVR_Input_Sources.Any].skeletalTransformSpace = value;
			}
		}

		public EVRSummaryType summaryDataType
		{
			get
			{
				return sourceMap[SteamVR_Input_Sources.Any].summaryDataType;
			}
			set
			{
				sourceMap[SteamVR_Input_Sources.Any].summaryDataType = value;
			}
		}

		public EVRSkeletalTrackingLevel skeletalTrackingLevel => sourceMap[SteamVR_Input_Sources.Any].skeletalTrackingLevel;

		public float thumbCurl => sourceMap[SteamVR_Input_Sources.Any].thumbCurl;

		public float indexCurl => sourceMap[SteamVR_Input_Sources.Any].indexCurl;

		public float middleCurl => sourceMap[SteamVR_Input_Sources.Any].middleCurl;

		public float ringCurl => sourceMap[SteamVR_Input_Sources.Any].ringCurl;

		public float pinkyCurl => sourceMap[SteamVR_Input_Sources.Any].pinkyCurl;

		public float thumbIndexSplay => sourceMap[SteamVR_Input_Sources.Any].thumbIndexSplay;

		public float indexMiddleSplay => sourceMap[SteamVR_Input_Sources.Any].indexMiddleSplay;

		public float middleRingSplay => sourceMap[SteamVR_Input_Sources.Any].middleRingSplay;

		public float ringPinkySplay => sourceMap[SteamVR_Input_Sources.Any].ringPinkySplay;

		public float lastThumbCurl => sourceMap[SteamVR_Input_Sources.Any].lastThumbCurl;

		public float lastIndexCurl => sourceMap[SteamVR_Input_Sources.Any].lastIndexCurl;

		public float lastMiddleCurl => sourceMap[SteamVR_Input_Sources.Any].lastMiddleCurl;

		public float lastRingCurl => sourceMap[SteamVR_Input_Sources.Any].lastRingCurl;

		public float lastPinkyCurl => sourceMap[SteamVR_Input_Sources.Any].lastPinkyCurl;

		public float lastThumbIndexSplay => sourceMap[SteamVR_Input_Sources.Any].lastThumbIndexSplay;

		public float lastIndexMiddleSplay => sourceMap[SteamVR_Input_Sources.Any].lastIndexMiddleSplay;

		public float lastMiddleRingSplay => sourceMap[SteamVR_Input_Sources.Any].lastMiddleRingSplay;

		public float lastRingPinkySplay => sourceMap[SteamVR_Input_Sources.Any].lastRingPinkySplay;

		public float[] fingerCurls => sourceMap[SteamVR_Input_Sources.Any].fingerCurls;

		public float[] fingerSplays => sourceMap[SteamVR_Input_Sources.Any].fingerSplays;

		public float[] lastFingerCurls => sourceMap[SteamVR_Input_Sources.Any].lastFingerCurls;

		public float[] lastFingerSplays => sourceMap[SteamVR_Input_Sources.Any].lastFingerSplays;

		public bool poseChanged => sourceMap[SteamVR_Input_Sources.Any].poseChanged;

		public bool onlyUpdateSummaryData
		{
			get
			{
				return sourceMap[SteamVR_Input_Sources.Any].onlyUpdateSummaryData;
			}
			set
			{
				sourceMap[SteamVR_Input_Sources.Any].onlyUpdateSummaryData = value;
			}
		}

		public int boneCount => (int)GetBoneCount();

		public event ActiveChangeHandler onActiveChange
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onActiveChange += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onActiveChange -= value;
			}
		}

		public event ActiveChangeHandler onActiveBindingChange
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange -= value;
			}
		}

		public event ChangeHandler onChange
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onChange += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onChange -= value;
			}
		}

		public event UpdateHandler onUpdate
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onUpdate += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onUpdate -= value;
			}
		}

		public event TrackingChangeHandler onTrackingChanged
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onTrackingChanged += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onTrackingChanged -= value;
			}
		}

		public event ValidPoseChangeHandler onValidPoseChanged
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onValidPoseChanged += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onValidPoseChanged -= value;
			}
		}

		public event DeviceConnectedChangeHandler onDeviceConnectedChanged
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onDeviceConnectedChanged += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onDeviceConnectedChanged -= value;
			}
		}

		public virtual void UpdateValue(bool skipStateAndEventUpdates)
		{
			sourceMap[SteamVR_Input_Sources.Any].UpdateValue(skipStateAndEventUpdates);
		}

		public void UpdateValueWithoutEvents()
		{
			sourceMap[SteamVR_Input_Sources.Any].UpdateValue(skipStateAndEventUpdates: true);
		}

		public void UpdateTransform(Transform transformToUpdate)
		{
			base.UpdateTransform(SteamVR_Input_Sources.Any, transformToUpdate);
		}

		public bool GetActive()
		{
			return sourceMap[SteamVR_Input_Sources.Any].active;
		}

		public bool GetSetActive()
		{
			return actionSet.IsActive();
		}

		public bool GetVelocitiesAtTimeOffset(float secondsFromNow, out Vector3 velocity, out Vector3 angularVelocity)
		{
			return sourceMap[SteamVR_Input_Sources.Any].GetVelocitiesAtTimeOffset(secondsFromNow, out velocity, out angularVelocity);
		}

		public bool GetPoseAtTimeOffset(float secondsFromNow, out Vector3 position, out Quaternion rotation, out Vector3 velocity, out Vector3 angularVelocity)
		{
			return sourceMap[SteamVR_Input_Sources.Any].GetPoseAtTimeOffset(secondsFromNow, out position, out rotation, out velocity, out angularVelocity);
		}

		public Vector3 GetLocalPosition()
		{
			return sourceMap[SteamVR_Input_Sources.Any].localPosition;
		}

		public Quaternion GetLocalRotation()
		{
			return sourceMap[SteamVR_Input_Sources.Any].localRotation;
		}

		public Vector3 GetVelocity()
		{
			return sourceMap[SteamVR_Input_Sources.Any].velocity;
		}

		public Vector3 GetAngularVelocity()
		{
			return sourceMap[SteamVR_Input_Sources.Any].angularVelocity;
		}

		public bool GetDeviceIsConnected()
		{
			return sourceMap[SteamVR_Input_Sources.Any].deviceIsConnected;
		}

		public bool GetPoseIsValid()
		{
			return sourceMap[SteamVR_Input_Sources.Any].poseIsValid;
		}

		public ETrackingResult GetTrackingResult()
		{
			return sourceMap[SteamVR_Input_Sources.Any].trackingState;
		}

		public Vector3 GetLastLocalPosition()
		{
			return sourceMap[SteamVR_Input_Sources.Any].lastLocalPosition;
		}

		public Quaternion GetLastLocalRotation()
		{
			return sourceMap[SteamVR_Input_Sources.Any].lastLocalRotation;
		}

		public Vector3 GetLastVelocity()
		{
			return sourceMap[SteamVR_Input_Sources.Any].lastVelocity;
		}

		public Vector3 GetLastAngularVelocity()
		{
			return sourceMap[SteamVR_Input_Sources.Any].lastAngularVelocity;
		}

		public bool GetLastDeviceIsConnected()
		{
			return sourceMap[SteamVR_Input_Sources.Any].lastDeviceIsConnected;
		}

		public bool GetLastPoseIsValid()
		{
			return sourceMap[SteamVR_Input_Sources.Any].lastPoseIsValid;
		}

		public ETrackingResult GetLastTrackingResult()
		{
			return sourceMap[SteamVR_Input_Sources.Any].lastTrackingState;
		}

		public Vector3[] GetBonePositions(bool copy = false)
		{
			if (copy)
			{
				return (Vector3[])sourceMap[SteamVR_Input_Sources.Any].bonePositions.Clone();
			}
			return sourceMap[SteamVR_Input_Sources.Any].bonePositions;
		}

		public Quaternion[] GetBoneRotations(bool copy = false)
		{
			if (copy)
			{
				return (Quaternion[])sourceMap[SteamVR_Input_Sources.Any].boneRotations.Clone();
			}
			return sourceMap[SteamVR_Input_Sources.Any].boneRotations;
		}

		public Vector3[] GetLastBonePositions(bool copy = false)
		{
			if (copy)
			{
				return (Vector3[])sourceMap[SteamVR_Input_Sources.Any].lastBonePositions.Clone();
			}
			return sourceMap[SteamVR_Input_Sources.Any].lastBonePositions;
		}

		public Quaternion[] GetLastBoneRotations(bool copy = false)
		{
			if (copy)
			{
				return (Quaternion[])sourceMap[SteamVR_Input_Sources.Any].lastBoneRotations.Clone();
			}
			return sourceMap[SteamVR_Input_Sources.Any].lastBoneRotations;
		}

		public void SetRangeOfMotion(EVRSkeletalMotionRange range)
		{
			sourceMap[SteamVR_Input_Sources.Any].rangeOfMotion = range;
		}

		public void SetSkeletalTransformSpace(EVRSkeletalTransformSpace space)
		{
			sourceMap[SteamVR_Input_Sources.Any].skeletalTransformSpace = space;
		}

		public uint GetBoneCount()
		{
			return sourceMap[SteamVR_Input_Sources.Any].GetBoneCount();
		}

		public int[] GetBoneHierarchy()
		{
			return sourceMap[SteamVR_Input_Sources.Any].GetBoneHierarchy();
		}

		public string GetBoneName(int boneIndex)
		{
			return sourceMap[SteamVR_Input_Sources.Any].GetBoneName(boneIndex);
		}

		public SteamVR_Utils.RigidTransform[] GetReferenceTransforms(EVRSkeletalTransformSpace transformSpace, EVRSkeletalReferencePose referencePose)
		{
			return sourceMap[SteamVR_Input_Sources.Any].GetReferenceTransforms(transformSpace, referencePose);
		}

		public EVRSkeletalTrackingLevel GetSkeletalTrackingLevel()
		{
			return sourceMap[SteamVR_Input_Sources.Any].GetSkeletalTrackingLevel();
		}

		public float[] GetFingerCurls(bool copy = false)
		{
			if (copy)
			{
				return (float[])sourceMap[SteamVR_Input_Sources.Any].fingerCurls.Clone();
			}
			return sourceMap[SteamVR_Input_Sources.Any].fingerCurls;
		}

		public float[] GetLastFingerCurls(bool copy = false)
		{
			if (copy)
			{
				return (float[])sourceMap[SteamVR_Input_Sources.Any].lastFingerCurls.Clone();
			}
			return sourceMap[SteamVR_Input_Sources.Any].lastFingerCurls;
		}

		public float[] GetFingerSplays(bool copy = false)
		{
			if (copy)
			{
				return (float[])sourceMap[SteamVR_Input_Sources.Any].fingerSplays.Clone();
			}
			return sourceMap[SteamVR_Input_Sources.Any].fingerSplays;
		}

		public float[] GetLastFingerSplays(bool copy = false)
		{
			if (copy)
			{
				return (float[])sourceMap[SteamVR_Input_Sources.Any].lastFingerSplays.Clone();
			}
			return sourceMap[SteamVR_Input_Sources.Any].lastFingerSplays;
		}

		public float GetFingerCurl(int finger)
		{
			return sourceMap[SteamVR_Input_Sources.Any].fingerCurls[finger];
		}

		public float GetSplay(int fingerGapIndex)
		{
			return sourceMap[SteamVR_Input_Sources.Any].fingerSplays[fingerGapIndex];
		}

		public float GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum finger)
		{
			return GetFingerCurl((int)finger);
		}

		public float GetSplay(SteamVR_Skeleton_FingerSplayIndexEnum fingerSplay)
		{
			return GetSplay((int)fingerSplay);
		}

		public float GetLastFingerCurl(int finger)
		{
			return sourceMap[SteamVR_Input_Sources.Any].lastFingerCurls[finger];
		}

		public float GetLastSplay(int fingerGapIndex)
		{
			return sourceMap[SteamVR_Input_Sources.Any].lastFingerSplays[fingerGapIndex];
		}

		public float GetLastFingerCurl(SteamVR_Skeleton_FingerIndexEnum finger)
		{
			return GetLastFingerCurl((int)finger);
		}

		public float GetLastSplay(SteamVR_Skeleton_FingerSplayIndexEnum fingerSplay)
		{
			return GetLastSplay((int)fingerSplay);
		}

		public string GetLocalizedName(params EVRInputStringBits[] localizedParts)
		{
			return sourceMap[SteamVR_Input_Sources.Any].GetLocalizedOriginPart(localizedParts);
		}

		public void AddOnDeviceConnectedChanged(DeviceConnectedChangeHandler functionToCall)
		{
			sourceMap[SteamVR_Input_Sources.Any].onDeviceConnectedChanged += functionToCall;
		}

		public void RemoveOnDeviceConnectedChanged(DeviceConnectedChangeHandler functionToStopCalling)
		{
			sourceMap[SteamVR_Input_Sources.Any].onDeviceConnectedChanged -= functionToStopCalling;
		}

		public void AddOnTrackingChanged(TrackingChangeHandler functionToCall)
		{
			sourceMap[SteamVR_Input_Sources.Any].onTrackingChanged += functionToCall;
		}

		public void RemoveOnTrackingChanged(TrackingChangeHandler functionToStopCalling)
		{
			sourceMap[SteamVR_Input_Sources.Any].onTrackingChanged -= functionToStopCalling;
		}

		public void AddOnValidPoseChanged(ValidPoseChangeHandler functionToCall)
		{
			sourceMap[SteamVR_Input_Sources.Any].onValidPoseChanged += functionToCall;
		}

		public void RemoveOnValidPoseChanged(ValidPoseChangeHandler functionToStopCalling)
		{
			sourceMap[SteamVR_Input_Sources.Any].onValidPoseChanged -= functionToStopCalling;
		}

		public void AddOnActiveChangeListener(ActiveChangeHandler functionToCall)
		{
			sourceMap[SteamVR_Input_Sources.Any].onActiveChange += functionToCall;
		}

		public void RemoveOnActiveChangeListener(ActiveChangeHandler functionToStopCalling)
		{
			sourceMap[SteamVR_Input_Sources.Any].onActiveChange -= functionToStopCalling;
		}

		public void AddOnChangeListener(ChangeHandler functionToCall)
		{
			sourceMap[SteamVR_Input_Sources.Any].onChange += functionToCall;
		}

		public void RemoveOnChangeListener(ChangeHandler functionToStopCalling)
		{
			sourceMap[SteamVR_Input_Sources.Any].onChange -= functionToStopCalling;
		}

		public void AddOnUpdateListener(UpdateHandler functionToCall)
		{
			sourceMap[SteamVR_Input_Sources.Any].onUpdate += functionToCall;
		}

		public void RemoveOnUpdateListener(UpdateHandler functionToStopCalling)
		{
			sourceMap[SteamVR_Input_Sources.Any].onUpdate -= functionToStopCalling;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			InitAfterDeserialize();
		}
	}
}
