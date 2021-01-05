// Decompiled with JetBrains decompiler
// Type: FistVR.FVRStrikeAnyWhereMatch
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRStrikeAnyWhereMatch : FVRPhysicalObject
  {
    [Header("Match Config")]
    public Transform[] BendingJoints;
    public Renderer MatchRenderer;
    public Collider MatchHeadCol;
    private bool m_hasBeenLit;
    private bool m_isBurning;
    private float m_burntState;
    private float m_burnSpeed = 0.05f;
    private Collider m_curStrikeCol;
    private int m_strikeFrames;
    private bool[] m_hasStartedBending = new bool[4];
    private Vector3 m_randBend = Vector3.zero;
    public GameObject FireGO;
    public ParticleSystem Fire1;
    public ParticleSystem Fire2;
    private float m_currentBurnPoint;
    private float m_currentBreakoff;
    private bool m_isTickingDownToDeath;
    private float m_deathTimer = 5f;
    public Color c1;
    public Color c2;
    private int m_matchTick = 4;

    protected override void FVRUpdate()
    {
      if (this.m_isTickingDownToDeath)
      {
        this.m_deathTimer -= Time.deltaTime;
        if ((double) this.m_deathTimer <= 0.0)
          Object.Destroy((Object) this.gameObject);
      }
      if (!this.m_isBurning)
        return;
      --this.m_matchTick;
      if (this.m_matchTick <= 0)
      {
        this.m_matchTick = Random.Range(2, 4);
        FXM.InitiateMuzzleFlash(this.FireGO.transform.position, this.transform.up, Random.Range(0.2f, 0.4f), Color.Lerp(this.c1, this.c2, Random.Range(0.0f, 1f)), Random.Range(0.9f, 1f));
      }
      this.m_burntState += Time.deltaTime * this.m_burnSpeed;
      if ((double) this.m_burntState >= 1.0)
      {
        this.m_burntState = 1f;
        this.m_isBurning = false;
        this.Fire1.emission.enabled = false;
        this.Fire2.emission.enabled = false;
        if (this.FireGO.activeSelf)
          this.FireGO.SetActive(false);
        this.m_isTickingDownToDeath = true;
      }
      this.m_currentBurnPoint = Mathf.Lerp(0.12f, 0.78f, this.m_burntState);
      this.MatchRenderer.material.SetFloat("_TransitionCutoff", this.m_currentBurnPoint);
      if ((double) this.m_burntState >= 0.200000002980232 && (double) this.m_burntState < 0.400000005960464)
      {
        this.FireGO.transform.position = Vector3.Lerp(this.BendingJoints[0].position, this.BendingJoints[1].position, (float) (((double) this.m_burntState - 0.200000002980232) * 5.0));
        if (!this.m_hasStartedBending[0])
        {
          this.m_hasStartedBending[0] = true;
          this.m_randBend = new Vector3(Random.Range(-55f, 55f), Random.Range(-25f, 25f), Random.Range(-25f, 25f));
        }
        this.BendingJoints[1].localEulerAngles = Vector3.Lerp(Vector3.zero, this.m_randBend, (float) (((double) this.m_burntState - 0.200000002980232) * 5.0));
      }
      else if ((double) this.m_burntState >= 0.400000005960464 && (double) this.m_burntState < 0.600000023841858)
      {
        this.FireGO.transform.position = Vector3.Lerp(this.BendingJoints[1].position, this.BendingJoints[2].position, (float) (((double) this.m_burntState - 0.400000005960464) * 5.0));
        if (!this.m_hasStartedBending[1])
        {
          this.m_hasStartedBending[1] = true;
          this.m_randBend = new Vector3(Random.Range(-55f, 55f), Random.Range(-25f, 25f), Random.Range(-25f, 25f));
        }
        this.BendingJoints[2].localEulerAngles = Vector3.Lerp(Vector3.zero, this.m_randBend, (float) (((double) this.m_burntState - 0.400000005960464) * 5.0));
      }
      else if ((double) this.m_burntState >= 0.600000023841858 && (double) this.m_burntState < 0.800000011920929)
      {
        this.FireGO.transform.position = Vector3.Lerp(this.BendingJoints[2].position, this.BendingJoints[3].position, (float) (((double) this.m_burntState - 0.600000023841858) * 5.0));
        if (!this.m_hasStartedBending[2])
        {
          this.m_hasStartedBending[2] = true;
          this.m_randBend = new Vector3(Random.Range(-55f, 55f), Random.Range(-25f, 25f), Random.Range(-25f, 25f));
        }
        this.BendingJoints[3].localEulerAngles = Vector3.Lerp(Vector3.zero, this.m_randBend, (float) (((double) this.m_burntState - 0.600000023841858) * 5.0));
      }
      else
      {
        if ((double) this.m_burntState < 0.800000011920929)
          return;
        this.FireGO.transform.position = Vector3.Lerp(this.BendingJoints[3].position, this.BendingJoints[4].position, (float) (((double) this.m_burntState - 0.800000011920929) * 5.0));
        if (!this.m_hasStartedBending[3])
        {
          this.m_hasStartedBending[3] = true;
          this.m_randBend = new Vector3(Random.Range(-15f, 15f), Random.Range(-25f, 25f), Random.Range(-25f, 25f));
        }
        this.BendingJoints[4].localEulerAngles = Vector3.Lerp(Vector3.zero, this.m_randBend, (float) (((double) this.m_burntState - 0.800000011920929) * 5.0));
      }
    }

    public void Ignite()
    {
      if (this.m_hasBeenLit)
        return;
      this.m_hasBeenLit = true;
      this.m_isBurning = true;
      this.FireGO.SetActive(true);
      this.GetComponent<AudioSource>().PlayOneShot(this.GetComponent<AudioSource>().clip, 0.4f);
      this.m_strikeFrames = 0;
    }

    private new void OnCollisionEnter(Collision col)
    {
      if (!this.m_hasBeenLit)
        return;
      this.m_currentBreakoff = Mathf.Max(this.m_currentBreakoff, this.m_currentBurnPoint - 0.15f);
      this.MatchRenderer.material.SetFloat("_DissolveCutoff", this.m_currentBreakoff);
      if (!this.m_isBurning || !((Object) col.collider.attachedRigidbody != (Object) null))
        return;
      IFVRDamageable fvrDamageable = col.collider.transform.gameObject.GetComponent<IFVRDamageable>() ?? col.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
      if (fvrDamageable != null)
        fvrDamageable.Damage(new Damage()
        {
          Dam_Thermal = 50f,
          Dam_TotalEnergetic = 50f,
          point = col.contacts[0].point,
          hitNormal = col.contacts[0].normal,
          strikeDir = this.transform.forward
        });
      FVRIgnitable component = col.collider.transform.gameObject.GetComponent<FVRIgnitable>();
      if ((Object) component == (Object) null && (Object) col.collider.attachedRigidbody != (Object) null)
        col.collider.attachedRigidbody.gameObject.GetComponent<FVRIgnitable>();
      if (!((Object) component != (Object) null))
        return;
      FXM.Ignite(component, 0.1f);
    }

    private void OnCollisionStay(Collision col)
    {
      if (!this.IsHeld || this.m_hasBeenLit || !((Object) col.contacts[0].thisCollider == (Object) this.MatchHeadCol))
        return;
      if ((Object) col.collider != (Object) this.m_curStrikeCol)
        this.m_curStrikeCol = col.collider;
      if ((double) Vector3.Angle(col.contacts[0].normal, col.relativeVelocity) > 45.0 && (double) col.relativeVelocity.magnitude > 1.0)
        ++this.m_strikeFrames;
      if (this.m_strikeFrames < 10)
        return;
      this.m_hasBeenLit = true;
      this.m_isBurning = true;
      this.FireGO.SetActive(true);
      this.GetComponent<AudioSource>().PlayOneShot(this.GetComponent<AudioSource>().clip);
      this.m_strikeFrames = 0;
    }

    private void OnCollisionExit(Collision col)
    {
      if (this.m_hasBeenLit)
        return;
      this.m_curStrikeCol = (Collider) null;
      this.m_strikeFrames = 0;
    }
  }
}
