// Decompiled with JetBrains decompiler
// Type: FistVR.SamplerPlatter_MelonBaller
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SamplerPlatter_MelonBaller : MonoBehaviour
  {
    public AudioEvent LaunchSound;
    public Transform[] LaunchPoints;
    private int m_currentLaunchPoint;
    public GameObject Payload;
    public float[] LaunchSpeeds;
    public Vector2 LaunchRefires = new Vector2(0.5f, 5.5f);
    public float LaunchRefire = 5.5f;
    private float m_refire = 2f;
    private bool m_isFiring;
    public AudioEvent Boop;

    private void Start()
    {
    }

    private void Update()
    {
      if (!this.m_isFiring)
        return;
      if ((double) this.m_refire > 0.0)
      {
        this.m_refire -= Time.deltaTime;
      }
      else
      {
        this.Fire();
        this.m_refire = this.LaunchRefire;
      }
    }

    public void SetSpeed(int i)
    {
      switch (i)
      {
        case 0:
          this.LaunchRefire = Mathf.Lerp(this.LaunchRefires.x, this.LaunchRefires.y, 1f);
          break;
        case 1:
          this.LaunchRefire = Mathf.Lerp(this.LaunchRefires.x, this.LaunchRefires.y, 0.75f);
          break;
        case 2:
          this.LaunchRefire = Mathf.Lerp(this.LaunchRefires.x, this.LaunchRefires.y, 0.5f);
          break;
        case 3:
          this.LaunchRefire = Mathf.Lerp(this.LaunchRefires.x, this.LaunchRefires.y, 0.25f);
          break;
        case 4:
          this.LaunchRefire = Mathf.Lerp(this.LaunchRefires.x, this.LaunchRefires.y, 0.0f);
          break;
      }
      SM.PlayGenericSound(this.Boop, this.transform.position);
    }

    public void TurnOn()
    {
      this.m_refire = this.LaunchRefire;
      this.m_isFiring = true;
      SM.PlayGenericSound(this.Boop, this.transform.position);
    }

    public void TurnOff()
    {
      this.m_isFiring = false;
      SM.PlayGenericSound(this.Boop, this.transform.position);
    }

    private void Fire()
    {
      Transform launchPoint = this.LaunchPoints[this.m_currentLaunchPoint];
      SM.PlayGenericSound(this.LaunchSound, launchPoint.position);
      Rigidbody component = Object.Instantiate<GameObject>(this.Payload, launchPoint.position, Random.rotation).GetComponent<Rigidbody>();
      component.velocity = launchPoint.forward * this.LaunchSpeeds[(int) GM.Options.SimulationOptions.ObjectGravityMode] * Random.Range(0.9f, 1.1f);
      component.velocity += launchPoint.right * Random.Range(-0.5f, 0.5f);
      ++this.m_currentLaunchPoint;
      if (this.m_currentLaunchPoint < this.LaunchPoints.Length)
        return;
      this.m_currentLaunchPoint = 0;
    }
  }
}
