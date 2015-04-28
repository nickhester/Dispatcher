using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class officerMenu : panelMenu {

	private Depot theDepot;
	private float headSpacingHorizontal = 200.0f;
	private float headScale = 1.0f;
	private List<GameObject> heads = new List<GameObject>();
	public GameObject OfficerProfileUI;

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
			GameObject instance = SpawnObjectWithSprite(OfficerProfileUI, officer.GetMySprite(), 1);
			instance.transform.position = SpaceOutElements(theDepot.GetAllOfficers().Count, i, headSpacingHorizontal);
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
