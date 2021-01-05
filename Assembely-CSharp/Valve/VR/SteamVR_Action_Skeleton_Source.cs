using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Action_Skeleton_Source : SteamVR_Action_Pose_Source, ISteamVR_Action_Skeleton_Source
	{
		protected static uint skeletonActionData_size;

		protected VRSkeletalSummaryData_t skeletalSummaryData = default(VRSkeletalSummaryData_t);

		protected VRSkeletalSummaryData_t lastSkeletalSummaryData = default(VRSkeletalSummaryData_t);

		protected SteamVR_Action_Skeleton skeletonAction;

		protected VRBoneTransform_t[] tempBoneTransforms = new VRBoneTransform_t[31];

		protected InputSkeletalActionData_t skeletonActionData = default(InputSkeletalActionData_t);

		protected InputSkeletalActionData_t lastSkeletonActionData = default(InputSkeletalActionData_t);

		protected InputSkeletalActionData_t tempSkeletonActionData = default(InputSkeletalActionData_t);

		public override bool activeBinding => skeletonActionData.bActive;

		public override bool lastActiveBinding => lastSkeletonActionData.bActive;

		public Vector3[] bonePositions
		{
			get;
			protected set;
		}

		public Quaternion[] boneRotations
		{
			get;
			protected set;
		}

		public Vector3[] lastBonePositions
		{
			get;
			protected set;
		}

		public Quaternion[] lastBoneRotations
		{
			get;
			protected set;
		}

		public EVRSkeletalMotionRange rangeOfMotion
		{
			get;
			set;
		}

		public EVRSkeletalTransformSpace skeletalTransformSpace
		{
			get;
			set;
		}

		public EVRSummaryType summaryDataType
		{
			get;
			set;
		}

		public float thumbCurl => fingerCurls[0];

		public float indexCurl => fingerCurls[1];

		public float middleCurl => fingerCurls[2];

		public float ringCurl => fingerCurls[3];

		public float pinkyCurl => fingerCurls[4];

		public float thumbIndexSplay => fingerSplays[0];

		public float indexMiddleSplay => fingerSplays[1];

		public float middleRingSplay => fingerSplays[2];

		public float ringPinkySplay => fingerSplays[3];

		public float lastThumbCurl => lastFingerCurls[0];

		public float lastIndexCurl => lastFingerCurls[1];

		public float lastMiddleCurl => lastFingerCurls[2];

		public float lastRingCurl => lastFingerCurls[3];

		public float lastPinkyCurl => lastFingerCurls[4];

		public float lastThumbIndexSplay => lastFingerSplays[0];

		public float lastIndexMiddleSplay => lastFingerSplays[1];

		public float lastMiddleRingSplay => lastFingerSplays[2];

		public float lastRingPinkySplay => lastFingerSplays[3];

		public float[] fingerCurls
		{
			get;
			protected set;
		}

		public float[] fingerSplays
		{
			get;
			protected set;
		}

		public float[] lastFingerCurls
		{
			get;
			protected set;
		}

		public float[] lastFingerSplays
		{
			get;
			protected set;
		}

		public bool poseChanged
		{
			get;
			protected set;
		}

		public bool onlyUpdateSummaryData
		{
			get;
			set;
		}

		public int boneCount => (int)GetBoneCount();

		public int[] boneHierarchy => GetBoneHierarchy();

		public EVRSkeletalTrackingLevel skeletalTrackingLevel => GetSkeletalTrackingLevel();

		public new event SteamVR_Action_Skeleton.ActiveChangeHandler onActiveChange;

		public new event SteamVR_Action_Skeleton.ActiveChangeHandler onActiveBindingChange;

		public new event SteamVR_Action_Skeleton.ChangeHandler onChange;

		public new event SteamVR_Action_Skeleton.UpdateHandler onUpdate;

		public new event SteamVR_Action_Skeleton.TrackingChangeHandler onTrackingChanged;

		public new event SteamVR_Action_Skeleton.ValidPoseChangeHandler onValidPoseChanged;

		public new event SteamVR_Action_Skeleton.DeviceConnectedChangeHandler onDeviceConnectedChanged;

		public override void Preinitialize(SteamVR_Action wrappingAction, SteamVR_Input_Sources forInputSource)
		{
			base.Preinitialize(wrappingAction, forInputSource);
			skeletonAction = (SteamVR_Action_Skeleton)wrappingAction;
			bonePositions = new Vector3[31];
			lastBonePositions = new Vector3[31];
			boneRotations = new Quaternion[31];
			lastBoneRotations = new Quaternion[31];
			rangeOfMotion = EVRSkeletalMotionRange.WithController;
			skeletalTransformSpace = EVRSkeletalTransformSpace.Parent;
			fingerCurls = new float[SteamVR_Skeleton_FingerIndexes.enumArray.Length];
			fingerSplays = new float[SteamVR_Skeleton_FingerSplayIndexes.enumArray.Length];
			lastFingerCurls = new float[SteamVR_Skeleton_FingerIndexes.enumArray.Length];
			lastFingerSplays = new float[SteamVR_Skeleton_FingerSplayIndexes.enumArray.Length];
		}

		public override void Initialize()
		{
			base.Initialize();
			if (skeletonActionData_size == 0)
			{
				skeletonActionData_size = (uint)Marshal.SizeOf(typeof(InputSkeletalActionData_t));
			}
		}

		public override void UpdateValue()
		{
			UpdateValue(skipStateAndEventUpdates: false);
		}

		public override void UpdateValue(bool skipStateAndEventUpdates)
		{
			lastActive = active;
			lastSkeletonActionData = skeletonActionData;
			lastSkeletalSummaryData = skeletalSummaryData;
			if (!onlyUpdateSummaryData)
			{
				for (int i = 0; i < 31; i++)
				{
					ref Vector3 reference = ref lastBonePositions[i];
					reference = bonePositions[i];
					ref Quaternion reference2 = ref lastBoneRotations[i];
					reference2 = boneRotations[i];
				}
			}
			for (int j = 0; j < SteamVR_Skeleton_FingerIndexes.enumArray.Length; j++)
			{
				lastFingerCurls[j] = fingerCurls[j];
			}
			for (int k = 0; k < SteamVR_Skeleton_FingerSplayIndexes.enumArray.Length; k++)
			{
				lastFingerSplays[k] = fingerSplays[k];
			}
			base.UpdateValue(skipStateAndEventUpdates: true);
			poseChanged = changed;
			EVRInputError skeletalActionData = OpenVR.Input.GetSkeletalActionData(base.handle, ref skeletonActionData, skeletonActionData_size);
			if (skeletalActionData != 0)
			{
				Debug.LogError("<b>[SteamVR]</b> GetSkeletalActionData error (" + base.fullPath + "): " + skeletalActionData.ToString() + " handle: " + base.handle);
				return;
			}
			if (active)
			{
				if (!onlyUpdateSummaryData)
				{
					skeletalActionData = OpenVR.Input.GetSkeletalBoneData(base.handle, skeletalTransformSpace, rangeOfMotion, tempBoneTransforms);
					if (skeletalActionData != 0)
					{
						Debug.LogError("<b>[SteamVR]</b> GetSkeletalBoneData error (" + base.fullPath + "): " + skeletalActionData.ToString() + " handle: " + base.handle);
					}
					for (int l = 0; l < tempBoneTransforms.Length; l++)
					{
						bonePositions[l].x = 0f - tempBoneTransforms[l].position.v0;
						bonePositions[l].y = tempBoneTransforms[l].position.v1;
						bonePositions[l].z = tempBoneTransforms[l].position.v2;
						boneRotations[l].x = tempBoneTransforms[l].orientation.x;
						boneRotations[l].y = 0f - tempBoneTransforms[l].orientation.y;
						boneRotations[l].z = 0f - tempBoneTransforms[l].orientation.z;
						boneRotations[l].w = tempBoneTransforms[l].orientation.w;
					}
					ref Quaternion reference3 = ref boneRotations[0];
					reference3 = SteamVR_Action_Skeleton.steamVRFixUpRotation * boneRotations[0];
				}
				UpdateSkeletalSummaryData(summaryDataType, force: true);
			}
			if (!changed)
			{
				for (int m = 0; m < tempBoneTransforms.Length; m++)
				{
					if (Vector3.Distance(lastBonePositions[m], bonePositions[m]) > changeTolerance)
					{
						changed = true;
						break;
					}
					if (Mathf.Abs(Quaternion.Angle(lastBoneRotations[m], boneRotations[m])) > changeTolerance)
					{
						changed = true;
						break;
					}
				}
			}
			if (changed)
			{
				base.changedTime = Time.realtimeSinceStartup;
			}
			if (!skipStateAndEventUpdates)
			{
				CheckAndSendEvents();
			}
		}

		public uint GetBoneCount()
		{
			uint pBoneCount = 0u;
			EVRInputError boneCount = OpenVR.Input.GetBoneCount(base.handle, ref pBoneCount);
			if (boneCount != 0)
			{
				Debug.LogError("<b>[SteamVR]</b> GetBoneCount error (" + base.fullPath + "): " + boneCount.ToString() + " handle: " + base.handle);
			}
			return pBoneCount;
		}

		public int[] GetBoneHierarchy()
		{
			int boneCount = (int)GetBoneCount();
			int[] array = new int[boneCount];
			EVRInputError boneHierarchy = OpenVR.Input.GetBoneHierarchy(base.handle, array);
			if (boneHierarchy != 0)
			{
				Debug.LogError("<b>[SteamVR]</b> GetBoneHierarchy error (" + base.fullPath + "): " + boneHierarchy.ToString() + " handle: " + base.handle);
			}
			return array;
		}

		public string GetBoneName(int boneIndex)
		{
			StringBuilder stringBuilder = new StringBuilder(255);
			EVRInputError boneName = OpenVR.Input.GetBoneName(base.handle, boneIndex, stringBuilder, 255u);
			if (boneName != 0)
			{
				Debug.LogError("<b>[SteamVR]</b> GetBoneName error (" + base.fullPath + "): " + boneName.ToString() + " handle: " + base.handle);
			}
			return stringBuilder.ToString();
		}

		public SteamVR_Utils.RigidTransform[] GetReferenceTransforms(EVRSkeletalTransformSpace transformSpace, EVRSkeletalReferencePose referencePose)
		{
			SteamVR_Utils.RigidTransform[] array = new SteamVR_Utils.RigidTransform[GetBoneCount()];
			VRBoneTransform_t[] array2 = new VRBoneTransform_t[array.Length];
			EVRInputError skeletalReferenceTransforms = OpenVR.Input.GetSkeletalReferenceTransforms(base.handle, transformSpace, referencePose, array2);
			if (skeletalReferenceTransforms != 0)
			{
				Debug.LogError("<b>[SteamVR]</b> GetSkeletalReferenceTransforms error (" + base.fullPath + "): " + skeletalReferenceTransforms.ToString() + " handle: " + base.handle);
			}
			for (int i = 0; i < array2.Length; i++)
			{
				Vector3 position = new Vector3(0f - array2[i].position.v0, array2[i].position.v1, array2[i].position.v2);
				Quaternion rotation = new Quaternion(array2[i].orientation.x, 0f - array2[i].orientation.y, 0f - array2[i].orientation.z, array2[i].orientation.w);
				ref SteamVR_Utils.RigidTransform reference = ref array[i];
				reference = new SteamVR_Utils.RigidTransform(position, rotation);
			}
			if (array.Length > 0)
			{
				Quaternion quaternion = Quaternion.AngleAxis(180f, Vector3.up);
				array[0].rot = quaternion * array[0].rot;
			}
			return array;
		}

		public EVRSkeletalTrackingLevel GetSkeletalTrackingLevel()
		{
			EVRSkeletalTrackingLevel pSkeletalTrackingLevel = EVRSkeletalTrackingLevel.VRSkeletalTracking_Estimated;
			EVRInputError skeletalTrackingLevel = OpenVR.Input.GetSkeletalTrackingLevel(base.handle, ref pSkeletalTrackingLevel);
			if (skeletalTrackingLevel != 0)
			{
				Debug.LogError("<b>[SteamVR]</b> GetSkeletalTrackingLevel error (" + base.fullPath + "): " + skeletalTrackingLevel.ToString() + " handle: " + base.handle);
			}
			return pSkeletalTrackingLevel;
		}

		protected VRSkeletalSummaryData_t GetSkeletalSummaryData(EVRSummaryType summaryType = EVRSummaryType.FromAnimation, bool force = false)
		{
			UpdateSkeletalSummaryData(summaryType, force);
			return skeletalSummaryData;
		}

		protected void UpdateSkeletalSummaryData(EVRSummaryType summaryType = EVRSummaryType.FromAnimation, bool force = false)
		{
			if (force || (summaryDataType != summaryDataType && active))
			{
				EVRInputError eVRInputError = OpenVR.Input.GetSkeletalSummaryData(base.handle, summaryType, ref skeletalSummaryData);
				if (eVRInputError != 0)
				{
					Debug.LogError("<b>[SteamVR]</b> GetSkeletalSummaryData error (" + base.fullPath + "): " + eVRInputError.ToString() + " handle: " + base.handle);
				}
				fingerCurls[0] = skeletalSummaryData.flFingerCurl0;
				fingerCurls[1] = skeletalSummaryData.flFingerCurl1;
				fingerCurls[2] = skeletalSummaryData.flFingerCurl2;
				fingerCurls[3] = skeletalSummaryData.flFingerCurl3;
				fingerCurls[4] = skeletalSummaryData.flFingerCurl4;
				fingerSplays[0] = skeletalSummaryData.flFingerSplay0;
				fingerSplays[1] = skeletalSummaryData.flFingerSplay1;
				fingerSplays[2] = skeletalSummaryData.flFingerSplay2;
				fingerSplays[3] = skeletalSummaryData.flFingerSplay3;
			}
		}

		protected override void CheckAndSendEvents()
		{
			if (base.trackingState != base.lastTrackingState && this.onTrackingChanged != null)
			{
				this.onTrackingChanged(skeletonAction, base.trackingState);
			}
			if (base.poseIsValid != base.lastPoseIsValid && this.onValidPoseChanged != null)
			{
				this.onValidPoseChanged(skeletonAction, base.poseIsValid);
			}
			if (base.deviceIsConnected != base.lastDeviceIsConnected && this.onDeviceConnectedChanged != null)
			{
				this.onDeviceConnectedChanged(skeletonAction, base.deviceIsConnected);
			}
			if (changed && this.onChange != null)
			{
				this.onChange(skeletonAction);
			}
			if (active != lastActive && this.onActiveChange != null)
			{
				this.onActiveChange(skeletonAction, active);
			}
			if (activeBinding != lastActiveBinding && this.onActiveBindingChange != null)
			{
				this.onActiveBindingChange(skeletonAction, activeBinding);
			}
			if (this.onUpdate != null)
			{
				this.onUpdate(skeletonAction);
			}
		}
	}
}
