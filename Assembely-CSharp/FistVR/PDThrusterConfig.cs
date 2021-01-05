using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New PD Thruster Config", menuName = "PancakeDrone/Thruster Definition", order = 0)]
	public class PDThrusterConfig : ScriptableObject
	{
		public float MaxThrustForce = 10f;

		public float ThrustEngagementResponseSpeedUp = 2f;

		public float ThrustEngagementResponseSpeedDown = 10f;

		public AnimationCurve ThrustResponseCurve;

		public float HeatGenerationBase = 100f;

		public AnimationCurve LoadToHeatCurve;

		public AnimationCurve HeatToMaxLoadCurve;

		public AnimationCurve IntegrityToMaxLoadCurve;

		public AnimationCurve IntegrityToControlFailureCurve;
	}
}
