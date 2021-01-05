// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.MeshUtilities
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
  internal static class MeshUtilities
  {
    private static Dictionary<PrimitiveType, Mesh> s_Primitives = new Dictionary<PrimitiveType, Mesh>();
    private static Dictionary<System.Type, PrimitiveType> s_ColliderPrimitives = new Dictionary<System.Type, PrimitiveType>()
    {
      {
        typeof (BoxCollider),
        PrimitiveType.Cube
      },
      {
        typeof (SphereCollider),
        PrimitiveType.Sphere
      },
      {
        typeof (CapsuleCollider),
        PrimitiveType.Capsule
      }
    };

    internal static Mesh GetColliderMesh(Collider collider)
    {
      System.Type type = collider.GetType();
      return type == typeof (MeshCollider) ? ((MeshCollider) collider).sharedMesh : MeshUtilities.GetPrimitive(MeshUtilities.s_ColliderPrimitives[type]);
    }

    internal static Mesh GetPrimitive(PrimitiveType primitiveType)
    {
      Mesh builtinMesh;
      if (!MeshUtilities.s_Primitives.TryGetValue(primitiveType, out builtinMesh))
      {
        builtinMesh = MeshUtilities.GetBuiltinMesh(primitiveType);
        MeshUtilities.s_Primitives.Add(primitiveType, builtinMesh);
      }
      return builtinMesh;
    }

    private static Mesh GetBuiltinMesh(PrimitiveType primitiveType)
    {
      GameObject primitive = GameObject.CreatePrimitive(primitiveType);
      Mesh sharedMesh = primitive.GetComponent<MeshFilter>().sharedMesh;
      RuntimeUtilities.Destroy((UnityEngine.Object) primitive);
      return sharedMesh;
    }
  }
}
