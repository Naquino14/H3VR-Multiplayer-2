// Decompiled with JetBrains decompiler
// Type: FistVR.PieOMizer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class PieOMizer : MonoBehaviour
  {
    public Transform Door;
    public LayerMask LM_IngredientDetect;
    public List<FVRObject> HerbIngredients;
    public Transform IngredientDetectCenter;
    public Vector3 IngredientDetectExtends;
    public List<FVRObject> Pie_Output;
    public Transform Pie_Output_Point;
    public AudioEvent AudEvent_BakeSuccess;
    public AudioEvent AudEvent_BakeFailure;

    private bool IsDoorShut() => (double) Vector3.Angle(this.Door.forward, this.transform.up) < 5.0;

    public void Bake(int v)
    {
      if (!this.IsDoorShut())
      {
        SM.PlayGenericSound(this.AudEvent_BakeFailure, this.transform.position);
      }
      else
      {
        bool flag = false;
        Collider[] colliderArray = Physics.OverlapBox(this.IngredientDetectCenter.position, this.IngredientDetectExtends, this.IngredientDetectCenter.rotation, (int) this.LM_IngredientDetect, QueryTriggerInteraction.Collide);
        List<RotrwMeatCore> rotrwMeatCoreList = new List<RotrwMeatCore>();
        for (int index = 0; index < colliderArray.Length; ++index)
        {
          if ((Object) colliderArray[index].GetComponent<RotrwMeatCore>() != (Object) null)
          {
            rotrwMeatCoreList.Add(colliderArray[index].GetComponent<RotrwMeatCore>());
            Debug.Log((object) ("Detected: " + colliderArray[index].gameObject.name));
          }
        }
        int index1 = 0;
        if (rotrwMeatCoreList.Count == 2)
        {
          if (rotrwMeatCoreList[0].Type == RotrwMeatCore.CoreType.Tasty)
          {
            switch (rotrwMeatCoreList[1].Type)
            {
              case RotrwMeatCore.CoreType.Tasty:
                index1 = 0;
                flag = true;
                break;
              case RotrwMeatCore.CoreType.Moldy:
                index1 = 2;
                flag = true;
                break;
              case RotrwMeatCore.CoreType.Zippy:
                index1 = 3;
                flag = true;
                break;
              case RotrwMeatCore.CoreType.Weighty:
                index1 = 4;
                flag = true;
                break;
              case RotrwMeatCore.CoreType.Shiny:
                index1 = 1;
                flag = true;
                break;
            }
          }
          else if (rotrwMeatCoreList[1].Type == RotrwMeatCore.CoreType.Tasty)
          {
            switch (rotrwMeatCoreList[0].Type)
            {
              case RotrwMeatCore.CoreType.Tasty:
                index1 = 0;
                flag = true;
                break;
              case RotrwMeatCore.CoreType.Moldy:
                index1 = 2;
                flag = true;
                break;
              case RotrwMeatCore.CoreType.Zippy:
                index1 = 3;
                flag = true;
                break;
              case RotrwMeatCore.CoreType.Weighty:
                index1 = 4;
                flag = true;
                break;
              case RotrwMeatCore.CoreType.Shiny:
                index1 = 1;
                flag = true;
                break;
            }
          }
        }
        if (flag)
        {
          Object.Destroy((Object) rotrwMeatCoreList[0].gameObject);
          Object.Destroy((Object) rotrwMeatCoreList[1].gameObject);
          rotrwMeatCoreList.Clear();
          Object.Instantiate<GameObject>(this.Pie_Output[index1].GetGameObject(), this.Pie_Output_Point.position, this.Pie_Output_Point.rotation);
        }
        if (flag)
        {
          if ((Object) GM.ZMaster != (Object) null)
            GM.ZMaster.FlagM.AddToFlag("s_c", 1);
          SM.PlayGenericSound(this.AudEvent_BakeSuccess, this.transform.position);
        }
        else
          SM.PlayGenericSound(this.AudEvent_BakeFailure, this.transform.position);
      }
    }
  }
}
