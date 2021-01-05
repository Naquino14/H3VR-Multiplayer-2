using UnityEngine;

namespace FistVR
{
	public class ColTester : MonoBehaviour
	{
		private void OnCollisionEnter(Collision col)
		{
			for (int i = 0; i < col.contacts.Length; i++)
			{
				Debug.Log(col.contacts[i].thisCollider.transform.gameObject.name);
			}
		}
	}
}
