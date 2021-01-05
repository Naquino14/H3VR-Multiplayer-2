using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Behaviour_SkeletonCustom : SteamVR_Behaviour_Skeleton
	{
		[SerializeField]
		protected Transform _wrist;

		[SerializeField]
		protected Transform _thumbMetacarpal;

		[SerializeField]
		protected Transform _thumbProximal;

		[SerializeField]
		protected Transform _thumbMiddle;

		[SerializeField]
		protected Transform _thumbDistal;

		[SerializeField]
		protected Transform _thumbTip;

		[SerializeField]
		protected Transform _thumbAux;

		[SerializeField]
		protected Transform _indexMetacarpal;

		[SerializeField]
		protected Transform _indexProximal;

		[SerializeField]
		protected Transform _indexMiddle;

		[SerializeField]
		protected Transform _indexDistal;

		[SerializeField]
		protected Transform _indexTip;

		[SerializeField]
		protected Transform _indexAux;

		[SerializeField]
		protected Transform _middleMetacarpal;

		[SerializeField]
		protected Transform _middleProximal;

		[SerializeField]
		protected Transform _middleMiddle;

		[SerializeField]
		protected Transform _middleDistal;

		[SerializeField]
		protected Transform _middleTip;

		[SerializeField]
		protected Transform _middleAux;

		[SerializeField]
		protected Transform _ringMetacarpal;

		[SerializeField]
		protected Transform _ringProximal;

		[SerializeField]
		protected Transform _ringMiddle;

		[SerializeField]
		protected Transform _ringDistal;

		[SerializeField]
		protected Transform _ringTip;

		[SerializeField]
		protected Transform _ringAux;

		[SerializeField]
		protected Transform _pinkyMetacarpal;

		[SerializeField]
		protected Transform _pinkyProximal;

		[SerializeField]
		protected Transform _pinkyMiddle;

		[SerializeField]
		protected Transform _pinkyDistal;

		[SerializeField]
		protected Transform _pinkyTip;

		[SerializeField]
		protected Transform _pinkyAux;

		protected override void AssignBonesArray()
		{
			bones[1] = _wrist;
			bones[2] = _thumbProximal;
			bones[3] = _thumbMiddle;
			bones[4] = _thumbDistal;
			bones[5] = _thumbTip;
			bones[26] = _thumbAux;
			bones[7] = _indexProximal;
			bones[8] = _indexMiddle;
			bones[9] = _indexDistal;
			bones[10] = _indexTip;
			bones[27] = _indexAux;
			bones[12] = _middleProximal;
			bones[13] = _middleMiddle;
			bones[14] = _middleDistal;
			bones[15] = _middleTip;
			bones[28] = _middleAux;
			bones[17] = _ringProximal;
			bones[18] = _ringMiddle;
			bones[19] = _ringDistal;
			bones[20] = _ringTip;
			bones[29] = _ringAux;
			bones[22] = _pinkyProximal;
			bones[23] = _pinkyMiddle;
			bones[24] = _pinkyDistal;
			bones[25] = _pinkyTip;
			bones[30] = _pinkyAux;
		}
	}
}
