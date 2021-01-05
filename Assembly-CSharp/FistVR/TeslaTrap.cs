// Decompiled with JetBrains decompiler
// Type: FistVR.TeslaTrap
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TeslaTrap : ZosigQuestManager
  {
    public List<GameObject> LightningFX;
    public Transform LightningOrigin;
    public LayerMask LM_SosigDetect;
    public float range;
    private bool m_isEnabled;
    public AudioSource Buzzing;
    public GameObject AmbientVFX;
    private ZosigGameManager M;
    public string Flag;
    public int ValueWhenOn;
    public AudioEvent AudEvent_Lightning;
    private bool m_isGassed;
    private float m_checkTick = 0.2f;

    public override void Init(ZosigGameManager m) => this.M = m;

    private void Start()
    {
    }

    public void TurnOn() => this.m_isGassed = true;

    public void TurnOff() => this.m_isGassed = false;

    private void Update()
    {
      this.m_checkTick -= Time.deltaTime;
      if ((double) this.m_checkTick >= 0.0)
        return;
      this.m_checkTick = Random.Range(0.3f, 0.8f);
      this.Check();
    }

    public void ON()
    {
      if (!this.m_isEnabled && (Object) GM.ZMaster != (Object) null)
        GM.ZMaster.FlagM.AddToFlag("s_t", 1);
      this.M.FlagM.SetFlag(this.Flag, this.ValueWhenOn);
      this.m_isEnabled = true;
    }

    public void OFF() => this.m_isEnabled = false;

    private void Check()
    {
      if (this.m_isEnabled && this.m_isGassed)
      {
        float num = Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position);
        if ((double) num < 40.0)
        {
          if (!this.Buzzing.isPlaying)
            this.Buzzing.Play();
        }
        else if (this.Buzzing.isPlaying)
          this.Buzzing.Stop();
        if ((double) num < 100.0)
          this.AmbientVFX.SetActive(true);
        else
          this.AmbientVFX.SetActive(false);
        Collider[] colliderArray = Physics.OverlapSphere(this.transform.position, this.range, (int) this.LM_SosigDetect, QueryTriggerInteraction.Collide);
        bool flag = false;
        for (int index = 0; index < colliderArray.Length; ++index)
        {
          if (!((Object) colliderArray[index].attachedRigidbody == (Object) null))
          {
            SosigLink component = colliderArray[index].attachedRigidbody.gameObject.GetComponent<SosigLink>();
            if ((Object) component != (Object) null && (Object) component.S != (Object) null)
            {
              component.S.Shudder(2f);
              Vector3 forward = component.transform.position - this.LightningOrigin.position;
              GameObject gameObject = Object.Instantiate<GameObject>(this.LightningFX[Random.Range(0, this.LightningFX.Count)], this.LightningOrigin.position, Quaternion.LookRotation(forward, Random.onUnitSphere));
              float magnitude = forward.magnitude;
              gameObject.transform.localScale = new Vector3(magnitude * 1.2f, magnitude * 1.2f, magnitude * 1.2f);
              flag = true;
              break;
            }
            break;
          }
        }
        if (!flag || (double) num >= 60.0)
          return;
        SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, this.AudEvent_Lightning, this.transform.position, num / 343f);
      }
      else
      {
        if (this.Buzzing.isPlaying)
          this.Buzzing.Stop();
        this.AmbientVFX.SetActive(false);
      }
    }
  }
}
