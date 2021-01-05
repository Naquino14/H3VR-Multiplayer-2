// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.PostProcessVolume
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
  [ExecuteInEditMode]
  [AddComponentMenu("Rendering/Post-process Volume", -1)]
  public sealed class PostProcessVolume : MonoBehaviour
  {
    public PostProcessProfile sharedProfile;
    [Tooltip("A global volume is applied to the whole scene.")]
    public bool isGlobal;
    [Min(0.0f)]
    [Tooltip("Outer distance to start blending from. A value of 0 means no blending and the volume overrides will be applied immediatly upon entry.")]
    public float blendDistance;
    [Range(0.0f, 1f)]
    [Tooltip("Total weight of this volume in the scene. 0 means it won't do anything, 1 means full effect.")]
    public float weight = 1f;
    [Tooltip("Volume priority in the stack. Higher number means higher priority. Negative values are supported.")]
    public float priority;
    private int m_PreviousLayer;
    private float m_PreviousPriority;
    private List<Collider> m_TempColliders;
    private PostProcessProfile m_InternalProfile;

    public PostProcessProfile profile
    {
      get
      {
        if ((UnityEngine.Object) this.m_InternalProfile == (UnityEngine.Object) null)
        {
          this.m_InternalProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
          if ((UnityEngine.Object) this.sharedProfile != (UnityEngine.Object) null)
          {
            foreach (PostProcessEffectSettings setting in this.sharedProfile.settings)
              this.m_InternalProfile.settings.Add(UnityEngine.Object.Instantiate<PostProcessEffectSettings>(setting));
          }
        }
        return this.m_InternalProfile;
      }
      set => this.m_InternalProfile = value;
    }

    internal PostProcessProfile profileRef => (UnityEngine.Object) this.m_InternalProfile == (UnityEngine.Object) null ? this.sharedProfile : this.m_InternalProfile;

    private void OnEnable()
    {
      PostProcessManager.instance.Register(this);
      this.m_PreviousLayer = this.gameObject.layer;
      this.m_TempColliders = new List<Collider>();
    }

    private void OnDisable() => PostProcessManager.instance.Unregister(this);

    private void Update()
    {
      int layer = this.gameObject.layer;
      if (layer != this.m_PreviousLayer)
      {
        PostProcessManager.instance.UpdateVolumeLayer(this, this.m_PreviousLayer, layer);
        this.m_PreviousLayer = layer;
      }
      if ((double) this.priority == (double) this.m_PreviousPriority)
        return;
      PostProcessManager.instance.SetLayerDirty(layer);
      this.m_PreviousPriority = this.priority;
    }

    private void OnDrawGizmos()
    {
      List<Collider> tempColliders = this.m_TempColliders;
      this.GetComponents<Collider>(tempColliders);
      if (this.isGlobal || tempColliders == null)
        return;
      Vector3 localScale = this.transform.localScale;
      Vector3 vector3 = new Vector3(1f / localScale.x, 1f / localScale.y, 1f / localScale.z);
      Gizmos.matrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, localScale);
      foreach (Collider collider in tempColliders)
      {
        if (collider.enabled)
        {
          System.Type type = collider.GetType();
          if (type == typeof (BoxCollider))
          {
            BoxCollider boxCollider = (BoxCollider) collider;
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size + vector3 * this.blendDistance * 4f);
          }
          else if (type == typeof (SphereCollider))
          {
            SphereCollider sphereCollider = (SphereCollider) collider;
            Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
            Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius + (float) ((double) vector3.x * (double) this.blendDistance * 2.0));
          }
          else if (type == typeof (MeshCollider))
          {
            MeshCollider meshCollider = (MeshCollider) collider;
            if (!meshCollider.convex)
              meshCollider.convex = true;
            Gizmos.DrawMesh(meshCollider.sharedMesh);
            Gizmos.DrawWireMesh(meshCollider.sharedMesh, Vector3.zero, Quaternion.identity, Vector3.one + vector3 * this.blendDistance * 4f);
          }
        }
      }
      tempColliders.Clear();
    }
  }
}
