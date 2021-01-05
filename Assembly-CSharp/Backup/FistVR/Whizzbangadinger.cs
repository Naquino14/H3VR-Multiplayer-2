// Decompiled with JetBrains decompiler
// Type: FistVR.Whizzbangadinger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class Whizzbangadinger : MonoBehaviour
  {
    public Transform Door;
    public LayerMask LM_IngredientDetect;
    public Transform IngredientDetectCenter;
    public Vector3 IngredientDetectExtends;
    public List<FVRObject> Bangers_Small;
    public List<FVRObject> Bangers_Medium;
    public List<FVRObject> Bangers_Large;
    public AudioEvent AudEvent_DingSuccess;
    public AudioEvent AudEvent_DingFailure;
    public Transform SpawnPoint;
    public Transform Accordian;
    private float m_accordianLerp;
    private bool m_isAnimating;
    public AnimationCurve AccordianCurve;
    public float AccordianSpeed = 1f;
    public AudioEvent AudEvent_Accordian;
    public BangerDetonator Detonator;

    private bool IsDoorShut() => (double) Vector3.Angle(this.Door.forward, -this.transform.right) < 2.0;

    private void SuccessSound() => SM.PlayGenericSound(this.AudEvent_DingSuccess, this.transform.position);

    private void FailSound() => SM.PlayGenericSound(this.AudEvent_DingFailure, this.transform.position);

    public void Ding(int v)
    {
      if (!this.IsDoorShut())
      {
        this.FailSound();
      }
      else
      {
        bool flag1 = false;
        bool flag2 = false;
        RotrwBangerJunk rotrwBangerJunk1 = (RotrwBangerJunk) null;
        RotrwBangerJunk rotrwBangerJunk2 = (RotrwBangerJunk) null;
        List<FVRPhysicalObject> fvrPhysicalObjectList = new List<FVRPhysicalObject>();
        Collider[] colliderArray = Physics.OverlapBox(this.IngredientDetectCenter.position, this.IngredientDetectExtends, this.IngredientDetectCenter.rotation, (int) this.LM_IngredientDetect, QueryTriggerInteraction.Collide);
        List<Rigidbody> rigidbodyList = new List<Rigidbody>();
        for (int index = 0; index < colliderArray.Length; ++index)
        {
          Rigidbody attachedRigidbody = colliderArray[index].attachedRigidbody;
          if ((Object) attachedRigidbody != (Object) null && !rigidbodyList.Contains(attachedRigidbody))
            rigidbodyList.Add(attachedRigidbody);
        }
        for (int index = rigidbodyList.Count - 1; index >= 0; --index)
        {
          RotrwBangerJunk component = rigidbodyList[index].gameObject.GetComponent<RotrwBangerJunk>();
          if ((Object) component != (Object) null)
          {
            if (component.Type == RotrwBangerJunk.BangerJunkType.Bucket || component.Type == RotrwBangerJunk.BangerJunkType.CoffeeCan || component.Type == RotrwBangerJunk.BangerJunkType.TinCan)
            {
              flag1 = true;
              if ((Object) rotrwBangerJunk1 == (Object) null)
                rotrwBangerJunk1 = component;
              else if (component.ContainerSize > rotrwBangerJunk1.ContainerSize)
                rotrwBangerJunk1 = component;
              rigidbodyList.RemoveAt(index);
            }
            else if (component.Type == RotrwBangerJunk.BangerJunkType.BangSnaps || component.Type == RotrwBangerJunk.BangerJunkType.EggTimer || (component.Type == RotrwBangerJunk.BangerJunkType.FishFinder || component.Type == RotrwBangerJunk.BangerJunkType.Radio))
            {
              flag2 = true;
              if ((Object) rotrwBangerJunk2 == (Object) null)
                rotrwBangerJunk2 = component;
              rigidbodyList.RemoveAt(index);
            }
          }
        }
        if (!flag1 || !flag2)
        {
          this.FailSound();
        }
        else
        {
          int num1 = 2;
          if (rotrwBangerJunk1.ContainerSize == 1)
            num1 = 5;
          else if (rotrwBangerJunk1.ContainerSize == 2)
            num1 = 9999;
          int num2 = 0;
          for (int index = 0; index < rigidbodyList.Count && num2 < num1; ++index)
          {
            FVRPhysicalObject component = rigidbodyList[index].gameObject.GetComponent<FVRPhysicalObject>();
            if ((Object) component.ObjectWrapper != (Object) null)
            {
              ++num2;
              fvrPhysicalObjectList.Add(component);
            }
          }
          if (num2 > 0)
          {
            int num3 = 0;
            FVRObject fvrObject = (FVRObject) null;
            int index1 = 0;
            int matIndex = rotrwBangerJunk1.MatIndex;
            switch (rotrwBangerJunk1.Type)
            {
              case RotrwBangerJunk.BangerJunkType.TinCan:
                num3 = 0;
                break;
              case RotrwBangerJunk.BangerJunkType.CoffeeCan:
                num3 = 1;
                break;
              case RotrwBangerJunk.BangerJunkType.Bucket:
                num3 = 2;
                break;
            }
            switch (rotrwBangerJunk2.Type)
            {
              case RotrwBangerJunk.BangerJunkType.Radio:
                index1 = 2;
                break;
              case RotrwBangerJunk.BangerJunkType.FishFinder:
                index1 = 3;
                break;
              case RotrwBangerJunk.BangerJunkType.BangSnaps:
                index1 = 0;
                break;
              case RotrwBangerJunk.BangerJunkType.EggTimer:
                index1 = 1;
                break;
            }
            switch (num3)
            {
              case 0:
                fvrObject = this.Bangers_Small[index1];
                break;
              case 1:
                fvrObject = this.Bangers_Medium[index1];
                break;
              case 2:
                fvrObject = this.Bangers_Large[index1];
                break;
            }
            Banger component = Object.Instantiate<GameObject>(fvrObject.GetGameObject(), this.SpawnPoint.position, this.SpawnPoint.rotation).GetComponent<Banger>();
            if ((Object) GM.ZMaster != (Object) null)
              GM.ZMaster.FlagM.AddToFlag("s_c", 1);
            if (matIndex > 0)
              component.SetMat(matIndex);
            for (int index2 = 0; index2 < fvrPhysicalObjectList.Count; ++index2)
              component.LoadPayload(fvrPhysicalObjectList[index2]);
            component.Complete();
            this.m_accordianLerp = 0.0f;
            this.m_isAnimating = true;
            SM.PlayGenericSound(this.AudEvent_Accordian, this.transform.position);
            this.SuccessSound();
            if (component.BType == Banger.BangerType.Remote)
              this.Detonator.RegisterBanger(component);
            Object.Destroy((Object) rotrwBangerJunk1.gameObject);
            Object.Destroy((Object) rotrwBangerJunk2.gameObject);
            for (int index2 = fvrPhysicalObjectList.Count - 1; index2 >= 0; --index2)
              Object.Destroy((Object) fvrPhysicalObjectList[index2].gameObject);
            fvrPhysicalObjectList.Clear();
          }
          else
            this.FailSound();
        }
      }
    }

    private void Update() => this.Accordianing();

    private void Accordianing()
    {
      if (!this.m_isAnimating)
        return;
      this.m_accordianLerp += Time.deltaTime;
      this.Accordian.localScale = new Vector3(1f, this.AccordianCurve.Evaluate(this.m_accordianLerp), 1f);
      if ((double) this.m_accordianLerp <= 1.0)
        return;
      this.m_isAnimating = false;
    }
  }
}
