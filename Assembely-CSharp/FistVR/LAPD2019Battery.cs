using UnityEngine;

namespace FistVR
{
	public class LAPD2019Battery : FVRPhysicalObject
	{
		private float m_energy = 1f;

		public Renderer LED_FauxBattery_Side;

		public Renderer LED_FauxBattery_Under;

		[ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
		public Color Color_Emissive_Red;

		[ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
		public Color Color_Emissive_Green;

		public void SetEnergy(float f)
		{
			m_energy = f;
		}

		public float GetEnergy()
		{
			return m_energy;
		}

		private void Update()
		{
			LED_FauxBattery_Side.material.SetColor("_Color", Color.Lerp(Color_Emissive_Red, Color_Emissive_Green, m_energy));
		}

		public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
		{
			GameObject gameObject = base.DuplicateFromSpawnLock(hand);
			LAPD2019Battery component = gameObject.GetComponent<LAPD2019Battery>();
			component.SetEnergy(m_energy);
			return gameObject;
		}
	}
}
