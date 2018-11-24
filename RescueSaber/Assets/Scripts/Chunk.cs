using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {
	[Header("References")]
	public MeshRenderer visuals;

	//Storage
	private GameManager gm { get { return GameManager.Instance; } }
	private LevelManager levelManager { get { return gm.levelManager; } }

	//Chunks list
	static List<Chunk> _chunks;
	public static List<Chunk> chunks { 
		get { if (_chunks == null) _chunks = new List<Chunk>(); 
			return _chunks; } }

	void Start () {
		chunks.Add(this);
	}

	void Update () {
		Move();
		CheckDestroy();
	}

	void OnDestroy () {
		chunks.Remove(this);
	}

	void Move () {
		transform.position += new Vector3 (0, 0, -gm.bus.currentSpeed * Time.deltaTime);
	}

	void CheckDestroy() {
		if (transform.position.z < levelManager.destroyPosition) {
			levelManager.CreateChunk(transform.position);
			Destroy(gameObject);
		}
	}
}
