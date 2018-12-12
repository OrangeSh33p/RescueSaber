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
	private List<Stopover> stopoverPrefabs = new List<Stopover>();
	private List<Material> materials = new List<Material>();


	//BASIC METHODS
	void Start () {
		LoadResources();
		PrepareNextStopover();
	}

	void LoadResources () { //Load stopovers and materials from game files
		foreach (Stopover s in Resources.LoadAll<Stopover>("Stopovers")) 
			if (s.isEnabled) stopoverPrefabs.Add(s);

        materials = Resources.LoadAll<Material>("Chunk_materials").ToList();
	}


	//STOPOVERS
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

	//CHUNKS
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