// Decompiled with JetBrains decompiler
// Type: FistVR.LAPD2019Laser
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class LAPD2019Laser : MonoBehaviour
  {
    public GameObject BeamHitPoint;
    public Transform Aperture;
    public LayerMask LM;
    private RaycastHit m_hit;
    private bool m_isOn;
    public bool UsesAutoOnOff;
    public FVRPhysicalObject PO;
    public Transform Muzzle;

    public void TurnOn() => this.m_isOn = true;

    public void TurnOff() => this.m_isOn = false;

    private void Update()
    {
      if (this.UsesAutoOnOff)
      {
        if (this.PO.IsHeld)
          this.TurnOn();
        else
          this.TurnOff();
      }
      if (this.m_isOn)
      {
        if ((Object) this.Muzzle != (Object) null)
        {
          this.Aperture.transform.LookAt(this.Muzzle.position + this.Muzzle.forward * 25f);
          this.Aperture.transform.localEulerAngles = new Vector3(this.Aperture.transform.localEulerAngles.x, 0.0f, 0.0f);
        }
        if (!this.BeamHitPoint.activeSelf)
          this.BeamHitPoint.SetActive(true);
        Vector3 vector3 = this.Aperture.position + this.Aperture.forward * 100f;
        float num1 = 100f;
        if (Physics.Raycast(this.Aperture.position, this.Aperture.forward, out this.m_hit, 100f, (int) this.LM, QueryTriggerInteraction.Ignore))
        {
          vector3 = this.m_hit.point;
          num1 = this.m_hit.distance;
        }
        float num2 = Mathf.Lerp(0.01f, 0.2f, num1 * 0.01f);
        this.BeamHitPoint.transform.position = vector3;
        this.BeamHitPoint.transform.localScale = new Vector3(num2, num2, num2);
      }
      else
      {
        if (!this.BeamHitPoint.activeSelf)
          return;
        this.BeamHitPoint.SetActive(false);
      }
    }
  }
}
