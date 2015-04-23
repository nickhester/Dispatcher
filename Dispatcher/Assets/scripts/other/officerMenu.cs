using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class officerMenu : panelMenu {

	public Depot theDepot;
	private List<Officer> officer = new List<Officer>();

	// Use this for initialization
	void Start ()
	{
		officer = theDepot.GetAllOfficers();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
