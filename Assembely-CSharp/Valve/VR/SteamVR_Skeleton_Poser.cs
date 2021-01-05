using System;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Skeleton_Poser : MonoBehaviour
	{
		public class SkeletonBlendablePose
		{
			public SteamVR_Skeleton_Pose pose;

			public SteamVR_Skeleton_PoseSnapshot snapshotR;

			public SteamVR_Skeleton_PoseSnapshot snapshotL;

			private Vector3[] additivePositionBuffer;

			private Quaternion[] additiveRotationBuffer;

			public SkeletonBlendablePose(SteamVR_Skeleton_Pose p)
			{
				pose = p;
				snapshotR = new SteamVR_Skeleton_PoseSnapshot(p.rightHand.bonePositions.Length, SteamVR_Input_Sources.RightHand);
				snapshotL = new SteamVR_Skeleton_PoseSnapshot(p.leftHand.bonePositions.Length, SteamVR_Input_Sources.LeftHand);
			}

			public SkeletonBlendablePose()
			{
			}

			public SteamVR_Skeleton_PoseSnapshot GetHandSnapshot(SteamVR_Input_Sources inputSource)
			{
				if (inputSource == SteamVR_Input_Sources.LeftHand)
				{
					return snapshotL;
				}
				return snapshotR;
			}

			public void UpdateAdditiveAnimation(SteamVR_Action_Skeleton skeletonAction, SteamVR_Input_Sources inputSource)
			{
				SteamVR_Skeleton_PoseSnapshot handSnapshot = GetHandSnapshot(inputSource);
				SteamVR_Skeleton_Pose_Hand hand = pose.GetHand(inputSource);
				if (additivePositionBuffer == null)
				{
					additivePositionBuffer = new Vector3[skeletonAction.boneCount];
				}
				if (additiveRotationBuffer == null)
				{
					additiveRotationBuffer = new Quaternion[skeletonAction.boneCount];
				}
				for (int i = 0; i < snapshotL.bonePositions.Length; i++)
				{
					int fingerForBone = SteamVR_Skeleton_JointIndexes.GetFingerForBone(i);
					SteamVR_Skeleton_FingerExtensionTypes movementTypeForBone = hand.GetMovementTypeForBone(i);
					if (inputSource == SteamVR_Input_Sources.LeftHand)
					{
						SteamVR_Behaviour_Skeleton.MirrorBonePosition(ref skeletonAction.bonePositions[i], ref additivePositionBuffer[i], i);
						SteamVR_Behaviour_Skeleton.MirrorBoneRotation(ref skeletonAction.boneRotations[i], ref additiveRotationBuffer[i], i);
					}
					else
					{
						ref Vector3 reference = ref additivePositionBuffer[i];
						reference = skeletonAction.bonePositions[i];
						ref Quaternion reference2 = ref additiveRotationBuffer[i];
						reference2 = skeletonAction.boneRotations[i];
					}
					switch (movementTypeForBone)
					{
					case SteamVR_Skeleton_FingerExtensionTypes.Free:
					{
						ref Vector3 reference7 = ref handSnapshot.bonePositions[i];
						reference7 = additivePositionBuffer[i];
						ref Quaternion reference8 = ref handSnapshot.boneRotations[i];
						reference8 = additiveRotationBuffer[i];
						break;
					}
					case SteamVR_Skeleton_FingerExtensionTypes.Extend:
					{
						ref Vector3 reference5 = ref handSnapshot.bonePositions[i];
						reference5 = Vector3.Lerp(hand.bonePositions[i], additivePositionBuffer[i], 1f - skeletonAction.fingerCurls[fingerForBone]);
						ref Quaternion reference6 = ref handSnapshot.boneRotations[i];
						reference6 = Quaternion.Lerp(hand.boneRotations[i], additiveRotationBuffer[i], 1f - skeletonAction.fingerCurls[fingerForBone]);
						break;
					}
					case SteamVR_Skeleton_FingerExtensionTypes.Contract:
					{
						ref Vector3 reference3 = ref handSnapshot.bonePositions[i];
						reference3 = Vector3.Lerp(hand.bonePositions[i], additivePositionBuffer[i], skeletonAction.fingerCurls[fingerForBone]);
						ref Quaternion reference4 = ref handSnapshot.boneRotations[i];
						reference4 = Quaternion.Lerp(hand.boneRotations[i], additiveRotationBuffer[i], skeletonAction.fingerCurls[fingerForBone]);
						break;
					}
					}
				}
			}

			public void PoseToSnapshots()
			{
				snapshotR.position = pose.rightHand.position;
				snapshotR.rotation = pose.rightHand.rotation;
				pose.rightHand.bonePositions.CopyTo(snapshotR.bonePositions, 0);
				pose.rightHand.boneRotations.CopyTo(snapshotR.boneRotations, 0);
				snapshotL.position = pose.leftHand.position;
				snapshotL.rotation = pose.leftHand.rotation;
				pose.leftHand.bonePositions.CopyTo(snapshotL.bonePositions, 0);
				pose.leftHand.boneRotations.CopyTo(snapshotL.boneRotations, 0);
			}
		}

		[Serializable]
		public class PoseBlendingBehaviour
		{
			public enum BlenderTypes
			{
				Manual,
				AnalogAction,
				BooleanAction
			}

			public string name;

			public bool enabled = true;

			public float influence = 1f;

			public int pose = 1;

			public float value;

			public SteamVR_Action_Single action_single;

			public SteamVR_Action_Boolean action_bool;

			public float smoothingSpeed;

			public BlenderTypes type;

			public bool useMask;

			public SteamVR_Skeleton_HandMask mask = new SteamVR_Skeleton_HandMask();

			public bool previewEnabled;

			public PoseBlendingBehaviour()
			{
				enabled = true;
				influence = 1f;
			}

			public void Update(float deltaTime, SteamVR_Input_Sources inputSource)
			{
				if (type == BlenderTypes.AnalogAction)
				{
					if (smoothingSpeed == 0f)
					{
						value = action_single.GetAxis(inputSource);
					}
					else
					{
						value = Mathf.Lerp(value, action_single.GetAxis(inputSource), deltaTime * smoothingSpeed);
					}
				}
				if (type == BlenderTypes.BooleanAction)
				{
					if (smoothingSpeed == 0f)
					{
						value = (action_bool.GetState(inputSource) ? 1 : 0);
					}
					else
					{
						value = Mathf.Lerp(value, action_bool.GetState(inputSource) ? 1 : 0, deltaTime * smoothingSpeed);
					}
				}
			}

			public void ApplyBlending(SteamVR_Skeleton_PoseSnapshot snapshot, SkeletonBlendablePose[] blendPoses, SteamVR_Input_Sources inputSource)
			{
				SteamVR_Skeleton_PoseSnapshot handSnapshot = blendPoses[pose].GetHandSnapshot(inputSource);
				if (mask.GetFinger(0) || !useMask)
				{
					snapshot.position = Vector3.Lerp(snapshot.position, handSnapshot.position, influence * value);
					snapshot.rotation = Quaternion.Slerp(snapshot.rotation, handSnapshot.rotation, influence * value);
				}
				for (int i = 0; i < snapshot.bonePositions.Length; i++)
				{
					if (mask.GetFinger(SteamVR_Skeleton_JointIndexes.GetFingerForBone(i) + 1) || !useMask)
					{
						ref Vector3 reference = ref snapshot.bonePositions[i];
						reference = Vector3.Lerp(snapshot.bonePositions[i], handSnapshot.bonePositions[i], influence * value);
						ref Quaternion reference2 = ref snapshot.boneRotations[i];
						reference2 = Quaternion.Slerp(snapshot.boneRotations[i], handSnapshot.boneRotations[i], influence * value);
					}
				}
			}
		}

		public bool poseEditorExpanded = true;

		public bool blendEditorExpanded = true;

		public string[] poseNames;

		public GameObject previewLeftHandPrefab;

		public GameObject previewRightHandPrefab;

		public SteamVR_Skeleton_Pose skeletonMainPose;

		public List<SteamVR_Skeleton_Pose> skeletonAdditionalPoses = new List<SteamVR_Skeleton_Pose>();

		[SerializeField]
		protected bool showLeftPreview;

		[SerializeField]
		protected bool showRightPreview = true;

		[SerializeField]
		protected GameObject previewLeftInstance;

		[SerializeField]
		protected GameObject previewRightInstance;

		[SerializeField]
		protected int previewPoseSelection;

		public List<PoseBlendingBehaviour> blendingBehaviours = new List<PoseBlendingBehaviour>();

		public SteamVR_Skeleton_PoseSnapshot blendedSnapshotL;

		public SteamVR_Skeleton_PoseSnapshot blendedSnapshotR;

		private SkeletonBlendablePose[] blendPoses;

		private int boneCount;

		private bool poseUpdatedThisFrame;

		public float scale;

		public int blendPoseCount => blendPoses.Length;

		protected void Awake()
		{
			if (previewLeftInstance != null)
			{
				UnityEngine.Object.DestroyImmediate(previewLeftInstance);
			}
			if (previewRightInstance != null)
			{
				UnityEngine.Object.DestroyImmediate(previewRightInstance);
			}
			blendPoses = new SkeletonBlendablePose[skeletonAdditionalPoses.Count + 1];
			for (int i = 0; i < blendPoseCount; i++)
			{
				blendPoses[i] = new SkeletonBlendablePose(GetPoseByIndex(i));
				blendPoses[i].PoseToSnapshots();
			}
			boneCount = skeletonMainPose.leftHand.bonePositions.Length;
			blendedSnapshotL = new SteamVR_Skeleton_PoseSnapshot(boneCount, SteamVR_Input_Sources.LeftHand);
			blendedSnapshotR = new SteamVR_Skeleton_PoseSnapshot(boneCount, SteamVR_Input_Sources.RightHand);
		}

		public void SetBlendingBehaviourValue(string behaviourName, float value)
		{
			PoseBlendingBehaviour poseBlendingBehaviour = blendingBehaviours.Find((PoseBlendingBehaviour b) => b.name == behaviourName);
			if (poseBlendingBehaviour == null)
			{
				Debug.LogError("[SteamVR] Blending Behaviour: " + behaviourName + " not found on Skeleton Poser: " + base.gameObject.name);
				return;
			}
			if (poseBlendingBehaviour.type != 0)
			{
				Debug.LogWarning("[SteamVR] Blending Behaviour: " + behaviourName + " is not a manual behaviour. Its value will likely be overriden.");
			}
			poseBlendingBehaviour.value = value;
		}

		public float GetBlendingBehaviourValue(string behaviourName)
		{
			PoseBlendingBehaviour poseBlendingBehaviour = blendingBehaviours.Find((PoseBlendingBehaviour b) => b.name == behaviourName);
			if (poseBlendingBehaviour == null)
			{
				Debug.LogError("[SteamVR] Blending Behaviour: " + behaviourName + " not found on Skeleton Poser: " + base.gameObject.name);
				return 0f;
			}
			return poseBlendingBehaviour.value;
		}

		public void SetBlendingBehaviourEnabled(string behaviourName, bool value)
		{
			PoseBlendingBehaviour poseBlendingBehaviour = blendingBehaviours.Find((PoseBlendingBehaviour b) => b.name == behaviourName);
			if (poseBlendingBehaviour == null)
			{
				Debug.LogError("[SteamVR] Blending Behaviour: " + behaviourName + " not found on Skeleton Poser: " + base.gameObject.name);
			}
			else
			{
				poseBlendingBehaviour.enabled = value;
			}
		}

		public bool GetBlendingBehaviourEnabled(string behaviourName)
		{
			PoseBlendingBehaviour poseBlendingBehaviour = blendingBehaviours.Find((PoseBlendingBehaviour b) => b.name == behaviourName);
			if (poseBlendingBehaviour == null)
			{
				Debug.LogError("[SteamVR] Blending Behaviour: " + behaviourName + " not found on Skeleton Poser: " + base.gameObject.name);
				return false;
			}
			return poseBlendingBehaviour.enabled;
		}

		public PoseBlendingBehaviour GetBlendingBehaviour(string behaviourName)
		{
			PoseBlendingBehaviour poseBlendingBehaviour = blendingBehaviours.Find((PoseBlendingBehaviour b) => b.name == behaviourName);
			if (poseBlendingBehaviour == null)
			{
				Debug.LogError("[SteamVR] Blending Behaviour: " + behaviourName + " not found on Skeleton Poser: " + base.gameObject.name);
				return null;
			}
			return poseBlendingBehaviour;
		}

		public SteamVR_Skeleton_Pose GetPoseByIndex(int index)
		{
			if (index == 0)
			{
				return skeletonMainPose;
			}
			return skeletonAdditionalPoses[index - 1];
		}

		private SteamVR_Skeleton_PoseSnapshot GetHandSnapshot(SteamVR_Input_Sources inputSource)
		{
			if (inputSource == SteamVR_Input_Sources.LeftHand)
			{
				return blendedSnapshotL;
			}
			return blendedSnapshotR;
		}

		public SteamVR_Skeleton_PoseSnapshot GetBlendedPose(SteamVR_Action_Skeleton skeletonAction, SteamVR_Input_Sources handType)
		{
			UpdatePose(skeletonAction, handType);
			return GetHandSnapshot(handType);
		}

		public SteamVR_Skeleton_PoseSnapshot GetBlendedPose(SteamVR_Behaviour_Skeleton skeletonBehaviour)
		{
			return GetBlendedPose(skeletonBehaviour.skeletonAction, skeletonBehaviour.inputSource);
		}

		public void UpdatePose(SteamVR_Action_Skeleton skeletonAction, SteamVR_Input_Sources inputSource)
		{
			if (!poseUpdatedThisFrame)
			{
				poseUpdatedThisFrame = true;
				blendPoses[0].UpdateAdditiveAnimation(skeletonAction, inputSource);
				SteamVR_Skeleton_PoseSnapshot handSnapshot = GetHandSnapshot(inputSource);
				handSnapshot.CopyFrom(blendPoses[0].GetHandSnapshot(inputSource));
				ApplyBlenderBehaviours(skeletonAction, inputSource, handSnapshot);
				switch (inputSource)
				{
				case SteamVR_Input_Sources.RightHand:
					blendedSnapshotR = handSnapshot;
					break;
				case SteamVR_Input_Sources.LeftHand:
					blendedSnapshotL = handSnapshot;
					break;
				}
			}
		}

		protected void ApplyBlenderBehaviours(SteamVR_Action_Skeleton skeletonAction, SteamVR_Input_Sources inputSource, SteamVR_Skeleton_PoseSnapshot snapshot)
		{
			for (int i = 0; i < blendingBehaviours.Count; i++)
			{
				blendingBehaviours[i].Update(Time.deltaTime, inputSource);
				if (blendingBehaviours[i].enabled && blendingBehaviours[i].influence * blendingBehaviours[i].value > 0.01f)
				{
					if (blendingBehaviours[i].pose != 0)
					{
						blendPoses[blendingBehaviours[i].pose].UpdateAdditiveAnimation(skeletonAction, inputSource);
					}
					blendingBehaviours[i].ApplyBlending(snapshot, blendPoses, inputSource);
				}
			}
		}

		protected void LateUpdate()
		{
			poseUpdatedThisFrame = false;
		}

		protected Vector3 BlendVectors(Vector3[] vectors, float[] weights)
		{
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < vectors.Length; i++)
			{
				zero += vectors[i] * weights[i];
			}
			return zero;
		}

		protected Quaternion BlendQuaternions(Quaternion[] quaternions, float[] weights)
		{
			Quaternion identity = Quaternion.identity;
			for (int i = 0; i < quaternions.Length; i++)
			{
				identity *= Quaternion.Slerp(Quaternion.identity, quaternions[i], weights[i]);
			}
			return identity;
		}
	}
}
