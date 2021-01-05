using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class OmniShape : MonoBehaviour, IFVRDamageable
	{
		public enum ShapeState
		{
			Growing,
			Shootable,
			Shrinking
		}

		private bool m_isDestroyed;

		private OmniSpawner_Shapes m_spawner;

		private bool m_isCorrectTarget;

		public Renderer Renderer;

		public OmniSpawnDef_Shape.OmniShapeColor Color;

		public List<Color> DisplayColors = new List<Color>
		{
			new Color(1f, 0f, 0f, 1f),
			new Color(1f, 0.6f, 0f, 1f),
			new Color(1f, 1f, 0f, 1f),
			new Color(0.2f, 0.8f, 0.2f, 1f),
			new Color(0f, 0.4f, 1f, 1f),
			new Color(0.6f, 0f, 0.6f, 1f),
			new Color(1f, 0.07f, 0.58f, 1f),
			new Color(0.63f, 0.32f, 0.16f, 1f)
		};

		private float m_tick;

		private ShapeState m_state;

		public void Init(OmniSpawner_Shapes spawner, bool isCorrectTarget, OmniSpawnDef_Shape.OmniShapeColor color)
		{
			m_spawner = spawner;
			m_isCorrectTarget = isCorrectTarget;
			m_state = ShapeState.Growing;
			Color = color;
			Renderer.material.SetColor("_Color", DisplayColors[(int)color]);
		}

		public void Damage(Damage dam)
		{
			if (m_state == ShapeState.Shootable && !m_isDestroyed)
			{
				m_isDestroyed = true;
				m_spawner.ShapeStruck(m_isCorrectTarget);
				TurnOff();
			}
		}

		public void Update()
		{
			switch (m_state)
			{
			case ShapeState.Growing:
			{
				m_tick += Time.deltaTime * 4f;
				bool flag2 = false;
				if (m_tick > 1f)
				{
					m_tick = 1f;
					flag2 = true;
				}
				base.transform.localScale = Vector3.Lerp(Vector3.one * 0.02f, Vector3.one, m_tick);
				if (flag2)
				{
					m_state = ShapeState.Shootable;
				}
				break;
			}
			case ShapeState.Shrinking:
			{
				m_tick -= Time.deltaTime * 6f;
				bool flag = false;
				if (m_tick <= 0f)
				{
					m_tick = 0f;
					flag = true;
				}
				base.transform.localScale = Vector3.Lerp(Vector3.one * 0.02f, Vector3.one, m_tick);
				if (flag)
				{
					Object.Destroy(Renderer.material);
					Object.Destroy(base.gameObject);
				}
				break;
			}
			}
		}

		public void TurnOff()
		{
			m_state = ShapeState.Shrinking;
		}
	}
}
