using UnityEngine;

[ExecuteInEditMode]
public class HolographicSight : MonoBehaviour
{
	public Transform VirtualQuad;

	public float Scale = 1f;

	public bool SizeCompensation = true;

	public Camera Camera;

	private MaterialPropertyBlock m_block;

	private void OnEnable()
	{
		m_block = new MaterialPropertyBlock();
		GetComponent<Renderer>().SetPropertyBlock(m_block);
	}

	private void OnWillRenderObject()
	{
		Vector3 vector = base.transform.InverseTransformPoint(VirtualQuad.transform.position);
		m_block.SetVector("_Offset", vector);
		m_block.SetFloat("_Scale", Scale);
		m_block.SetFloat("_SizeCompensation", (!SizeCompensation) ? 0f : 1f);
		GetComponent<Renderer>().SetPropertyBlock(m_block);
	}
}
