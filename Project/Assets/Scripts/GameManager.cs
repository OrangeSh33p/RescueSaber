using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager> {
    public Bus bus;
    public CameraManager cameraManager;
    public ChunkManager chunkManager;
    public UIManager uIManager;
    public Stopover stopover;
}
