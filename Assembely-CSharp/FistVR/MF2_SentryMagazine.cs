using UnityEngine;

namespace FistVR
{
	public class MF2_SentryMagazine : FVRPhysicalObject
	{
		public AudioEvent AudEvent_MagLoad;

		public override void OnCollisionEnter(Collision col)
		{
			base.OnCollisionEnter(col);
			AutoMeaterHitZone component = col.collider.gameObject.GetComponent<AutoMeaterHitZone>();
			if (component != null && component.IsSpecificMagazine && !component.M.FireControl.Firearms[component.FirearmIndex].HasMag)
			{
				SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_MagLoad, base.transform.position);
				component.M.FireControl.Firearms[component.FirearmIndex].Load();
				Object.Destroy(base.gameObject);
			}
		}
	}
}
