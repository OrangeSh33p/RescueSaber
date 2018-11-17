using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {
	public Material[] materials;
	public GameObject ground;

	private GameManager gm { get { return GameManager.Instance; } }
	private ChunkManager chunkManager { get { return gm.chunkManager; } }

	void Start () {
		ground.GetComponent<MeshRenderer>().material = materials[Random.Range(0,materials.Length)];
		chunkManager.chunks.Add(this);
	}

	void Update () {
		if (transform.position.z < -30) {
			chunkManager.CreateChunk(transform.position);
			Destroy(gameObject);
		}
	}

	void OnDestroy () {
		chunkManager.chunks.Remove(this);
	}
}
