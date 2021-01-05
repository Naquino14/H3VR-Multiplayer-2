// Decompiled with JetBrains decompiler
// Type: DinoFracture.TriggerExplosionOnCollision
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Threading;
using UnityEngine;

namespace DinoFracture
{
  public class TriggerExplosionOnCollision : MonoBehaviour
  {
    public FractureGeometry[] Explosives;
    public float Force;
    public float Radius;

    private void OnCollisionEnter(Collision col)
    {
      AsyncFractureResult[] asyncFractureResultArray = new AsyncFractureResult[this.Explosives.Length];
      for (int index = 0; index < this.Explosives.Length; ++index)
      {
        if ((Object) this.Explosives[index] != (Object) null && this.Explosives[index].gameObject.activeSelf)
          asyncFractureResultArray[index] = this.Explosives[index].Fracture();
      }
      for (int index = 0; index < asyncFractureResultArray.Length; ++index)
      {
        if (asyncFractureResultArray[index] != null)
        {
          while (!asyncFractureResultArray[index].IsComplete)
            Thread.Sleep(1);
          this.Explode(asyncFractureResultArray[index].PiecesRoot, asyncFractureResultArray[index].EntireMeshBounds);
        }
      }
    }

    private void Explode(GameObject root, Bounds bounds)
    {
      Vector3 explosionPosition = root.transform.localToWorldMatrix.MultiplyPoint(bounds.center);
      Transform transform = root.transform;
      for (int index = 0; index < transform.childCount; ++index)
      {
        Rigidbody component = transform.GetChild(index).GetComponent<Rigidbody>();
        if ((Object) component != (Object) null)
          component.AddExplosionForce(this.Force, explosionPosition, this.Radius);
      }
    }
  }
}
