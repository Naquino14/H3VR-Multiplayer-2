using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRFireArmBeltSegment : FVRPhysicalObject
	{
		public List<FVRLoadedRound> RoundList = new List<FVRLoadedRound>();

		public List<GameObject> ProxyRounds = new List<GameObject>();

		public List<Renderer> ProxyRends = new List<Renderer>();

		public List<MeshFilter> ProxyMeshes = new List<MeshFilter>();

		private FVRFireArmBeltRemovalTrigger m_trig;

		protected override void Awake()
		{
			base.Awake();
			UpdateBulletDisplay();
		}

		public void UpdateBulletDisplay()
		{
			for (int i = 0; i < RoundList.Count; i++)
			{
				if (!ProxyRounds[i].activeSelf)
				{
					ProxyRounds[i].SetActive(value: true);
				}
				ProxyRends[i].material = RoundList[i].LR_Material;
				ProxyMeshes[i].mesh = RoundList[i].LR_Mesh;
			}
			for (int j = RoundList.Count; j < ProxyRounds.Count; j++)
			{
				if (ProxyRounds[j].activeSelf)
				{
					ProxyRounds[j].SetActive(value: false);
				}
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			if (m_trig != null && !m_trig.FireArm.HasBelt && (!m_trig.FireArm.UsesTopCover || m_trig.FireArm.IsTopCoverUp) && (m_trig.FireArm.Magazine == null || m_trig.FireArm.Magazine.IsBeltBox) && RoundList.Count > 0)
			{
				m_trig.FireArm.BeltDD.MountBeltSegment(this);
				Object.Destroy(base.gameObject);
			}
			else
			{
				base.EndInteraction(hand);
			}
		}

		public void OnTriggerEnter(Collider col)
		{
			FVRFireArmBeltRemovalTrigger component = col.gameObject.GetComponent<FVRFireArmBeltRemovalTrigger>();
			if (component != null)
			{
				m_trig = component;
			}
		}

		public void OnTriggerExit(Collider col)
		{
			FVRFireArmBeltRemovalTrigger component = col.gameObject.GetComponent<FVRFireArmBeltRemovalTrigger>();
			if (component != null && component == m_trig)
			{
				m_trig = null;
			}
		}
	}
}
