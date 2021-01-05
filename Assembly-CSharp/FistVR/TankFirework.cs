// Decompiled with JetBrains decompiler
// Type: FistVR.TankFirework
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TankFirework : MonoBehaviour
  {
    public GameObject FuseFire;
    public GameObject BurnFireTrigger;
    public Transform Fuse_Point1;
    public Transform Fuse_Point2;
    public float burnSpeed = 0.15f;
    public Renderer TankRend;
    private float m_burnLerp;
    private bool m_isIgnited;
    private bool m_isFireworkTriggered;
    private bool m_doneBurning;
    public ParticleSystem PSystem_BarrelSparkles;
    public ParticleSystem PSystem_BarrelSparkles2;
    public ParticleSystem PSystem_Splodes;
    public ParticleSystem PSystem_Smoke;
    public ParticleSystem BurnFire;
    public AudioSource Aud_Crackling;
    public AudioSource Aud_Sparkler;
    public Transform Muzzle;
    public GameObject DragonProj;
    public Rigidbody RB;
    private int popTick = 4;
    private float projTick = 1f;
    private float dieCounter;

    public void Ignite()
    {
      if (this.m_isIgnited)
        return;
      this.m_isIgnited = true;
      this.FuseFire.SetActive(true);
      this.FuseFire.transform.localPosition = this.Fuse_Point1.localPosition;
    }

    public void Update() => this.Burn();

    public void Awake()
    {
      this.popTick = Random.Range(5, 15);
      this.projTick = Random.Range(0.25f, 0.75f);
    }

    private void Burn()
    {
      if (!this.m_isIgnited || this.m_doneBurning)
      {
        if (this.m_doneBurning)
          this.dieCounter += Time.deltaTime;
        if ((double) this.dieCounter <= 10.0)
          return;
        Object.Destroy((Object) this.gameObject);
      }
      else
      {
        this.m_burnLerp += Time.deltaTime * this.burnSpeed;
        if ((double) this.m_burnLerp < 0.300000011920929)
          this.FuseFire.transform.localPosition = Vector3.Lerp(this.Fuse_Point1.localPosition, this.Fuse_Point2.localPosition, 4f * this.m_burnLerp);
        else if ((double) this.m_burnLerp < 0.990000009536743)
        {
          this.PSystem_BarrelSparkles.Emit(3);
          this.PSystem_BarrelSparkles2.Emit(1);
          this.PSystem_Splodes.Emit(1);
          this.BurnFire.Emit(1);
          if (!this.m_isFireworkTriggered)
          {
            this.m_isFireworkTriggered = true;
            this.Aud_Crackling.Play();
            this.Aud_Sparkler.Play();
          }
          if (this.FuseFire.activeSelf)
            this.FuseFire.SetActive(false);
          if (!this.BurnFireTrigger.activeSelf)
            this.BurnFireTrigger.SetActive(true);
          if (!this.PSystem_Smoke.gameObject.activeSelf)
            this.PSystem_Smoke.gameObject.SetActive(true);
          if (this.popTick > 0)
          {
            --this.popTick;
          }
          else
          {
            this.popTick = Random.Range(5, 15);
            FXM.InitiateMuzzleFlash(this.PSystem_BarrelSparkles.transform.position, this.PSystem_BarrelSparkles.transform.forward, Random.Range(0.3f, 2f), new Color(1f, Random.Range(0.2f, 1f), 0.2f), Random.Range(0.5f, 2f));
          }
          if ((double) this.projTick > 0.0)
          {
            this.projTick -= Time.deltaTime;
          }
          else
          {
            this.projTick = Random.Range(0.25f, 0.75f);
            GameObject gameObject = Object.Instantiate<GameObject>(this.DragonProj, this.Muzzle.position, this.Muzzle.rotation);
            gameObject.transform.Rotate(new Vector3(Random.Range(-15f, 15f), Random.Range(-15f, 15f), Random.Range(-15f, 15f)));
            this.RB.AddForceAtPosition(-gameObject.transform.forward * Random.Range(0.1f, 0.4f), this.Muzzle.position, ForceMode.Impulse);
            gameObject.GetComponent<BallisticProjectile>().Fire(gameObject.transform.forward, (FVRFireArm) null);
          }
        }
        else if (!this.m_doneBurning)
        {
          this.m_isFireworkTriggered = false;
          this.m_doneBurning = true;
          if (this.PSystem_Smoke.gameObject.activeSelf)
            this.PSystem_Smoke.gameObject.SetActive(false);
          this.Aud_Crackling.Stop();
          this.Aud_Sparkler.Stop();
          this.BurnFireTrigger.SetActive(false);
        }
        float num1 = Mathf.Clamp(this.m_burnLerp, 0.0f, 1f);
        float num2 = Mathf.Clamp(this.m_burnLerp - 0.2f, 0.0f, 0.775f);
        this.TankRend.material.SetFloat("_TransitionCutoff", num1);
        this.TankRend.material.SetFloat("_DissolveCutoff", num2);
      }
    }
  }
}
