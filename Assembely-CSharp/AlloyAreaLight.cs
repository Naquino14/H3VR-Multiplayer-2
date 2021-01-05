using System;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
[AddComponentMenu("Alloy/Alloy Area Light")]
public class AlloyAreaLight : MonoBehaviour
{
	private const float c_minimumLightSize = 1E-05f;

	private const float c_minimumLightIntensity = 0.01f;

	[HideInInspector]
	public Texture2D DefaultSpotLightCookie;

	[FormerlySerializedAs("m_size")]
	[SerializeField]
	private float m_radius;

	[SerializeField]
	private float m_length;

	[SerializeField]
	private bool m_hasSpecularHightlight = true;

	private Light m_light;

	private Color m_lastColor;

	private float m_lastIntensity;

	private float m_lastRange;

	private Light Light
	{
		get
		{
			if (m_light == null)
			{
				m_light = GetComponent<Light>();
			}
			return m_light;
		}
	}

	public float Radius
	{
		get
		{
			return m_radius;
		}
		set
		{
			if (m_radius != value)
			{
				m_radius = value;
				UpdateBinding();
			}
		}
	}

	public float Length
	{
		get
		{
			return m_length;
		}
		set
		{
			if (m_length != value)
			{
				m_length = value;
				UpdateBinding();
			}
		}
	}

	public bool HasSpecularHighlight
	{
		get
		{
			return m_hasSpecularHightlight;
		}
		set
		{
			if (m_hasSpecularHightlight != value)
			{
				m_hasSpecularHightlight = value;
				UpdateBinding();
			}
		}
	}

	[Obsolete("Please use Unity Light component's \"color\" field.")]
	public Color Color
	{
		get
		{
			return Light.color;
		}
		set
		{
			Light.color = value;
		}
	}

	[Obsolete("Please use Unity Light component's \"intensity\" field.")]
	public float Intensity
	{
		get
		{
			return Light.intensity;
		}
		set
		{
			Light.intensity = value;
		}
	}

	[Obsolete("No longer used. Please remove all references to it.")]
	public bool IsAnimated
	{
		get;
		set;
	}

	private void Reset()
	{
		m_hasSpecularHightlight = true;
		m_radius = 0f;
		m_length = 0f;
		m_lastColor = Color.black;
		m_lastIntensity = 0f;
		m_lastRange = 0f;
		UpdateBinding();
	}

	private void LateUpdate()
	{
		Light light = Light;
		if (light.color != m_lastColor || light.intensity != m_lastIntensity || light.range != m_lastRange)
		{
			UpdateBinding();
		}
	}

	public void UpdateBinding()
	{
		Light light = Light;
		Color color = light.color;
		float intensity = light.intensity;
		float range = light.range;
		if (light.type == LightType.Directional)
		{
			m_radius = Mathf.Clamp01(m_radius);
			color.a = 10f * m_radius;
		}
		else
		{
			float num = range;
			m_radius = Mathf.Clamp(m_radius, 0f, num);
			color.a = Mathf.Min(0.999f, m_radius / num);
			if (light.type == LightType.Point)
			{
				float num2 = 2f * range;
				m_length = Mathf.Clamp(m_length, 0f, num2);
				color.a += Mathf.Ceil(1000f * Mathf.Min(1f, m_length / num2));
			}
		}
		color.a = Mathf.Max(1E-05f, color.a);
		color.a *= ((!m_hasSpecularHightlight) ? (-1f) : 1f);
		color.a /= Mathf.Max(intensity, 0.01f);
		light.color = color;
		m_lastColor = color;
		m_lastIntensity = intensity;
		m_lastRange = range;
	}
}
