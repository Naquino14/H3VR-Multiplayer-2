// Decompiled with JetBrains decompiler
// Type: FistVR.FVRIgnitable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRIgnitable : MonoBehaviour
  {
    public FXM.FireFXType FireType;
    public bool HasDamageAble;
    public IFVRDamageable Dam;
    public float IgnitionThreshold = 1f;
    private ParticleSystem m_fireInstance;
    public bool m_hasFireInstance;
    [Header("Damage Stuff")]
    public bool DamagesSelf = true;
    public float Dam_Thermal;
    public float Dam_Frequency = 0.25f;
    public float Dam_Radius = 0.25f;
    private float m_frequencyTick = 0.25f;
    private Damage dam;
    public bool UsesTransformOverride;
    public Transform TransformOverride;
    [Header("LightFlash Stuff")]
    public bool UsesLightFlash;
    public Color LightFlashColor;
    public Vector2 LightFlashIntensityRange = new Vector2(0.5f, 2f);
    public Vector2 LightFlashRadiusRange = new Vector2(0.5f, 2f);
    private float tick = 0.1f;

    public void Start()
    {
      IFVRDamageable component = this.GetComponent<IFVRDamageable>();
      if (component != null)
      {
        this.Dam = component;
        this.HasDamageAble = true;
      }
      this.dam = new Damage();
      this.tick = Random.Range(0.02f, 0.1f);
    }

    public bool IsIgniteable() => !this.m_hasFireInstance;

    public Transform GetSpawnPos() => this.UsesTransformOverride ? this.TransformOverride : this.transform;

    public bool IsOnFire() => this.m_hasFireInstance;

    public void Ignite(ParticleSystem p)
    {
      this.m_fireInstance = p;
      this.m_hasFireInstance = true;
    }

    private void Update()
    {
      if (!this.m_hasFireInstance)
        return;
      if (this.UsesLightFlash)
      {
        if ((double) this.tick > 0.0)
        {
          this.tick -= Time.deltaTime;
        }
        else
        {
          this.tick = Random.Range(0.02f, 0.25f);
          float num = 1f;
          if (GM.CurrentSceneSettings.IsSceneLowLight)
            num = 2.5f;
          FXM.InitiateMuzzleFlashLowPriority(this.transform.position, Vector3.up, Random.Range(this.LightFlashIntensityRange.x, this.LightFlashIntensityRange.y) * num, this.LightFlashColor, Random.Range(this.LightFlashRadiusRange.x, this.LightFlashRadiusRange.y) * num);
        }
      }
      if ((double) this.m_frequencyTick > 0.0)
        this.m_frequencyTick -= Time.deltaTime;
      else if (this.DamagesSelf)
      {
        this.m_frequencyTick = this.Dam_Frequency * Random.Range(1f, 1.2f);
        if (this.HasDamageAble)
        {
          this.dam.Dam_Thermal = this.Dam_Thermal * Random.Range(0.5f, 1f);
          this.dam.Dam_TotalEnergetic = this.dam.Dam_Thermal;
          this.dam.strikeDir = Random.onUnitSphere;
          this.dam.hitNormal = -this.dam.strikeDir;
          this.dam.point = this.dam.hitNormal * this.Dam_Radius;
          this.dam.Class = Damage.DamageClass.Abstract;
          this.Dam.Damage(this.dam);
        }
      }
      if (this.m_fireInstance.particleCount >= 1)
        return;
      Object.Destroy((Object) this.m_fireInstance);
      this.m_hasFireInstance = false;
    }

    private void OnDestroy()
    {
      this.Dam = (IFVRDamageable) null;
      this.HasDamageAble = false;
    }

    public void OnParticleCollision(GameObject other)
    {
      if (!this.m_hasFireInstance)
      {
        if (!other.CompareTag("IgnitorSystem"))
          return;
        FXM.Ignite(this, 1f);
      }
      else
      {
        if (!this.DamagesSelf || !this.HasDamageAble || !other.CompareTag("IgnitorSystem"))
          return;
        this.dam.Dam_Thermal = this.Dam_Thermal * Random.Range(0.25f, 1f);
        this.dam.Dam_TotalEnergetic = this.dam.Dam_Thermal;
        this.dam.strikeDir = Random.onUnitSphere;
        this.dam.hitNormal = -this.dam.strikeDir;
        this.dam.point = this.dam.hitNormal * this.Dam_Radius;
        this.dam.Class = Damage.DamageClass.Explosive;
        this.Dam.Damage(this.dam);
      }
    }
  }
}
