using System.Collections;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Behaviour_Skeleton : MonoBehaviour
	{
		public enum MirrorType
		{
			None,
			LeftToRight,
			RightToLeft
		}

		public delegate void ActiveChangeHandler(SteamVR_Behaviour_Skeleton fromAction, SteamVR_Input_Sources inputSource, bool active);

		public delegate void ChangeHandler(SteamVR_Behaviour_Skeleton fromAction, SteamVR_Input_Sources inputSource);

		public delegate void UpdateHandler(SteamVR_Behaviour_Skeleton fromAction, SteamVR_Input_Sources inputSource);

		public delegate void TrackingChangeHandler(SteamVR_Behaviour_Skeleton fromAction, SteamVR_Input_Sources inputSource, ETrackingResult trackingState);

		public delegate void ValidPoseChangeHandler(SteamVR_Behaviour_Skeleton fromAction, SteamVR_Input_Sources inputSource, bool validPose);

		public delegate void DeviceConnectedChangeHandler(SteamVR_Behaviour_Skeleton fromAction, SteamVR_Input_Sources inputSource, bool deviceConnected);

		[Tooltip("If not set, will try to auto assign this based on 'Skeleton' + inputSource")]
		public SteamVR_Action_Skeleton skeletonAction;

		[Tooltip("The device this action should apply to. Any if the action is not device specific.")]
		public SteamVR_Input_Sources inputSource;

		[Tooltip("The range of motion you'd like the hand to move in. With controller is the best estimate of the fingers wrapped around a controller. Without is from a flat hand to a fist.")]
		public EVRSkeletalMotionRange rangeOfMotion = EVRSkeletalMotionRange.WithoutController;

		[Tooltip("This needs to be in the order of: root -> wrist -> thumb, index, middle, ring, pinky")]
		public Transform skeletonRoot;

		[Tooltip("If not set, relative to parent")]
		public Transform origin;

		[Tooltip("Set to true if you want this script to update its position and rotation. False if this will be handled elsewhere")]
		public bool updatePose = true;

		[Tooltip("Check this to not set the positions of the bones. This is helpful for differently scaled skeletons.")]
		public bool onlySetRotations;

		[Range(0f, 1f)]
		[Tooltip("Modify this to blend between animations setup on the hand")]
		public float skeletonBlend = 1f;

		public SteamVR_Behaviour_SkeletonEvent onBoneTransformsUpdated;

		public SteamVR_Behaviour_SkeletonEvent onTransformUpdated;

		public SteamVR_Behaviour_SkeletonEvent onTransformChanged;

		public SteamVR_Behaviour_Skeleton_ConnectedChangedEvent onConnectedChanged;

		public SteamVR_Behaviour_Skeleton_TrackingChangedEvent onTrackingChanged;

		public UpdateHandler onBoneTransformsUpdatedEvent;

		public UpdateHandler onTransformUpdatedEvent;

		public ChangeHandler onTransformChangedEvent;

		public DeviceConnectedChangeHandler onConnectedChangedEvent;

		public TrackingChangeHandler onTrackingChangedEvent;

		protected SteamVR_Skeleton_Poser blendPoser;

		protected SteamVR_Skeleton_PoseSnapshot blendSnapshot;

		[Tooltip("Is this rendermodel a mirror of another one?")]
		public MirrorType mirroring;

		protected Coroutine blendRoutine;

		protected Coroutine rangeOfMotionBlendRoutine;

		protected Coroutine attachRoutine;

		protected Transform[] bones;

		protected EVRSkeletalMotionRange? temporaryRangeOfMotion;

		private Vector3[] oldROMPositionBuffer = new Vector3[31];

		private Vector3[] newROMPositionBuffer = new Vector3[31];

		private Quaternion[] oldROMRotationBuffer = new Quaternion[31];

		private Quaternion[] newROMRotationBuffer = new Quaternion[31];

		private Vector3[] bonePositionBuffer = new Vector3[31];

		private Quaternion[] boneRotationBuffer = new Quaternion[31];

		private static readonly Quaternion rightFlipAngle = Quaternion.AngleAxis(180f, Vector3.right);

		public bool isActive => skeletonAction.GetActive();

		public float[] fingerCurls => skeletonAction.GetFingerCurls();

		public float thumbCurl => skeletonAction.GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.thumb);

		public float indexCurl => skeletonAction.GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.index);

		public float middleCurl => skeletonAction.GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.middle);

		public float ringCurl => skeletonAction.GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.ring);

		public float pinkyCurl => skeletonAction.GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.pinky);

		public Transform root => bones[0];

		public Transform wrist => bones[1];

		public Transform indexMetacarpal => bones[6];

		public Transform indexProximal => bones[7];

		public Transform indexMiddle => bones[8];

		public Transform indexDistal => bones[9];

		public Transform indexTip => bones[10];

		public Transform middleMetacarpal => bones[11];

		public Transform middleProximal => bones[12];

		public Transform middleMiddle => bones[13];

		public Transform middleDistal => bones[14];

		public Transform middleTip => bones[15];

		public Transform pinkyMetacarpal => bones[21];

		public Transform pinkyProximal => bones[22];

		public Transform pinkyMiddle => bones[23];

		public Transform pinkyDistal => bones[24];

		public Transform pinkyTip => bones[25];

		public Transform ringMetacarpal => bones[16];

		public Transform ringProximal => bones[17];

		public Transform ringMiddle => bones[18];

		public Transform ringDistal => bones[19];

		public Transform ringTip => bones[20];

		public Transform thumbMetacarpal => bones[2];

		public Transform thumbProximal => bones[2];

		public Transform thumbMiddle => bones[3];

		public Transform thumbDistal => bones[4];

		public Transform thumbTip => bones[5];

		public Transform thumbAux => bones[26];

		public Transform indexAux => bones[27];

		public Transform middleAux => bones[28];

		public Transform ringAux => bones[29];

		public Transform pinkyAux => bones[30];

		public Transform[] proximals
		{
			get;
			protected set;
		}

		public Transform[] middles
		{
			get;
			protected set;
		}

		public Transform[] distals
		{
			get;
			protected set;
		}

		public Transform[] tips
		{
			get;
			protected set;
		}

		public Transform[] auxs
		{
			get;
			protected set;
		}

		public EVRSkeletalTrackingLevel skeletalTrackingLevel => skeletonAction.skeletalTrackingLevel;

		public bool isBlending => blendRoutine != null;

		public SteamVR_ActionSet actionSet => skeletonAction.actionSet;

		public SteamVR_ActionDirections direction => skeletonAction.direction;

		protected virtual void Awake()
		{
			AssignBonesArray();
			proximals = new Transform[5]
			{
				thumbProximal,
				indexProximal,
				middleProximal,
				ringProximal,
				pinkyProximal
			};
			middles = new Transform[5]
			{
				thumbMiddle,
				indexMiddle,
				middleMiddle,
				ringMiddle,
				pinkyMiddle
			};
			distals = new Transform[5]
			{
				thumbDistal,
				indexDistal,
				middleDistal,
				ringDistal,
				pinkyDistal
			};
			tips = new Transform[5]
			{
				thumbTip,
				indexTip,
				middleTip,
				ringTip,
				pinkyTip
			};
			auxs = new Transform[5]
			{
				thumbAux,
				indexAux,
				middleAux,
				ringAux,
				pinkyAux
			};
			CheckSkeletonAction();
		}

		protected virtual void CheckSkeletonAction()
		{
			if (skeletonAction == null)
			{
				skeletonAction = SteamVR_Input.GetAction<SteamVR_Action_Skeleton>("Skeleton" + inputSource);
			}
		}

		protected virtual void AssignBonesArray()
		{
			bones = skeletonRoot.GetComponentsInChildren<Transform>();
		}

		protected virtual void OnEnable()
		{
			CheckSkeletonAction();
			SteamVR_Input.onSkeletonsUpdated += SteamVR_Input_OnSkeletonsUpdated;
			if (skeletonAction != null)
			{
				skeletonAction.onDeviceConnectedChanged += OnDeviceConnectedChanged;
				skeletonAction.onTrackingChanged += OnTrackingChanged;
			}
		}

		protected virtual void OnDisable()
		{
			SteamVR_Input.onSkeletonsUpdated -= SteamVR_Input_OnSkeletonsUpdated;
			if (skeletonAction != null)
			{
				skeletonAction.onDeviceConnectedChanged -= OnDeviceConnectedChanged;
				skeletonAction.onTrackingChanged -= OnTrackingChanged;
			}
		}

		private void OnDeviceConnectedChanged(SteamVR_Action_Skeleton fromAction, bool deviceConnected)
		{
			if (onConnectedChanged != null)
			{
				onConnectedChanged.Invoke(this, inputSource, deviceConnected);
			}
			if (onConnectedChangedEvent != null)
			{
				onConnectedChangedEvent(this, inputSource, deviceConnected);
			}
		}

		private void OnTrackingChanged(SteamVR_Action_Skeleton fromAction, ETrackingResult trackingState)
		{
			if (onTrackingChanged != null)
			{
				onTrackingChanged.Invoke(this, inputSource, trackingState);
			}
			if (onTrackingChangedEvent != null)
			{
				onTrackingChangedEvent(this, inputSource, trackingState);
			}
		}

		protected virtual void SteamVR_Input_OnSkeletonsUpdated(bool skipSendingEvents)
		{
			UpdateSkeleton();
		}

		protected virtual void UpdateSkeleton()
		{
			if (skeletonAction == null || !skeletonAction.active)
			{
				return;
			}
			if (updatePose)
			{
				UpdatePose();
			}
			if (blendPoser != null && skeletonBlend < 1f)
			{
				if (blendSnapshot == null)
				{
					blendSnapshot = blendPoser.GetBlendedPose(this);
				}
				blendSnapshot.CopyFrom(blendPoser.GetBlendedPose(this));
			}
			if (rangeOfMotionBlendRoutine == null)
			{
				EVRSkeletalMotionRange? eVRSkeletalMotionRange = temporaryRangeOfMotion;
				if (eVRSkeletalMotionRange.HasValue)
				{
					skeletonAction.SetRangeOfMotion(temporaryRangeOfMotion.Value);
				}
				else
				{
					skeletonAction.SetRangeOfMotion(rangeOfMotion);
				}
				UpdateSkeletonTransforms();
			}
		}

		public void SetTemporaryRangeOfMotion(EVRSkeletalMotionRange newRangeOfMotion, float blendOverSeconds = 0.1f)
		{
			if (rangeOfMotion != newRangeOfMotion || temporaryRangeOfMotion != newRangeOfMotion)
			{
				TemporaryRangeOfMotionBlend(newRangeOfMotion, blendOverSeconds);
			}
		}

		public void ResetTemporaryRangeOfMotion(float blendOverSeconds = 0.1f)
		{
			ResetTemporaryRangeOfMotionBlend(blendOverSeconds);
		}

		public void SetRangeOfMotion(EVRSkeletalMotionRange newRangeOfMotion, float blendOverSeconds = 0.1f)
		{
			if (rangeOfMotion != newRangeOfMotion)
			{
				RangeOfMotionBlend(newRangeOfMotion, blendOverSeconds);
			}
		}

		public void BlendToSkeleton(float overTime = 0.1f)
		{
			blendSnapshot = blendPoser.GetBlendedPose(this);
			blendPoser = null;
			BlendTo(1f, overTime);
		}

		public void BlendToPoser(SteamVR_Skeleton_Poser poser, float overTime = 0.1f)
		{
			if (!(poser == null))
			{
				blendPoser = poser;
				BlendTo(0f, overTime);
			}
		}

		public void BlendToAnimation(float overTime = 0.1f)
		{
			BlendTo(0f, overTime);
		}

		public void BlendTo(float blendToAmount, float overTime)
		{
			if (blendRoutine != null)
			{
				StopCoroutine(blendRoutine);
			}
			if (base.gameObject.activeInHierarchy)
			{
				blendRoutine = StartCoroutine(DoBlendRoutine(blendToAmount, overTime));
			}
		}

		protected IEnumerator DoBlendRoutine(float blendToAmount, float overTime)
		{
			float startTime = Time.time;
			float endTime = startTime + overTime;
			float startAmount = skeletonBlend;
			while (Time.time < endTime)
			{
				yield return null;
				skeletonBlend = Mathf.Lerp(startAmount, blendToAmount, (Time.time - startTime) / overTime);
			}
			skeletonBlend = blendToAmount;
			blendRoutine = null;
		}

		protected void RangeOfMotionBlend(EVRSkeletalMotionRange newRangeOfMotion, float blendOverSeconds)
		{
			if (rangeOfMotionBlendRoutine != null)
			{
				StopCoroutine(rangeOfMotionBlendRoutine);
			}
			EVRSkeletalMotionRange oldRangeOfMotion = rangeOfMotion;
			rangeOfMotion = newRangeOfMotion;
			if (base.gameObject.activeInHierarchy)
			{
				rangeOfMotionBlendRoutine = StartCoroutine(DoRangeOfMotionBlend(oldRangeOfMotion, newRangeOfMotion, blendOverSeconds));
			}
		}

		protected void TemporaryRangeOfMotionBlend(EVRSkeletalMotionRange newRangeOfMotion, float blendOverSeconds)
		{
			if (rangeOfMotionBlendRoutine != null)
			{
				StopCoroutine(rangeOfMotionBlendRoutine);
			}
			EVRSkeletalMotionRange value = rangeOfMotion;
			EVRSkeletalMotionRange? eVRSkeletalMotionRange = temporaryRangeOfMotion;
			if (eVRSkeletalMotionRange.HasValue)
			{
				value = temporaryRangeOfMotion.Value;
			}
			temporaryRangeOfMotion = newRangeOfMotion;
			if (base.gameObject.activeInHierarchy)
			{
				rangeOfMotionBlendRoutine = StartCoroutine(DoRangeOfMotionBlend(value, newRangeOfMotion, blendOverSeconds));
			}
		}

		protected void ResetTemporaryRangeOfMotionBlend(float blendOverSeconds)
		{
			EVRSkeletalMotionRange? eVRSkeletalMotionRange = temporaryRangeOfMotion;
			if (eVRSkeletalMotionRange.HasValue)
			{
				if (rangeOfMotionBlendRoutine != null)
				{
					StopCoroutine(rangeOfMotionBlendRoutine);
				}
				EVRSkeletalMotionRange value = temporaryRangeOfMotion.Value;
				EVRSkeletalMotionRange newRangeOfMotion = rangeOfMotion;
				temporaryRangeOfMotion = null;
				if (base.gameObject.activeInHierarchy)
				{
					rangeOfMotionBlendRoutine = StartCoroutine(DoRangeOfMotionBlend(value, newRangeOfMotion, blendOverSeconds));
				}
			}
		}

		protected IEnumerator DoRangeOfMotionBlend(EVRSkeletalMotionRange oldRangeOfMotion, EVRSkeletalMotionRange newRangeOfMotion, float overTime)
		{
			float startTime = Time.time;
			float endTime = startTime + overTime;
			while (Time.time < endTime)
			{
				yield return null;
				float lerp = (Time.time - startTime) / overTime;
				if (skeletonBlend > 0f)
				{
					skeletonAction.SetRangeOfMotion(oldRangeOfMotion);
					skeletonAction.UpdateValueWithoutEvents();
					CopyBonePositions(oldROMPositionBuffer);
					CopyBoneRotations(oldROMRotationBuffer);
					skeletonAction.SetRangeOfMotion(newRangeOfMotion);
					skeletonAction.UpdateValueWithoutEvents();
					CopyBonePositions(newROMPositionBuffer);
					CopyBoneRotations(newROMRotationBuffer);
					for (int i = 0; i < bones.Length; i++)
					{
						if (bones[i] == null || !SteamVR_Utils.IsValid(newROMRotationBuffer[i]) || !SteamVR_Utils.IsValid(oldROMRotationBuffer[i]))
						{
							continue;
						}
						Vector3 vector = Vector3.Lerp(oldROMPositionBuffer[i], newROMPositionBuffer[i], lerp);
						Quaternion quaternion = Quaternion.Lerp(oldROMRotationBuffer[i], newROMRotationBuffer[i], lerp);
						if (skeletonBlend < 1f)
						{
							if (blendPoser != null)
							{
								SetBonePosition(i, Vector3.Lerp(blendSnapshot.bonePositions[i], vector, skeletonBlend));
								SetBoneRotation(i, Quaternion.Lerp(blendSnapshot.boneRotations[i], quaternion, skeletonBlend));
							}
							else
							{
								SetBonePosition(i, Vector3.Lerp(bones[i].localPosition, vector, skeletonBlend));
								SetBoneRotation(i, Quaternion.Lerp(bones[i].localRotation, quaternion, skeletonBlend));
							}
						}
						else
						{
							SetBonePosition(i, vector);
							SetBoneRotation(i, quaternion);
						}
					}
				}
				if (onBoneTransformsUpdated != null)
				{
					onBoneTransformsUpdated.Invoke(this, inputSource);
				}
				if (onBoneTransformsUpdatedEvent != null)
				{
					onBoneTransformsUpdatedEvent(this, inputSource);
				}
			}
			rangeOfMotionBlendRoutine = null;
		}

		protected virtual void UpdateSkeletonTransforms()
		{
			CopyBonePositions(bonePositionBuffer);
			CopyBoneRotations(boneRotationBuffer);
			if (skeletonBlend <= 0f)
			{
				if (blendPoser != null)
				{
					SteamVR_Skeleton_Pose_Hand hand = blendPoser.skeletonMainPose.GetHand(inputSource);
					for (int i = 0; i < bones.Length; i++)
					{
						if (!(bones[i] == null))
						{
							if ((i == 1 && hand.ignoreWristPoseData) || (i == 0 && hand.ignoreRootPoseData))
							{
								SetBonePosition(i, bonePositionBuffer[i]);
								SetBoneRotation(i, boneRotationBuffer[i]);
							}
							else
							{
								SetBonePosition(i, blendSnapshot.bonePositions[i]);
								SetBoneRotation(i, blendSnapshot.boneRotations[i]);
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < bones.Length; j++)
					{
						SetBonePosition(j, blendSnapshot.bonePositions[j]);
						SetBoneRotation(j, blendSnapshot.boneRotations[j]);
					}
				}
			}
			else if (skeletonBlend >= 1f)
			{
				for (int k = 0; k < bones.Length; k++)
				{
					if (!(bones[k] == null))
					{
						SetBonePosition(k, bonePositionBuffer[k]);
						SetBoneRotation(k, boneRotationBuffer[k]);
					}
				}
			}
			else
			{
				for (int l = 0; l < bones.Length; l++)
				{
					if (bones[l] == null)
					{
						continue;
					}
					if (blendPoser != null)
					{
						SteamVR_Skeleton_Pose_Hand hand2 = blendPoser.skeletonMainPose.GetHand(inputSource);
						if ((l == 1 && hand2.ignoreWristPoseData) || (l == 0 && hand2.ignoreRootPoseData))
						{
							SetBonePosition(l, bonePositionBuffer[l]);
							SetBoneRotation(l, boneRotationBuffer[l]);
						}
						else
						{
							SetBonePosition(l, Vector3.Lerp(blendSnapshot.bonePositions[l], bonePositionBuffer[l], skeletonBlend));
							SetBoneRotation(l, Quaternion.Lerp(blendSnapshot.boneRotations[l], boneRotationBuffer[l], skeletonBlend));
						}
					}
					else if (blendSnapshot == null)
					{
						SetBonePosition(l, Vector3.Lerp(bones[l].localPosition, bonePositionBuffer[l], skeletonBlend));
						SetBoneRotation(l, Quaternion.Lerp(bones[l].localRotation, boneRotationBuffer[l], skeletonBlend));
					}
					else
					{
						SetBonePosition(l, Vector3.Lerp(blendSnapshot.bonePositions[l], bonePositionBuffer[l], skeletonBlend));
						SetBoneRotation(l, Quaternion.Lerp(blendSnapshot.boneRotations[l], boneRotationBuffer[l], skeletonBlend));
					}
				}
			}
			if (onBoneTransformsUpdated != null)
			{
				onBoneTransformsUpdated.Invoke(this, inputSource);
			}
			if (onBoneTransformsUpdatedEvent != null)
			{
				onBoneTransformsUpdatedEvent(this, inputSource);
			}
		}

		protected virtual void SetBonePosition(int boneIndex, Vector3 localPosition)
		{
			if (!onlySetRotations)
			{
				bones[boneIndex].localPosition = localPosition;
			}
		}

		protected virtual void SetBoneRotation(int boneIndex, Quaternion localRotation)
		{
			bones[boneIndex].localRotation = localRotation;
		}

		public virtual Transform GetBone(int joint)
		{
			if (bones == null || bones.Length == 0)
			{
				Awake();
			}
			return bones[joint];
		}

		public Vector3 GetBonePosition(int joint, bool local = false)
		{
			if (local)
			{
				return bones[joint].localPosition;
			}
			return bones[joint].position;
		}

		public Quaternion GetBoneRotation(int joint, bool local = false)
		{
			if (local)
			{
				return bones[joint].localRotation;
			}
			return bones[joint].rotation;
		}

		protected void CopyBonePositions(Vector3[] positionBuffer)
		{
			Vector3[] bonePositions = skeletonAction.GetBonePositions();
			if (mirroring == MirrorType.LeftToRight || mirroring == MirrorType.RightToLeft)
			{
				for (int i = 0; i < positionBuffer.Length; i++)
				{
					MirrorBonePosition(ref bonePositions[i], ref positionBuffer[i], i);
				}
			}
			else
			{
				bonePositions.CopyTo(positionBuffer, 0);
			}
		}

		protected void CopyBoneRotations(Quaternion[] rotationBuffer)
		{
			Quaternion[] boneRotations = skeletonAction.GetBoneRotations();
			if (mirroring == MirrorType.LeftToRight || mirroring == MirrorType.RightToLeft)
			{
				for (int i = 0; i < rotationBuffer.Length; i++)
				{
					MirrorBoneRotation(ref boneRotations[i], ref rotationBuffer[i], i);
				}
			}
			else
			{
				boneRotations.CopyTo(rotationBuffer, 0);
			}
		}

		public static void MirrorBonePosition(ref Vector3 source, ref Vector3 dest, int boneIndex)
		{
			if (boneIndex == 1 || IsMetacarpal(boneIndex))
			{
				dest.x = 0f - source.x;
				dest.y = source.y;
				dest.z = source.z;
			}
			else if (boneIndex != 0)
			{
				dest.x = 0f - source.x;
				dest.y = 0f - source.y;
				dest.z = 0f - source.z;
			}
			else
			{
				dest = source;
			}
		}

		public static void MirrorBoneRotation(ref Quaternion source, ref Quaternion dest, int boneIndex)
		{
			if (boneIndex == 1)
			{
				dest.x = source.x;
				dest.y = source.y * -1f;
				dest.z = source.z * -1f;
				dest.w = source.w;
			}
			else if (IsMetacarpal(boneIndex))
			{
				dest = rightFlipAngle * source;
			}
			else
			{
				dest = source;
			}
		}

		protected virtual void UpdatePose()
		{
			if (skeletonAction == null)
			{
				return;
			}
			Vector3 position = skeletonAction.GetLocalPosition();
			Quaternion quaternion = skeletonAction.GetLocalRotation();
			if (origin == null)
			{
				if (base.transform.parent != null)
				{
					position = base.transform.parent.TransformPoint(position);
					quaternion = base.transform.parent.rotation * quaternion;
				}
			}
			else
			{
				position = origin.TransformPoint(position);
				quaternion = origin.rotation * quaternion;
			}
			if (skeletonAction.poseChanged)
			{
				if (onTransformChanged != null)
				{
					onTransformChanged.Invoke(this, inputSource);
				}
				if (onTransformChangedEvent != null)
				{
					onTransformChangedEvent(this, inputSource);
				}
			}
			base.transform.position = position;
			base.transform.rotation = quaternion;
			if (onTransformUpdated != null)
			{
				onTransformUpdated.Invoke(this, inputSource);
			}
		}

		public void ForceToReferencePose(EVRSkeletalReferencePose referencePose)
		{
			bool flag = false;
			if (Application.isEditor && !Application.isPlaying)
			{
				flag = SteamVR.InitializeTemporarySession(initInput: true);
				Awake();
				skeletonAction.actionSet.Activate();
				SteamVR_ActionSet_Manager.UpdateActionStates(force: true);
				skeletonAction.UpdateValueWithoutEvents();
			}
			if (!skeletonAction.active)
			{
				Debug.LogError("<b>[SteamVR Input]</b> Please turn on your " + inputSource.ToString() + " controller and ensure SteamVR is open.");
				return;
			}
			SteamVR_Utils.RigidTransform[] referenceTransforms = skeletonAction.GetReferenceTransforms(EVRSkeletalTransformSpace.Parent, referencePose);
			if (referenceTransforms == null || referenceTransforms.Length == 0)
			{
				Debug.LogError("<b>[SteamVR Input]</b> Unable to get the reference transform for " + inputSource.ToString() + ". Please make sure SteamVR is open and both controllers are connected.");
			}
			for (int i = 0; i < referenceTransforms.Length; i++)
			{
				bones[i].localPosition = referenceTransforms[i].pos;
				bones[i].localRotation = referenceTransforms[i].rot;
			}
			if (flag)
			{
				SteamVR.ExitTemporarySession();
			}
		}

		protected static bool IsMetacarpal(int boneIndex)
		{
			return boneIndex == 6 || boneIndex == 11 || boneIndex == 16 || boneIndex == 21 || boneIndex == 2;
		}
	}
}
