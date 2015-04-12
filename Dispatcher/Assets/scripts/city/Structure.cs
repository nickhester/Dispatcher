using UnityEngine;
using System.Collections;

public class Structure : MonoBehaviour {


	private Neighborhood m_neighborhood;

	public Neighborhood GetNeighborhood()
	{
		return m_neighborhood;
	}

	public void SetNeighborhood(Neighborhood _n)
	{
		m_neighborhood = _n;
	}
}
