// Decompiled with JetBrains decompiler
// Type: FistVR.FlintlockPowderHornCap
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FlintlockPowderHornCap : FVRInteractiveObject
  {
    public FlintlockPowderHorn Horn;
    public bool UsesPourTrigger;
    public Transform PourTriggerUp;
    public Transform OverflowPoint;
    public GameObject PowderPrefab;
    public Transform PowderSpawnPoint;
    public AudioEvent AudEvent_PowderIn;
    private int m_numGrains;
    public Renderer Powder;
    public Vector3 Pos_Empty;
    public Vector3 Pos_Full;
    public Vector3 Scale_Empty;
    public Vector3 Scale_Full;
    private float m_insertSoundRefire = 0.2f;
    private float timeSinceSpawn;

    public bool IsFull() => this.m_numGrains >= 20;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      this.Horn.ToggleCap();
    }

    protected override void Awake()
    {
      base.Awake();
      this.UpdateViz();
    }

    public void OnTriggerEnter(Collider other)
    {
      if (!this.UsesPourTrigger || (Object) other.attachedRigidbody == (Object) null || (this.Horn.IsCapped || (double) Vector3.Angle(this.PourTriggerUp.forward, Vector3.up) > 90.0) || !other.attachedRigidbody.gameObject.CompareTag("flintlock_powdergrain"))
        return;
      if ((double) this.m_insertSoundRefire > 0.150000005960464)
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_PowderIn, this.transform.position);
      this.AddGrain();
      Object.Destroy((Object) other.gameObject);
    }

    private void AddGrain()
    {
      ++this.m_numGrains;
      if (this.m_numGrains > 20)
        this.SpawnOverflow();
      this.UpdateViz();
    }

    private void UpdateViz()
    {
      float t = (float) this.m_numGrains / 20f;
      if (!((Object) this.Powder != (Object) null))
        return;
      this.Powder.transform.localPosition = Vector3.Lerp(this.Pos_Empty, this.Pos_Full, t);
      this.Powder.transform.localScale = Vector3.Lerp(this.Scale_Empty, this.Scale_Full, t);
    }

    private void SpawnOverflow()
    {
      --this.m_numGrains;
      Object.Instantiate<GameObject>(this.PowderPrefab, this.OverflowPoint.position, Random.rotation);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) this.m_insertSoundRefire < 0.200000002980232)
        this.m_insertSoundRefire += Time.deltaTime;
      if ((double) this.timeSinceSpawn < 1.0)
        this.timeSinceSpawn += Time.deltaTime;
      if (this.m_numGrains <= 0 || this.Horn.IsCapped || ((double) Vector3.Angle(this.PourTriggerUp.forward, Vector3.up) <= 105.0 || (double) this.timeSinceSpawn <= 0.0399999991059303))
        return;
      --this.m_numGrains;
      this.timeSinceSpawn = 0.0f;
      Object.Instantiate<GameObject>(this.PowderPrefab, this.PowderSpawnPoint.position, Random.rotation);
      this.UpdateViz();
    }
  }
}
