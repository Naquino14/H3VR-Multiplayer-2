using UnityEngine;

namespace FistVR
{
	public class ZosigRIsen : MonoBehaviour
	{
		private float speed;

		private void Start()
		{
		}

		private void Update()
		{
			if (speed < 3f)
			{
				speed += Time.deltaTime * 0.25f;
			}
			else if (speed < 50f)
			{
				speed += Time.deltaTime;
			}
			base.transform.Translate(Vector3.up * speed * Time.deltaTime);
			if (base.transform.position.y > 600f)
			{
				Object.Destroy(base.gameObject);
				GM.ZMaster.FlagM.SetFlag("lj_free", 2);
			}
		}
	}
}
