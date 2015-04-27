using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class officerMenu : panelMenu {

	private Depot theDepot;
	private float headSpacingHorizontal = 75.0f;
	private float headScale = 0.4f;
	private float distVertical = 250.0f;
	private List<GameObject> heads = new List<GameObject>();

	void Awake()
	{
		theDepot = GameObject.Find("Depot").GetComponent<Depot>();
		theDepot.OnGenerateOfficer += UpdateNumOfficers;
	}

	void UpdateNumOfficers()
	{
		ClearExistingHeads();

		for (int i = 0; i < theDepot.GetAllOfficers().Count; i++)
		{
			Officer officer = theDepot.GetAllOfficers()[i];
			GameObject instance = SpawnObjectWithSprite(officer.GetMyHead(), officer.GetMySprite(), 0);
			instance.transform.position = (-instance.transform.up * distVertical) + SpaceOutElements (theDepot.GetAllOfficers().Count, i, headSpacingHorizontal);
			instance.GetComponent<RectTransform>().localScale = new Vector3(headScale, headScale);
			instance.transform.SetParent(transform, false);
			heads.Add (instance);
		}
	}

	void ClearExistingHeads()
	{
		foreach (GameObject head in heads)
		{
			Destroy(head);
		}
		heads.Clear();
	}
}
