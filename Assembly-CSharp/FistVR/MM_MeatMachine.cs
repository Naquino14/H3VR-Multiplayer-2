// Decompiled with JetBrains decompiler
// Type: FistVR.MM_MeatMachine
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class MM_MeatMachine : MonoBehaviour
  {
    public ParticleSystem PSystem_Sparks;
    public ParticleSystem PSystem_Sauce;
    public Transform[] Rollers;
    private float m_roll;
    public AudioSource Aud;
    public AudioClip Aud_Grind;
    public FVRObject[] Exclusions;
    private HashSet<FVRObject> hashy = new HashSet<FVRObject>();
    private bool m_shouldSave;
    private List<FVRPhysicalObject> objs_list = new List<FVRPhysicalObject>();
    private HashSet<FVRPhysicalObject> objs_hash = new HashSet<FVRPhysicalObject>();
    private float saveTick = 1f;
    private float GrindTick = 0.1f;

    private void Start()
    {
      for (int index = 0; index < this.Exclusions.Length; ++index)
        this.hashy.Add(this.Exclusions[index]);
    }

    private void Update()
    {
      this.m_roll += Time.deltaTime * 1720f;
      this.m_roll = Mathf.Repeat(this.m_roll, 360f);
      this.Rollers[0].localEulerAngles = new Vector3(0.0f, 0.0f, -this.m_roll);
      this.Rollers[1].localEulerAngles = new Vector3(0.0f, 0.0f, this.m_roll);
      if ((double) this.GrindTick > 0.0)
        this.GrindTick -= Time.deltaTime;
      if ((double) this.saveTick > 0.0)
      {
        this.saveTick -= Time.deltaTime;
        if (this.m_shouldSave)
          this.m_shouldSave = false;
      }
      if (this.objs_list.Count <= 0)
        return;
      for (int index = this.objs_list.Count - 1; index >= 0; --index)
      {
        if ((Object) this.objs_list[index] == (Object) null)
        {
          this.objs_hash.Remove(this.objs_list[index]);
          this.objs_list.RemoveAt(index);
        }
      }
    }

    private void OnTriggerEnter(Collider col) => this.CheckCol(col);

    private void CheckCol(Collider col)
    {
      if ((Object) col.attachedRigidbody == (Object) null)
        return;
      FVRPhysicalObject component = col.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
      if ((Object) component == (Object) null || (Object) component.QuickbeltSlot != (Object) null || (component.m_isHardnessed || !this.objs_hash.Add(component)))
        return;
      this.objs_list.Add(component);
      if ((double) this.GrindTick <= 0.0 && (Object) this.Aud_Grind != (Object) null)
      {
        this.GrindTick = Random.Range(0.2f, 0.5f);
        this.PSystem_Sparks.Emit(50);
        this.Aud.pitch = Random.Range(0.85f, 1.05f);
        this.Aud.PlayOneShot(this.Aud_Grind, 0.5f);
      }
      int num = 0;
      if ((Object) component.ObjectWrapper != (Object) null && this.hashy.Contains(component.ObjectWrapper))
      {
        num = 0;
      }
      else
      {
        switch (component)
        {
          case FVRFireArm _:
            break;
          case FVRFireArmMagazine _:
            num = 0;
            break;
          case FVRFireArmAttachment _:
            num = 20;
            break;
          case FVRMeleeWeapon _:
            num = 50;
            break;
          case FVRFireArmRound _:
            num = 0;
            break;
          case MM_Currency _:
            int type = (int) (component as MM_Currency).Type;
            num = type < 15 ? (type < 9 ? (type < 5 ? (type < 3 ? 2 : 20) : 50) : 200) : 500;
            break;
          default:
            num = 0;
            break;
        }
      }
      if (num > 0)
      {
        this.PSystem_Sauce.Emit(num);
        this.m_shouldSave = true;
        GM.Omni.OmniUnlocks.GainCurrency(num);
      }
      Object.Destroy((Object) component.gameObject);
    }
  }
}
