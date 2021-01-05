// Decompiled with JetBrains decompiler
// Type: FistVR.NoiseGrip
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class NoiseGrip : MonoBehaviour
  {
    public AudioSource AudSource;
    private float m_volume;
    private float m_actualVolume;
    public LayerMask DetectMask;
    public float Radius = 1.25f;
    public Transform VolumeCenter;

    public void ProcessInput(FVRViveHand hand, FVRInteractiveObject o)
    {
      if (o.m_hasTriggeredUpSinceBegin && hand.Input.TriggerDown)
      {
        if (this.AudSource.isPlaying)
          this.AudSource.Stop();
        this.AudSource.Play();
        this.Horn();
        GM.CurrentSceneSettings.OnPerceiveableSound(80f, 32f, this.transform.position, GM.CurrentPlayerBody.GetPlayerIFF());
      }
      if (!o.m_hasTriggeredUpSinceBegin || !hand.Input.TriggerPressed)
        return;
      this.m_actualVolume = 1f;
    }

    private void Update()
    {
      this.m_volume = (float) (0.25 + (180.0 - (double) Vector3.Angle(GM.CurrentPlayerBody.Head.position - this.transform.position, this.transform.forward)) / 180.0 * 0.75);
      this.AudSource.volume = this.m_volume * this.m_actualVolume;
    }

    private void Horn()
    {
      Collider[] colliderArray = Physics.OverlapSphere(this.VolumeCenter.position, this.Radius, (int) this.DetectMask, QueryTriggerInteraction.Collide);
      for (int index = 0; index < colliderArray.Length; ++index)
      {
        if (!((Object) colliderArray[index].attachedRigidbody == (Object) null))
        {
          SosigLink component = colliderArray[index].attachedRigidbody.gameObject.GetComponent<SosigLink>();
          if ((Object) component != (Object) null && component.BodyPart == SosigLink.SosigBodyPart.Head)
            component.Damage(new Damage()
            {
              Dam_Stunning = 10f
            });
        }
      }
    }
  }
}
