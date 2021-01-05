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
			if (args.OriginalObject.gameObject == base.gameObject)
			{
				TransferOurJoint(args);
				RewriteOtherJoints(args);
			}
		}

		private void TransferOurJoint(OnFractureEventArgs args)
		{
			Joint component = args.OriginalObject.GetComponent<Joint>();
			if (!(component != null))
			{
				return;
			}
			Vector3 vector = args.OriginalObject.transform.localToWorldMatrix.MultiplyPoint(component.anchor);
			Transform transform = args.FracturePiecesRootObject.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				Collider component2 = child.GetComponent<Collider>();
				Vector3 vector2 = ((!(component2 != null)) ? base.transform.position : component2.ClosestPointOnBounds(vector));
				if ((vector - child.position).sqrMagnitude < (vector2 - child.position).sqrMagnitude + DistanceTolerance * DistanceTolerance)
				{
					Joint joint = (Joint)child.gameObject.AddComponent(component.GetType());
					string name = joint.name;
					PropertyInfo[] properties = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
					foreach (PropertyInfo propertyInfo in properties)
					{
						if (propertyInfo.CanWrite && propertyInfo.CanRead)
						{
							propertyInfo.SetValue(joint, propertyInfo.GetValue(component, null), null);
						}
					}
					PropertyInfo[] properties2 = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
					foreach (PropertyInfo propertyInfo2 in properties2)
					{
						if (propertyInfo2.CanWrite && propertyInfo2.CanRead && propertyInfo2.GetCustomAttributes(typeof(SerializeField), inherit: true).Length != 0)
						{
							propertyInfo2.SetValue(joint, propertyInfo2.GetValue(component, null), null);
						}
					}
					joint.name = name;
					joint.anchor = child.worldToLocalMatrix.MultiplyPoint(vector);
					Vector3 v = args.OriginalObject.transform.localToWorldMatrix.MultiplyPoint(component.connectedAnchor);
					joint.connectedAnchor = child.worldToLocalMatrix.MultiplyPoint(v);
					Vector3 v2 = args.OriginalObject.transform.localToWorldMatrix.MultiplyVector(component.axis);
					joint.axis = child.worldToLocalMatrix.MultiplyVector(v2).normalized;
				}
				if (child.GetComponent<TransferJointsOnFracture>() == null)
				{
					TransferJointsOnFracture transferJointsOnFracture = child.gameObject.AddComponent<TransferJointsOnFracture>();
					transferJointsOnFracture.IncomingJointsSearchRoot = IncomingJointsSearchRoot;
					transferJointsOnFracture.DistanceTolerance = DistanceTolerance;
				}
			}
		}

		private void RewriteOtherJoints(OnFractureEventArgs args)
		{
			Joint[] array = ((!(IncomingJointsSearchRoot != null)) ? Object.FindObjectsOfType<Joint>() : IncomingJointsSearchRoot.GetComponentsInChildren<Joint>());
			if (array == null)
			{
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i] != null) || !(array[i].connectedBody == args.OriginalObject.GetComponent<Rigidbody>()))
				{
					continue;
				}
				Transform transform = args.FracturePiecesRootObject.transform;
				for (int j = 0; j < transform.childCount; j++)
				{
					Transform child = transform.GetChild(j);
					Vector3 vector = array[i].transform.localToWorldMatrix.MultiplyPoint(array[i].anchor);
					Collider component = child.GetComponent<Collider>();
					Vector3 vector2 = ((!(component != null)) ? base.transform.position : component.ClosestPointOnBounds(vector));
					if ((vector - child.position).sqrMagnitude < (vector2 - child.position).sqrMagnitude + DistanceTolerance * DistanceTolerance)
					{
						array[i].connectedBody = child.GetComponent<Rigidbody>();
					}
				}
			}
		}
	}
}
