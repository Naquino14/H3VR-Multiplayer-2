// Decompiled with JetBrains decompiler
// Type: FistVR.TeslaWand
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TeslaWand : FVRMeleeWeapon
  {
    public List<GameObject> LightningFX;
    public Transform LightningOrigin;
    public LayerMask LM_SosigDetect;
    public LayerMask LM_EnvBlock;
    public float range;
    public AudioEvent AudEvent_Lightning;
    public GameObject SplodeOut;
    private int m_charges = 10;

    protected override void Start()
    {
      base.Start();
      this.m_charges = Random.Range(20, 500);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!((Object) this.AltGrip != (Object) null) || this.IsAltHeld || !hand.Input.TriggerDown)
        return;
      this.Blast();
    }

    private void Blast()
    {
      Debug.Log((object) "b");
      Collider[] colliderArray = Physics.OverlapSphere(GM.CurrentPlayerBody.Head.position + GM.CurrentPlayerBody.Head.forward * 8f, 10f, (int) this.LM_SosigDetect, QueryTriggerInteraction.Collide);
      Debug.Log((object) colliderArray.Length);
      bool flag = false;
      for (int index = 0; index < colliderArray.Length; ++index)
      {
        if (!((Object) colliderArray[index].attachedRigidbody == (Object) null))
        {
          SosigLink component1 = colliderArray[index].attachedRigidbody.gameObject.GetComponent<SosigLink>();
          Vector3 pos = Vector3.zero;
          if ((Object) component1 != (Object) null && (Object) component1.S != (Object) null)
          {
            if (component1.S.BodyState != Sosig.SosigBodyState.Dead && !Physics.Linecast(this.LightningOrigin.position, component1.transform.position, (int) this.LM_EnvBlock))
            {
              component1.S.Shudder(2f);
              component1.R.AddForce(Random.onUnitSphere * Random.Range(5f, 8f), ForceMode.Impulse);
              Vector3 forward = component1.transform.position - this.LightningOrigin.position;
              GameObject gameObject = Object.Instantiate<GameObject>(this.LightningFX[Random.Range(0, this.LightningFX.Count)], this.LightningOrigin.position, Quaternion.LookRotation(forward, Random.onUnitSphere));
              float magnitude = forward.magnitude;
              FVRIgnitable component2 = component1.gameObject.GetComponent<FVRIgnitable>();
              if ((Object) component2 != (Object) null)
                FXM.Ignite(component2, 1f);
              gameObject.transform.localScale = new Vector3(magnitude * 1.2f, magnitude * 1.2f, magnitude * 1.2f);
              flag = true;
              pos = component1.transform.position;
            }
            else
              continue;
          }
          if (!flag)
            break;
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Lightning, this.LightningOrigin.position);
          --this.m_charges;
          if (this.m_charges < 0)
          {
            Object.Instantiate<GameObject>(this.SplodeOut, this.transform.position, this.transform.rotation);
            this.ForceBreakInteraction();
            Object.Destroy((Object) this.gameObject);
          }
          FXM.InitiateMuzzleFlash(pos, Random.onUnitSphere, 10f, Color.yellow, Random.Range(1f, 5f));
          break;
        }
      }
    }
  }
}
