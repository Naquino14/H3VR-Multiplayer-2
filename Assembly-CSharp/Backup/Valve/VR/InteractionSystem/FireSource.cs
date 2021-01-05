// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.FireSource
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class FireSource : MonoBehaviour
  {
    public GameObject fireParticlePrefab;
    public bool startActive;
    private GameObject fireObject;
    public ParticleSystem customParticles;
    public bool isBurning;
    public float burnTime;
    public float ignitionDelay;
    private float ignitionTime;
    private Hand hand;
    public AudioSource ignitionSound;
    public bool canSpreadFromThisSource = true;

    private void Start()
    {
      if (!this.startActive)
        return;
      this.StartBurning();
    }

    private void Update()
    {
      if ((double) this.burnTime == 0.0 || (double) Time.time <= (double) this.ignitionTime + (double) this.burnTime || !this.isBurning)
        return;
      this.isBurning = false;
      if ((Object) this.customParticles != (Object) null)
        this.customParticles.Stop();
      else
        Object.Destroy((Object) this.fireObject);
    }

    private void OnTriggerEnter(Collider other)
    {
      if (!this.isBurning || !this.canSpreadFromThisSource)
        return;
      other.SendMessageUpwards("FireExposure", SendMessageOptions.DontRequireReceiver);
    }

    private void FireExposure()
    {
      if ((Object) this.fireObject == (Object) null)
        this.Invoke("StartBurning", this.ignitionDelay);
      if (!(bool) (Object) (this.hand = this.GetComponentInParent<Hand>()))
        return;
      this.hand.TriggerHapticPulse((ushort) 1000);
    }

    private void StartBurning()
    {
      this.isBurning = true;
      this.ignitionTime = Time.time;
      if ((Object) this.ignitionSound != (Object) null)
        this.ignitionSound.Play();
      if ((Object) this.customParticles != (Object) null)
      {
        this.customParticles.Play();
      }
      else
      {
        if (!((Object) this.fireParticlePrefab != (Object) null))
          return;
        this.fireObject = Object.Instantiate<GameObject>(this.fireParticlePrefab, this.transform.position, this.transform.rotation);
        this.fireObject.transform.parent = this.transform;
      }
    }
  }
}
