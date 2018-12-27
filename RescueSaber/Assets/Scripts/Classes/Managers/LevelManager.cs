using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelManager : MonoSingleton<LevelManager> {
	[Header("Balancing : Chunks")]
	public float destroyPosition;
	public int amountOfChunks;
	public float chunkWidth;

	[Header("Balancing : Stopovers")]
	public int minChunksBetweenStopovers;
	public int maxChunksBetweenStopovers;

	[Header("References")]
	public GameObject chunkPrefab;

	//State
	private int chunksToNextStopover;
	private Stopover nextStopover;

	//Storage
	private GameManager gm { get { return GameManager.Instance; } }

	//Resources
	private List<Stopover> stopoverPrefabs = new List<Stopover>();
	private List<Material> materials = new List<Material>();


	//--------------------
	// BASIC METHODS
	//--------------------

	void Start () {
		LoadResources();
		PrepareNextStopover();
	}


	//--------------------
	// LOADING RESOURCES
	//--------------------

	void LoadResources () { //Load stopovers and materials from game files	
		LoadMaterials();
		LoadStopovers();
	}

	void LoadStopovers () { //Load stopovers from /Exclusive_test. If none, load from /Done and /Test
		foreach (Stopover s in Resources.LoadAll<Stopover>("Stopovers/Exclusive_test")) stopoverPrefabs.Add(s);

		if (stopoverPrefabs.Count == 0) {
			foreach (Stopover s in Resources.LoadAll<Stopover>("Stopovers/Done")) stopoverPrefabs.Add(s);
			foreach (Stopover s in Resources.LoadAll<Stopover>("Stopovers/Test")) stopoverPrefabs.Add(s);
		}
	}

	void LoadMaterials () {
        materials = Resources.LoadAll<Material>("Chunk_materials").ToList();
	}


	//--------------------
	// PREPARING STOPOVERS
	//--------------------

	void PrepareNextStopover () { //Initialize next stopover's type and distance
		nextStopover = stopoverPrefabs[Random.Range(0,stopoverPrefabs.Count-1)];
		chunksToNextStopover = Random.Range(minChunksBetweenStopovers, maxChunksBetweenStopovers);
	}

	void CreateStopover (Transform parent) { //Instanciate Stopover
			Instantiate(
				nextStopover,
				parent.position,
				Quaternion.identity,
				parent);
	}


	//--------------------
	// CREATING LEVELS
	//--------------------

	public void CreateChunk (Vector3 origin) { //Create chunk at the very front (called by back chunk when destroyed)
		Chunk chunk = Instantiate( //Create chunk as a child
				chunkPrefab, 
				origin + Vector3.forward * chunkWidth * amountOfChunks, 
				Quaternion.identity, 
				transform
			).GetComponent<Chunk>();
		
		chunk.visuals.material = materials[Random.Range(0,materials.Count)]; //Assign random color to chunk

		if (chunksToNextStopover == 0) { //Create stopover in chunk if needed
			CreateStopover(chunk.transform);
			PrepareNextStopover();
			chunk.name = "Stopover";
		} else
			chunksToNextStopover --;
	}
}