using UnityEngine;

namespace FistVR
{
	public class MuzzleDevice : FVRFireArmAttachment
	{
		public Transform Muzzle;

		[Header("MuzzleEffects")]
		public bool ForcesEffectSize;

		public FVRFireArmMechanicalAccuracyClass MechanicalAccuracy;

		public MuzzleEffect[] MuzzleEffects;

		private float m_mechanicalAccuracy;

		private float m_dropmult;

		private float m_driftmult;

		public float GetMechanicalAccuracy()
		{
			return m_mechanicalAccuracy;
		}

		public float GetDropMult(FVRFireArm f)
		{
			return m_dropmult;
		}

		public Vector2 GetDriftMult(FVRFireArm f)
		{
			string text = "empty";
			if (f.ObjectWrapper != null)
			{
				text = f.ObjectWrapper.ItemID;
			}
			float x = ObjectIDsToFloatHash(ObjectWrapper.ItemID, text);
			float y = ObjectIDsToFloatHash(text, ObjectWrapper.ItemID);
			return new Vector2(x, y) * m_driftmult;
		}

		protected override void Awake()
		{
			base.Awake();
			m_mechanicalAccuracy = AM.GetFireArmMechanicalSpread(MechanicalAccuracy);
			m_dropmult = AM.GetDropMult(MechanicalAccuracy);
			m_driftmult = AM.GetDriftMult(MechanicalAccuracy);
		}

		public virtual void OnShot(FVRFireArm f, FVRTailSoundClass tailClass)
		{
		}

		public float ObjectIDsToFloatHash(string objectID0, string objectID1)
		{
			int num = 17;
			num = num * 31 + objectID0.GetDeterministicHashCode();
			num = num * 31 + objectID1.GetDeterministicHashCode();
			int num2 = num % 10000;
			num2 = ((num2 >= 0) ? num2 : (num2 + 10000));
			double num3 = (double)num2 / 5000.0 - 1.0;
			return (float)num3;
		}
	}
}
