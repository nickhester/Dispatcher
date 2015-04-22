using UnityEngine;
using System.Collections;

public class Structure : MonoBehaviour {

	protected Neighborhood m_neighborhood;
	protected Path m_path;

	public void Initialize(Path _p)
	{
		m_path = _p;
	}

	public Neighborhood GetNeighborhood()
	{
		return m_neighborhood;
	}

	public void SetNeighborhood(Neighborhood _n)
	{
		m_neighborhood = _n;
	}

	public Path GetPath()
	{
		return m_path;
	}
}
