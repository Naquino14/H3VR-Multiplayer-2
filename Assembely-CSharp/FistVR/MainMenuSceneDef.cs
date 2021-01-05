using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Scene Def", menuName = "MainMenu/SceneDefinition", order = 0)]
	public class MainMenuSceneDef : ScriptableObject
	{
		public string Name;

		public string Type;

		[Multiline(10)]
		public string Desciption;

		public Sprite Image;

		public string SceneName;
	}
}
