// Decompiled with JetBrains decompiler
// Type: FistVR.BaitCloud
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class BaitCloud : MonoBehaviour
  {
    public BaitCloud.PieEffect Effect;
    public float EffectRadius = 10f;
    public LayerMask LM_Collide_AIBody;
    private float m_tickTilCheck = 0.02f;
    public float m_timeToRemoval = 30f;
    private HashSet<Sosig> m_baitedSosigs = new HashSet<Sosig>();

    private void Start()
    {
    }

    private void Update()
    {
      this.m_tickTilCheck -= Time.deltaTime;
      if ((double) this.m_tickTilCheck <= 0.0)
      {
        this.m_tickTilCheck = Random.Range(0.1f, 0.2f);
        this.Check();
      }
      this.m_timeToRemoval -= Time.deltaTime;
      if ((double) this.m_timeToRemoval > 0.0)
        return;
      this.Unbait();
      Object.Destroy((Object) this.gameObject);
    }

    private void Check()
    {
      if (this.Effect == BaitCloud.PieEffect.Attractor)
      {
        this.m_baitedSosigs.Clear();
        Collider[] colliderArray = Physics.OverlapSphere(this.transform.position, this.EffectRadius, (int) this.LM_Collide_AIBody, QueryTriggerInteraction.Collide);
        for (int index = 0; index < colliderArray.Length; ++index)
        {
          if (!((Object) colliderArray[index].attachedRigidbody == (Object) null))
          {
            SosigLink component = colliderArray[index].attachedRigidbody.gameObject.GetComponent<SosigLink>();
            if (component.S.E.IFFCode != 0 && component.S.E.IFFCode != 2)
              this.m_baitedSosigs.Add(component.S);
          }
        }
        foreach (Sosig baitedSosig in this.m_baitedSosigs)
        {
          Vector3 onUnitSphere = Random.onUnitSphere;
          onUnitSphere.y = 0.0f;
          baitedSosig.CommandAssaultPoint(this.transform.position + onUnitSphere * 1.2f);
          baitedSosig.SetCurrentOrder(Sosig.SosigOrder.Assault);
          baitedSosig.FallbackOrder = Sosig.SosigOrder.Assault;
          baitedSosig.SetAssaultPointOverrideDistance(0.1f);
        }
      }
      else if (this.Effect == BaitCloud.PieEffect.Repellant)
      {
        Collider[] colliderArray = Physics.OverlapSphere(this.transform.position, this.EffectRadius, (int) this.LM_Collide_AIBody, QueryTriggerInteraction.Collide);
        for (int index = 0; index < colliderArray.Length; ++index)
        {
          if (!((Object) colliderArray[index].attachedRigidbody == (Object) null))
            colliderArray[index].attachedRigidbody.gameObject.GetComponent<SosigLink>().S.Blind(1f);
        }
      }
      else if (this.Effect == BaitCloud.PieEffect.Stun)
      {
        Collider[] colliderArray = Physics.OverlapSphere(this.transform.position, this.EffectRadius, (int) this.LM_Collide_AIBody, QueryTriggerInteraction.Collide);
        for (int index = 0; index < colliderArray.Length; ++index)
        {
          if (!((Object) colliderArray[index].attachedRigidbody == (Object) null))
          {
            SosigLink component = colliderArray[index].attachedRigidbody.gameObject.GetComponent<SosigLink>();
            component.S.Stun(2f);
            component.S.Shudder(0.25f);
          }
        }
      }
      else if (this.Effect == BaitCloud.PieEffect.Freeze)
      {
        Collider[] colliderArray = Physics.OverlapSphere(this.transform.position, this.EffectRadius, (int) this.LM_Collide_AIBody, QueryTriggerInteraction.Collide);
        for (int index = 0; index < colliderArray.Length; ++index)
        {
          if (!((Object) colliderArray[index].attachedRigidbody == (Object) null))
          {
            SosigLink component = colliderArray[index].attachedRigidbody.gameObject.GetComponent<SosigLink>();
            if ((Object) component.S != (Object) null)
              component.S.ActivatePower(PowerupType.ChillOut, PowerUpIntensity.High, PowerUpDuration.Blip, false, false);
          }
        }
      }
      else
      {
        if (this.Effect != BaitCloud.PieEffect.IFFScramble)
          return;
        Collider[] colliderArray = Physics.OverlapSphere(this.transform.position, this.EffectRadius, (int) this.LM_Collide_AIBody, QueryTriggerInteraction.Collide);
        for (int index = 0; index < colliderArray.Length; ++index)
        {
          if (!((Object) colliderArray[index].attachedRigidbody == (Object) null))
          {
            SosigLink component = colliderArray[index].attachedRigidbody.gameObject.GetComponent<SosigLink>();
            if ((Object) component.S != (Object) null && component.S.BodyState != Sosig.SosigBodyState.Dead)
              component.S.E.IFFCode = Random.Range(3, 20);
          }
        }
      }
    }

    private void Unbait()
    {
      if (this.Effect == BaitCloud.PieEffect.Attractor || this.Effect == BaitCloud.PieEffect.Repellant)
      {
        foreach (Sosig baitedSosig in this.m_baitedSosigs)
        {
          if ((Object) baitedSosig != (Object) null)
          {
            baitedSosig.SetAssaultPointOverrideDistance(50f);
            baitedSosig.SetCurrentOrder(Sosig.SosigOrder.Wander);
            baitedSosig.FallbackOrder = Sosig.SosigOrder.Wander;
          }
        }
      }
      this.m_baitedSosigs.Clear();
    }

    public enum PieEffect
    {
      Attractor,
      Repellant,
      Stun,
      Freeze,
      IFFScramble,
    }
  }
}
