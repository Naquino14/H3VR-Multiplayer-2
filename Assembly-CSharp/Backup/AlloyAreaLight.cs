// Decompiled with JetBrains decompiler
// Type: AlloyAreaLight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
[RequireComponent(typeof (Light))]
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
      if ((UnityEngine.Object) this.m_light == (UnityEngine.Object) null)
        this.m_light = this.GetComponent<Light>();
      return this.m_light;
    }
  }

  public float Radius
  {
    get => this.m_radius;
    set
    {
      if ((double) this.m_radius == (double) value)
        return;
      this.m_radius = value;
      this.UpdateBinding();
    }
  }

  public float Length
  {
    get => this.m_length;
    set
    {
      if ((double) this.m_length == (double) value)
        return;
      this.m_length = value;
      this.UpdateBinding();
    }
  }

  public bool HasSpecularHighlight
  {
    get => this.m_hasSpecularHightlight;
    set
    {
      if (this.m_hasSpecularHightlight == value)
        return;
      this.m_hasSpecularHightlight = value;
      this.UpdateBinding();
    }
  }

  private void Reset()
  {
    this.m_hasSpecularHightlight = true;
    this.m_radius = 0.0f;
    this.m_length = 0.0f;
    this.m_lastColor = Color.black;
    this.m_lastIntensity = 0.0f;
    this.m_lastRange = 0.0f;
    this.UpdateBinding();
  }

  private void LateUpdate()
  {
    Light light = this.Light;
    if (!(light.color != this.m_lastColor) && (double) light.intensity == (double) this.m_lastIntensity && (double) light.range == (double) this.m_lastRange)
      return;
    this.UpdateBinding();
  }

  public void UpdateBinding()
  {
    Light light = this.Light;
    Color color = light.color;
    float intensity = light.intensity;
    float range = light.range;
    if (light.type == LightType.Directional)
    {
      this.m_radius = Mathf.Clamp01(this.m_radius);
      color.a = 10f * this.m_radius;
    }
    else
    {
      float max1 = range;
      this.m_radius = Mathf.Clamp(this.m_radius, 0.0f, max1);
      color.a = Mathf.Min(0.999f, this.m_radius / max1);
      if (light.type == LightType.Point)
      {
        float max2 = 2f * range;
        this.m_length = Mathf.Clamp(this.m_length, 0.0f, max2);
        color.a += Mathf.Ceil(1000f * Mathf.Min(1f, this.m_length / max2));
      }
    }
    color.a = Mathf.Max(1E-05f, color.a);
    color.a *= !this.m_hasSpecularHightlight ? -1f : 1f;
    color.a /= Mathf.Max(intensity, 0.01f);
    light.color = color;
    this.m_lastColor = color;
    this.m_lastIntensity = intensity;
    this.m_lastRange = range;
  }

  [Obsolete("Please use Unity Light component's \"color\" field.")]
  public Color Color
  {
    get => this.Light.color;
    set => this.Light.color = value;
  }

  [Obsolete("Please use Unity Light component's \"intensity\" field.")]
  public float Intensity
  {
    get => this.Light.intensity;
    set => this.Light.intensity = value;
  }

  [Obsolete("No longer used. Please remove all references to it.")]
  public bool IsAnimated { get; set; }
}
