using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class OmniGridCell : MonoBehaviour, IFVRDamageable
	{
		public OmniGrid Grid;

		public Text CellText;

		public Image CellImage;

		public Color DefaultImageColor;

		public Color ErrorImageColor;

		private int m_cellNumber;

		private bool m_canBeShot;

		public void SetCanBeShot(bool b)
		{
			m_canBeShot = b;
		}

		public void SetState(int i, string s)
		{
			CellText.text = s;
			m_cellNumber = i;
			CellImage.color = DefaultImageColor;
		}

		public void Damage(Damage dam)
		{
			if (dam.Class == FistVR.Damage.DamageClass.Projectile && m_canBeShot && !Grid.InputNumber(m_cellNumber, this))
			{
				CellImage.color = ErrorImageColor;
			}
		}
	}
}
