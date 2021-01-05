// Decompiled with JetBrains decompiler
// Type: CynJsonExample
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class CynJsonExample : MonoBehaviour
{
  public string rootFolderName = "Gameplanner";
  public string catFolderName = "Scenarios";
  public string subFolderName = "TestLevel";
  public string baseFileName = "MyFile";
  public SBTeamDef teamDef;
  public SBSavedRange savedRange;

  [ContextMenu("Save")]
  private void Save()
  {
    string errorMessage;
    CynJson.Save(this.rootFolderName, this.catFolderName, this.subFolderName, this.baseFileName + "_TeamDef.json", (object) this.teamDef, out errorMessage);
    MonoBehaviour.print((object) errorMessage);
    CynJson.Save(this.rootFolderName, this.catFolderName, this.subFolderName, this.baseFileName + "_SavedRange.json", (object) this.savedRange, out errorMessage);
    MonoBehaviour.print((object) errorMessage);
  }

  [ContextMenu("Load")]
  private void Load()
  {
    string errorMessage;
    CynJson.Load<SBTeamDef>(this.rootFolderName, this.catFolderName, this.subFolderName, this.baseFileName + "_TeamDef.json", this.teamDef, out errorMessage);
    MonoBehaviour.print((object) errorMessage);
    CynJson.Load<SBSavedRange>(this.rootFolderName, this.catFolderName, this.subFolderName, this.baseFileName + "_SavedRange.json", this.savedRange, out errorMessage);
    MonoBehaviour.print((object) errorMessage);
  }

  [ContextMenu("Print")]
  private void PrintPaths()
  {
    foreach (object file in CynJson.GetFiles(this.rootFolderName, this.catFolderName, this.subFolderName, "_TeamDef.json"))
      MonoBehaviour.print(file);
  }
}
