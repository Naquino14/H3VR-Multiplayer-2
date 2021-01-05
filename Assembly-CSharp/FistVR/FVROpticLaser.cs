// Decompiled with JetBrains decompiler
// Type: FistVR.FVROpticLaser
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVROpticLaser : MonoBehaviour
  {
    public Transform SightLine;
    public LayerMask CastLayer;
    public Transform Laser;
    public Transform LaserDot;
    private bool m_isOn;
    private RaycastHit m_hit;

    public bool IsOn => this.m_isOn;

    private void Awake()
    {
      this.Laser.gameObject.SetActive(false);
      this.LaserDot.gameObject.SetActive(false);
    }

    public void TurnLaserOn() => this.m_isOn = true;

    public void TurnLaserOff() => this.m_isOn = false;

    private void Update()
    {
      if (!this.m_isOn)
      {
        this.Laser.gameObject.SetActive(false);
        this.LaserDot.gameObject.SetActive(false);
      }
      else
      {
        this.Laser.gameObject.SetActive(true);
        this.LaserDot.gameObject.SetActive(true);
        Vector3 worldPosition = !Physics.Raycast(this.SightLine.position, this.SightLine.forward, out this.m_hit, 100f, (int) this.CastLayer) ? this.SightLine.position + this.SightLine.forward * 100f : this.m_hit.point;
        this.Laser.LookAt(worldPosition, Vector3.up);
        if (Physics.Raycast(this.Laser.position, this.Laser.forward, out this.m_hit, 100f, (int) this.CastLayer))
        {
          this.Laser.localScale = new Vector3(1f, 1f, this.m_hit.distance * 0.7692308f);
          this.LaserDot.position = this.m_hit.point;
        }
        else
        {
          this.Laser.localScale = new Vector3(1f, 1f, 100f);
          this.LaserDot.position = worldPosition;
        }
      }
    }
  }
}
