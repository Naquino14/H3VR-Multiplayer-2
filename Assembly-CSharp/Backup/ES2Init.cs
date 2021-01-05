// Decompiled with JetBrains decompiler
// Type: ES2Init
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using FistVR;
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ES2Init : MonoBehaviour
{
  public void Awake() => ES2Init.Init();

  public void Start()
  {
    if (Application.isEditor)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.gameObject);
    else
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  public static void Init()
  {
    ES2TypeManager.types = new Dictionary<System.Type, ES2Type>();
    ES2TypeManager.types[typeof (Vector2)] = (ES2Type) new ES2_Vector2();
    ES2TypeManager.types[typeof (Vector3)] = (ES2Type) new ES2_Vector3();
    ES2TypeManager.types[typeof (Vector4)] = (ES2Type) new ES2_Vector4();
    ES2TypeManager.types[typeof (Texture2D)] = (ES2Type) new ES2_Texture2D();
    ES2TypeManager.types[typeof (Quaternion)] = (ES2Type) new ES2_Quaternion();
    ES2TypeManager.types[typeof (Mesh)] = (ES2Type) new ES2_Mesh();
    ES2TypeManager.types[typeof (Color)] = (ES2Type) new ES2_Color();
    ES2TypeManager.types[typeof (Color32)] = (ES2Type) new ES2_Color32();
    ES2TypeManager.types[typeof (Material)] = (ES2Type) new ES2_Material();
    ES2TypeManager.types[typeof (Rect)] = (ES2Type) new ES2_Rect();
    ES2TypeManager.types[typeof (Bounds)] = (ES2Type) new ES2_Bounds();
    ES2TypeManager.types[typeof (Transform)] = (ES2Type) new ES2_Transform();
    ES2TypeManager.types[typeof (BoxCollider)] = (ES2Type) new ES2_BoxCollider();
    ES2TypeManager.types[typeof (CapsuleCollider)] = (ES2Type) new ES2_CapsuleCollider();
    ES2TypeManager.types[typeof (SphereCollider)] = (ES2Type) new ES2_SphereCollider();
    ES2TypeManager.types[typeof (MeshCollider)] = (ES2Type) new ES2_MeshCollider();
    ES2TypeManager.types[typeof (bool)] = (ES2Type) new ES2_bool();
    ES2TypeManager.types[typeof (byte)] = (ES2Type) new ES2_byte();
    ES2TypeManager.types[typeof (char)] = (ES2Type) new ES2_char();
    ES2TypeManager.types[typeof (Decimal)] = (ES2Type) new ES2_decimal();
    ES2TypeManager.types[typeof (double)] = (ES2Type) new ES2_double();
    ES2TypeManager.types[typeof (float)] = (ES2Type) new ES2_float();
    ES2TypeManager.types[typeof (int)] = (ES2Type) new ES2_int();
    ES2TypeManager.types[typeof (long)] = (ES2Type) new ES2_long();
    ES2TypeManager.types[typeof (short)] = (ES2Type) new ES2_short();
    ES2TypeManager.types[typeof (string)] = (ES2Type) new ES2_string();
    ES2TypeManager.types[typeof (uint)] = (ES2Type) new ES2_uint();
    ES2TypeManager.types[typeof (ulong)] = (ES2Type) new ES2_ulong();
    ES2TypeManager.types[typeof (ushort)] = (ES2Type) new ES2_ushort();
    ES2TypeManager.types[typeof (Enum)] = (ES2Type) new ES2_Enum();
    ES2TypeManager.types[typeof (Matrix4x4)] = (ES2Type) new ES2_Matrix4x4();
    ES2TypeManager.types[typeof (BoneWeight)] = (ES2Type) new ES2_BoneWeight();
    ES2TypeManager.types[typeof (sbyte)] = (ES2Type) new ES2_sbyte();
    ES2TypeManager.types[typeof (GradientAlphaKey)] = (ES2Type) new ES2_GradientAlphaKey();
    ES2TypeManager.types[typeof (GradientColorKey)] = (ES2Type) new ES2_GradientColorKey();
    ES2TypeManager.types[typeof (Gradient)] = (ES2Type) new ES2_Gradient();
    ES2TypeManager.types[typeof (Sprite)] = (ES2Type) new ES2_Sprite();
    ES2TypeManager.types[typeof (ES2AutoSaveManager)] = (ES2Type) new ES2_ES2AutoSaveManager();
    ES2TypeManager.types[typeof (DateTime)] = (ES2Type) new ES2_DateTime();
    ES2TypeManager.types[typeof (PolygonCollider2D)] = (ES2Type) new ES2_PolygonCollider2D();
    ES2TypeManager.types[typeof (object)] = (ES2Type) new ES2_object();
    ES2TypeManager.types[typeof (Texture)] = (ES2Type) new ES2_Texture();
    ES2TypeManager.types[typeof (OmniScore)] = (ES2Type) new ES2UserType_FistVROmniScore();
    ES2TypeManager.types[typeof (OmniScoreList)] = (ES2Type) new ES2UserType_FistVROmniScoreList();
    ES2TypeManager.types[typeof (SavedGunComponent)] = (ES2Type) new ES2UserType_FistVRSavedGunComponent();
    ES2TypeManager.types[typeof (AudioClip)] = (ES2Type) new ES2_AudioClip();
    ES2TypeManager.types[typeof (SavedGun)] = (ES2Type) new ES2UserType_FistVRSavedGun();
    ES2.initialised = true;
  }
}
