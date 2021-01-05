using UnityEngine;

namespace FistVR
{
	public class PMat : MonoBehaviour
	{
		public PMaterialDefinition Def;

		public MatDef MatDef;

		private float m_condition = 1f;

		public float Condition
		{
			get
			{
				return m_condition;
			}
			set
			{
				m_condition = Mathf.Clamp(value, 0f, 1f);
			}
		}

		public float GetYieldStrength()
		{
			return Mathf.Lerp(0.001f, Def.yieldStrength, Condition);
		}

		public float GetRoughness()
		{
			return Mathf.Lerp(0.001f, Def.roughness, Condition);
		}

		public float GetStiffness()
		{
			return Mathf.Lerp(Def.stiffness * 0.2f, Def.stiffness, Condition);
		}

		public float GetDensity()
		{
			return Mathf.Lerp(Def.density * 0.2f, Def.density, Condition);
		}

		public float GetBounciness()
		{
			return Mathf.Lerp(Def.bounciness * 0.3f, Def.bounciness, Condition);
		}

		public float GetToughness()
		{
			return Mathf.Lerp(0.001f, Def.toughness, Condition);
		}
	}
}
