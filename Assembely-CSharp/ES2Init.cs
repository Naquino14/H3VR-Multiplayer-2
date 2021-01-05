using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

[ExecuteInEditMode]
public class ES2Init : MonoBehaviour
{
	public void Awake()
	{
		Init();
	}

	public void Start()
	{
		if (Application.isEditor)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public static void Init()
	{
		ES2TypeManager.types = new Dictionary<Type, ES2Type>();
		ES2TypeManager.types[typeof(Vector2)] = new ES2_Vector2();
		ES2TypeManager.types[typeof(Vector3)] = new ES2_Vector3();
		ES2TypeManager.types[typeof(Vector4)] = new ES2_Vector4();
		ES2TypeManager.types[typeof(Texture2D)] = new ES2_Texture2D();
		ES2TypeManager.types[typeof(Quaternion)] = new ES2_Quaternion();
		ES2TypeManager.types[typeof(Mesh)] = new ES2_Mesh();
		ES2TypeManager.types[typeof(Color)] = new ES2_Color();
		ES2TypeManager.types[typeof(Color32)] = new ES2_Color32();
		ES2TypeManager.types[typeof(Material)] = new ES2_Material();
		ES2TypeManager.types[typeof(Rect)] = new ES2_Rect();
		ES2TypeManager.types[typeof(Bounds)] = new ES2_Bounds();
		ES2TypeManager.types[typeof(Transform)] = new ES2_Transform();
		ES2TypeManager.types[typeof(BoxCollider)] = new ES2_BoxCollider();
		ES2TypeManager.types[typeof(CapsuleCollider)] = new ES2_CapsuleCollider();
		ES2TypeManager.types[typeof(SphereCollider)] = new ES2_SphereCollider();
		ES2TypeManager.types[typeof(MeshCollider)] = new ES2_MeshCollider();
		ES2TypeManager.types[typeof(bool)] = new ES2_bool();
		ES2TypeManager.types[typeof(byte)] = new ES2_byte();
		ES2TypeManager.types[typeof(char)] = new ES2_char();
		ES2TypeManager.types[typeof(decimal)] = new ES2_decimal();
		ES2TypeManager.types[typeof(double)] = new ES2_double();
		ES2TypeManager.types[typeof(float)] = new ES2_float();
		ES2TypeManager.types[typeof(int)] = new ES2_int();
		ES2TypeManager.types[typeof(long)] = new ES2_long();
		ES2TypeManager.types[typeof(short)] = new ES2_short();
		ES2TypeManager.types[typeof(string)] = new ES2_string();
		ES2TypeManager.types[typeof(uint)] = new ES2_uint();
		ES2TypeManager.types[typeof(ulong)] = new ES2_ulong();
		ES2TypeManager.types[typeof(ushort)] = new ES2_ushort();
		ES2TypeManager.types[typeof(Enum)] = new ES2_Enum();
		ES2TypeManager.types[typeof(Matrix4x4)] = new ES2_Matrix4x4();
		ES2TypeManager.types[typeof(BoneWeight)] = new ES2_BoneWeight();
		ES2TypeManager.types[typeof(sbyte)] = new ES2_sbyte();
		ES2TypeManager.types[typeof(GradientAlphaKey)] = new ES2_GradientAlphaKey();
		ES2TypeManager.types[typeof(GradientColorKey)] = new ES2_GradientColorKey();
		ES2TypeManager.types[typeof(Gradient)] = new ES2_Gradient();
		ES2TypeManager.types[typeof(Sprite)] = new ES2_Sprite();
		ES2TypeManager.types[typeof(ES2AutoSaveManager)] = new ES2_ES2AutoSaveManager();
		ES2TypeManager.types[typeof(DateTime)] = new ES2_DateTime();
		ES2TypeManager.types[typeof(PolygonCollider2D)] = new ES2_PolygonCollider2D();
		ES2TypeManager.types[typeof(object)] = new ES2_object();
		ES2TypeManager.types[typeof(Texture)] = new ES2_Texture();
		ES2TypeManager.types[typeof(OmniScore)] = new ES2UserType_FistVROmniScore();
		ES2TypeManager.types[typeof(OmniScoreList)] = new ES2UserType_FistVROmniScoreList();
		ES2TypeManager.types[typeof(SavedGunComponent)] = new ES2UserType_FistVRSavedGunComponent();
		ES2TypeManager.types[typeof(AudioClip)] = new ES2_AudioClip();
		ES2TypeManager.types[typeof(SavedGun)] = new ES2UserType_FistVRSavedGun();
		ES2.initialised = true;
	}
}
