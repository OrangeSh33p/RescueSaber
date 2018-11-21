using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour {
	public GameObject chunkPrefab;
	public List<Chunk> chunks;

	private Stopover[] stopoverPrefabs;
	private GameManager gm { get { return GameManager.Instance; } }

	public void Start () {
        stopoverPrefabs = Resources.LoadAll<Stopover>("Stopovers");
	}

	public void CreateChunk (Vector3 origin) {
		Chunk chunk = Instantiate(
				chunkPrefab, 
				origin + Vector3.forward*80, 
				Quaternion.identity, 
				transform
			).GetComponent<Chunk>();

		if (gm.stopover == null && Random.value < 0.1f) {
			Stopover stopover = Instantiate(
				stopoverPrefabs[Random.Range(0,stopoverPrefabs.Length-1)],
				chunk.transform.position,
				Quaternion.identity,
				chunk.transform)
				.GetComponent<Stopover>();
		}
	}
}
