// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigFlagOnItemDetect
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigFlagOnItemDetect : MonoBehaviour
  {
    [Header("Flag")]
    public string Flag;
    public int SetsToValue;
    [Header("Blocking Flag")]
    public bool HasBlockingFlag;
    public string BlockingFlag;
    public int BlockingFlagValue;
    [Header("Required Flag Flag")]
    public bool RequiresFlagForDetect;
    public string RequiredFlag;
    public int RequiredFlagValueOrAbove;
    private bool m_hasSetFlag;
    [Header("Other Stuff")]
    public List<FVRObject> ObjectsToBeDetected = new List<FVRObject>();
    private HashSet<FVRPhysicalObject> m_objsDetected = new HashSet<FVRPhysicalObject>();
    private List<string> m_IDsToBeDetected = new List<string>();
    public bool DestroysOnDetect;
    public bool PingSpawners;
    public List<ZosigSpawnFromTable> TablesToPing;
    public bool KeepsDetecting;
    public bool InstaSpawnsSomething;
    public GameObject SpawnThing;

    private void Start()
    {
      if (GM.ZMaster.FlagM.GetFlagValue(this.Flag) >= this.SetsToValue)
        this.gameObject.SetActive(false);
      for (int index = 0; index < this.ObjectsToBeDetected.Count; ++index)
        this.m_IDsToBeDetected.Add(this.ObjectsToBeDetected[index].ItemID);
    }

    public void RefreshFlagCache()
    {
      this.m_IDsToBeDetected.Clear();
      this.m_objsDetected.Clear();
      for (int index = 0; index < this.ObjectsToBeDetected.Count; ++index)
        this.m_IDsToBeDetected.Add(this.ObjectsToBeDetected[index].ItemID);
    }

    private void OnTriggerEnter(Collider col) => this.ProcessTrigger(col);

    private void ProcessTrigger(Collider col)
    {
      if (this.m_hasSetFlag && !this.KeepsDetecting || (Object) col.attachedRigidbody == (Object) null || (this.RequiresFlagForDetect && GM.ZMaster.FlagM.GetFlagValue(this.RequiredFlag) < this.RequiredFlagValueOrAbove || this.HasBlockingFlag && GM.ZMaster.FlagM.GetFlagValue(this.BlockingFlag) > this.BlockingFlagValue))
        return;
      FVRPhysicalObject component = col.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
      if ((Object) component == (Object) null || (Object) component.QuickbeltSlot != (Object) null || ((Object) component.ObjectWrapper == (Object) null || this.m_objsDetected.Contains(component)) || !this.m_IDsToBeDetected.Contains(component.ObjectWrapper.ItemID))
        return;
      this.m_objsDetected.Add(component);
      if (this.InstaSpawnsSomething)
        Object.Instantiate<GameObject>(this.SpawnThing, this.transform.position, this.transform.rotation);
      if (!this.m_hasSetFlag)
      {
        this.m_hasSetFlag = true;
        GM.ZMaster.FlagM.SetFlagMaxBlend(this.Flag, this.SetsToValue);
        if (!this.KeepsDetecting)
          this.gameObject.SetActive(false);
      }
      if (this.PingSpawners)
      {
        for (int index = 0; index < this.TablesToPing.Count; ++index)
          this.TablesToPing[index].SpawnKernel();
      }
      if (!this.DestroysOnDetect)
        return;
      Object.Destroy((Object) component.gameObject);
    }
  }
}
