using UnityEngine;

namespace FistVR
{
	public class FVRIgnitable : MonoBehaviour
	{
		public FXM.FireFXType FireType;

		public bool HasDamageAble;

		public IFVRDamageable Dam;

		public float IgnitionThreshold = 1f;

		private ParticleSystem m_fireInstance;

		public bool m_hasFireInstance;

		[Header("Damage Stuff")]
		public bool DamagesSelf = true;

		public float Dam_Thermal;

		public float Dam_Frequency = 0.25f;

		public float Dam_Radius = 0.25f;

		private float m_frequencyTick = 0.25f;

		private Damage dam;

		public bool UsesTransformOverride;

		public Transform TransformOverride;

		[Header("LightFlash Stuff")]
		public bool UsesLightFlash;

		public Color LightFlashColor;

		public Vector2 LightFlashIntensityRange = new Vector2(0.5f, 2f);

		public Vector2 LightFlashRadiusRange = new Vector2(0.5f, 2f);

		private float tick = 0.1f;

		public void Start()
		{
			IFVRDamageable component = GetComponent<IFVRDamageable>();
			if (component != null)
			{
				Dam = component;
				HasDamageAble = true;
			}
			dam = new Damage();
			tick = Random.Range(0.02f, 0.1f);
		}

		public bool IsIgniteable()
		{
			if (m_hasFireInstance)
			{
				return false;
			}
			return true;
		}

		public Transform GetSpawnPos()
		{
			if (UsesTransformOverride)
			{
				return TransformOverride;
			}
			return base.transform;
		}

		public bool IsOnFire()
		{
			return m_hasFireInstance;
		}

		public void Ignite(ParticleSystem p)
		{
			m_fireInstance = p;
			m_hasFireInstance = true;
		}

		private void Update()
		{
			if (!m_hasFireInstance)
			{
				return;
			}
			if (UsesLightFlash)
			{
				if (tick > 0f)
				{
					tick -= Time.deltaTime;
				}
				else
				{
					tick = Random.Range(0.02f, 0.25f);
					float num = 1f;
					if (GM.CurrentSceneSettings.IsSceneLowLight)
					{
						num = 2.5f;
					}
					FXM.InitiateMuzzleFlashLowPriority(base.transform.position, Vector3.up, Random.Range(LightFlashIntensityRange.x, LightFlashIntensityRange.y) * num, LightFlashColor, Random.Range(LightFlashRadiusRange.x, LightFlashRadiusRange.y) * num);
				}
			}
			if (m_frequencyTick > 0f)
			{
				m_frequencyTick -= Time.deltaTime;
			}
			else if (DamagesSelf)
			{
				m_frequencyTick = Dam_Frequency * Random.Range(1f, 1.2f);
				if (HasDamageAble)
				{
					dam.Dam_Thermal = Dam_Thermal * Random.Range(0.5f, 1f);
					dam.Dam_TotalEnergetic = dam.Dam_Thermal;
					dam.strikeDir = Random.onUnitSphere;
					dam.hitNormal = -dam.strikeDir;
					dam.point = dam.hitNormal * Dam_Radius;
					dam.Class = Damage.DamageClass.Abstract;
					Dam.Damage(dam);
				}
			}
			if (m_fireInstance.particleCount < 1)
			{
				Object.Destroy(m_fireInstance);
				m_hasFireInstance = false;
			}
		}

		private void OnDestroy()
		{
			Dam = null;
			HasDamageAble = false;
		}

		public void OnParticleCollision(GameObject other)
		{
			if (!m_hasFireInstance)
			{
				if (other.CompareTag("IgnitorSystem"))
				{
					FXM.Ignite(this, 1f);
				}
			}
			else if (DamagesSelf && HasDamageAble && other.CompareTag("IgnitorSystem"))
			{
				dam.Dam_Thermal = Dam_Thermal * Random.Range(0.25f, 1f);
				dam.Dam_TotalEnergetic = dam.Dam_Thermal;
				dam.strikeDir = Random.onUnitSphere;
				dam.hitNormal = -dam.strikeDir;
				dam.point = dam.hitNormal * Dam_Radius;
				dam.Class = Damage.DamageClass.Explosive;
				Dam.Damage(dam);
			}
		}
	}
}
