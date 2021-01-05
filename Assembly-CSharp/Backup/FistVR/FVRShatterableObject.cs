// Decompiled with JetBrains decompiler
// Type: FistVR.FVRShatterableObject
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRShatterableObject : MonoBehaviour, IFVRDamageable
  {
    public int NumShotsToShatter = 1;
    public Rigidbody[] Shards;
    public AudioEvent AudEvent_Destruction;
    public ParticleSystem[] DestructionPSystems;
    public int[] DestructionPSystemEmits;
    protected Rigidbody m_rb;
    private AudioSource m_aud;
    private Renderer m_rend;
    private Collider m_col;
    private bool m_isDestroyed;
    public float explosionMultiplier = 1f;
    public GameObject OnDieTarget;
    public string OnDieMessage;
    public bool TakesColDamage;

    public virtual void Awake()
    {
      this.m_rb = this.GetComponent<Rigidbody>();
      this.m_aud = this.GetComponent<AudioSource>();
      this.m_rend = this.GetComponent<Renderer>();
      this.m_col = this.GetComponent<Collider>();
      for (int index = 0; index < this.Shards.Length; ++index)
        this.Shards[index].maxAngularVelocity = 30f;
    }

    public void OnCollisionEnter(Collision col)
    {
      if (!((Object) col.collider.attachedRigidbody != (Object) null) || !this.TakesColDamage)
        return;
      float magnitude = col.relativeVelocity.magnitude;
      if ((double) magnitude <= 2.5)
        return;
      int num = (int) ((double) magnitude * (double) col.collider.attachedRigidbody.mass / (double) this.m_rb.mass);
      if (num <= 0)
        return;
      this.NumShotsToShatter -= num;
      if (this.NumShotsToShatter > 0)
        return;
      this.Destroy(col.contacts[0].point, col.relativeVelocity);
    }

    public void Damage(FistVR.Damage dam)
    {
      float damTotalKinetic = dam.Dam_TotalKinetic;
      float num;
      if (dam.Class == FistVR.Damage.DamageClass.Explosive)
        num = damTotalKinetic * 0.005f;
      else if (dam.Class == FistVR.Damage.DamageClass.Projectile)
        num = damTotalKinetic * 0.01f;
      else if (dam.Class == FistVR.Damage.DamageClass.Melee)
        num = damTotalKinetic * 0.005f;
      this.NumShotsToShatter -= (int) dam.Dam_TotalKinetic;
      if (this.NumShotsToShatter <= 0)
      {
        this.Destroy(dam.point, dam.strikeDir * dam.Dam_TotalKinetic * 0.01f);
      }
      else
      {
        if (!((Object) this.m_rb != (Object) null))
          return;
        this.m_rb.AddForceAtPosition(dam.strikeDir * dam.Dam_TotalKinetic * 0.01f, dam.point);
      }
    }

    protected void GoNonKinematic()
    {
      if (!((Object) this.m_rb != (Object) null))
        return;
      this.m_rb.isKinematic = false;
      this.m_rb.useGravity = true;
    }

    protected void GoNonKinematic(Vector3 point, Vector3 force)
    {
      if (!((Object) this.m_rb != (Object) null))
        return;
      this.m_rb.isKinematic = false;
      this.m_rb.useGravity = true;
      this.m_rb.AddForceAtPosition(force * this.explosionMultiplier, point, ForceMode.Impulse);
    }

    public virtual void Destroy(Vector3 damagePoint, Vector3 damageDir)
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      if ((Object) this.OnDieTarget != (Object) null)
        this.OnDieTarget.SendMessage(this.OnDieMessage);
      this.m_rend.enabled = false;
      this.m_col.enabled = false;
      for (int index = 0; index < this.DestructionPSystems.Length; ++index)
      {
        this.DestructionPSystems[index].gameObject.SetActive(true);
        this.DestructionPSystems[index].transform.SetParent((Transform) null);
        this.DestructionPSystems[index].Emit(this.DestructionPSystemEmits[index]);
      }
      for (int index = 0; index < this.Shards.Length; ++index)
      {
        this.Shards[index].gameObject.SetActive(true);
        if ((Object) this.transform.parent != (Object) null)
          this.Shards[index].transform.SetParent(this.transform.parent);
        else
          this.Shards[index].transform.SetParent((Transform) null);
        this.Shards[index].AddForceAtPosition(this.explosionMultiplier * damageDir * (1f / (float) this.Shards.Length), damagePoint, ForceMode.Impulse);
      }
      if (this.AudEvent_Destruction.Clips.Count <= 0)
        return;
      SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, this.AudEvent_Destruction, this.transform.position, Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
    }

    private void Update()
    {
      if (!this.m_isDestroyed)
        return;
      Object.Destroy((Object) this.gameObject);
    }
  }
}
