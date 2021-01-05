// Decompiled with JetBrains decompiler
// Type: FistVR.GenericCleanupController
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class GenericCleanupController : MonoBehaviour
  {
    public GameObject DestructibleTargets;
    private Vector3 m_storedPos = Vector3.zero;
    private GameObject SpawnedTargets;

    public void Awake()
    {
      this.DestructibleTargets.SetActive(false);
      this.m_storedPos = this.DestructibleTargets.transform.position;
      this.SpawnedTargets = Object.Instantiate<GameObject>(this.DestructibleTargets, this.m_storedPos, Quaternion.identity);
      this.SpawnedTargets.SetActive(true);
    }

    public void DeleteMagazines()
    {
      FVRFireArmMagazine[] objectsOfType1 = Object.FindObjectsOfType<FVRFireArmMagazine>();
      for (int index = objectsOfType1.Length - 1; index >= 0; --index)
      {
        if (!objectsOfType1[index].IsHeld && (Object) objectsOfType1[index].QuickbeltSlot == (Object) null && (Object) objectsOfType1[index].FireArm == (Object) null)
          Object.Destroy((Object) objectsOfType1[index].gameObject);
      }
      FVRFireArmRound[] objectsOfType2 = Object.FindObjectsOfType<FVRFireArmRound>();
      for (int index = objectsOfType2.Length - 1; index >= 0; --index)
      {
        if (!objectsOfType2[index].IsHeld && (Object) objectsOfType2[index].QuickbeltSlot == (Object) null)
          Object.Destroy((Object) objectsOfType2[index].gameObject);
      }
    }

    public void ResetTargetJunk()
    {
      Object.Destroy((Object) this.SpawnedTargets);
      ShatterablePhysicalObject[] objectsOfType = Object.FindObjectsOfType<ShatterablePhysicalObject>();
      for (int index = objectsOfType.Length - 1; index >= 0; --index)
        Object.Destroy((Object) objectsOfType[index].gameObject);
      foreach (ReactiveSteelTarget reactiveSteelTarget in Object.FindObjectsOfType<ReactiveSteelTarget>())
        reactiveSteelTarget.ClearHoles();
      this.SpawnedTargets = Object.Instantiate<GameObject>(this.DestructibleTargets, this.m_storedPos, Quaternion.identity);
      this.SpawnedTargets.SetActive(true);
    }
  }
}
