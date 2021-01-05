// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.TargetHitEffect
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class TargetHitEffect : MonoBehaviour
  {
    public Collider targetCollider;
    public GameObject spawnObjectOnCollision;
    public bool colorSpawnedObject = true;
    public bool destroyOnTargetCollision = true;

    private void OnCollisionEnter(Collision collision)
    {
      if (!((Object) collision.collider == (Object) this.targetCollider))
        return;
      ContactPoint contact = collision.contacts[0];
      float num = 1f;
      Ray ray = new Ray(contact.point - -contact.normal * num, -contact.normal);
      RaycastHit hitInfo;
      if (collision.collider.Raycast(ray, out hitInfo, 2f) && this.colorSpawnedObject)
      {
        Color color = ((Texture2D) collision.gameObject.GetComponent<Renderer>().material.mainTexture).GetPixelBilinear(hitInfo.textureCoord.x, hitInfo.textureCoord.y);
        if ((double) color.r > 0.699999988079071 && (double) color.g > 0.699999988079071 && (double) color.b < 0.699999988079071)
          color = Color.yellow;
        else if ((double) Mathf.Max(color.r, color.g, color.b) == (double) color.r)
          color = Color.red;
        else
          color = (double) Mathf.Max(color.r, color.g, color.b) != (double) color.g ? Color.yellow : Color.green;
        color *= 15f;
        GameObject gameObject = Object.Instantiate<GameObject>(this.spawnObjectOnCollision);
        gameObject.transform.position = contact.point;
        gameObject.transform.forward = ray.direction;
        foreach (Renderer componentsInChild in gameObject.GetComponentsInChildren<Renderer>())
        {
          componentsInChild.material.color = color;
          if (componentsInChild.material.HasProperty("_EmissionColor"))
            componentsInChild.material.SetColor("_EmissionColor", color);
        }
      }
      Debug.DrawRay(ray.origin, ray.direction, Color.cyan, 5f, true);
      if (!this.destroyOnTargetCollision)
        return;
      Object.Destroy((Object) this.gameObject);
    }
  }
}
