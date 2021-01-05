using UnityEngine;

namespace Valve.VR.Extras
{
	public class SteamVR_LaserPointer : MonoBehaviour
	{
		public SteamVR_Behaviour_Pose pose;

		public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

		public bool active = true;

		public Color color;

		public float thickness = 0.002f;

		public Color clickColor = Color.green;

		public GameObject holder;

		public GameObject pointer;

		private bool isActive;

		public bool addRigidBody;

		public Transform reference;

		private Transform previousContact;

		public event PointerEventHandler PointerIn;

		public event PointerEventHandler PointerOut;

		public event PointerEventHandler PointerClick;

		private void Start()
		{
			if (pose == null)
			{
				pose = GetComponent<SteamVR_Behaviour_Pose>();
			}
			if (pose == null)
			{
				Debug.LogError("No SteamVR_Behaviour_Pose component found on this object");
			}
			if (interactWithUI == null)
			{
				Debug.LogError("No ui interaction action has been set on this component.");
			}
			holder = new GameObject();
			holder.transform.parent = base.transform;
			holder.transform.localPosition = Vector3.zero;
			holder.transform.localRotation = Quaternion.identity;
			pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
			pointer.transform.parent = holder.transform;
			pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
			pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
			pointer.transform.localRotation = Quaternion.identity;
			BoxCollider component = pointer.GetComponent<BoxCollider>();
			if (addRigidBody)
			{
				if ((bool)component)
				{
					component.isTrigger = true;
				}
				Rigidbody rigidbody = pointer.AddComponent<Rigidbody>();
				rigidbody.isKinematic = true;
			}
			else if ((bool)component)
			{
				Object.Destroy(component);
			}
			Material material = new Material(Shader.Find("Unlit/Color"));
			material.SetColor("_Color", color);
			pointer.GetComponent<MeshRenderer>().material = material;
		}

		public virtual void OnPointerIn(PointerEventArgs e)
		{
			if (this.PointerIn != null)
			{
				this.PointerIn(this, e);
			}
		}

		public virtual void OnPointerClick(PointerEventArgs e)
		{
			if (this.PointerClick != null)
			{
				this.PointerClick(this, e);
			}
		}

		public virtual void OnPointerOut(PointerEventArgs e)
		{
			if (this.PointerOut != null)
			{
				this.PointerOut(this, e);
			}
		}

		private void Update()
		{
			if (!isActive)
			{
				isActive = true;
				base.transform.GetChild(0).gameObject.SetActive(value: true);
			}
			float num = 100f;
			Ray ray = new Ray(base.transform.position, base.transform.forward);
			RaycastHit hitInfo;
			bool flag = Physics.Raycast(ray, out hitInfo);
			if ((bool)previousContact && previousContact != hitInfo.transform)
			{
				PointerEventArgs e = default(PointerEventArgs);
				e.fromInputSource = pose.inputSource;
				e.distance = 0f;
				e.flags = 0u;
				e.target = previousContact;
				OnPointerOut(e);
				previousContact = null;
			}
			if (flag && previousContact != hitInfo.transform)
			{
				PointerEventArgs e2 = default(PointerEventArgs);
				e2.fromInputSource = pose.inputSource;
				e2.distance = hitInfo.distance;
				e2.flags = 0u;
				e2.target = hitInfo.transform;
				OnPointerIn(e2);
				previousContact = hitInfo.transform;
			}
			if (!flag)
			{
				previousContact = null;
			}
			if (flag && hitInfo.distance < 100f)
			{
				num = hitInfo.distance;
			}
			if (flag && interactWithUI.GetStateUp(pose.inputSource))
			{
				PointerEventArgs e3 = default(PointerEventArgs);
				e3.fromInputSource = pose.inputSource;
				e3.distance = hitInfo.distance;
				e3.flags = 0u;
				e3.target = hitInfo.transform;
				OnPointerClick(e3);
			}
			if (interactWithUI != null && interactWithUI.GetState(pose.inputSource))
			{
				pointer.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, num);
				pointer.GetComponent<MeshRenderer>().material.color = clickColor;
			}
			else
			{
				pointer.transform.localScale = new Vector3(thickness, thickness, num);
				pointer.GetComponent<MeshRenderer>().material.color = color;
			}
			pointer.transform.localPosition = new Vector3(0f, 0f, num / 2f);
		}
	}
}
