using UnityEngine;

public class fixchildissue : MonoBehaviour
{
	public Transform Parent;

	public Transform Child;

	[ContextMenu("FixMe")]
	public void FixMe()
	{
		Vector3 position = Child.position;
		Quaternion rotation = Child.rotation;
		Parent.position = position;
		Parent.rotation = rotation;
	}

	[ContextMenu("FixMe2")]
	public void FixMe2()
	{
		Child.position = Parent.position;
		Child.rotation = Parent.rotation;
	}
}
