using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager> {
    [Header("References")]
    public Bus bus;
    public Stopover stopover;

    [Header("Manager")]
    public CameraManager cameraManager;
    public CharacterManager characterManager;
    public DialogueManager dialogueManager;
    public FoodManager foodManager;
    public LevelManager levelManager;
    public StatsManager statsManager;
    public StopoverManager stopoverManager;
    public TimeManager timeManager;
    public UIManager uIManager;


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
