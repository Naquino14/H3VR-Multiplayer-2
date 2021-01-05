// Decompiled with JetBrains decompiler
// Type: FistVR.SaveableTreeSystem
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class SaveableTreeSystem : MonoBehaviour
  {
    private Dictionary<string, GameObject> HangableDefs = new Dictionary<string, GameObject>();
    public GameObject SkyRing;

    private void Awake()
    {
      this.PrimeDics();
      this.LoadTreeFromDisk();
    }

    private void PrimeDics()
    {
      HangableDef[] hangableDefArray = Resources.LoadAll<HangableDef>("HangableDefs");
      Debug.Log((object) hangableDefArray.Length);
      for (int index = 0; index < hangableDefArray.Length; ++index)
        this.HangableDefs.Add(hangableDefArray[index].ID, hangableDefArray[index].Prefab);
    }

    public void ToggleSkyRing() => this.SkyRing.SetActive(!this.SkyRing.activeSelf);

    private void LoadTreeFromDisk()
    {
      if (!ES2.Exists("MeatmasTree.txt"))
        return;
      using (ES2Reader es2Reader = ES2Reader.Create("MeatmasTree.txt"))
      {
        List<string> stringList = new List<string>();
        List<Vector3> vector3List1 = new List<Vector3>();
        List<Vector3> vector3List2 = new List<Vector3>();
        if (es2Reader.TagExists("OrnamentList_IDs"))
          stringList = es2Reader.ReadList<string>("OrnamentList_IDs");
        if (es2Reader.TagExists("OrnamentList_Positions"))
          vector3List1 = es2Reader.ReadList<Vector3>("OrnamentList_Positions");
        if (es2Reader.TagExists("OrnamentList_Eulers"))
          vector3List2 = es2Reader.ReadList<Vector3>("OrnamentList_Eulers");
        if (stringList.Count <= 0)
          return;
        for (int index = 0; index < stringList.Count; ++index)
        {
          GameObject gameObject = Object.Instantiate<GameObject>(this.HangableDefs[stringList[index]], vector3List1[index], Quaternion.identity);
          gameObject.transform.eulerAngles = vector3List2[index];
          gameObject.GetComponent<MeatmasHangable>().SetIsKinematicLocked(true);
        }
      }
    }

    public void SaveTreeToDisk()
    {
      using (ES2Writer es2Writer = ES2Writer.Create("MeatmasTree.txt"))
      {
        MeatmasHangable[] objectsOfType = Object.FindObjectsOfType<MeatmasHangable>();
        List<string> stringList = new List<string>();
        List<Vector3> vector3List1 = new List<Vector3>();
        List<Vector3> vector3List2 = new List<Vector3>();
        for (int index = 0; index < objectsOfType.Length; ++index)
        {
          if ((double) Vector3.Distance(new Vector3(this.transform.position.x, 0.0f, this.transform.position.z), new Vector3(objectsOfType[index].transform.position.x, 0.0f, objectsOfType[index].transform.position.z)) <= 2.5 && (Object) objectsOfType[index].QuickbeltSlot == (Object) null && !objectsOfType[index].IsHeld)
          {
            stringList.Add(objectsOfType[index].Def.ID);
            vector3List1.Add(objectsOfType[index].transform.position);
            vector3List2.Add(objectsOfType[index].transform.eulerAngles);
          }
        }
        es2Writer.Write<string>(stringList, "OrnamentList_IDs");
        es2Writer.Write<Vector3>(vector3List1, "OrnamentList_Positions");
        es2Writer.Write<Vector3>(vector3List2, "OrnamentList_Eulers");
        es2Writer.Save();
      }
    }
  }
}
