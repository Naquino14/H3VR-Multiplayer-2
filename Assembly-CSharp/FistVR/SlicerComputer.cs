// Decompiled with JetBrains decompiler
// Type: FistVR.SlicerComputer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SlicerComputer : FVRDestroyableObject
  {
    [Header("Slicer Computer Params")]
    public Rigidbody BaseRB;
    public AIThrusterControlBox[] ControlBoxes;
    public AISensorSystem Sensors;
    private float m_desiredElevation = 1.3f;
    private bool m_needsElevationBoost;
    private Vector3 m_desiredDir = Vector3.zero;
    public Vector3 CurrentThrusterForce = Vector3.zero;
    public LayerMask LM_GroundCast;
    private RaycastHit m_hit;
    private bool m_hasBegunDestroy;
    private float m_TimeTilAgro = 3f;
    private bool m_isAgro;

    public override void Awake() => base.Awake();

    public override void Update()
    {
      this.Sensors.UpdateSensorSystem();
      this.CombatComputer();
      this.FlightComputer();
      base.Update();
    }

    public void FixedUpdate() => this.ThrustMaster();

    public override void DestroyEvent()
    {
      if (this.m_hasBegunDestroy)
        return;
      this.m_hasBegunDestroy = true;
      this.Invoke("BlowUpAControlBox", Random.Range(0.85f, 1.5f));
      this.Invoke("BlowUpAControlBox", Random.Range(1.6f, 2.1f));
      this.Invoke("BlowUpAControlBox", Random.Range(2.2f, 2.7f));
      this.Invoke("DelayedDestroy", Random.Range(3f, 3.2f));
    }

    private void BlowUpAControlBox()
    {
      int index = Random.Range(0, this.ControlBoxes.Length);
      if (!((Object) this.ControlBoxes[index] != (Object) null))
        return;
      this.ControlBoxes[index].DestroyEvent();
    }

    private void DelayedDestroy() => base.DestroyEvent();

    private void CombatComputer()
    {
      if (this.Sensors.PriorityTarget != null)
      {
        this.m_TimeTilAgro -= Time.deltaTime;
        if ((double) this.m_TimeTilAgro > 0.0)
          return;
        this.m_isAgro = true;
      }
      else
      {
        this.m_TimeTilAgro = 3f;
        this.m_isAgro = false;
      }
    }

    private void FlightComputer()
    {
      this.m_desiredDir = Vector3.zero;
      float b1 = this.transform.position.y + 50f;
      float num = this.transform.position.y - 50f;
      if (Physics.Raycast(this.transform.position, -Vector3.up, out this.m_hit, 50f, (int) this.LM_GroundCast, QueryTriggerInteraction.Ignore))
        num = this.m_hit.point.y;
      if (Physics.Raycast(this.transform.position, Vector3.up, out this.m_hit, 50f, (int) this.LM_GroundCast, QueryTriggerInteraction.Ignore))
        b1 = this.m_hit.point.y;
      float b2 = Mathf.Min(num + 1.4f, b1);
      if (this.Sensors.PriorityTarget != null && this.m_isAgro && !this.m_hasBegunDestroy)
      {
        Vector3 vector = this.Sensors.PriorityTarget.LastKnownPosition - this.transform.position;
        vector.y = 0.0f;
        float maxLength = 0.05f;
        if ((double) vector.magnitude > 12.0)
          maxLength = 0.12f;
        this.m_desiredDir = Vector3.ClampMagnitude(vector, maxLength);
        b2 = Mathf.Max(Mathf.Min(this.Sensors.PriorityTarget.LastKnownPosition.y, b1), b2);
      }
      if ((double) b2 > (double) this.transform.position.y)
      {
        this.m_desiredDir += Vector3.up * Mathf.Abs(b2 - this.transform.position.y) * 0.5f;
        this.m_needsElevationBoost = true;
      }
      else
      {
        this.m_desiredDir += Vector3.down * Mathf.Abs(b2 - this.transform.position.y) * 0.3f;
        this.m_needsElevationBoost = true;
      }
    }

    private void ThrustMaster()
    {
      if (!this.m_needsElevationBoost)
        return;
      bool flag = false;
      float magnitude = 0.0f;
      Vector3 normalized = this.m_desiredDir.normalized;
      for (int index = 0; index < this.ControlBoxes.Length; ++index)
      {
        if ((Object) this.ControlBoxes[index] != (Object) null && this.ControlBoxes[index].Thrust(normalized, ref magnitude))
          flag = true;
      }
      if (!flag)
        return;
      this.CurrentThrusterForce = this.m_desiredDir * Mathf.Clamp(30f * magnitude, 0.0f, 100f);
      this.BaseRB.AddForce(this.CurrentThrusterForce, ForceMode.Acceleration);
      if (!this.m_isAgro && !this.m_hasBegunDestroy)
        return;
      this.m_desiredDir.y = 0.0f;
      this.m_desiredDir = Vector3.Cross(this.m_desiredDir, Vector3.up);
      this.BaseRB.AddTorque(this.m_desiredDir * 20f);
    }
  }
}
