using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Recoil Profile", menuName = "Firearms/RecoilProfile", order = 0)]
	public class FVRFireArmRecoilProfile : ScriptableObject
	{
		[Header("Recoil Impulse Params")]
		public float VerticalRotPerShot;

		public float HorizontalRotPerShot;

		public float MaxVerticalRot;

		public float MaxHorizontalRot;

		public float VerticalRotRecovery;

		public float HorizontalRotRecovery;

		public float ZLinearPerShot;

		public float ZLinearMax;

		public float ZLinearRecovery;

		public float XYLinearPerShot;

		public float XYLinearMax;

		public float XYLinearRecovery;

		[Header("Bipodded Params")]
		public float VerticalRotPerShot_Bipodded;

		public float HorizontalRotPerShot_Bipodded;

		public float MaxVerticalRot_Bipodded;

		public float MaxHorizontalRot_Bipodded;

		[Header("Recoil Recovery Params")]
		public Vector4 RecoveryStabilizationFactors_Foregrip = new Vector4(1f, 1f, 0.8f, 0.8f);

		public Vector4 RecoveryStabilizationFactors_Twohand = new Vector4(0.5f, 0.8f, 0.3f, 0.75f);

		public Vector4 RecoveryStabilizationFactors_None = new Vector4(0.25f, 0.5f, 0.25f, 0.25f);

		[Header("Weapon Dimension/Mass Related Params")]
		public float MassDriftIntensity = 0.1f;

		public Vector4 MassDriftFactors = new Vector4(0.25f, 0.6f, 1f, 0.1f);

		public float MaxMassDriftMagnitude = 5f;

		public float MaxMassMaxRotation = 20f;

		public float MassDriftRecoveryFactor = 5f;
	}
}
