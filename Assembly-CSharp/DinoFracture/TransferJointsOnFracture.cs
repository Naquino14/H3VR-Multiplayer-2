// Decompiled with JetBrains decompiler
// Type: DinoFracture.TransferJointsOnFracture
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Reflection;
using UnityEngine;

namespace DinoFracture
{
  public class TransferJointsOnFracture : MonoBehaviour
  {
    public Transform IncomingJointsSearchRoot;
    public float DistanceTolerance = 0.05f;

    private void OnFracture(OnFractureEventArgs args)
    {
      if (!((Object) args.OriginalObject.gameObject == (Object) this.gameObject))
        return;
      this.TransferOurJoint(args);
      this.RewriteOtherJoints(args);
    }

    private void TransferOurJoint(OnFractureEventArgs args)
    {
      Joint component1 = args.OriginalObject.GetComponent<Joint>();
      if (!((Object) component1 != (Object) null))
        return;
      Vector3 vector3_1 = args.OriginalObject.transform.localToWorldMatrix.MultiplyPoint(component1.anchor);
      Transform transform = args.FracturePiecesRootObject.transform;
      for (int index = 0; index < transform.childCount; ++index)
      {
        Transform child = transform.GetChild(index);
        Collider component2 = child.GetComponent<Collider>();
        Vector3 vector3_2 = !((Object) component2 != (Object) null) ? this.transform.position : component2.ClosestPointOnBounds(vector3_1);
        if ((double) (vector3_1 - child.position).sqrMagnitude < (double) (vector3_2 - child.position).sqrMagnitude + (double) this.DistanceTolerance * (double) this.DistanceTolerance)
        {
          Joint joint = (Joint) child.gameObject.AddComponent(component1.GetType());
          string name = joint.name;
          foreach (PropertyInfo property in component1.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
          {
            if (property.CanWrite && property.CanRead)
              property.SetValue((object) joint, property.GetValue((object) component1, (object[]) null), (object[]) null);
          }
          foreach (PropertyInfo property in component1.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic))
          {
            if (property.CanWrite && property.CanRead && property.GetCustomAttributes(typeof (SerializeField), true).Length != 0)
              property.SetValue((object) joint, property.GetValue((object) component1, (object[]) null), (object[]) null);
          }
          joint.name = name;
          joint.anchor = child.worldToLocalMatrix.MultiplyPoint(vector3_1);
          Vector3 v1 = args.OriginalObject.transform.localToWorldMatrix.MultiplyPoint(component1.connectedAnchor);
          joint.connectedAnchor = child.worldToLocalMatrix.MultiplyPoint(v1);
          Vector3 v2 = args.OriginalObject.transform.localToWorldMatrix.MultiplyVector(component1.axis);
          joint.axis = child.worldToLocalMatrix.MultiplyVector(v2).normalized;
        }
        if ((Object) child.GetComponent<TransferJointsOnFracture>() == (Object) null)
        {
          TransferJointsOnFracture jointsOnFracture = child.gameObject.AddComponent<TransferJointsOnFracture>();
          jointsOnFracture.IncomingJointsSearchRoot = this.IncomingJointsSearchRoot;
          jointsOnFracture.DistanceTolerance = this.DistanceTolerance;
        }
      }
    }

    private void RewriteOtherJoints(OnFractureEventArgs args)
    {
      Joint[] jointArray = !((Object) this.IncomingJointsSearchRoot != (Object) null) ? Object.FindObjectsOfType<Joint>() : this.IncomingJointsSearchRoot.GetComponentsInChildren<Joint>();
      if (jointArray == null)
        return;
      for (int index1 = 0; index1 < jointArray.Length; ++index1)
      {
        if ((Object) jointArray[index1] != (Object) null && (Object) jointArray[index1].connectedBody == (Object) args.OriginalObject.GetComponent<Rigidbody>())
        {
          Transform transform = args.FracturePiecesRootObject.transform;
          for (int index2 = 0; index2 < transform.childCount; ++index2)
          {
            Transform child = transform.GetChild(index2);
            Vector3 position = jointArray[index1].transform.localToWorldMatrix.MultiplyPoint(jointArray[index1].anchor);
            Collider component = child.GetComponent<Collider>();
            Vector3 vector3 = !((Object) component != (Object) null) ? this.transform.position : component.ClosestPointOnBounds(position);
            if ((double) (position - child.position).sqrMagnitude < (double) (vector3 - child.position).sqrMagnitude + (double) this.DistanceTolerance * (double) this.DistanceTolerance)
              jointArray[index1].connectedBody = child.GetComponent<Rigidbody>();
          }
        }
      }
    }
  }
}
