using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseUI : MonoBehaviour {

	protected Vector3 SpaceOutElements(int _numElementsTotal, int _elementIndex, float _horizontalSpacing)
	{
		Vector3 loc = Vector3.up;
		float spacing = (_horizontalSpacing * _elementIndex) - ((_horizontalSpacing * ((float)_numElementsTotal - 1)) / 2.0f);
		loc += Vector3.right * spacing;
		return loc;
	}

}
