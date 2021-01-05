using UnityEngine;

[ExecuteInEditMode]
public class TreeEditScript : MonoBehaviour
{
	public Vector2 SizeY;

	public Vector2 SizeXZ;

	private void Update()
	{
		Scaler();
	}

	private void Scaler()
	{
		float num = Random.Range(SizeXZ.x, SizeXZ.y);
		float y = Random.Range(SizeY.x, SizeY.y);
		float y2 = Random.Range(0f, 360f);
		base.transform.localScale = new Vector3(num, y, num);
		base.transform.localEulerAngles = new Vector3(0f, y2, 0f);
	}
}
