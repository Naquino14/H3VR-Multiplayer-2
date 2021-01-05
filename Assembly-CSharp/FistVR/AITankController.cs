// Decompiled with JetBrains decompiler
// Type: FistVR.AITankController
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
  public class AITankController : MonoBehaviour
  {
    private bool IsActivated;
    public AISensorSystem SensorSystem;
    public AITurretController TurretController;
    private NavMeshAgent m_agent;
    private Vector3 m_currentNavTarget;
    private NavMeshHit m_nHit;
    public AIStrikePlateSlideDown StrikePlate1;
    public AIStrikePlateRotateResetting StrikePlate2;
    private bool IsLocomotionDisrupted;
    public Transform BotChassis;
    public float curChassisXRot;
    public float tarChassisXRot;
    public float deadCharrisXRot = 30f;
    private AudioSource m_aud;
    public AudioClip AudClip_Die;

    private void Awake()
    {
      this.m_agent = this.GetComponent<NavMeshAgent>();
      this.m_currentNavTarget = this.transform.position;
      this.m_aud = this.GetComponent<AudioSource>();
      this.Invoke("Activate", 5f);
    }

    public void Disable()
    {
      if (!this.IsActivated)
        return;
      this.IsActivated = false;
      this.m_aud.PlayOneShot(this.AudClip_Die, 1f);
      this.m_agent.Stop();
      this.m_agent.ResetPath();
    }

    public void Activate()
    {
      this.StrikePlate1.Reset();
      this.StrikePlate2.Reset();
      this.IsActivated = true;
    }

    public void UndisruptLocomotion()
    {
      this.IsLocomotionDisrupted = false;
      this.m_agent.Resume();
    }

    public void DisruptLocomotion()
    {
      this.IsLocomotionDisrupted = true;
      this.m_agent.Stop();
      this.m_agent.ResetPath();
    }

    private void Update()
    {
      if (this.IsActivated)
      {
        this.TurretController.UpdateTurretController();
        this.SensorSystem.UpdateSensorSystem();
        if (this.SensorSystem.PriorityTarget != null)
        {
          this.TurretController.SetTargetPoint(this.SensorSystem.PriorityTarget.LastKnownPosition);
          if (!this.IsLocomotionDisrupted)
            this.CheckIfNeedNewPath(this.SensorSystem.PriorityTarget.LastKnownPosition);
          this.TurretController.SetFireAtWill(true);
        }
        else
        {
          this.TurretController.SetTargetPoint(this.transform.position + this.transform.forward + this.transform.up);
          this.TurretController.SetFireAtWill(false);
        }
        this.tarChassisXRot = 0.0f;
      }
      else
      {
        this.tarChassisXRot = this.deadCharrisXRot;
        this.TurretController.SetFireAtWill(false);
      }
      if ((double) this.curChassisXRot == (double) this.tarChassisXRot)
        return;
      this.curChassisXRot = Mathf.Lerp(this.curChassisXRot, this.tarChassisXRot, Time.deltaTime * 2f);
      this.BotChassis.localEulerAngles = new Vector3(this.curChassisXRot, 0.0f, 0.0f);
    }

    private void CheckIfNeedNewPath(Vector3 target)
    {
      Vector3 vector3_1 = this.m_currentNavTarget;
      Vector3 vector3_2 = Random.onUnitSphere * 0.5f;
      vector3_2.y = 0.0f;
      if (NavMesh.SamplePosition(target + vector3_2 + Vector3.down * 0.5f, out this.m_nHit, 1.9f, -1))
        vector3_1 = this.m_nHit.position;
      if ((double) Vector3.Distance(vector3_1, this.m_currentNavTarget) <= 2.0)
        return;
      this.m_currentNavTarget = vector3_1;
      this.m_agent.SetDestination(vector3_1);
    }
  }
}
