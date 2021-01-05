using UnityEngine;

namespace FistVR
{
	public class ShieldBars : MonoBehaviour
	{
		public Transform FollowTarg;

		public Transform ShieldBar;

		public Cubegame Game;

		public Renderer Rend;

		private void Update()
		{
			ShieldBar.transform.localScale = new Vector3(Game.Health * 0.01f + 0.01f, 1f, 1f);
			base.transform.position = FollowTarg.position;
			base.transform.rotation = FollowTarg.rotation;
			float num = Game.Health * 0.01f;
			Rend.material.SetColor("_TintColor", new Color(1f - num, num, 0f, 1f));
		}
	}
}
