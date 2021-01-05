// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Balloon
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class Balloon : MonoBehaviour
  {
    private Hand hand;
    public GameObject popPrefab;
    public float maxVelocity = 5f;
    public float lifetime = 15f;
    public bool burstOnLifetimeEnd;
    public GameObject lifetimeEndParticlePrefab;
    public SoundPlayOneshot lifetimeEndSound;
    private float destructTime;
    private float releaseTime = 99999f;
    public SoundPlayOneshot collisionSound;
    private float lastSoundTime;
    private float soundDelay = 0.2f;
    private Rigidbody balloonRigidbody;
    private bool bParticlesSpawned;
    private static float s_flLastDeathSound;

    private void Start()
    {
      this.destructTime = Time.time + this.lifetime + Random.value;
      this.hand = this.GetComponentInParent<Hand>();
      this.balloonRigidbody = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
      if ((double) this.destructTime == 0.0 || (double) Time.time <= (double) this.destructTime)
        return;
      if (this.burstOnLifetimeEnd)
        this.SpawnParticles(this.lifetimeEndParticlePrefab, this.lifetimeEndSound);
      Object.Destroy((Object) this.gameObject);
    }

    private void SpawnParticles(GameObject particlePrefab, SoundPlayOneshot sound)
    {
      if (this.bParticlesSpawned)
        return;
      this.bParticlesSpawned = true;
      if ((Object) particlePrefab != (Object) null)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(particlePrefab, this.transform.position, this.transform.rotation);
        gameObject.GetComponent<ParticleSystem>().Play();
        Object.Destroy((Object) gameObject, 2f);
      }
      if (!((Object) sound != (Object) null))
        return;
      if ((double) (Time.time - Balloon.s_flLastDeathSound) < 0.100000001490116)
      {
        sound.volMax *= 0.25f;
        sound.volMin *= 0.25f;
      }
      sound.Play();
      Balloon.s_flLastDeathSound = Time.time;
    }

    private void FixedUpdate()
    {
      if ((double) this.balloonRigidbody.velocity.sqrMagnitude <= (double) this.maxVelocity)
        return;
      this.balloonRigidbody.velocity *= 0.97f;
    }

    private void ApplyDamage()
    {
      this.SpawnParticles(this.popPrefab, (SoundPlayOneshot) null);
      Object.Destroy((Object) this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
      if (this.bParticlesSpawned)
        return;
      Hand hand = (Hand) null;
      BalloonHapticBump component = collision.gameObject.GetComponent<BalloonHapticBump>();
      if ((Object) component != (Object) null && (Object) component.physParent != (Object) null)
        hand = component.physParent.GetComponentInParent<Hand>();
      if ((double) Time.time > (double) this.lastSoundTime + (double) this.soundDelay)
      {
        if ((Object) hand != (Object) null)
        {
          if ((double) Time.time > (double) this.releaseTime + (double) this.soundDelay)
          {
            this.collisionSound.Play();
            this.lastSoundTime = Time.time;
          }
        }
        else
        {
          this.collisionSound.Play();
          this.lastSoundTime = Time.time;
        }
      }
      if ((double) this.destructTime > 0.0)
        return;
      if ((double) this.balloonRigidbody.velocity.magnitude > (double) this.maxVelocity * 10.0)
        this.balloonRigidbody.velocity = this.balloonRigidbody.velocity.normalized * this.maxVelocity;
      if (!((Object) this.hand != (Object) null))
        return;
      this.hand.TriggerHapticPulse((ushort) Mathf.Clamp(Util.RemapNumber(collision.relativeVelocity.magnitude, 0.0f, 3f, 500f, 800f), 500f, 800f));
    }

    public void SetColor(Balloon.BalloonColor color) => this.GetComponentInChildren<MeshRenderer>().material.color = this.BalloonColorToRGB(color);

    private Color BalloonColorToRGB(Balloon.BalloonColor balloonColorVar)
    {
      Color color = new Color((float) byte.MaxValue, 0.0f, 0.0f);
      switch (balloonColorVar)
      {
        case Balloon.BalloonColor.Red:
          return new Color(237f, 29f, 37f, (float) byte.MaxValue) / (float) byte.MaxValue;
        case Balloon.BalloonColor.OrangeRed:
          return new Color(241f, 91f, 35f, (float) byte.MaxValue) / (float) byte.MaxValue;
        case Balloon.BalloonColor.Orange:
          return new Color(245f, 140f, 31f, (float) byte.MaxValue) / (float) byte.MaxValue;
        case Balloon.BalloonColor.YellowOrange:
          return new Color(253f, 185f, 19f, (float) byte.MaxValue) / (float) byte.MaxValue;
        case Balloon.BalloonColor.Yellow:
          return new Color(254f, 243f, 0.0f, (float) byte.MaxValue) / (float) byte.MaxValue;
        case Balloon.BalloonColor.GreenYellow:
          return new Color(172f, 209f, 54f, (float) byte.MaxValue) / (float) byte.MaxValue;
        case Balloon.BalloonColor.Green:
          return new Color(0.0f, 167f, 79f, (float) byte.MaxValue) / (float) byte.MaxValue;
        case Balloon.BalloonColor.BlueGreen:
          return new Color(108f, 202f, 189f, (float) byte.MaxValue) / (float) byte.MaxValue;
        case Balloon.BalloonColor.Blue:
          return new Color(0.0f, 119f, 178f, (float) byte.MaxValue) / (float) byte.MaxValue;
        case Balloon.BalloonColor.VioletBlue:
          return new Color(82f, 80f, 162f, (float) byte.MaxValue) / (float) byte.MaxValue;
        case Balloon.BalloonColor.Violet:
          return new Color(102f, 46f, 143f, (float) byte.MaxValue) / (float) byte.MaxValue;
        case Balloon.BalloonColor.RedViolet:
          return new Color(182f, 36f, 102f, (float) byte.MaxValue) / (float) byte.MaxValue;
        case Balloon.BalloonColor.LightGray:
          return new Color(192f, 192f, 192f, (float) byte.MaxValue) / (float) byte.MaxValue;
        case Balloon.BalloonColor.DarkGray:
          return new Color(128f, 128f, 128f, (float) byte.MaxValue) / (float) byte.MaxValue;
        case Balloon.BalloonColor.Random:
          return this.BalloonColorToRGB((Balloon.BalloonColor) Random.Range(0, 12));
        default:
          return color;
      }
    }

    public enum BalloonColor
    {
      Red,
      OrangeRed,
      Orange,
      YellowOrange,
      Yellow,
      GreenYellow,
      Green,
      BlueGreen,
      Blue,
      VioletBlue,
      Violet,
      RedViolet,
      LightGray,
      DarkGray,
      Random,
    }
  }
}
