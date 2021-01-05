// Decompiled with JetBrains decompiler
// Type: FistVR.MainMenuSceneDef
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
