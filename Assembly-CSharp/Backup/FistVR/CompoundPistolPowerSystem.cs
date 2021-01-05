// Decompiled with JetBrains decompiler
// Type: FistVR.CompoundPistolPowerSystem
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class CompoundPistolPowerSystem : MonoBehaviour
  {
    [GradientHDR]
    public Gradient colorGrad;
    public Renderer Rend;
    public Handgun Hangun;
    public AudioEvent AudEvent_Overheat;
    public AudioEvent AudEvent_AllSet;
    public ParticleSystem PSystem_Overheat;
    public ParticleSystem PSystem_Overheat2;
    private float m_heat;
    private float m_timeSinceLastShot = 1f;
    private bool m_isOverheating;
    private float m_coolTime = 3.5f;

    private void OnShotFired(FVRFireArm firearm)
    {
      if (!((Object) firearm == (Object) this.Hangun))
        return;
      this.AddHeat();
      this.PSystem_Overheat.Emit(5);
      this.PSystem_Overheat2.Emit(5);
    }

    private void AddHeat()
    {
      this.m_heat += 0.1f;
      this.m_timeSinceLastShot = 0.0f;
      if ((double) this.m_heat >= 1.0 && !this.m_isOverheating)
        this.Overheat();
      this.m_heat = Mathf.Clamp(this.m_heat, 0.0f, 1f);
    }

    private void Overheat()
    {
      this.m_isOverheating = true;
      this.Hangun.Magazine.ForceEmpty();
      this.m_coolTime = 3.5f;
      this.Hangun.PlayAudioAsHandling(this.AudEvent_Overheat, this.transform.position).FollowThisTransform(this.transform);
    }

    private void Reset()
    {
      this.m_isOverheating = false;
      this.Hangun.Magazine.ForceFull();
      this.Hangun.DropSlideRelease();
      this.m_heat = 0.0f;
      this.Hangun.PlayAudioAsHandling(this.AudEvent_AllSet, this.transform.position).FollowThisTransform(this.transform);
    }

    private void Update()
    {
      this.Hangun.IsSlideLockExternalHeldDown = false;
      if (!this.m_isOverheating)
      {
        if ((double) this.m_timeSinceLastShot < 0.300000011920929)
          this.m_timeSinceLastShot += Time.deltaTime;
        else if ((double) this.m_heat > 0.0)
          this.m_heat -= Time.deltaTime;
      }
      else
      {
        this.PSystem_Overheat.Emit(1);
        if ((double) this.m_coolTime > 0.0)
          this.m_coolTime -= Time.deltaTime;
        else
          this.Reset();
      }
      float y = Mathf.Lerp(0.5f, -0.5f, this.m_heat);
      this.Rend.material.SetColor("_EmissionColor", this.colorGrad.Evaluate(this.m_heat));
      this.Rend.material.SetTextureOffset("_IncandescenceMap", new Vector2(0.0f, y));
    }

    private void Start() => GM.CurrentSceneSettings.ShotFiredEvent += new FVRSceneSettings.ShotFired(this.OnShotFired);

    private void OnDisable() => GM.CurrentSceneSettings.ShotFiredEvent -= new FVRSceneSettings.ShotFired(this.OnShotFired);
  }
}
