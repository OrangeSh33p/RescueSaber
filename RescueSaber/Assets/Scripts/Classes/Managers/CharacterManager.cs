using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterManager : MonoSingleton<CharacterManager> {
	[Header("Balancing")]
	public int StartingAmountOfCharacters;
	public List<string> names;

	[Header("References")]
	public Character characterPrefab;
	public CharacterIcon characterIconPrefab;

	//Reosurces
	public List<Material> materials = new List<Material>();

	//Storage
	private GameManager gm { get { return GameManager.Instance; } }
		private Bus bus { get { return gm.bus; } }
		private UIManager ui { get { return gm.uIManager; } }


	//--------------------
	// BASIC METHODS
	//--------------------

	void Start () {
		LoadResources();
		foreach (Character c in Character.characters) c.Remove();
		for (int i=0;i<StartingAmountOfCharacters;i++) Add();
	}


	//--------------------
	// LOADING RESOURCES
	//--------------------

	void LoadResources () {
		LoadMaterials();
	}

	void LoadMaterials () {
        materials = Resources.LoadAll<Material>("Character_materials").ToList();
	}


	//--------------------
	// ADD CHARACTERS
	//--------------------

	public void Add () {
		if (Character.characters.Count >= 2 * bus.amountOfRanks - 1) {
			ui.Log("Your bus is full !");
		} else {
			Character chara = Instantiate (
				characterPrefab,
				Vector3.zero,
				Quaternion.identity,
				GameManager.Instance.bus.characterHolder.transform
			).GetComponent<Character>();

			CharacterIcon charaIcon = Instantiate (
				characterIconPrefab,
				Vector3.zero,
				Quaternion.identity,
				GameManager.Instance.uIManager.sideMenu.transform
			).GetComponent<CharacterIcon>();

			chara.icon = charaIcon;
			Material color = materials[Random.Range(0, materials.Count)];
			chara.SetColor(color);
			chara.icon.icon.color = color.color;
			chara.name = names[Random.Range(0,names.Count)];
		}
	}
}