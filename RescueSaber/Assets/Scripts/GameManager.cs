using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager> {
    [Header("References")]
    public Bus bus;
    public CameraManager cameraManager;
    public LevelManager levelManager;
    public UIManager uIManager;
    public Stopover stopover;
    public FoodManager foodManager;
    public TimeManager timeManager;


    public void Defeat () {
        float rawDays = Time.timeSinceLevelLoad/timeManager.dayDuration +0.04f;
        int days = (int)rawDays;
        int hours = Mathf.FloorToInt((rawDays - Mathf.Floor(rawDays)) * 24f);

        string gameOverText = "All your crew is dead. Congratulations, you have survived for "+days+" day";
        gameOverText += days==1?"":"s";
        gameOverText +=  " and "+hours+" hour";
        gameOverText += hours==1?"":"s";
        gameOverText +=  " in the wasteland.";
        uIManager.LogPermanent(gameOverText);

        bus.maxSpeed = 0;
    }
}
