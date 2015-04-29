using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class officerMenu : panelMenu {

	private Depot theDepot;
	private float headSpacingHorizontal = 200.0f;
	private float headScale = 1.0f;
	private List<GameObject> profiles = new List<GameObject>();
	private List<Officer> officersWithProfiles = new List<Officer>();
	public GameObject OfficerProfileUI;
	public Sprite[] badgeSprites;

	void Awake()
	{
		theDepot = GameObject.Find("Depot").GetComponent<Depot>();
		theDepot.OnGenerateOfficer += UpdateNumOfficers;
	}

	void Update ()
	{
		for (int i = 0; i < profiles.Count; i++)
		{
			UpdateBadgesFromProfile(profiles[i],
                officersWithProfiles[i].GetCrimeLevel(types.CrimeType.Robbery),
                officersWithProfiles[i].GetCrimeLevel(types.CrimeType.Vandalism),
                officersWithProfiles[i].GetCrimeLevel(types.CrimeType.Violence));
		}
	}

	void UpdateNumOfficers()
	{
		ClearExistingHeads();
		officersWithProfiles.Clear();

		for (int i = 0; i < theDepot.GetAllOfficers().Count; i++)
		{
			Officer officer = theDepot.GetAllOfficers()[i];
			GameObject instance = SpawnObjectWithSprite(OfficerProfileUI, officer.GetMySprite(), "UI_headOfficer");
			instance.transform.position = SpaceOutElements(theDepot.GetAllOfficers().Count, i, headSpacingHorizontal);
			instance.GetComponent<RectTransform>().localScale = new Vector3(headScale, headScale);
			instance.transform.SetParent(transform, false);
			/*
			UpdateBadgesFromProfile(instance,
	            officer.GetCrimeLevel(types.CrimeType.Robbery),
	            officer.GetCrimeLevel(types.CrimeType.Vandalism),
	          	officer.GetCrimeLevel(types.CrimeType.Violence));
	          	*/
			profiles.Add (instance);
			officersWithProfiles.Add(officer);
		}
	}

	void ClearExistingHeads()
	{
		foreach (GameObject profile in profiles)
		{
			Destroy(profile);
		}
		profiles.Clear();
	}

	void UpdateBadgesFromProfile(GameObject _profile, int _robbery, int _vandalism, int _violence)
	{
		for (int i = 0; i < _profile.transform.childCount - 1; i++)
		{
			GameObject child = _profile.transform.GetChild(i).gameObject;
		
			if (child.name == "badge_robbery")
			{
				if (_robbery == 0)
				{
					child.GetComponent<Image>().enabled = false;
				}
				else
				{
					child.GetComponent<Image>().enabled = true;
					child.GetComponent<Image>().sprite = badgeSprites[-1 + _robbery];
				}
			}
			else if (child.name == "badge_vandalism")
			{
				if (_vandalism == 0)
				{
					child.GetComponent<Image>().enabled = false;
				}
				else
				{
					child.GetComponent<Image>().enabled = true;
					child.GetComponent<Image>().sprite = badgeSprites[2 + _vandalism];
				}
			}
			else if (child.name == "badge_violence")
			{
				if (_violence == 0)
				{
					child.GetComponent<Image>().enabled = false;
				}
				else
				{
					child.GetComponent<Image>().enabled = true;
					child.GetComponent<Image>().sprite = badgeSprites[5 + _violence];
				}
			}
		}
	}
}
