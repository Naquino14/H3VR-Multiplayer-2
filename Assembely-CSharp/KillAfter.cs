using UnityEngine;

public class KillAfter : MonoBehaviour
{
	public float DieTime;

	private void Start()
	{
		Invoke("KillMe", DieTime);
	}

	private void KillMe()
	{
		Object.Destroy(base.gameObject);
	}
}
