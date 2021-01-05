// Decompiled with JetBrains decompiler
// Type: FistVR.LumberjackMailbox
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class LumberjackMailbox : MonoBehaviour
  {
    public AudioEvent AudEvent_Success;
    public Transform Door;
    public LayerMask LM_IngredientDetect;
    public Transform IngredientDetectCenter;
    public Vector3 IngredientDetectExtends;
    public FVRObject n1;
    public FVRObject n2;
    public FVRObject n3;
    public FVRObject n4;
    public Transform n4pos;
    private bool m_wasDoorShut = true;
    private bool m_hasSpawned;

    private bool IsDoorShut() => (double) Vector3.Angle(this.Door.forward, this.transform.up) < 2.0;

    private void Update()
    {
      if (this.m_hasSpawned)
        return;
      bool flag = this.IsDoorShut();
      if (flag && !this.m_wasDoorShut)
        this.Check();
      this.m_wasDoorShut = flag;
    }

    private void Check()
    {
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      Collider[] colliderArray = Physics.OverlapBox(this.IngredientDetectCenter.position, this.IngredientDetectExtends, this.IngredientDetectCenter.rotation, (int) this.LM_IngredientDetect, QueryTriggerInteraction.Collide);
      List<Rigidbody> rigidbodyList = new List<Rigidbody>();
      List<FVRPhysicalObject> fvrPhysicalObjectList = new List<FVRPhysicalObject>();
      for (int index = 0; index < colliderArray.Length; ++index)
      {
        Rigidbody attachedRigidbody = colliderArray[index].attachedRigidbody;
        if ((Object) attachedRigidbody != (Object) null && !rigidbodyList.Contains(attachedRigidbody))
          rigidbodyList.Add(attachedRigidbody);
      }
      for (int index = rigidbodyList.Count - 1; index >= 0; --index)
      {
        FVRPhysicalObject component = rigidbodyList[index].gameObject.GetComponent<FVRPhysicalObject>();
        if ((Object) component != (Object) null && (Object) component.ObjectWrapper != (Object) null)
        {
          if (component.ObjectWrapper.ItemID == this.n1.ItemID)
          {
            flag1 = true;
            fvrPhysicalObjectList.Add(component);
          }
          else if (component.ObjectWrapper.ItemID == this.n2.ItemID)
          {
            flag2 = true;
            fvrPhysicalObjectList.Add(component);
          }
          else if (component.ObjectWrapper.ItemID == this.n3.ItemID)
          {
            flag3 = true;
            fvrPhysicalObjectList.Add(component);
          }
        }
      }
      if (!flag1 || !flag2 || !flag3)
        return;
      for (int index = fvrPhysicalObjectList.Count - 1; index >= 0; --index)
        Object.Destroy((Object) fvrPhysicalObjectList[index].gameObject);
      SM.PlayGenericSound(this.AudEvent_Success, this.transform.position);
      Object.Instantiate<GameObject>(this.n4.GetGameObject(), this.n4pos.position, this.n4pos.rotation);
      this.m_hasSpawned = true;
    }
  }
}
