// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFuse
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRFuse : MonoBehaviour, IFVRDamageable
  {
    public FVRPhysicalObject Dynamite;
    public Transform DynamiteCenter;
    public Transform[] JointPos;
    public GameObject FuseFire;
    public AudioClip FuseIgnite;
    public GameObject ExplosionVFX;
    public GameObject ExplosionSFX;
    private bool m_isIgnited;
    private float m_igniteTick;
    private float m_igniteSpeed = 0.1f;
    public Renderer FuseRend;
    protected bool hasBoomed;

    public void Damage(FistVR.Damage d)
    {
      if ((double) d.Dam_Thermal <= 0.0)
        return;
      this.Ignite(0.5f);
    }

    public bool IsIgnited() => this.m_isIgnited;

    public void OnParticleCollision(GameObject other)
    {
      if (!other.CompareTag("IgnitorSystem"))
        return;
      this.Ignite(Random.Range(0.1f, 0.9f));
    }

    private void Update()
    {
      if (!this.m_isIgnited)
        return;
      this.FuseFire.SetActive(true);
      this.m_igniteTick += Time.deltaTime * this.m_igniteSpeed;
      if ((double) this.m_igniteTick >= 1.0)
        this.Boom();
      int num1 = Mathf.FloorToInt(this.m_igniteTick * (float) this.JointPos.Length);
      int num2 = Mathf.RoundToInt(this.m_igniteTick * (float) this.JointPos.Length);
      int index1 = Mathf.Clamp(num1, 0, this.JointPos.Length - 1);
      int index2 = Mathf.Clamp(num2, 0, this.JointPos.Length - 1);
      this.FuseFire.transform.position = Vector3.Lerp(this.JointPos[index1].transform.position, this.JointPos[index2].transform.position, this.m_igniteTick);
      if (index1 > 0 && (Object) this.JointPos[index1 - 1].gameObject.GetComponent<Joint>() != (Object) null)
        Object.Destroy((Object) this.JointPos[index1 - 1].gameObject.GetComponent<Collider>());
      this.FuseRend.material.SetFloat("_DissolveCutoff", this.m_igniteTick);
    }

    public void Ignite(float f)
    {
      if (!((Object) this.Dynamite == (Object) null) && !((Object) this.Dynamite.QuickbeltSlot == (Object) null))
        return;
      this.m_igniteTick = Mathf.Max(this.m_igniteTick, f);
      if (this.m_isIgnited)
        return;
      this.m_isIgnited = true;
      this.FuseFire.SetActive(true);
      this.FuseFire.GetComponent<AudioSource>().Play();
      this.FuseFire.GetComponent<AudioSource>().PlayOneShot(this.FuseIgnite, 0.2f);
    }

    public virtual void Boom()
    {
      if (this.hasBoomed)
        return;
      this.hasBoomed = true;
      Object.Instantiate<GameObject>(this.ExplosionVFX, this.DynamiteCenter.position, Quaternion.identity);
      Object.Instantiate<GameObject>(this.ExplosionSFX, this.DynamiteCenter.position, Quaternion.identity);
      Object.Destroy((Object) this.DynamiteCenter.gameObject);
    }
  }
}
