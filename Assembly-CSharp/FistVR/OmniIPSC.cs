// Decompiled with JetBrains decompiler
// Type: FistVR.OmniIPSC
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OmniIPSC : MonoBehaviour, IFVRDamageable
  {
    public bool IsNoShoot;
    public Transform SpawnPoint;
    public Texture2D MaskTexture;
    private bool HasBeenShot;
    public GameObject[] HitZones;
    public Transform XYGridOrigin;
    public OmniSpawner_IPSC Spawner;
    private float m_stateTick;
    private float m_timeLeft = 1f;
    private Vector3 m_startPos;
    private Vector3 m_endPos;
    private OmniIPSC.TargetState m_state;

    public void Configure(OmniSpawner_IPSC spawner, Transform point, Vector2 TimeActivated)
    {
      this.Spawner = spawner;
      this.SpawnPoint = point;
      this.m_startPos = point.position;
      this.m_startPos.y = -3f;
      this.m_endPos = point.position;
      this.m_stateTick = 0.0f;
      this.m_timeLeft = Random.Range(TimeActivated.x, TimeActivated.y);
    }

    private void Update()
    {
      switch (this.m_state)
      {
        case OmniIPSC.TargetState.Activating:
          if ((double) this.m_stateTick < 1.0)
          {
            this.m_stateTick += Time.deltaTime * 4f;
          }
          else
          {
            this.m_stateTick = 1f;
            this.m_state = OmniIPSC.TargetState.Activated;
          }
          this.transform.position = Vector3.Lerp(this.m_startPos, this.m_endPos, this.m_stateTick);
          break;
        case OmniIPSC.TargetState.Deactivating:
          if ((double) this.m_stateTick > 0.0)
          {
            this.m_stateTick -= Time.deltaTime * 4f;
          }
          else
          {
            this.m_stateTick = 0.0f;
            this.Spawner.TargetDeactivating(this);
          }
          this.transform.position = Vector3.Lerp(this.m_startPos, this.m_endPos, this.m_stateTick);
          break;
        case OmniIPSC.TargetState.Activated:
          if ((double) this.m_timeLeft > 0.0)
          {
            this.m_timeLeft -= Time.deltaTime;
            break;
          }
          this.m_timeLeft = 0.0f;
          this.m_state = OmniIPSC.TargetState.Deactivating;
          break;
      }
    }

    private void Deactivate() => this.m_state = OmniIPSC.TargetState.Deactivating;

    public void Damage(FistVR.Damage dam)
    {
      if (this.HasBeenShot)
        return;
      this.HasBeenShot = true;
      if (this.IsNoShoot)
        this.Spawner.Invoke("PlayFailureSound", 0.15f);
      else
        this.Spawner.Invoke("PlaySuccessSound", 0.15f);
      this.Invoke("Deactivate", 0.5f);
      Vector3 vector3 = this.XYGridOrigin.InverseTransformPoint(dam.point);
      vector3.z = 0.0f;
      vector3.x = Mathf.Clamp(vector3.x, 0.0f, 1f);
      vector3.y = Mathf.Clamp(vector3.y, 0.0f, 1f);
      Color pixel = this.MaskTexture.GetPixel(Mathf.RoundToInt((float) this.MaskTexture.width * vector3.x), Mathf.RoundToInt((float) this.MaskTexture.width * vector3.y));
      if ((double) pixel.r > 0.5 && (double) pixel.g < 0.5)
      {
        this.HasBeenShot = true;
        this.RegisterHit(0);
      }
      else if ((double) pixel.r > 0.5 && (double) pixel.g > 0.5)
      {
        this.HasBeenShot = true;
        this.RegisterHit(1);
      }
      else if ((double) pixel.g > 0.5 && (double) pixel.r < 0.5)
      {
        this.HasBeenShot = true;
        this.RegisterHit(2);
      }
      else
      {
        if ((double) pixel.b <= 0.5)
          return;
        this.HasBeenShot = true;
        this.RegisterHit(3);
      }
    }

    private void RegisterHit(int i)
    {
      this.HitZones[i].SetActive(true);
      int num = 1;
      if (this.IsNoShoot)
        num = -1;
      switch (i)
      {
        case 0:
          this.Spawner.AddPoints(100 * num);
          break;
        case 1:
          this.Spawner.AddPoints(80 * num);
          break;
        case 2:
          this.Spawner.AddPoints(60 * num);
          break;
        case 3:
          this.Spawner.AddPoints(20 * num);
          break;
      }
    }

    public enum TargetState
    {
      Activating,
      Deactivating,
      Activated,
    }
  }
}
