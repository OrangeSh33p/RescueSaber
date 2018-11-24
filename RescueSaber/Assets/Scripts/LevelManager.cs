using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	private Stopover[] stopoverPrefabs;
	private Material[] materials;


	//BASIC METHODS
	void Start () {
		LoadResources();
		PrepareNextStopover();
	}

	void LoadResources () { //Load stopovers and materials from game files
        stopoverPrefabs = Resources.LoadAll<Stopover>("Stopovers"); 
        materials = Resources.LoadAll<Material>("Chunk_materials");
	}


	//STOPOVERS
	void PrepareNextStopover () { //Initialize next stopover's type and distance
		nextStopover = stopoverPrefabs[Random.Range(0,stopoverPrefabs.Length-1)];
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
		
		chunk.visuals.material = materials[Random.Range(0,materials.Length)]; //Assign random color to chunk

		if (chunksToNextStopover == 0) { //Create stopover in chunk if needed
			CreateStopover(chunk.transform);
			PrepareNextStopover();
		} else {
			chunksToNextStopover --;
		}
	}
}
